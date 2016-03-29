using KMCCC.Launcher;
using MTMCL.Lang;
using MTMCL.util;
using MTMCL.Versions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MTMCL.Threads
{
    public class LaunchMCThread : MTMCLThread
    {
        private readonly LaunchOptions _LaunchOptions;
        private int _clientCrashReportCount;

        public delegate void StateChangeEventHandler(string state);
        public delegate void GameExitHandler();
        public delegate void GameLaunched();
        public delegate void LaunchFail();
        public delegate void GameCrashedHandler(string content, string reportpath);
        public delegate void Log(string log);
        public delegate void UpdateAuth(KMCCC.Authentication.AuthenticationInfo info);
        public event UpdateAuth OnAuthUpdate;
        public event StateChangeEventHandler StateChange;
        public event GameExitHandler GameExit;
        public event GameLaunched TaskCountTime;
        public event GameCrashedHandler GameCrash;
        public event LaunchFail Failed;
        public event Log OnLogged;
        private void OnStateChange(string state)
        {
            var handler = StateChange;
            if (handler != null) MeCore.Invoke(new Action(() => handler(state)));
        }
        private void OnGameCrash(string content, string path)
        {
            var handler = GameCrash;
            if (handler != null) MeCore.Invoke(new Action(() => handler(content, path)));
        }
        public LaunchMCThread(LaunchOptions options) {
            _LaunchOptions = options;
        }
        public override Thread Start()
        {
            var task = new Thread(Run);
            task.Start();
            return task;
        }
        private async void Run() {
            OnStateChange(LangManager.GetLangFromResource("SubTaskLaunch"));
            Thread.SpinWait(750);
            OnStateChange(LangManager.GetLangFromResource("SubTaskCheckLib"));
            if (!_LaunchOptions.Version.CheckLibrary())
            {
                try
                {
                    List<LibraryUniversal> libs = new List<LibraryUniversal>();
                    VersionJson lib = LitJson.JsonMapper.ToObject<VersionJson>(new LitJson.JsonReader(new StreamReader(App.core.GetVersionJsonPath(_LaunchOptions.Version))));
                    libs = lib.libraries.ToUniversalLibrary();
                        WebClient _downer = new WebClient();
                        int i = 0;
                        foreach (var libfile in libs)
                        {
                            i++;
                            OnStateChange(string.Format(LangManager.GetLangFromResource("SubTaskDLLib"), (((float)i / libs.Count()) * 100f).ToString() + "%"));
                            if (!File.Exists(libfile.path))
                            {
                                OnLogged?.Invoke(Logger.HelpLog("Start downloading " + libfile.path, Logger.LogType.Info));
                                Download(libfile);
                            }
                        }
                }
                catch { }
            }
            new Assets.Assets(_LaunchOptions.Version);
            await System.Threading.Tasks.Task.Delay(1000);
            App.core.GameExit += delegate (LaunchHandle handle, int code)
            {
                GameExit?.Invoke();
                //Dispatcher.Invoke(new Action(() => butPlayQuick.IsEnabled = true));
                if (code != 0)
                {
                    if (Directory.Exists(App.core.GameRootPath + @"\crash-reports"))
                    {
                        if (_clientCrashReportCount != Directory.GetFiles(App.core.GameRootPath + @"\crash-reports").Count())
                        {
                            OnLogged?.Invoke(Logger.HelpLog("found new crash report"));
                            var clientCrashReportDir = new DirectoryInfo(App.core.GameRootPath + @"\crash-reports");
                            var lastClientCrashReportPath = "";
                            var lastClientCrashReportModifyTime = DateTime.MinValue;
                            foreach (var clientCrashReport in clientCrashReportDir.GetFiles())
                            {
                                if (lastClientCrashReportModifyTime < clientCrashReport.LastWriteTime)
                                {
                                    lastClientCrashReportPath = clientCrashReport.FullName;
                                }
                            }
                            var crashReportReader = new StreamReader(lastClientCrashReportPath, Encoding.Default);
                            var s = crashReportReader.ReadToEnd();
                            Logger.log(s, Logger.LogType.Crash);
                            OnGameCrash(s, lastClientCrashReportPath);
                            MeCore.Dispatcher.Invoke(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("CrashNameFormat"), DateTime.Now.ToLongTimeString()), s) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/mccrash-banner.jpg")) }));
                            //MeCore.Dispatcher.Invoke(new Action(() => new MCCrash(s, lastClientCrashReportPath).Show()));
                            crashReportReader.Close();
                        }
                    }
                }
            };
            var result = App.core.Launch(_LaunchOptions);
            if (!result.Success)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.NoJAVA:
                        OnLogged?.Invoke(Logger.HelpLog("error occurred: no java is found"));
                        MeCore.Dispatcher.Invoke(() => MeCore.MainWindow.addNotice( new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), result.ErrorMessage, LangManager.GetLangFromResource("JavaFaultSolve")) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) }));
                        break;
                    case ErrorType.AuthenticationFailed:
                        OnLogged?.Invoke(Logger.HelpLog("error occurred: failed to authenticate"));
                        MeCore.Dispatcher.Invoke(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), result.ErrorMessage, LangManager.GetLangFromResource("AuthFaultSolve")) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) }));
                        break;
                    case ErrorType.OperatorException:
                        OnLogged?.Invoke(Logger.HelpLog("error occurred: failed to create operator"));
                        MeCore.Dispatcher.Invoke(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), result.ErrorMessage) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) }));
                        break;
                    case ErrorType.UncompressingFailed:
                        OnLogged?.Invoke(Logger.HelpLog("error occurred: failed to uncompress native"));
                        MeCore.Dispatcher.Invoke(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), result.ErrorMessage, LangManager.GetLangFromResource("LibFaultSolve")) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) }));
                        break;
                    case ErrorType.Unknown:
                        OnLogged?.Invoke(Logger.HelpLog("error occurred: unknown exception: " + result.Exception));
                        MeCore.Dispatcher.Invoke(()=> MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), result.ErrorMessage) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) }));
                        break;
                }
                Failed?.Invoke();
            }
            else
            {
                _clientCrashReportCount = Directory.Exists(App.core.GameRootPath + @"\crash-reports") ? Directory.GetFiles(App.core.GameRootPath + @"\crash-reports").Count() : 0;
                //butPlayQuick.IsEnabled = false;
                //OnStateChange("");
                TaskCountTime?.Invoke();
                OnLogged?.Invoke(Logger.HelpLog("game launched"));
                OnAuthUpdate?.Invoke(result.Handle.Info);
                MeCore.Config.QuickChange("LastPlayVer",_LaunchOptions.Version.Id);
                //MeCore.NIcon.ShowBalloonTip(3000, "Successful to launch " + versions[comboVer.SelectedIndex].Id);
            }
        }
        private void Download(LibraryUniversal file)
        {
            WebClient _downer = new WebClient();
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(file.path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file.path));
                }
                string url = Resources.UrlReplacer.getLibraryUrl();
                if (!string.IsNullOrWhiteSpace(file.url))
                {
                    if (file.url.StartsWith("https://libraries.minecraft.net/"))
                    {
                        url = Resources.UrlReplacer.toGoodLibUrl(file.url);
                    }
                    else {
                        url = Resources.UrlReplacer.getForgeMaven(file.url) + file.path.Remove(0, App.core.GameRootPath.Length + 11).Replace("\\", "/");
                    }
                }
                else
                {
                    url = url + file.path.Remove(0, App.core.GameRootPath.Length + 11).Replace("\\", "/");
                }
