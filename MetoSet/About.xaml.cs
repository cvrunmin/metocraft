using Microsoft.Win32;
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
#if DEBUG
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
#else
            lblCVer1.Content = Lang.LangManager.GetLangFromResource("NoCollectingData");
            lblBit1.Content = Lang.LangManager.GetLangFromResource("NoCollectingData");
            lblJava1.Content = Lang.LangManager.GetLangFromResource("NoCollectingData");
#endif
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
