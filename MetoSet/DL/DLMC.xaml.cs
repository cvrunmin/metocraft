using MetoCraft.Versions;
using MetoCraft.Lang;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace MetoCraft.DL
{
    /// <summary>
    /// DLMC.xaml 的互動邏輯
    /// </summary>
    public partial class DLMC : Grid
    {
        public DLMC()
        {
            InitializeComponent();
        }

        private void butRefresh_Click(object sender, RoutedEventArgs e)
        {
            butRefresh.IsEnabled = false;
            listRemoteVer.DataContext = null;
            var rawJson = new DataContractJsonSerializer(typeof(RawVersionListType));
            var getJson = (HttpWebRequest)WebRequest.Create(MeCore.UrlDownloadBase + "versions/versions.json");
            getJson.Timeout = 10000;
            getJson.ReadWriteTimeout = 10000;
            getJson.UserAgent = "Metocraft" + MeCore.version;
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
                    if (MessageBox.Show(LangManager.GetLangFromResource("RemoteVerFailedTimeout") + "\n" + ex.Message, "",MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
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

        private void butDL_Click(object sender, RoutedEventArgs e)
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
                downer.Headers.Add("User-Agent", "Metocraft" + MeCore.version);
                var downurl = new StringBuilder(MeCore.UrlDownloadBase);
                downurl.Append(@"versions\");
                downurl.Append(selectver).Append("\\");
                downurl.Append(selectver).Append(".jar");
#if DEBUG
                MessageBox.Show(downpath + "\n" + downurl);
#endif
                butDL.Content = LangManager.GetLangFromResource("RemoteVerDownloading");
                butDL.IsEnabled = false;
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
                    butDL.Content = LangManager.GetLangFromResource("Download");
                    butDL.IsEnabled = true;
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
            butDL.Content = LangManager.GetLangFromResource("Download");
            butDL.IsEnabled = true;
            MeCore.MainWindow.gridPlay.gridVer.LoadVersionList();
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
    }
}
