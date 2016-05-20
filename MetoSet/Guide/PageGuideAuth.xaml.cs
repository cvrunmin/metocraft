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

namespace MTMCL.Guide
{
    /// <summary>
    /// PageGuideAuth.xaml 的互動邏輯
    /// </summary>
    public partial class PageGuideAuth : Page
    {
        public PageGuideAuth()
        {
            InitializeComponent();
        }

        private void ForgotPW_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void butNext_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageGuideAuthTry(new Launch.Login.YggdrasilLoginAuth(txtEmail.Text, txtPW.Password)));
        }
    }
}
