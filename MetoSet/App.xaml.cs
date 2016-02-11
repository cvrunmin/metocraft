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
        private static FileStream _appLock;
        public static bool forceNonDedicate = false;
        public static EventWaitHandle ProgramStarted;


        protected override void OnStartup(StartupEventArgs e)
        {
            bool createNew;
            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "MTMCLStart", out createNew);
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
            if (e.Args.Length == 0)   // 判断debug模式
                Logger.debug = false;
            else
                if (Array.IndexOf(e.Args, "-Debug") != -1)
                Logger.debug = true;
            Logger.start();
#if DEBUG
#else
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
            for (int i = 0; i < e.Args.Length; i++)
            {
                Logger.log(e.Args[i]);
            }
            if (Array.IndexOf(e.Args, "-Update") != -1)
            {
                Logger.log("found update argument");
                var index = Array.IndexOf(e.Args, "-Update");
                if (index < e.Args.Length - 1)
                {
                    Logger.log("found one or more arguments");
                    if (!e.Args[index + 1].StartsWith("-"))
                    {
                        DoUpdate(e.Args[index + 1]);
                    }
                    else
                    {
                        DoUpdate();
                    }
                }
                else
                {
                    Logger.log("try do update");
                    DoUpdate();
                }
            }
            /*if (Array.IndexOf(e.Args, "-SkipPlugin") != -1)
            {
                  App._skipPlugin = true;
            }*/
            /*try
            {
                _appLock = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "MEC.lck", FileMode.Create);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture));
                _appLock.Write(buffer, 0, buffer.Length);
                _appLock.Close();
                _appLock = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "MEC.lck", FileMode.Open);
            }
            catch (IOException)
            {
                MessageBox.Show(LangManager.GetLangFromResource("StartupDuplicate"));
                Environment.Exit(3);
            }*/
            WebRequest.DefaultWebProxy = null;  //禁用默认代理
            base.OnStartup(e);
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var crash = new ErrorReport(e.ExceptionObject as Exception);
            crash.Show();
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            var crash = new ErrorReport(e.Exception);
            crash.Show();
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
            //_appLock.Close();
            Logger.stop();
        }
        private void DoUpdate()
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            var time = 0;
            while (time < 10)
            {
                try
                {
                    Logger.log(processName);
                    File.Copy(processName, "MTMCL.exe", true);
                    Process.Start("MTMCL.exe", "-Update " + processName);
                    Current.Shutdown(0);
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
    }
}
