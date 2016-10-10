using Microsoft.Win32;
using MTMCL.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MTMCL.Setting
{
    /// <summary>
    /// GridBG.xaml 的互動邏輯
    /// </summary>
    public partial class GridBG
    {
        public string uri { get; private set; }
        public Stream steam { get; private set; }

        private Grid parent;
        public GridBG(Grid parent)
        {
            InitializeComponent();
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
            if (parent is Settings) ((Settings)parent).ChangeBG(uri,steam);
        }
        private void BGButton_Click(object sender, RoutedEventArgs e) {
            if (sender is PreviewItem)
            {
                var a = ((BitmapImage)((PreviewItem)sender).ImgSrc);
                if (a.ToString().StartsWith("pack://application:,,,/"))
                {
                    steam = Application.GetResourceStream(a.UriSource).Stream;
                    uri = a.ToString();
                }
                else/* if (a.UriSource.IsWellFormedOriginalString())*/
                {
                    uri = a.UriSource.OriginalString;
                }
                /*else if (a.UriSource.IsAbsoluteUri)
                {
                    uri = a.UriSource.AbsolutePath;
                }
                else
                {
                    uri = a.UriSource.ToString();
                }*/
                Back();
            }
        }

        private void MetroWindow_Initialized(object sender, EventArgs e)
        {
            try
            {
                AddPreviewItem("pack://application:,,,/Resources/bg.png", "Default");
                if (MeCore.bghistory == null)
                {
                    MeCore.bghistory = BGHistory.Load();
                }
                foreach (var item in MeCore.bghistory.uri)
                {
                    AddPreviewItem(item);
                }
                AddPreviewItem("pack://application:,,,/Resources/easteregg-marble.jpg", "6ce2|5b50|73a9|624b|6a5f");
                AddPreviewItem("pack://application:,,,/Resources/easteregg-sudou-kayo.png", "9996|85e4|798d|4e16");
            }
            catch{

            }
        }
        private void AddPreviewItem(string uri) {
            AddPreviewItem(uri, uri.Substring(uri.LastIndexOf('\\') + 1));
        }
        private void AddPreviewItem(string uri, string desc)
        {
            var pi = new PreviewItem() { ImgSrc = new BitmapImage(new Uri(uri)), Description = desc, Stretch = System.Windows.Media.Stretch.UniformToFill };
            pi.Background = (System.Windows.Media.Brush)Application.Current.Resources["AccentColorBrush3"];
            pi.Click += BGButton_Click;
            pi.uri = new Uri(uri);
            var menu = new ContextMenu();
            var b1 = new MenuItem();
            b1.Header = Lang.LangManager.GetLangFromResource("Select");
            b1.Click += MenuItemSelect_Click;
            var b2 = new MenuItem();
            b2.Header = Lang.LangManager.GetLangFromResource("Preview");
            b2.Click += MenuItemPreview_Click;
            menu.Items.Add(b1);
            menu.Items.Add(b2);
            pi.ContextMenu = menu;
            panelBG.Children.Add(pi);
        }
        private void MenuItemSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                if (((MenuItem)sender).Parent is ContextMenu)
                {
                    if (((ContextMenu)((MenuItem)sender).Parent).PlacementTarget is PreviewItem)
                    {
                        var a = (PreviewItem)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
                            BGButton_Click(a, e);
                    }
                }
            }
        }
        private void MenuItemPreview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                if (((MenuItem)sender).Parent is ContextMenu)
                {
                    if (((ContextMenu)((MenuItem)sender).Parent).PlacementTarget is PreviewItem)
                    {
                        var a = (PreviewItem)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
                        var b = new BGPreviewWindow(a.uri);
                        b.ShowDialog();
                        if (b.isSelected)
                        {
                            BGButton_Click(a, e);
                        }
                    }
                }
            }
        }

        private void butBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG(*.png)|*.png|JPG(*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg; *.jpeg; *.jpe; *.jfif|TIFF(*.tif, *.tiff)|*.tif; *.tiff|Bitmap(*.bmp, *.dib)|*.bmp; *.dib|GIF(*.gif)|*.gif|All supported file|*.png; *.jpg; *.jpeg; *.jpe; *.jfif; *.tif; *.tiff; *.bmp; *.dib; *.gif";
            dialog.ShowDialog();
            if (string.IsNullOrWhiteSpace(dialog.FileName))
            {
                return;
            }
            //MeCore.Config.QuickChange("Background", dialog.FileName);
            if (MeCore.bghistory == null)
            {
                MeCore.bghistory = BGHistory.Load();
            }
            if (MeCore.bghistory == null)
            {
                MeCore.bghistory = new BGHistory();
            }
            MeCore.bghistory.AddAndSave(dialog.FileName);
            Grid_Loaded(sender, e);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                panelBG.Children.Clear();
                AddPreviewItem("pack://application:,,,/Resources/bg.png", "Default");
                if (MeCore.bghistory == null)
                {
                    MeCore.bghistory = BGHistory.Load();
                }
                foreach (var item in MeCore.bghistory.uri)
                {
                    AddPreviewItem(item);
                }
                AddPreviewItem("pack://application:,,,/Resources/easteregg-marble.jpg", "6ce2|5b50|73a9|624b|6a5f");
                AddPreviewItem("pack://application:,,,/Resources/easteregg-sudou-kayo.png", "9996|85e4|798d|4e16");
            }
            catch
            {

            }
        }
    }
}
