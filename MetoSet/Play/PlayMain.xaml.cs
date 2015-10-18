using KMCCC.Launcher;
using MetoCraft.NewGui;
using MetoCraft.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetoCraft.Play
{
    /// <summary>
    /// PlayMain.xaml 的互動邏輯
    /// </summary>
    public partial class PlayMain : Grid
    {
        public static LauncherCore launcher;
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
        private Profile.Profile[] profiles;
        private string _path = Environment.CurrentDirectory + "\\profiles.xml";
        private ProfileInXML xmlLoader;
        public PlayMain()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var javas = KMCCC.Tools.SystemTools.FindJava().ToList();
            foreach (var java in javas)
            {
                comboJava.Items.Add(java);
            }
        }
        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
            ACLogin login = new ACLogin();
            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
            {
                login.ShowDialog();
            }));
            if (login.auth != null)
            {
                //                new Assets.Assets(MeCore.MainWindow.gridPlay.gridVer.versions[MeCore.MainWindow.gridPlay.gridVer.listBoxVer.SelectedIndex]);
                TaskGui gui = new TaskGui();
                var task = new Thread(new ThreadStart(delegate {
                        Dispatcher.Invoke(new MethodInvoker(delegate
                        {
                            gui.setTaskStatus("正在啟動");
                        var result = launcher.Launch(new LaunchOptions
                        {
                            Version = versions[comboVer.SelectedIndex],
                            MaxMemory = (int)sliderRAM.Value,
                            Authenticator = login.auth
                        });
                        if (!result.Success)
                        {
                            switch (result.ErrorType)
                            {
                                case ErrorType.NoJAVA:
                                        new ErrorReport("Your Java in OS is abnormal, please try to reinstall Java", result.ErrorMessage).Show();
                                    break;
                                case ErrorType.AuthenticationFailed:
                                        new ErrorReport("Cannot login. Please check your email and password are correct or not", result.ErrorMessage).Show();
                                    break;
                                case ErrorType.OperatorException:
                                        new ErrorReport("Operator Exception.", result.ErrorMessage).Show();
                                    break;
                                case ErrorType.UncompressingFailed:
                                        new ErrorReport("Uncompressing failed. Please check your libraries are intect or not", result.ErrorMessage).Show();
                                    break;
                                case ErrorType.Unknown:
                                        new ErrorReport("Unknown Exception", result.ErrorMessage).Show();
                                    break;
                            }
                        }
                        else
                        {
                            gui.setTaskStatus("");
                            gui.countTime();
                            MeCore.NIcon.ShowBalloonTip(3000, "Susceesful to launch " + versions[comboVer.SelectedIndex].Id);
                        }
                    }));
                }));
                gui.setTask("啟動" + versions[comboVer.SelectedIndex].Id).setThread(task).Show();
            }

        }

        private void butDown_Click(object sender, RoutedEventArgs e)
        {
            var mover = new ThicknessAnimationUsingKeyFrames();
            mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0)));
            mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, -(ActualHeight), 0, (ActualHeight)), TimeSpan.FromSeconds(0.2)));
            var mover1 = new ThicknessAnimationUsingKeyFrames();
            mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, (ActualHeight), 0, -(ActualHeight)), TimeSpan.FromSeconds(0)));
            mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0.2)));
            gridBasic.BeginAnimation(MarginProperty, mover);
            gridPro.BeginAnimation(MarginProperty, mover1);
            gridBasic.Margin = new Thickness(0, -(ActualHeight), 0, (ActualHeight));
            gridPro.Margin = new Thickness(0);
        }

        private void butUp_Click(object sender, RoutedEventArgs e)
        {
            var mover = new ThicknessAnimationUsingKeyFrames();
            mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, -(ActualHeight), 0, (ActualHeight)), TimeSpan.FromSeconds(0)));
            mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0.2)));
            var mover1 = new ThicknessAnimationUsingKeyFrames();
            mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0)));
            mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, (ActualHeight), 0, -(ActualHeight)), TimeSpan.FromSeconds(0.2)));
            gridBasic.BeginAnimation(MarginProperty, mover);
            gridPro.BeginAnimation(MarginProperty, mover1);
            gridBasic.Margin = new Thickness(0);
            gridPro.Margin = new Thickness(0, (ActualHeight), 0, -(ActualHeight));
        }
        public void LoadVersionList()
        {
            comboVer.SelectedIndex = -1;
//            MeCore.MainWindow.gridDL.gridLib.listVer.SelectedIndex = -1;
//            MeCore.MainWindow.gridDL.gridAsset.listVer.SelectedIndex = -1;
            comboVer.Items.Clear();
//            MeCore.MainWindow.gridDL.gridLib.listVer.Items.Clear();
//            MeCore.MainWindow.gridDL.gridAsset.listVer.Items.Clear();
            try
            {
                launcher = LauncherCore.Create(new LauncherCoreCreationOption(txtBoxP.Text, comboJava.SelectedItem as string));
                versions = PlayMain.launcher.GetVersions().ToArray();
                for (int i = 0; i < versions.Length; i++)
                {
                    comboVer.Items.Add(versions[i].Id);
//                    MeCore.MainWindow.gridDL.gridLib.listVer.Items.Add(versions[i].Id);
//                    MeCore.MainWindow.gridDL.gridAsset.listVer.Items.Add(versions[i].Id);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    new ErrorReport(ex).Show();
                }));
            }
        }

        public void loadConfig()
        {
            txtBoxP.Text = MeCore.Config.MCPath;
            comboJava.SelectedItem = MeCore.Config.Javaw;
            txtBoxArg.Text = MeCore.Config.ExtraJvmArg;
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
                var result = PlayMain.launcher.Launch(new LaunchOptions
                {
                    Version = PlayMain.launcher.GetVersion(profiles[comboProfile.SelectedIndex].version),
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
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                new ErrorReport("Your Java in OS is abnormal, please try to reinstall Java", result.ErrorMessage).Show();
                            }));
                            break;
                        case ErrorType.AuthenticationFailed:
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                new ErrorReport("Cannot login. Please check your email and password are correct or not", result.ErrorMessage).Show();
                            }));
                            break;
                        case KMCCC.Launcher.ErrorType.OperatorException:
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                new ErrorReport("Operator Exception.", result.ErrorMessage).Show();
                            }));
                            break;
                        case KMCCC.Launcher.ErrorType.UncompressingFailed:
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                new ErrorReport("Uncompressing failed. Please check your libraries are intect or not", result.ErrorMessage).Show();
                            }));
                            break;
                        case KMCCC.Launcher.ErrorType.Unknown:
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                new ErrorReport("Unknown Exception", result.ErrorMessage).Show();
                            }));
                            break;
                    }
                }
                else
                {
                    MeCore.NIcon.ShowBalloonTip(3000, "Susceesful to launch" + profiles[comboProfile.SelectedIndex].version);
                }
            }
        }
        private void butF5Profile_Click(object sender, RoutedEventArgs e)
        {
            loadProfile(_path);
        }

        private void loadProfile(string path)
        {
            comboProfile.SelectedIndex = -1;
            comboProfile.Items.Clear();
            xmlLoader = new ProfileInXML();
            profiles = xmlLoader.readProfile(path);
            if (profiles != null)
            {
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
    }
}
