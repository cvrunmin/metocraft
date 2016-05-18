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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTMCL.Notice
{
    /// <summary>
    /// Notice.xaml 的互動邏輯
    /// </summary>
    public partial class Notice : Grid
    {
        public Notice()
        {
            InitializeComponent();
        }
        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }
        private void Back()
        {
            MeCore.MainWindow.gridMain.Visibility = Visibility.Visible;
            MeCore.MainWindow.gridOthers.Visibility = Visibility.Collapsed;
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridMain.BeginAnimation(OpacityProperty, ani);
            MeCore.MainWindow.gridOthers.Children.Clear();
        }

        private void Grid_Initialized(object sender, EventArgs e)
        {
            putTaskBar();
            MeCore.MainWindow.OnNoticeAdded += (notice) =>
            {
                notice.Click += TaskBar_Click;
                panelNotice.Children.Add(notice);
                gridNullth.Visibility = Visibility.Collapsed;
            };
        }
        private void putTaskBar()
        {
            panelNotice.Children.Clear();
            foreach (var item in MeCore.MainWindow.noticelist)
            {
                item.Click += TaskBar_Click;
                panelNotice.Children.Add(item);
            }
            if (MeCore.MainWindow.noticelist.Count == 0) gridNullth.Visibility = Visibility.Visible;
        }
        private void TaskBar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CrashErrorBar)
            {
                MeCore.MainWindow.gridOthers.Children.Clear();
                MeCore.MainWindow.gridOthers.Children.Add(new CrashDetail(this, ((CrashErrorBar)sender)));
                var ani = new DoubleAnimationUsingKeyFrames();
                ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
                ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
                MeCore.MainWindow.gridOthers.BeginAnimation(OpacityProperty, ani);
            }
            else if (sender is InheritMissingBar)
            {
                MeCore.MainWindow.gridOthers.Children.Clear();
                MeCore.MainWindow.gridOthers.Children.Add(new InheritMissingDetail(this, ((InheritMissingBar)sender)));
                var ani = new DoubleAnimationUsingKeyFrames();
                ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
                ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
                MeCore.MainWindow.gridOthers.BeginAnimation(OpacityProperty, ani);
            }
        }

        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {
            panelNotice.Children.Clear();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            putTaskBar();
        }
    }
}
