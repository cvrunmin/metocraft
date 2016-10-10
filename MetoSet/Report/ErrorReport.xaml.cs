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
    public partial class ErrorReport
    {
        public ErrorReport ()
        {
            InitializeComponent();
        }
        public ErrorReport (Exception ex)
        {
            InitializeComponent();
            var message = new StringBuilder();
            message.AppendLine("MTMCL, version " + MeCore.version);
            message.AppendFormat("Target Site: {0}", ex.TargetSite).AppendLine();
            message.AppendFormat("Error Type: {0}", ex.GetType()).AppendLine();
            message.AppendFormat("Messge: {0}", ex.Message).AppendLine();
            foreach (DictionaryEntry data in ex.Data)
                message.AppendLine(string.Format("Key:{0}\nValue:{1}", data.Key, data.Value));
            message.AppendLine("StackTrace");
            message.AppendLine(ex.StackTrace);
            var iex = ex;
            while (iex.InnerException != null)
            {
                message.AppendLine("------------Inner Exception------------");
                iex = iex.InnerException;
                message.AppendFormat("Target Site: {0}", ex.TargetSite).AppendLine();
                message.AppendFormat("Error Type: {0}", ex.GetType()).AppendLine();
                message.AppendFormat("Messge: {0}", ex.Message).AppendLine();
                foreach (DictionaryEntry data in ex.Data)
                    message.AppendLine(string.Format("Key:{0}\nValue:{1}", data.Key, data.Value));
                message.AppendLine("StackTrace");
                message.AppendLine(iex.StackTrace);
            }
            CreateLogInfoPart(message);
            txtMes.Text = message.ToString();
        }
        public ErrorReport (string s, Exception ex)
        {
            InitializeComponent();
            var message = new StringBuilder();
            message.AppendLine("MTMCL, version " + MeCore.version);
            message.AppendLine(s);
            message.AppendLine("\n\n-----------------ERROR REPORT----------------------\n");
            message.AppendFormat("Target Site: {0}", ex.TargetSite).AppendLine();
            message.AppendFormat("Error Type: {0}", ex.GetType()).AppendLine();
            message.AppendFormat("Messge: {0}", ex.Message).AppendLine();
            foreach (DictionaryEntry data in ex.Data)
                message.AppendLine(string.Format("Key:{0}\nValue:{1}", data.Key, data.Value));
            message.AppendLine(ex.StackTrace);
            var iex = ex;
            while (iex.InnerException != null)
            {
                message.AppendLine("------------Inner Exception------------");
                iex = iex.InnerException;
                message.AppendFormat("Target Site: {0}", ex.TargetSite).AppendLine();
                message.AppendFormat("Error Type: {0}", ex.GetType()).AppendLine();
                message.AppendFormat("Messge: {0}", ex.Message).AppendLine();
                foreach (DictionaryEntry data in ex.Data)
                    message.AppendLine(string.Format("Key:{0}\nValue:{1}", data.Key, data.Value));
                message.AppendLine("StackTrace");
                message.AppendLine(iex.StackTrace);
            }
            CreateLogInfoPart(message);
            txtMes.Text = message.ToString();
        }
        public ErrorReport (string s, string ex)
        {
            InitializeComponent();
            var message = new StringBuilder();
            message.AppendLine("MTMCL, version " + MeCore.version);
            message.AppendLine(s);
            message.AppendLine("\n\n-----------------ERROR REPORT----------------------\n");
            message.AppendLine(ex);
            CreateLogInfoPart(message);
            txtMes.Text = message.ToString();
        }

        private void CreateLogInfoPart(StringBuilder message) {
            Logger.stop(false);
            message.AppendLine("\n-----------------MTMCL LOG----------------------\n");
            var sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "mtmcl.log");
            message.AppendLine(sr.ReadToEnd());
            sr.Close();
            message.AppendLine("-------------System Infomation------------------\n");
            message.AppendFormat("Operating System : {0} ({1}) version {2}", util.OSHelper.GetOSName(), Environment.Is64BitOperatingSystem ? "x64" : "x32", Environment.OSVersion).AppendLine();
            message.AppendLine(".NET Framework:");
            message.AppendLine(util.OSHelper.GetDotNETs());
            message.AppendLine("and updates:");
            message.AppendLine(util.OSHelper.GetDotNETUpdates());
            Logger.start(FileMode.Append);
            Logger.log(message.ToString(), Logger.LogType.Error);
        }

        private void butEmail_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("mailto:lung1a16@hotmail.com?subject=" + HttpUtility.UrlEncode("MTMCL_Crash_Report") + "&body=" + HttpUtility.UrlEncode(txtMes.Text, Encoding.UTF8));
            }
            catch (Exception ex) {
                MessageBox.Show("unable to send email: " + ex.Message);
            }
        }

        private void butMCBBS_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.mcbbs.net/thread-555314-1-1.html");
            Copy();
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

        private void butClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void butRestart_Click(object sender, RoutedEventArgs e)
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            if (processName.IndexOf(".exe") == -1)
            {
                processName = processName + ".exe";
            }
            Process.Start(processName);
            Environment.Exit(-1);
        }
    }
}
