using Microsoft.Win32;
using MTMCL.Forge;
using MTMCL.Lang;
using MTMCL.Task;
using MTMCL.util;
using MTMCL.Versions;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Linq;

namespace MTMCL.Install
{
    /// <summary>
    /// Install.xaml 的互動邏輯
    /// </summary>
    public partial class GridInstall
    {
        public GridInstall()
        {
            InitializeComponent();
        }
        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            MeCore.MainWindow.gridMain.Visibility = Visibility.Visible;
            MeCore.MainWindow.gridOthers.Visibility = Visibility.Collapsed;
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridMain.BeginAnimation(OpacityProperty, ani);
        }

        private void butInstallF_Click(object sender, RoutedEventArgs e)
        {
            TaskListBar task = new TaskListBar() { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/download-banner.jpg")) };
            var thDL = new Thread(new ThreadStart(delegate
            {
                try
                {
                    string a = "";
                    Dispatcher.Invoke(new Action(() => a = txtPathF.Text));
                    MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLocalized("SubTaskInstallForge"))));
                    new ForgeInstaller().install(a);
                    MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLocalized("TaskFinish"))));
                    task.noticeFinished();
                }
                catch (Exception ex)
                {
                    MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLocalized("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), ex.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                    Logger.log(ex);
                    task.noticeFailed();
                }
            }));
            MeCore.MainWindow.addTask("instl-forgeclient", task.setThread(thDL).setTask(LangManager.GetLocalized("TaskInstallForge")).setDetectAlive(false));
            MeCore.MainWindow.addBalloonNotice(new Notice.NoticeBalloon(LangManager.GetLocalized("Download"), string.Format(LangManager.GetLocalized("BalloonNoticeSTTaskFormat"), LangManager.GetLocalized("TaskInstallForge"))));
        }

        private void butBF_Click(object sender, RoutedEventArgs e)
        {
            FileDialog dialog = new OpenFileDialog();
            dialog.Filter = "MinecraftForge Installer|*.jar;*.exe|All Files|*.*";
            dialog.ShowDialog();
            txtPathF.Text = dialog.FileName;
        }

        private void butInstallLL_Click(object sender, RoutedEventArgs e)
        {
            TaskListBar task = new TaskListBar() { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/download-banner.jpg")) };
            var thDL = new Thread(new ThreadStart(delegate
            {
                try
                {
                    string a = "", b = "";
                    bool c = false;
                    Dispatcher.Invoke(new Action(() => { a = txtPathLL.Text; b = comboIVer.SelectedItem as string; c = (bool)toggleWF.IsChecked; }));
                    MeCore.Invoke(new Action(() => task.setTaskStatus(string.Format(LangManager.GetLocalized("SubTaskInstallMod"), "LiteLoader"))));
                    new LiteLoaderInstaller(c, b).install(a);
                    MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLocalized("TaskFinish"))));
                    task.noticeFinished();
                }
                catch (Exception ex)
                {
                    MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLocalized("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), ex.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                    Logger.log(ex);
                    task.noticeFailed();
                }
            }));
            MeCore.MainWindow.addTask("instl-liteloader", task.setThread(thDL).setTask(string.Format(LangManager.GetLocalized("TaskInstallMod"), "LiteLoader")).setDetectAlive(false));
            MeCore.MainWindow.addBalloonNotice(new Notice.NoticeBalloon(LangManager.GetLocalized("Download"), string.Format(LangManager.GetLocalized("BalloonNoticeSTTaskFormat"), string.Format(LangManager.GetLocalized("TaskInstallMod"), "LiteLoader"))));
        }

        private void butBLL_Click(object sender, RoutedEventArgs e)
        {
            FileDialog dialog = new OpenFileDialog();
            dialog.Filter = "LiteLoader Installer|*.jar;*.exe|All Files|*.*";
            dialog.ShowDialog();
            txtPathLL.Text = dialog.FileName;
        }

        private void toggleWF_IsCheckedChanged(object sender, EventArgs e)
        {
            var ani = new DoubleAnimation() { To = (bool)toggleWF.IsChecked ? 200 : 0,EasingFunction = new ExponentialEase() { Exponent = 9},Duration = TimeSpan.FromSeconds(0.5) };
            gridIVer.BeginAnimation(WidthProperty, ani);
        }
        public VersionJson[] versions;

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            string path = MeCore.Config.MCPath;
            if (MeCore.IsServerDedicated)
            {
                if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                {
                    path = path.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                }
            }
            if (!string.IsNullOrWhiteSpace(path))
            {
                //App.core = LauncherCore.Create(new LauncherCoreCreationOption(path, MeCore.Config.Javaw, new KMCCC.Modules.JVersion.NewJVersionLocator()));
                versions = VersionReader.GetVersion(MeCore.Config.MCPath).Where(vid=> vid.id.Contains("forge") | vid.id.Contains("Forge")).ToArray();
                foreach (var version in versions)
                {
                    comboIVer.Items.Add(version.id);
                }
            }
        }
    }
}
