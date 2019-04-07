using System;
using System.Xml;
using IcyWind.Chat.Jid;

namespace IcyWind.Chat.Presence
{
    public class PresenceManager
    {
        public ChatPresence Presence { get; private set; }
        internal ChatClient ChatClient { get; }

        /// <summary>
        /// Got when a presence has been recieved.
        /// </summary>
        /// <param name="pres">The presence</param>
        public delegate void OnPresence(ChatPresence pres);
        public event OnPresence OnPlayerPresenceRecieved;

        internal PresenceManager(ChatClient client)
        {
            ChatClient = client;
        }


        /// <summary>
        /// Lets you change the XMPP Presence
        /// </summary>
        /// <param name="status">Your status</param>
        /// <param name="pres">The presence</param>
        /// <param name="show">The show</param>
        public void SetPresence(string status, PresenceType pres, PresenceShow show)
        {
            Presence = new ChatPresence
            {
                FromJid = ChatClient.MainJid,
                LastOnline = DateTime.Now.ToString("dd-MM-yyyy"),
                PresenceShow = show,
                PresenceType = pres,
                Status = status
            };

            var encodedXml = System.Security.SecurityElement.Escape(status);

            ChatClient.TcpClient.SendString(PresenceAsString(status, pres, show));
        }

        public string PresenceAsString(ChatPresence pres)
        {
            return PresenceAsString(pres.Status, pres.PresenceType, pres.PresenceShow);
        }

        /// <summary>
        /// Lets you change the XMPP Presence
        /// </summary>
        /// <param name="status">Your status</param>
        /// <param name="pres">The presence</param>
        /// <param name="show">The show</param>
        /// <returns>The output presence</returns>
        public string PresenceAsString(string status, PresenceType pres, PresenceShow show)
        {
            var encodedXml = System.Security.SecurityElement.Escape(status);
            return $"<presence type=\"{ConvertPresenceType.ConvertPresenceTypeToString(pres)}\">" +
                   "<priority>0</priority>" +
                   $"<show>{ConvertPresenceShow.ConvertPresenceShowToString(show)}</show>" +
                   $"<status>{encodedXml}</status>" +
                   "</presence>";
        }

        internal bool HandleReceivedPresence(XmlElement el)
        {
            try
            {
                var pres = ReadPresence(el);
                //If presence is from self, set this as player's presence
                if (pres.FromJid == pres.ToJid)
                {
                    Presence = pres;
                    //For this project we want to lock the presenece to mobile
                    Presence.PresenceShow = PresenceShow.Mobile;
                }

                if (OnPlayerPresenceRecieved != null)
                {
                    OnPlayerPresenceRecieved(pres);
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private ChatPresence ReadPresence(XmlElement el)
        {
            //Create the new presence
            var pres = new ChatPresence
            {
                FromJid = new UserJid(el.Attributes["from"].Value),
                ToJid = new UserJid(el.Attributes["to"].Value)
            };
            try
            {
                //Get the presence type
                pres.PresenceType =
                    (PresenceType)Enum.Parse(typeof(PresenceType), el.Attributes["type"].Value, true);
            }
            catch
            {
                //Ignored
            }

            //Handle more presence data
            foreach (var presData in el.ChildNodes)
            {
                var xmlPres = (XmlNode)presData;
                switch (xmlPres.Name)
                {
                    case "show":
                        pres.PresenceShow = (PresenceShow)Enum.Parse(typeof(PresenceShow),
                            xmlPres.InnerText, true);
                        break;
                    case "status":
                        pres.Status = System.Web.HttpUtility.HtmlDecode(xmlPres.InnerText);
                        break;
                    case "last_online":
                        pres.LastOnline = xmlPres.InnerText;
                        break;
                    default:
                        //TODO: Log this
                        break;
                }
            }

            return pres;
        }
    }
}
