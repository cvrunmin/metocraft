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

namespace MetocraftWpf
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void butMenu_Click(object sender, RoutedEventArgs e)
        {
            if (girdMenu.Margin.Left == -150) {
                var mover = new ThicknessAnimationUsingKeyFrames();
                mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(-150, 0, 0, 0), TimeSpan.FromSeconds(0)));
                mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(-150 + 100, 0, 0, 0), TimeSpan.FromSeconds(0.2)));
                mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0.4)));
                var mover1 = new ThicknessAnimationUsingKeyFrames();
                mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0)));
                mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0 + 100, 0, 0, 0), TimeSpan.FromSeconds(0.2)));
                mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(150, 0, 0, 0), TimeSpan.FromSeconds(0.4)));
                girdMenu.BeginAnimation(FrameworkElement.MarginProperty, mover);
                girdMain.BeginAnimation(FrameworkElement.MarginProperty, mover1);
            }
            else if (girdMenu.Margin.Left == 0)
            {
                var mover = new ThicknessAnimationUsingKeyFrames();
                mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0)));
                mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0 - 100, 0, 0, 0), TimeSpan.FromSeconds(0.2)));
                mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(-150, 0, 0, 0), TimeSpan.FromSeconds(0.4)));
                var mover1 = new ThicknessAnimationUsingKeyFrames();
                mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(150, 0, 0, 0), TimeSpan.FromSeconds(0)));
                mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(150 - 100, 0, 0, 0), TimeSpan.FromSeconds(0.2)));
                mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0.4)));
                girdMenu.BeginAnimation(FrameworkElement.MarginProperty, mover);
                girdMain.BeginAnimation(FrameworkElement.MarginProperty, mover1);
            }
        }

        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
            PlayWin.PlayWin win = new PlayWin.PlayWin();
            ((Window)win).Show();
        }
    }
}
