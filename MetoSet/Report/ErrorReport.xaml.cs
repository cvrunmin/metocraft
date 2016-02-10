using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Windows;

namespace MTMCL
{
    /// <summary>
    /// ErrorReport.xaml 的互動邏輯
    /// </summary>
    public partial class ErrorReport : Window
    {
        public ErrorReport()
        {
            InitializeComponent();
        }
        public ErrorReport(Exception ex)
        {
            InitializeComponent();
            var message = new StringBuilder();
            message.AppendLine("MTMCL," + MeCore.version);
            message.AppendLine(ex.Source);
            message.AppendLine(ex.ToString());
            message.AppendLine(ex.Message);
            foreach (DictionaryEntry data in ex.Data)
                message.AppendLine(string.Format("Key:{0}\nValue:{1}", data.Key, data.Value));
            message.AppendLine(ex.StackTrace);
            var iex = ex;
            while (iex.InnerException != null)
            {
                message.AppendLine("------------------------");
                iex = iex.InnerException;
                message.AppendLine(iex.Source);
                message.AppendLine(iex.ToString());
                message.AppendLine(iex.Message);
                foreach (DictionaryEntry data in ex.Data)
                    message.AppendLine(string.Format("Key:{0}\nValue:{1}", data.Key, data.Value));
                message.AppendLine(iex.StackTrace);
            }
            message.AppendLine("\n\n-----------------MTMCL LOG----------------------\n");
            var sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "mtmcl.log");
            message.AppendLine(sr.ReadToEnd());
            sr.Close();
            txtMes.Text = message.ToString();
        }
        public ErrorReport(String s, Exception ex)
        {
            InitializeComponent();
            var message = new StringBuilder();
            message.AppendLine("MTMCL," + MeCore.version);
            message.AppendLine(s);
            message.AppendLine("\n\n-----------------ERROR REPORT----------------------\n");
            message.AppendLine(ex.Source);
            message.AppendLine(ex.ToString());
            message.AppendLine(ex.Message);
            foreach (DictionaryEntry data in ex.Data)
                message.AppendLine(string.Format("Key:{0}\nValue:{1}", data.Key, data.Value));
            message.AppendLine(ex.StackTrace);
            var iex = ex;
            while (iex.InnerException != null)
            {
                message.AppendLine("------------------------");
                iex = iex.InnerException;
                message.AppendLine(iex.Source);
                message.AppendLine(iex.ToString());
                message.AppendLine(iex.Message);
                foreach (DictionaryEntry data in ex.Data)
                    message.AppendLine(string.Format("Key:{0}\nValue:{1}", data.Key, data.Value));
                message.AppendLine(iex.StackTrace);
            }
            message.AppendLine("\n\n-----------------MTMCL LOG----------------------\n");
            var sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "mtmcl.log");
            message.AppendLine(sr.ReadToEnd());
            sr.Close();
            txtMes.Text = message.ToString();
        }
        public ErrorReport(String s, String ex)
        {
            InitializeComponent();
            var message = new StringBuilder();
            message.AppendLine("MTMCL," + MeCore.version);
            message.AppendLine(s);
            message.AppendLine("\n\n-----------------ERROR REPORT----------------------\n");
            message.AppendLine(ex);
            message.AppendLine("\n\n-----------------MTMCL LOG----------------------\n");
            var sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "mtmcl.log");
            message.AppendLine(sr.ReadToEnd());
            sr.Close();
            txtMes.Text = message.ToString();
        }

        private void butEmail_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("mailto:lung1a16@hotmail.com?subject=" + HttpUtility.UrlEncode("MTMCL_Crash_Report") + "&body=" + HttpUtility.UrlEncode(txtMes.Text, Encoding.Default));
        }

        private void butMCBBS_Click(object sender, RoutedEventArgs e)
        {
            //Process.Start("http://www.mcbbs.net/thread-??????-1-1.html");
            //Copy();
        }
        private void Copy()
        {
            try
            {
                Clipboard.SetText(txtMes.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn\'t copy into clipboard, please copy it manually: \n" + ex.Message);
            }
        }
    }
}
