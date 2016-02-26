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

#if DEBUG
#else
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif

            WebRequest.DefaultWebProxy = null;  //禁用默认代理
            base.OnStartup(e);
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //var crash = new ErrorReport(e.ExceptionObject as Exception);
            //crash.Show();
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            //var crash = new ErrorReport(e.Exception);
            //crash.Show();
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
            //var crash = new ErrorReport(e.Exception);
            //crash.Show();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            //Logger.stop();
        }

        public static void AboutToExit()
        {
            //_appLock.Close();
            //Logger.stop();
        }
    }
}
