using MetoCraft.Lang;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetoCraft
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MeCore.NIcon.MainWindow = this;
            MeCore.MainWindow = this;
            InitializeComponent();
            this.Title = "MetoCraft V1 Ver." + MeCore.version;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 700) {
                butAbout.ListType = true;
                butPlay.ListType = true;
                butConfig.ListType = true;
                butDL.ListType = true;
                butAbout.Width = wrap.ActualWidth;
                butPlay.Width = wrap.ActualWidth;
                butConfig.Width = wrap.ActualWidth;
                butDL.Width = wrap.ActualWidth;
                butAbout.Height = 50;
                butPlay.Height = 50;
                butConfig.Height = 50;
                butDL.Height = 50;
            }
            if (e.NewSize.Width >= 700)
            {
                butAbout.ListType = false;
                butPlay.ListType = false;
                butConfig.ListType = false;
                butDL.ListType = false;
                butAbout.Width = 200;
                butPlay.Width = 200;
                butConfig.Width = 200;
                butDL.Width = 200;
                butAbout.Height = 200;
                butPlay.Height = 200;
                butConfig.Height = 200;
                butDL.Height = 200;
            }
            if (gridPlay.Margin != new Thickness(0))
            {
                gridPlay.Margin = new Thickness(0, 0, 0, gridMain.ActualHeight);
            }
            if (gridDL.Margin != new Thickness(0))
            {
                gridDL.Margin = new Thickness(0, 0, 0, gridMain.ActualHeight);
            }
            if (gridSettings.Margin != new Thickness(0))
            {
                gridSettings.Margin = new Thickness(0, 0, 0, gridMain.ActualHeight);
            }
            if (gridAbout.Margin != new Thickness(0))
            {
                gridAbout.Margin = new Thickness(0, 0, 0, gridMain.ActualHeight);
            }
        }

        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
            gridPlay.Margin = new Thickness(0);
            butHP.IsEnabled = true;
        }

        private void butAbout_Click(object sender, RoutedEventArgs e)
        {
            gridAbout.Margin = new Thickness(0);
            butHP.IsEnabled = true;
        }

        private void butHP_Click(object sender, RoutedEventArgs e)
        {
            gridPlay.Margin = new Thickness(0, 0, 0, gridMain.ActualHeight);
            gridDL.Margin = new Thickness(0, 0, 0, gridMain.ActualHeight);
            gridSettings.Margin = new Thickness(0, 0, 0, gridMain.ActualHeight);
            gridAbout.Margin = new Thickness(0, 0, 0, gridMain.ActualHeight);
            butHP.IsEnabled = false;
        }
        private void Window_Closed(object sender, EventArgs e)
        {

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MeCore.NIcon.Hide();
        }

        private void butConfig_Click(object sender, RoutedEventArgs e)
        {
            gridSettings.Margin = new Thickness(0);
            butHP.IsEnabled = true;
        }
        public void ChangeLanguage()
        {
//            GridConfig.listDownSource.Items[1] = LangManager.GetLangFromResource("listOfficalSource");
//            GridConfig.listDownSource.Items[0] = LangManager.GetLangFromResource("listAuthorSource");
//            BmclCore.LoadPlugin(LangManager.GetLangFromResource("LangName"));
            gridAbout.loadOSData();
        }
        public bool FinishLoad = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gridSettings.listLang.SelectedItem = LangManager.GetLangFromResource("DisplayName");
            gridPlay.gridEn.loadConfig();
            this.gridPlay.gridEn.sliderRAM.Maximum = Config.GetMemory();
            if (gridPlay.gridEn.txtBoxP.Text != "")
            {
                gridPlay.gridVer.LoadVersionList();
            }
            FinishLoad = true;
        }

        private void butDL_Click(object sender, RoutedEventArgs e)
        {
            gridDL.Margin = new Thickness(0);
            butHP.IsEnabled = true;
        }
    }
}
