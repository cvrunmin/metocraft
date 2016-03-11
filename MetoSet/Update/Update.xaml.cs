using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTMCL.Update
{
    /// <summary>
    /// Update.xaml 的互動邏輯
    /// </summary>
    public partial class Update
    {
        private readonly int _build;
        private readonly string _url;
        private readonly System.Net.WebClient _client = new System.Net.WebClient();
        public Update(int build, string url)
        {
            InitializeComponent();
            _build = build;
            _url = url;
            _client.DownloadProgressChanged += ClientOnDownloadProgressChanged;
            _client.DownloadFileCompleted += ClientOnDownloadFileCompleted;
            _client.DownloadFileAsync(new Uri(_url), "MTMCL." + _build + ".exe");
        }
        private async void ClientOnDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            _client.Dispose();
            App.AboutToExit();
            lblProcess.Content = Lang.LangManager.GetLangFromResource("Restarting");
            await System.Threading.Tasks.Task.Delay(1500);
            //Process.Start("MTMCL." + _build + ".exe", "-Update");
            Process.Start(new ProcessStartInfo { FileName = "MTMCL." + _build + ".exe", Arguments = "-UpdateReplace " + Process.GetCurrentProcess().ProcessName });
            Logger.log(string.Format("MTMCL V2 Ver.{0} exited to upodate", MeCore.version));
            Environment.Exit(0);
        }

        private void ClientOnDownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            lblProcess.Content = string.Format(Lang.LangManager.GetLangFromResource("TaskUpdating"), downloadProgressChangedEventArgs.ProgressPercentage.ToString(System.Globalization.CultureInfo.InvariantCulture) + "%");
        }
    }
}
