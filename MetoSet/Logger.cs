using System;
using System.Globalization;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Collections;
using MTMCL.util;

namespace MTMCL
{
    static public class Logger
    {
        public enum LogType
        {
            Error, Info, Crash, Exception, Game, Warning
        }

        static public bool debug = false;
        static public bool LogReadOnly = false;
        static readonly Report.WinLog frmLog = new Report.WinLog();
        static StreamWriter swlog;
        internal static bool loaded;

        static public void start (FileMode mode = FileMode.Create, bool logshow = true)
        {
            try
            {
                swlog = new StreamWriter(new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\mtmcl.log", mode, FileAccess.Write, FileShare.Read), Encoding.UTF8);
                swlog.Flush();
                swlog.AutoFlush = true;
                loaded = true;
            }
            catch (UnauthorizedAccessException)
            {
                //System.Windows.MessageBox.Show("無法修改日誌\n无法修改日志\nFailed to edit the log\n" + e);
                LogReadOnly = true;
            }
            if (debug&logshow)
            {
                frmLog.Show();
            }
        }
        static public void stop (bool logclose = true)
        {
            swlog.Close();
            if (debug&logclose) frmLog.Close();
        }

        static private string writeInfo (LogType type = LogType.Info)
        {
            switch (type)
            {
                case LogType.Error:
                    return (DateTime.Now.ToString(CultureInfo.InvariantCulture) + "[ERROR]");
                case LogType.Info:
                    return (DateTime.Now.ToString(CultureInfo.InvariantCulture) + "[INFO]");
                case LogType.Crash:
                    return (DateTime.Now.ToString(CultureInfo.InvariantCulture) + "[CRASH]");
                case LogType.Exception:
                    return (DateTime.Now.ToString(CultureInfo.InvariantCulture) + "[WARN]");
                case LogType.Game:
                    return (DateTime.Now.ToString(CultureInfo.InvariantCulture) + "[GAME]");
                case LogType.Warning:
                    return (DateTime.Now.ToString(CultureInfo.InvariantCulture) + "[WARN]");
                default:
                    return (DateTime.Now.ToString(CultureInfo.InvariantCulture) + "[INFO]");
            }
        }
        static private void write (string str, LogType type = LogType.Info)
        {
            if (LogReadOnly) return;
            str = writeInfo(type) + str;
            swlog.WriteLine(str);
            if (debug) frmLog.WriteLine(str);
        }
        static private string HelpWrite (string str, LogType type = LogType.Info)
        {
            string a = writeInfo(type) + str;
            if (LogReadOnly) return a;
            swlog.WriteLine(a);
            if (debug) frmLog.WriteLine(a);
            return a;
        }
        static private void write (Stream s, LogType type = LogType.Info)
        {
            if (LogReadOnly) return;
            StreamReader sr = new StreamReader(s);
            write(sr.ReadToEnd(), type);
        }


        static public void log (string str, LogType type = LogType.Info)
        {
            write(str, type);
        }
        static public string HelpLog (string str, LogType type = LogType.Info)
        {
            return HelpWrite(str, type);
        }
        static public void log (Config cfg, LogType type = LogType.Info)
        {
            DataContractSerializer cfgSerializer = new DataContractSerializer(typeof(Config));
            MemoryStream ms = new MemoryStream();
            cfgSerializer.WriteObject(ms, cfg);
            ms.Position = 0;
            write(ms, type);
        }
        static public void log (Stream s, LogType type = LogType.Info)
        {
            StreamReader sr = new StreamReader(s);
            write(sr.ReadToEnd(), type);
        }
        static public void log (Exception ex, LogType type = LogType.Exception)
        {
            StringBuilder message = new StringBuilder();
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
            write(message.ToString(), type);
        }

        static public void log (LogType type = LogType.Info, params string[] messages)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string str in messages)
            {
                sb.Append(str);
            }
            write(sb.ToString(), type);
        }

        static public void log (params string[] messages)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string str in messages)
            {
                sb.Append(str);
            }
            write(sb.ToString());
        }

        static public void info (string message)
        {
            log(message);
        }

        static public void error (string message)
        {
            log(message, LogType.Error);
        }

        static public void error (Exception ex)
        {
            log(ex);
        }


    }
}
