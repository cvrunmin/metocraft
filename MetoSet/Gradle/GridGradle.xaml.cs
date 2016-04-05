using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.Windows.Controls;

namespace MTMCL.Gradle
{
    /// <summary>
    /// GridGradle.xaml 的互動邏輯
    /// </summary>
    public partial class GridGradle
    {
        public GridGradle()
        {
            InitializeComponent();
        }
        private void butBack_Click(object sender, RoutedEventArgs e)
        {
            MeCore.MainWindow.gridMain.Visibility = Visibility.Visible;
            MeCore.MainWindow.gridOthers.Visibility = Visibility.Collapsed;
            var ani = new DoubleAnimationUsingKeyFrames();
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            ani.KeyFrames.Add(new LinearDoubleKeyFrame(1, TimeSpan.FromSeconds(0.2)));
            MeCore.MainWindow.gridMain.BeginAnimation(OpacityProperty, ani);
        }

        private void butBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "gradlew|gradlew|gradlew.bat|gradlew.bat";
            dialog.ShowDialog();
            if (string.IsNullOrWhiteSpace(dialog.FileName))
            {
                return;
            }
            txtPath.Text = dialog.FileName;
        }

        private async void butOther_Click(object sender, RoutedEventArgs e)
        {
            string arg = await MeCore.MainWindow.ShowInputAsync("Input your arguments", "The arguments will be executed after.");
            if (!string.IsNullOrWhiteSpace(arg))
            {
                executeCommand(arg);
            }
        }
        private void executeCommand(string cmd) {
            if (string.IsNullOrWhiteSpace(txtPath.Text))
            {
                return;
            }
            cmd = cmd.Replace("gradlew.bat", "").Replace("gradlew", "");
            //disableCMD();
            listOutput.Items.Clear();
            Process pro = new Process();
            pro.StartInfo.FileName = "cmd.exe";
            pro.StartInfo.WorkingDirectory = System.IO.Directory.GetParent(txtPath.Text).FullName;
            pro.StartInfo.Arguments = "/C " + System.IO.Path.GetFileName(txtPath.Text) + " " + cmd;
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.CreateNoWindow = true;
            pro.StartInfo.RedirectStandardOutput = true;
            pro.StartInfo.RedirectStandardError = true;
            pro.Start();
            pro.BeginOutputReadLine();
            pro.OutputDataReceived += (sender, e) => {
                if (e.Data != null)
                {
                    Dispatcher.Invoke(()=> listOutput.Items.Add(e.Data));
                }
            };
            pro.BeginErrorReadLine();
            pro.ErrorDataReceived += (sender, e) => {
                if (e.Data != null)
                {
                    Dispatcher.Invoke(() => listOutput.Items.Add(e.Data));
                }
            };
            System.Threading.Tasks.Task.Factory.StartNew(pro.WaitForExit).ContinueWith(e => enableCMD());
        }
        private void enableCMD()
        {
            butCI.IsEnabled =
                butDecomp.IsEnabled =
                butDev.IsEnabled =
                butEclipse.IsEnabled =
                butGui.IsEnabled =
                butIdea.IsEnabled =
                butOther.IsEnabled =
                butrC.IsEnabled =
                butrS.IsEnabled =
                butsetDcEc.IsEnabled =
                butsetDcIe.IsEnabled =
                true;
        }
        private void disableCMD()
        {
            butCI.IsEnabled =
                butDecomp.IsEnabled =
                butDev.IsEnabled =
                butEclipse.IsEnabled =
                butGui.IsEnabled =
                butIdea.IsEnabled =
                butOther.IsEnabled =
                butrC.IsEnabled =
                butrS.IsEnabled =
                butsetDcEc.IsEnabled =
                butsetDcIe.IsEnabled =
                false;
        }

        private void butNormalCMD_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                executeCommand(((Button)sender).Content as string);
            }
        }
    }
}
