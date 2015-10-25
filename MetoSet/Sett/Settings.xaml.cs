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
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Threading;

namespace MetoCraft.Sett
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
        private void butBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "PNG(*.png)|*.png|JPG(*.jpg)|*.jpg|Bitmap(*.bmp)|*.bmp|All Files(*.*)|*.*";
            dialog.ShowDialog();
            txtBoxP.Text = dialog.FileName;
        }

        private void butSave_Click(object sender, RoutedEventArgs e)
        {
            saveAndRender();
        }

        private void butReset_Click(object sender, RoutedEventArgs e)
        {
            txtBoxP.Text = "default";
            saveAndRender();
        }

        private void saveAndRender() {
            MeCore.Config.BackGround = txtBoxP.Text;
            MeCore.Config.Save(null);
            try
            {
                if (MeCore.Config.BackGround.Equals("default", StringComparison.InvariantCultureIgnoreCase))
                {
                    var da = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.25));
                    MeCore.MainWindow.BeginAnimation(OpacityProperty, da);
                    MeCore.MainWindow.gridParent.Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/bg.png")),
                        Stretch = Stretch.UniformToFill
                    };
                    da = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
                    MeCore.MainWindow.BeginAnimation(OpacityProperty, da);
                }
                else
                {
                    var da = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.25));
                    MeCore.MainWindow.BeginAnimation(OpacityProperty, da);
                    MeCore.MainWindow.gridParent.Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(MeCore.Config.BackGround)),
                        Stretch = Stretch.Fill
                    };
                    da = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
                    MeCore.MainWindow.BeginAnimation(OpacityProperty, da);
                }
            }
            catch (Exception ex)
            {
                new ErrorReport(ex).ShowDialog();
                MeCore.MainWindow.Close();
                System.Windows.Forms.Application.Restart();

            }
        }

        private void butColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.ShowDialog();
            txtBoxColor.Text = (dialog.Color.ToArgb() & 0xFFFFFFFF).ToString();
        }

        private void butCSave_Click(object sender, RoutedEventArgs e)
        {
            MeCore.Config.color = System.Drawing.Color.FromArgb((int)(uint.Parse(txtBoxColor.Text) & 0x7FFFFFFF));
            MeCore.Config.Save(null);
            try
            {
                {
                    var da = new ColorAnimation(Color.FromArgb(MeCore.Config.color.A, MeCore.Config.color.R, MeCore.Config.color.G, MeCore.Config.color.B) , TimeSpan.FromSeconds(0.25));
                    MeCore.MainWindow.gridMenu.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
                    MeCore.MainWindow.gridAbout.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
                    MeCore.MainWindow.gridDL.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
//                    MeCore.MainWindow.gridPlay.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
                    gridParent.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
                    var col = System.Drawing.Color.FromArgb((int)(~((uint)MeCore.Config.color.ToArgb())));
                    MeCore.MainWindow.gridAbout.setLblColor(Color.FromRgb(col.R, col.G, col.B));
                    MeCore.MainWindow.gridDL.setLblColor(Color.FromRgb(col.R, col.G, col.B));
                    MeCore.MainWindow.gridPlay.setLblColor(Color.FromRgb(col.R, col.G, col.B));
                    setLblColor(Color.FromRgb(col.R, col.G, col.B));
                }
            }
            catch (Exception ex)
            {
                new ErrorReport(ex).ShowDialog();
                MeCore.MainWindow.Close();
                System.Windows.Forms.Application.Restart();

            }
        }
        public void setLblColor(Color color)
        {
            lblBG.Foreground = new SolidColorBrush(color);
            lblColor.Foreground = new SolidColorBrush(color);
            lblDLUrl.Foreground = new SolidColorBrush(color);
            lblLangTitle.Foreground = new SolidColorBrush(color);
        }
    }
}
