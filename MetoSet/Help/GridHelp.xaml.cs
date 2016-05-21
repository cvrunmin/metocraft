using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTMCL.Help
{
    /// <summary>
    /// GridHelp.xaml 的互動邏輯
    /// </summary>
    public partial class GridHelp
    {
        public GridHelp()
        {
            InitializeComponent();
        }
        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }
        private void Back()
        {
            MeCore.MainWindow.gridMain.Visibility = Visibility.Visible;
            MeCore.MainWindow.gridOthers.Visibility = Visibility.Collapsed;
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridMain.BeginAnimation(OpacityProperty, ani);
        }

        private void comboLang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboLang.SelectedIndex == -1) return;
            try
            {
                frame.Source = new Uri(string.Format("http://cvronmin.github.io/metocraft/{0}/main.html", comboLang.SelectedItem as string));
            }
            catch (WebException ex) {
                var ru = ex.Response.ResponseUri;
                string s = ((HttpWebResponse)ex.Response).StatusCode.GetTypeCode() + ((HttpWebResponse)ex.Response).StatusCode.ToString();
                /*switch (((HttpWebResponse)ex.Response).StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        s = "404 Not Found";
                        break;
                    case HttpStatusCode.Forbidden:
                        s = "403 Forbidden";
                        break;
                    case HttpStatusCode.BadGateway:

                        break;
                    case HttpStatusCode.GatewayTimeout:
                        break;
                    default:
                        break;
                }*/
                var a = new PageHttpError();
                a.OnRetry += delegate {
                    frame.Source = ru;
                };
                frame.Navigate(a.SetStatus(s));
            }
                //frame.Source = new Uri(string.Format("pack://siteoforigin:,,,/Help/{0}/main.html", comboLang.SelectedItem as string));
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            comboLang.Items.Add("en");
            comboLang.Items.Add("cht");
            comboLang.Items.Add("chs");
        }

        private void frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            if (e.Exception is WebException) {
                WebException ex = e.Exception as WebException;
                var ru = ex.Response.ResponseUri;
                string s = ((int)((HttpWebResponse)ex.Response).StatusCode) + " " + ((HttpWebResponse)ex.Response).StatusCode.ToString();
                var a = new PageHttpError();
                a.OnRetry += delegate {
                    frame.Source = ru;
                };
                frame.Navigate(a.SetStatus(s));
            }
        }
    }
}
