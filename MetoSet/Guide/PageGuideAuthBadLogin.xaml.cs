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
    /// PageGuideAuthBadLogin.xaml 的互動邏輯
    /// </summary>
    public partial class PageGuideAuthBadLogin : Page
    {
        public PageGuideAuthBadLogin()
        {
            InitializeComponent();
        }
        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void butNext_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageGuideAuthTry(new KMCCC.Authentication.OfflineAuthenticator(txtUN.Text)));
        }
    }
}
