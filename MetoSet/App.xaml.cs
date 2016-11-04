using MTMCL.Lang;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xaml;

namespace MTMCL
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        public static bool forceNonDedicate = false;
        public static EventWaitHandle ProgramStarted;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createNew;
            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, Process.GetCurrentProcess().ProcessName, out createNew);
            if (!createNew)
            {
                ProgramStarted.Set();
                Environment.Exit(3);
                return;
            }
            if (Array.IndexOf(e.Args, "-NotServer") != -1)
            {
                forceNonDedicate = true;
            }
            if (Array.IndexOf(e.Args, "-Debug") != -1)
                Logger.debug = true;
#if DEBUG
            Logger.debug = true;
#endif
            Logger.start();
#if DEBUG
#else
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
            if (Array.IndexOf(e.Args, "-Update") != -1)
            {
                var index = Array.IndexOf(e.Args, "-Update");
                if (index < e.Args.Length - 1)
                {
                    if (!e.Args[index + 1].StartsWith("-"))
                    {
                        DoUpdate(e.Args[index + 1]);
                    }
                }
            }
            if (Array.IndexOf(e.Args, "-UpdateReplace") != -1)
            {
                var index = Array.IndexOf(e.Args, "-UpdateReplace");
                if (index < e.Args.Length - 1)
                {
                    if (!e.Args[index + 1].StartsWith("-"))
                    {
                        DoUpdateReplace(e.Args[index + 1]);
                    }
                    else
                    {
                        DoUpdateReplace();
                    }
                }
                else
                {
                    DoUpdateReplace();
                }
            }
            if (Array.IndexOf(e.Args, "-UpdateDelete") != -1)
            {
                var index = Array.IndexOf(e.Args, "-UpdateDelete");
                if (index < e.Args.Length - 1)
                {
                    if (!e.Args[index + 1].StartsWith("-"))
                    {
                        DoUpdate(e.Args[index + 1]);
                    }
                }
            }
            Tuple<Launch.LaunchGameInfo, Tuple<string, MTMCL.Task.TaskListBar>, Notice.NoticeBalloon> a = null;
            if (Array.IndexOf(e.Args, "-QuickLaunch") != -1) {
                var index = Array.IndexOf(e.Args, "-QuickLaunch");
                var version = "";
                var auths = "";
                //var jvm = "";
                if (index < e.Args.Length - 1) {
                    for (int i = 1; i < e.Args.Length - index; i++)
                    {
                        var arg = e.Args[index + i];
                        if (!arg.StartsWith("-"))
                        {
                            var matches = System.Text.RegularExpressions.Regex.Match(arg, "(\\w+)=([\\w\\d\\s\\.\"'-]+)");
                            switch (matches.Groups[1].Value) {
                                case "version":
                                    version = matches.Groups[2].Value;
                                    break;
                                case "auth":
                                    auths = matches.Groups[2].Value;
                                    break;
                                //case "jvm":
                                //    jvm = matches.Groups[2].Value;
                                //    break;
                            }
                        }
                        else break;
                    }
                }
                bool flag = string.IsNullOrWhiteSpace(version), flag1 = string.IsNullOrWhiteSpace(auths)/*, flag2 = string.IsNullOrWhiteSpace(jvm)*/;
                if (flag & flag1)
                {
                    a = util.LaunchGameHelper.QuickLaunch();
                    goto last;
                }
                else
                {
                    if (flag) version = MeCore.Config.LastPlayVer;
                    if (flag1) auths = MeCore.Config.DefaultAuth;
                    //if (flag2) jvm = MeCore.Config.ExtraJvmArg;
                    var ver = Versions.VersionReader.GetVersion(MeCore.Config.MCPath, version);
                    if (ver == null) goto last;
                    Launch.Login.IAuth auth;
                    if (string.IsNullOrWhiteSpace(auths))
                    {
                        ACSelect ac = new ACSelect();
                        ac.ShowDialog();
                        auth = ac.auth;
                    }
                    else
                    {
                        SavedAuth dauth;
                        MeCore.Config.SavedAuths.TryGetValue(auths, out dauth);
                        if (dauth == null)
                        {
                            ACSelect ac = new ACSelect();
                            ac.ShowDialog();
                            auth = ac.auth;
                        }
                        else auth = dauth.AuthType.Equals("Yggdrasil") ? new Launch.Login.YggdrasilRefreshAuth(dauth.AccessToken) : new Launch.Login.AuthWarpper(new Launch.Login.AuthInfo { DisplayName = MeCore.Config.DefaultAuth, Session = dauth.AccessToken, UUID = dauth.UUID, UserType = dauth.UserType, Prop = dauth.Properies }) as Launch.Login.IAuth;
                    }

                    var option = Launch.LaunchGameInfo.CreateInfo(MeCore.Config.MCPath, auth, ver, MeCore.Config.Javaw, (int) MeCore.Config.Javaxmx);
                    a = util.LaunchGameHelper.LaunchGame(option, Launch.LaunchMode.GetMode(MeCore.Config.LastLaunchMode));
                }
            }
            last:
            WebRequest.DefaultWebProxy = null;
            base.OnStartup(e);
            if (a != null) {
                if (MeCore.MainWindow != null)
                {
                    MeCore.MainWindow.addTask(a.Item2.Item1, a.Item2.Item2);
                    MeCore.MainWindow.addBalloonNotice(a.Item3);
                }
            }
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var crash = new ErrorReport(e.ExceptionObject as Exception);
            crash.Show();
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            MeCore.Invoke(new Action(()=> {
                var crash = new ErrorReport(e.Exception);
                crash.Show();
            }));
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            if (e.Exception is XamlParseException)
            {
                if (e.Exception.InnerException != null)
                {
                    if (e.Exception.InnerException is FileLoadException)
                    {

                        return;
                    }
                }
            }
            var crash = new ErrorReport(e.Exception);
            crash.Show();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Logger.stop();
        }

        public static void AboutToExit()
        {
            Logger.stop();
        }
        [Obsolete("only for supporting old MTMCL update method")]
        private void DoUpdate()
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            if (processName.IndexOf(".exe") == -1)
            {
                processName = processName + ".exe";
            }
            var time = 0;
            while (time < 10)
            {
                try
                {
                    Logger.log(processName);
                    File.Copy(processName, "MTMCL.exe", true);
                    //Process.Start("MTMCL.exe", "-Update " + processName);
                    Logger.log("try launch");
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "MTMCL.exe",
                        Arguments = "-UpdateDelete " + processName
                    });
                    Logger.log("tried launch");
                    //Current.Shutdown(0);
                    Environment.Exit(0);
                    return;
                }
                catch (Exception e)
                {
                    Logger.error(e);
                }
                finally
                {
                    time++;
                }
            }
            MessageBox.Show("Failed to update automatically, please replace the outdated version with " + processName + " manually.\n自動升級失敗，請手動使用" + processName + "替代舊版文件");
            MessageBox.Show("Failed to update automatically, please replace the outdated version with " + processName + " manually.\n自動升級失敗，請手動使用" + processName + "替代舊版文件");
        }

        private void DoUpdate(string fileName)
        {
            File.Delete(fileName);
        }

        private void DoUpdateDelete(string fileName) {
            DoUpdate(fileName);
        }
        private void DoUpdateReplace() {
            DoUpdateReplace("MTMCL.exe");
        }
        private void DoUpdateReplace(string fileName) {
            var processName = Process.GetCurrentProcess().ProcessName;
            if (processName.IndexOf(".exe") == -1)
            {
                processName = processName + ".exe";
            }
            if (fileName.IndexOf(".exe") == -1)
            {
                fileName = fileName + ".exe";
            }
            var time = 0;
            while (time < 10)
            {
                try
                {
                    Logger.log(processName);
                    File.Copy(processName, fileName, true);
                    //Process.Start("MTMCL.exe", "-Update " + processName);
                    Logger.log("try launch");
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = "-UpdateDelete " + processName
                    });
                    Logger.log("tried launch");
                    //Current.Shutdown(0);
                    Environment.Exit(0);
                    return;
                }
                catch (Exception e)
                {
                    Logger.error(e);
                }
                finally
                {
                    time++;
                }
            }
            MessageBox.Show("Failed to update automatically, please replace the outdated version with " + processName + " manually.\n自動升級失敗，請手動使用" + processName + "替代舊版文件");
            MessageBox.Show("Failed to update automatically, please replace the outdated version with " + processName + " manually.\n自動升級失敗，請手動使用" + processName + "替代舊版文件");
        }
    }
}
