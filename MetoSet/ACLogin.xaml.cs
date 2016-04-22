using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MTMCL
{
    /// <summary>
    /// ACLogin.xaml 的互動邏輯
    /// </summary>
    public partial class ACLogin : MetroWindow
    {
        public string UN;
        public string PW;
        public Launch.Login.IAuth auth;
        public Launch.Login.AuthInfo info;
        public readonly bool requiredPreLogin;
        public ACLogin(bool requiredPreLogin = false)
        {
            InitializeComponent();
            this.requiredPreLogin = requiredPreLogin;
        }

        private void butOffline_Click(object sender, RoutedEventArgs e)
        {
            gridMenu.Visibility = Visibility.Collapsed;
            gridOff.Visibility = Visibility.Visible;
        }

        private void butMojang_Click(object sender, RoutedEventArgs e)
        {
            gridMenu.Visibility = Visibility.Collapsed;
            gridMojang.Visibility = Visibility.Visible;
        }

        private void butBackOff_Click(object sender, RoutedEventArgs e)
        {
            gridOff.Visibility = Visibility.Collapsed;
            gridMenu.Visibility = Visibility.Visible;
        }

        private void butLoginOff_Click(object sender, RoutedEventArgs e)
        {
            auth = new Launch.Login.OfflineAuth(txtBoxUN.Text);
            if (requiredPreLogin)
            {
                info = auth.Login();
                //auth = !string.IsNullOrWhiteSpace(info.ErrorMsg) ? null : new KMCCC.Authentication.WarpedAuhenticator(new KMCCC.Authentication.AuthenticationInfo { DisplayName = info.DisplayName, AccessToken = info.AccessToken, UUID = info.UUID, UserType = info.UserType, Properties = info.Properties });
            }
            if ((bool)butRM1.IsChecked)
            {
                MeCore.Config.username = txtBoxUN.Text;
                MeCore.Config.Save(null);
            }
            Close();
        }

        private void butBackM_Click(object sender, RoutedEventArgs e)
        {
            gridMojang.Visibility = Visibility.Collapsed;
            gridMenu.Visibility = Visibility.Visible;
        }

        private void butLoginM_Click(object sender, RoutedEventArgs e)
        {
            auth = new Launch.Login.YggdrasilLoginAuth(txtBoxUNE.Text, pwbox.Password);
            if (requiredPreLogin)
            {
                info = auth.Login();
                auth = !string.IsNullOrWhiteSpace(info.ErrorMsg) ? null : new Launch.Login.YggdrasilRefreshAuth(info.Session.ToString("N"));
            }
            if ((bool)butRM2.IsChecked)
            {
                MeCore.Config.username = txtBoxUNE.Text;
                MeCore.Config.Save(null);
            }
            else
            {
                MeCore.Config.QuickChange("username", "");
            }
            if ((bool)butRPW.IsChecked)
            {
                MeCore.Config.ScoreStringToOver50(pwbox.Password);
                MeCore.Config.Save(null);
            }
            else
            {
                MeCore.Config.QuickChange("password", "");
            }
            Close();
        }

        private void butClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void CreateCustomAuth() {
            if (MeCore.Config.Server.Auths.Count != 0)
            {
                short i = 0;
                foreach (var item in MeCore.Config.Server.Auths)
                {
                    ++i;
                    Grid gridCustomAuth = new Grid();
                    gridCustomAuth.Visibility = Visibility.Collapsed;
                    Label lbltitie = new Label();
                    lbltitie.Content = Lang.LangManager.GetLangFromResource("Login");
                    lbltitie.FontSize = 21.333;
                    lbltitie.Margin = new Thickness(10, 10, 0, 0);
                    lbltitie.HorizontalAlignment = HorizontalAlignment.Left;
                    lbltitie.VerticalAlignment = VerticalAlignment.Top;
                    gridCustomAuth.Children.Add(lbltitie);
                    Label lblune = new Label();
                    lblune.Content = Lang.LangManager.GetLangFromResource("UNEM");
                    lblune.FontSize = 16;
                    lblune.Margin = new Thickness(10, 45, 0, 0);
                    lblune.HorizontalAlignment = HorizontalAlignment.Left;
                    lblune.VerticalAlignment = VerticalAlignment.Top;
                    gridCustomAuth.Children.Add(lblune);
                    TextBox txtboxune = new TextBox();
                    txtboxune.Text = "";
                    txtboxune.FontSize = 18.667;
                    txtboxune.Margin = new Thickness(10, 75, 10, 0);
                    txtboxune.Height = 30;
                    txtboxune.HorizontalAlignment = HorizontalAlignment.Stretch;
                    txtboxune.VerticalAlignment = VerticalAlignment.Top;
                    txtboxune.TextWrapping = TextWrapping.Wrap;
                    gridCustomAuth.Children.Add(txtboxune);
                    Label lblpw = new Label();
                    lblpw.Content = Lang.LangManager.GetLangFromResource("PW");
                    lblpw.FontSize = 16;
                    lblpw.Margin = new Thickness(10, 105, 0, 0);
                    lblpw.HorizontalAlignment = HorizontalAlignment.Left;
                    lblpw.VerticalAlignment = VerticalAlignment.Top;
                    gridCustomAuth.Children.Add(lblpw);
                    PasswordBox pwbox = new PasswordBox();
                    pwbox.Password = "";
                    pwbox.FontSize = 18.667;
                    pwbox.Margin = new Thickness(10, 135, 10, 0);
                    pwbox.Height = 30;
                    pwbox.HorizontalAlignment = HorizontalAlignment.Stretch;
                    pwbox.VerticalAlignment = VerticalAlignment.Top;
                    gridCustomAuth.Children.Add(pwbox);
                    Button butback = new Button();
                    butback.Content = Lang.LangManager.GetLangFromResource("Back");
                    butback.FontSize = 16;
                    butback.Margin = new Thickness(0, 0, 10, 10);
                    butback.Width = 202; butback.Height = 32;
                    butback.HorizontalAlignment = HorizontalAlignment.Right;
                    butback.VerticalAlignment = VerticalAlignment.Bottom;
                    butback.BorderThickness = new Thickness(2);
                    butback.Style = (Style)Resources["NormalButton"];
                    butback.Click += delegate (object sender, RoutedEventArgs e) {
                        gridCustomAuth.Visibility = Visibility.Collapsed;
                        gridMenu.Visibility = Visibility.Visible;
                    };
                    gridCustomAuth.Children.Add(butback);
                    Button butlogin = new Button();
                    butlogin.Content = Lang.LangManager.GetLangFromResource("Login");
                    butlogin.FontSize = 16;
                    butlogin.Margin = new Thickness(0, 0, 217, 10);
                    butlogin.Width = 202; butlogin.Height = 32;
                    butlogin.HorizontalAlignment = HorizontalAlignment.Right;
                    butlogin.VerticalAlignment = VerticalAlignment.Bottom;
                    butlogin.BorderThickness = new Thickness(2);
                    butlogin.Style = (Style)Resources["NormalButton"];
                    butlogin.Click += delegate (object sender, RoutedEventArgs e) {
                        auth = new Launch.Login.YggdrasilLoginAuth(txtboxune.Text, pwbox.Password,new Launch.Login.YggdrasilHelper(item.Url));
                        if (requiredPreLogin)
                        {
                            info = auth.Login();
                            //auth = !string.IsNullOrWhiteSpace(info.Error) ? null : new KMCCC.Authentication.YggdrasilRefresh(info.AccessToken, false, item.Url);
                        }
                        Close();
                    };
                    gridCustomAuth.Children.Add(butlogin);
                    gridMain.Children.Add(gridCustomAuth);
                    ButtonMenu butcustomauth = new ButtonMenu();
                    butcustomauth.Content = string.Format(Lang.LangManager.GetLangFromResource("Online"), item.Name);
                    butcustomauth.MenuImage = new BitmapImage(new Uri("pack://application:,,,/Resources/others.png"));
                    butcustomauth.FontSize = 16;
                    butcustomauth.Margin = new Thickness(0, 60 * (i + 1), -17, 0);
                    butcustomauth.Height = 60;
                    butcustomauth.Background = new SolidColorBrush(Colors.White);
                    butcustomauth.VerticalAlignment = VerticalAlignment.Top;
                    butcustomauth.Style = (Style)Resources["ButtonMeListSolid"];
                    butcustomauth.ListType = true;
                    butcustomauth.Click += delegate (object sender, RoutedEventArgs e) {
                        gridMenu.Visibility = Visibility.Collapsed;
                        gridCustomAuth.Visibility = Visibility.Visible;
                    };
                    gridscrolllist.Children.Add(butcustomauth);
                }
            }
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MeCore.IsServerDedicated)
            {
                CreateCustomAuth();
            }
            if (!string.IsNullOrWhiteSpace(MeCore.Config.username))
            {
                txtBoxUN.Text = MeCore.Config.username;
                txtBoxUNE.Text = MeCore.Config.username;
            }
            if (!string.IsNullOrWhiteSpace(MeCore.Config.GetOver50String()))
            {
                pwbox.Password = MeCore.Config.GetOver50String();
            }
        }
    }
}
