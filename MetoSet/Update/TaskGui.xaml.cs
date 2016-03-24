using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace MTMCL.Update
{
    /// <summary>
    /// TaskGui.xaml 的互動邏輯
    /// </summary>
    public partial class TaskGui : Window
    {
        System.Windows.Forms.Timer timer;
        bool startCount = false;
        Thread _task;
        Process _subTask;
        int hr = 0,min = 0, sec = 0;
        private readonly int _build;
        private readonly string _url;
        private readonly System.Net.WebClient _client = new System.Net.WebClient();
        public TaskGui(int build, string url)
        {
            InitializeComponent();
            _build = build;
            _url = url;
            InitializeComponent();
            _client.DownloadProgressChanged += ClientOnDownloadProgressChanged;
            _client.DownloadFileCompleted += ClientOnDownloadFileCompleted;
            _client.DownloadFileAsync(new Uri(_url), "MTMCL." + _build + ".exe");
        }

        private void ClientOnDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            _client.Dispose();
            App.AboutToExit();
            Thread.Sleep(1000);
            //Process.Start("MTMCL." + _build + ".exe", "-Update");
            Process.Start(new ProcessStartInfo { FileName = "MTMCL." + _build + ".exe", Arguments = "-UpdateReplace " + Process.GetCurrentProcess().ProcessName});
            Logger.log(string.Format("MTMCL V2 Ver.{0} exited to upodate", MeCore.version));
            Close();
            Environment.Exit(0);
        }

        private void ClientOnDownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            setTaskStatus(downloadProgressChangedEventArgs.ProgressPercentage.ToString(System.Globalization.CultureInfo.InvariantCulture) + "%");
        }
        public TaskGui setTask(string name)
        {
            lblTaskName.Content = name;
            return this;
        }
        public TaskGui setTaskStatus(string status)
        {
            lblTaskStatus.Content = status;
            return this;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new System.Windows.Forms.Timer();
            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(this.timer_Tick);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lblTime.Content = DateTime.Now.ToLocalTime().ToShortTimeString();
            if (startCount) {
                ++sec;
                if (sec >= 60) {
                    ++min;
                    sec -= 60;
                    if (min >= 60) {
                        ++hr;
                        min -= 60;
                    }
                }
                setTaskStatus(hr + ":" + toGoodString(min) + ":" + toGoodString(sec));
            }
        }
        private string toGoodString(int i) {
            string s = i.ToString();
            if (s.Length == 1) { return "0" + s; } else return s;
        }
    }
}
