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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTMCL.Help
{
    /// <summary>
    /// PageHttpError.xaml 的互動邏輯
    /// </summary>
    public partial class PageHttpError : Page
    {
        public delegate void Retry();
        public event Retry OnRetry;
        public PageHttpError()
        {
            InitializeComponent();
        }
        public PageHttpError SetStatus(string s)
        {
            lblStatus.Content = s;
            return this;
        }
        private void butRetry_Click(object sender, RoutedEventArgs e)
        {
            OnRetry?.Invoke();
        }
    }
}
