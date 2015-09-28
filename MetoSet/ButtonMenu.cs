using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MetoCraft
{
    public class ButtonMenu : Button
    {
        static ButtonMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonMenu), new FrameworkPropertyMetadata(typeof(ButtonMenu)));
        }
        public static readonly DependencyProperty MenuImageProperty = DependencyProperty.Register("MenuImage",typeof(ImageSource),typeof(ButtonMenu));
        public static readonly DependencyProperty ListTypeProperty = DependencyProperty.Register("ListType",typeof(bool),typeof(ButtonMenu));
        public ImageSource MenuImage {
            get {
                return (ImageSource)GetValue(MenuImageProperty);
            }
            set {
                SetValue(MenuImageProperty, value);
            }
        }
        public bool ListType {
            get {
                return (bool)GetValue(ListTypeProperty);
            }
            set {
                SetValue(ListTypeProperty,value);
            }
        }
    }
}
