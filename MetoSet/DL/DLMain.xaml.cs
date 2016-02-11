using MTMCL.Assets;
using MTMCL.Forge;
using MTMCL.Lang;
using MTMCL.util;
using MTMCL.Versions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MTMCL.DL
{
    /// <summary>
    /// DLMain.xaml 的互動邏輯
    /// </summary>
    public partial class DLMain : Grid
    {
        private readonly WebClient _downer = new WebClient();
        public DLMain()
        {
            InitializeComponent();
        }
        #region DLMC
        private void butF5MC_Click(object sender, RoutedEventArgs e)
        {
            butRefresh.IsEnabled = false;
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
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate { butRefresh.Content = LangManager.GetLangFromResource("RemoteVerGetting"); }));
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
                        butRefresh.Content = LangManager.GetLangFromResource("Refresh");
                        butRefresh.IsEnabled = true;
                        listRemoteVer.DataContext = dt;
                        listRemoteVer.Items.SortDescriptions.Add(new SortDescription("RelTime", ListSortDirection.Descending));
                    }));
                }
                catch (WebException ex)
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        KnownErrorReport er = new KnownErrorReport(LangManager.GetLangFromResource("RemoteVerFailedTimeout") + " : " + ex.Message);
                        er.ShowDialog();
                        butRefresh.Content = LangManager.GetLangFromResource("Refresh");
                        butRefresh.IsEnabled = true;
                    }));
                }
                catch (TimeoutException ex)
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        KnownErrorReport er = new KnownErrorReport(LangManager.GetLangFromResource("RemoteVerFailedTimeout") + " : " + ex.Message);
                        er.ShowDialog();
                        butRefresh.Content = LangManager.GetLangFromResource("Refresh");
                        butRefresh.IsEnabled = true;
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
                NewGui.TaskBar taskbar = new NewGui.TaskBar();
                var task = new Thread(new ThreadStart(delegate
                {
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
                            taskbar.noticeFinished();
                            Dispatcher.Invoke(new Action(() => MeCore.MainWindow.gridPlay.LoadVersionList()));
                        };
                        downer.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
                        {
                            Dispatcher.Invoke(new Action(() => taskbar.setTaskStatus(e.ProgressPercentage + "%")));
                        };
                        Logger.log("download:" + downjsonfile);
                        downer.DownloadFile(new Uri(downjsonfile), downjsonpath);
                        VersionJson ver = LitJson.JsonMapper.ToObject<VersionJson>(new StreamReader(downjsonpath));
                        Logger.log("download:" + (ver.downloads.client.url != null & MeCore.Config.DownloadSource == 0 ? ver.downloads.client.url : downurl.ToString()));
                        downer.DownloadFileAsync(new Uri((ver.downloads.client.url != null & MeCore.Config.DownloadSource == 0 ? ver.downloads.client.url : downurl.ToString())), downpath.ToString());
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                        {
                            KnownErrorReport er = new KnownErrorReport(ex.Message);
                            er.ShowDialog();
                            butDLMC.Content = LangManager.GetLangFromResource("Download");
                            butDLMC.IsEnabled = true;
                        }));
                    }
                }));
                MeCore.MainWindow.addTask(taskbar.setThread(task).setTask("下載核心文件").setDetectAlive(false));
            }
        }
        private void listRemoteVer_MouseDoubleClick(object sender, EventArgs e)
        {
            downloadVVer();
        }

        #endregion
        #region DLLib
        IEnumerable<KMCCC.Launcher.Library> libt;
        IEnumerable<string> libs;
        IEnumerable<KMCCC.Launcher.Native> nativet;
        IEnumerable<string> natives;
        private void listVerFLib_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listVerFLib.SelectedIndex != -1)
            {
                listLib.DataContext = null;
                try
                {
                    var dt = new DataTable();
                    dt.Columns.Add("Lib");
                    dt.Columns.Add("Exist");
                    libt = MeCore.MainWindow.gridPlay.versions[listVerFLib.SelectedIndex].Libraries;
                    libs = libt.Select(lib => KMCCC.Launcher.LauncherCoreItemResolverExtensions.GetLibPath(MeCore.MainWindow.gridPlay.launcher, lib));
                    foreach (string libfile in libs)
                    {
                        dt.Rows.Add(new object[] { libfile.Substring(libfile.IndexOf("libraries")), File.Exists(libfile) });
                    }
                    nativet = MeCore.MainWindow.gridPlay.versions[listVerFLib.SelectedIndex].Natives;
                    natives = nativet.Select(native => KMCCC.Launcher.LauncherCoreItemResolverExtensions.GetNativePath(MeCore.MainWindow.gridPlay.launcher, native));
                    foreach (string nafile in natives)
                    {
                        dt.Rows.Add(new object[] { nafile.Substring(nafile.IndexOf("libraries")), File.Exists(nafile) });
                    }
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        listLib.DataContext = dt;
                        listLib.Items.SortDescriptions.Add(new SortDescription("Exist", System.ComponentModel.ListSortDirection.Ascending));
                    }));
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        new KnownErrorReport(ex.Message).Show();
                    }));
                }
            }
        }

        private void butDLLib_Click(object sender, RoutedEventArgs e)
        {
            NewGui.TaskBar task = new NewGui.TaskBar();
            var thDL = new Thread(new ThreadStart(delegate
            {
                WebClient _downer = new WebClient();
                int i = 0;
                foreach (string libfile in libs)
                {
                    i++;
                    MeCore.Invoke(new Action(() => task.setTaskStatus("下載Library " + (((float)i / libs.Count()) * 100f).ToString() + "%")));
                    if (!File.Exists(libfile))
                    {
                        Logger.log("开始下载" + libfile, Logger.LogType.Info);
                        try
                        {
                            if (!Directory.Exists(Path.GetDirectoryName(libfile)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(libfile));
                            }
                            string url = MTMCL.Resources.UrlReplacer.getLibraryUrl();
                            if (!string.IsNullOrWhiteSpace(libt.ElementAt(libs.ToList().IndexOf(libfile)).Url))
                            {
                                url = libt.ElementAt(libs.ToList().IndexOf(libfile)).Url;
                            }
#if DEBUG
                            MessageBox.Show(url + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/"));
#endif
                            Logger.log(url + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/"));
                            //                Logger.log(_urlLib + libfile.Remove(0, Environment.CurrentDirectory.Length + 22).Replace("\\", "/"));
                            _downer.DownloadFile(
                                url +
                                libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
                        }
                        catch (WebException ex)
                        {
                            Logger.log(ex);
                            Logger.log("原地址下载失败，尝试BMCL源" + libfile);
                            try
                            {
                                _downer.DownloadFile(MTMCL.Resources.Url.URL_DOWNLOAD_bangbang93 + "libraries/" + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
                            }
                            catch (WebException exception)
                            {
                                MeCore.Invoke(new Action(() => new ErrorReport(exception).Show()));
                                return;
                            }
                        }
                    }
                }
                i = 0;
                foreach (string libfile in natives)
                {
                    MeCore.Invoke(new Action(() => task.setTaskStatus("下載Native " + (((float)i / libs.Count()) * 100f).ToString() + "%")));
                    if (!File.Exists(libfile))
                    {
                        Logger.log("开始下载" + libfile, Logger.LogType.Info);
                        try
                        {
                            //                    OnStateChangeEvent(LangManager.GetLangFromResource("LauncherDownloadLib") + lib.name);
                            //                    Downloading++;
                            if (!Directory.Exists(Path.GetDirectoryName(libfile)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(libfile));
                            }
                            string url = MTMCL.Resources.UrlReplacer.getLibraryUrl();
                            if (!string.IsNullOrWhiteSpace(nativet.ElementAt(natives.ToList().IndexOf(libfile)).Url))
                            {
                                url = nativet.ElementAt(natives.ToList().IndexOf(libfile)).Url;
                            }
#if DEBUG
                            MessageBox.Show(url + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/"));
#endif
                            Logger.log(url + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/"));
                            //                Logger.log(_urlLib + libfile.Remove(0, Environment.CurrentDirectory.Length + 22).Replace("\\", "/"));
                            _downer.DownloadFile(
                                url +
                                libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
                        }
                        catch (WebException ex)
                        {
                            Logger.log(ex);
                            Logger.log("原地址下载失败，尝试BMCL源" + libfile);
                            try
                            {
                                _downer.DownloadFile(MTMCL.Resources.Url.URL_DOWNLOAD_bangbang93 + "libraries/" + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
                            }
                            catch (WebException exception)
                            {
                                MeCore.Invoke(new Action(() => new KnownErrorReport(exception.Message).Show()));
                                return;
                            }
                        }
                    }
                }
            }));
            MeCore.MainWindow.addTask(task.setThread(thDL).setTask("下載必要文件"));
            //            thDL.Start();
        }
        private void butRDLLib_Click(object sender, RoutedEventArgs e)
        {
            NewGui.TaskBar task = new NewGui.TaskBar();
            var thDL = new Thread(new ThreadStart(delegate
            {
                WebClient _downer = new WebClient();
                int i = 0;
                foreach (string libfile in libs)
                {
                    i++;
                    MeCore.Invoke(new Action(() => task.setTaskStatus("下載Library " + (((float)i / libs.Count()) * 100f).ToString() + "%")));
                    Logger.log("开始重新下载" + libfile, Logger.LogType.Info);
                    try
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(libfile)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(libfile));
                        }
                        if (File.Exists(libfile))
                        {
                            File.Delete(libfile);
                        }
                        string url = MTMCL.Resources.UrlReplacer.getLibraryUrl();
                        if (!string.IsNullOrWhiteSpace(libt.ElementAt(libs.ToList().IndexOf(libfile)).Url))
                        {
                            url = libt.ElementAt(libs.ToList().IndexOf(libfile)).Url;
                        }
#if DEBUG
                        MessageBox.Show(url + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/"));
#endif
                        Logger.log(url + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/"));
                        _downer.DownloadFile(
                            url +
                            libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
                    }
                    catch (WebException ex)
                    {
                        Logger.log(ex);
                        Logger.log("原地址下载失败，尝试BMCL源" + libfile);
                        try
                        {
                            _downer.DownloadFile(MTMCL.Resources.Url.URL_DOWNLOAD_bangbang93 + "libraries/" + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
                        }
                        catch (WebException exception)
                        {
                            MeCore.Invoke(new Action(() => new ErrorReport(exception).Show()));
                            return;
                        }
                    }
                }
                i = 0;
                foreach (string libfile in natives)
                {
                    MeCore.Invoke(new Action(() => task.setTaskStatus("下載Native " + (((float)i / libs.Count()) * 100f).ToString() + "%")));

                    Logger.log("开始重新下载" + libfile, Logger.LogType.Info);
                    try
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(libfile)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(libfile));
                        }
                        if (File.Exists(libfile))
                        {
                            File.Delete(libfile);
                        }
                        string url = MTMCL.Resources.UrlReplacer.getLibraryUrl();
                        if (!string.IsNullOrWhiteSpace(nativet.ElementAt(natives.ToList().IndexOf(libfile)).Url))
                        {
                            url = nativet.ElementAt(natives.ToList().IndexOf(libfile)).Url;
                        }
#if DEBUG
                        MessageBox.Show(url + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/"));
#endif
                        Logger.log(url + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/"));
                        _downer.DownloadFile(
                            url +
                            libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
                    }
                    catch (WebException ex)
                    {
                        Logger.log(ex);
                        Logger.log("原地址下载失败，尝试BMCL源" + libfile);
                        try
                        {
                            _downer.DownloadFile(MTMCL.Resources.Url.URL_DOWNLOAD_bangbang93 + "libraries/" + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
                        }
                        catch (WebException exception)
                        {
                            MeCore.Invoke(new Action(() => new KnownErrorReport(exception.Message).Show()));
                            return;
                        }
                    }
                }
            }));
            MeCore.MainWindow.addTask(task.setThread(thDL).setTask("重新下載必要文件"));
        }

        #endregion
        #region DLAsset
        AssetIndex assets;
        KMCCC.Launcher.Version _ver;
        bool _init = true;
        private void listVerFAsset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listVerFAsset.SelectedIndex != -1)
            {
                listAsset.DataContext = null;
                try
                {
                    _ver = MeCore.MainWindow.gridPlay.versions[listVerFAsset.SelectedIndex];
#if DEBUG
                    MessageBox.Show(_ver.Assets);
#endif
                    if (_ver.Assets == null || _ver.Assets.Equals(""))
                    {
                        MessageBox.Show(_ver.Id + " doesn't define a asset version!");
                        return;
                    }
                    if (File.Exists(MeCore.Config.MCPath + "\\assets\\indexes\\" + _ver.Assets + ".json"))
                    {
                        Downloader_DownloadStringCompleted(this, null);
                        return;
                    }
                    var thGet = new Thread(new ThreadStart(delegate
                    {
                        string gameVersion = _ver.Assets;
                        try
                        {
                            _downer.DownloadStringAsync(new Uri(MTMCL.Resources.UrlReplacer.getDownloadUrl() + "indexes/" + gameVersion + ".json"));
                            Logger.info(MTMCL.Resources.UrlReplacer.getDownloadUrl() + "indexes/" + gameVersion + ".json");
                        }
                        catch (WebException ex)
                        {
                            Logger.info("游戏版本" + gameVersion);
                            Logger.error(ex);
                        }
                        _downer.DownloadStringCompleted += Downloader_DownloadStringCompleted;
                        _downer.DownloadFileCompleted += Downloader_DownloadFileCompleted;
                    }));
                    thGet.Start();
                }
                catch (Exception ex)
                {
                    new KnownErrorReport(ex.Message).Show();
                }
            }
        }

        private void butDLAsset_Click(object sender, RoutedEventArgs e)
        {
            NewGui.TaskBar task = new NewGui.TaskBar();
            var thGet = new Thread(new ThreadStart(delegate
            {
                int i = 0;
                foreach (KeyValuePair<string, AssetsEntity> entity in assets.objects)
                {
                    i++;
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        task.setTaskStatus(((float)i / assets.objects.Count * 100).ToString() + "%");
                    }));
                    string url = MTMCL.Resources.UrlReplacer.getResourceUrl() + entity.Value.hash.Substring(0, 2) + "/" + entity.Value.hash;
                    string file = MeCore.Config.MCPath + @"\assets\objects\" + entity.Value.hash.Substring(0, 2) + "\\" + entity.Value.hash;
                    if (assets._virtual)
                    {
                        file = MeCore.Config.MCPath + @"\assets\" + entity.Key;
                    }
                    FileHelper.CreateDirectoryForFile(file);
                    try
                    {
                        if (FileHelper.IfFileVaild(file, entity.Value.size)) continue;
                        if (_init)
                        {
                            _init = false;
                        }
                        //Downloader.DownloadFileAsync(new Uri(Url), File,Url);
                        _downer.DownloadFile(new Uri(url), file);
                        Logger.log(i.ToString(CultureInfo.InvariantCulture), "/", assets.objects.Count.ToString(CultureInfo.InvariantCulture), file.Substring(MeCore.Config.MCPath.Length), "下载完毕");
                        if (i == assets.objects.Count)
                        {
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                Logger.log("assets下载完毕");
                                MeCore.NIcon.ShowBalloonTip(3000, LangManager.GetLangFromResource("SyncAssetsFinish"));
                            }));
                        }
                    }
                    catch (WebException ex)
                    {
                        Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                        {
                            Logger.log(ex.Response.ResponseUri.ToString());
                            Logger.error(ex);
                            new ErrorReport(ex).Show();
                        }));
                    }
                }
                if (_init)
                {
                    Logger.info("无需更新assets");
                }
            }));
            MeCore.MainWindow.addTask(task.setThread(thGet).setTask("下載資源文件"));
            //            thGet.Start();
        }
        private void butRDLAsset_Click(object sender, RoutedEventArgs e)
        {
            NewGui.TaskBar task = new NewGui.TaskBar();
            var thGet = new Thread(new ThreadStart(delegate
            {
                int i = 0;
                foreach (KeyValuePair<string, AssetsEntity> entity in assets.objects)
                {
                    i++;
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        task.setTaskStatus(((float)i / assets.objects.Count * 100).ToString() + "%");
                    }));
                    string url = MTMCL.Resources.UrlReplacer.getResourceUrl() + entity.Value.hash.Substring(0, 2) + "/" + entity.Value.hash;
                    string file = MeCore.Config.MCPath + @"\assets\objects\" + entity.Value.hash.Substring(0, 2) + "\\" + entity.Value.hash;
                    if (assets._virtual)
                    {
                        file = MeCore.Config.MCPath + @"\assets\" + entity.Key;
                    }
                    FileHelper.CreateDirectoryForFile(file);
                    try
                    {
                        if (FileHelper.IfFileVaild(file, entity.Value.size)) File.Delete(file);
                        _downer.DownloadFile(new Uri(url), file);
                        Logger.log(i.ToString(CultureInfo.InvariantCulture), "/", assets.objects.Count.ToString(CultureInfo.InvariantCulture), file.Substring(MeCore.Config.MCPath.Length), "下载完毕");
                        if (i == assets.objects.Count)
                        {
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                Logger.log("assets重新下载完毕");
                                //MeCore.NIcon.ShowBalloonTip(3000, LangManager.GetLangFromResource("SyncAssetsFinish"));
                            }));
                        }
                    }
                    catch (WebException ex)
                    {
                        Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                        {
                            Logger.log(ex.Response.ResponseUri.ToString());
                            Logger.error(ex);
                            new ErrorReport(ex).Show();
                        }));
                    }
                }
            }));
            MeCore.MainWindow.addTask(task.setThread(thGet).setTask("重新下載資源文件"));
        }

        private void butF5Asset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dt = new DataTable();
                dt.Columns.Add("Assets");
                dt.Columns.Add("Size", typeof(int));
                dt.Columns.Add("Hash");
                dt.Columns.Add("Exist");
                foreach (KeyValuePair<string, AssetsEntity> entity in assets.objects)
                {
                    dt.Rows.Add(new object[] { entity.Key, entity.Value.size, entity.Value.hash, assetExist(entity, assets._virtual).ToString() });
                }
                Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    listAsset.DataContext = dt;
                    listAsset.Items.SortDescriptions.Add(new SortDescription("Exist", ListSortDirection.Ascending));
                }));
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    new KnownErrorReport(ex.Message).Show();
                }));
            }
        }
        void Downloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Logger.error(e.UserState.ToString());
                Logger.error(e.Error);
            }
        }

        void Downloader_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            _downer.DownloadStringCompleted -= Downloader_DownloadStringCompleted;
            if (e != null)
            {
                if (e.Error != null)
                {
                    if (e.Error is WebException)
                    {
                        var ex = e.Error as WebException;
                        Logger.log(ex.Response.ResponseUri.ToString());
                    }
                    Logger.error(e.Error);
                }
                else
                {
                    try
                    {
                        string gameVersion = _ver.Assets;
                        FileHelper.CreateDirectoryForFile(MeCore.Config.MCPath + "/assets/indexes/" + gameVersion + ".json");
                        var sw = new StreamWriter(MeCore.Config.MCPath + "/assets/indexes/" + gameVersion + ".json");
                        sw.Write(e.Result);
                        sw.Close();
                        assets = LitJson.JsonMapper.ToObject<AssetIndex>(e.Result);
                        Logger.log("共", assets.objects.Count.ToString(CultureInfo.InvariantCulture), "项assets");
                        try
                        {
                            var dt = new DataTable();
                            dt.Columns.Add("Assets");
                            dt.Columns.Add("Size", typeof(int));
                            dt.Columns.Add("Hash");
                            dt.Columns.Add("Exist");
                            foreach (KeyValuePair<string, AssetsEntity> entity in assets.objects)
                            {
                                dt.Rows.Add(new object[] { entity.Key, entity.Value.size, entity.Value.hash, assetExist(entity, assets._virtual).ToString() });
                            }
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                listAsset.DataContext = dt;
                                listAsset.Items.SortDescriptions.Add(new SortDescription("Exist", ListSortDirection.Ascending));
                            }));
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                new KnownErrorReport(ex.Message).Show();
                            }));
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
            }
            else
            {
                try
                {
                    string gameVersion = _ver.Assets;
                    FileHelper.CreateDirectoryForFile(MeCore.Config.MCPath + "/assets/indexes/" + gameVersion + ".json");
                    var sr = new StreamReader(MeCore.Config.MCPath + "/assets/indexes/" + gameVersion + ".json");
                    assets = LitJson.JsonMapper.ToObject<AssetIndex>(sr.ReadToEnd());
                    sr.Close();
                    Logger.log("共", assets.objects.Count.ToString(CultureInfo.InvariantCulture), "项assets");
                    try
                    {
                        var dt = new DataTable();
                        dt.Columns.Add("Assets");
                        dt.Columns.Add("Size", typeof(int));
                        dt.Columns.Add("Hash");
                        dt.Columns.Add("Exist");
                        foreach (KeyValuePair<string, AssetsEntity> entity in assets.objects)
                        {
                            dt.Rows.Add(new object[] { entity.Key, entity.Value.size, entity.Value.hash, assetExist(entity, assets._virtual).ToString() });
                        }
                        Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                        {
                            listAsset.DataContext = dt;
                            listAsset.Items.SortDescriptions.Add(new SortDescription("Exist", ListSortDirection.Ascending));
                        }));
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                        {
                            new KnownErrorReport(ex.Message).Show();
                        }));
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

        }
        private bool assetExist(KeyValuePair<string, AssetsEntity> entity, bool isVirtual)
        {
            if (isVirtual)
            {
                return File.Exists(MeCore.Config.MCPath + @"\assets\" + entity.Key);
            }
            else
            {
                return File.Exists(MeCore.Config.MCPath + @"\assets\objects\" + entity.Value.hash.Substring(0, 2) + @"\" + entity.Value.hash);
            }
        }

        #endregion
        #region DLForge
        readonly ForgeVersionList _forgeVer = new ForgeVersionList();
        private void butReload_Click(object sender, RoutedEventArgs e)
        {
            butReload.Content = LangManager.GetLangFromResource("RemoteVerGetting");
            butReload.IsEnabled = false;
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
                butReload.Content = LangManager.GetLangFromResource("Refresh");
                butReload.IsEnabled = true;
            }));
        }
        private void DownloadForge(string ver)
        {
            if (!_forgeVer.ForgeDownloadUrl.ContainsKey(ver))
            {
                MessageBox.Show(LangManager.GetLangFromResource("ForgeDoNotSupportInstaller"));
                return;
            }
            NewGui.TaskBar task = new NewGui.TaskBar();
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
                    MeCore.Invoke(new Action(() => task.setTaskStatus("下載Forge安裝檔 " + e.ProgressPercentage + "%")));
                };
                downer.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e)
                {
                    MeCore.Invoke(new Action(() => task.setTaskStatus("嘗試安裝Forge")));
                    new ForgeInstaller().install(filename);
                    File.Delete(filename);
                    MeCore.Invoke(new Action(() => task.setTaskStatus("完成")));
                    task.noticeFinished();
                };
                MeCore.Invoke(new Action(() => task.setTaskStatus("下載Forge安裝檔")));
                downer.DownloadFileAsync(url, filename);
            }));
            MeCore.MainWindow.addTask(task.setThread(thDL).setTask("安裝Forge").setDetectAlive(false));
        }

        private void butFDL_Click(object sender, RoutedEventArgs e)
        {
            if (this.listForge.SelectedItem == null)
                return;
            DataRowView selectVer = listForge.SelectedItem as DataRowView;
            DownloadForge(selectVer[0] as string);
        }

        private void listForge_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listForge.SelectedItem == null)
            {
                return;
            }
            DataRowView selectVer = listForge.SelectedItem as DataRowView;
            DownloadForge(selectVer[0] as string);
        }
        #endregion
        #region DLPack
        private void butPDL_Click(object sender, RoutedEventArgs e)
        {
            NewGui.TaskBar task = new NewGui.TaskBar();
            var thDL = new Thread(new ThreadStart(delegate
            {
                try
                {
                    MeCore.Invoke(new System.Windows.Forms.MethodInvoker(()=> {
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
                            MeCore.Invoke(new Action(() => task.setTaskStatus("下載壓縮檔 " + e1.ProgressPercentage + "%")));
                        };
                        downer.DownloadFileCompleted += delegate (object sender1, AsyncCompletedEventArgs e1)
                        {
                            MeCore.Invoke(new Action(() => task.setTaskStatus("嘗試解壓")));
                            new ModPackProcesser().install(filename);
                            File.Delete(filename);
                            MeCore.Invoke(new Action(() => task.setTaskStatus("完成")));
                            task.noticeFinished();
                        };
                        MeCore.Invoke(new Action(() => task.setTaskStatus("下載壓縮檔")));
                        downer.DownloadFileAsync(url, filename);
                    }));
                }
                catch (Exception e1)
                {
                    MeCore.Invoke(new Action(() => new KnownErrorReport(e1.Message, e1.StackTrace).Show()));
                    MeCore.Invoke(new Action(() => task.setTaskStatus("失敗")));
                    task.noticeFinished();
                }
            }));
            MeCore.MainWindow.addTask(task.setThread(thDL).setTask("下載整合包").setDetectAlive(false));
        }
        #endregion
        public void setLblColor(Color color)
        {
            lblDLI.Foreground = new SolidColorBrush(color);
            lblTitle.Foreground = new SolidColorBrush(color);
            lblTitle_Copy.Foreground = new SolidColorBrush(color);
            lblTitle_Copy1.Foreground = new SolidColorBrush(color);
            lblVer.Foreground = new SolidColorBrush(color);
            lblVer_Copy.Foreground = new SolidColorBrush(color);
            lblTitle_Copy2.Foreground = new SolidColorBrush(color);
            lblDLTitle.Foreground = new SolidColorBrush(color);
            butAsset.Foreground = new SolidColorBrush(color);
            butLib.Foreground = new SolidColorBrush(color);
            butForge.Foreground = new SolidColorBrush(color);
            butVanilla.Foreground = new SolidColorBrush(color);
        }

        private void butVanilla_Click(object sender, RoutedEventArgs e)
        {
            gridHome.Visibility = Visibility.Collapsed;
            gridMC.Visibility = Visibility.Visible;
        }

        private void butLib_Click(object sender, RoutedEventArgs e)
        {
            gridHome.Visibility = Visibility.Collapsed;
            gridLib.Visibility = Visibility.Visible;
        }

        private void butAsset_Click(object sender, RoutedEventArgs e)
        {
            gridHome.Visibility = Visibility.Collapsed;
            gridAssets.Visibility = Visibility.Visible;
        }

        private void butForge_Click(object sender, RoutedEventArgs e)
        {
            gridHome.Visibility = Visibility.Collapsed;
            gridForge.Visibility = Visibility.Visible;
        }

        private void butBack1_Click(object sender, RoutedEventArgs e)
        {
            gridHome.Visibility = Visibility.Visible;
            gridMC.Visibility = Visibility.Collapsed;
        }

        private void butBack2_Click(object sender, RoutedEventArgs e)
        {
            gridHome.Visibility = Visibility.Visible;
            gridLib.Visibility = Visibility.Collapsed;
        }

        private void butBack3_Click(object sender, RoutedEventArgs e)
        {
            gridHome.Visibility = Visibility.Visible;
            gridAssets.Visibility = Visibility.Collapsed;
        }

        private void butBack4_Click(object sender, RoutedEventArgs e)
        {
            gridHome.Visibility = Visibility.Visible;
            gridForge.Visibility = Visibility.Collapsed;
        }
        private void butBack5_Click(object sender, RoutedEventArgs e)
        {
            gridHome.Visibility = Visibility.Visible;
            gridPack.Visibility = Visibility.Collapsed;
        }

        private void butPack_Click(object sender, RoutedEventArgs e)
        {
            gridPack.Visibility = Visibility.Visible;
            gridHome.Visibility = Visibility.Collapsed;
        }
    }
}
