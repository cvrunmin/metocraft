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

namespace MTMCL.Resources
{
    /// <summary>
    /// PreviewItem.xaml 的互動邏輯
    /// </summary>
    public partial class PreviewItem
    {
        public PreviewItem()
        {
            InitializeComponent();
        }

        public static DependencyProperty ImgSrcProperty = DependencyProperty.Register("ImgSrc", typeof(ImageSource), typeof(PreviewItem));

        [System.ComponentModel.Bindable(true)]
        [System.ComponentModel.Description("Image Source")]
        [System.ComponentModel.Category("Content")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public ImageSource ImgSrc
        {
            get
            {
                return ((ImageSource)(GetValue(ImgSrcProperty)));
            }
            set
            {
                SetValue(ImgSrcProperty, value);
            }
        }

        public static DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(PreviewItem));

        [System.ComponentModel.Bindable(true)]
        [System.ComponentModel.Description("Color")]
        [System.ComponentModel.Category("Content")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public Color Color
        {
            get
            {
                return ((Color) (GetValue(ColorProperty)));
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        public static DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(PreviewItem));

        [System.ComponentModel.Bindable(true)]
        [System.ComponentModel.Description("Description")]
        [System.ComponentModel.Category("Content")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public string Description
        {
            get
            {
                return ((string)(GetValue(DescriptionProperty)));
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        public Uri uri { get; set; }

        private void button_Loaded(object sender, RoutedEventArgs e)
        {
            int i = System.Windows.Forms.TextRenderer.MeasureText(txtPreview.Text, new System.Drawing.Font(txtPreview.FontFamily.Source, (float)txtPreview.FontSize)).Width;
            if (i > lblPreview.ActualWidth)
            {
                var a = new Storyboard() { RepeatBehavior = RepeatBehavior.Forever};
                var b = new ThicknessAnimationUsingKeyFrames();
                b.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, 0, -(i - lblPreview.ActualWidth), 0), TimeSpan.FromSeconds(0)));
                b.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(-(i - lblPreview.ActualWidth), 0, 0, 0), TimeSpan.FromSeconds(5/2)));
                b.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(-(i - lblPreview.ActualWidth), 0, 0, 0), TimeSpan.FromSeconds(10/2)));
                b.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, 0, -(i - lblPreview.ActualWidth), 0), TimeSpan.FromSeconds(15/2)));
                b.KeyFrames.Add(new LinearThicknessKeyFrame(new Thickness(0, 0, -(i - lblPreview.ActualWidth), 0), TimeSpan.FromSeconds(20/2)));
                a.Children.Add(b);
                Storyboard.SetTarget(b, txtPreview);
                Storyboard.SetTargetProperty(b, new PropertyPath("Margin"));
                //txtPreview.BeginStoryboard(a, HandoffBehavior.Compose);
            }
        }
    }
}
