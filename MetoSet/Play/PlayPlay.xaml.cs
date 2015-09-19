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

namespace MetoSet.Play
{
    /// <summary>
    /// PlayPlay.xaml 的互動邏輯
    /// </summary>
    public partial class PlayPlay : Grid
    {
        public PlayPlay()
        {
            InitializeComponent();
        }

        private void butPlay_Click(object sender, RoutedEventArgs e)
        {
            ACLogin login = new ACLogin();
            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
            {
                login.ShowDialog();
            }));
            if (login.auth != null) {
                var result = PlayMain.launcher.Launch(new KMCCC.Launcher.LaunchOptions {
                    Version = MeCore.MainWindow.gridPlay.gridVer.versions[MeCore.MainWindow.gridPlay.gridVer.listBoxVer.SelectedIndex],
                    MaxMemory = (int)MeCore.MainWindow.gridPlay.gridEn.sliderRAM.Value,
                    Authenticator = login.auth
                });
                if (!result.Success) {
                    switch (result.ErrorType) {
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
            }
            
        }
    }
}
