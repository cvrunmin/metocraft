using MahApps.Metro;
using MTMCL.Task;
using MTMCL.Threads;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using MTMCL.Launch;
using MTMCL.Lang;
using System.Windows.Controls;

namespace MTMCL
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : IDisposable
    {
        System.Windows.Forms.Timer timer;
        public LaunchGameInfo _LaunchOptions;
        Download reserve_dl;
        public MainWindow()
        {
            //MeCore.NIcon.MainWindow = this;
            MeCore.MainWindow = this;
            InitializeComponent();
            Title = "MTMCL V2 Ver." + MeCore.version;
            if (MeCore.needGuide)
            {
                gridMain.Visibility = Visibility.Collapsed;
                gridOthers.Visibility = Visibility.Visible;
                gridOthers.Children.Add(new Guide.GridGuide(new Uri("Guide\\PageGuideLang.xaml", UriKind.Relative)));
                gridOthers.Margin = new Thickness(0);
                //new Guide.GuideWindow(new Uri("Guide\\PageGuideLang.xaml", UriKind.Relative)).Show();
                //Close();
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
            ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, -105, 0, 210), TimeSpan.FromSeconds(0.25), new ExponentialEase() { EasingMode = EasingMode.EaseIn, Exponent = 9 }));
            var ani2 = new ThicknessAnimationUsingKeyFrames();
            //ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 75, 0, 25), TimeSpan.FromSeconds(0.15), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 0, 0, 105), TimeSpan.FromSeconds(0.25), new ExponentialEase() { EasingMode = EasingMode.EaseIn, Exponent = 9 }));
            butPlayQuick_a.BeginAnimation(MarginProperty, ani1);
            butPlayQuick_b.BeginAnimation(MarginProperty, ani2);
        }

        private void butPlayQuick_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var ani1 = new ThicknessAnimationUsingKeyFrames();
            //ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, -75, 0, 175), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15)), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 0, 0, 105), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25)), new ExponentialEase() { EasingMode = EasingMode.EaseIn, Exponent = 9 }));
            var ani2 = new ThicknessAnimationUsingKeyFrames();
            //ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 25, 0, 75), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.15)), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
            ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 105, 0, 0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25)), new ExponentialEase() { EasingMode = EasingMode.EaseIn, Exponent = 9 }));
            butPlayQuick_a.BeginAnimation(MarginProperty, ani1);
            butPlayQuick_b.BeginAnimation(MarginProperty, ani2);
        }

        private void butSetting_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("settings");
        }
        internal async void ChangePage(string type, bool required0Margin = false)
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
                    grid = reserve_dl ?? (reserve_dl = new Download());
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
                case "install":
                    tile = butInstall;
                    grid = new Install.GridInstall();
                    break;
                case "gradle":
                    tile = butGradle;
                    grid = new Gradle.GridGradle();
                    break;
                case "help":
                    tile = butHelp;
                    grid = new Help.GridHelp();
                    break;
                default:
                    return;
            }
            gridOthers.Children.Clear();
            ((Rectangle)gridLoadingScreen.Children[0]).Fill = ((Rectangle)tile.GetValue(ContentProperty)).Fill;
            ((Rectangle)gridLoadingScreen.Children[0]).Width = ((Rectangle)tile.GetValue(ContentProperty)).Width * 2;
            ((Rectangle)gridLoadingScreen.Children[0]).Height = ((Rectangle)tile.GetValue(ContentProperty)).Height * 2;
            gridLoadingScreen.Margin = new Thickness(gridMain.Margin.Left + gridMenu.Margin.Left + tile.Margin.Left, gridMain.Margin.Top + gridMenu.Margin.Top + tile.Margin.Top, gridMain.Margin.Right + gridMenu.Margin.Right + (gridMenu.Width - tile.Width - tile.Margin.Left), gridMain.Margin.Bottom + gridMenu.Margin.Bottom + (gridMenu.Height - tile.Height - tile.Margin.Top));
            gridLoadingScreen.Background = new SolidColorBrush(Color.FromRgb(((SolidColorBrush)tile.Background).Color.R, ((SolidColorBrush)tile.Background).Color.G, ((SolidColorBrush)tile.Background).Color.B));
            gridLoadingScreen.Visibility = Visibility.Visible;
            var ani = new ThicknessAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(gridMain.Margin.Left + gridMenu.Margin.Left + tile.Margin.Left, gridMain.Margin.Top + gridMenu.Margin.Top + tile.Margin.Top, gridMain.Margin.Right + gridMenu.Margin.Right + (gridMenu.Width - tile.Width - tile.Margin.Left), gridMain.Margin.Bottom + gridMenu.Margin.Bottom + (gridMenu.Height - tile.Height - tile.Margin.Top)), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)), new ExponentialEase() { EasingMode = EasingMode.EaseInOut, Exponent = 9 }));
            ani.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)), new ExponentialEase() { EasingMode = EasingMode.EaseInOut, Exponent = 9 }));
            gridLoadingScreen.BeginAnimation(MarginProperty, ani);
            await System.Threading.Tasks.TaskEx.Delay(1000);
            gridMain.Visibility = Visibility.Collapsed;
            gridLoadingScreen.Visibility = Visibility.Collapsed;
            gridOthers.Visibility = Visibility.Visible;
            gridOthers.Children.Add(grid);
            gridOthers.Margin = required0Margin ? new Thickness(0) : new Thickness(30, 0, 30, 30);
            var ani2 = new DoubleAnimationUsingKeyFrames();
            ani2.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani2.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            gridOthers.BeginAnimation(OpacityProperty, ani2);
        }
        private void SwitchBetweenHomeAndMenu(bool toHome) {
            gridMa.Visibility = Visibility.Visible;
            var aniHome1 = new DoubleAnimation(toHome ? 1 : 0, TimeSpan.FromSeconds(0.15));
            var aniHome2 = new ThicknessAnimationUsingKeyFrames();
            aniHome2.KeyFrames.Add(new EasingThicknessKeyFrame(toHome ? new Thickness(0) : new Thickness(0, 410, 0, -410), TimeSpan.FromSeconds(0.15), new CubicEase() { EasingMode = EasingMode.EaseInOut}));
            var aniMenu1 = new ThicknessAnimationUsingKeyFrames();
            aniMenu1.KeyFrames.Add( new EasingThicknessKeyFrame( !toHome ? new Thickness(0) : new Thickness(0, 410, 0, -410), TimeSpan.FromSeconds(0.15), new CubicEase() { EasingMode = EasingMode.EaseInOut}));
            var aniMenu2 = new DoubleAnimation(!toHome ? 1 : 0, TimeSpan.FromSeconds(0.15));
            aniMenu2.Completed += delegate {
                gridMa.Visibility = toHome ? Visibility.Collapsed : Visibility.Visible;
            };
            //gridMa.BeginAnimation(MarginProperty, aniMenu1);
            gridMa.BeginAnimation(OpacityProperty, aniMenu2);
            gridHome.BeginAnimation(OpacityProperty, aniHome1);
            gridHome.BeginAnimation(MarginProperty, aniHome2);
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
            LaunchGame(LaunchMode.Normal);
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
            LaunchGame(LaunchMode.BMCL);
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
            LaunchGame(LaunchMode.BakaXL);
            MeCore.MainWindow.launchFlyout.IsOpen = false;
        }
        public void LaunchGame(LaunchMode mode)
        {
            if (_LaunchOptions != null)
            {
                LaunchGame(_LaunchOptions, mode);
            }
        }
        public void LaunchGame(LaunchGameInfo options, LaunchMode mode)
        {
            if (options != null)
            {
                _LaunchOptions = options;
                _LaunchOptions.SetMode(mode);
                string uri = "pack://application:,,,/Resources/play-normal-banner.jpg";
                if (mode is BMCLLaunchMode)
                {
                    uri = "pack://application:,,,/Resources/play-bmcl-banner.jpg";
                }
                if (mode is BakaXLLaunchMode)
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
                task.OnAuthUpdate += delegate (Launch.Login.AuthInfo info)
                {
                    try
                    {
                        MeCore.Config.SavedAuths[info.DisplayName].AccessToken = info.Session.ToString();
                    }
                    catch (Exception) { }
                };
                task.Failed += () => {
                    Dispatcher.Invoke(new Action(() => gui.noticeFailed()));
                };
                addTask("game", gui.setTask(string.Format(LangManager.GetLangFromResource("TaskLaunch"), _LaunchOptions.Version.id)).setThread(task).setDetectAlive(false));
                MeCore.MainWindow.addBalloonNotice(new Notice.NoticeBalloon("MTMCL", string.Format(LangManager.GetLangFromResource("BalloonNoticeSTTaskFormat"), LangManager.GetLangFromResource("TaskLaunch"))));
            }
        }
        private void butPlayQuick_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (butPlayQuick.IsEnabled)
            {
                var ani1 = new ThicknessAnimationUsingKeyFrames();
                ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 0, 0, 105), TimeSpan.FromSeconds(0.25), new CubicEase() { EasingMode = EasingMode.EaseInOut }));
                var ani2 = new ThicknessAnimationUsingKeyFrames();
                ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, -105, 0, 210), TimeSpan.FromSeconds(0.25), new CubicEase() { EasingMode = EasingMode.EaseInOut }));
                var ani3 = new ThicknessAnimationUsingKeyFrames();
                ani3.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 105, 0, 0), TimeSpan.FromSeconds(0.25), new CubicEase() { EasingMode = EasingMode.EaseInOut }));
                butPlayQuick_a.BeginAnimation(MarginProperty, ani1);
                butPlayQuick_c.BeginAnimation(MarginProperty, ani2);
                butPlayQuick_b.BeginAnimation(MarginProperty, ani3);
            }
            else
            {
                var ani1 = new ThicknessAnimationUsingKeyFrames();
                //ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, -25, 0, 125), TimeSpan.FromSeconds(0.15), new QuarticEase() { EasingMode = EasingMode.EaseIn}));
                ani1.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 105, 0, 0), TimeSpan.FromSeconds(0.25), new QuarticEase() { EasingMode = EasingMode.EaseInOut }));
                var ani2 = new ThicknessAnimationUsingKeyFrames();
                //ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 75, 0, 25), TimeSpan.FromSeconds(0.15), new QuarticEase() { EasingMode = EasingMode.EaseIn }));
                ani2.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 0, 0, 105), TimeSpan.FromSeconds(0.25), new QuarticEase() { EasingMode = EasingMode.EaseInOut }));
                var ani3 = new ThicknessAnimationUsingKeyFrames();
                ani3.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0, 210, 0, -105), TimeSpan.FromSeconds(0.25), new CubicEase() { EasingMode = EasingMode.EaseInOut }));
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
        internal List<Notice.INotice> noticelist = new List<Notice.INotice>();
        public delegate void TaskAdded(TaskListBar tlb);
        public event TaskAdded OnTaskAdded;
        public void addTask(string identifier, TaskListBar task)
        {
            if (taskdict.ContainsKey(identifier))
            {
                if (taskdict[identifier].isFinished())
                {
                    string _identifier = identifier + "_";
                    int ie = 0;
                    while (taskdict.ContainsKey(_identifier))
                    {
                        ie++;
                        _identifier = identifier + ie;
                    }
                    identifier = _identifier;
                }
                else
                {
                    task.noticeExisted();
                    string _identifier = identifier + "exist";
                    int i = 0;
                    while (taskdict.ContainsKey(_identifier))
                    {
                        i++;
                        _identifier = identifier + "exist" + i;
                    }
                identifier = _identifier;
                }
            }
            task.Margin = new Thickness(0);
            taskdict.Add(identifier, task);
            OnTaskAdded?.Invoke(task);
            butTask.Count = taskdict.Where(n => !n.Value.isFinished()).Count() > 0 ? taskdict.Where(n => !n.Value.isFinished()).Count().ToString() : "";
        }
        public void removeTask(string s, TaskListBar task)
        {
            taskdict.Remove(s);
            butTask.Count = taskdict.Where(n => !n.Value.isFinished()).Count() > 0 ? taskdict.Where(n => !n.Value.isFinished()).Count().ToString() : "";
        }

        private void butTask_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("tasklist", true);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            butTask.Count = taskdict.Where(n => !n.Value.isFinished()).Count() > 0 ? taskdict.Where(n => !n.Value.isFinished()).Count().ToString() : "";
            /*if (taskdict.Count != 0)
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
            }*/
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
            RenderColor();
            ThemeManager.ChangeAppTheme(Application.Current, MeCore.Config.reverseColor ? "BaseDark" : "BaseLight");
            ChangeTileColor();
        }
        private void ChangeTileColor() {
            try
            {
                foreach (var field in typeof(Customize.TileColor).GetFields())
                {
                    string color = field.GetValue(MeCore.TileColor) as string;
                    if (color.Equals("default", StringComparison.InvariantCultureIgnoreCase)) continue;
                    //if (color.Length < 6) color = "000000".Substring(color.Length) + color;
                    int colori = Convert.ToInt32(color, 16);
                    SolidColorBrush newBrush = new SolidColorBrush(Color.FromArgb(0xCC, (byte)((colori >> 16) & 255), (byte)((colori >> 8) & 255), (byte)(colori & 255) ));
                    switch (field.Name.Replace("tileColor", ""))
                    {
                        case "DL":
                            butDL.Background = newBrush;
                            break;
                        case "Gradle":
                            butGradle.Background = newBrush;
                            break;
                        case "Install":
                            butHelp.Background = newBrush;
                            break;
                        case "Notice":
                            butNotice.Background = newBrush;
                            break;
                        case "Play":
                            butPlay.Background = newBrush;
                            break;
                        case "QuickPlay":
                            butPlayQuick.Background = newBrush;
                            break;
                        case "Server":
                            butServer.Background = newBrush;
                            break;
                        case "Setting":
                            butSetting.Background = newBrush;
                            break;
                        case "Task":
                            butTask.Background = newBrush;
                            break;
                    }
                }
            } catch (Exception e) { Logger.log(e); };
        }
        private void butPlayQuick_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /*if (App.core == null)
                {
                    try
                    {
                        App.core = LauncherCore.Create(new LauncherCoreCreationOption(MeCore.IsServerDedicated ? (string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath) ? ".minecraft" : MeCore.Config.Server.ClientPath) : MeCore.Config.MCPath, MeCore.Config.Javaw, new KMCCC.Modules.JVersion.NewJVersionLocator()));
                    }
                    catch { }
                }*/
                if (!string.IsNullOrWhiteSpace(MeCore.Config.LastPlayVer))
                {
                    Versions.VersionJson version = Versions.VersionReader.GetFurtherVersion(MeCore.Config.MCPath, MeCore.Config.LastPlayVer);
                    if (version != null)
                    {
                        Launch.Login.IAuth auth;
                        if (string.IsNullOrWhiteSpace(MeCore.Config.DefaultAuth))
                        {
                            ACSelect ac = new ACSelect();
                            ac.ShowDialog();
                            auth = ac.auth;
                        }
                        else
                        {
                            Config.SavedAuth dauth;
                            MeCore.Config.SavedAuths.TryGetValue(MeCore.Config.DefaultAuth, out dauth);
                            auth = dauth.AuthType.Equals("Yggdrasil") ? new Launch.Login.YggdrasilRefreshAuth(dauth.AccessToken) : new Launch.Login.AuthWarpper(new Launch.Login.AuthInfo { DisplayName = MeCore.Config.DefaultAuth, Session = dauth.AccessToken, UUID = dauth.UUID, UserType = dauth.UserType, Prop = dauth.Properies }) as Launch.Login.IAuth;
                        }
                        /*ACLogin ac = new ACLogin();
                        ac.ShowDialog();
                        auth = ac.auth;*/
                        if (auth != null)
                        {
                            LaunchGameInfo option = LaunchGameInfo.CreateInfo(MeCore.Config.MCPath, auth, version,MeCore.Config.Javaw, (int)MeCore.Config.Javaxmx, CreateServerInfo());
                            LaunchGame(option, LaunchMode.GetMode(MeCore.Config.LastLaunchMode));
                        }
                    }
                }
            }
            catch { }
        }
        private ServerInfo CreateServerInfo() {
            if (MeCore.IsServerDedicated)
            {
                if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ServerIP))
                {
                    if (MeCore.Config.Server.ServerIP.IndexOf(':') != -1)
                    {
                        ushort port = 25565;
                        if (!ushort.TryParse(MeCore.Config.Server.ServerIP.Substring(MeCore.Config.Server.ServerIP.IndexOf(':')).Trim(':'), out port))
                        {
                            port = 25565;
                        }
                        return new Launch.ServerInfo
                        {
                            Ip = MeCore.Config.Server.ServerIP.Substring(0, MeCore.Config.Server.ServerIP.IndexOf(':')).Trim(':'),
                            Port = port
                        };
                    }
                }
            }
            return null;
        }
        private void butNotice_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("notice", true);
        }
        public delegate void NoticeAdded(Notice.INotice notice);
        public event NoticeAdded OnNoticeAdded;
        internal void addNotice(Notice.INotice notice) {
            noticelist.Add(notice);
            OnNoticeAdded?.Invoke(notice);
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
        public void RenderColor() {
            Tuple<AppTheme, Accent> theme = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(MeCore.Config.ColorScheme), theme.Item1);
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
                            await System.Threading.Tasks.TaskEx.Delay(250);
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
                            await System.Threading.Tasks.TaskEx.Delay(250);
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
                    System.Windows.Forms.Application.Restart();
                }
            }
        internal void RenderTheme(Themes.Theme theme) {
            var image = theme.Image;
            var accent = theme.Accent;
            if (theme is Themes.DefaultTheme) {
                image = ((Themes.DefaultTheme) theme).Image;
                accent = ((Themes.DefaultTheme) theme).Accent;
            }
                MeCore.MainWindow.gridBG.Opacity = 0;
                MeCore.MainWindow.gridBG.Background = new ImageBrush
                {
                    ImageSource = image,
                    Stretch = Stretch.UniformToFill
                };
                var ani = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
                ani.Completed += async delegate
                {
                    await System.Threading.Tasks.TaskEx.Delay(250);
                    MeCore.MainWindow.gridParent.Background = new ImageBrush
                    {
                        ImageSource = image,
                        Stretch = Stretch.UniformToFill
                    };
                    MeCore.MainWindow.gridBG.Opacity = 0;
                };
                MeCore.MainWindow.gridBG.BeginAnimation(OpacityProperty, ani);
            
            Tuple<AppTheme, Accent> AppTheme = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManager.ChangeAppStyle(Application.Current, accent, AppTheme.Item1);
        }

        private void butServer_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("server");
        }

        private void butInstall_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("install");
        }

        private void butGradle_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("gradle");
        }

        private void quickbutTask_Click(object sender, RoutedEventArgs e)
        {
            SwitchBetweenHomeAndMenu(false);
            butTask_Click(sender, e);
        }

        private void butHome_Click(object sender, RoutedEventArgs e)
        {
            SwitchBetweenHomeAndMenu(gridHome.Opacity == 0);
        }

        private void quickbutNotice_Click(object sender, RoutedEventArgs e)
        {
            SwitchBetweenHomeAndMenu(false);
            butNotice_Click(sender, e);
        }

        private void quickbutPlay_Click(object sender, RoutedEventArgs e)
        {
            SwitchBetweenHomeAndMenu(false);
            butPlay_Click(sender, e);
        }

        private void quickbutGradle_Click(object sender, RoutedEventArgs e)
        {
            SwitchBetweenHomeAndMenu(false);
            butGradle_Click(sender, e);
        }

        private void quickbutSetting_Click(object sender, RoutedEventArgs e)
        {
            SwitchBetweenHomeAndMenu(false);
            butSetting_Click(sender, e);
        }

        private void quickbutDL_Click(object sender, RoutedEventArgs e)
        {
            SwitchBetweenHomeAndMenu(false);
            butDL_Click(sender, e);
        }

        private void butHelp_Click(object sender, RoutedEventArgs e)
        {
            ChangePage("help", true);
        }
        public void addBalloonNotice(Notice.NoticeBalloon balloon) {
            panelBalloon.Children.Add(balloon);
            balloon.Margin = new Thickness(250, 0, -250, 0);
            var ani = new ThicknessAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(250, 0, -250, 0), TimeSpan.FromSeconds(0)) { EasingFunction = new SineEase() {EasingMode = EasingMode.EaseInOut } });
            ani.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0.5)) { EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut } });
            ani.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(2.5)) { EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut } });
            ani.KeyFrames.Add(new EasingThicknessKeyFrame(new Thickness(250, 0, -250, 0), TimeSpan.FromSeconds(3)) { EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut } });
            ani.Completed += (obj, e)=> {
                panelBalloon.Children.Remove(balloon);
            };
            balloon.BeginAnimation(MarginProperty, ani);
        }

        private void MenuChangeColor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                if (((MenuItem)sender).Parent is ContextMenu)
                {
                    if (((ContextMenu)((MenuItem)sender).Parent).PlacementTarget is MahApps.Metro.Controls.Tile)
                    {
                        var tile = (MahApps.Metro.Controls.Tile)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
                        System.Windows.Forms.ColorDialog dialog = new System.Windows.Forms.ColorDialog();
                        dialog.FullOpen = true;
                        dialog.AnyColor = true;
                        dialog.ShowDialog();
                        tile.Background = new SolidColorBrush(Color.FromArgb(0xCC, dialog.Color.R, dialog.Color.G, dialog.Color.B));
                        MeCore.TileColor.QuickChange(tile.Name, Convert.ToString(dialog.Color.ToArgb() & 0xFFFFFF,16));
                    }
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose (bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置 Managed 狀態 (Managed 物件)。
                    timer.Dispose();
                    _LaunchOptions = null;
                    noticelist = null;
                    taskdict = null;
                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        // ~MainWindow() {
        //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose ()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
            // GC.SuppressFinalize(this);
        }
        #endregion

        private void MetroWindow_Closing (object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.AboutToExit();
        }
    }
}
