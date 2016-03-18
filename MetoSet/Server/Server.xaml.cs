using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MTMCL.Server
{
    /// <summary>
    /// Server.xaml 的互動邏輯
    /// </summary>
    public partial class Server : Grid
    {
        public Server()
        {
            InitializeComponent();
        }
        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            MeCore.MainWindow.gridMain.Visibility = Visibility.Visible;
            MeCore.MainWindow.gridOthers.Visibility = Visibility.Collapsed;
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridMain.BeginAnimation(OpacityProperty, ani);
        }
    }
}
