using KMCCC.Launcher;
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

namespace MetoCraft.Play
{
    /// <summary>
    /// PlayMain.xaml 的互動邏輯
    /// </summary>
    public partial class PlayMain : Grid
    {
        public static LauncherCore launcher;
        public PlayMain()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var javas = KMCCC.Tools.SystemTools.FindJava().ToList();
            foreach (var java in javas)
            {
                gridEn.comboJava.Items.Add(java);
            }
        }
    }
}
