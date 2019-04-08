using IcyWind.Chat;
using IcyWind.Chat.Auth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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

namespace IcyWind.LightlyChat
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        RiotAuthOpenIdConfiguration config;

        public Login()
        {
            InitializeComponent();
            Region.Items.Add("BR");
            Region.Items.Add("EUNE");
            Region.Items.Add("EUW");
            Region.Items.Add("JP");
            Region.Items.Add("LA1");
            Region.Items.Add("LA2");
            Region.Items.Add("NA");
            Region.Items.Add("OC1");
            Region.Items.Add("RU");
            Region.Items.Add("TEST");
            Region.Items.Add("TR");

            config = AuthClass.GetOpenIdConfig();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var regionData = AuthClass.ReadSystemRegionData(System.IO.Path.Combine(MainWindow.MyLocation, "system.yaml"), Region.SelectedItem.ToString());
            var auth = AuthClass.GetLoginToken(user.Text, pass.Password, regionData, config);

            var servers = Dns.GetHostAddresses(regionData.Servers.Chat.ChatHost);
            var chat = new ChatClient(
                new IPEndPoint(servers.First(), regionData.Servers.Chat.ChatPort));
            var chatAuth = new AuthCred
            {
                Token = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth.AccessTokenJson.AccessToken))
            };

            MainWindow.UpdateView(new Main(chat, chatAuth));
        }
    }
}
