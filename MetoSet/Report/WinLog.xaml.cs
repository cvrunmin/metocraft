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
        object _obj = new object();
        public void WriteLine (string Log)
        {
            lock (_obj)
            {
                try
                {
                    sb.AppendLine(Log);
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate () { textBlock.Text = sb.ToString(); }));
                }
                catch (Exception)
                {
                    textBlock.Text = "Error occurred, ";
                    System.Threading.Tasks.TaskEx.Delay(5000);
                    Dispatcher.Invoke(new Action(() => textBlock.Text = sb.ToString()));
                }
            }
        }
    }
}
