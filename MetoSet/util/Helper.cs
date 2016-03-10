using KMCCC.Launcher;
using MTMCL.Lang;
using MTMCL.Versions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTMCL.util
{
    static class FileHelper
    {
        static public void dircopy(string from, string to)
        {
            DirectoryInfo dir = new DirectoryInfo(from);
            if (!Directory.Exists(to))
            {
                Directory.CreateDirectory(to);
            }
            foreach (DirectoryInfo sondir in dir.GetDirectories())
            {
                dircopy(sondir.FullName, to + "\\" + sondir.Name);
            }
            foreach (FileInfo file in dir.GetFiles())
            {
                File.Copy(file.FullName, to + "\\" + file.Name, true);
            }
        }

        static public bool IfFileVaild(string Path, long Length = -1)
        {
            if (!File.Exists(Path))
            {
                return false;
            }
            if (new FileInfo(Path).Length == 0)
            {
                return false;
            }
            if (Length != -1)
            {
                if (new FileInfo(Path).Length != Length)
                    return false;
            }
            return true;
        }
        static public bool IfFileVaild(string Path, long Length, string sha1)
        {
            if (!File.Exists(Path))
            {
                return false;
            }
            if (new FileInfo(Path).Length == 0)
            {
                return false;
            }
            if (Length > 0)
            {
                if (new FileInfo(Path).Length != Length)
                {
                    return false;
                }
            }
            if (!string.IsNullOrWhiteSpace(sha1))
            {
                if (!hashSHA1(File.ReadAllBytes(Path)).Equals(sha1))
                {
                    return false;
                }
            }
            return true;
        }
        private static string hashSHA1(byte[] data)
        {
            byte[] hashed = System.Security.Cryptography.SHA1.Create().ComputeHash(data);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashed.Length; i++)
            {
                builder.Append(hashed[i].ToString());
            }
            return builder.ToString();
        }
        static public void CreateDirectoryIfNotExist(string Dir)
        {
            if (!Directory.Exists(Dir))
            {
                Directory.CreateDirectory(Dir);
            }
        }

        static public void CreateDirectoryForFile(string File)
        {
            CreateDirectoryIfNotExist(Path.GetDirectoryName(File));
        }

    }
    static class TimeHelper
    {
        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        static readonly double MaxUnixSeconds = (DateTime.MaxValue - UnixEpoch).TotalSeconds;

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            return unixTimeStamp > MaxUnixSeconds
               ? UnixEpoch.AddMilliseconds(unixTimeStamp)
               : UnixEpoch.AddSeconds(unixTimeStamp);
        }
    }
    static class ContentHelper
    {
        public static void AddContentFromSpecficString(this System.Windows.Controls.TextBlock textblock, string spstring)
        {
            string[] strings = spstring.Split('[', ']');
            bool recordBold = false;
            StringBuilder st = new StringBuilder();
            for (int i = 0; i < strings.Length; i++)
            {
                switch (strings[i])
                {
                    case "/bold/":
                        recordBold = true;
                        break;
                    case "/endbold/":
                        if (recordBold)
                        {
                            recordBold = false;
                            textblock.Inlines.Add(new System.Windows.Documents.Run(st.ToString()) { FontWeight = System.Windows.FontWeights.Bold });
                            st.Clear();
                        }
                        break;
                    case "/newline/":
                        textblock.Inlines.Add(new System.Windows.Documents.LineBreak());
                        break;
                    default:
                        if (!recordBold)
                        {
                            textblock.Inlines.Add(strings[i]);
                        }
                        else
                        {
                            st.Append(strings[i]);
                        }
                        break;
                }
            }
        }

        public static string ToWellKnownExceptionString(this Exception ex) {
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
            message.AppendLine("\n\n-----------------MTMCL LOG----------------------\n");
            var sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "mtmcl.log");
            message.AppendLine(sr.ReadToEnd());
            sr.Close();
            return message.ToString();
        }
    }
    static class LibraryHelper
    {
        public static bool CheckLibrary(this KMCCC.Launcher.Version version)
        {
            try
            {
                var libs = version.Libraries.Select(lib => App.core.GetLibPath(lib));
                var natives = version.Natives.Select(native => App.core.GetNativePath(native));
                foreach (string libflie in libs)
                {
                    if (!File.Exists(libflie))
                    {
                        return false;
                    }
                }
                foreach (string libflie in natives)
                {
                    if (!File.Exists(libflie))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}