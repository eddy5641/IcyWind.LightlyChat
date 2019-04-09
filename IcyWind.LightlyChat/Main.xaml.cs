using IcyWind.Chat;
using IcyWind.Chat.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace IcyWind.LightlyChat
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Page
    {
        public Main(ChatClient chat, AuthCred cred)
        {
            InitializeComponent();

            _ = chat.ConnectSSL("pvp.net", cred);
            chat.PresenceManager.OnPlayerPresenceRecieved += PresenceManager_OnPlayerPresenceRecieved;
            chat.IqManager.OnRosterItemRecieved += IqManager_OnRosterItemRecieved;
            chat.MessageManager.OnMessage += MessageManager_OnMessage;

        }

        private void IqManager_OnRosterItemRecieved(IcyWind.Chat.Jid.UserJid jid)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                if (string.IsNullOrWhiteSpace(jid.SumName))
                    fakeFriends.Items.Add(jid.RawJid);
                else
                    fakeFriends.Items.Add(jid.SumName);

            }));
        }

        private void MessageManager_OnMessage(IcyWind.Chat.Jid.UserJid toJid, IcyWind.Chat.Jid.UserJid fromJid, string msg)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                MessageBox.Show(msg, fromJid.PlayerJid);
            }));
        }

        private void PresenceManager_OnPlayerPresenceRecieved(IcyWind.Chat.Presence.ChatPresence pres)
        {
            Debugger.Log(0, "", JsonConvert.SerializeObject(pres) + Environment.NewLine);
            return;
#pragma warning disable CS0162 // Unreachable code detected
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
#pragma warning restore CS0162 // Unreachable code detected
            {
                if (string.IsNullOrWhiteSpace(pres.FromJid.SumName))
                    fakeFriends.Items.Add(pres.FromJid.RawJid);
                else
                    fakeFriends.Items.Add(pres.FromJid.SumName);
            }));
        }
    }
}