#if DEBUG
                System.Windows.MessageBox.Show(url);
#endif
                Logger.log(url);
                _downer.DownloadFile(url, file.path);
            }
            catch (WebException ex)
            {
                Logger.log(ex);
                Logger.log("原地址下载失败，尝试BMCL源" + file.url);
                try
                {
                    string url = Resources.UrlReplacer.getLibraryUrl(1);
                    if (!string.IsNullOrWhiteSpace(file.url))
                    {
                        if (file.url.StartsWith("https://libraries.minecraft.net/"))
                        {
                            url = Resources.UrlReplacer.toGoodLibUrl(file.url, 1);
                        }
                        else {
                            url = Resources.UrlReplacer.getForgeMaven(file.url, 1) + file.path.Remove(0, App.core.GameRootPath.Length + 11).Replace("\\", "/");
                        }
                    }
                    else
                    {
                        url = url + file.path.Remove(0, App.core.GameRootPath.Length + 11).Replace("\\", "/");
                    }
                    Logger.log(url);
                    _downer.DownloadFile(Resources.UrlReplacer.toGoodLibUrl(url), file.path);
                }
                catch (WebException exception)
                {
                    OnLogged?.Invoke(Logger.HelpLog("failed to download library: " + file.name));
                    MeCore.Dispatcher.Invoke(()=> MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), exception.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) }));
                    return;
                }
            }
        }
    }
}
