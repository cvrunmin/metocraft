using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MetocraftWpf
{
    class ButtonMetroTab : Button
    {
        static ButtonMetroTab()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonMetroTab), new FrameworkPropertyMetadata(typeof(ButtonMetroTab)));
        }
        public static readonly DependencyProperty MenuImageProperty = DependencyProperty.Register("MenuImage", typeof(ImageSource), typeof(ButtonMetroTab));
        public static readonly DependencyProperty ListTypeProperty = DependencyProperty.Register("ListType", typeof(bool), typeof(ButtonMetroTab));
        public ImageSource MenuImage
        {
            get
            {
                return (ImageSource)GetValue(MenuImageProperty);
            }
            set
            {
                SetValue(MenuImageProperty, value);
            }
        }
        public bool ListType
        {
            get
            {
                return (bool)GetValue(ListTypeProperty);
            }
            set
            {
                SetValue(ListTypeProperty, value);
            }
        }
    }
}
