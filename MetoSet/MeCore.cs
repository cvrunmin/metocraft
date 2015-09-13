using MetoSet.Lang;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace MetoSet
{
    public static class MeCore
    {
        public static string version;
        public static Config Config;
        public static Dictionary<string, object> Language = new Dictionary<string, object>();
        public static string BaseDirectory = Environment.CurrentDirectory + '\\';
        private readonly static string Cfgfile = BaseDirectory + "metocraft.xml";
        public static NotiIcon NIcon = new NotiIcon();
        public static MainWindow MainWindow = null;
        public static Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;
        static MeCore()
        {
            version = Application.ResourceAssembly.FullName.Split('=')[1];
            version = version.Substring(0, version.IndexOf(','));
            Logger.log("Metocraft Ver." + version + "正在启动");
            if (File.Exists(Cfgfile))
            {
                Config = Config.Load(Cfgfile);
                if (Config.Passwd == null)
                {
                    Config.Passwd = new byte[0];   //V2的密码存储兼容
                }
                Logger.log(String.Format("加载{0}文件", Cfgfile));
                Logger.log(Config);
                LoadLanguage();
                ChangeLanguage(Config.Lang);
            }
            else
            {
                Config = new Config();
                Logger.log("加载默认配置");
                LoadLanguage();
            }
            if (Config.Javaw == "autosearch")
            {
                Config.Javaw = Config.GetJavaDir();
            }
            if (Config.Javaxmx == "autosearch")
            {
                Config.Javaxmx = (Config.GetMemory() / 4).ToString(CultureInfo.InvariantCulture);
            }
            LangManager.UseLanguage(Config.Lang);
/*            if (!App.SkipPlugin)
            {
                LoadPlugin(LangManager.GetLangFromResource("LangName"));
            }*/
#if DEBUG
#else
            ReleaseCheck();
#endif
        }
        public static void Invoke(Delegate invoke, object[] argObjects = null)
        {
            MeCore.Dispatcher.Invoke(invoke, argObjects);
        }
        public static void ChangeLanguage(string lang)
        {
            LangManager.UseLanguage(lang);
        }
        private static void LoadLanguage()
        {
            ResourceDictionary lang = LangManager.LoadLangFromResource("pack://application:,,,/Lang/en.xaml");
            MeCore.Language.Add((string)lang["DisplayName"], lang["LangName"]);
            LangManager.Add(lang["LangName"] as string, "pack://application:,,,/Lang/en.xaml");

            lang = LangManager.LoadLangFromResource("pack://application:,,,/Lang/zh-hans.xaml");
            MeCore.Language.Add((string)lang["DisplayName"], lang["LangName"]);
            LangManager.Add(lang["LangName"] as string, "pack://application:,,,/Lang/zh-hans.xaml");

            lang = LangManager.LoadLangFromResource("pack://application:,,,/Lang/zh-hant.xaml");
            MeCore.Language.Add((string)lang["DisplayName"], lang["LangName"]);
            LangManager.Add(lang["LangName"] as string, "pack://application:,,,/Lang/zh-hant.xaml");
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Lang"))
            {
                foreach (string langFile in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Lang", "*.xaml", SearchOption.TopDirectoryOnly))
                {
                    lang = LangManager.LoadLangFromResource(langFile);
                    MeCore.Language.Add((string)lang["DisplayName"], lang["LangName"]);
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
