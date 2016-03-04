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

namespace MTMCL.Threads
{
    public class LaunchMCThread : MTMCLThread
    {
        private readonly LaunchOptions _LaunchOptions;
        private int _clientCrashReportCount;

        public delegate void StateChangeEventHandler(string state);
        public delegate void GameExitHandler();
        public delegate void CountTime();
        public delegate void GameCrashedHandler(string content, string reportpath);
        public event StateChangeEventHandler StateChange;
        public event GameExitHandler GameExit;
        public event CountTime TaskCountTime;
        public event GameCrashedHandler GameCrash;
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
        public void Start()
        {
            //var thread = new Thread(Run);
            var task = new System.Threading.Tasks.Task(Run);
            task.Start();
            //thread.Start();
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
                    var thDL = new Thread(new ThreadStart(delegate
                    {
                        WebClient _downer = new WebClient();
                        int i = 0;
                        foreach (var libfile in libs)
                        {
                            i++;
                            OnStateChange(string.Format(LangManager.GetLangFromResource("SubTaskDLLib"), (((float)i / libs.Count()) * 100f).ToString() + "%"));
                            if (!File.Exists(libfile.path))
                            {
                                Logger.log("Start downloading " + libfile.path, Logger.LogType.Info);
                                Download(libfile);
                            }
                        }
                    }));
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
                            Logger.log("found new crash report");
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
                        //new KnownErrorReport(result.ErrorMessage, Lang.LangManager.GetLangFromResource("JavaFaultSolve")).Show();
                        break;
                    case ErrorType.AuthenticationFailed:
                        //new KnownErrorReport(result.ErrorMessage, Lang.LangManager.GetLangFromResource("AuthFaultSolve")).Show();
                        break;
                    case ErrorType.OperatorException:
                        //new KnownErrorReport(result.ErrorMessage).Show();
                        break;
                    case ErrorType.UncompressingFailed:
                        //new KnownErrorReport(result.ErrorMessage, Lang.LangManager.GetLangFromResource("LibFaultSolve")).Show();
                        break;
                    case ErrorType.Unknown:
                        //new KnownErrorReport(result.ErrorMessage).Show();
                        break;
                }
            }
            else
            {
                _clientCrashReportCount = Directory.Exists(App.core.GameRootPath + @"\crash-reports") ? Directory.GetFiles(App.core.GameRootPath + @"\crash-reports").Count() : 0;
                //butPlayQuick.IsEnabled = false;
                OnStateChange("");
                TaskCountTime?.Invoke();
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
                        url = Resources.UrlReplacer.getForgeMaven(file.url) + file.path.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/");
                    }
                }
                else
                {
                    url = url + file.path.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/");
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
                            url = Resources.UrlReplacer.getForgeMaven(file.url, 1) + file.path.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/");
                        }
                    }
                    else
                    {
                        url = url + file.path.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/");
                    }
                    Logger.log(url);
                    _downer.DownloadFile(Resources.UrlReplacer.toGoodLibUrl(url), file.path);
                }
                catch (WebException exception)
                {
                    //MeCore.Invoke(new Action(() => new ErrorReport(exception).Show()));
                    return;
                }
            }
        }
    }
}
