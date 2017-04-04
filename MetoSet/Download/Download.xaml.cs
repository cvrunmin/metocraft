using MTMCL.Forge;
using MTMCL.Lang;
using MTMCL.Task;
using MTMCL.util;
using MTMCL.Versions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MTMCL
{
    /// <summary>
    /// Download.xaml 的互動邏輯
    /// </summary>
    public partial class Download : Grid
    {
        bool doneInit, doneForgeInit;
        public Download()
        {
            InitializeComponent();
        }

        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }
        private void Back()
        {
            MeCore.MainWindow.gridMain.Visibility = Visibility.Visible;
            MeCore.MainWindow.gridOthers.Visibility = Visibility.Collapsed;
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridMain.BeginAnimation(OpacityProperty, ani);
        }
        
        private void butDLPack_Click(object sender, RoutedEventArgs e)
        {
            TaskListBar task = new TaskListBar() { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/download-banner.jpg")) };
            var thDL = new Thread(new ThreadStart(delegate
            {
                try
                {
                    MeCore.Invoke(new System.Windows.Forms.MethodInvoker(() =>
                    {
                        Uri url = new Uri(MeCore.Config.Server.ServerPackUrl);
                        if (url == null | string.IsNullOrWhiteSpace(url.AbsoluteUri))
                        {
                            throw new InvalidOperationException("url");
                        }
                        var downer = new WebClient();
                        downer.Headers.Add("User-Agent", "MTMCL" + MeCore.version);
                        var filename = "pack.zip";
                        var filecount = 0;
                        while (File.Exists(filename))
                        {
                            ++filecount;
                            filename = "pack-" + filecount + ".zip";
                        }
                        downer.DownloadProgressChanged += delegate (object sender1, DownloadProgressChangedEventArgs e1)
                        {
                            MeCore.Invoke(new Action(() => task.setTaskStatus(string.Format(LangManager.GetLocalized("SubTaskDLModPack"), e1.ProgressPercentage))));
                        };
                        downer.DownloadFileCompleted += delegate (object sender1, AsyncCompletedEventArgs e1)
                        {
                            MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLocalized("SubTaskExtractModPack"))));
                            new Threads.ModPackProcesser().install(filename);
                            File.Delete(filename);
                            MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLocalized("TaskFinish"))));
                            task.noticeFinished();
                        };
                        MeCore.Invoke(new Action(() => task.setTaskStatus(string.Format(LangManager.GetLocalized("SubTaskDLModPack"), 0))));
                        downer.DownloadFileAsync(url, filename);
                    }));
                }
                catch (Exception ex)
                {
                    MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLocalized("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), ex.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg"))})));
                    MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLocalized("TaskFail"))));
                    task.noticeFailed();
                }
            }));
            MeCore.MainWindow.addTask("dl-modpack", task.setThread(thDL).setTask(LangManager.GetLocalized("TaskDLModPack")).setDetectAlive(false));
            MeCore.MainWindow.addBalloonNotice(new Notice.NoticeBalloon(LangManager.GetLocalized("Download"), string.Format(LangManager.GetLocalized("BalloonNoticeSTTaskFormat"), LangManager.GetLocalized("TaskDLModPack"))));
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!doneInit)
            {
                if (MeCore.IsServerDedicated)
                {
                    LoadServerDeDicatedVersion();
                }
                doneInit = true;
            }
        }
        private void LoadServerDeDicatedVersion()
        {
            if (MeCore.Config.Server.NeedServerPack & !string.IsNullOrWhiteSpace(MeCore.Config.Server.ServerPackUrl))
            {
                //tabDLPack.Visibility = Visibility.Visible;
                //txtboxUrl.Text = MeCore.Config.Server.ServerPackUrl;
            }
            if (!MeCore.Config.Server.AllowSelfDownloadClient)
            {
                butMC.Visibility = Visibility.Collapsed;
                butForge.Visibility = Visibility.Collapsed;
            }
        }

        GridMCDL gridmc;
        GridForgeDLMain gridforge;
        private void button_Click(object sender, RoutedEventArgs e)
        {
            MeCore.MainWindow.gridOthers.Children.Clear();
            MeCore.MainWindow.gridOthers.Children.Add(gridmc ?? (gridmc = new GridMCDL(this)));
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridOthers.BeginAnimation(OpacityProperty, ani);
        }

        private void button1_Click (object sender, RoutedEventArgs e)
        {
            MeCore.MainWindow.gridOthers.Children.Clear();
            MeCore.MainWindow.gridOthers.Children.Add(gridforge ?? (gridforge = new GridForgeDLMain(this)));
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridOthers.BeginAnimation(OpacityProperty, ani);
        }

        public async Task<bool> DirectDownloadMC(string version)
        {
            button_Click(this, new RoutedEventArgs());
            return await gridmc.DirectDownload(version);
        }
    }
}
