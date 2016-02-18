using Microsoft.Win32;
using MTMCL.Lang;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MTMCL.Sett
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
            MeCore.Config.DownloadSource = comboDLSrc.SelectedIndex;
            if (MeCore.Config.DownloadSource == 1)
            {
                //System.Windows.MessageBox.Show("使用BMCLAPI時需要限制連線頻率\n使用BMCLAPI时需要限制连线频率\nFrequency of connecting to BMCLAPI have to be limited", "注意 Attention", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            MeCore.Config.Save(null);
        }
        private void butBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "PNG(*.png)|*.png|JPG(*.jpg, *.jpeg)|*.jpg; *.jpeg|TIFF(*.tif, *.tiff)|*.tif; *.tiff|Bitmap(*.bmp)|*.bmp";
            dialog.ShowDialog();
            txtBoxP.Text = dialog.FileName;
        }

        private void butSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void butReset_Click(object sender, RoutedEventArgs e)
        {
            txtBoxP.Text = "default";
            Save();
        }

        private void Save()
        {
            MeCore.Config.BackGround = txtBoxP.Text;
            uint c;
            if (uint.TryParse(txtBoxColor.Text, out c))
            {
                MeCore.Config.color[0] = System.Drawing.Color.FromArgb((int)(c & 0xFFFFFF)).R;
                MeCore.Config.color[1] = System.Drawing.Color.FromArgb((int)(c & 0xFFFFFF)).G;
                MeCore.Config.color[2] = System.Drawing.Color.FromArgb((int)(c & 0xFFFFFF)).B;
            }
            else
            {
                MeCore.Config.color[0] = System.Drawing.Color.FromArgb(0xFFFFFF).R;
                MeCore.Config.color[1] = System.Drawing.Color.FromArgb(0xFFFFFF).G;
                MeCore.Config.color[2] = System.Drawing.Color.FromArgb(0xFFFFFF).B;
            }
            MeCore.Config.Save(null);
            Render();
            RenderColor();
        }

        private void Render()
        {
            if (butSave.IsEnabled)
            {
                try
                {
                    if (MeCore.Config.BackGround.Equals("default", StringComparison.InvariantCultureIgnoreCase) | string.IsNullOrWhiteSpace(MeCore.Config.BackGround))
                    {
                        var da = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.25));
                        MeCore.MainWindow.gridParent.BeginAnimation(OpacityProperty, da);
                        MeCore.MainWindow.gridParent.Background = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri(MeCore.DefaultBG)),
                            Stretch = Stretch.UniformToFill
                        };
                        da = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
                        MeCore.MainWindow.gridParent.BeginAnimation(OpacityProperty, da);
                    }
                    else
                    {
                        var da = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.25));
                        MeCore.MainWindow.gridParent.BeginAnimation(OpacityProperty, da);
                        MeCore.MainWindow.gridParent.Background = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri(MeCore.Config.BackGround)),
                            Stretch = Stretch.Fill
                        };
                        da = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
                        MeCore.MainWindow.gridParent.BeginAnimation(OpacityProperty, da);
                    }
                }
                catch (Exception ex)
                {
                    //new ErrorReport(ex).ShowDialog();
                    MeCore.Config.BackGround = "default";
                    MeCore.Config.Save(null);
                    MeCore.MainWindow.Close();
                    System.Windows.Forms.Application.Restart();
                }
            }
            else
            {
                var da = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.25));
                MeCore.MainWindow.gridParent.BeginAnimation(OpacityProperty, da);
                MeCore.MainWindow.gridParent.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(MeCore.DefaultBG)),
                    Stretch = Stretch.UniformToFill
                };
                da = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.25));
                MeCore.MainWindow.gridParent.BeginAnimation(OpacityProperty, da);
            }
        }

        private void butColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.ShowDialog();
            txtBoxColor.Text = (dialog.Color.ToArgb() & 0xFFFFFF).ToString();
        }

        public void loadConfig()
        {
            comboDLSrc.SelectedIndex = MeCore.Config.DownloadSource;
            txtBoxP.Text = MeCore.Config.BackGround;
            txtBoxColor.Text = (ByteArrayToArgb() & 0xFFFFFF).ToString();
            butExpandTask.IsChecked = MeCore.Config.ExpandTaskGui;
            //            sliderTransparency.Value = MeCore.Config.WindowTransparency;
            Render();
            RenderColor();
            //            RenderTransparency();
        }
        private uint ByteArrayToArgb()
        {
            uint color = 0;
            color += MeCore.Config.color[2];
            color += (uint)(MeCore.Config.color[1] << 8);
            color += (uint)(MeCore.Config.color[0] << 16);
            return color;
        }
        private void butCSave_Click(object sender, RoutedEventArgs e)
        {
            uint c;
            if (uint.TryParse(txtBoxColor.Text, out c))
            {
                MeCore.Config.color[0] = System.Drawing.Color.FromArgb((int)(c & 0xFFFFFF)).R;
                MeCore.Config.color[1] = System.Drawing.Color.FromArgb((int)(c & 0xFFFFFF)).G;
                MeCore.Config.color[2] = System.Drawing.Color.FromArgb((int)(c & 0xFFFFFF)).B;
            }
            else
            {
                MeCore.Config.color[0] = System.Drawing.Color.FromArgb(0xFFFFFF).R;
                MeCore.Config.color[1] = System.Drawing.Color.FromArgb(0xFFFFFF).G;
                MeCore.Config.color[2] = System.Drawing.Color.FromArgb(0xFFFFFF).B;
            }
            MeCore.Config.Save(null);
            RenderColor();
        }
        private void RenderColor()
        {
            try
            {
                {
                    var da = new ColorAnimation(Color.FromArgb(127, MeCore.Config.color[0], MeCore.Config.color[1], MeCore.Config.color[2]), TimeSpan.FromSeconds(0.25));
                    MeCore.MainWindow.gridMenu.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
                    MeCore.MainWindow.gridAbout.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
                    MeCore.MainWindow.gridDL.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
                    MeCore.MainWindow.gridPlay.animateLblBG(da);
                    gridParent.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
                    MeCore.MainWindow.expanderTask.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
                    var col = System.Drawing.Color.FromArgb((int)(~ByteArrayToArgb()));
                    MeCore.MainWindow.gridAbout.setLblColor(Color.FromRgb(col.R, col.G, col.B));
                    MeCore.MainWindow.gridDL.setLblColor(Color.FromRgb(col.R, col.G, col.B));
                    MeCore.MainWindow.gridPlay.setLblColor(Color.FromRgb(col.R, col.G, col.B));
                    setLblColor(Color.FromRgb(col.R, col.G, col.B));
                }
            }
            catch (Exception ex)
            {
                new ErrorReport(ex).ShowDialog();
                MeCore.Config.color = new byte[] { 255, 255, 255 };
                MeCore.MainWindow.Close();
                System.Windows.Forms.Application.Restart();

            }
        }
        /*
        private void RenderTransparency() {
            try
            {
                {
                    var ani = new DoubleAnimation(MeCore.Config.WindowTransparency, TimeSpan.FromSeconds(0.5));
                    MeCore.MainWindow.BeginAnimation(OpacityProperty, ani);
                }
            }
            catch (Exception ex)
            {
                new ErrorReport(ex).ShowDialog();
                MeCore.MainWindow.Close();
                System.Windows.Forms.Application.Restart();

            }
        }*/
        public void setLblColor(Color color)
        {
            lblBG.Foreground = new SolidColorBrush(color);
            lblColor.Foreground = new SolidColorBrush(color);
            lblDLUrl.Foreground = new SolidColorBrush(color);
            lblTitle.Foreground = new SolidColorBrush(color);
            lblLang.Foreground = new SolidColorBrush(color);
        }

        private void butSave1_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void butExpandTask_Checked(object sender, RoutedEventArgs e)
        {
            MeCore.Config.ExpandTaskGui = true;
            MeCore.Config.Save(null);
        }

        private void butExpandTask_Unchecked(object sender, RoutedEventArgs e)
        {
            MeCore.Config.ExpandTaskGui = false;
            MeCore.Config.Save(null);
        }
    }
}
