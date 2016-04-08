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
    }
}
