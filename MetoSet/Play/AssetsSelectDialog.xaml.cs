using MahApps.Metro.Controls.Dialogs;
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

namespace MTMCL
{
    /// <summary>
    /// AssetsSelectDialog.xaml 的互動邏輯
    /// </summary>
    public partial class AssetsSelectDialog : CustomDialog
    {
        public delegate void Closing(object sender, ClosingEventArgs e);
        public event Closing OnClosing;
        public Dictionary<string, Assets.AssetsEntity> AssetsObjects { get; set; }
        public Dictionary<string, Assets.PackedAssetsEntity> PackedAssetsObjects => PackAssetsEntity(AssetsObjects);

        public AssetsSelectDialog()
        {
            InitializeComponent();
        }

        private void butOk_Click(object sender, RoutedEventArgs e)
        {
            var a = listAsset.SelectedItems;
            var b = (IList<KeyValuePair<string, Assets.PackedAssetsEntity>>)a;
            OnClosing?.Invoke(this, new ClosingEventArgs(true, b.ToDictionary(pair=>pair.Key, pair=>pair.Value)));
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            OnClosing?.Invoke(this, new ClosingEventArgs(false, null));
        }

        public class ClosingEventArgs : EventArgs {
            public bool IsOk { get; private set; }
            public Dictionary<string, Assets.PackedAssetsEntity> SelectedEntities { get; private set; }
            public ClosingEventArgs(bool isok, Dictionary<string, Assets.PackedAssetsEntity> dict)
            {
                IsOk = isok;
                SelectedEntities = dict;
            }
        }

        private void listAsset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 1 && listAsset.SelectedItems.Count == 1)
            {
                if(e.AddedItems.Count > 0)
                    foreach (var item in e.AddedItems)
                        listAsset.SelectedItems.Remove(item);
                foreach (var item in e.RemovedItems)
                    listAsset.SelectedItems.Add(item);
                e.Handled = true;
            }
        }

        private Dictionary<string, Assets.PackedAssetsEntity> PackAssetsEntity(Dictionary<string, Assets.AssetsEntity> raw) {
            var pack = new Dictionary<string, Assets.PackedAssetsEntity>();
            if(raw != null)
            foreach (var item in raw)
            {
                pack.Add(item.Key, new Assets.PackedAssetsEntity() { Hash = item.Value.Hash, Size = item.Value.Size, Assets = item.Key });
            }
            return pack;
        }
    }
}
