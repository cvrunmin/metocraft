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
    /// InheritMissingBar.xaml 的互動邏輯
    /// </summary>
    public partial class InheritMissingBar : INotice
    {
        public string ErrorName { get; set; }
        public Dictionary<string,List<string>> FatherlessVersions { get; set; }
        public ImageSource ImgSrc { get; set; }
        public InheritMissingBar()
        {
            InitializeComponent();
        }
        public InheritMissingBar(string name) : this() {
            ErrorName = name;
        }
        public InheritMissingBar(string name, Dictionary<string,List<string>> errdict) : this(name) {
            FatherlessVersions = errdict;
        }
    }
}
