using System;
using System.Collections.Generic;
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

namespace MTMCL.Guide
{
    /// <summary>
    /// PageGuideAuthTry.xaml 的互動邏輯
    /// </summary>
    public partial class PageGuideAuthTry
    {
        Launch.Login.AuthInfo info;
        Launch.Login.IAuth author;
        public PageGuideAuthTry(Launch.Login.IAuth author)
        {
            InitializeComponent();
            GetInfo(this.author = author);

        }
        private async void GetInfo(Launch.Login.IAuth author) {
            info = await author.LoginAsync(System.Threading.CancellationToken.None);
            progressbar.Visibility = Visibility.Collapsed;
            if (info != null)
            {
                if (string.IsNullOrWhiteSpace(info.ErrorMsg) & info.Pass)
                {
                    lblName.Content = info.DisplayName;
                    lblUUID.Content = info.UUID;
                    butNext.IsEnabled = true;
                }
                else
                {
                    lblName.Content = Lang.LangManager.GetLangFromResource("Error");
                    lblUUID.Content = Lang.LangManager.GetLangFromResource("Error");
                }
            }
            else
            {
                lblName.Content = Lang.LangManager.GetLangFromResource("Error");
                lblUUID.Content = Lang.LangManager.GetLangFromResource("Error");
            }
        }

        private void butNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MeCore.Config.SavedAuths.Add(info.DisplayName, new Config.SavedAuth { AuthType = author.Type, DisplayName = info.DisplayName, AccessToken = info.Session.ToString(), UUID = info.UUID.ToString(), Properies = info.Prop, UserType = info.UserType });
                MeCore.Config.Save();
                NavigationService.Navigate(new Uri("Guide\\PageGuideFinish.xaml", UriKind.Relative));
            }
            catch
            {
                Logger.log("repeated username:",info.DisplayName);
            }

        }

        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
