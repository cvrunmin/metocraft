using MTMCL.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Shapes;

namespace MTMCL.Notice
{
    /// <summary>
    /// NoticeDetail.xaml 的互動邏輯
    /// </summary>
    public partial class InheritMissingDetail
    {
        public readonly InheritMissingBar task;
        public readonly Grid parent;
        public ImageSource ImgSrc { get; set; }
        public InheritMissingDetail()
        {
            InitializeComponent();
        }
        public InheritMissingDetail(Grid parent, InheritMissingBar task) : this() {
            this.parent = parent;
            this.task = task;
            //string uri = ((BitmapImage)task.ImgSrc).UriSource.OriginalString;
            //ImgSrc = new BitmapImage(new Uri(uri.Replace("-banner", "")));
            lblName.Content = task.ErrorName;
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
        private void PostInit(Dictionary<string, List<string>> dict) {
            foreach(var items in dict){
                var grid = new Grid() { VerticalAlignment = VerticalAlignment.Top };
                var sb = new StringBuilder();
                foreach(var vals in items.Value) {
                    sb.AppendLine(vals);
                }
                grid.Children.Add(new Label() { Content =  string.Format(Lang.LangManager.GetLocalized("InheritVerMissFormat"), items.Key) + "\n" + sb.ToString()});
                gridDetaila.Children.Add(grid);
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            PostInit(task.FatherlessVersions);
        }
    }
}
