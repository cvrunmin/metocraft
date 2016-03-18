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

namespace MTMCL.Guide
{
    /// <summary>
    /// GuideWindow.xaml 的互動邏輯
    /// </summary>
    public partial class GuideWindow
    {
        public GuideWindow(Uri Child)
        {
            InitializeComponent();
            frameGuide.Navigate(Child);
        }
    }
}
