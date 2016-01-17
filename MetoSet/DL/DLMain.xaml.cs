using MetoCraft.Assets;
using MetoCraft.Forge;
using MetoCraft.Lang;
using MetoCraft.Play;
using MetoCraft.util;
using MetoCraft.Versions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetoCraft.DL
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
            var getJson = (HttpWebRequest)WebRequest.Create(MeCore.UrlDownload + "versions/versions.json");
            getJson.Timeout = 10000;
            getJson.ReadWriteTimeout = 10000;
            getJson.UserAgent = "MetoCraft" + MeCore.version;
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
                    dt.Columns.Add("RelTime");
                    dt.Columns.Add("Type");
                    if (remoteVersion != null)
                        foreach (RemoteVerType rv in remoteVersion.getVersions())
                        {
                            dt.Rows.Add(new object[] { rv.id, rv.releaseTime, rv.type });
                        }
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        butRefresh.Content = LangManager.GetLangFromResource("Refresh");
                        butRefresh.IsEnabled = true;
                        listRemoteVer.DataContext = dt;
                        listRemoteVer.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("RelTime", System.ComponentModel.ListSortDirection.Descending));
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
                    var selectver = selectVer[0] as string;
                    var downpath = new StringBuilder(MeCore.Config.MCPath + @"\versions\");
                    downpath.Append(selectver).Append("\\");
                    downpath.Append(selectver).Append(".jar");
                    var downer = new WebClient();
                    downer.Headers.Add("User-Agent", "MetoCraft" + MeCore.version);
                    var downurl = new StringBuilder(MeCore.UrlDownload);
                    downurl.Append(@"versions\");
                    downurl.Append(selectver).Append("\\");
                    downurl.Append(selectver).Append(".jar");
#if DEBUG
                    MessageBox.Show(downpath + "\n" + downurl);
#endif
                butDLMC.Content = LangManager.GetLangFromResource("RemoteVerDownloading");
                butDLMC.IsEnabled = false;
                // ReSharper disable once AssignNullToNotNullAttribute
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(downpath.ToString())))
                    {
                        // ReSharper disable AssignNullToNotNullAttribute
                        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(downpath.ToString()));
                        // ReSharper restore AssignNullToNotNullAttribute
                    }
                    string downjsonfile = downurl.ToString().Substring(0, downurl.Length - 4) + ".json";
                    string downjsonpath = downpath.ToString().Substring(0, downpath.Length - 4) + ".json";
                    try
                    {
                        downer.DownloadFileCompleted += downer_DownloadClientFileCompleted;
                        downer.DownloadProgressChanged += downer_DownloadProgressChanged;
                        Logger.log("download:" + downjsonfile);
                        downer.DownloadFile(new Uri(downjsonfile), downjsonpath);
                        Logger.log("download:" + downurl);
                        downer.DownloadFileAsync(new Uri(downurl.ToString()), downpath.ToString());
                        _downedtime = Environment.TickCount - 1;
                        _downed = 0;
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
                }
        }
        int _downedtime;
        int _downed;
        void downer_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
                ChangeDownloadProgress((int)e.BytesReceived, (int)e.TotalBytesToReceive);
                //            TaskbarManager.Instance.SetProgressValue((int)e.BytesReceived, (int)e.TotalBytesToReceive);
                var info = new StringBuilder(LangManager.GetLangFromResource("DownloadSpeedInfo"));
                try
                {
                    info.Append(((e.BytesReceived - _downed) / ((Environment.TickCount - _downedtime) / 1000.0) / 1024.0).ToString("F2")).Append("KB/s,");
                }
                catch (DivideByZeroException) { info.Append("0B/s,"); }
                info.Append(e.ProgressPercentage.ToString(CultureInfo.InvariantCulture)).Append("%");
                SetDownloadInfo(info.ToString());
        }
        void downer_DownloadClientFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Logger.log("Success to download client file.");
            MessageBox.Show(LangManager.GetLangFromResource("RemoteVerDownloadSuccess"));
            butDLMC.Content = LangManager.GetLangFromResource("Download");
            butDLMC.IsEnabled = true;
            MeCore.MainWindow.gridPlay.LoadVersionList();
        }
        public void ChangeDownloadProgress(int value, int maxValue)
        {
            prsDown.Maximum = maxValue;
            prsDown.Value = value;
        }
        public void ChangeDownloadProgress(long value, long maxValue)
        {
            this.ChangeDownloadProgress((int)value, (int)maxValue);
        }
        private void listRemoteVer_MouseDoubleClick(object sender, EventArgs e)
        {
            downloadVVer();
        }
        public void SetDownloadInfo(string info)
        {
            labDownInfo.Content = info;
        }
        #endregion
        #region DLLib
        IEnumerable<string> libs;
        IEnumerable<string> natives;
        private readonly string _urlLib = MeCore.UrlLibraries;
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
                    libs = MeCore.MainWindow.gridPlay.versions[listVerFLib.SelectedIndex].Libraries.Select(lib => KMCCC.Launcher.LauncherCoreItemResolverExtensions.GetLibPath(PlayMain.launcher, lib));
                    foreach (string libfile in libs)
                    {
                        dt.Rows.Add(new object[] { libfile.Substring(libfile.IndexOf("libraries")), File.Exists(libfile) });
                    }
                    natives = MeCore.MainWindow.gridPlay.versions[listVerFLib.SelectedIndex].Natives.Select(native => KMCCC.Launcher.LauncherCoreItemResolverExtensions.GetNativePath(PlayMain.launcher, native));
                    foreach (string nafile in natives)
                    {
                        dt.Rows.Add(new object[] { nafile.Substring(nafile.IndexOf("libraries")), File.Exists(nafile) });
                    }
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        listLib.DataContext = dt;
                        listLib.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Exist", System.ComponentModel.ListSortDirection.Ascending));
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
                            //                    OnStateChangeEvent(LangManager.GetLangFromResource("LauncherDownloadLib") + lib.name);
                            //                    Downloading++;
                            if (!Directory.Exists(System.IO.Path.GetDirectoryName(libfile)))
                            {
                                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(libfile));
                            }
