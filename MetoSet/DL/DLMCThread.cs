using MetoCraft.Lang;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace MetoCraft.DL
{
    class DLMCThread
    {
        private readonly string version;
        private void Run() {
                var selectver = version;
                var downpath = new StringBuilder(MeCore.Config.MCPath + @"\versions\");
                downpath.Append(selectver).Append("\\");
                downpath.Append(selectver).Append(".jar");
                var downer = new WebClient();
                downer.Headers.Add("User-Agent", "MetoCraft" + MeCore.version);
                var downurl = new StringBuilder(MetoCraft.Resources.UrlReplacer.getDownloadUrl());
                downurl.Append(@"versions\");
                downurl.Append(selectver).Append("\\");
                downurl.Append(selectver).Append(".jar");
#if DEBUG
                MessageBox.Show(downpath + "\n" + downurl);
#endif
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
                    MessageBox.Show(ex.Message + "\n");
                }
        }
        int _downedtime;
        int _downed;
        void downer_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
//            info.Append(e.ProgressPercentage.ToString(CultureInfo.InvariantCulture)).Append("%");
        }
        void downer_DownloadClientFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Logger.log("Success to download client file.");
            MessageBox.Show(LangManager.GetLangFromResource("RemoteVerDownloadSuccess"));
            MeCore.MainWindow.gridPlay.LoadVersionList();
        }
    }
}
