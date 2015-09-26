using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Threading;

namespace MetoCraft
{
    /// <summary>
    /// About.xaml 的互動邏輯
    /// </summary>
    public partial class About : Grid
    {
        public Thread bbGet, rGet;
        public About()
        {
            InitializeComponent();
        }

        private void PGrid_Loaded(object sender, RoutedEventArgs e)
        {
            loadOSData();
            loadUrlStatus();
            lblLaVer1.Content = MeCore.version;
        }
        public string HKLM_GetString(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return "";
                return (string)rk.GetValue(key);
            }
            catch { return ""; }
        }
        private void loadUrlStatus() {
            var bb = (HttpWebRequest)WebRequest.Create(MetoCraft.Resources.Url.URL_DOWNLOAD_bangbang93);
            bb.Timeout = 5000;
            var r = (HttpWebRequest)WebRequest.Create("http://mirrors.rapiddata.org/");
            r.Timeout = 5000;
            bbGet = new Thread(new ThreadStart(delegate
            {
                try
                {
                    var bbans = (HttpWebResponse)bb.GetResponse();
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate { lblBBS.Content = bbans.StatusCode; }));
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate { lblBBS.Content = ex.GetType(); }));
                }
            }));
            rGet = new Thread(new ThreadStart(delegate
            {
                try
                {
                    var rans = (HttpWebResponse)r.GetResponse();
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate { lblrapidS.Content = rans.StatusCode; }));
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate { lblrapidS.Content = ex.GetType(); }));
                }
            }));
            bbGet.SetApartmentState(ApartmentState.STA);
            rGet.SetApartmentState(ApartmentState.STA);
            bbGet.Start();
            rGet.Start();
        }
        public void loadOSData() {
            string oss = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            int ins = 0;
            ins = oss.IndexOf("Pro");
            if (ins != -1)
            {
                oss = oss.Substring(0, ins) + Lang.LangManager.GetLangFromResource("Pro");
            }
            lblCVer1.Content = oss;
            lblBit1.Content = (Environment.Is64BitOperatingSystem ? Lang.LangManager.GetLangFromResource("COMx64") : Lang.LangManager.GetLangFromResource("COMx86")) + ", " + (Environment.Is64BitProcess ? Lang.LangManager.GetLangFromResource("PCSx64") : Lang.LangManager.GetLangFromResource("PCSx86"));
            lblJava1.Content = (Directory.Exists(@"C:\Program Files\Java") && Directory.GetDirectories(@"C:\Program Files\Java") != null ? (Environment.Is64BitOperatingSystem ? "x64" : "x86") : "") + (Directory.Exists(@"C:\Program Files (x86)\Java") && Directory.GetDirectories(@"C:\Program Files\Java") != null ? " & x86" : "");
        }

        private void butBBTry_Click(object sender, RoutedEventArgs e)
        {
            lblBBS.Content = "Connecting";
            var bb = (HttpWebRequest)WebRequest.Create(MetoCraft.Resources.Url.URL_DOWNLOAD_bangbang93);
            bb.Timeout = 2000;
            bbGet = new Thread(new ThreadStart(delegate
            {
                try
                {
                    var bbans = (HttpWebResponse)bb.GetResponse();
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate { lblBBS.Content = bbans.StatusCode; }));
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate { lblBBS.Content = ex.GetType(); }));
                }
            }));
            bbGet.SetApartmentState(ApartmentState.STA);
            bbGet.Start();
        }

        private void butRTry_Click(object sender, RoutedEventArgs e)
        {
            lblrapidS.Content = "Connecting";
            var r = (HttpWebRequest)WebRequest.Create("http://mirrors.rapiddata.org/");
            r.Timeout = 2000;
            rGet = new Thread(new ThreadStart(delegate
            {
                try
                {
                    var rans = (HttpWebResponse)r.GetResponse();
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate { lblrapidS.Content = rans.StatusCode; }));
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate { lblrapidS.Content = ex.GetType(); }));
                }
            }));
            rGet.SetApartmentState(ApartmentState.STA);
            rGet.Start();
        }

        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {
/*            if (thGet.IsAlive) {
                MessageBox.Show("aaaaa");
            }
/*            thGet.Join();
            thGet.Interrupt();
            thGet.Abort(0);*/
        }
    }
}
