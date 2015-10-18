using MetoCraft.Assets;
using MetoCraft.Lang;
using MetoCraft.Play;
using MetoCraft.util;
using MetoCraft.Versions;
using System;
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
            var getJson = (HttpWebRequest)WebRequest.Create(MeCore.UrlDownloadBase + "versions/versions.json");
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
                    if (MessageBox.Show(LangManager.GetLangFromResource("RemoteVerFailedTimeout") + "\n" + ex.Message, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                        {
                            ErrorReport er = new ErrorReport(ex);
                            er.ShowDialog();
                        }));
                    }
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        butRefresh.Content = LangManager.GetLangFromResource("Refresh");
                        butRefresh.IsEnabled = true;
                    }));
                }
                catch (TimeoutException ex)
                {
                    if (MessageBox.Show(LangManager.GetLangFromResource("RemoteVerFailedTimeout") + "\n" + ex.Message, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                        {
                            ErrorReport er = new ErrorReport(ex);
                            er.ShowDialog();
                        }));
                    }
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
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
            var selectVer = listRemoteVer.SelectedItem as DataRowView;
            if (selectVer != null)
            {
                var selectver = selectVer[0] as string;
                var downpath = new StringBuilder(MeCore.Config.MCPath + @"\versions\");
                downpath.Append(selectver).Append("\\");
                downpath.Append(selectver).Append(".jar");
                var downer = new WebClient();
                downer.Headers.Add("User-Agent", "MetoCraft" + MeCore.version);
                var downurl = new StringBuilder(MeCore.UrlDownloadBase);
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
                    MetoCraft.Logger.log("download:" + downjsonfile);
                    downer.DownloadFile(new Uri(downjsonfile), downjsonpath);
                    MetoCraft.Logger.log("download:" + downurl);
                    downer.DownloadFileAsync(new Uri(downurl.ToString()), downpath.ToString());
                    _downedtime = Environment.TickCount - 1;
                    _downed = 0;
                    //                    BmclCore.MainWindow.SwitchDownloadGrid(Visibility.Visible);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n");
                    butDLMC.Content = LangManager.GetLangFromResource("Download");
                    butDLMC.IsEnabled = true;
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
            MetoCraft.Logger.log("Success to download client file.");
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
        private readonly string _urlLib = MeCore.UrlLibrariesBase;
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
                        new ErrorReport(ex).Show();
                    }));
                }
            }
        }

        private void butDLLib_Click(object sender, RoutedEventArgs e)
        {
            var thDL = new Thread(new ThreadStart(delegate
            {
                int i = 0;
                foreach (string libfile in libs)
                {
                    i++;
                    MeCore.Invoke(new Action(() => lblDLI.Content = i + "/" + libs.Count()));
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
                    MeCore.Invoke(new Action(() => lblDLI.Content = i + "/" + natives.Count()));
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
            }));
            thDL.Start();
        }

        #endregion
        #region DLAsset
        Dictionary<string, AssetsEntity> asset;
        private readonly string _urlDownload = MeCore.UrlDownloadBase;
        private readonly string _urlResource = MeCore.UrlResourceBase;
        KMCCC.Launcher.Version _ver;
        bool _init = true;
        private void listVerFAsset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listVerFAsset.SelectedIndex != -1)
            {
                listLib.DataContext = null;
                try
                {
                    _ver = MeCore.MainWindow.gridPlay.versions[listVerFAsset.SelectedIndex];
                    var thGet = new Thread(new ThreadStart(delegate
                    {
                        string gameVersion = _ver.Assets;
                        try
                        {
                            _downer.DownloadStringAsync(new Uri(_urlDownload + "indexes/" + gameVersion + ".json"));
                            Logger.info(_urlDownload + "indexes/" + gameVersion + ".json");
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
                    new ErrorReport(ex).Show();
                }
            }
        }

        private void butDLAsset_Click(object sender, RoutedEventArgs e)
        {
            var thGet = new Thread(new ThreadStart(delegate
            {
                int i = 0;
                foreach (KeyValuePair<string, AssetsEntity> entity in asset)
                {
                    i++;
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        lblDr.Content = i + "/" + asset.Count.ToString(CultureInfo.InvariantCulture);
                    }));
                    string url = _urlResource + entity.Value.hash.Substring(0, 2) + "/" + entity.Value.hash;
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
                                MeCore.NIcon.ShowBalloonTip(3000, Lang.LangManager.GetLangFromResource("SyncAssetsFinish"));
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
            thGet.Start();
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
                    dt.Rows.Add(new object[] { entity.Key, entity.Value.size, entity.Value.hash, File.Exists(entity.Key) });
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
                    new ErrorReport(ex).Show();
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
                            dt.Rows.Add(new object[] { entity.Key, entity.Value.size, entity.Value.hash, File.Exists(entity.Key) });
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
                            new ErrorReport(ex).Show();
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
        #endregion
        private void butDown1_Click(object sender, RoutedEventArgs e)
        {
            var mover = new ThicknessAnimationUsingKeyFrames();
            mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0)));
            mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, -(ActualHeight), 0, (ActualHeight)), TimeSpan.FromSeconds(0.2)));
            var mover1 = new ThicknessAnimationUsingKeyFrames();
            mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, (ActualHeight), 0, -(ActualHeight)), TimeSpan.FromSeconds(0)));
            mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0.2)));
            var mover2 = new ThicknessAnimationUsingKeyFrames();
            mover2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, (ActualHeight) * 2, 0, -(ActualHeight) * 2), TimeSpan.FromSeconds(0)));
            mover2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, (ActualHeight), 0, -(ActualHeight)), TimeSpan.FromSeconds(0.2)));
            gridMC.BeginAnimation(MarginProperty, mover);
            gridLib.BeginAnimation(MarginProperty, mover1);
            gridAssets.BeginAnimation(MarginProperty, mover2);
        }

        private void butUp1_Click(object sender, RoutedEventArgs e)
        {
            var mover = new ThicknessAnimationUsingKeyFrames();
            mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, -(ActualHeight), 0, (ActualHeight)), TimeSpan.FromSeconds(0)));
            mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0.2)));
            var mover1 = new ThicknessAnimationUsingKeyFrames();
            mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0)));
            mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, (ActualHeight), 0, -(ActualHeight)), TimeSpan.FromSeconds(0.2)));
            var mover2 = new ThicknessAnimationUsingKeyFrames();
            mover2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, (ActualHeight), 0, -(ActualHeight)), TimeSpan.FromSeconds(0)));
            mover2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, (ActualHeight) * 2, 0, -(ActualHeight) * 2), TimeSpan.FromSeconds(0.2)));
            gridMC.BeginAnimation(MarginProperty, mover);
            gridLib.BeginAnimation(MarginProperty, mover1);
            gridAssets.BeginAnimation(MarginProperty, mover2);
        }
        private void butDown2_Click(object sender, RoutedEventArgs e)
        {
            var mover = new ThicknessAnimationUsingKeyFrames();
            mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, -(ActualHeight), 0, (ActualHeight)), TimeSpan.FromSeconds(0)));
            mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, -(ActualHeight) * 2, 0, (ActualHeight) * 2), TimeSpan.FromSeconds(0.2)));
            var mover1 = new ThicknessAnimationUsingKeyFrames();
            mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0)));
            mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, -(ActualHeight), 0, (ActualHeight)), TimeSpan.FromSeconds(0.2)));
            var mover2 = new ThicknessAnimationUsingKeyFrames();
            mover2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, (ActualHeight), 0, -(ActualHeight)), TimeSpan.FromSeconds(0)));
            mover2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0.2)));
            gridMC.BeginAnimation(MarginProperty, mover);
            gridLib.BeginAnimation(MarginProperty, mover1);
            gridAssets.BeginAnimation(MarginProperty, mover2);
        }

        private void butUp2_Click(object sender, RoutedEventArgs e)
        {
            var mover = new ThicknessAnimationUsingKeyFrames();
            mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, -(ActualHeight) * 2, 0, (ActualHeight) * 2), TimeSpan.FromSeconds(0)));
            mover.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, -(ActualHeight), 0, (ActualHeight)), TimeSpan.FromSeconds(0.2)));
            var mover1 = new ThicknessAnimationUsingKeyFrames();
            mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, -(ActualHeight), 0, (ActualHeight)), TimeSpan.FromSeconds(0)));
            mover1.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0.2)));
            var mover2 = new ThicknessAnimationUsingKeyFrames();
            mover2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0), TimeSpan.FromSeconds(0)));
            mover2.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, (ActualHeight), 0, -(ActualHeight)), TimeSpan.FromSeconds(0.2)));
            gridMC.BeginAnimation(MarginProperty, mover);
            gridLib.BeginAnimation(MarginProperty, mover1);
            gridAssets.BeginAnimation(MarginProperty, mover2);
        }
    }
}
