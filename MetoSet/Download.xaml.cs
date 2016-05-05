using MTMCL.Forge;
using MTMCL.Lang;
using MTMCL.Task;
using MTMCL.util;
using MTMCL.Versions;
using Newtonsoft.Json;
using System;
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
        bool doneInit;
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

        private void butReloadMC_Click(object sender, RoutedEventArgs e)
        {
            ReloadVanillaVersion();
        }
        private void ReloadVanillaVersion() {
            butReloadMC.IsEnabled = false;
            gridMCRFail.Visibility = Visibility.Collapsed;
            gridMCRing.Visibility = Visibility.Visible;
            listRemoteVer.DataContext = null;
            var rawJson = new DataContractJsonSerializer(typeof(RawVersionListType));
            var getJson = (HttpWebRequest)WebRequest.Create(MTMCL.Resources.UrlReplacer.getVersionsUrl());
            getJson.Timeout = 10000;
            getJson.ReadWriteTimeout = 10000;
            getJson.UserAgent = "MTMCL" + MeCore.version;
            var thGet = new Thread(new ThreadStart(delegate
            {
                try
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(async delegate
                    {
                        butReloadMC.Content = LangManager.GetLangFromResource("RemoteVerGetting");
                        if (MeCore.Config.DownloadSource == 1)
                        {
                            await TaskEx.Delay(TimeSpan.FromSeconds(1));
                        }
                    }));
                    var getJsonAns = (HttpWebResponse)getJson.GetResponse();
                    // ReSharper disable once AssignNullToNotNullAttribute
                    var remoteVersion = rawJson.ReadObject(getJsonAns.GetResponseStream()) as RawVersionListType;
                    var dt = new DataTable();
                    dt.Columns.Add("Ver");
                    dt.Columns.Add("RelTime", typeof(DateTime));
                    dt.Columns.Add("Type");
                    dt.Columns.Add("Url");
                    if (remoteVersion != null)
                        foreach (RemoteVerType rv in remoteVersion.getVersions())
                        {
                            dt.Rows.Add(new object[] { rv.id, DateTime.Parse(rv.releaseTime), rv.type, rv.url });
                        }
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        gridMCRing.Visibility = Visibility.Collapsed;
                        butReloadMC.Content = LangManager.GetLangFromResource("Reload");
                        butReloadMC.IsEnabled = true;
                        listRemoteVer.DataContext = dt;
                        listRemoteVer.Items.SortDescriptions.Add(new SortDescription("RelTime", ListSortDirection.Descending));
                    }));
                }
                catch (WebException ex)
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        gridMCRing.Visibility = Visibility.Collapsed;
                        gridMCRFail.Visibility = Visibility.Visible;
                        Dispatcher.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), LangManager.GetLangFromResource("RemoteVerFailedTimeout"), ex.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                        butReloadMC.Content = LangManager.GetLangFromResource("Reload");
                        butReloadMC.IsEnabled = true;
                    }));
                }
                catch (TimeoutException ex)
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        gridMCRing.Visibility = Visibility.Collapsed;
                        gridMCRFail.Visibility = Visibility.Visible;
                        MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), LangManager.GetLangFromResource("RemoteVerFailedTimeout"), ex.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) });
                        butReloadMC.Content = LangManager.GetLangFromResource("Reload");
                        butReloadMC.IsEnabled = true;
                    }));
                }
                catch (Exception ex) {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        gridMCRing.Visibility = Visibility.Collapsed;
                        gridMCRFail.Visibility = Visibility.Visible;
                        MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat")), ex.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) });
                        butReloadMC.Content = LangManager.GetLangFromResource("Reload");
                        butReloadMC.IsEnabled = true;
                    }));
                }
            }));
            thGet.Start();
        }

        private void butDLMC_Click(object sender, RoutedEventArgs e)
        {
            downloadVVer();
        }
        private void downloadVVer()
        {
            if (listRemoteVer.SelectedItems == null)
            {
                MessageBox.Show(LangManager.GetLangFromResource("RemoteVerErrorNoVersionSelect"));
                return;
            }
            DataRowView selectVer = listRemoteVer.SelectedItem as DataRowView;
            if (selectVer != null)
            {
                TaskListBar taskbar = new TaskListBar() { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/download-banner.jpg")) };
                var task = new Thread(new ThreadStart(async delegate
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(async delegate
                    {
                        if (MeCore.Config.DownloadSource == 1)
                        {
                            await TaskEx.Delay(TimeSpan.FromSeconds(1));
                        }
                    }));
                    var selectver = selectVer[0] as string;
                    var downpath = new StringBuilder(MeCore.Config.MCPath + @"\versions\");
                    downpath.Append(selectver).Append("\\");
                    downpath.Append(selectver).Append(".jar");
                    if (MeCore.IsServerDedicated)
                    {
                        if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                        {
                            downpath.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                        }
                    }
                    var downer = new WebClient();
                    downer.Headers.Add("User-Agent", "MTMCL" + MeCore.version);
                    var downurl = new StringBuilder(MTMCL.Resources.UrlReplacer.getDownloadUrl());
                    downurl.Append(@"versions\");
                    downurl.Append(selectver).Append("\\");
                    downurl.Append(selectver).Append(".jar");
#if DEBUG
                    MessageBox.Show(downpath + "\n" + downurl);
#endif
                    if (!Directory.Exists(Path.GetDirectoryName(downpath.ToString())))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(downpath.ToString()));
                    }
                    string downjsonfile = downurl.ToString().Substring(0, downurl.Length - 4) + ".json";
                    if (selectVer[3] != null & MeCore.Config.DownloadSource == 0)
                    {
                        downjsonfile = selectVer[3] as string;
                    }
                    string downjsonpath = downpath.ToString().Substring(0, downpath.Length - 4) + ".json";
                    try
                    {
                        downer.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e)
                        {
                            taskbar.log(Logger.HelpLog("Success to download client file."));
                            taskbar.noticeFinished();
                        };
                        downer.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
                        {
                            Dispatcher.Invoke(new Action(() => taskbar.setTaskStatus(e.ProgressPercentage + "%")));
                        };
                        taskbar.log(Logger.HelpLog("Start download file from url " + downjsonfile));
                        try
                        {
                            downer.DownloadFile(new Uri(downjsonfile), downjsonpath);
                        }
                        catch (Exception)
                        {
                            taskbar.noticeFailed();
                            return;
                        }
                        taskbar.log(Logger.HelpLog("Finish downloading file " + downjsonfile));
                        await TaskEx.Delay(TimeSpan.FromMilliseconds(500));
                        taskbar.log(Logger.HelpLog(string.Format("Start reading json of version {0} for further downloading", selectver)));
                        var sr = new StreamReader(downjsonpath);
                        VersionJson ver = JsonConvert.DeserializeObject<VersionJson>(sr.ReadToEnd());
                        sr.Close();
                        var sw = new StreamWriter(downjsonpath);
                        sw.Write(JsonConvert.SerializeObject(ver.Simplify(), Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
                        sw.Close();
                        if (ver.downloads != null)
                        {
                            if (ver.downloads.client != null)
                            {
                                if (ver.downloads.client.url != null)
                                {
                                    downurl.Clear().Append(ver.downloads.client.url);
                                }
                            }

                        }
                        taskbar.log(Logger.HelpLog("Start download file from url " + downurl.ToString()));
                        downer.DownloadFileAsync(new Uri(downurl.ToString()), downpath.ToString());
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                        {
                            Dispatcher.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), ex.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                            taskbar.noticeFailed();
                        }));
                    }
                }));
                MeCore.MainWindow.addTask("dl-mcclient-" + selectVer[0] as string, taskbar.setThread(task).setTask(LangManager.GetLangFromResource("TaskDLMC")).setDetectAlive(false));
            }
        }
        readonly ForgeVersionList _forgeVer = new ForgeVersionList();
        private void butReloadForge_Click(object sender, RoutedEventArgs e)
        {
            ((AccessText)butReloadForge.Content).Text = LangManager.GetLangFromResource("RemoteVerGetting");
            butReloadForge.IsEnabled = false;
            gridMFRFail.Visibility = Visibility.Collapsed;
            gridMFRing.Visibility = Visibility.Visible;
            RefreshForgeVersionList();
        }
        private void RefreshForgeVersionList()
        {
            _forgeVer.ForgePageReadyEvent += ForgeVer_ForgePageReadyEvent;
            _forgeVer.GetVersion();
        }
        void ForgeVer_ForgePageReadyEvent()
        {
            var dt = new DataTable();
            dt.Columns.Add("Ver");
            dt.Columns.Add("MCVer");
            dt.Columns.Add("Time", typeof(DateTime));
            dt.Columns.Add("Tag");
            var fl = _forgeVer.GetNew();
            if (fl == null) {
                Dispatcher.Invoke(new Action(() => {
                    gridMFRing.Visibility = Visibility.Collapsed;
                    gridMFRFail.Visibility = Visibility.Visible;
                }));
            }
            if (fl.Length == 0)
            {
                Dispatcher.Invoke(new Action(() => {
                    gridMFRing.Visibility = Visibility.Collapsed;
                    gridMFRFail.Visibility = Visibility.Visible;
                }));
            }
            else {
                foreach (object[] t in _forgeVer.GetNew())
                {
                    dt.Rows.Add(t[0], t[1], t[2], t[3]);
                }
            }
            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(() =>
            {
                listForge.DataContext = dt;
                gridMFRing.Visibility = Visibility.Collapsed;
                ((AccessText)butReloadForge.Content).Text = LangManager.GetLangFromResource("Reload");
                butReloadForge.IsEnabled = true;
            }));
        }
        private void DownloadForge(string ver)
        {
            if (!_forgeVer.ForgeDownloadUrl.ContainsKey(ver))
            {
                MessageBox.Show(LangManager.GetLangFromResource("ForgeDoNotSupportInstaller"));
                return;
            }
            TaskListBar task = new TaskListBar() { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/download-banner.jpg")) };
            var thDL = new Thread(new ThreadStart(delegate
            {
                var url = new Uri(_forgeVer.ForgeDownloadUrl[ver]);
                var downer = new WebClient();
                downer.Headers.Add("User-Agent", "MTMCL" + MeCore.version);
                var filename = "forge.jar";
                var filecount = 0;
                while (File.Exists(filename))
                {
                    ++filecount;
                    filename = "forge-" + filecount + ".jar";
                }
                downer.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
                {
                    MeCore.Invoke(new Action(() => task.setTaskStatus(string.Format(LangManager.GetLangFromResource("SubTaskDLForge"), e.ProgressPercentage))));
                };
                downer.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e)
                {
                    try
                    {
                        task.log(Logger.HelpLog("Trying to install forge"));
                        MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLangFromResource("SubTaskInstallForge"))));
                        new ForgeInstaller().install(filename);
                        File.Delete(filename);
                        task.log(Logger.HelpLog("Installation finished"));
                        MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLangFromResource("TaskFinish"))));
                        task.noticeFinished();
                    }
                    catch (Exception ex)
                    {
                        task.log(Logger.HelpLog("Installation failed"));
                        Logger.log(ex);
                        task.noticeFailed();
                    }
                };
                task.log(Logger.HelpLog("Start downloading forge installer"));
                MeCore.Invoke(new Action(() => task.setTaskStatus(string.Format(LangManager.GetLangFromResource("SubTaskDLForge"), "0"))));
                downer.DownloadFileAsync(url, filename);
            }));
            MeCore.MainWindow.addTask("dl-instl-forgeclient-" + ver, task.setThread(thDL).setTask(LangManager.GetLangFromResource("TaskInstallForge")).setDetectAlive(false));
        }

        private void butDLForge_Click(object sender, RoutedEventArgs e)
        {
            if (listForge.SelectedItem == null)
                return;
            DataRowView selectVer = listForge.SelectedItem as DataRowView;
            DownloadForge(selectVer[0] as string);
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
                        Uri url = new Uri(txtboxUrl.Text);
                        if (url == null | string.IsNullOrWhiteSpace(url.AbsoluteUri))
                        {
                            throw new InvalidOperationException("null url");
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
                            MeCore.Invoke(new Action(() => task.setTaskStatus(string.Format(LangManager.GetLangFromResource("SubTaskDLModPack"), e1.ProgressPercentage))));
                        };
                        downer.DownloadFileCompleted += delegate (object sender1, AsyncCompletedEventArgs e1)
                        {
                            MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLangFromResource("SubTaskExtractModPack"))));
                            new Threads.ModPackProcesser().install(filename);
                            File.Delete(filename);
                            MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLangFromResource("TaskFinish"))));
                            task.noticeFinished();
                        };
                        MeCore.Invoke(new Action(() => task.setTaskStatus(string.Format(LangManager.GetLangFromResource("SubTaskDLModPack"), 0))));
                        downer.DownloadFileAsync(url, filename);
                    }));
                }
                catch (Exception ex)
                {
                    MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), ex.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg"))})));
                    MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLangFromResource("TaskFail"))));
                    task.noticeFailed();
                }
            }));
            MeCore.MainWindow.addTask("dl-modpack", task.setThread(thDL).setTask(LangManager.GetLangFromResource("TaskDLModPack")).setDetectAlive(false));
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!doneInit)
            {
                if (MeCore.IsServerDedicated)
                {
                    LoadServerDeDicatedVersion();
                }
                ReloadVanillaVersion();
                RefreshForgeVersionList();
                doneInit = true;
            }
        }
        private void LoadServerDeDicatedVersion()
        {
            if (MeCore.Config.Server.NeedServerPack & !string.IsNullOrWhiteSpace(MeCore.Config.Server.ServerPackUrl))
            {
                tabDLPack.Visibility = Visibility.Visible;
                txtboxUrl.Text = MeCore.Config.Server.ServerPackUrl;
            }
            if (!MeCore.Config.Server.AllowSelfDownloadClient)
            {
                tabDLMC.Visibility = Visibility.Collapsed;
                tabDLForge.Visibility = Visibility.Collapsed;
            }
        }
    }
}
