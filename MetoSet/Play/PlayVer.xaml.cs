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
    /// PlayVer.xaml 的互動邏輯
    /// </summary>
    public partial class PlayVer : Grid
    {
        private KMCCC.Launcher.Version[] vers;
        public KMCCC.Launcher.Version[] versions {
            get {
                return vers;
            }
            private set {
                vers = value;
            }
        }
        public PlayVer()
        {
            InitializeComponent();
        }

        private void butLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadVersionList();
        }
        public void LoadVersionList() {
            listBoxVer.SelectedIndex = -1;
            MeCore.MainWindow.gridDL.gridLib.listVer.SelectedIndex = -1;
            MeCore.MainWindow.gridDL.gridAsset.listVer.SelectedIndex = -1;
            listBoxVer.Items.Clear();
            MeCore.MainWindow.gridDL.gridLib.listVer.Items.Clear();
            MeCore.MainWindow.gridDL.gridAsset.listVer.Items.Clear();
            try
            {
                PlayMain.launcher = KMCCC.Launcher.LauncherCore.Create(new KMCCC.Launcher.LauncherCoreCreationOption(MeCore.MainWindow.gridPlay.gridEn.txtBoxP.Text, MeCore.MainWindow.gridPlay.gridEn.comboJava.SelectedItem as string));
                versions = PlayMain.launcher.GetVersions().ToArray();
                for (int i = 0; i < versions.Length; i++)
                {
                    listBoxVer.Items.Add(versions[i].Id);
                    MeCore.MainWindow.gridDL.gridLib.listVer.Items.Add(versions[i].Id);
                    MeCore.MainWindow.gridDL.gridAsset.listVer.Items.Add(versions[i].Id);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    new ErrorReport(ex).Show();
                }));
            }
        }

        private void listBoxVer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(listBoxVer.SelectedIndex != -1)
            try
            {
                KMCCC.Modules.JVersion.JVersion jsonv = new KMCCC.Modules.JVersion.JVersionLocator().LoadVersion(KMCCC.Launcher.LauncherCoreItemResolverExtensions.GetVersionJsonPath(PlayMain.launcher, versions[listBoxVer.SelectedIndex]));
                string id = null, type = null;DateTime rtime;
                id = jsonv.Id;
                type = jsonv.Type;
                rtime = jsonv.ReleaseTime;
                if (type == "release" || type == "snapshot")
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(id, "^\\d{1}\\.\\d{1}$"))
                    {
                        if (!System.Text.RegularExpressions.Regex.IsMatch(id, "^\\d{1}\\.\\d{1}\\.\\d{1}$"))
                        {
                            if (!System.Text.RegularExpressions.Regex.IsMatch(id, "^\\d{1}\\.\\d{1}\\.\\d{2}$"))
                            {
                                if (!System.Text.RegularExpressions.Regex.IsMatch(id, "^\\d{2}w\\d{2}[a-h]$"))
                                {
                                    type = type + " (modified)";
                                }
                            }
                        }
                    }
                }
                lblVerId1.Content = id;
                MeCore.MainWindow.gridPlay.gridPlay.lblLVer1.Content = id;
                lblType1.Content = type;
                MeCore.MainWindow.gridPlay.gridPlay.lblType1.Content = type;
                lblRTime1.Content = rtime;
            }
            catch (Exception ex)
            {
                lblVerId1.Content = "Error";
                lblType1.Content = "Error";
                lblRTime1.Content = "Error";
                Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    new ErrorReport(ex).Show();
                }));
            }
        }
    }
}
