using MTMCL.Forge;
using MTMCL.Lang;
using MTMCL.Task;
using MTMCL.util;
using MTMCL.Versions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace MTMCL
{
    /// <summary>
    /// GridMCDL.xaml 的互動邏輯
    /// </summary>
    public partial class GridForgeDLMain : Grid
    {
        Grid parent;
        public GridForgeDLMain ()
        {
            InitializeComponent();
        }

        public GridForgeDLMain(Grid parent) : this() {
            this.parent = parent;
        }

        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }

        private void Back()
        {
            MeCore.MainWindow.gridOthers.Children.Clear();
            MeCore.MainWindow.gridOthers.Children.Add(parent);
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridOthers.BeginAnimation(OpacityProperty, ani);
        }

        internal readonly ForgeVersionList _forgeVer = new ForgeVersionList();
        private void Grid_Initialized(object sender, EventArgs e)
        {
            RefreshForgeVersionList();
        }

        private void butReloadMC_Click(object sender, EventArgs e) {
            RefreshForgeVersionList();
        }
        private void RefreshForgeVersionList ()
        {
            butReloadForge.SetLocalizedContent("RemoteVerGetting");
            butReloadForge.IsEnabled = false;
            gridMFRFail.Visibility = Visibility.Collapsed;
            gridMFRing.Visibility = Visibility.Visible;
            _forgeVer.ForgePageReadyEvent += ForgeVer_ForgePageReadyEvent;
            _forgeVer.GetVersion();
        }
        async void ForgeVer_ForgePageReadyEvent ()
        {
            var fl = _forgeVer.GetMCVersionsAvailable_();
            if (fl == null)
            {
                Dispatcher.Invoke(new Action(() => {
                    gridMFRing.Visibility = Visibility.Collapsed;
                    gridMFRFail.Visibility = Visibility.Visible;
                    butReloadForge.SetLocalizedContent("Reload");
                    butReloadForge.IsEnabled = true;
                }));
                return;
            }
            if (fl.Length == 0)
            {
                Dispatcher.Invoke(new Action(() => {
                    gridMFRing.Visibility = Visibility.Collapsed;
                    gridMFRFail.Visibility = Visibility.Visible;
                    butReloadForge.SetLocalizedContent("Reload");
                    butReloadForge.IsEnabled = true;
                }));
                return;
            }
            Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(() =>
            {
                listForge.ItemsSource = fl;
                gridMFRing.Visibility = Visibility.Collapsed;
                listForge.Visibility = Visibility.Visible;
                butReloadForge.SetLocalizedContent("Reload");
                butReloadForge.IsEnabled = true;
                ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(listForge.ItemsSource);
                view.CustomSort = new VersionComparer() { Descending = true } ;
            }));
            await TaskEx.Delay(3000);
            var verlist = _forgeVer.GetVersionBranch();
        }
        
        private void butForges_Click (object sender, RoutedEventArgs e)
        {
            if (sender is Button)
                if (((Button) sender).DataContext is ForgeMajorVersions) {
                    MeCore.MainWindow.gridOthers.Children.Clear();
                    MeCore.MainWindow.gridOthers.Children.Add(new GridForgeDLMinor(this, (((Button) sender).DataContext as ForgeMajorVersions).Version));
                    var ani = new DoubleAnimationUsingKeyFrames();
                    ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
                    ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
                    MeCore.MainWindow.gridOthers.BeginAnimation(OpacityProperty, ani);
                }
        }
    }
}
