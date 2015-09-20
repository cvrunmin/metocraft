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
using MetoCraft.Lang;
using MetoCraft.Resources;

namespace MetoCraft
{
    /// <summary>
    /// About.xaml 的互動邏輯
    /// </summary>
    public partial class Settings : Grid
    {
        public Settings()
        {
            InitializeComponent();
            RefreshLangList();
        }

        private void PGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshLangList();
            if (comboDLSrc.SelectedIndex == -1) { comboDLSrc.SelectedIndex = 0; }
            comboDLSrc.SelectedIndex = MeCore.Config.DownloadSource;
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

        private void listLang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listLang.SelectedItem as string != null)
                if (MeCore.Language.ContainsKey(listLang.SelectedItem as string))
                {
                    LangManager.UseLanguage(MeCore.Language[listLang.SelectedItem as string] as string);
                    MeCore.Config.Lang = LangManager.GetLangFromResource("LangName");
                    MeCore.Config.Save(null);
                }
            MeCore.MainWindow.ChangeLanguage();
        }
        public void RefreshLangList()
        {
            listLang.Items.Clear();
            var langs = LangManager.ListLanuage();
            foreach (var lang in langs)
            {
                listLang.Items.Add(lang);
            }
            listLang.SelectedItem = LangManager.GetLangFromResource("LangName");
        }

        private void comboDLSrc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboDLSrc.SelectedIndex)
            {
                case 0:
                    MeCore.UrlDownloadBase = Url.URL_DOWNLOAD_BASE;
                    MeCore.UrlResourceBase = Url.URL_RESOURCE_BASE;
                    MeCore.UrlLibrariesBase = Url.URL_LIBRARIES_BASE;
                    break;
                case 1:
                    MeCore.UrlDownloadBase = Url.URL_DOWNLOAD_bangbang93;
                    MeCore.UrlResourceBase = Url.URL_RESOURCE_bangbang93;
                    MeCore.UrlLibrariesBase = Url.URL_LIBRARIES_bangbang93;
                    break;
                case 2:
                    MeCore.UrlDownloadBase = Url.URL_DOWNLOAD_rapiddata;
                    MeCore.UrlResourceBase = Url.URL_RESOURCE_rapiddata;
                    MeCore.UrlLibrariesBase = Url.URL_LIBRARIES_rapiddata;
                    break;
                default:
                    goto case 0;
            }
            MeCore.Config.DownloadSource = comboDLSrc.SelectedIndex;
            MeCore.Config.Save(null);
        }
    }
}
