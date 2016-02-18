using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MTMCL
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
            lblLaVer1.Content = MeCore.version;
#if DEBUG
            lblLaVer1.Content += " DEBUG";
#endif
            lblKM1.Content = KMCCC.Launcher.Reporter.KMCCC_TYPE + " " + KMCCC.Launcher.Reporter.Version;
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
        public void loadOSData()
        {
            string oss = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            int ins = 0;
            ins = oss.IndexOf("Pro");
            string[] ss = oss.Split(' ');
            oss = oss.Substring(0, (ss[0] + " " + ss[1]).Length) + " " + Lang.LangManager.GetLangFromResource(oss.Substring((ss[0] + " " + ss[1] + " ").Length).Replace(' ', '_'));
            /*if (ins != -1)
            {
                oss = oss.Substring(0, ins) + Lang.LangManager.GetLangFromResource("Pro");
            }*/
            lblCVer1.Content = oss;
            lblBit1.Content = (Environment.Is64BitOperatingSystem ? Lang.LangManager.GetLangFromResource("COMx64") : Lang.LangManager.GetLangFromResource("COMx86")) + ", " + (Environment.Is64BitProcess ? Lang.LangManager.GetLangFromResource("PCSx64") : Lang.LangManager.GetLangFromResource("PCSx86"));
            StringBuilder builder = new StringBuilder();
            if (Directory.Exists(@"C:\Program Files\Java"))
            {
                if (Directory.GetDirectories(@"C:\Program Files\Java") != null)
                {
                    builder.Append(Environment.Is64BitOperatingSystem ? "x64" : "x86");
                }
            }
            if (Directory.Exists(@"C:\Program Files (x86)\Java"))
            {
                if (Directory.GetDirectories(@"C:\Program Files (x86)\Java") != null)
                {
                    if (builder.Length != 0)
                    {
                        builder.Append(" & x86");
                    }
                    else
                    {
                        builder.Append("x86");
                    }
                }
            }
            lblJava1.Content = builder.ToString();

        }
        public void setLblColor(Color color)
        {
            lblBit.Foreground = new SolidColorBrush(color);
            lblBit1.Foreground = new SolidColorBrush(color);
            lblCOM.Foreground = new SolidColorBrush(color);
            lblCVer.Foreground = new SolidColorBrush(color);
            lblCVer1.Foreground = new SolidColorBrush(color);
            lblJava.Foreground = new SolidColorBrush(color);
            lblJava1.Foreground = new SolidColorBrush(color);
            lblKM.Foreground = new SolidColorBrush(color);
            lblKM1.Foreground = new SolidColorBrush(color);
            lblLauncher.Foreground = new SolidColorBrush(color);
            lblLaVer.Foreground = new SolidColorBrush(color);
            lblLaVer1.Foreground = new SolidColorBrush(color);
        }
    }
}
