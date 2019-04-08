using System;
using System.Collections.Generic;
using System.IO;
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
using zlib;
using Path = System.IO.Path;

namespace IcyWind.LightlyChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow MainWin;
        public static string MyLocation => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)?.Replace("file:\\", "");
        public MainWindow()
        {
            InitializeComponent();
            MainWin = this;
            GetLatestRad();
            UpdateView(new Login());
        }

        public static void UpdateView(Page page)
        {
            //Fucking lazest code NA
            MainWin.MainContent.Content = page.Content;
        }

        public static void GetLatestRad()
        {
            if (File.Exists(Path.Combine(MyLocation, "system.yaml")))
            {
                File.Delete(Path.Combine(MyLocation, "system.yaml"));
            }
            using (var client = new WebClient())
            {
                var LatestGCSln = client
                    .DownloadString(
                        "http://l3cdn.riotgames.com/releases/live/solutions/league_client_sln/releases/releaselisting_NA")
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None).First();
                var LatestSolutionMan = client
                    .DownloadString(
                        $"http://l3cdn.riotgames.com/releases/live/solutions/league_client_sln/releases/{LatestGCSln}/solutionmanifest")
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                //LatestSolutionMan
                var pos = Array.IndexOf(LatestSolutionMan, "league_client");
                client.DownloadFile(
                    $"http://l3cdn.riotgames.com/releases/live/projects/league_client/releases/{LatestSolutionMan[pos + 1]}/files/system.yaml.compressed",
                    Path.Combine(MyLocation, "system.yaml.compressed"));
                DecompressFile(Path.Combine(MyLocation, "system.yaml.compressed"),
                    Path.Combine(MyLocation, "system.yaml"));
                File.Delete(Path.Combine(MyLocation, "system.yaml.compressed"));
            }
        }

        public static void DecompressFile(string inFile, string outFile)
        {
            int data;
            const int stopByte = -1;
            var outFileStream = new FileStream(outFile, FileMode.Create);
            var inZStream = new ZInputStream(File.Open(inFile, FileMode.Open, FileAccess.Read));

            while (stopByte != (data = inZStream.Read()))
            {
                var databyte = (byte)data;
                outFileStream.WriteByte(databyte);
            }

            inZStream.Close();
            outFileStream.Close();
        }
    }
}
