using MetoCraft.Lang;
using MetoCraft.NewGui;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace MetoCraft
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        double nOldWndLeft, nOldWndTop, nClickX, nClickY;
        bool mdowned = false;
        System.Windows.Forms.Timer timer;
        List<TaskBar> tasklist = new List<TaskBar>();
        public MainWindow()
        {
            MeCore.NIcon.MainWindow = this;
            MeCore.MainWindow = this;
            InitializeComponent();
            this.Title = "MTMCL V1 Ver." + MeCore.version;
        }
        private void butMenu_Click(object sender, RoutedEventArgs e)
        {
            if (gridMenu.ActualWidth == 55)
            {
                var mover = new DoubleAnimationUsingKeyFrames();
                mover.KeyFrames.Add(new LinearDoubleKeyFrame(55, TimeSpan.FromSeconds(0)));
                mover.KeyFrames.Add(new LinearDoubleKeyFrame(100, TimeSpan.FromSeconds(0.1)));
                mover.KeyFrames.Add(new LinearDoubleKeyFrame(120, TimeSpan.FromSeconds(0.2)));
                var mover1 = new ThicknessAnimationUsingKeyFrames();
                mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(55,0,0,0), TimeSpan.FromSeconds(0)));
                mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(100, 0, -45, 0), TimeSpan.FromSeconds(0.1)));
                mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(120, 0, -65, 0), TimeSpan.FromSeconds(0.2)));
                gridMenu.BeginAnimation(WidthProperty, mover);
                gridMain.BeginAnimation(MarginProperty, mover1);
            }
            else if (gridMenu.ActualWidth == 120)
            {
                var mover = new DoubleAnimationUsingKeyFrames();
                mover.KeyFrames.Add(new LinearDoubleKeyFrame(120, TimeSpan.FromSeconds(0)));
                mover.KeyFrames.Add(new LinearDoubleKeyFrame(75, TimeSpan.FromSeconds(0.1)));
                mover.KeyFrames.Add(new LinearDoubleKeyFrame(55, TimeSpan.FromSeconds(0.2)));
                var mover1 = new ThicknessAnimationUsingKeyFrames();
                mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(120, 0, -65, 0), TimeSpan.FromSeconds(0)));
                mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(75, 0, -20, 0), TimeSpan.FromSeconds(0.1)));
                mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(55, 0, 0, 0), TimeSpan.FromSeconds(0.2)));
                gridMenu.BeginAnimation(WidthProperty, mover);
                gridMain.BeginAnimation(MarginProperty, mover1);
            }
        }
        
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (gridPlay.Margin != new Thickness(0))
            {
                gridPlay.Margin = new Thickness(0, 0, 0, gridMain.ActualHeight);
            }
            else
            {
                if (gridPlay.gridBasic.Margin != new Thickness(0))
                {
                    gridPlay.gridBasic.Margin = new Thickness(0, -(gridMain.ActualHeight), 0, gridMain.ActualHeight);
                }
                if (gridPlay.gridPro.Margin != new Thickness(0))
                {
                    gridPlay.gridPro.Margin = new Thickness(0, (gridMain.ActualHeight), 0, -(gridMain.ActualHeight));
                }
            }
            if (gridDL.Margin != new Thickness(0))
            {
                gridDL.Margin = new Thickness(0, 0, 0, gridMain.ActualHeight);
            }
            if (gridSet.Margin != new Thickness(0))
            {
                gridSet.Margin = new Thickness(0, 0, 0, gridMain.ActualHeight);
            }
            if (gridAbout.Margin != new Thickness(0))
            {
                gridAbout.Margin = new Thickness(0, 0, 0, gridMain.ActualHeight);
            }
        }

        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
/*            gridPlay.Margin = new Thickness(0);
            gridDL.Margin = new Thickness(0, 0, 0, ActualHeight);
            gridSet.Margin = new Thickness(0,0,0,ActualHeight);
            gridAbout.Margin = new Thickness(0,0,0,ActualHeight);*/
            gridPlay.Visibility = Visibility.Visible;
            gridDL.Visibility = Visibility.Hidden;
            gridSet.Visibility = Visibility.Hidden;
            gridAbout.Visibility = Visibility.Hidden;
        }

        private void butAbout_Click(object sender, RoutedEventArgs e)
        {
/*            gridPlay.Margin = new Thickness(0,0,0,ActualHeight);
            gridDL.Margin = new Thickness(0, 0, 0, ActualHeight);
            gridSet.Margin = new Thickness(0, 0, 0, ActualHeight);
            gridAbout.Margin = new Thickness(0);*/
            gridPlay.Visibility = Visibility.Hidden;
            gridDL.Visibility = Visibility.Hidden;
            gridSet.Visibility = Visibility.Hidden;
            gridAbout.Visibility = Visibility.Visible;
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MeCore.NIcon.Hide();
        }

        private void butConfig_Click(object sender, RoutedEventArgs e)
        {
/*            gridPlay.Margin = new Thickness(0, 0, 0, ActualHeight);
            gridDL.Margin = new Thickness(0,0,0,ActualHeight);
            gridSet.Margin = new Thickness(0);
            gridAbout.Margin = new Thickness(0, 0, 0, ActualHeight);*/
            gridPlay.Visibility = Visibility.Hidden;
            gridDL.Visibility = Visibility.Hidden;
            gridSet.Visibility = Visibility.Visible;
            gridAbout.Visibility = Visibility.Hidden;
        }
        public void ChangeLanguage()
        {
//            GridConfig.listDownSource.Items[1] = LangManager.GetLangFromResource("listOfficalSource");
//            GridConfig.listDownSource.Items[0] = LangManager.GetLangFromResource("listAuthorSource");
//            BmclCore.LoadPlugin(LangManager.GetLangFromResource("LangName"));
            gridAbout.loadOSData();
        }
        public bool FinishLoad = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new System.Windows.Forms.Timer();
            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(this.timer_Tick);
            gridSet.listLang.SelectedItem = LangManager.GetLangFromResource("DisplayName");
            gridSet.loadConfig();
            gridPlay.loadConfig();
            gridPlay.sliderRAMPro.Maximum = gridPlay.sliderRAM.Maximum = KMCCC.Tools.SystemTools.GetTotalMemory() / 1024 / 1024;
            if (gridPlay.txtBoxP.Text != "")
            {
                gridPlay.LoadVersionList();
            }
            FinishLoad = true;
        }

        private void butDL_Click(object sender, RoutedEventArgs e)
        {
/*            gridPlay.Margin = new Thickness(0,0,0,ActualHeight);
            gridDL.Margin = new Thickness(0);
            gridSet.Margin = new Thickness(0,0,0,ActualHeight);
            gridAbout.Margin = new Thickness(0, 0, 0, ActualHeight);*/
            gridPlay.Visibility = Visibility.Hidden;
            gridDL.Visibility = Visibility.Visible;
            gridSet.Visibility = Visibility.Hidden;
            gridAbout.Visibility = Visibility.Hidden;
        }

        private void butClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void gridTitle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mdowned = false;
        }

        private void gridTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
