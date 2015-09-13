using MetoSet.Lang;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace MetoSet
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        private static FileStream _appLock;
        protected override void OnStartup(StartupEventArgs e)
        {
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

#endif
/*            if (Array.IndexOf(e.Args, "-Update") != -1)
            {
                var index = Array.IndexOf(e.Args, "-Update");
                if (index < e.Args.Length - 1)
                {
                    if (!e.Args[index + 1].StartsWith("-"))
                    {
                        DoUpdate(e.Args[index + 1]);
                    }
                    else
                    {
                        DoUpdate();
                    }
                }
            }*/
/*            if (Array.IndexOf(e.Args, "-SkipPlugin") != -1)
            {
                App._skipPlugin = true;
            }*/
            try
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
                MessageBox.Show(LangManager.GetLangFromResource("StartupDuplicate"));
                Environment.Exit(3);
            }
            WebRequest.DefaultWebProxy = null;  //禁用默认代理
            base.OnStartup(e);
        }
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
//            var crash = new CrashHandle(e.Exception);
//            crash.Show();
        }

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
//            var crash = new CrashHandle(e.Exception);
//            crash.Show();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Logger.stop();
        }

        public static void AboutToExit()
        {
            _appLock.Close();
            Logger.stop();
        }
    }
}
