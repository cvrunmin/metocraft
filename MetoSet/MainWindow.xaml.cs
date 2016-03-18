using KMCCC.Launcher;
using MTMCL.Task;
using MTMCL.Threads;
using MTMCL.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
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
        System.Windows.Forms.Timer timer;
        public LaunchOptions _LaunchOptions;
        bool gameSentLaunch;
        public MainWindow()
        {
            //MeCore.NIcon.MainWindow = this;
            MeCore.MainWindow = this;
            InitializeComponent();
            Title = "MTMCL V2 Ver." + MeCore.version;
            MeCore.needGuide = true;
            if (MeCore.needGuide)
            {
                new Guide.GuideWindow(new Uri("Guide\\PageGuideLang.xaml", UriKind.Relative)).Show();
                Close();
            }
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
            //ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, -25, 0, 125), TimeSpan.FromSeconds(0.15), new QuarticEase() { EasingMode = EasingMode.EaseIn}));
            ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, -100, 0, 200), TimeSpan.FromSeconds(0.25), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            var ani2 = new ThicknessAnimationUsingKeyFrames();
            //ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 75, 0, 25), TimeSpan.FromSeconds(0.15), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 0, 0, 100), TimeSpan.FromSeconds(0.25), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            butPlayQuick_a.BeginAnimation(MarginProperty, ani1);
            butPlayQuick_b.BeginAnimation(MarginProperty, ani2);
        }

        private void butPlayQuick_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ani1 = new ThicknessAnimationUsingKeyFrames();
            //ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, -75, 0, 175), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15)), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 0, 0, 100), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25)), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            var ani2 = new ThicknessAnimationUsingKeyFrames();
            //ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 25, 0, 75), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15)), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 100, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25)), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            butPlayQuick_a.BeginAnimation(MarginProperty, ani1);
            butPlayQuick_b.BeginAnimation(MarginProperty, ani2);
        }

        private void butSetting_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("settings");
        }
        private async void ChangePage(string type)
        {
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
                case "download":
                    tile = butDL;
                    grid = new Download();
                    break;
                case "tasklist":
                    tile = butTask;
                    grid = new TaskList();
                    break;
                case "notice":
                    tile = butNotice;
                    grid = new Notice.Notice();
                    break;
                case "server":
                    tile = butServer;
                    grid = new Server.Server();
                    break;
                default:
                    return;
            }
            ((Rectangle)gridLoadingScreen.Children[0]).Fill = ((Rectangle)tile.GetValue(ContentProperty)).Fill;
            gridLoadingScreen.Margin = new Thickness(gridMenu.Margin.Left + tile.Margin.Left, gridMenu.Margin.Top + tile.Margin.Top, gridMenu.Margin.Right + (gridMenu.Width - tile.Width - tile.Margin.Left), gridMenu.Margin.Bottom + (gridMenu.Height - tile.Height - tile.Margin.Top));
            gridLoadingScreen.Background = new SolidColorBrush(Color.FromRgb(((SolidColorBrush)tile.Background).Color.R, ((SolidColorBrush)tile.Background).Color.G, ((SolidColorBrush)tile.Background).Color.B));
            gridLoadingScreen.Visibility = Visibility.Visible;
            var ani = new ThicknessAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(gridMenu.Margin.Left + tile.Margin.Left, gridMenu.Margin.Top + tile.Margin.Top, gridMenu.Margin.Right + (gridMenu.Width - tile.Width - tile.Margin.Left), gridMenu.Margin.Bottom + (gridMenu.Height - tile.Height - tile.Margin.Top)), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), new CubicEase() { EasingMode = EasingMode.EaseInOut }));
            //ani.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(10), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.05))));
            ani.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)), new CubicEase() { EasingMode = EasingMode.EaseInOut }));
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
            gridBG.Opacity = 0;
            gridBG.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/play-normal.jpg"))) { Stretch = Stretch.UniformToFill };
            var ani = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
            gridBG.BeginAnimation(OpacityProperty, ani);
        }
        private void butLaunchNormal_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ani = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.25));
            gridBG.BeginAnimation(OpacityProperty, ani);
        }
        private void butLaunchNormal_Click(object sender, RoutedEventArgs e)
        {
            LaunchGame(null);
            MeCore.MainWindow.launchFlyout.IsOpen = false;
        }

        private void butLaunchBMCL_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            gridBG.Opacity = 0;
            gridBG.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/play-bmcl.jpg"))) { Stretch = Stretch.UniformToFill };
            var ani = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
            gridBG.BeginAnimation(OpacityProperty, ani);
        }
        private void butLaunchBMCL_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ani = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.25));
            gridBG.BeginAnimation(OpacityProperty, ani);
        }
        private void butLaunchBMCL_Click(object sender, RoutedEventArgs e)
        {
            LaunchGame(LaunchMode.BmclMode);
            MeCore.MainWindow.launchFlyout.IsOpen = false;
        }

        private void butLaunchBaka_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            gridBG.Opacity = 0;
            gridBG.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/play-bakaxl.jpg"))) { Stretch = Stretch.UniformToFill };
            var ani = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
            gridBG.BeginAnimation(OpacityProperty, ani);
        }
        private void butLaunchBaka_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ani = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.25));
            gridBG.BeginAnimation(OpacityProperty, ani);
        }
        private void butLaunchBaka_Click(object sender, RoutedEventArgs e)
        {
            LaunchGame(new BakaXLMode());
            MeCore.MainWindow.launchFlyout.IsOpen = false;
        }
        public void LaunchGame(LaunchMode mode)
        {
            if (_LaunchOptions != null)
            {
                LaunchGame(_LaunchOptions, mode);
            }
        }
        public void LaunchGame(LaunchOptions options, LaunchMode mode)
        {
            gameSentLaunch = true;
            if (options != null)
            {
                _LaunchOptions = options;
                _LaunchOptions.Mode = mode;
                string uri = "pack://application:,,,/Resources/play-normal-banner.jpg";
                if (mode is BmclLaunchMode)
                {
                    uri = "pack://application:,,,/Resources/play-bmcl-banner.jpg";
                }
                if (mode is BakaXLMode)
                {
                    uri = "pack://application:,,,/Resources/play-bakaxl-banner.jpg";
                }
                TaskListBar gui = new TaskListBar() { ImgSrc = new BitmapImage(new Uri(uri)) };
                var task = new LaunchMCThread(_LaunchOptions);
                task.StateChange += delegate (string state)
                {
                    Dispatcher.Invoke(new Action(() => gui.setTaskStatus(state)));
                };
                task.TaskCountTime += delegate
                {
                    Dispatcher.Invoke(new Action(() => butPlayQuick.IsEnabled = false));
                    Dispatcher.Invoke(new Action(() => gui.countTime()));
                };
                task.GameExit += delegate
                {
                    Dispatcher.Invoke(new Action(() => gameSentLaunch = false));
                    Dispatcher.Invoke(new Action(() => butPlayQuick.IsEnabled = true));
                    Dispatcher.Invoke(new Action(() => gui.stopCountTime().noticeFinished()));
                };
                task.OnLogged += delegate (string s) {
                    Dispatcher.Invoke(new Action(() => gui.log(s)));
                };
                task.GameCrash += delegate (string content, string path)
                {
                    //new MCCrash(content, path).Show();
                };
                task.Failed += () => {
                    Dispatcher.Invoke(() => gui.noticeFailed());
                };
                addTask("game", gui.setTask(string.Format(Lang.LangManager.GetLangFromResource("TaskLaunch"), _LaunchOptions.Version.Id)).setThread(task).setDetectAlive(false));
            }
            else
            {
                gameSentLaunch = false;
            }
        }
        private void butPlayQuick_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (butPlayQuick.IsEnabled)
            {
                var ani1 = new ThicknessAnimationUsingKeyFrames();
                ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 0, 0, 100), TimeSpan.FromSeconds(0.25), new CubicEase() { EasingMode = EasingMode.EaseInOut }));
                var ani2 = new ThicknessAnimationUsingKeyFrames();
                ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, -100, 0, 200), TimeSpan.FromSeconds(0.25), new CubicEase() { EasingMode = EasingMode.EaseInOut }));
                var ani3 = new ThicknessAnimationUsingKeyFrames();
                ani3.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 100, 0, 0), TimeSpan.FromSeconds(0.25), new CubicEase() { EasingMode = EasingMode.EaseInOut }));
                butPlayQuick_a.BeginAnimation(MarginProperty, ani1);
                butPlayQuick_c.BeginAnimation(MarginProperty, ani2);
                butPlayQuick_b.BeginAnimation(MarginProperty, ani3);
            }
            else
            {
                var ani1 = new ThicknessAnimationUsingKeyFrames();
                //ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, -25, 0, 125), TimeSpan.FromSeconds(0.15), new QuarticEase() { EasingMode = EasingMode.EaseIn}));
                ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 100, 0, 0), TimeSpan.FromSeconds(0.25), new QuarticEase() { EasingMode = EasingMode.EaseInOut }));
                var ani2 = new ThicknessAnimationUsingKeyFrames();
                //ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 75, 0, 25), TimeSpan.FromSeconds(0.15), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
                ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 0, 0, 100), TimeSpan.FromSeconds(0.25), new QuarticEase() { EasingMode = EasingMode.EaseInOut }));
                var ani3 = new ThicknessAnimationUsingKeyFrames();
                ani3.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 200, 0, -100), TimeSpan.FromSeconds(0.25), new CubicEase() { EasingMode = EasingMode.EaseInOut }));
                butPlayQuick_a.BeginAnimation(MarginProperty, ani1);
                butPlayQuick_c.BeginAnimation(MarginProperty, ani2);
                butPlayQuick_b.BeginAnimation(MarginProperty, ani3);
            }
        }

        private void butDL_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("download");
        }

        public Dictionary<string, TaskListBar> taskdict = new Dictionary<string, TaskListBar>();
        public List<Notice.CrashErrorBar> noticelist = new List<Notice.CrashErrorBar>();
        public void addTask(string identifier, TaskListBar task)
        {
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
            butTask.Count = taskdict.Count > 0 ? taskdict.Count.ToString() : "";
        }
        public void removeTask(string s, TaskListBar task)
        {
            taskdict.Remove(s);
            butTask.Count = taskdict.Count > 0 ? taskdict.Count.ToString() : "";
        }

        private void butTask_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("tasklist");
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (taskdict.Count != 0)
            {
                Dictionary<string, TaskListBar> deletable = new Dictionary<string, TaskListBar>();
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
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (MeCore.IsServerDedicated)
            {
                LoadServerDeDicatedVersion();
            }
            timer = new System.Windows.Forms.Timer();
            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
            Render();
        }

        private void butPlayQuick_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.core == null)
                {
                    try
                    {
                        App.core = LauncherCore.Create(new LauncherCoreCreationOption(MeCore.IsServerDedicated ? (string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath) ? ".minecraft" : MeCore.Config.Server.ClientPath) : MeCore.Config.MCPath, MeCore.Config.Javaw));
                    }
                    catch { }
                }
                if (!string.IsNullOrWhiteSpace(MeCore.Config.LastPlayVer))
                {
                    KMCCC.Launcher.Version version = App.core.GetVersion(MeCore.Config.LastPlayVer);
                    if (version != null)
                    {
                        ACLogin ac = new ACLogin();
                        ac.ShowDialog();
                        if (ac.auth != null)
                        {
                            LaunchOptions option = new LaunchOptions
                            {
                                Authenticator = ac.auth,
                                MaxMemory = (int)MeCore.Config.Javaxmx,
                                Version = version
                            };
                            if (MeCore.IsServerDedicated)
                            {
                                if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ServerIP))
                                {
                                    if (MeCore.Config.Server.ServerIP.IndexOf(':') != -1)
                                    {
                                        ushort port = 25565;
                                        if (!ushort.TryParse(MeCore.Config.Server.ServerIP.Substring(MeCore.Config.Server.ServerIP.IndexOf(':')).Trim(':'), out port)) {
                                            port = 25565;
                                        }
                                        option.Server = new ServerInfo {
                                            Address = MeCore.Config.Server.ServerIP.Substring(0, MeCore.Config.Server.ServerIP.IndexOf(':')).Trim(':'),
                                            Port = port
                                        };
                                    }
                                }
                            }
                            LaunchGame(option, null);
                        }
                    }
                }
            }
            catch { }
        }

        private void butNotice_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("notice");
        }
        public void addNotice(Notice.CrashErrorBar notice) {
            noticelist.Add(notice);
            butNotice.Count = noticelist.Count > 0 ? noticelist.Count.ToString() : "";
        }
        private void LoadServerDeDicatedVersion()
        {
            Title = !string.IsNullOrWhiteSpace(MeCore.Config.Server.Title) ? MeCore.Config.Server.Title + ", powered by MTMCL" : Title;
            if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.BackgroundPath))
            {
                MeCore.DefaultBG = MeCore.Config.Server.BackgroundPath;
            }
        }
        public void Render()
        {
            bool lockedBG = false;
            if (MeCore.Config.Server != null)
            {
                lockedBG = MeCore.Config.Server.LockBackground;
            }
                try
                {
                    if (MeCore.Config.Background.Equals("default", StringComparison.InvariantCultureIgnoreCase) | string.IsNullOrWhiteSpace(MeCore.Config.Background) | lockedBG)
                    {
                        MeCore.MainWindow.gridBG.Opacity = 0;
                        MeCore.MainWindow.gridBG.Background = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri(MeCore.DefaultBG)),
                            Stretch = Stretch.UniformToFill
                        };
                        var ani = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
                        ani.Completed += async delegate
                        {
                            await System.Threading.Tasks.Task.Delay(250);
                            MeCore.MainWindow.gridParent.Background = new ImageBrush
                            {
                                ImageSource = new BitmapImage(new Uri(MeCore.DefaultBG)),
                                Stretch = Stretch.UniformToFill
                            };
                            MeCore.MainWindow.gridBG.Opacity = 0;
                        };
                        MeCore.MainWindow.gridBG.BeginAnimation(OpacityProperty, ani);
                    }
                    else
                    {
                        MeCore.MainWindow.gridBG.Opacity = 0;
                        MeCore.MainWindow.gridBG.Background = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri(MeCore.Config.Background)),
                            Stretch = Stretch.UniformToFill
                        };
                        var ani = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
                        ani.Completed += async delegate
                        {
                            await System.Threading.Tasks.Task.Delay(250);
                            MeCore.MainWindow.gridParent.Background = new ImageBrush
                            {
                                ImageSource = new BitmapImage(new Uri(MeCore.Config.Background)),
                                Stretch = Stretch.UniformToFill
                            };
                            MeCore.MainWindow.gridBG.Opacity = 0;
                        };
                        MeCore.MainWindow.gridBG.BeginAnimation(OpacityProperty, ani);
                    }
                }
                catch (Exception)
                {
                    MeCore.Config.Background = "default";
                    MeCore.Config.Save(null);
                    MeCore.MainWindow.Close();
                    System.Windows.Forms.Application.Restart();
                }
            }

        private void butServer_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("server");
        }
    }
}
