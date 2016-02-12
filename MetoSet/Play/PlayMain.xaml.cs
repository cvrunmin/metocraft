using KMCCC.Launcher;
using MTMCL.NewGui;
using MTMCL.Profile;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace MTMCL.Play
{
    /// <summary>
    /// PlayMain.xaml 的互動邏輯
    /// </summary>
    public partial class PlayMain : Grid
    {
        public LauncherCore launcher;
        private KMCCC.Launcher.Version[] vers;
        public KMCCC.Launcher.Version[] versions
        {
            get
            {
                return vers;
            }
            private set
            {
                vers = value;
            }
        }
        private int _clientCrashReportCount;
        private Profile.Profile[] profiles;
        private string _path = Environment.CurrentDirectory + "\\profiles.xml";
        private ProfileInXML xmlLoader;
        public string serverip;
        public string serverport;
        public PlayMain()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            comboJava.Items.Clear();
            var javas = KMCCC.Tools.SystemTools.FindValidJava().ToList();
            foreach (var java in javas)
            {
                comboJava.Items.Add(java);
            }
        }
        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
            ACLogin login = new ACLogin();
            Dispatcher.Invoke(new MethodInvoker(delegate
            {
                login.ShowDialog();
            }));
            if (login.auth != null)
            {
                TaskBar gui = new TaskBar();
                var task = new Thread(new ThreadStart(delegate
                {
                    Dispatcher.Invoke(new MethodInvoker(delegate
                    {
                        gui.setTaskStatus("正在啟動");
                        new Assets.Assets(versions[comboVer.SelectedIndex]);
                        launcher.GameExit += onGameExit;
                        var result = launcher.Launch(new LaunchOptions
                        {
                            Version = versions[comboVer.SelectedIndex],
                            MaxMemory = (int)sliderRAM.Value,
                            Authenticator = login.auth,
                            Server = new ServerInfo {
                                Address = serverip,
                                Port = Convert.ToUInt16(serverport)
                            }
                        });
                        if (!result.Success)
                        {
                            switch (result.ErrorType)
                            {
                                case ErrorType.NoJAVA:
                                    new KnownErrorReport(result.ErrorMessage, Lang.LangManager.GetLangFromResource("JavaFault")).Show();
                                    break;
                                case ErrorType.AuthenticationFailed:
                                    new KnownErrorReport(result.ErrorMessage, Lang.LangManager.GetLangFromResource("AuthFault")).Show();
                                    break;
                                case ErrorType.OperatorException:
                                    new KnownErrorReport(result.ErrorMessage).Show();
                                    break;
                                case ErrorType.UncompressingFailed:
                                    new KnownErrorReport(result.ErrorMessage, Lang.LangManager.GetLangFromResource("LibFault")).Show();
                                    break;
                                case ErrorType.Unknown:
                                    new KnownErrorReport(result.ErrorMessage).Show();
                                    break;
                            }
                        }
                        else
                        {
                            _clientCrashReportCount = System.IO.Directory.Exists(launcher.GameRootPath + @"\crash-reports") ? System.IO.Directory.GetFiles(launcher.GameRootPath + @"\crash-reports").Count() : 0;
                            gui.setTaskStatus("");
                            gui.setSubProcess(result.Handle.GetType().GetField("Process", System.Reflection.BindingFlags.NonPublic |
                                    System.Reflection.BindingFlags.Instance).GetValue(result.Handle) as System.Diagnostics.Process);
                            gui.countTime();
                            MeCore.NIcon.ShowBalloonTip(3000, "Successful to launch " + versions[comboVer.SelectedIndex].Id);
                        }
                    }));
                }));
                MeCore.MainWindow.addTask(gui.setTask("啟動" + versions[comboVer.SelectedIndex].Id).setThread(task));
                MeCore.Config.LastPlayVer = versions[comboVer.SelectedIndex].Id;
                MeCore.Config.Save(null);
            }

        }
        private void butPlayPro_Click(object sender, RoutedEventArgs e)
        {
            ACLogin login = new ACLogin();
            Dispatcher.Invoke(new MethodInvoker(delegate
            {
                login.ShowDialog();
            }));
            if (login.auth != null)
            {
                TaskBar gui = new TaskBar();
                var task = new Thread(new ThreadStart(delegate
                {
                    Dispatcher.Invoke(new MethodInvoker(delegate
                    {
                        gui.setTaskStatus("正在啟動");
                        launcher.GameExit += onGameExit;
                        var result = launcher.Launch(new LaunchOptions
                        {
                            Version = launcher.GetVersion(profiles[comboProfile.SelectedIndex].version),
                            MaxMemory = profiles[comboProfile.SelectedIndex].Xmx,
                            Size = new WindowSize
                            {
                                Width = (ushort?)profiles[comboProfile.SelectedIndex].winSizeX,
                                Height = (ushort?)profiles[comboProfile.SelectedIndex].winSizeY
                            },
                            Authenticator = login.auth
                        });

                        if (!result.Success)
                        {
                            switch (result.ErrorType)
                            {
                                case ErrorType.NoJAVA:
                                    new KnownErrorReport(result.ErrorMessage, Lang.LangManager.GetLangFromResource("JavaFault")).Show();
                                    break;
                                case ErrorType.AuthenticationFailed:
                                    new KnownErrorReport(result.ErrorMessage, Lang.LangManager.GetLangFromResource("AuthFault")).Show();
                                    break;
                                case ErrorType.OperatorException:
                                    new KnownErrorReport(result.ErrorMessage).Show();
                                    break;
                                case ErrorType.UncompressingFailed:
                                    new KnownErrorReport(result.ErrorMessage, Lang.LangManager.GetLangFromResource("LibFault")).Show();
                                    break;
                                case ErrorType.Unknown:
                                    new KnownErrorReport(result.ErrorMessage).Show();
                                    break;
                            }
                        }
                        else
                        {
                            gui.setTaskStatus("");
                            gui.setSubProcess(result.Handle.GetType().GetField("Process", System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance).GetValue(result.Handle) as System.Diagnostics.Process);
                            gui.countTime();
                            MeCore.NIcon.ShowBalloonTip(3000, "Successful to launch " + profiles[comboProfile.SelectedIndex].version);
                        }
                    }));
                }));
                MeCore.MainWindow.addTask(gui.setTask("啟動" + profiles[comboProfile.SelectedIndex].version).setThread(task));
            }
        }
        private void onGameExit(LaunchHandle handle, int code) {
            if (code != 0)
            {
                if (Directory.Exists(launcher.GameRootPath + @"\crash-reports"))
                {
                    if (_clientCrashReportCount != Directory.GetFiles(launcher.GameRootPath + @"\crash-reports").Count())
                    {
                        Logger.log("found new crash report");
                        var clientCrashReportDir = new DirectoryInfo(launcher.GameRootPath + @"\crash-reports");
                        var lastClientCrashReportPath = "";
                        var lastClientCrashReportModifyTime = DateTime.MinValue;
                        foreach (var clientCrashReport in clientCrashReportDir.GetFiles())
                        {
                            if (lastClientCrashReportModifyTime < clientCrashReport.LastWriteTime)
                            {
                                lastClientCrashReportPath = clientCrashReport.FullName;
                            }
                        }
                        var crashReportReader = new StreamReader(lastClientCrashReportPath, Encoding.Default);
                        var s = crashReportReader.ReadToEnd();
                        Logger.log(s, Logger.LogType.Crash);
                        MeCore.Dispatcher.Invoke(new Action(() => new MCCrash(s, lastClientCrashReportPath).Show()));
                        crashReportReader.Close();
                    }
                }
            }
        }
        private void butDown_Click(object sender, RoutedEventArgs e)
        {
            gridBasic.Visibility = Visibility.Hidden;
            gridPro.Visibility = Visibility.Visible;
        }

        private void butUp_Click(object sender, RoutedEventArgs e)
        {
            gridPro.Visibility = Visibility.Hidden;
            gridBasic.Visibility = Visibility.Visible;
        }
        public void LoadVersionList()
        {
            comboVer.SelectedIndex = -1;
            MeCore.MainWindow.gridDL.listVerFLib.SelectedIndex = -1;
            MeCore.MainWindow.gridDL.listVerFAsset.SelectedIndex = -1;
            comboVer.Items.Clear();
            MeCore.MainWindow.gridDL.listVerFLib.Items.Clear();
            MeCore.MainWindow.gridDL.listVerFAsset.Items.Clear();
            try
            {
                launcher = LauncherCore.Create(new LauncherCoreCreationOption(txtBoxP.Text, comboJava.SelectedItem as string));
                versions = launcher.GetVersions().ToArray();
                for (int i = 0; i < versions.Length; i++)
                {
                    comboVer.Items.Add(versions[i].Id);
                    MeCore.MainWindow.gridDL.listVerFLib.Items.Add(versions[i].Id);
                    MeCore.MainWindow.gridDL.listVerFAsset.Items.Add(versions[i].Id);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new MethodInvoker(delegate
                {
                    new KnownErrorReport(ex.Message).Show();
                }));
            }
        }

        public void loadConfig()
        {
            txtBoxP.Text = MeCore.Config.MCPath;
            comboJava.SelectedItem = MeCore.Config.Javaw;
            txtBoxArg.Text = MeCore.Config.ExtraJvmArg;
            sliderRAM.Value = sliderRAMPro.Value = Convert.ToDouble(MeCore.Config.Javaxmx);

        }

        private void butF5Ver_Click(object sender, RoutedEventArgs e)
        {
            LoadVersionList();
        }
        private void butBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            txtBoxP.Text = dialog.SelectedPath;
            MeCore.Config.MCPath = dialog.SelectedPath;
            MeCore.Config.Save(null);
        }

        private void butF5Profile_Click(object sender, RoutedEventArgs e)
        {
            loadProfile(Environment.CurrentDirectory + "\\profiles.xml");
        }

        private void loadProfile(string path)
        {
            comboProfile.SelectedIndex = -1;
            comboProfile.Items.Clear();
            xmlLoader = new ProfileInXML();
            profiles = xmlLoader.readProfile(path);
            if (profiles != null){
                for (int i = 0; i < profiles.Length; i++)
                {
                    comboProfile.Items.Add(profiles[i].name);
                }
            }
        }
        private void butEdit_Click(object sender, RoutedEventArgs e)
        {
            new ProfileEditor(Environment.CurrentDirectory + "\\profiles.xml").Show();
        }
        public void setLblColor(Color color) {
            lblArg.Foreground = new SolidColorBrush(color);
            lblJava.Foreground = new SolidColorBrush(color);
            lblMM.Foreground = new SolidColorBrush(color);
            lblMM_Copy.Foreground = new SolidColorBrush(color);
            lblMP.Foreground = new SolidColorBrush(color);
            lblProfile.Foreground = new SolidColorBrush(color);
            lblVer.Foreground = new SolidColorBrush(color);
        }

        private void sliderRAM_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MeCore.Config.Javaxmx = sliderRAM.Value.ToString();
            MeCore.Config.Save(null);
        }

        private void sliderRAMPro_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MeCore.Config.Javaxmx = sliderRAMPro.Value.ToString();
            MeCore.Config.Save(null);
        }
    }
}
