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
        KMCCC.Authentication.AuthenticationInfo info;
        KMCCC.Authentication.IAuthenticator author;
        public PageGuideAuthTry(KMCCC.Authentication.IAuthenticator author)
        {
            InitializeComponent();
            GetInfo(this.author = author);

        }
        private async void GetInfo(KMCCC.Authentication.IAuthenticator author) {
            info = await author.DoAsync(System.Threading.CancellationToken.None);
            progressbar.Visibility = Visibility.Collapsed;
            if (info != null)
            {
                if (string.IsNullOrWhiteSpace(info.Error))
                {
                    lblName.Content = info.DisplayName;
                    lblUUID.Content = info.AccessToken;
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
            MeCore.Config.SavedAuths.Add(new Config.SavedAuth { AuthType = author.Type, DisplayName = info.DisplayName, UUID = info.AccessToken.ToString() });
            MeCore.Config.Save();
            NavigationService.Navigate(new Uri("Guide\\PageGuideFinish.xaml", UriKind.Relative));
        }

        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
