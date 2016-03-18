using System;
using System.Windows.Controls;
using MTMCL.util;

namespace MTMCL.Guide
{
    /// <summary>
    /// PageGuideFast.xaml 的互動邏輯
    /// </summary>
    public partial class PageGuideFast : Page
    {
        public PageGuideFast()
        {
            InitializeComponent();
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            lblInfo.AddContentFromSpecficString(Lang.LangManager.GetLangFromResource("FastText"));
        }

        private void butBack_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void butUse_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Guide\\PageGuideAuth.xaml", UriKind.Relative));
        }
    }
}
