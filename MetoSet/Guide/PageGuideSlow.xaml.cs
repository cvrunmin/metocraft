using MTMCL.Lang;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace MTMCL.Guide
{
    /// <summary>
    /// PageGuideSlow.xaml 的互動邏輯
    /// </summary>
    public partial class PageGuideSlow : Page
    {
        public bool ValidMC = false;
        public bool CouldMC = false;
        public PageGuideSlow()
        {
            InitializeComponent();
        }
        private void butBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            txtboxMP.Text = dialog.SelectedPath;
        }
        private void butBack_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void butNext_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Guide\\PageGuideAuth.xaml", UriKind.Relative));
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

        private void Grid_Initialized(object sender, EventArgs e)
        {
            PreInit();
            LoadConfig();
        }
        private void PreInit()
        {
            sliderRAM.Maximum = Config.GetMemory();
        }
        private void LoadConfig()
        {
            comboDLSrc.SelectedIndex = MeCore.Config.DownloadSource;
            comboUdtSrc.SelectedIndex = MeCore.Config.UpdateSource;
            txtboxMP.Text = MeCore.Config.MCPath;
            txtboxArg.Text = MeCore.Config.ExtraJvmArg;
            comboJava.Text = MeCore.Config.Javaw;
            sliderRAM.Value = MeCore.Config.Javaxmx;
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