/*            if (gridTitle.IsMouseOver)
            {
                DragMove();
            }*/

        }

        private void gridTitle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void butMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void gridTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
            nOldWndLeft = Left;
            nOldWndTop = Top;
            nClickX = e.GetPosition(this).X;
            nClickY = e.GetPosition(this).Y;
            mdowned = true;
        }

/*        private void gridTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (gridTitle.CaptureMouse() && mdowned)
            {
                Top = e.GetPosition(this).Y + nOldWndTop - nClickY;
                Left = e.GetPosition(this).X + nOldWndLeft - nClickX;
                nOldWndLeft = Left;
                nOldWndTop = Top;
            }
        }*/
        public override void OnApplyTemplate()
        {
            Button butMin = GetTemplateChild("butMin") as Button;
            if (butMin != null)
                butMin.Click += butMin_Click;

            Button butCls = GetTemplateChild("butClose") as Button;
            if (butCls != null)
                butCls.Click += butClose_Click;

            Grid gridTitle = GetTemplateChild("gridTitle") as Grid;
            if (gridTitle != null)
                gridTitle.MouseLeftButtonDown += gridTitle_MouseLeftButtonDown;

            base.OnApplyTemplate();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            lblTime.Content = DateTime.Now.ToLocalTime().ToShortTimeString();
            if (tasklist.Count != 0)
            {
                List<TaskBar> deletable = new List<TaskBar>();
                foreach (var task in tasklist)
                {
                    if (task.isFinished())
                    {
                        deletable.Add(task);
                    }
                }
                if (deletable.Count != 0)
                {
                    foreach (var task in deletable)
                    {
                        removeTask(task);
                    }
                }
            }
        }

        private void expanderTask_Expanded(object sender, RoutedEventArgs e)
        {
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(ActualWidth, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1015, TimeSpan.FromSeconds(0.2)));
            var ani1 = new DoubleAnimationUsingKeyFrames();
            ani1.KeyFrames.Add(new LinearDoubleKeyFrame(expanderTask.ActualWidth, TimeSpan.FromSeconds(0)));
            ani1.KeyFrames.Add(new LinearDoubleKeyFrame(415, TimeSpan.FromSeconds(0.2)));
            var ani2 = new ThicknessAnimationUsingKeyFrames();
            ani2.KeyFrames.Add(new LinearThicknessKeyFrame(gridParent.Margin, TimeSpan.FromSeconds(0)));
            ani2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, 0, 415, 0), TimeSpan.FromSeconds(0.2)));
            BeginAnimation(WidthProperty, ani);
            expanderTask.BeginAnimation(WidthProperty, ani1);
            gridParent.BeginAnimation(MarginProperty, ani2);
        }

        private void expanderTask_Collapsed(object sender, RoutedEventArgs e)
        {
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(ActualWidth, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(621, TimeSpan.FromSeconds(0.2)));
            var ani1 = new DoubleAnimationUsingKeyFrames();
            ani1.KeyFrames.Add(new LinearDoubleKeyFrame(expanderTask.ActualWidth, TimeSpan.FromSeconds(0)));
            ani1.KeyFrames.Add(new LinearDoubleKeyFrame(21, TimeSpan.FromSeconds(0.2)));
            var ani2 = new ThicknessAnimationUsingKeyFrames();
            ani2.KeyFrames.Add(new LinearThicknessKeyFrame(gridParent.Margin, TimeSpan.FromSeconds(0)));
            ani2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0,0,21,0), TimeSpan.FromSeconds(0.2)));
            BeginAnimation(WidthProperty, ani);
            expanderTask.BeginAnimation(WidthProperty, ani1);
            gridParent.BeginAnimation(MarginProperty, ani2);
        }

        public void addTask(TaskBar task) {
            task.Margin = new Thickness(0);
            tasklist.Add(task);
            taskPanal.Children.Add(task);
        }
        public void removeTask(TaskBar task)
        {
            tasklist.Remove(task);
            taskPanal.Children.Remove(task);
        }
        private string toGoodString(int i)
        {
            string s = i.ToString();
            if (s.Length == 1) { return "0" + s; } else return s;
        }
    }
}
