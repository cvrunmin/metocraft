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
using System.Windows.Shapes;

namespace MTMCL.Resources
{
    /// <summary>
    /// BGPreviewWindow.xaml 的互動邏輯
    /// </summary>
    public partial class BGPreviewWindow
    {
        public bool isSelected { get; private set; }
        public BGPreviewWindow(Uri uri)
        {
            InitializeComponent();
            ((ImageBrush)imgPreview.Background).ImageSource = new BitmapImage(uri);
        }

        private void butSelect_Click(object sender, RoutedEventArgs e)
        {
            isSelected = true;
            Close();
        }

        private void butClose_Click(object sender, RoutedEventArgs e)
        {
            isSelected = false;
            Close();
        }
    }
}
