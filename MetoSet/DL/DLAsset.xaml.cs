using MetoCraft.Assets;
using MetoCraft.Play;
using MetoCraft.util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetoCraft.DL
{
    /// <summary>
    /// DLLib.xaml 的互動邏輯
    /// </summary>
    public partial class DLAsset : Grid
    {
        Dictionary<string, AssetsEntity> asset;
        private readonly WebClient _downer = new WebClient();
        private readonly string _urlDownload = MeCore.UrlDownloadBase;
        private readonly string _urlResource = MeCore.UrlResourceBase;
        KMCCC.Launcher.Version _ver;
        bool _init = true;
        public DLAsset()
        {
            InitializeComponent();
        }

        private void listVer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listVer.SelectedIndex != -1)
            {
                listLib.DataContext = null;
                try
                {
                    _ver = MeCore.MainWindow.gridPlay.gridVer.versions[listVer.SelectedIndex];
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

        private void butDL_Click(object sender, RoutedEventArgs e)
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

        private void butReload_Click(object sender, RoutedEventArgs e)
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
        }
}
