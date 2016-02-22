using KMCCC.Launcher;
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
using System.Threading.Tasks;
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
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(async delegate
                    {
                        butRefresh.Content = LangManager.GetLangFromResource("RemoteVerGetting");
                        if (MeCore.Config.DownloadSource == 1)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
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
                        new KnownErrorReport(LangManager.GetLangFromResource("RemoteVerFailedTimeout") + " : " + ex.Message, LangManager.GetLangFromResource("NoConnectionSolve")).Show();
                        butRefresh.Content = LangManager.GetLangFromResource("Refresh");
                        butRefresh.IsEnabled = true;
                    }));
                }
                catch (TimeoutException ex)
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        new KnownErrorReport(LangManager.GetLangFromResource("RemoteVerFailedTimeout") + " : " + ex.Message, LangManager.GetLangFromResource("NoConnectionSolve")).Show();
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
                var task = new Thread(new ThreadStart(async delegate
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(async delegate
                    {
                        if (MeCore.Config.DownloadSource == 1)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
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
                            taskbar.noticeFinished();
                            Dispatcher.Invoke(new Action(() => MeCore.MainWindow.gridPlay.LoadVersionList()));
                        };
                        downer.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
                        {
                            Dispatcher.Invoke(new Action(() => taskbar.setTaskStatus(e.ProgressPercentage + "%")));
                        };
                        Logger.log("download:" + downjsonfile);
                        try
                        {
                            downer.DownloadFile(new Uri(downjsonfile), downjsonpath);
                        }
                        catch (Exception)
                        {
                            taskbar.noticeFailed();
                            return;
                        }
                        await Task.Delay(TimeSpan.FromMilliseconds(500));
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
                            new ErrorReport(ex).Show();
                            taskbar.noticeFailed();
                        }));
                    }
                }));
                MeCore.MainWindow.addTask(taskbar.setThread(task).setTask(LangManager.GetLangFromResource("TaskDLMC")).setDetectAlive(false), "dl-mcclient-" + selectVer[0] as string);
            }
        }
        private void listRemoteVer_MouseDoubleClick(object sender, EventArgs e)
        {
            downloadVVer();
        }

        #endregion
        #region DLLib
        List<LibraryUniversal> libs = new List<LibraryUniversal>();
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
                    var ver = MeCore.MainWindow.gridPlay.versions[listVerFLib.SelectedIndex];
                    libs.Clear();
                    VersionJson lib = LitJson.JsonMapper.ToObject<VersionJson>(new LitJson.JsonReader(new StreamReader(MeCore.MainWindow.gridPlay.launcher.GetVersionJsonPath(ver))));
                    foreach (var item in lib.libraries.ToUniversalLibrary())
                    {
                        libs.Add(item);
                    }
                    foreach (LibraryUniversal libfile in libs)
                    {
                        dt.Rows.Add(new object[] { libfile.path.Substring(libfile.path.IndexOf("libraries")), FileHelper.IfFileVaild(libfile.path) });
                    }
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        listLib.DataContext = dt;
                        listLib.Items.SortDescriptions.Add(new SortDescription("Exist", ListSortDirection.Ascending));
                        butDLLib.IsEnabled = true;
                        butRDLLib.IsEnabled = true;
                    }));
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        Logger.log(ex);
                        new KnownErrorReport(ex.Message, string.Format(LangManager.GetLangFromResource("FormatFaultSolve"), MeCore.MainWindow.gridPlay.versions[listVerFLib.SelectedIndex].Id + ".json")).Show();
                    }));
                }
            }
        }

        private void butDLLib_Click(object sender, RoutedEventArgs e)
        {
            NewGui.TaskBar task = new NewGui.TaskBar();
            var thDL = new Thread(new ThreadStart(delegate
            {
                Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(async delegate
                {
                    if (MeCore.Config.DownloadSource == 1)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }));
                WebClient _downer = new WebClient();
                int i = 0;
                foreach (var libfile in libs)
                {
                    i++;
                    MeCore.Invoke(new Action(() => task.setTaskStatus(string.Format(LangManager.GetLangFromResource("SubTaskDLLib"), (((float)i / libs.Count()) * 100f).ToString() + "%"))));
                    if (!File.Exists(libfile.path))
                    {
                        Logger.log("Start downloading " + libfile.path, Logger.LogType.Info);
                        DownloadLib(libfile);
                    }
                }
            }));
            MeCore.MainWindow.addTask(task.setThread(thDL).setTask(LangManager.GetLangFromResource("TaskDLLib")), "dl-lib");
        }
        private void butRDLLib_Click(object sender, RoutedEventArgs e)
        {
            NewGui.TaskBar task = new NewGui.TaskBar();
            var thDL = new Thread(new ThreadStart(delegate
            {
                Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(async delegate
                {
                    if (MeCore.Config.DownloadSource == 1)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }));
                WebClient _downer = new WebClient();
                int i = 0;
                foreach (var libfile in libs)
                {
                    i++;
                    MeCore.Invoke(new Action(() => task.setTaskStatus(string.Format(LangManager.GetLangFromResource("SubTaskDLLib"), (((float)i / libs.Count()) * 100f).ToString() + "%"))));
                    Logger.log("开始重新下载" + libfile.path, Logger.LogType.Info);
                    DownloadLib(libfile);
                }
            }));
            MeCore.MainWindow.addTask(task.setThread(thDL).setTask(LangManager.GetLangFromResource("TaskRDLLib")), "dl-lib");
        }
        private void DownloadLib(LibraryUniversal file)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(file.path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file.path));
                }
                string url = MTMCL.Resources.UrlReplacer.getLibraryUrl();
                if (!string.IsNullOrWhiteSpace(file.url))
                {
                    if (file.url.StartsWith("https://libraries.minecraft.net/"))
                    {
                        url = MTMCL.Resources.UrlReplacer.toGoodLibUrl(file.url);
                    }
                    else {
                        url = MTMCL.Resources.UrlReplacer.getForgeMaven(file.url) + file.path.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/");
                    }
                }
                else
                {
                    url = url + file.path.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/");
                }
