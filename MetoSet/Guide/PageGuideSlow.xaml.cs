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
            comboJava.Items.Clear();
            try
            {
                var javas = KMCCC.Tools.SystemTools.FindValidJava().ToList();
                foreach (var java in javas)
                {
                    comboJava.Items.Add(java);
                }
            }
            catch { }
            sliderRAM.Maximum = KMCCC.Tools.SystemTools.GetTotalMemory() / 1024 / 1024;
        }
        private void LoadConfig()
        {
            comboDLSrc.SelectedIndex = MeCore.Config.DownloadSource;
            comboUdtSrc.SelectedIndex = MeCore.Config.UpdateSource;
            txtboxMP.Text = MeCore.Config.MCPath;
            txtboxArg.Text = MeCore.Config.ExtraJvmArg;
            comboJava.SelectedItem = MeCore.Config.Javaw;
            sliderRAM.Value = MeCore.Config.Javaxmx;
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

        private void comboJava_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MeCore.Config.Javaw = (string)comboJava.SelectedItem;
            MeCore.Config.Save();
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
    }
}
