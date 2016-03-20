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
using System.Windows.Media.Animation;
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

        private void frameGuide_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var ani = new ThicknessAnimation(new Thickness(10, 0, -10, 0), new Thickness(0), TimeSpan.FromSeconds(0.25));
            var ani1 = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
            frameGuide.BeginAnimation(MarginProperty, ani);
            frameGuide.BeginAnimation(OpacityProperty, ani1);
            if (e.Uri != null)
            {
                if (e.Uri.OriginalString.Equals("Guide/PageGuideEnd.xaml"))
                {
                    Close();
                }
            }
        }
    }
}
