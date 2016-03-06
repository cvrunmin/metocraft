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

namespace MTMCL.Task
{
    /// <summary>
    /// TaskDetail.xaml 的互動邏輯
    /// </summary>
    public partial class TaskDetail : Grid
    {
        public readonly TaskListBar task;
        public readonly Grid parent;
        public ImageSource ImgSrc { get; set; }
        public TaskDetail()
        {
            InitializeComponent();
        }
        public TaskDetail(Grid parent, TaskListBar task) : this() {
            this.parent = parent;
            this.task = task;
            string uri = ((BitmapImage)task.ImgSrc).UriSource.OriginalString;
            ImgSrc = new BitmapImage(new Uri(uri.Replace("-banner", "")));
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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            lblTitle.Content = task.TaskName;

        }
    }
}
