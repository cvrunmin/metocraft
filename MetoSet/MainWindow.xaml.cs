using MetoSet.Lang;
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

namespace MetoSet
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
            this.Title = "Metocraft Va1 Ver." + MeCore.version;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 800) { }
        }

        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
            gridPlay.Height = gridMain.ActualHeight;
            butHP.IsEnabled = true;
        }

        private void butAbout_Click(object sender, RoutedEventArgs e)
        {
            gridAbout.Height = gridMain.ActualHeight;
            butHP.IsEnabled = true;
        }

        private void butHP_Click(object sender, RoutedEventArgs e)
        {
            gridPlay.Height = 0;
            gridDL.Height = 0;
            gridSettings.Height = 0;
            gridAbout.Height = 0;
            butHP.IsEnabled = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MeCore.NIcon.Hide();
        }

        private void butConfig_Click(object sender, RoutedEventArgs e)
        {
            gridSettings.Height = gridMain.ActualHeight;
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
            gridDL.Height = gridMain.ActualHeight;
            butHP.IsEnabled = true;
        }
    }
}