#if DEBUG
                MessageBox.Show(url);
#endif
                Logger.log(url);
                _downer.DownloadFile(url, file.path);
            }
            catch (WebException ex)
            {
                Logger.log(ex);
                Logger.log("原地址下载失败，尝试BMCL源" + file.url);
                try
                {
                    string url = MTMCL.Resources.UrlReplacer.getLibraryUrl(1);
                    if (!string.IsNullOrWhiteSpace(file.url))
                    {
                        if (file.url.StartsWith("https://libraries.minecraft.net/"))
                        {
                            url = MTMCL.Resources.UrlReplacer.toGoodLibUrl(file.url, 1);
                        }
                        else {
                            url = MTMCL.Resources.UrlReplacer.getForgeMaven(file.url, 1) + file.path.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/");
                        }
                    }
                    else
                    {
                        url = url + file.path.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/");
                    }
                    Logger.log(url);
                    _downer.DownloadFile(MTMCL.Resources.UrlReplacer.toGoodLibUrl(url), file.path);
                }
                catch (WebException exception)
                {
                    MeCore.Invoke(new Action(() => new ErrorReport(exception).Show()));
                    return;
                }
            }
        }

        #endregion
        #region DLAsset
        Assets.AssetIndex assets;
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
                            Dispatcher.Invoke(new Action(()=> listVerFAsset.IsEnabled = false));
                            _downer.DownloadStringAsync(new Uri(MTMCL.Resources.UrlReplacer.getDownloadUrl() + "indexes/" + gameVersion + ".json"));
                            Logger.info(MTMCL.Resources.UrlReplacer.getDownloadUrl() + "indexes/" + gameVersion + ".json");
                        }
                        catch (WebException ex)
                        {
                            Logger.info("game version" + gameVersion);
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
                            new KnownErrorReport(ex.Message, LangManager.GetLangFromResource("NoConnectionSolve")).Show();
                        }));
                    }
                }
                if (_init)
                {
                    Logger.info("无需更新assets");
                }
            }));
            MeCore.MainWindow.addTask(task.setThread(thGet).setTask(LangManager.GetLangFromResource("TaskDLAssets")), "dl-assets");
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
                            new KnownErrorReport(ex.Message, LangManager.GetLangFromResource("NoConnectionSolve")).Show();
                        }));
                    }
                }
            }));
            MeCore.MainWindow.addTask(task.setThread(thGet).setTask(LangManager.GetLangFromResource("TaskRDLAssets")), "dl-assets");
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
                    new KnownErrorReport(ex.Message, string.Format(LangManager.GetLangFromResource("FormatFaultSolve"))).Show();
                }));
            }
        }
        void Downloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => listVerFAsset.IsEnabled = true));
            if (e.Error != null)
            {
                Logger.error(e.UserState.ToString());
                Logger.error(e.Error);
            }
        }

        void Downloader_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => listVerFAsset.IsEnabled = true));
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
                        assets = LitJson.JsonMapper.ToObject<Assets.AssetIndex>(e.Result);
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
                                butDLAsset.IsEnabled = true;
                                butRDLAsset.IsEnabled = true;
                                butF5Asset.IsEnabled = true;
                            }));
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                new KnownErrorReport(ex.Message, string.Format(LangManager.GetLangFromResource("FormatFaultSolve"), gameVersion + ".json")).Show();
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
                    assets = LitJson.JsonMapper.ToObject<Assets.AssetIndex>(sr.ReadToEnd());
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
                            butDLAsset.IsEnabled = true;
                            butRDLAsset.IsEnabled = true;
                            butF5Asset.IsEnabled = true;
                        }));
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                        {
                            new KnownErrorReport(ex.Message, string.Format(LangManager.GetLangFromResource("FormatFaultSolve"), gameVersion + ".json")).Show();
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
            return File.Exists(MeCore.Config.MCPath + @"\assets\objects\" + entity.Value.hash.Substring(0, 2) + @"\" + entity.Value.hash);
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
                    MeCore.Invoke(new Action(() => task.setTaskStatus(string.Format(LangManager.GetLangFromResource("SubTaskDLForge"), e.ProgressPercentage))));
                };
                downer.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e)
                {
                    MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLangFromResource("SubTaskInstallForge"))));
                    new ForgeInstaller().install(filename);
                    File.Delete(filename);
                    MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLangFromResource("TaskFinish"))));
                    task.noticeFinished();
                };
                MeCore.Invoke(new Action(() => task.setTaskStatus(string.Format(LangManager.GetLangFromResource("SubTaskDLForge"), "0"))));
                downer.DownloadFileAsync(url, filename);
            }));
            MeCore.MainWindow.addTask(task.setThread(thDL).setTask(LangManager.GetLangFromResource("TaskInstallForge")).setDetectAlive(false), "dl-instl-forgeclient-" + ver);
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
                            new ModPackProcesser().install(filename);
                            File.Delete(filename);
                            MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLangFromResource("TaskFinish"))));
                            task.noticeFinished();
                        };
                        MeCore.Invoke(new Action(() => task.setTaskStatus(string.Format(LangManager.GetLangFromResource("SubTaskDLModPack"), 0))));
                        downer.DownloadFileAsync(url, filename);
                    }));
                }
                catch (Exception e1)
                {
                    MeCore.Invoke(new Action(() => new KnownErrorReport(e1.Message, e1.StackTrace).Show()));
                    MeCore.Invoke(new Action(() => task.setTaskStatus(LangManager.GetLangFromResource("TaskFinish"))));
                    task.noticeFinished();
                }
            }));
            MeCore.MainWindow.addTask(task.setThread(thDL).setTask(LangManager.GetLangFromResource("TaskDLModPack")).setDetectAlive(false), "dl-modpack");
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
