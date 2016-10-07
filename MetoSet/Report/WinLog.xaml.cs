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
using System.Windows.Shapes;

namespace MTMCL.Report
{
    /// <summary>
    /// WinLog.xaml 的互動邏輯
    /// </summary>
    public partial class WinLog : Window
    {
        StringBuilder sb = new StringBuilder();
        public WinLog ()
        {
            InitializeComponent();
        }
        public void WriteLine (string Log)
        {
            sb.AppendLine(Log);
            textBlock.Text = sb.ToString();
            //Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker( delegate() { textBlock.Text = sb; }));
        }
    }
}
