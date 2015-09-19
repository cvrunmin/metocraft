using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace MetoSet.Play
{
    /// <summary>
    /// PlayEnvir.xaml 的互動邏輯
    /// </summary>
    public partial class PlayEnvir : Grid
    {
        public PlayEnvir()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBoxP.Text = MeCore.Config.MCPath;
            comboJava.SelectedItem = MeCore.Config.Javaw;
            txtBoxArg.Text = MeCore.Config.ExtraJvmArg;
        }

        private void butBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            txtBoxP.Text = dialog.SelectedPath;
        }

        private void butSave_Click(object sender, RoutedEventArgs e)
        {
            MeCore.Config.MCPath = this.txtBoxP.Text;
            MeCore.Config.Javaw = this.comboJava.SelectedItem as string;
            MeCore.Config.ExtraJvmArg = this.txtBoxArg.Text;
            MeCore.Config.Save(null);
        }
    }
}
