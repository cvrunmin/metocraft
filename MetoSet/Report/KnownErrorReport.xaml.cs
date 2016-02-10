using MTMCL.util;
using System.Windows;

namespace MTMCL
{
    /// <summary>
    /// KnownErrorReport.xaml 的互動邏輯
    /// </summary>
    public partial class KnownErrorReport : Window
    {
        public KnownErrorReport()
        {
            InitializeComponent();
        }
        public KnownErrorReport(string desc)
        {
            InitializeComponent();
            this.lblDesc.Content = desc;
        }
        public KnownErrorReport(string desc, string help)
        {
            InitializeComponent();
            lblDesc.Content = desc;
            lblHelp.AddContentFromSpecficString(help);
        }

        private void butOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
