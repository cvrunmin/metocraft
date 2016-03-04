using KMCCC.Launcher;
using MTMCL.Assets;
using MTMCL.util;
using MTMCL.Versions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MTMCL
{
    public partial class Play : Grid
    {
        public KMCCC.Launcher.Version[] versions;
        public Play() {
            InitializeComponent();
        }

        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }
        private void Back() {
            MeCore.MainWindow.gridMain.Visibility = Visibility.Visible;
            MeCore.MainWindow.gridOthers.Visibility = Visibility.Collapsed;
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridMain.BeginAnimation(OpacityProperty, ani);
        }

        private void Grid_Initialized(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MeCore.Config.MCPath))
            {
                App.core = LauncherCore.Create(new LauncherCoreCreationOption(MeCore.Config.MCPath, MeCore.Config.Javaw));
                versions = App.core.GetVersions().ToArray();
                var dt = new DataTable();
                dt.Columns.Add("Version");
                dt.Columns.Add("Type");
                foreach (var version in versions)
                {
                    dt.Rows.Add(new object[] { version.Id, version.Type });
                }
                listVer.DataContext = dt;
            }
        }
        List<LibraryUniversal> libs = new List<Versions.LibraryUniversal>();
        private void listVer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.core != null)
            {
                var version = versions[listVer.SelectedIndex];
                var dtlib = new DataTable();
                dtlib.Columns.Add("Lib");
                dtlib.Columns.Add("Exist");
                libs.Clear();
                VersionJson lib = LitJson.JsonMapper.ToObject<VersionJson>(new LitJson.JsonReader(new StreamReader(App.core.GetVersionJsonPath(version))));
                foreach (var item in lib.libraries.ToUniversalLibrary())
                {
                    libs.Add(item);
                }
                foreach (LibraryUniversal libfile in libs)
                {
                    dtlib.Rows.Add(new object[] { libfile.name, FileHelper.IfFileVaild(libfile.path) });
                }
                listLib.DataContext = dtlib;
                new Thread(new ThreadStart(delegate
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate {
                        var _version = versions[listVer.SelectedIndex];
                        string indexpath = MeCore.Config.MCPath + "\\assets\\indexes\\" + _version.Assets + ".json";
                        if (!File.Exists(indexpath))
                        {
                            FileHelper.CreateDirectoryForFile(indexpath);
                            string result = new WebClient().DownloadString(new Uri(MTMCL.Resources.UrlReplacer.getDownloadUrl() + "indexes/" + _version.Assets + ".json"));
                            StreamWriter sw = new StreamWriter(indexpath);
                            LitJson.JsonWriter jw = new LitJson.JsonWriter(sw);
                            jw.PrettyPrint = true;
                            LitJson.JsonMapper.ToJson(LitJson.JsonMapper.ToObject<Assets.AssetIndex>(result), jw);
                            sw.Close();
                        }
                        var sr = new StreamReader(indexpath);
                        AssetIndex assets = LitJson.JsonMapper.ToObject<Assets.AssetIndex>(sr);
                        Logger.log(assets.objects.Count.ToString(CultureInfo.InvariantCulture), " assets in total");
                        try
                        {
                            var dt = new DataTable();
                            dt.Columns.Add("Assets");
                            dt.Columns.Add("Exist");
                            foreach (KeyValuePair<string, AssetsEntity> entity in assets.objects)
                            {
                                dt.Rows.Add(new object[] { entity.Key, assetExist(entity).ToString() });
                            }
                            listAsset.DataContext = dt;
                        }
                        catch
                        {

                        }
                    }));

                })).Start();
                lblSelectVer.Content = version.Id;
            }
        }
        private bool assetExist(KeyValuePair<string, AssetsEntity> entity)
        {
            return File.Exists(MeCore.Config.MCPath + @"\assets\objects\" + entity.Value.hash.Substring(0, 2) + @"\" + entity.Value.hash);
        }
        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
            var login = new ACLogin();
            login.ShowDialog();
            if (login.auth == null)
            {
                return;
            }
            MeCore.MainWindow._LaunchOptions = new LaunchOptions {
                Authenticator = login.auth, MaxMemory = (int)MeCore.Config.Javaxmx, Version = versions[listVer.SelectedIndex]
            };
            MeCore.MainWindow.launchFlyout.IsOpen = true;
            Back();
        }
    }
}
