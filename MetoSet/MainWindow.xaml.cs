using MetoCraft.Lang;
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
        public MainWindow()
        {
            MeCore.NIcon.MainWindow = this;
            MeCore.MainWindow = this;
            InitializeComponent();
            this.Title = "MetoCraft V1 Ver." + MeCore.version;
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
            gridPlay.Margin = new Thickness(0);
            gridDL.Margin = new Thickness(0, 0, 0, ActualHeight);
            gridSet.Margin = new Thickness(0,0,0,ActualHeight);
            gridAbout.Margin = new Thickness(0,0,0,ActualHeight);
        }

        private void butAbout_Click(object sender, RoutedEventArgs e)
        {
            gridPlay.Margin = new Thickness(0,0,0,ActualHeight);
            gridDL.Margin = new Thickness(0, 0, 0, ActualHeight);
            gridSet.Margin = new Thickness(0, 0, 0, ActualHeight);
            gridAbout.Margin = new Thickness(0);
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
            gridPlay.Margin = new Thickness(0, 0, 0, ActualHeight);
            gridDL.Margin = new Thickness(0,0,0,ActualHeight);
            gridSet.Margin = new Thickness(0);
            gridAbout.Margin = new Thickness(0, 0, 0, ActualHeight);
        }
        public void ChangeLanguage()
        {
//            GridConfig.listDownSource.Items[1] = LangManager.GetLangFromResource("listOfficalSource");
//            GridConfig.listDownSource.Items[0] = LangManager.GetLangFromResource("listAuthorSource");
//            BmclCore.LoadPlugin(LangManager.GetLangFromResource("LangName"));
//            gridAbout.loadOSData();
        }
        public bool FinishLoad = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
//            gridSettings.listLang.SelectedItem = LangManager.GetLangFromResource("DisplayName");
            gridPlay.loadConfig();
            this.gridPlay.sliderRAM.Maximum = Config.GetMemory();
            if (gridPlay.txtBoxP.Text != "")
            {
                gridPlay.LoadVersionList();
            }
            FinishLoad = true;
        }

        private void butDL_Click(object sender, RoutedEventArgs e)
        {
            gridPlay.Margin = new Thickness(0,0,0,ActualHeight);
            gridDL.Margin = new Thickness(0);
            gridSet.Margin = new Thickness(0,0,0,ActualHeight);
            gridAbout.Margin = new Thickness(0, 0, 0, ActualHeight);
        }

        private void but_Click(object sender, RoutedEventArgs e)
        {
            MetoCraft.NewGui.TaskGui task = new NewGui.TaskGui();
            task.Show();
            task.setTask("啟動Corn Him's UHC").setTaskStatus("");
        }
    }
}
