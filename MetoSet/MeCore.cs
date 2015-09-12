using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MetoSet
{
    class MeCore
    {
        public static Config Config;
        public static Dictionary<string, object> Language = new Dictionary<string, object>();
        public static string BaseDirectory = Environment.CurrentDirectory + '\\';
        private readonly static string Cfgfile = BaseDirectory + "metocraft.xml";
        static MeCore()
        {
//            BmclVersion = Application.ResourceAssembly.FullName.Split('=')[1];
//            BmclVersion = BmclVersion.Substring(0, BmclVersion.IndexOf(','));
//            Logger.log("BMCL V3 Ver." + BmclVersion + "正在启动");
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
            if (!App.SkipPlugin)
            {
                LoadPlugin(LangManager.GetLangFromResource("LangName"));
            }
#if DEBUG
#else
            ReleaseCheck();
#endif
        }

    }
}
