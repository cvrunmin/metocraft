using MTMCL.Lang;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace MTMCL
{
    public static class MeCore
    {
        public static string version;
        public static Config Config;
        public static Server.ServerInfo ServerCfg;
        public static bool IsServerDedicated;
        public static Dictionary<string, object> Language = new Dictionary<string, object>();
        public static string BaseDirectory = Environment.CurrentDirectory + '\\';
        private readonly static string Cfgfile = BaseDirectory + "mtmcl_config.json";
        private readonly static string Serverfile = BaseDirectory + "mtmcl_server_config.json";
        public static string DefaultBG = "pack://application:,,,/Resources/bg.png";
        public static NotiIcon NIcon = new NotiIcon();
        public static MainWindow MainWindow = null;
        public static Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;

        static MeCore()
        {
            version = Application.ResourceAssembly.FullName.Split('=')[1];
            version = version.Substring(0, version.IndexOf(','));
            Logger.log("----------"+DateTime.Now.ToLongTimeString()+" launch log----------");
            Logger.log("MTMCL Ver." + version + " launching");
            if (File.Exists(Serverfile))
            {
                ServerCfg = Server.ServerInfo.Load(Serverfile);
                if (ServerCfg.Ignore)
                {
                    IsServerDedicated = false;
                    Logger.log("Launching normal version due to failure to read the server config");
                }
                else if (App.forceNonDedicate)
                {
                    IsServerDedicated = false;
                    Logger.log("Launching normal version due to the argument");
                }
                else
                {
                    IsServerDedicated = true;
                    Logger.log("Launching server-dedicated version");
                }
            }
            else
            {
                Logger.log("Launching normal version as the server config file is missing");
            }
            if (File.Exists(Cfgfile))
            {
                Config = Config.Load(Cfgfile);
                Logger.log(string.Format("loaded {0}", Cfgfile));
                Logger.log(Config.ToReadableLog());
                LoadLanguage();
                ChangeLanguage(Config.Lang);
            }
            else
            {
                Config = new Config();
                Logger.log("loaded default config");
                LoadLanguage();
            }
            if (Config.Javaw == "autosearch")
            {
                Config.Javaw = KMCCC.Tools.SystemTools.FindValidJava().First();
            }
            if (Config.Javaxmx == "autosearch")
            {
                Config.Javaxmx = (KMCCC.Tools.SystemTools.GetTotalMemory() / 4).ToString(CultureInfo.InvariantCulture);
            }
            LangManager.UseLanguage(Config.Lang);
#if DEBUG
#else
//            ReleaseCheck();
#endif
        }
        public static void Invoke(Delegate invoke, object[] argObjects = null)
        {
            Dispatcher.Invoke(invoke, argObjects);
        }
        public static void ChangeLanguage(string lang)
        {
            LangManager.UseLanguage(lang);
        }
        private static void LoadLanguage()
        {
            ResourceDictionary lang = LangManager.LoadLangFromResource("pack://application:,,,/Lang/en.xaml");
            Language.Add((string)lang["DisplayName"], lang["LangName"]);
            LangManager.Add(lang["LangName"] as string, "pack://application:,,,/Lang/en.xaml");

            lang = LangManager.LoadLangFromResource("pack://application:,,,/Lang/zh-CHS.xaml");
            Language.Add((string)lang["DisplayName"], lang["LangName"]);
            LangManager.Add(lang["LangName"] as string, "pack://application:,,,/Lang/zh-CHS.xaml");

            lang = LangManager.LoadLangFromResource("pack://application:,,,/Lang/zh-CHT.xaml");
            Language.Add((string)lang["DisplayName"], lang["LangName"]);
            LangManager.Add(lang["LangName"] as string, "pack://application:,,,/Lang/zh-CHT.xaml");
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Lang"))
            {
                foreach (string langFile in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Lang", "*.xaml", SearchOption.TopDirectoryOnly))
                {
                    lang = LangManager.LoadLangFromResource(langFile);
                    Language.Add((string)lang["DisplayName"], lang["LangName"]);
                    LangManager.Add(lang["LangName"] as string, langFile);
                }
            }
            else
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\Lang");
            }
        }
    }
}
