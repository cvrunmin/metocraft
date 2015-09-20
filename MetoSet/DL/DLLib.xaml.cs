using MetoCraft.Play;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetoCraft.DL
{
    /// <summary>
    /// DLLib.xaml 的互動邏輯
    /// </summary>
    public partial class DLLib : Grid
    {
        IEnumerable<string> libs;
        IEnumerable<string> natives;
        private readonly WebClient _downer = new WebClient();
        private readonly string _urlLib = MeCore.UrlLibrariesBase;
        public DLLib()
        {
            InitializeComponent();
        }

        private void listVer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listLib.DataContext = null;
                try
                {
                    var dt = new DataTable();
                    dt.Columns.Add("Lib");
                    dt.Columns.Add("Exist");
                libs = MeCore.MainWindow.gridPlay.gridVer.versions[listVer.SelectedIndex].Libraries.Select(lib => KMCCC.Launcher.LauncherCoreItemResolverExtensions.GetLibPath(PlayMain.launcher, lib));
                    foreach (string libfile in libs)
                    {
                        dt.Rows.Add(new object[] { libfile.Substring(libfile.IndexOf("libraries")), File.Exists(libfile) });
                    }
                natives = MeCore.MainWindow.gridPlay.gridVer.versions[listVer.SelectedIndex].Natives.Select(native => KMCCC.Launcher.LauncherCoreItemResolverExtensions.GetNativePath(PlayMain.launcher, native));
                foreach (string nafile in natives)
                {
                    dt.Rows.Add(new object[] { nafile.Substring(nafile.IndexOf("libraries")), File.Exists(nafile) });
                }
                Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        listLib.DataContext = dt;
                        listLib.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Exist", System.ComponentModel.ListSortDirection.Ascending));
                    }));
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        new ErrorReport(ex).Show();
                    }));
                }
        }

        private void butDL_Click(object sender, RoutedEventArgs e)
        {
            foreach (string libfile in libs)
            {
                if (!File.Exists(libfile))
                {
                    Logger.log("开始下载" + libfile, Logger.LogType.Info);
                    try
                    {
                        //                    OnStateChangeEvent(LangManager.GetLangFromResource("LauncherDownloadLib") + lib.name);
                        //                    Downloading++;
                        if (!Directory.Exists(System.IO.Path.GetDirectoryName(libfile)))
                        {
                            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(libfile));
                        }
#if DEBUG
                        //                        System.Windows.MessageBox.Show(_urlLib + libp.Remove(0, Environment.CurrentDirectory.Length + 22).Replace("\\", "/"));
#endif
                        Logger.log(_urlLib + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/"));
                        //                Logger.log(_urlLib + libfile.Remove(0, Environment.CurrentDirectory.Length + 22).Replace("\\", "/"));
                        _downer.DownloadFile(
                            _urlLib +
                            libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
                    }
                    catch (WebException ex)
                    {
                        Logger.log(ex);
                        Logger.log("原地址下载失败，尝试BMCL源" + libfile);
                        try
                        {
                            _downer.DownloadFile(MetoCraft.Resources.Url.URL_DOWNLOAD_bangbang93 + "libraries/" + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
                        }
                        catch (WebException exception)
                        {
                            MeCore.Invoke(new Action(() => new ErrorReport(exception).Show()));
                            return;
                        }
                    }
                }
            }
            foreach (string libfile in natives)
            {
                if (!File.Exists(libfile))
                {
                    Logger.log("开始下载" + libfile, Logger.LogType.Info);
                    try
                    {
                        //                    OnStateChangeEvent(LangManager.GetLangFromResource("LauncherDownloadLib") + lib.name);
                        //                    Downloading++;
                        if (!Directory.Exists(System.IO.Path.GetDirectoryName(libfile)))
                        {
                            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(libfile));
                        }
#if DEBUG
                        //                        System.Windows.MessageBox.Show(_urlLib + libp.Remove(0, Environment.CurrentDirectory.Length + 22).Replace("\\", "/"));
#endif
                        Logger.log(_urlLib + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("\\", "/"));
                        //                Logger.log(_urlLib + libfile.Remove(0, Environment.CurrentDirectory.Length + 22).Replace("\\", "/"));
                        _downer.DownloadFile(
                            _urlLib +
                            libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
                    }
                    catch (WebException ex)
                    {
                        Logger.log(ex);
                        Logger.log("原地址下载失败，尝试BMCL源" + libfile);
                        try
                        {
                            _downer.DownloadFile(MetoCraft.Resources.Url.URL_DOWNLOAD_bangbang93 + "libraries/" + libfile.Remove(0, MeCore.Config.MCPath.Length + 11).Replace("/", "\\"), libfile);
                        }
                        catch (WebException exception)
                        {
                            MeCore.Invoke(new Action(() => new ErrorReport(exception).Show()));
                            return;
                        }
                    }
                }
            }
        }
    }
}
