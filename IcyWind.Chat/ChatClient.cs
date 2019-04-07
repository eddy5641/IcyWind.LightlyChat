/*
 *  IcyWind.Chat - A simple XMPP chat library
 *  Copyright (C) 2018  IcyWind Software
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *  
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using IcyWind.Chat.Auth;
using IcyWind.Chat.Iq;
using IcyWind.Chat.Jid;
using IcyWind.Chat.Messages;
using IcyWind.Chat.Presence;
using IcyWind.Chat.TcpConnection;

namespace IcyWind.Chat
{
    public class ChatClient
    {
        #region XMPPClientData
        /// <summary>
        /// Handles authentication
        /// </summary>
        internal AuthHandler AuthHandler { get; set; }

        /// <summary>
        /// Basically anything TCP this does all the fuckery
        /// </summary>
        internal TcpStringClient TcpClient { get; set; }

        /// <summary>
        /// Handles presence
        /// </summary>
        public PresenceManager PresenceManager { get; internal set; }

        /// <summary>
        /// Manages messages
        /// </summary>
        public MessageManager MessageManager { get; internal set; }

        /// <summary>
        /// Handles Iq (whatever that is)
        /// </summary>
        public IqHandler IqManager { get; internal set; }
        #endregion XMPPClientData

        #region InternalVars
        /// <summary>
        /// Idk what this does. Ask WildBook
        /// </summary>
        internal int Id { get; set; }

        /// <summary>
        /// Represents of the client is disconnecting
        /// </summary>
        internal bool Disconnecting { get; set; }

        /// <summary>
        /// Represents if authentication has happened
        /// </summary>
        internal bool HasHandledAuth { get; set; } = false;

        /// <summary>
        /// Represents authentication credentials to pass to the server
        /// </summary>
        internal AuthCred AuthCred { get; private set; }

        /// <summary>
        /// Represents the auth method that will be used (override)
        /// </summary>
        public BaseAuth AuthMethod { get; private set; }

        internal string Host { get; private set; }
        #endregion InternalVars
        
        #region Delegates
        /// <summary>
        /// Returned when the server returns a successful login
        /// </summary>
        public delegate void SuccessLogin();
        public event SuccessLogin OnSuccessLogin;
        #endregion Delegates

        #region PublicVars
        /// <summary>
        /// The current JID (Your JID)
        /// </summary>
        public UserJid MainJid { get; internal set; }
        #endregion PublicVars

        public ChatClient(IPEndPoint endPoint)
        {
            TcpClient = new TcpStringClient(endPoint, true);
        }

        public ChatClient(IPEndPoint endPoint, bool ssl)
        {
            TcpClient = new TcpStringClient(endPoint, ssl);
        }

        [Obsolete("This method does not support SSL. Consider using ConnectSSL")]
        public async Task Connect(string host, AuthCred cred)
        {
            PresenceManager = new PresenceManager(this);
            IqManager = new IqHandler(this);
            MessageManager = new MessageManager(this);

            await TcpClient.Connect(host);
            Host = host;
            TcpClient.SendString(
                $"<stream:stream to=\"{host}\" xml:lang=\"*\" version=\"1.0\" xmlns:stream=\"http://etherx.jabber.org/streams\" xmlns=\"jabber:client\">");
            AuthCred = cred;
        }

        public async Task ConnectSSL(string host, AuthCred cred)
        {
            PresenceManager = new PresenceManager(this);
            IqManager = new IqHandler(this);
            MessageManager = new MessageManager(this);

            await TcpClient.ConnectSSL(host);
            Host = host;
            TcpClient.SendString(
                $"<stream:stream to=\"{host}\" xml:lang=\"*\" version=\"1.0\" xmlns:stream=\"http://etherx.jabber.org/streams\" xmlns=\"jabber:client\">");
            AuthCred = cred;
        }

        public bool AddSASLAuth(BaseAuth authMethod)
        {
            AuthMethod = authMethod;
            return true;
        }

        /// <summary>
        /// Sends the disconnect packet
        /// </summary>
        public void Disconnect()
        {
            TcpClient.SendString("</stream:stream>");
            Disconnecting = true;
        }

        /// <summary>
        /// The handler for when the string is received
        /// </summary>
        /// <param name="x">The received string</param>
        private bool ReadXMPPMessage(string x)
        {
            if (x.Contains("</stream:stream>"))
            {
                //TODO: Handle a disconnect
            }
            //Lazy hack, this converts the initial starting output to a valid XML doc
            else if (x.Contains("<stream:stream"))
            {
                x += "</stream:stream>";
            }
            //Lazy hack 2, This stops any stupid blah blah blah is not a part of this domain stuff
            x = x.Replace("stream:", "");

            try
            {
                //The XML Reader
                using (var reader = XmlReader.Create(new StringReader(x)))
                {
                    //Convert to XmlDocument for reading
                    var doc = new XmlDocument();
                    doc.Load(reader);
                    var el = doc.DocumentElement;

                    #region XmppPresenceHandler

                    if (el.Name == "presence" && el.HasChildNodes)
                    {
                        PresenceManager.HandleReceivedPresence(el);
                    }

                    #endregion XmppPresenceHandler

                    #region XmppMessageRecieveHandler

                    else if (el.Name == "message" && el.HasChildNodes)
                    {
                        MessageManager.HandleMessage(el);
                    }

                    #endregion XmppMessageRecieveHandler

                    else if (el.HasChildNodes)
                    {
                        foreach (var node in el.ChildNodes)
                        {
                            var xmlNode = (XmlNode) node;

                            //Handle RsoAuth

                            #region AuthHandler

                            if (xmlNode.Name == "mechanisms")
                            {
                                if (AuthHandler.HandleAuth(xmlNode))
                                {
                                    break;
                                }
                            }

                            #endregion AuthHandler

                            #region ConnectionSuccessHandler

                            if (el.Name == "success" && xmlNode.Name == "#text")
                            {
                                //I have no idea why I did this, but I did it. Don't question me Wild
                                if (int.TryParse(xmlNode.InnerText, out var output))
                                    Id = output;

                                //Say we logged in okay
                                OnSuccessLogin?.Invoke();

                                //Send another random static string
                                TcpClient.SendString(
                                    "<stream:stream " +
                                    $"to=\"{Host}\" " +
                                    "xml:lang=\"*\" " +
                                    "version=\"1.0\" " +
                                    "xmlns:stream=\"http://etherx.jabber.org/streams\" " +
                                    "xmlns=\"jabber:client\">");
                            }

                            #endregion ConnectionSuccessHandler

                            #region IDontKnowWtfThisIsButReturnBinding

                            if (xmlNode.Name == "rxep")
                            {
                                //This is sent from client to server after this is received. I honestly don't know this xmpp stuff so lets pretend we read that

                                TcpClient.SendString("<iq type=\"set\" id=\"0\"><bind xmlns=\"urn:ietf:params:xml:ns:xmpp-bind\"><resource>RC</resource></bind></iq>");

                                break;
                            }

                            #endregion IDontKnowWtfThisIsButReturnBinding

                            #region IQManager
                            if (el.Name == "iq")
                            {
                                IqManager.HandleIq(xmlNode, el);
                            }
                            #endregion IQManager
                        }
                    }
                    else
                    {
                        //TODO: <?xml version='1.0'?><stream:stream xmlns='jabber:client' xmlns:stream='http://etherx.jabber.org/streams' id='3891861776' from='pvp.net' version='1.0'>
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
