using MahApps.Metro;
using MTMCL.Lang;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MTMCL
{
    /// <summary>
    /// Settings.xaml 的互動邏輯
    /// </summary>
    public partial class Settings : Grid
    {
        public bool ValidMC = false;
        public bool CouldMC = false;
        public Settings()
        {
            InitializeComponent();
        }

        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            MeCore.MainWindow.gridMain.Visibility = Visibility.Visible;
            MeCore.MainWindow.gridOthers.Visibility = Visibility.Collapsed;
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridMain.BeginAnimation(OpacityProperty, ani);
        }

        private void butBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            txtboxMP.Text = dialog.SelectedPath;
        }

        private void txtboxMP_TextChanged(object sender, TextChangedEventArgs e)
        {
            ChangeMCPathState();
            MeCore.Config.MCPath = txtboxMP.Text;
            MeCore.Config.Save();
        }
        private void ChangeMCPathState()
        {
            try
            {
                if (!Directory.Exists(Directory.GetDirectoryRoot(txtboxMP.Text)))
                {
                    rectMPState.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                    rectMPState.OpacityMask = new VisualBrush() { Visual = (Visual)System.Windows.Application.Current.Resources["appbar_warning"] };
                    rectMPState.ToolTip = LangManager.GetLangFromResource("MCPath_NotExist");
                    ValidMC = false; CouldMC = true;
                    return;
                }
                if (!Directory.Exists(txtboxMP.Text))
                {
                    rectMPState.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                    rectMPState.OpacityMask = new VisualBrush() { Visual = (Visual)System.Windows.Application.Current.Resources["appbar_warning"] };
                    rectMPState.ToolTip = LangManager.GetLangFromResource("MCPath_NotExist");
                    ValidMC = false; CouldMC = true;
                    return;
                }
                if (!Directory.Exists(Path.Combine(txtboxMP.Text, "versions")))
                {
                    rectMPState.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                    rectMPState.OpacityMask = new VisualBrush() { Visual = (Visual)System.Windows.Application.Current.Resources["appbar_warning"] };
                    rectMPState.ToolTip = LangManager.GetLangFromResource("MCPath_NotValid");
                    ValidMC = false; CouldMC = true;
                    return;
                }
                rectMPState.Fill = new SolidColorBrush(Color.FromRgb(0, 0x99, 0));
                rectMPState.OpacityMask = new VisualBrush() { Visual = (Visual)System.Windows.Application.Current.Resources["appbar_check"] };
                rectMPState.ToolTip = null;
                ValidMC = true; CouldMC = true;
            }
            catch
            {
                rectMPState.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                rectMPState.OpacityMask = new VisualBrush() { Visual = (Visual)System.Windows.Application.Current.Resources["appbar_close"] };
                rectMPState.ToolTip = LangManager.GetLangFromResource("MCPath_Catched");
                ValidMC = false; CouldMC = false;
            }
        }
        private void ChangeJavaPathState()
        {
            try
            {
                if (!File.Exists(comboJava.Text))
                {
                    rectJPState.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    rectJPState.OpacityMask = new VisualBrush() { Visual = (Visual)System.Windows.Application.Current.Resources["appbar_close"] };
                    rectJPState.ToolTip = LangManager.GetLangFromResource("MCPath_NotExist");
                    return;
                }
                rectJPState.Fill = new SolidColorBrush(Color.FromRgb(0, 0x99, 0));
                rectJPState.OpacityMask = new VisualBrush() { Visual = (Visual)System.Windows.Application.Current.Resources["appbar_check"] };
                rectJPState.ToolTip = null;
            }
            catch
            {
                rectJPState.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                rectJPState.OpacityMask = new VisualBrush() { Visual = (Visual)System.Windows.Application.Current.Resources["appbar_close"] };
                rectJPState.ToolTip = LangManager.GetLangFromResource("MCPath_Catched");
            }
        }
        private void Grid_Initialized(object sender, EventArgs e)
        {
            PreInit();
            RefreshLangList();
            RefreshColorList();
            LoadConfig();
            if (MeCore.IsServerDedicated)
            {
                LoadServerDeDicatedVersion();
            }
        }
        private void PreInit()
        {
            sliderRAM.Maximum = Config.GetMemory();
            lblLauncherVersion.Content = MeCore.version;
        }
        private void LoadConfig()
        {
            comboDLSrc.SelectedIndex = MeCore.Config.DownloadSource;
            comboUdtSrc.SelectedIndex = MeCore.Config.UpdateSource;
            txtboxMP.Text = MeCore.Config.MCPath;
            txtboxArg.Text = MeCore.Config.ExtraJvmArg;
            comboJava.Text = MeCore.Config.Javaw;
            sliderRAM.Value = MeCore.Config.Javaxmx;
            comboLang.SelectedItem = LangManager.GetLangFromResource("DisplayName");
            txtboxBG.Text = MeCore.Config.Background;
            toggleReverse.IsChecked = MeCore.Config.reverseColor;
            toggleLatest.IsChecked = MeCore.Config.SearchLatest;
        }
        public void RefreshLangList()
        {
            comboLang.Items.Clear();
            var langs = LangManager.ListLanuage();
            foreach (var lang in langs)
            {
                comboLang.Items.Add(lang);
            }
            comboLang.SelectedItem = LangManager.GetLangFromResource("LangName");
        }
        public void RefreshColorList()
        {
            comboColor.Items.Clear();
            var colors = MeCore.Color;
            foreach (var color in colors)
            {
                comboColor.Items.Add(color.Key);
            }
            comboColor.Items.Add("Create New Color Scheme");
            comboColor.SelectedItem = MeCore.Config.ColorScheme;
        }
        private void comboColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboColor.SelectedItem as string != null)
            {
                if (comboColor.SelectedItem as string == "Create New Color Scheme")
                {
                    new Accents.AccentWindow().ShowDialog();
                    MeCore.Refresh();
                    RefreshLangList();
                    RefreshColorList();
                    LoadConfig();
                }
                if (MeCore.Color.ContainsKey(comboColor.SelectedItem as string))
                {
                    MeCore.Config.QuickChange("ColorScheme", comboColor.SelectedItem);
                    MeCore.MainWindow.RenderColor();
                }
            }
        }
        private void comboLang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboLang.SelectedItem as string != null)
                if (MeCore.Language.ContainsKey(comboLang.SelectedItem as string))
                {
                    LangManager.UseLanguage(MeCore.Language[comboLang.SelectedItem as string] as string);
                    MeCore.Config.Lang = LangManager.GetLangFromResource("LangName");
                    MeCore.Config.Save(null);
                }
        }

        private void comboDLSrc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MeCore.Config.DownloadSource = comboDLSrc.SelectedIndex;
            MeCore.Config.Save();
        }

        private void comboUdtSrc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MeCore.Config.UpdateSource = (byte)comboUdtSrc.SelectedIndex;
            MeCore.Config.Save();
        }

        private void comboJava_SelectionChanged(object sender, TextChangedEventArgs e)
        {
            MeCore.Config.QuickChange("Javaw", comboJava.Text);
            ChangeJavaPathState();
        }

        private void sliderRAM_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MeCore.Config.Javaxmx = sliderRAM.Value;
            MeCore.Config.Save();
        }

        private void txtboxArg_TextChanged(object sender, TextChangedEventArgs e)
        {
            MeCore.Config.ExtraJvmArg = txtboxArg.Text;
            MeCore.Config.Save();
            if (txtboxArg.Text.IndexOf("-Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true", StringComparison.Ordinal) != -1)
            {
                toggleModded.IsChecked = true;
            }
            else toggleModded.IsChecked = false;
            if (txtboxArg.Text.IndexOf("-XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy", StringComparison.Ordinal) != -1)
            {
                toggleLL.IsChecked = true;
            }
            else toggleLL.IsChecked = false;
        }

        private void txtboxMP_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (CouldMC)
            {
                //App.core = KMCCC.Launcher.LauncherCore.Create(new KMCCC.Launcher.LauncherCoreCreationOption(txtboxMP.Text, comboJava.SelectedItem as string, new KMCCC.Modules.JVersion.NewJVersionLocator()));
            }
        }

        private void butCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            MeCore.ReleaseCheck();
        }

        private void butBGBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG(*.png)|*.png|JPG(*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg; *.jpeg; *.jpe; *.jfif|TIFF(*.tif, *.tiff)|*.tif; *.tiff|Bitmap(*.bmp, *.dib)|*.bmp; *.dib|GIF(*.gif)|*.gif|All supported file|*.png; *.jpg; *.jpeg; *.jpe; *.jfif; *.tif; *.tiff; *.bmp; *.dib; *.gif";
            dialog.ShowDialog();
            if (string.IsNullOrWhiteSpace(dialog.FileName))
            {
                return;
            }
            txtboxBG.Text = dialog.FileName;
            //MeCore.Config.QuickChange("Background", dialog.FileName);
            MeCore.Config.Background = dialog.FileName; MeCore.Config.Save();
            if (MeCore.bghistory == null)
            {
                MeCore.bghistory = MTMCL.Resources.BGHistory.Load();
            }
            if (MeCore.bghistory == null)
            {
                MeCore.bghistory = new Resources.BGHistory();
            }
            MeCore.bghistory.AddAndSave(dialog.FileName);
            MeCore.MainWindow.Render();
            CheckDarkness(dialog.FileName);
        }
        private async void CheckDarkness(string path)
        {
            try
            {
                toggleReverse.IsChecked = await System.Threading.Tasks.TaskEx.Run(new Func<bool>(() =>
                {
                    System.Drawing.Bitmap map = new System.Drawing.Bitmap(path);
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, map.Width, map.Height);
                    System.Drawing.Imaging.BitmapData data = map.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, map.PixelFormat);
                    IntPtr ptr = data.Scan0;
                    int bytes = data.Stride * map.Height;
                    byte[] rgb = new byte[bytes];
                    System.Runtime.InteropServices.Marshal.Copy(ptr, rgb, 0, bytes);
                    float average = 0;
                    for (int i = 0; i < rgb.Length; i++)
                    {
                        average += rgb[i];
                    }
                    average /= rgb.Length;
                    return average <= 127;
                }));
            }
            catch (Exception)
            {
                toggleReverse.IsChecked = false;
            }
        }
        private async void CheckDarkness(Stream stream)
        {
            try
            {
                toggleReverse.IsChecked = await System.Threading.Tasks.TaskEx.Run(new Func<bool>(() =>
                {
                    System.Drawing.Bitmap map = new System.Drawing.Bitmap(stream);
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, map.Width, map.Height);
                    System.Drawing.Imaging.BitmapData data = map.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, map.PixelFormat);
                    IntPtr ptr = data.Scan0;
                    int bytes = data.Stride * map.Height;
                    byte[] rgb = new byte[bytes];
                    System.Runtime.InteropServices.Marshal.Copy(ptr, rgb, 0, bytes);
                    float average = 0;
                    for (int i = 0; i < rgb.Length; i++)
                    {
                        average += rgb[i];
                    }
                    average /= rgb.Length;
                    return average <= 127;
                }));
            }
            catch (Exception)
            {
                toggleReverse.IsChecked = false;
            }
        }
        private void butReset_Click(object sender, RoutedEventArgs e)
        {
            txtboxBG.Text = "default";
            //MeCore.Config.QuickChange("Background", "default");
            MeCore.Config.Background = "default";MeCore.Config.Save();
            MeCore.MainWindow.Render();
            toggleReverse.IsChecked = false;
        }
        private void LoadServerDeDicatedVersion()
        {
            if (MeCore.Config.Server.LockBackground)
            {
                txtboxBG.IsEnabled = butBGBrowse.IsEnabled = butReset.IsEnabled = false;
            }
            if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.BackgroundPath))
            {
                MeCore.DefaultBG = MeCore.Config.Server.BackgroundPath;
            }
            if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
            {
                txtboxMP.IsEnabled = butBrowse.IsEnabled = false;
            }
        }

        private void butAuth_Click(object sender, RoutedEventArgs e)
        {
            //new ACSelect().ShowDialog();
        }

        private void toggleReverse_IsCheckedChanged(object sender, EventArgs e)
        {
            ThemeManager.ChangeAppTheme(System.Windows.Application.Current, (bool)toggleReverse.IsChecked ? "BaseDark" : "BaseLight");
            MeCore.Config.QuickChange("reverseColor", toggleReverse.IsChecked);
        }

        private void butSelect_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Resources.BGWindow();
            dialog.ShowDialog();
            txtboxBG.Text = dialog.uri;
            //MeCore.Config.QuickChange("Background", dialog.uri);
            MeCore.Config.Background = dialog.uri; MeCore.Config.Save();
            MeCore.MainWindow.Render();
            if (dialog.steam != null)
            {
                CheckDarkness(dialog.steam);
            }
            else
                CheckDarkness(dialog.uri);
        }

        private void toggleLatest_IsCheckedChanged(object sender, EventArgs e)
        {
            MeCore.Config.QuickChange("SearchLatest", (bool)toggleLatest.IsChecked);
        }

        private void butJavawBrowse_Click(object sender, RoutedEventArgs e)
        {
            FileDialog dialog = new OpenFileDialog();
            dialog.Filter = "javaw.exe | javaw.exe";
            dialog.ShowDialog();
            comboJava.Text = dialog.FileName;
        }

        private void toggleModded_IsCheckedChanged(object sender, EventArgs e)
        {
            if ((bool)toggleModded.IsChecked)
            {
                if (txtboxArg.Text.IndexOf("-Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true", StringComparison.Ordinal) == -1)
                    txtboxArg.Text += " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true";
            }
            else if (!(bool)toggleModded.IsChecked)
            {
                if (txtboxArg.Text.IndexOf("-Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true", StringComparison.Ordinal) != -1)
                    txtboxArg.Text = txtboxArg.Text.Replace(" -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true", "");
            }
        }

        private void toggleLL_IsCheckedChanged(object sender, EventArgs e)
        {
            if ((bool)toggleLL.IsChecked)
            {
                if (txtboxArg.Text.IndexOf("-XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy", StringComparison.Ordinal) == -1)
                    txtboxArg.Text += " -XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy";
            }
            else if (!(bool)toggleLL.IsChecked)
            {
                if (txtboxArg.Text.IndexOf("-XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy", StringComparison.Ordinal) != -1)
                    txtboxArg.Text = txtboxArg.Text.Replace(" -XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy", "");
            }
        }
    }
}
