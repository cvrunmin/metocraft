using MTMCL.Lang;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;

namespace MTMCL
{
    public static class MeCore
    {
        public static string version;
        public static Config Config;
        internal static Customize.TileColor TileColor;
        public static Resources.BGHistory bghistory;
        public static bool needGuide = false;
        //public static Server.ServerInfo ServerCfg;
        public static bool IsServerDedicated;
        public static Dictionary<string, object> Language = new Dictionary<string, object>();
        public static Dictionary<string, MahApps.Metro.Accent> Color = new Dictionary<string, MahApps.Metro.Accent>();
        internal static List<Themes.Theme> themes = new List<Themes.Theme>();
        public static string BaseDirectory = Environment.CurrentDirectory + '\\';
        public static string DataDirectory = Environment.CurrentDirectory + '\\' + "MTMCL" + '\\';
        private readonly static string Cfgfile = DataDirectory + "mtmcl_config.json";
        private readonly static string CfgfileOrigin = BaseDirectory + "mtmcl_config.json";
        public static string DefaultBG = "pack://application:,,,/Resources/bg.png";
        //public static NotiIcon NIcon = new NotiIcon();
        public static MainWindow MainWindow = null;
        private static Application thisApplication = Application.Current;
        public static Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;
        static MeCore()
        {
            version = Application.ResourceAssembly.FullName.Split('=')[1];
            version = version.Substring(0, version.IndexOf(','));
            Logger.log("----------" + DateTime.Now.ToLongTimeString() + " launch log----------");
            Logger.log("MTMCL Ver." + version + " launching");
            Logger.log("Appdata: " + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            if (File.Exists(Cfgfile))
            {
                LoadConfig(Cfgfile);
            }
            else if (File.Exists(CfgfileOrigin)) {
                LoadConfig(CfgfileOrigin);
            }
            else
            {
                needGuide = true;
                Config = new Config();
                Logger.log("loaded default config");
                LoadLanguage();
                LoadColor();
            }
            if (Config.requiredGuide)
            {
                needGuide = true;
            }
            if (Config.Javaw == "autosearch")
            {
                Config.Javaw = Config.GetJavaDir() ?? "javaw.exe";
            }
            if (Config.Javaxmx == -1)
            {
                Config.Javaxmx = Config.GetMemory() / 4;
            }
            LangManager.UseLanguage(Config.Lang);
            TileColor = Customize.TileColor.Load(DataDirectory + "\\mtmcl_tile_color.json");
#if DEBUG
#else
            if (!Config.failUpdateLastTime) ReleaseCheck();
            else Config.failUpdateLastTime = false;
#endif
        }
        private static void LoadConfig(string path) {
            Config = Config.Load(path);
            Logger.log(string.Format("loaded {0}", path));
            Logger.log(Config.ToReadableLog());

            LoadLanguage();
            LoadColor();
            LoadTheme();
            if (Config.Server != null)
            {
                if (App.forceNonDedicate)
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
                IsServerDedicated = false;
                Logger.log("Launching normal version due to null server info");
            }
            if (string.IsNullOrWhiteSpace(Config.MCPath))
            {
                Logger.log("Minecraft path is null or whitespace. Guide is required.");
                needGuide = true;
            }
            if (string.IsNullOrWhiteSpace(Config.Javaw))
            {
                Logger.log("javaw.exe path is null or whitespace. Guide is required.");
                needGuide = true;
            }
        }
        public static void ReleaseCheck()
        {
            if (Config.CheckUpdate)
            {
                var updateChecker = new Update.Updater();
                updateChecker.CheckFinishEvent += UpdateCheckerOnCheckFinishEvent;
            }
        }
        private static async void UpdateCheckerOnCheckFinishEvent(object sender, Update.CheckUpdateFinishEventArgs e)
        {
            if (e.UpdateAvailable)
            {
                if (MainWindow != null)
                {
                dddd:
                    if (!MainWindow.IsLoaded)
                    {
                        await System.Threading.Tasks.TaskEx.Delay(100);
                        goto dddd;
                    }
                    MessageDialogResult result = await MainWindow.ShowMessageAsync(LangManager.GetLangFromResource("UpdateFound"), e.UpdateInfo, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { AffirmativeButtonText = LangManager.GetLangFromResource("UpdateAccept"), NegativeButtonText = LangManager.GetLangFromResource("UpdateDeny") });
                    if (result == MessageDialogResult.Affirmative)
                    {
                        MainWindow.gridMain.Visibility = Visibility.Collapsed;
                        MainWindow.gridHome.Visibility = Visibility.Collapsed;
                        MainWindow.butHome.Visibility = Visibility.Collapsed;
                        MainWindow.gridOthers.Children.Clear();
                        MainWindow.gridOthers.Visibility = Visibility.Visible;
                        MainWindow.gridOthers.Margin = new Thickness(0);
                        await System.Threading.Tasks.TaskEx.Delay(500);
                        MainWindow.gridOthers.Children.Add(new Update.Update(e.UpdateBuild, e.UpdateAddress));
                    }
                }
            }
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
            if (Directory.Exists(DataDirectory + "Lang"))
            {
                foreach (string langFile in Directory.GetFiles(DataDirectory + "Lang", "*.xaml", SearchOption.TopDirectoryOnly))
                {
                    lang = LangManager.LoadLangFromResource(langFile);
                    Language.Add((string)lang["DisplayName"], lang["LangName"]);
                    LangManager.Add(lang["LangName"] as string, langFile);
                }
            }
            else
            {
                Directory.CreateDirectory(DataDirectory + "Lang");
            }
        }
        private static void LoadColor()
        {
            ResourceDictionary color = new ResourceDictionary();
            for (int i = 0; i < 23; i++)
            {
                string s = Enum.GetName(typeof(ColorScheme), i);
                Color.Add(s, MahApps.Metro.ThemeManager.GetAccent(s));
            }
            if (Directory.Exists(DataDirectory + "Color"))
            {
                foreach (string file in Directory.GetFiles(DataDirectory + "Color", "*.xaml", SearchOption.TopDirectoryOnly))
                {
                    color.Source = new Uri(file);
                    string s = Path.GetFileNameWithoutExtension(file);
                    MahApps.Metro.ThemeManager.AddAccent(s, color.Source);
                    Color.Add(s, MahApps.Metro.ThemeManager.GetAccent(s));
                }
            }
            else
            {
                Directory.CreateDirectory(DataDirectory + "Color");
            }
        }
        internal static void LoadTheme () {
            if (Directory.Exists(DataDirectory + "Themes")) {
                foreach (string file in Directory.GetFiles(DataDirectory + "Themes", "*.mtheme", SearchOption.TopDirectoryOnly))
                {
                    var t = Themes.Theme.LoadMTMCLTheme(file);
                    if(t!=null) themes.Add(t);
                }
                foreach (string file in Directory.GetFiles(DataDirectory + "Themes", "*.mthemepack", SearchOption.TopDirectoryOnly))
                {
                    var t = Themes.Theme.readMTMCLThemeInstantly(file);
                    if(t!=null)themes.Add(t);
                }
            }
        }
        internal static void Refresh()
        {
            Language.Clear();
            LangManager.Clear();
            Color.Clear();
            LoadLanguage();
            LoadColor();
        }
        private enum ColorScheme
        {
            Red, Green, Blue, Purple, Orange, Lime, Emerald, Teal, Cyan, Cobalt, Indigo, Violet, Pink, Magenta, Crimson, Amber, Yellow, Brown, Olive, Steel, Mauve, Taupe, Sienna
        }
        public static void Halt(int code = 0)
        {
            thisApplication.Shutdown(code);
        }

        public static void SingleInstance(Window window)
        {
            System.Threading.ThreadPool.RegisterWaitForSingleObject(App.ProgramStarted, OnAnotherProgramStarted, window, -1, false);
        }

        private static void OnAnotherProgramStarted(object state, bool timedout)
        {
            var window = state as Window;
            //NIcon.ShowBalloonTip(2000, LangManager.GetLangFromResource("MTMCLHiddenInfo"));
            if (window != null)
            {
                Dispatcher.Invoke(new Action(window.Show));
                Dispatcher.Invoke(new Action(() => { window.Activate(); }));
            }
        }
    }
}
