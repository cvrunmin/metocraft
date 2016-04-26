using Microsoft.Win32;
using MTMCL.Forge;
using MTMCL.Lang;
using MTMCL.Task;
using MTMCL.util;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

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
                    MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLangFromResource("SubTaskInstallForge"))));
                    new ForgeInstaller().install(a);
                    MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLangFromResource("TaskFinish"))));
                    task.noticeFinished();
                }
                catch (Exception ex)
                {
                    MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), ex.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                    Logger.log(ex);
                    task.noticeFailed();
                }
            }));
            MeCore.MainWindow.addTask("instl-forgeclient", task.setThread(thDL).setTask(LangManager.GetLangFromResource("TaskInstallForge")).setDetectAlive(false));
        }

        private void butBF_Click(object sender, RoutedEventArgs e)
        {
            FileDialog dialog = new OpenFileDialog();
            dialog.Filter = "MinecraftForge Installer|*.jar|All Files|*.*";
            dialog.ShowDialog();
            txtPathF.Text = dialog.FileName;
        }
    }
}
