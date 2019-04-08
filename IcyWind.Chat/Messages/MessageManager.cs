using IcyWind.Chat.Jid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace IcyWind.Chat.Messages
{
    public class MessageManager
    {
        internal ChatClient ChatClient { get; }

        public delegate void OnMessageRecieved(UserJid toJid, UserJid fromJid, string msg);
        public event OnMessageRecieved OnMessage;
        public event OnMessageRecieved OnUnknownMessage;

        internal event OnMessageRecieved OnMessageInternal;


        internal MessageManager(ChatClient client)
        {
            ChatClient = client;
        }

        public bool HandleMessage(XmlElement el)
        {
            try
            {
                var fromUserString = el.Attributes["from"].Value;
                var toUserString = el.Attributes["from"].Value;
                if (!string.IsNullOrWhiteSpace(fromUserString) && !string.IsNullOrWhiteSpace(toUserString))
                {
                    var fromJid = new UserJid(fromUserString);
                    var toJid = new UserJid(toUserString);
                    //el.InnerText is the message

                    if (toJid == ChatClient.MainJid)
                    {
                        if (OnMessage != null)
                        {
                            OnMessage(toJid, fromJid, el.InnerText);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (OnMessage != null)
                        {
                            OnMessageInternal(toJid, fromJid, el.InnerText);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Send a message to a user
        /// </summary>
        /// <param name="to">The user's Jid you want to send the message to</param>
        /// <param name="message">The message to send</param>
        public void SendMessage(UserJid to, string message)
        {
            //Set to high priority
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            var encodedXml = System.Web.HttpUtility.HtmlEncode(message);
            //Send that message
            ChatClient.TcpClient.SendString($"<message from=\'{ChatClient.MainJid.RawJid}\' to=\'{to.RawJid}\' type=\'chat\'><body>{encodedXml}</body></message>");
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
        }

        public ChatRoom JoinRoom(UserJid roomJid, string password)
        {
            if (roomJid.Type != JidType.GroupChatJid)
            {
                throw new InvalidJidTypeException("To join a room, the Jid must be a type of GroupChat");
            }

            ChatClient.TcpClient.SendString(
                $"<presence from=\'{ChatClient.MainJid.RawJid}\' to=\'{roomJid.RawJid}\'>" +
                "<x xmlns=\'http://jabber.org/protocol/muc\'>" +
                $"<password>{password}</password>" +
                "</x>" +
                "</presence>");

            return new ChatRoom(ChatClient, roomJid);
        }

        public ChatRoom JoinRoom(string roomJid, string password)
        {
            return JoinRoom(new UserJid(roomJid) { Type = JidType.GroupChatJid }, password);
        }

        public ChatRoom JoinRoom(UserJid roomJid)
        {
            if (roomJid.Type != JidType.GroupChatJid)
            {
                throw new InvalidJidTypeException("To join a room, the Jid must be a type of GroupChat");
            }

            ChatClient.TcpClient.SendString($"<presence from=\'{ChatClient.MainJid.RawJid}\' to=\'{roomJid.RawJid}\'>" +
                              "<x xmlns=\'http://jabber.org/protocol/muc\'/>" +
                              "</presence>");
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

            return new ChatRoom(ChatClient, roomJid);
        }

        public ChatRoom JoinRoom(string roomJid)
        {
            return JoinRoom(new UserJid(roomJid) { Type = JidType.GroupChatJid });
        }
    }
}
