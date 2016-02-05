using MetoCraft.Lang;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace MetoCraft
{
    public static class MeCore
    {
        public static string version;
        public static Config Config;
        public static Dictionary<string, object> Language = new Dictionary<string, object>();
        public static string BaseDirectory = Environment.CurrentDirectory + '\\';
        private readonly static string Cfgfile = BaseDirectory + "mtmcl_config.json";
        public static NotiIcon NIcon = new NotiIcon();
        public static MainWindow MainWindow = null;
        public static Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;

        static MeCore()
        {
            version = Application.ResourceAssembly.FullName.Split('=')[1];
            version = version.Substring(0, version.IndexOf(','));
            Logger.log("MTMCL Ver." + version + "launching");
            if (File.Exists(Cfgfile))
            {
                Config = Config.Load(Cfgfile);
                Logger.log(string.Format("loaded {0}", Cfgfile));
                Logger.log(Config);
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
/*            if (!App.SkipPlugin)
            {
                LoadPlugin(LangManager.GetLangFromResource("LangName"));
            }*/
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
