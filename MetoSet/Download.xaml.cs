using MTMCL.Forge;
using MTMCL.Lang;
using MTMCL.Versions;
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

namespace MTMCL
{
    /// <summary>
    /// Download.xaml 的互動邏輯
    /// </summary>
    public partial class Download : Grid
    {
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
                            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1));
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
                        //new KnownErrorReport(LangManager.GetLangFromResource("RemoteVerFailedTimeout") + " : " + ex.Message, LangManager.GetLangFromResource("NoConnectionSolve")).Show();
                        butReloadMC.Content = LangManager.GetLangFromResource("Reload");
                        butReloadMC.IsEnabled = true;
                    }));
                }
                catch (TimeoutException ex)
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        //new KnownErrorReport(LangManager.GetLangFromResource("RemoteVerFailedTimeout") + " : " + ex.Message, LangManager.GetLangFromResource("NoConnectionSolve")).Show();
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
                //NewGui.TaskBar taskbar = new NewGui.TaskBar();
                var task = new Thread(new ThreadStart(async delegate
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(async delegate
                    {
                        if (MeCore.Config.DownloadSource == 1)
                        {
                            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    }));
                    var selectver = selectVer[0] as string;
                    var downpath = new StringBuilder(MeCore.Config.MCPath + @"\versions\");
                    downpath.Append(selectver).Append("\\");
                    downpath.Append(selectver).Append(".jar");
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
                            Logger.log("Success to download client file.");
                            //taskbar.noticeFinished();
                            //Dispatcher.Invoke(new Action(() => MeCore.MainWindow.gridPlay.LoadVersionList()));
                        };
                        downer.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
                        {
                            //Dispatcher.Invoke(new Action(() => taskbar.setTaskStatus(e.ProgressPercentage + "%")));
                        };
                        Logger.log("download:" + downjsonfile);
                        try
                        {
                            downer.DownloadFile(new Uri(downjsonfile), downjsonpath);
                        }
                        catch (Exception)
                        {
                            //taskbar.noticeFailed();
                            return;
                        }
                        await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(500));
                        var sr = new StreamReader(downjsonpath);
                        VersionJson ver = LitJson.JsonMapper.ToObject<VersionJson>(sr);
                        sr.Close();
                        var jw = new LitJson.JsonWriter(new StreamWriter(downjsonpath));
                        jw.PrettyPrint = true;
                        LitJson.JsonMapper.ToJson(ver.Simplify(), jw);
                        jw.TextWriter.Close();
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
                        Logger.log("download:" + downurl.ToString());
                        downer.DownloadFileAsync(new Uri(downurl.ToString()), downpath.ToString());
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                        {
                            //new ErrorReport(ex).Show();
                            //taskbar.noticeFailed();
                        }));
                    }
                }));
                task.Start();
                //MeCore.MainWindow.addTask(taskbar.setThread(task).setTask(LangManager.GetLangFromResource("TaskDLMC")).setDetectAlive(false), "dl-mcclient-" + selectVer[0] as string);
            }
        }
        readonly ForgeVersionList _forgeVer = new ForgeVersionList();
        private void butReloadForge_Click(object sender, RoutedEventArgs e)
        {
            butReloadForge.Content = LangManager.GetLangFromResource("RemoteVerGetting");
            butReloadForge.IsEnabled = false;
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
            if (_forgeVer.GetNew() != null)
                foreach (object[] t in _forgeVer.GetNew())
                {
                    dt.Rows.Add(new object[] { t[0], t[1], t[2], t[3] });
                }
            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(() =>
            {
                listForge.DataContext = dt;
                butReloadForge.Content = LangManager.GetLangFromResource("Reload");
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
            //NewGui.TaskBar task = new NewGui.TaskBar();
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
                    //MeCore.Invoke(new Action(() => task.setTaskStatus(string.Format(LangManager.GetLangFromResource("SubTaskDLForge"), e.ProgressPercentage))));
                };
                downer.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e)
                {
                    try
                    {
                        //MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLangFromResource("SubTaskInstallForge"))));
                        new ForgeInstaller().install(filename);
                        File.Delete(filename);
                        //MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLangFromResource("TaskFinish"))));
                        //task.noticeFinished();
                    }
                    catch (Exception ex)
                    {
                        Logger.log(ex);
                        //task.noticeFailed();
                    }
                };
                //MeCore.Invoke(new Action(() => task.setTaskStatus(string.Format(LangManager.GetLangFromResource("SubTaskDLForge"), "0"))));
                downer.DownloadFileAsync(url, filename);
            }));
            //MeCore.MainWindow.addTask(task.setThread(thDL).setTask(LangManager.GetLangFromResource("TaskInstallForge")).setDetectAlive(false), "dl-instl-forgeclient-" + ver);
            thDL.Start();
        }

        private void butDLForge_Click(object sender, RoutedEventArgs e)
        {
            if (listForge.SelectedItem == null)
                return;
            DataRowView selectVer = listForge.SelectedItem as DataRowView;
            DownloadForge(selectVer[0] as string);
        }
    }
}
