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

namespace MTMCL.Notice
{
    /// <summary>
    /// CrashErrorBar.xaml 的互動邏輯
    /// </summary>
    public partial class CrashErrorBar : INotice
    {
        public string ErrorName { get; set; }
        public string ErrorContent { get; set; }
        public ImageSource ImgSrc { get; set; }
        public CrashErrorBar()
        {
            InitializeComponent();
        }
        public CrashErrorBar(string name) : this(name, "", "") { }
        public CrashErrorBar(string name, string message) : this(name, message, "") { }
        public CrashErrorBar(string name, string message, string desc) : this() {
            ErrorName = name;
            ErrorContent = message + "[/newline/]" + desc;
        }
    }
}
