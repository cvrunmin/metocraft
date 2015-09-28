using MetoCraft.Lang;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
    /// DLForge.xaml 的互動邏輯
    /// </summary>
    public partial class DLForge : Grid
    {
        readonly Forge.ForgeVersionList _forgeVer;
        int _downedtime;
        int _downed;

        public DLForge()
        {
            InitializeComponent();
        }

        private void RefreshForgeVersionList()
        {
            treeForgeVer.Items.Add(LangManager.GetLangFromResource("ForgeListGetting"));
            _forgeVer.ForgePageReadyEvent += ForgeVer_ForgePageReadyEvent;
            _forgeVer.GetVersion();
        }

        void ForgeVer_ForgePageReadyEvent()
        {
            treeForgeVer.Items.Clear();
            foreach (TreeViewItem t in _forgeVer.GetNew())
            {
                treeForgeVer.Items.Add(t);
            }
            butReForge.Content = LangManager.GetLangFromResource("butReForge");
            butReForge.IsEnabled = true;
        }

        private void btnLastForge_Click(object sender, RoutedEventArgs e)
        {
            DownloadForge("Latest");
        }

        private void butReForge_Click(object sender, RoutedEventArgs e)
        {
            if (butReForge.Content.ToString() == LangManager.GetLangFromResource("butReForgeGetting"))
                return;
            butReForge.Content = LangManager.GetLangFromResource("butReForgeGetting");
            butReForge.IsEnabled = false;
            RefreshForgeVersionList();
        }

        private void DownloadForge(string ver)
        {
            if (!_forgeVer.ForgeDownloadUrl.ContainsKey(ver))
            {
                MessageBox.Show(LangManager.GetLangFromResource("ForgeDoNotSupportInstaller"));
                return;
            }
//            MeCore.Invoke(new Action(() => MeCore.MainWindow.SwitchDownloadGrid(Visibility.Visible)));
            Uri url;
            if (MeCore.Config.DownloadSource == 1)
            {
                url = new Uri(_forgeVer.ForgeDownloadUrl[ver].Replace("files.minecraftforge.net", "bmclapi2.bangbang93.com"));
            }
            else
            {
                url = new Uri(_forgeVer.ForgeDownloadUrl[ver]);
            }
            var downer = new WebClient();
            downer.Headers.Add("User-Agent", "MetoCraft" + MeCore.version);
            downer.DownloadProgressChanged += downer_DownloadProgressChanged;
            downer.DownloadFileCompleted += downer_DownloadForgeCompleted;
            _downedtime = Environment.TickCount - 1;
            _downed = 0;
            var w = new StreamWriter(MeCore.Config.MCPath + "\\launcher_profiles.json");
//            w.Write(Resource.NormalProfile.Profile);
            w.Close();
            downer.DownloadFileAsync(url, "forge.jar");
        }


        void downer_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ChangeDownloadProgress(e.BytesReceived, e.TotalBytesToReceive);
            var info = new StringBuilder(LangManager.GetLangFromResource("DownloadSpeedInfo"));
            try
            {
                info.Append(((e.BytesReceived - _downed) / ((Environment.TickCount - _downedtime) / 1000.0) / 1024.0).ToString("F2")).Append("KB/s,");
            }
            catch (DivideByZeroException) { info.Append("0B/s,"); }
            info.Append(e.ProgressPercentage.ToString(CultureInfo.InvariantCulture)).Append("%");
            SetDownloadInfo(info.ToString());
        }
        public void SetDownloadInfo(string info)
        {
            labDownInfo.Content = info;
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
        void downer_DownloadForgeCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    //Clipboard.SetText(txtInsPath.Text);
                    MessageBox.Show(LangManager.GetLangFromResource("ForgeInstallInfo"));
                }
                catch
                {
                    MessageBox.Show(LangManager.GetLangFromResource("ForgeCopyError"));
                }
                var forgeIns = new Process();
                if (!File.Exists(MeCore.Config.Javaw))
                {
                    MessageBox.Show(LangManager.GetLangFromResource("ForgeJavaError"));
                    return;
                }
                forgeIns.StartInfo.FileName = MeCore.Config.Javaw;
                forgeIns.StartInfo.Arguments = "-jar \"" + MeCore.BaseDirectory + "\\forge.jar\"";
                Logger.log(forgeIns.StartInfo.Arguments);
                forgeIns.Start();
                forgeIns.WaitForExit();
                MeCore.MainWindow.gridPlay.gridVer.LoadVersionList();
            }
            else
            {
                Logger.error(e.Error);
                new ErrorReport(e.Error).Show();
            }

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
        private void txtInsPath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Clipboard.SetText(txtInsPath.Text);
                MessageBox.Show(LangManager.GetLangFromResource("ForgeCopySuccess"));
            }
            catch
            {
                MessageBox.Show(LangManager.GetLangFromResource("ForgeCopyError"));
            }
        }

        private void treeForgeVer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.treeForgeVer.SelectedItem == null)
                return;
            if (this.treeForgeVer.SelectedItem is string)
            {
                if (_forgeVer.ForgeChangeLogUrl.ContainsKey(this.treeForgeVer.SelectedItem as string))
                {
                    //txtChangeLog.Text = LangManager.GetLangFromResource("FetchingForgeChangeLog");
                    var getLog = new WebClient();
                    getLog.DownloadStringCompleted += GetLog_DownloadStringCompleted;
                    getLog.DownloadStringAsync(new Uri(_forgeVer.ForgeChangeLogUrl[this.treeForgeVer.SelectedItem as string]));
                }
                else
                {
                    MessageBox.Show(LangManager.GetLangFromResource("ForgeDoNotHaveChangeLog"));
                }
            }
        }

        void GetLog_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            //txtChangeLog.Text = e.Result;
        }

        public void RefreshForge()
        {
            this.butReForge_Click(null, null);
        }

    }
}
