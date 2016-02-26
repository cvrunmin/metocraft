using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
            ani1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, -25, 0, 125), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15))));
            ani1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0,-100,0,200), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25))));
            var ani2 = new ThicknessAnimationUsingKeyFrames();
            ani2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, 75, 0, 25), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15))));
            ani2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, 0, 0, 100), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25))));
            butPlayQuick_a.BeginAnimation(MarginProperty, ani1);
            butPlayQuick_b.BeginAnimation(MarginProperty, ani2);
        }

        private void butPlayQuick_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ani1 = new ThicknessAnimationUsingKeyFrames();
            ani1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, -75, 0, 175), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15))));
            ani1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, 0, 0, 100), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25))));
            var ani2 = new ThicknessAnimationUsingKeyFrames();
            ani2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, 25, 0, 75), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15))));
            ani2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, 100, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25))));
            butPlayQuick_a.BeginAnimation(MarginProperty, ani1);
            butPlayQuick_b.BeginAnimation(MarginProperty, ani2);
        }

        private void butSetting_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("settings");
        }
        private async void ChangePage(string type) {
            ((Rectangle)gridLoadingScreen.Children[0]).Fill = ((Rectangle)butSetting.GetValue(ContentProperty)).Fill;
            gridLoadingScreen.Margin = new Thickness(10, 149, 430, 10);
            gridLoadingScreen.Background = new SolidColorBrush(Color.FromRgb(((SolidColorBrush)butSetting.Background).Color.R, ((SolidColorBrush)butSetting.Background).Color.G, ((SolidColorBrush)butSetting.Background).Color.B));
            gridLoadingScreen.Visibility = Visibility.Visible;
            var ani = new ThicknessAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(50), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15))));
            ani.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25))));
            gridLoadingScreen.BeginAnimation(MarginProperty, ani);
            await System.Threading.Tasks.Task.Delay(2000);
            gridOthers.Children.Clear();
            gridOthers.Children.Add(new Settings());
            gridOthers.Visibility = Visibility.Visible;
            gridMain.Visibility = Visibility.Collapsed;
            gridLoadingScreen.Visibility = Visibility.Collapsed;
        }
    }
}
