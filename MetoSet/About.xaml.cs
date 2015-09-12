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
            lblCVer1.Content = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            lblBit1.Content = (Environment.Is64BitOperatingSystem ? "64-bit Operating System" : "32-bit Operating System") + "," + (Environment.Is64BitProcess ? "64-bit Processor" : "32-bit Processor");
            lblJava1.Content = (Directory.Exists(@"C:\Program Files\Java") && Directory.GetDirectories(@"C:\Program Files\Java") != null ? (Environment.Is64BitOperatingSystem ? "x64": "x86"):"") + (Directory.Exists(@"C:\Program Files (x86)\Java") && Directory.GetDirectories(@"C:\Program Files\Java") != null ? " & x86" : "");
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
    }
}
