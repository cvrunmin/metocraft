using ICSharpCode.SharpZipLib.Zip;
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
using System.Windows.Media.Imaging;

namespace MTMCL.Threads
{
    public class LaunchMCThread : MTMCLThread
    {
        private readonly Launch.LaunchGameInfo _LaunchOptions;
        private int _clientCrashReportCount;
        private System.Diagnostics.Process process = new System.Diagnostics.Process();
        public delegate void StateChangeEventHandler(string state);
        public delegate void GameExitHandler();
        public delegate void GameLaunched();
        public delegate void LaunchFail();
        public delegate void GameCrashedHandler(string content, string reportpath);
        public delegate void Log(string log);
        public delegate void UpdateAuth(Launch.Login.AuthInfo info);
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
        public LaunchMCThread(Launch.LaunchGameInfo options) {
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
                    VersionJson lib = _LaunchOptions.Version;
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
            //await TaskEx.Delay(1000);
            Dictionary<string, string> args = new Dictionary<string, string>();
            Launch.Login.AuthInfo ai = _LaunchOptions.Auth.Login();
            if (!ai.Pass | !string.IsNullOrWhiteSpace(ai.ErrorMsg))
            {
                OnLogged?.Invoke(Logger.HelpLog("error occurred: failed to authenticate"));
                MeCore.Dispatcher.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), ai.ErrorMsg, LangManager.GetLangFromResource("AuthFaultSolve")) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                Failed?.Invoke();
                return;
            }
            OnAuthUpdate?.Invoke(ai);
            args.Add("auth_access_token", ai.Session.ToString().Replace("-", ""));
            args.Add("auth_session", ai.Session.ToString().Replace("-", ""));
            args.Add("auth_player_name", ai.DisplayName);
            args.Add("version_name", _LaunchOptions.Version.id);
            args.Add("game_directory", ".");
            args.Add("game_assets", "assets");
            args.Add("assets_root", "assets");
            args.Add("assets_index_name", _LaunchOptions.Version.assets);
            args.Add("auth_uuid", ai.UUID.ToString().Replace("-", ""));
            args.Add("user_properties", ai.Prop);
            args.Add("user_type", ai.UserType);
            args.Add("version_type", "MTMCL_" +  MeCore.version);
            _LaunchOptions.Mode.Do(_LaunchOptions, ref args);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = _LaunchOptions.JavaPath;
            process.StartInfo.Arguments = Launch.LaunchHandler.CreateArgument(_LaunchOptions, args, MeCore.Config.ExtraJvmArg);
            process.StartInfo.WorkingDirectory = _LaunchOptions.MCPath;
            Logger.HelpLog(process.StartInfo.WorkingDirectory);
            Logger.HelpLog(process.StartInfo.FileName);
            Logger.HelpLog(process.StartInfo.Arguments);
            process.Exited += delegate (object sender, EventArgs e)
            {
                _LaunchOptions.Mode.DoAfter(_LaunchOptions);
                GameExit?.Invoke();
                //Dispatcher.Invoke(new Action(() => butPlayQuick.IsEnabled = true));
                if (process.ExitCode != 0)
                {
                    if (Directory.Exists(_LaunchOptions.MCPath + @"\crash-reports"))
                    {
                        if (_clientCrashReportCount != Directory.GetFiles(_LaunchOptions.MCPath + @"\crash-reports").Count())
                        {
                            OnLogged?.Invoke(Logger.HelpLog("found new crash report"));
                            var clientCrashReportDir = new DirectoryInfo(_LaunchOptions.MCPath + @"\crash-reports");
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
                            MeCore.Dispatcher.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("CrashNameFormat"), DateTime.Now.ToLongTimeString()), s) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/mccrash-banner.jpg")) })));
                            //MeCore.Dispatcher.Invoke(new Action(() => new MCCrash(s, lastClientCrashReportPath).Show()));
                            crashReportReader.Close();
                            _clientCrashReportCount = Directory.Exists(_LaunchOptions.MCPath + @"\crash-reports") ? Directory.GetFiles(_LaunchOptions.MCPath + @"\crash-reports").Count() : 0;
                        }
                    }
                }
            };
            try
            {
                foreach (var lib in _LaunchOptions.Version.libraries.ToUniversalLibrary())
                {
                    if (lib.isNative) {
                        Logger.HelpLog("Uncompress zip: " + lib.path);
                        var zipfile = new ZipInputStream(File.OpenRead(lib.path));
                        ZipEntry theEntry;
                        while ((theEntry = zipfile.GetNextEntry()) != null)
                        {
                            bool exc = false;
                            if (lib.extract != null)
                            {
                                if (lib.extract.exclude != null)
                                {
                                    if (lib.extract.exclude.Any(excfile => theEntry.Name.Contains(excfile)))
                                    {
                                        exc = true;
                                    }
                                }
                            }
                            if (exc) continue;
                            var filepath = new StringBuilder(Path.Combine(_LaunchOptions.MCPath, "$natives"));
                            filepath.Append("\\").Append(theEntry.Name);
                            Logger.HelpLog("Uncompressing object: " + filepath.ToString());
                            FileStream fileWriter = File.Create(filepath.ToString());
                            var data = new byte[2048];
                            while (true)
                            {
                                int size = zipfile.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    fileWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            fileWriter.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                OnLogged?.Invoke(Logger.HelpLog("error occurred: failed to uncompress native"));
                MeCore.Dispatcher.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), e.ToWellKnownExceptionString(), LangManager.GetLangFromResource("LibFaultSolve")) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                Failed?.Invoke();
                return;
            }
            
            if (!File.Exists(_LaunchOptions.JavaPath))
            {
                OnLogged?.Invoke(Logger.HelpLog("error occurred: no java is found"));
                MeCore.Dispatcher.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), new FileNotFoundException("javaw.exe not found", _LaunchOptions.JavaPath).ToWellKnownExceptionString(), LangManager.GetLangFromResource("JavaFaultSolve")) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                Failed?.Invoke();
                return;
            }
            try {
                /*if (!result.Success)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.OperatorException:
                        OnLogged?.Invoke(Logger.HelpLog("error occurred: failed to create operator"));
                        MeCore.Dispatcher.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), result.ErrorMessage) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                        break;
                    case ErrorType.Unknown:
                        OnLogged?.Invoke(Logger.HelpLog("error occurred: unknown exception: " + result.Exception));
                        MeCore.Dispatcher.Invoke(new Action(()=> MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), result.ErrorMessage) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                        break;
                }
                Failed?.Invoke();
            }
            else
            {*/
                _clientCrashReportCount = Directory.Exists(_LaunchOptions.MCPath + @"\crash-reports") ? Directory.GetFiles(_LaunchOptions.MCPath + @"\crash-reports").Count() : 0;
                TaskCountTime?.Invoke();
                OnLogged?.Invoke(Logger.HelpLog("game launched"));
                //OnAuthUpdate?.Invoke(result.Handle.Info);
                MeCore.Config.QuickChange("LastPlayVer", _LaunchOptions.Version.id);
                MeCore.Config.QuickChange("LastLaunchMode", _LaunchOptions.Mode.ModeType);
                process.EnableRaisingEvents = true;
                process.Start();
                await System.Threading.Tasks.Task.Factory.StartNew(process.WaitForExit);
            }
            catch (Exception e) {
                Logger.log(e);
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
                        url = url + file.path.Remove(0,MeCore.Config.MCPath.Length + 11).Replace("\\", "/");
                    }
                    Logger.log(url);
                    _downer.DownloadFile(Resources.UrlReplacer.toGoodLibUrl(url), file.path);
                }
                catch (WebException exception)
                {
                    OnLogged?.Invoke(Logger.HelpLog("failed to download library: " + file.name));
                    MeCore.Dispatcher.Invoke(new Action(()=> MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), exception.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                    return;
                }
            }
        }
    }
}
