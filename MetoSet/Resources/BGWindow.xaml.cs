using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MTMCL.Resources
{
    /// <summary>
    /// BGWindow.xaml 的互動邏輯
    /// </summary>
    public partial class BGWindow
    {
        public string uri { get; private set; }
        public Stream steam { get; private set; }
        public BGWindow()
        {
            InitializeComponent();
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
                Close();
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
    }
    [DataContract]
    public class BGHistory {
        [DataMember]
        public List<string> uri;
        public BGHistory() {
            uri = new List<string>();
        }
        public static BGHistory Load() {
            try
            {
                return JsonConvert.DeserializeObject<BGHistory>(File.ReadAllText(Path.Combine(MeCore.DataDirectory, "bghistory.json")));
            }
            catch (Exception)
            {
                return new BGHistory();
            }
        }
        public void AddAndSave(string uri) {
            if (this.uri == null)
            {
                this.uri = new List<string>();
            }
            if (!this.uri.Contains(uri))
            {
                this.uri.Add(uri);
            }
            File.WriteAllText(Path.Combine(MeCore.DataDirectory, "bghistory.json"), JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), System.Text.Encoding.UTF8);
        }
    }
}
