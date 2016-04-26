using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MTMCL
{
    /// <summary>
    /// ACLogin.xaml 的互動邏輯
    /// </summary>
    public partial class ACSelect : MetroWindow
    {
        public Launch.Login.IAuth auth;
        public ACSelect()
        {
            InitializeComponent();
        }

        private void butClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void CreateCustomAuth() {
            gridscrolllist.Children.Clear();
            if (MeCore.Config.SavedAuths.Count != 0)
            {
                short i = 0;
                foreach (var item in MeCore.Config.SavedAuths)
                {
                    ButtonMenu butcustomauth = new ButtonMenu();
                    butcustomauth.Content = item.Value.DisplayName;
                    butcustomauth.MenuImage = new BitmapImage(new Uri(item.Value.AuthType.Equals("Yggdrasil") ? "Resources/mojang.png" : (item.Value.AuthType.Equals("Offline") ? "Resources/offline.png" : "Resources/others.png"), UriKind.Relative));
                    butcustomauth.FontSize = 16;
                    //butcustomauth.Margin = new Thickness(0, 60 * i, -17, 0);
                    butcustomauth.Width = 465;
                    butcustomauth.Height = 60;
                    butcustomauth.Background = new SolidColorBrush(Colors.Transparent);
                    butcustomauth.VerticalAlignment = VerticalAlignment.Top;
                    butcustomauth.Style = (Style)Resources["ButtonMeListSolid"];
                    butcustomauth.ListType = true;
                    butcustomauth.Click += AuthButton_Click;
                    gridscrolllist.Children.Add(butcustomauth);
                    i++;
                }
            }
                ButtonMenu butcreateauth = new ButtonMenu();
                butcreateauth.Name = "butCreateAuth";
                butcreateauth.Content = "Create new account";
                butcreateauth.MenuImage = new BitmapImage(new Uri("Resources/others.png", UriKind.Relative));
                butcreateauth.FontSize = 16;
                //butcustomauth.Margin = new Thickness(0, 60 * i, -17, 0);
                butcreateauth.Width = 465;
                butcreateauth.Height = 60;
                butcreateauth.Background = new SolidColorBrush(Colors.Transparent);
                butcreateauth.VerticalAlignment = VerticalAlignment.Top;
                butcreateauth.Style = (Style)Resources["ButtonMeListSolid"];
                butcreateauth.ListType = true;
                butcreateauth.Click += delegate (object sender, RoutedEventArgs e) {
                    if (e.Source is ButtonMenu)
                    {
                        ACLogin ac = new ACLogin(true);
                        ac.ShowDialog();
                        if (ac.info != null && ac.auth != null)
                        {
                            MeCore.Config.SavedAuths.Add(ac.info.DisplayName, new Config.SavedAuth { AuthType = ac.auth.Type, DisplayName = ac.info.DisplayName, AccessToken = ac.info.Session.ToString(), UUID = ac.info.UUID.ToString(), Properies = ac.info.Prop, UserType = ac.info.UserType });
                            MeCore.Config.Save();
                            CreateCustomAuth();
                        }
                    }
                };
                gridscrolllist.Children.Add(butcreateauth);
        }
        private async void AuthButton_Click(object sender, RoutedEventArgs e) {
            if (e.Source is ButtonMenu)
            {
                if ((bool)toggleDetele.IsChecked)
                {
                    var ani = new ThicknessAnimationUsingKeyFrames();
                    ani.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(-20, ((ButtonMenu)e.Source).Margin.Top, 20, ((ButtonMenu)e.Source).Margin.Bottom), TimeSpan.FromSeconds(0.2)));
                    ani.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(-100, ((ButtonMenu)e.Source).Margin.Top, 100, ((ButtonMenu)e.Source).Margin.Bottom), TimeSpan.FromSeconds(0.3)));
                    ani.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(-465, ((ButtonMenu)e.Source).Margin.Top, 465, ((ButtonMenu)e.Source).Margin.Bottom), TimeSpan.FromSeconds(0.5)));
                    ((ButtonMenu)e.Source).BeginAnimation(MarginProperty, ani);
                    foreach (ButtonMenu button in gridscrolllist.Children)
                    {
                        if (button != ((ButtonMenu)e.Source))
                        {
                            var ani1 = new ThicknessAnimationUsingKeyFrames();
                            ani1.KeyFrames.Add(new LinearThicknessKeyFrame(button.Margin, TimeSpan.FromSeconds(0.5)));
                            ani1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(button.Margin.Left, button.Margin.Top - 60, button.Margin.Right, button.Margin.Bottom + 60), TimeSpan.FromSeconds(0.8)));
                            button.BeginAnimation(MarginProperty, ani1);
                        }
                    }
                    MeCore.Config.SavedAuths.Remove(((ButtonMenu)e.Source).Content as string);
                    if (MeCore.Config.DefaultAuth.Equals(((ButtonMenu)e.Source).Content as string))
                    {
                        MeCore.Config.DefaultAuth = "";
                    }
                    MeCore.Config.Save();
                    await System.Threading.Tasks.TaskEx.Delay(2000);
                    CreateCustomAuth();
                    return;
                }
                Config.SavedAuth auth; string name = ((ButtonMenu)e.Source).Content as string;
                MeCore.Config.SavedAuths.TryGetValue(name, out auth);
                this.auth = auth.AuthType.Equals("Yggdrasil") ? new  Launch.Login.YggdrasilRefreshAuth(Guid.Parse(auth.AccessToken).ToString()) : new Launch.Login.OfflineAuth(name) as Launch.Login.IAuth;
                MeCore.Config.QuickChange("default-auth", name);
                Close();
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
                CreateCustomAuth();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in gridscrolllist.Children)
            {
                if (!((ButtonMenu)item).Name.Equals("butCreateAuth"))
                {
                    var ani = new ColorAnimation(!((bool)toggleDetele.IsChecked) ? Colors.Red : Colors.Transparent, ((bool)toggleDetele.IsChecked) ? Colors.Red : Colors.Transparent, TimeSpan.FromSeconds(0.125));
                    ((SolidColorBrush)((ButtonMenu)item).Background).BeginAnimation(SolidColorBrush.ColorProperty, ani);
                }
            }
        }
    }
}
