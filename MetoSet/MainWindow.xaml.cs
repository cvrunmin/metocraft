using MTMCL.Lang;
using MTMCL.NewGui;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MTMCL
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.Timer timer;
        Dictionary<string, TaskBar> taskdict = new Dictionary<string, TaskBar>();
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
        
        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
            gridPlay.Visibility = Visibility.Visible;
            gridDL.Visibility = Visibility.Hidden;
            gridSet.Visibility = Visibility.Hidden;
            gridAbout.Visibility = Visibility.Hidden;
        }

        private void butAbout_Click(object sender, RoutedEventArgs e)
        {
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
            gridPlay.Visibility = Visibility.Hidden;
            gridDL.Visibility = Visibility.Hidden;
            gridSet.Visibility = Visibility.Visible;
            gridAbout.Visibility = Visibility.Hidden;
        }
        public void ChangeLanguage()
        {
            gridAbout.loadOSData();
        }
        public bool FinishLoad = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MeCore.IsServerDedicated)
            {
                LoadServerDeDicatedVersion();
            }
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
                try
                {
                    if (gridPlay.launcher.GetVersion(MeCore.Config.LastPlayVer) != null)
                    {
                        gridPlay.comboVer.SelectedItem = MeCore.Config.LastPlayVer;
                    }
                }
                catch (Exception ex)
                {
                    new ErrorReport(ex).Show();
                }
            }
            if (!MeCore.Config.ExpandTaskGui)
            {
                expanderTask_Collapsed(this, null);
            }
            FinishLoad = true;
        }

        private void butDL_Click(object sender, RoutedEventArgs e)
        {
            gridPlay.Visibility = Visibility.Hidden;
            gridDL.Visibility = Visibility.Visible;
            gridSet.Visibility = Visibility.Hidden;
            gridAbout.Visibility = Visibility.Hidden;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lblTime.Content = DateTime.Now.ToLocalTime().ToShortTimeString();
            if (taskdict.Count != 0)
            {
                Dictionary<string, TaskBar> deletable = new Dictionary<string, TaskBar>();
                foreach (var task in taskdict)
                {
                    if (task.Value.isFinished())
                    {
                        deletable.Add(task.Key, task.Value);
                    }
                }
                if (deletable.Count != 0)
                {
                    foreach (var task in deletable)
                    {
                        removeTask(task.Key, task.Value);
                    }
                }
            }
            expanderTask.Header = taskdict.Count != 0 ? taskdict.Count.ToString() : "";
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

        public void addTask(TaskBar task, string identifier) {
            if (taskdict.ContainsKey(identifier))
            {
                //return;
                task.noticeExisted();
                string _identifier = identifier + "exist";
                int i = 0;
                while (taskdict.ContainsKey(_identifier))
                {
                    ++i;
                    _identifier = identifier + "exist" + i;
                }
                identifier = _identifier;
            }
            task.Margin = new Thickness(0);
            taskdict.Add(identifier, task);
            taskPanal.Children.Add(task);
        }
        public async void removeTask(string s, TaskBar task)
        {
            taskdict.Remove(s);
            await Task.Delay(1000);
            taskPanal.Children.Remove(task);
        }
        private string toGoodString(int i)
        {
            string s = i.ToString();
            if (s.Length == 1) { return "0" + s; } else return s;
        }
        private void LoadServerDeDicatedVersion() {
            Title = !string.IsNullOrWhiteSpace(MeCore.ServerCfg.Title) ? MeCore.ServerCfg.Title + ", powered by MTMCL" : Title;
            gridDL.butPack.Visibility = Visibility.Visible;
            if (!MeCore.ServerCfg.AllowDownloadLibAndAsset)
            {
                gridDL.butDLLib.IsEnabled = false;
                gridDL.butDLAsset.IsEnabled = false;
            }
            if (!MeCore.ServerCfg.AllowReDownloadLibAndAsset)
            {
                gridDL.butRDLLib.IsEnabled = false;
                gridDL.butRDLAsset.IsEnabled = false;
            }
            if (!MeCore.ServerCfg.AllowSelfDownloadClient)
            {
                gridDL.butDLMC.IsEnabled = false;
                gridDL.butFDL.IsEnabled = false;
            }
            if (MeCore.ServerCfg.NeedServerPack & !string.IsNullOrWhiteSpace(MeCore.ServerCfg.ServerPackUrl))
            {
                gridDL.txtboxUrl.Text = MeCore.ServerCfg.ServerPackUrl;
            }
            if (MeCore.ServerCfg.LockBackground)
            {
                gridSet.butSave.IsEnabled = false;
            }
            if (!string.IsNullOrWhiteSpace(MeCore.ServerCfg.BackgroundPath))
            {
                MeCore.DefaultBG = MeCore.ServerCfg.BackgroundPath;
            }
            if (!string.IsNullOrWhiteSpace(MeCore.ServerCfg.ClientPath))
            {
                gridPlay.txtBoxP.Text = System.IO.Path.Combine(MeCore.BaseDirectory, MeCore.ServerCfg.ClientPath);
                MeCore.Config.MCPath = gridPlay.txtBoxP.Text;
                MeCore.Config.Save(null);
                gridPlay.butBrowse.IsEnabled = false;
            }
            if (!string.IsNullOrWhiteSpace(MeCore.ServerCfg.ServerIP))
            {
                if (MeCore.ServerCfg.ServerIP.IndexOf(':') != -1)
                {
                    gridPlay.serverip = MeCore.ServerCfg.ServerIP.Substring(0,MeCore.ServerCfg.ServerIP.IndexOf(':')).Trim(':');
                    gridPlay.serverport = MeCore.ServerCfg.ServerIP.Substring(MeCore.ServerCfg.ServerIP.IndexOf(':')).Trim(':');
                }
            }
            gridPlay.butDown.IsEnabled = false;
        }
    }
}
