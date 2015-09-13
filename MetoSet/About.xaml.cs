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

namespace MetoSet
{
    /// <summary>
    /// About.xaml 的互動邏輯
    /// </summary>
    public partial class About : Grid
    {
        public About()
        {
            InitializeComponent();
        }

        private void PGrid_Loaded(object sender, RoutedEventArgs e)
        {
            loadOSData();
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
        public void loadOSData() {
            string oss = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            int ins = 0;
            if ((ins = oss.IndexOf("Pro")) != -1)
            {
                oss = oss.Substring(0, oss.Length - ins) + Lang.LangManager.GetLangFromResource("Pro");
            }
            lblCVer1.Content = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            lblBit1.Content = (Environment.Is64BitOperatingSystem ? Lang.LangManager.GetLangFromResource("COMx64") : Lang.LangManager.GetLangFromResource("COMx86")) + "," + (Environment.Is64BitProcess ? Lang.LangManager.GetLangFromResource("PCSx64") : Lang.LangManager.GetLangFromResource("PCSx86"));
            lblJava1.Content = (Directory.Exists(@"C:\Program Files\Java") && Directory.GetDirectories(@"C:\Program Files\Java") != null ? (Environment.Is64BitOperatingSystem ? "x64" : "x86") : "") + (Directory.Exists(@"C:\Program Files (x86)\Java") && Directory.GetDirectories(@"C:\Program Files\Java") != null ? " & x86" : "");
        }
    }
}
