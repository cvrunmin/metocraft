using MetoCraft.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetoCraft.Play
{
    /// <summary>
    /// PlayAdv.xaml 的互動邏輯
    /// </summary>
    public partial class PlayAdv : Grid
    {
        private Profile.Profile[] profiles;
        private string _path = Environment.CurrentDirectory + "\\profiles.xml";
        private ProfileInXML xmlLoader;
        public PlayAdv()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
/*            MeCore.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
            {*/
                new Profile.ProfileEditor(Environment.CurrentDirectory + "\\profiles.xml").Show();
//            }));
        }

        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
            ACLogin login = new ACLogin();
            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
            {
                login.ShowDialog();
            }));
            if (login.auth != null)
            {
                var result = PlayMain.launcher.Launch(new KMCCC.Launcher.LaunchOptions
                {
                    Version = PlayMain.launcher.GetVersion(profiles[listFile.SelectedIndex].version),
                    MaxMemory = profiles[listFile.SelectedIndex].Xmx,
                    Size = new KMCCC.Launcher.WindowSize {
                        Width = (ushort?)profiles[listFile.SelectedIndex].winSizeX,
                        Height = (ushort?)profiles[listFile.SelectedIndex].winSizeY
                    },
                    Authenticator = login.auth
                });
                if (!result.Success)
                {
                    switch (result.ErrorType)
                    {
                        case KMCCC.Launcher.ErrorType.NoJAVA:
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                new ErrorReport("Your Java in OS is abnormal, please try to reinstall Java", result.ErrorMessage).Show();
                            }));
                            break;
                        case KMCCC.Launcher.ErrorType.AuthenticationFailed:
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                new ErrorReport("Cannot login. Please check your email and password are correct or not", result.ErrorMessage).Show();
                            }));
                            break;
                        case KMCCC.Launcher.ErrorType.OperatorException:
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                new ErrorReport("Operator Exception.", result.ErrorMessage).Show();
                            }));
                            break;
                        case KMCCC.Launcher.ErrorType.UncompressingFailed:
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                new ErrorReport("Uncompressing failed. Please check your libraries are intect or not", result.ErrorMessage).Show();
                            }));
                            break;
                        case KMCCC.Launcher.ErrorType.Unknown:
                            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                            {
                                new ErrorReport("Unknown Exception", result.ErrorMessage).Show();
                            }));
                            break;
                    }
                }
                else {
                    MeCore.NIcon.ShowBalloonTip(3000, "Susceesful to launch" + profiles[listFile.SelectedIndex].version);
                }
            }
        }

        private void butReload_Click(object sender, RoutedEventArgs e)
        {
            loadProfile(_path);
        }

        private void loadProfile(string path)
        {
            listFile.SelectedIndex = -1;
            listFile.Items.Clear();
            xmlLoader = new ProfileInXML();
            profiles = xmlLoader.readProfile(path);
            if (profiles != null)
            {
                for (int i = 0; i < profiles.Length; i++)
                {
                    listFile.Items.Add(profiles[i].name);
                }
            }
        }
    }
}
