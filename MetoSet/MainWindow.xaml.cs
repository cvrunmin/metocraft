using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MTMCL
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            //MeCore.NIcon.MainWindow = this;
            MeCore.MainWindow = this;
            InitializeComponent();
            Title = "MTMCL V2 Ver." + MeCore.version;
            //Blank.main = this;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MeCore.NIcon.Hide();
        }

        private string toGoodString(int i)
        {
            string s = i.ToString();
            if (s.Length == 1) { return "0" + s; } else return s;
        }

        private void butPlayQuick_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ani1 = new ThicknessAnimationUsingKeyFrames();
            ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, -25, 0, 125), TimeSpan.FromSeconds(0.15), new QuarticEase() { EasingMode = EasingMode.EaseIn}));
            ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0,-100,0,200), TimeSpan.FromSeconds(0.25), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            var ani2 = new ThicknessAnimationUsingKeyFrames();
            ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 75, 0, 25), TimeSpan.FromSeconds(0.15), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 0, 0, 100), TimeSpan.FromSeconds(0.25), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            butPlayQuick_a.BeginAnimation(MarginProperty, ani1);
            butPlayQuick_b.BeginAnimation(MarginProperty, ani2);
        }

        private void butPlayQuick_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ani1 = new ThicknessAnimationUsingKeyFrames();
            ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, -75, 0, 175), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15)), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 0, 0, 100), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25)), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            var ani2 = new ThicknessAnimationUsingKeyFrames();
            ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 25, 0, 75), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15)), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 100, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25)), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            butPlayQuick_a.BeginAnimation(MarginProperty, ani1);
            butPlayQuick_b.BeginAnimation(MarginProperty, ani2);
        }

        private void butSetting_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("settings");
        }
        private async void ChangePage(string type) {
            MahApps.Metro.Controls.Tile tile;
            System.Windows.Controls.Grid grid;
            switch (type)
            {
                case "settings":
                    tile = butSetting;
                    grid = new Settings();
                    break;
                case "play":
                    tile = butPlay;
                    grid = new Play();
                    break;
                default:
                    return;
            }
            ((Rectangle)gridLoadingScreen.Children[0]).Fill = ((Rectangle)tile.GetValue(ContentProperty)).Fill;
            gridLoadingScreen.Margin = new Thickness(gridMenu.Margin.Left + tile.Margin.Left, gridMenu.Margin.Top + tile.Margin.Top, gridMenu.Margin.Right + (gridMenu.Width - tile.Width - tile.Margin.Left), gridMenu.Margin.Bottom + (gridMenu.Height - tile.Height - tile.Margin.Top));
            gridLoadingScreen.Background = new SolidColorBrush(Color.FromRgb(((SolidColorBrush)tile.Background).Color.R, ((SolidColorBrush)tile.Background).Color.G, ((SolidColorBrush)tile.Background).Color.B));
            gridLoadingScreen.Visibility = Visibility.Visible;
            var ani = new ThicknessAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(gridMenu.Margin.Left + tile.Margin.Left, gridMenu.Margin.Top + tile.Margin.Top, gridMenu.Margin.Right + (gridMenu.Width - tile.Width - tile.Margin.Left), gridMenu.Margin.Bottom + (gridMenu.Height - tile.Height - tile.Margin.Top)),KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            ani.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(10), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.05))));
            ani.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2))));
            gridLoadingScreen.BeginAnimation(MarginProperty, ani);
            await System.Threading.Tasks.Task.Delay(1000);
            gridOthers.Children.Clear();
            gridOthers.Children.Add(grid);
            gridOthers.Visibility = Visibility.Visible;
            gridMain.Visibility = Visibility.Collapsed;
            gridLoadingScreen.Visibility = Visibility.Collapsed;
            var ani2 = new DoubleAnimationUsingKeyFrames();
            ani2.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani2.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            gridOthers.BeginAnimation(OpacityProperty, ani2);
        }

        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("play");
        }

        private void butLaunchNormal_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ani = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1.25));
            gridBG.BeginAnimation(OpacityProperty, ani);
            gridBG.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/play-normal.jpg"))) { Stretch = Stretch.UniformToFill};
            ani = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
            gridBG.BeginAnimation(OpacityProperty, ani);
        }

        private void butLaunchNormal_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ani = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1.25));
            gridBG.BeginAnimation(OpacityProperty, ani);
            gridBG.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/bg.png"))) { Stretch = Stretch.UniformToFill };
            ani = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
            gridBG.BeginAnimation(OpacityProperty, ani);
        }
    }
}
