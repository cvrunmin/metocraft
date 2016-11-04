using MTMCL.Versions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTMCL
{
    /// <summary>
    /// PlayNew.xaml 的互動邏輯
    /// </summary>
    public partial class PlayNew : Grid
    {
        public VersionJson[] versions;
        public PlayNew()
        {
            InitializeComponent();
        }
        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }
        private void Back()
        {
            MeCore.MainWindow.gridMain.Visibility = Visibility.Visible;
            MeCore.MainWindow.gridOthers.Visibility = Visibility.Collapsed;
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridMain.BeginAnimation(OpacityProperty, ani);
        }

        private void Grid_Initialized(object sender, EventArgs e)
        {
            string path = MeCore.Config.MCPath;
            if (MeCore.IsServerDedicated)
            {
                if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                {
                    path = path.Replace(MeCore.Config.MCPath, System.IO.Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                }
            }
            if (!string.IsNullOrWhiteSpace(path))
            {
                versions = VersionReader.GetVersion(MeCore.Config.MCPath);
                listVer.ItemsSource = versions;
            }
            if (versions == null)
                gridNoVersion.Visibility = Visibility.Visible;
            else if (versions.Length == 0)
                gridNoVersion.Visibility = Visibility.Visible;
            else
                gridNoVersion.Visibility = Visibility.Collapsed;
        }
        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
                if (((Button)sender).DataContext is VersionJson)
                    Play((VersionJson)((Button)sender).DataContext);
        }
        private void butSetting_Click(object sender, EventArgs e) {
            if (sender is Button)
                if (((Button)sender).DataContext is VersionJson) {
                    MeCore.MainWindow.gridOthers.Children.Clear();
                    MeCore.MainWindow.gridOthers.Children.Add(new PlaySetting(this, (VersionJson)((Button)sender).DataContext));
                    var ani = new DoubleAnimationUsingKeyFrames();
                    ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
                    ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
                    MeCore.MainWindow.gridOthers.BeginAnimation(OpacityProperty, ani);
                }
        }
        private void Play(VersionJson json)
        {
            Launch.Login.IAuth auth;
            if (string.IsNullOrWhiteSpace(MeCore.Config.DefaultAuth))
            {
                ACSelect ac = new ACSelect();
                ac.ShowDialog();
                auth = ac.auth;
            }
            else
            {
                SavedAuth dauth;
                MeCore.Config.SavedAuths.TryGetValue(MeCore.Config.DefaultAuth, out dauth);
                if (dauth == null)
                {
                    ACSelect ac = new ACSelect();
                    ac.ShowDialog();
                    auth = ac.auth;
                }
                else auth = dauth.AuthType.Equals("Yggdrasil") ? new Launch.Login.YggdrasilRefreshAuth(dauth.AccessToken) : new Launch.Login.AuthWarpper(new Launch.Login.AuthInfo { DisplayName = MeCore.Config.DefaultAuth, Session = dauth.AccessToken, UUID = dauth.UUID, UserType = dauth.UserType, Prop = dauth.Properies }) as Launch.Login.IAuth;
            }
            /*ACLogin ac = new ACLogin();
            ac.ShowDialog();
            auth = ac.auth;*/
            if (auth == null)
            {
                return;
            }
            MeCore.MainWindow._LaunchOptions = Launch.LaunchGameInfo.CreateInfo(MeCore.Config.MCPath, auth, json, MeCore.Config.Javaw, (int)MeCore.Config.Javaxmx, CreateServerInfo());
            MeCore.MainWindow.launchFlyout.IsOpen = true;
            Back();
        }

        private Launch.ServerInfo CreateServerInfo()
        {
            if (MeCore.IsServerDedicated)
            {
                if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ServerIP))
                {
                    if (MeCore.Config.Server.ServerIP.IndexOf(':') != -1)
                    {
                        ushort port = 25565;
                        if (!ushort.TryParse(MeCore.Config.Server.ServerIP.Substring(MeCore.Config.Server.ServerIP.IndexOf(':')).Trim(':'), out port))
                        {
                            port = 25565;
                        }
                        return new Launch.ServerInfo
                        {
                            Ip = MeCore.Config.Server.ServerIP.Substring(0, MeCore.Config.Server.ServerIP.IndexOf(':')).Trim(':'),
                            Port = port
                        };
                    }
                }
            }
            return null;
        }
        private async void butGoDLMC_Click (object sender, RoutedEventArgs e)
        {
            await MeCore.MainWindow.ChangePage("download");
        }

        private async void butGoSetting_Click (object sender, RoutedEventArgs e)
        {
            await MeCore.MainWindow.ChangePage("settings");
        }
    }
}
