using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using MTMCL.Lang;
using System;
using System.Collections.Generic;
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
                    rectJPState.ToolTip = LangManager.GetLangFromResource("JavaPath_NotExist");
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
                rectJPState.ToolTip = LangManager.GetLangFromResource("JavaPath_Catched");
            }
        }
        private void Grid_Initialized(object sender, EventArgs e)
        {
            PreInit();
            RefreshLangList();
            LoadConfig();
            List<Themes.Theme> list = MeCore.themes;
            if(!(list[0] is Themes.DefaultTheme)) list.Insert(0, new Themes.DefaultTheme());
            var sth = MeCore.Config.Theme;
            if (string.IsNullOrWhiteSpace(sth))
            {
                if (MeCore.Config.Background.Equals("default"))
                    if (MeCore.Config.ColorScheme.Equals("Green"))
                        sth = "Default";
                sth = "Custom";
            }
            var a = list.Where(t => t.Name.Equals(MeCore.Config.Theme));
            var b = a.First();
            panelTheme.ItemsSource = list;
            if (b != null) panelTheme.SelectedItem = b;
            if (MeCore.IsServerDedicated)
            {
                LoadServerDeDicatedVersion();
            }
#if DEBUG
#else
            button.Visibility = Visibility.Collapsed;
#endif
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
                    byte[] bgr = new byte[bytes];
                    System.Runtime.InteropServices.Marshal.Copy(ptr, bgr, 0, bytes);
                    float averageB = 0, averageG = 0, averageR = 0;
                    for (int i = 0; i < bgr.Length; i += 3)
                    {
                        averageB += bgr[i];
                        averageG += bgr[i + 1];
                        averageR += bgr[i + 2];
                    }
                    averageR /= (bgr.Length / 3);
                    averageG /= (bgr.Length / 3);
                    averageB /= (bgr.Length / 3);
                    const int nThreshold = 105;
                    var bgDelta = Convert.ToInt32((averageR * 0.299) + (averageG * 0.587) + (averageB * 0.114));
                    return 255 - bgDelta < nThreshold;
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
                    System.Drawing.Imaging.BitmapData data = map.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    IntPtr ptr = data.Scan0;
                    int bytes = data.Stride * map.Height;
                    byte[] bgr = new byte[bytes];
                    System.Runtime.InteropServices.Marshal.Copy(ptr, bgr, 0, bytes);
                    float averageB = 0, averageG = 0, averageR = 0;
                    for (int i = 0; i < bgr.Length; i += 3)
                    {
                        averageB += bgr[i];
                        averageG += bgr[i + 1];
                        averageR += bgr[i + 2];
                    }
                    averageR /= (bgr.Length / 3);
                    averageG /= (bgr.Length / 3);
                    averageB /= (bgr.Length / 3);
                    const int nThreshold = 105;
                    var bgDelta = Convert.ToInt32((averageR * 0.299) + (averageG * 0.587) + (averageB * 0.114));
                    map.UnlockBits(data);
                    return 255 - bgDelta < nThreshold;
                }));
            }
            catch (Exception)
            {
                toggleReverse.IsChecked = false;
            }
        }

        private void LoadServerDeDicatedVersion()
        {
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
            new ACSelect().ShowDialog();
        }

        private void toggleReverse_IsCheckedChanged(object sender, EventArgs e)
        {
            ThemeManager.ChangeAppTheme(System.Windows.Application.Current, (bool)toggleReverse.IsChecked ? "BaseDark" : "BaseLight");
            MeCore.Config.QuickChange("reverseColor", toggleReverse.IsChecked);
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

        private void toggleTryOnce_IsCheckedChanged(object sender, EventArgs e)
        {
            MeCore.Config.QuickChange("download-once", toggleTryOnce.IsChecked);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            new ErrorReport(new AggregateException()).Show();
        }

        private void panelTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (panelTheme.SelectedIndex != -1)
            {
                if (panelTheme.SelectedItem is Themes.DefaultTheme)
                {
                    bgItem.ImgSrc = ((Themes.DefaultTheme)panelTheme.SelectedItem).Image;
                    var s = ((Themes.DefaultTheme)panelTheme.SelectedItem).ImageSource;
                    var a = s.LastIndexOf('\\');
                    if (a == -1) a = s.LastIndexOf('/');
                    if (a == -1)
                        bgItem.Description = s;
                    else bgItem.Description = s.Substring(a + 1);
                    colorItem.Color = (Color)((Themes.DefaultTheme)panelTheme.SelectedItem).Accent.Resources["AccentColor"];
                    colorItem.Description = ((Themes.DefaultTheme)panelTheme.SelectedItem).AccentName;
                    MeCore.MainWindow.RenderTheme(((Themes.DefaultTheme)panelTheme.SelectedItem));
                    MeCore.Config.QuickChange("Theme", ((Themes.DefaultTheme)panelTheme.SelectedItem).Name);
                }
                else
                {
                    bgItem.ImgSrc = ((Themes.Theme)panelTheme.SelectedItem).Image;
                    var s = ((Themes.Theme)panelTheme.SelectedItem).ImageSource;
                    var a = s.LastIndexOf('\\');
                    if (a == -1) a = s.LastIndexOf('/');
                    if (a == -1)
                        bgItem.Description = s;
                    else bgItem.Description = s.Substring(a + 1);
                    colorItem.Color = (Color)((Themes.Theme)panelTheme.SelectedItem).Accent.Resources["AccentColor"];
                    colorItem.Description = ((Themes.Theme)panelTheme.SelectedItem).AccentName;
                    MeCore.MainWindow.RenderTheme(((Themes.Theme)panelTheme.SelectedItem));
                    MeCore.Config.QuickChange("Theme", ((Themes.Theme)panelTheme.SelectedItem).Name);
                }
            }
        }
        private void createTmpBySelectedTheme()
        {
            var a = panelTheme.ItemsSource.OfType<Themes.Theme>();
            var b = a.ToList();
            var c = ((Themes.Theme)panelTheme.SelectedItem);
            var d = b.Where(t => t.isTmp);
            if (d.Count() > 0)
            {
                c = d.First();
                b.Remove(c);
            }
            if (c is Themes.DefaultTheme) c = Themes.ThemeHelper.NormalizeTheme((Themes.DefaultTheme)c);
            b.Add(c.MakeChanges("isTmp", true).MakeChanges("Name", "Custom"));
            panelTheme.ItemsSource = b;
            panelTheme.SelectedItem = c;
            
        }
        private void updateTheme(int index, Themes.Theme theme)
        {
            var a = panelTheme.ItemsSource.OfType<Themes.Theme>();
            var b = a.ToList();
            b.RemoveAt(index);
            b.Insert(index, theme);
            panelTheme.ItemsSource = b;
            panelTheme.SelectedIndex = index;
        }
        private void bgItem_Click(object sender, RoutedEventArgs e)
        {
            if (bgItem.ImgSrc != null)
            {
                var a = new Setting.GridBG(this);
                MeCore.MainWindow.gridOthers.Children.Clear();
                MeCore.MainWindow.gridOthers.Children.Add(a);
                var ani = new DoubleAnimationUsingKeyFrames();
                ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
                ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
                MeCore.MainWindow.gridOthers.BeginAnimation(OpacityProperty, ani);
            }
        }
        internal void ChangeBG(string uri, Stream steam = null) {
            if (string.IsNullOrWhiteSpace(uri) & steam == null) return;
            Themes.Theme x;
            if (panelTheme.SelectedItem != null)
            {
                if (!((Themes.Theme)panelTheme.SelectedItem).isTmp)
                {
                    createTmpBySelectedTheme();
                }
                x = ((Themes.Theme)panelTheme.SelectedItem).MakeChanges("ImageSource", uri);
                updateTheme(panelTheme.SelectedIndex, x);
                //MeCore.Config.QuickChange("Background", dialog.uri);
                //MeCore.Config.Background = dialog.uri; MeCore.Config.Save();
                if (steam != null)
                {
                    CheckDarkness(steam);
                }
                else
                {
                    CheckDarkness(uri);
                }
                    x = ((Themes.Theme)panelTheme.SelectedItem).MakeChanges("Image", new BitmapImage(new Uri(uri)));
                    updateTheme(panelTheme.SelectedIndex, x);
                
                MeCore.MainWindow.RenderTheme(x);
                panelTheme_SelectionChanged(this, null);
            }
        }

        private void colorItem_Click(object sender, RoutedEventArgs e)
        {
            var b = ((Themes.Theme)panelTheme.SelectedItem).AccentName;
            var c = ((Themes.Theme)panelTheme.SelectedItem).Accent;
            if (panelTheme.SelectedItem is Themes.DefaultTheme) {
                b = ((Themes.DefaultTheme)panelTheme.SelectedItem).AccentName;
                c = ((Themes.DefaultTheme)panelTheme.SelectedItem).Accent;
            }
            var a = new Setting.GridColor(this, b, c);
            MeCore.MainWindow.gridOthers.Children.Clear();
            MeCore.MainWindow.gridOthers.Children.Add(a);
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridOthers.BeginAnimation(OpacityProperty, ani);
        }
        internal void ChangeColor(string color)
        {
            if(((Themes.Theme)panelTheme.SelectedItem).AccentName.Equals(color))return;
            Themes.Theme x;
            if (panelTheme.SelectedItem != null)
            {
                if (!((Themes.Theme)panelTheme.SelectedItem).isTmp)
                {
                    createTmpBySelectedTheme();
                }
                x = ((Themes.Theme)panelTheme.SelectedItem).MakeChanges("AccentName", color).MakeChanges("Accent", MeCore.Color[color]);
                updateTheme(panelTheme.SelectedIndex, x);

                MeCore.MainWindow.RenderTheme(x);
                panelTheme_SelectionChanged(this, null);
            }
        }

        private void saveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.MenuItem)
            {
                if (((System.Windows.Controls.MenuItem)sender).Parent is System.Windows.Controls.ContextMenu)
                {
                    if (((System.Windows.Controls.ContextMenu)((System.Windows.Controls.MenuItem)sender).Parent).PlacementTarget is ListBoxItem)
                    {
                        if (((ListBoxItem)(((System.Windows.Controls.ContextMenu)((System.Windows.Controls.MenuItem)sender).Parent).PlacementTarget)).DataContext is Themes.Theme) {
                            var a = (Themes.Theme)((ListBoxItem)(((System.Windows.Controls.ContextMenu)((System.Windows.Controls.MenuItem)sender).Parent).PlacementTarget)).DataContext;
                            a=a.MakeChanges("isTmp", false);
                            a.SaveMTMCLTheme();
                        }
                    }
                }
            }
        }

        private void packItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.MenuItem)
            {
                if (((System.Windows.Controls.MenuItem)sender).Parent is System.Windows.Controls.ContextMenu)
                {
                    if (((System.Windows.Controls.ContextMenu)((System.Windows.Controls.MenuItem)sender).Parent).PlacementTarget is ListBoxItem)
                    {
                        if (((ListBoxItem)(((System.Windows.Controls.ContextMenu)((System.Windows.Controls.MenuItem)sender).Parent).PlacementTarget)).DataContext is Themes.Theme)
                        {
                            var a = (Themes.Theme)((ListBoxItem)(((System.Windows.Controls.ContextMenu)((System.Windows.Controls.MenuItem)sender).Parent).PlacementTarget)).DataContext;
                            a=a.MakeChanges("isTmp", false);
                            a.PackMTMCLTheme();
                        }
                    }
                }
            }
        }

        private async void renameItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.MenuItem)
            {
                if (((System.Windows.Controls.MenuItem)sender).Parent is System.Windows.Controls.ContextMenu)
                {
                    if (((System.Windows.Controls.ContextMenu)((System.Windows.Controls.MenuItem)sender).Parent).PlacementTarget is ListBoxItem)
                    {
                        if (((ListBoxItem)(((System.Windows.Controls.ContextMenu)((System.Windows.Controls.MenuItem)sender).Parent).PlacementTarget)).DataContext is Themes.Theme)
                        {
                            var a = (Themes.Theme)((ListBoxItem)(((System.Windows.Controls.ContextMenu)((System.Windows.Controls.MenuItem)sender).Parent).PlacementTarget)).DataContext;
                            string name = await MeCore.MainWindow.ShowInputAsync("Rename", "Type the new name");
                            a.EraseMTMCLTheme();
                            a.EraseMTMCLThemePack();
                            a = a.MakeChanges("Name",name).MakeChanges("isTmp",false);
                            updateTheme(panelTheme.SelectedIndex, a);
                            panelTheme_SelectionChanged(sender, null);
                            a.SaveMTMCLTheme();
                        }
                    }
                }
            }


        }

        private void panelTheme_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var p = e.GetType().GetProperty("TargetElement", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ListBoxItem v = (ListBoxItem)p.GetValue(e, null);
            if (v.DataContext is Themes.DefaultTheme)
            {
                e.Handled = true;
                FrameworkElement fe = e.Source as FrameworkElement;
                fe.ContextMenu = new System.Windows.Controls.ContextMenu();
                fe.ContextMenu.IsOpen = true;
            }
        }
    }
}
