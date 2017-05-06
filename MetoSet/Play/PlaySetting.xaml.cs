using MahApps.Metro.Controls.Dialogs;
using MTMCL.Assets;
using MTMCL.Lang;
using MTMCL.Task;
using MTMCL.util;
using MTMCL.Versions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using static MTMCL.util.FileHelper;

namespace MTMCL
{
    /// <summary>
    /// PlaySetting.xaml 的互動邏輯
    /// </summary>
    public partial class PlaySetting : Grid
    {
        PlayNew parent;
        Versions.VersionJson mcversion { get; set; }
        public PlaySetting()
        {
            InitializeComponent();
        }

        public PlaySetting(PlayNew parent, VersionJson mcver) : this() {
            this.parent = parent;
            mcversion = mcver;
            DataContext = mcversion;
            if (mcversion.errored)
            {
                tabAssets.Visibility = Visibility.Collapsed;
                tabLib.Visibility = Visibility.Collapsed;
                tabs.SelectedIndex = 2;
                butPlay.IsEnabled = false;
                butDLAssets.IsEnabled = false;
                gridMissInherit.Visibility = Visibility.Visible;
                if (mcversion.baseErrored)
                    butDLtoFix.Visibility = Visibility.Collapsed;
            }
        }

        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }
        private void Back()
        {
            MeCore.MainWindow.gridOthers.Children.Clear();
            MeCore.MainWindow.gridOthers.Children.Add(parent);
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridOthers.BeginAnimation(OpacityProperty, ani);
        }

