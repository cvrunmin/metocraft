using MahApps.Metro.Controls.Dialogs;
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


namespace MTMCL.Setting
{
    /// <summary>
    /// GridColor.xaml 的互動邏輯
    /// </summary>
    public partial class GridColor : Grid
    {
        Grid parent;
        string color;
        MahApps.Metro.Accent accent;
        public GridColor ()
        {
            InitializeComponent();
        }

        public GridColor(Grid parent) : this() {
            this.parent = parent;
        }

        public GridColor(Grid parent, string color) : this(parent)
        {
            this.color = color;
        }
        public GridColor(Grid parent, string color, MahApps.Metro.Accent accent) : this(parent, color)
        {
            this.accent = accent;
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
            if (parent is Settings) ((Settings)parent).ChangeColor(color);
        }
        private void Grid_Initialized(object sender, EventArgs e)
        {

        }

        private void listColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            color = ((KeyValuePair<string,MahApps.Metro.Accent>)listColor.SelectedItem).Key;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var list = MeCore.Color;
            if (!MeCore.Color.ContainsKey(color)) list.Add(color, accent);
            listColor.ItemsSource = list;
            listColor.SelectedItem = list.Where(kvp => kvp.Key.Equals(color)).First();
        }

        private void butCreate_Click(object sender, RoutedEventArgs e)
        {
            new Accents.AccentWindow().ShowDialog();
            MeCore.Refresh();
            Grid_Loaded(sender, e);
        }
    }
}