#if DEBUG
                            //                        System.Windows.MessageBox.Show(_urlLib + libp.Remove(0, Environment.CurrentDirectory.Length + 22).Replace("\\", "/"));
#endif
                            Logger.log(_urlLib + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/"));
                            //                Logger.log(_urlLib + libfile.Remove(0, Environment.CurrentDirectory.Length + 22).Replace("\\", "/"));
                            _downer.DownloadFile(
                                _urlLib +
                                libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
                        }
                        catch (WebException ex)
                        {
                            Logger.log(ex);
                            Logger.log("原地址下载失败，尝试BMCL源" + libfile);
                            try
                            {
                                _downer.DownloadFile(MetoCraft.Resources.Url.URL_DOWNLOAD_bangbang93 + "libraries/" + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
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
                            if (!Directory.Exists(System.IO.Path.GetDirectoryName(libfile)))
                            {
                                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(libfile));
                            }
#if DEBUG
                            //                        System.Windows.MessageBox.Show(_urlLib + libp.Remove(0, Environment.CurrentDirectory.Length + 22).Replace("\\", "/"));
#endif
                            Logger.log(_urlLib + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/"));
                            //                Logger.log(_urlLib + libfile.Remove(0, Environment.CurrentDirectory.Length + 22).Replace("\\", "/"));
                            _downer.DownloadFile(
                                _urlLib +
                                libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
                        }
                        catch (WebException ex)
                        {
                            Logger.log(ex);
                            Logger.log("原地址下载失败，尝试BMCL源" + libfile);
                            try
                            {
                                _downer.DownloadFile(MetoCraft.Resources.Url.URL_DOWNLOAD_bangbang93 + "libraries/" + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
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

        #endregion
        #region DLAsset
        Dictionary<string, AssetsEntity> asset;
//        private readonly string _urlDownload = MeCore.UrlDownload;
//        private readonly string _urlResource = MeCore.UrlResource;
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
                    MessageBox.Show(_ver.Assets);
                    if (_ver.Assets == null || _ver.Assets.Equals(""))
                    {
                        MessageBox.Show(_ver.Id + " doesn't define a asset version!");
                        return;
                    }
                    if (_ver.Assets.Equals("legacy"))
                    {
                        MessageBox.Show(_ver.Id + " doesn't define a supported asset version!");
                        return;
                    }
                    var thGet = new Thread(new ThreadStart(delegate
                    {
                        string gameVersion = _ver.Assets;
                        try
                        {
                            _downer.DownloadStringAsync(new Uri(MeCore.UrlDownload + "indexes/" + gameVersion + ".json"));
                            Logger.info(MeCore.UrlDownload + "indexes/" + gameVersion + ".json");
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
                foreach (KeyValuePair<string, AssetsEntity> entity in asset)
                {
                    i++;
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        task.setTaskStatus(((float)i / asset.Count * 100).ToString()+"%");
//                        lblDr.Content = i + "/" + asset.Count.ToString(CultureInfo.InvariantCulture);
                    }));
                    string url = MeCore.UrlResource + entity.Value.hash.Substring(0, 2) + "/" + entity.Value.hash;
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
                        Logger.log(i.ToString(CultureInfo.InvariantCulture), "/", asset.Count.ToString(CultureInfo.InvariantCulture), file.Substring(AppDomain.CurrentDomain.BaseDirectory.Length), "下载完毕");
                        if (i == asset.Count)
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

        private void butF5Asset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dt = new DataTable();
                dt.Columns.Add("Assets");
                dt.Columns.Add("Size");
                dt.Columns.Add("Hash");
                dt.Columns.Add("Exist");
                foreach (KeyValuePair<string, AssetsEntity> entity in asset)
                {
                    dt.Rows.Add(new object[] { entity.Key, entity.Value.size, entity.Value.hash, File.Exists(@"assets\objects\" + entity.Value.hash.Substring(0,2) + @"\" + entity.Value.hash).ToString() });
                }
                Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    listAsset.DataContext = dt;
                    listAsset.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Exist", System.ComponentModel.ListSortDirection.Ascending));
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
        void Downloader_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
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
                    var jsSerializer = new JavaScriptSerializer();
                    var assetsObject = jsSerializer.Deserialize<Dictionary<string, Dictionary<string, AssetsEntity>>>(e.Result);
//                    var assetsObject = jsSerializer.Deserialize<Dictionary<string, Dictionary<string, AssetsEntity>>>(new StreamReader(MeCore.Config.MCPath + "/assets/indexes/" + gameVersion + ".json").ReadToEnd());
                    asset = assetsObject["objects"];
                    Logger.log("共", asset.Count.ToString(CultureInfo.InvariantCulture), "项assets");
                    try
                    {
                        var dt = new DataTable();
                        dt.Columns.Add("Assets");
                        dt.Columns.Add("Size");
                        dt.Columns.Add("Hash");
                        dt.Columns.Add("Exist");
                        foreach (KeyValuePair<string, AssetsEntity> entity in asset)
                        {
                            dt.Rows.Add(new object[] { entity.Key, entity.Value.size, entity.Value.hash, File.Exists(MeCore.Config.MCPath + @"\assets\objects\" + entity.Value.hash.Substring(0, 2) + @"\" + entity.Value.hash).ToString() });
                        }
                        Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                        {
                            listAsset.DataContext = dt;
                            listAsset.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Exist", System.ComponentModel.ListSortDirection.Ascending));
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
        private void butDLMusic_Click(object sender, RoutedEventArgs e)
        {
            /*            JavaScriptSerializer SoundsJsonSerizlizer = new JavaScriptSerializer();
                        var sounds = SoundsJsonSerizlizer.Deserialize<Dictionary<string, Dictionary<string, object>>>((new WebClient()).DownloadString("http://www.bangbang93.com/bmcl/resources/sounds.json"));
                        Hashtable DownloadFile = new Hashtable();
                        int FileCount = 0;
                        int DuplicateFileCount = 0;
                        int JsonDuplicateFileCount = 0;
                        foreach (KeyValuePair<string, Dictionary<string, object>> SoundEntity in sounds)
                        {
                            switch (SoundEntity.Value["category"] as string)
                            {
                                case "ambient":
                                case "weather":
                                case "player":
                                case "neutral":
                                case "hostile":
                                case "block":
                                case "master":
                                    //arraylist
                                    var SoundFile = SoundEntity.Value["sounds"] as ArrayList;
                                    if (SoundFile == null) goto case "music";
                                    foreach (string FileName in SoundFile)
                                    {
                                        FileCount++;
                                        string Url = "http://www.bangbang93.com/bmcl/resources/" + "sounds/" + FileName + ".ogg";
                                        string SoundName = MeCore.Config.MCPath + @"assets\sounds\" + FileName + ".ogg";
                                        DataRow[] result = _dt.Select("FileName = " + "'sounds/" + FileName + ".ogg'");
                                        if (result.Count() != 0)
                                        {
                                            DuplicateFileCount++;
                                            continue;
                                        }
                                        if (DownloadFile.ContainsKey(Url))
                                        {
                                            JsonDuplicateFileCount++;
                                            continue;
                                        }
                                        DownloadFile.Add(Url, SoundName);
                                    }
                                    break;
                                case "music":
                                    var MusicFile = SoundEntity.Value["sounds"] as ArrayList;
                                    foreach (Dictionary<string, object> music in MusicFile)
                                    {
                                        if (!music.ContainsKey("stream")) continue;
                                        if ((bool)music["stream"] == false) continue;
                                        FileCount++;
                                        string Url = "http://www.bangbang93.com/bmcl/resources/" + "sounds/" + music["name"] + ".ogg";
                                        string SoundName = MeCore.Config.MCPath + @"\assets\sounds\" + music["name"] + ".ogg";
                                        DataRow[] result = _dt.Select("FileName = " + "'sounds/" + music["name"] as string + ".ogg'");
                                        if (result.Count() != 0)
                                        {
                                            DuplicateFileCount++;
                                            continue;
                                        }
                                        if (DownloadFile.ContainsKey(Url))
                                        {
                                            JsonDuplicateFileCount++;
                                            continue;
                                        }
                                        DownloadFile.Add(Url, SoundName);
                                    }
                                    break;
                                case "record":
                                    var RecordFile = SoundEntity.Value["sounds"] as ArrayList;
                                    if (RecordFile[0] is string)
                                        goto case "master";
                                    else
                                        goto case "music";

                            }
                        }
                        Logger.log(string.Format("共计{0}个文件，{1}个文件重复,{2}个文件json内部重复，{3}个文件待下载", FileCount, DuplicateFileCount, JsonDuplicateFileCount, DownloadFile.Count));
                        FrmDownload frmDownload = new FrmDownload(DownloadFile);
                        frmDownload.Show();*/
        }

        #endregion
        #region DLForge
        readonly ForgeVersionList _forgeVer = new ForgeVersionList();
        private void butReload_Click(object sender, RoutedEventArgs e)
        {
            if (butReload.Content.ToString() == LangManager.GetLangFromResource("btnReForgeGetting"))
                return;
            butReload.Content = LangManager.GetLangFromResource("btnReForgeGetting");
            butReload.IsEnabled = false;
            RefreshForgeVersionList();
        }
        private void RefreshForgeVersionList()
        {
            treeForgeVer.Items.Add(LangManager.GetLangFromResource("ForgeListGetting"));
            _forgeVer.ForgePageReadyEvent += ForgeVer_ForgePageReadyEvent;
            _forgeVer.GetVersion();
        }
        void ForgeVer_ForgePageReadyEvent()
        {
            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(() =>
            {
                treeForgeVer.Items.Clear();
                foreach (TreeViewItem t in _forgeVer.GetNew())
                {
                    treeForgeVer.Items.Add(t);
                }
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
                downer.Headers.Add("User-Agent", "MetoCraft" + MeCore.version);
                downer.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs e) {
                    MeCore.Invoke(new Action(() => task.setTaskStatus(e.ProgressPercentage + "%")));
                };
                downer.DownloadFile(url, "forge.jar");
            }));
            MeCore.MainWindow.addTask(task.setThread(thDL).setTask("下載Forge安裝檔"));
        }
        private void treeForgeVer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.treeForgeVer.SelectedItem == null)
                return;
            if (this.treeForgeVer.SelectedItem is string)
            {
                DownloadForge(this.treeForgeVer.SelectedItem as string);
            }
        }
        #endregion
        private void butDown1_Click(object sender, RoutedEventArgs e)
        {
            gridMC.Visibility = Visibility.Hidden;
            gridLib.Visibility = Visibility.Visible;
        }

        private void butUp1_Click(object sender, RoutedEventArgs e)
        {
            gridLib.Visibility = Visibility.Hidden;
            gridMC.Visibility = Visibility.Visible;
        }
        private void butDown2_Click(object sender, RoutedEventArgs e)
        {
            gridLib.Visibility = Visibility.Hidden;
            gridAssets.Visibility = Visibility.Visible;
        }

        private void butUp2_Click(object sender, RoutedEventArgs e)
        {
            gridAssets.Visibility = Visibility.Hidden;
            gridLib.Visibility = Visibility.Visible;
        }
        private void butDown3_Click(object sender, RoutedEventArgs e)
        {
            gridAssets.Visibility = Visibility.Hidden;
            gridForge.Visibility = Visibility.Visible;
        }

        private void butUp3_Click(object sender, RoutedEventArgs e)
        {
            gridForge.Visibility = Visibility.Hidden;
            gridAssets.Visibility = Visibility.Visible;
        }
        public void setLblColor(Color color)
        {
            labDownInfo.Foreground = new SolidColorBrush(color);
            lblDLI.Foreground = new SolidColorBrush(color);
            lblDr.Foreground = new SolidColorBrush(color);
            lblTitle.Foreground = new SolidColorBrush(color);
            lblTitle_Copy.Foreground = new SolidColorBrush(color);
            lblTitle_Copy1.Foreground = new SolidColorBrush(color);
            lblVer.Foreground = new SolidColorBrush(color);
            lblVer_Copy.Foreground = new SolidColorBrush(color);
        }

    }
}
