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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTMCL.Guide
{
    /// <summary>
    /// GridGuide.xaml 的互動邏輯
    /// </summary>
    public partial class GridGuide
    {
        public GridGuide(Uri Child)
        {
            InitializeComponent();
            frameGuide.Navigate(Child);
        }
        private void frameGuide_Navigated(object sender, NavigationEventArgs e)
        {
            var ani = new ThicknessAnimation(new Thickness(10, 0, -10, 0), new Thickness(0), TimeSpan.FromSeconds(0.25));
            var ani1 = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
            VisualStateManager.GoToState(this, "AfterLoaded", true);
            frameGuide.BeginAnimation(MarginProperty, ani);
            frameGuide.BeginAnimation(OpacityProperty, ani1);
            if (e.Uri != null)
            {
                if (e.Uri.OriginalString.Equals("Guide/PageGuideEnd.xaml"))
                {
                    Back();
                }
            }
        }
        private void Back()
        {
            MeCore.MainWindow.gridMain.Visibility = Visibility.Visible;
            MeCore.MainWindow.gridOthers.Visibility = Visibility.Collapsed;
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridMain.BeginAnimation(OpacityProperty, ani);
            VisualStateManager.GoToState(this, "AfterLoaded", true);
        }
    }
}
