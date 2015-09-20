using MetoCraft.Lang;
using System;
using System.Collections.Generic;
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
            this.Title = "Metocraft V1 Ver." + MeCore.version;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 800) { }
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
            this.gridPlay.gridEn.sliderRAM.Maximum = Config.GetMemory();
            FinishLoad = true;
        }

        private void butDL_Click(object sender, RoutedEventArgs e)
        {
            gridDL.Margin = new Thickness(0);
            butHP.IsEnabled = true;
        }
    }
}
