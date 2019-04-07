using IcyWind.Chat.Jid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static IcyWind.Chat.Messages.MessageManager;

namespace IcyWind.Chat.Messages
{
    public class ChatRoom
    {
        public UserJid RoomJid { get; }

        private ChatClient ChatClient { get; }

        public event OnMessageRecieved OnMessage;

        internal ChatRoom(ChatClient chatClient, UserJid roomJid)
        {
            ChatClient = chatClient;
            RoomJid = roomJid;
            ChatClient.MessageManager.OnMessageInternal += MessageManager_OnMessageInternal;
        }

        private void MessageManager_OnMessageInternal(UserJid toJid, UserJid fromJid, string msg)
        {
            if (toJid == RoomJid)
            {
                OnMessage?.Invoke(toJid, fromJid, msg);
            }
        }

        public void SendRoomMessage(string message)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            var encodedXml = System.Web.HttpUtility.HtmlEncode(message);
            ChatClient.TcpClient.SendString($"<message from=\'{ChatClient.MainJid.RawJid}\' to=\'{RoomJid.RawJid}\' type=\'groupchat\'><body>{encodedXml}</body></message>");
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
        }
    }
}
