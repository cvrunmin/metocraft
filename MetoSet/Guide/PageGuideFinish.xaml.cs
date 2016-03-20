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
    /// PageGuideAuthCheck.xaml 的互動邏輯
    /// </summary>
    public partial class PageGuideFinish
    {
        public PageGuideFinish()
        {
            InitializeComponent();
        }

        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void butOK_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            NavigationService.Navigate(new Uri("Guide\\PageGuideEnd.xaml", UriKind.Relative));
        }
    }
}