        private void RefreshAsset()
        {
            try
            {
                Dispatcher.Invoke(new Action(() => {
                    gridNoIndex.Visibility = Visibility.Collapsed;
                    gridRefreshing.Visibility = Visibility.Visible;
                }));
                var assets = GetAssetsIndexObject(null);
                Logger.log(assets.Count.ToString(CultureInfo.InvariantCulture), " assets in total");
                var dt = new DataTable();
                dt.Columns.Add("Assets");
                dt.Columns.Add("Exist");
                foreach (KeyValuePair<string, AssetsEntity> entity in assets)
                {
                    dt.Rows.Add(entity.Key, AssetsExist(entity));
                }
                Dispatcher.Invoke(new Action(() => listAsset.DataContext = dt));
                Dispatcher.Invoke(new Action(() => gridRefreshing.Visibility = Visibility.Collapsed));
            }
            catch
            {
                Dispatcher.Invoke(new Action(() => {
                    gridNoIndex.Visibility = Visibility.Visible;
                    gridRefreshing.Visibility = Visibility.Collapsed;
                }));
            }
        }

        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
            Launch.Login.IAuth auth;
            if (string.IsNullOrWhiteSpace(MeCore.Config.DefaultAuth))
            {
                ACSelect ac = new ACSelect();
                ac.ShowDialog();
                auth = ac.auth;
            }
            else
            {
                SavedAuth dauth;
                MeCore.Config.SavedAuths.TryGetValue(MeCore.Config.DefaultAuth, out dauth);
                if (dauth == null)
                {
                    ACSelect ac = new ACSelect();
                    ac.ShowDialog();
                    auth = ac.auth;
                }
                else auth = dauth.AuthType.Equals("Yggdrasil") ? new Launch.Login.YggdrasilRefreshAuth(dauth.AccessToken) : new Launch.Login.AuthWarpper(new Launch.Login.AuthInfo { Pass = true, DisplayName = MeCore.Config.DefaultAuth, Session = dauth.AccessToken, UUID = dauth.UUID, UserType = dauth.UserType, Prop = dauth.Properies }) as Launch.Login.IAuth;
            }
            /*ACLogin ac = new ACLogin();
            ac.ShowDialog();
            auth = ac.auth;*/
            if (auth == null)
            {
                return;
            }
            MeCore.MainWindow._LaunchOptions = Launch.LaunchGameInfo.CreateInfo(MeCore.Config.MCPath, auth, mcversion, MeCore.Config.Javaw, (int)MeCore.Config.Javaxmx, CreateServerInfo());
            MeCore.MainWindow.launchFlyout.IsOpen = true;
            Back();
        }
        private Launch.ServerInfo CreateServerInfo()
        {
            if (MeCore.IsServerDedicated)
            {
                if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ServerIP))
                {
                    if (MeCore.Config.Server.ServerIP.IndexOf(':') != -1)
                    {
                        ushort port = 25565;
                        if (!ushort.TryParse(MeCore.Config.Server.ServerIP.Substring(MeCore.Config.Server.ServerIP.IndexOf(':')).Trim(':'), out port))
                        {
                            port = 25565;
                        }
                        return new Launch.ServerInfo
                        {
                            Ip = MeCore.Config.Server.ServerIP.Substring(0, MeCore.Config.Server.ServerIP.IndexOf(':')).Trim(':'),
                            Port = port
                        };
                    }
                }
            }
            return null;
        }

        private void butDLAssets_Click(object sender, RoutedEventArgs e)
        {
            TaskListBar task = new TaskListBar() { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/download-banner.jpg")) };
            var thGet = new Thread(new ThreadStart(delegate
            {
                DownloadAssets(task, GetAssetsIndexObject(task));
            }));
            MeCore.MainWindow.addTask("dl-assets", task.setThread(thGet).setTask(LangManager.GetLocalized("TaskDLAssets")));
            MeCore.MainWindow.addBalloonNotice(new Notice.NoticeBalloon("MTMCL", string.Format(LangManager.GetLocalized("BalloonNoticeSTTaskFormat"), LangManager.GetLocalized("TaskDLAssets"))));
        }

        private void DownloadAssets(TaskListBar task, Dictionary<string, AssetsEntity> dlList, bool force = false)
        {
            using (WebClient _downer = new WebClient())
            {
                int i = 0;
                foreach (KeyValuePair<string, AssetsEntity> entity in dlList)
                {
                    i++;
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        task.setTaskStatus((((float)i) / dlList.Count * 100).ToString() + "%");
                    }));
                    string url = MTMCL.Resources.UrlReplacer.getResourceUrl() + entity.Value.Hash.Substring(0, 2) + "/" + entity.Value.Hash;
                    string file = MeCore.Config.MCPath + @"\assets\objects\" + entity.Value.Hash.Substring(0, 2) + "\\" + entity.Value.Hash;
                    if (MeCore.IsServerDedicated)
                    {
                        if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                        {
                            file = file.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                        }
                    }
                    FileHelper.CreateDirectoryForFile(file);
                    try
                    {
                        if (FileHelper.IfFileVaild(file, entity.Value.Size))
                        {
                            if (force) {
                                task.log(Logger.HelpLog(string.Format("{0} exists, delete it", entity.Key)));
                                File.Delete(file);
                            }
                            else
                            {
                                task.log(Logger.HelpLog(string.Format("{0} exists, skip it", entity.Key)));
                                continue;
                            }
                        }
                        task.log(Logger.HelpLog(string.Format("Start downloading {0}", entity.Key)));
                        _downer.DownloadFile(new Uri(url), file);
                        task.log(Logger.HelpLog(string.Format("Finish downloading {0}, Progress {1}", entity.Key, i.ToString(CultureInfo.InvariantCulture) + "/" + dlList.Count.ToString(CultureInfo.InvariantCulture))));
                        if (i == dlList.Count)
                        {
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                task.log(Logger.HelpLog("Finish downloading assets"));
                            }));
                        }
                    }
                    catch (WebException ex)
                    {
                        Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                        {
                            Logger.log(ex.Response.ResponseUri.ToString());
                            Logger.error(ex);
                        }));
                    }
                }
            }
        }

        private Dictionary<string, AssetsEntity> GetAssetsIndexObject(TaskListBar task)
        {            
            VersionJson _version = null;
            do
            {
                Dispatcher.Invoke(new Action(() => _version = mcversion));
            } while (_version == null);
            string indexpath = MeCore.Config.MCPath + "\\assets\\indexes\\" + _version.assets + ".json";
            if (MeCore.IsServerDedicated && !string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                {
                    indexpath = indexpath.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                }
            if (!File.Exists(indexpath))
            {
                task?.log(Logger.HelpLog("Assets Index is missing, try downloading"));
                FileHelper.CreateDirectoryForFile(indexpath);
                string result = new WebClient().DownloadString(new Uri(MTMCL.Resources.UrlReplacer.getDownloadUrl() + "indexes/" + _version.assets + ".json"));
                StreamWriter sw = new StreamWriter(indexpath);
                sw.Write(result);
                sw.Close();
            }
            var sr = new StreamReader(indexpath);
            AssetIndex assets = JsonConvert.DeserializeObject<AssetIndex>(sr.ReadToEnd());
            sr.Dispose();
            return assets.objects;
        }

        private void butRDLAsset_Click(object sender, RoutedEventArgs e)
        {
            TaskListBar task = new TaskListBar() { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/download-banner.jpg")) };
            var thGet = new Thread(new ThreadStart(delegate
            {
                DownloadAssets(task, GetAssetsIndexObject(task),true);
            }));
            MeCore.MainWindow.addTask("dl-assets", task.setThread(thGet).setTask(LangManager.GetLocalized("TaskRDLAssets")));
            MeCore.MainWindow.addBalloonNotice(new Notice.NoticeBalloon("MTMCL", string.Format(LangManager.GetLocalized("BalloonNoticeSTTaskFormat"), LangManager.GetLocalized("TaskRDLAssets"))));
        }
        private async void butRDLAI_Click(object sender, RoutedEventArgs e)
        {
            await System.Threading.Tasks.Task.Factory.StartNew(RefreshAsset);
        }
        List<LibraryUniversal> libs = new List<LibraryUniversal>();
        private void grid_Initialized(object sender, EventArgs e)
        {


        }

        private async void grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mcversion.errored)
            {
                var dtlib = new DataTable();
                dtlib.Columns.Add("Lib");
                dtlib.Columns.Add("Exist");
                libs.Clear();
                foreach (var item in mcversion.libraries.ToUniversalLibrary())
                {
                    libs.Add(item);
                }
                foreach (LibraryUniversal libfile in libs)
                {
                    dtlib.Rows.Add(new object[] { libfile.name, FileHelper.IfFileVaild(libfile.path) });
                }
                listLib.DataContext = dtlib;
                await System.Threading.Tasks.Task.Factory.StartNew(RefreshAsset);
            }
        }

        private async void butDLInherit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(mcversion.inheritsFrom)) return;
            if (mcversion.inheritsFrom.Contains("forge"))
            {

            }
            else
            {
                if (!System.Text.RegularExpressions.Regex.Match(mcversion.inheritsFrom, @"\d+\.\d+\.\d*").Success) {
                    await MeCore.MainWindow.ShowMessageAsync(LangManager.GetLocalized("Oops"),string.Format(LangManager.GetLocalized("FormatFaultSolve"), mcversion.inheritsFrom));
                    return;
                }
                Grid dl = await MeCore.MainWindow.ChangePage("download");
                if(dl is Download)
                {
                    Download download = dl as Download;
                    await download.DirectDownloadMC(mcversion.inheritsFrom);
                }
            }
        }

        private async void butDLAssetsSelected_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AssetsSelectDialog() { AssetsObjects = GetAssetsIndexObject(null) };
            dialog.Height = 280;
            dialog.listAsset.ItemsSource = dialog.PackedAssetsObjects;
            dialog.OnClosing += async (sender1, e1) => {
                await MeCore.MainWindow.HideMetroDialogAsync(dialog);
                if (e1.IsOk) {
                    TaskListBar task = new TaskListBar() { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/download-banner.jpg")) };
                    var thGet = new Thread(new ThreadStart(delegate
                    {
                        DownloadAssets(task, e1.SelectedEntities.ToDictionary(pair=> pair.Key, pair=> (Assets.AssetsEntity)pair.Value));
                    }));
                    MeCore.MainWindow.addTask("dl-assets", task.setThread(thGet).setTask(LangManager.GetLocalized("TaskDLAssets")));
                    MeCore.MainWindow.addBalloonNotice(new Notice.NoticeBalloon("MTMCL", string.Format(LangManager.GetLocalized("BalloonNoticeSTTaskFormat"), LangManager.GetLocalized("TaskDLAssets"))));
                }
            };
            await MeCore.MainWindow.ShowMetroDialogAsync(dialog);
        }
    }
}
