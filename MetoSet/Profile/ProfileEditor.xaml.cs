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
using System.Windows.Shapes;

namespace MetoCraft.Profile
{
    /// <summary>
    /// ProfileEditor.xaml 的互動邏輯
    /// </summary>
    public partial class ProfileEditor : Window
    {
        private Profile[] profiles;
        private string _path;
        private ProfileInXML xmlLoader;
        public ProfileEditor()
        {
            InitializeComponent();
        }

        public ProfileEditor(string path)
        {
            InitializeComponent();
            loadVersion();
            _path = path;
            loadProfile(_path);
        }
        private void loadVersion()
        {
            try
            {
                foreach (var items in MeCore.MainWindow.gridPlay.versions)
                {
                    combeVer.Items.Add(items.Id);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    new ErrorReport(ex).Show();
                }));
            }
        }
        private void loadProfile(string path) {
            int tmpdex = listFile.SelectedIndex;
            listFile.SelectedIndex = -1;
            listFile.Items.Clear();
            xmlLoader = new ProfileInXML();
            profiles = xmlLoader.readProfile(path);
            if (profiles != null) {
                for (int i = 0; i < profiles.Length; i++)
                {
                    listFile.Items.Add(profiles[i].name);
                }
            }
            listFile.SelectedIndex = tmpdex;
        }

        private void listFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listFile.SelectedIndex != -1) {
                try
                {
                    inText(profiles[listFile.SelectedIndex]);
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        new ErrorReport(ex).Show();
                    }));
                }
            }
        }
        private void inText(Profile file) {
            txtBoxName.Text = file.name;
            combeVer.SelectedItem = file.version;
            txtBoxX.Text = file.winSizeX.ToString();
            txtBoxY.Text = file.winSizeY.ToString();
            txtBoxXmx.Text = file.Xmx.ToString();
        }
        private Profile outText() {
            if (listFile.SelectedIndex != -1) {
                int x = 0, y = 0;
                Profile file = new Profile
                {
                    oldName = profiles[listFile.SelectedIndex].name,
                    name = txtBoxName.Text,
                    version = combeVer.SelectedItem.ToString(),
                    winSizeX = int.TryParse(txtBoxX.Text, out x) ? x : 854,
                    winSizeY = int.TryParse(txtBoxY.Text, out y) ? y : 480,
                    Xmx = int.Parse(txtBoxXmx.Text)
                };
                //            profiles[listFile.SelectedIndex] = file;
                return file;
            }
            return null;
        }
        private Profile outNewText()
        {
            int x = 0, y = 0;
            Profile file = new Profile
            {
                oldName = "",
                name = txtBoxName.Text,
                version = combeVer.SelectedItem.ToString(),
                winSizeX = int.TryParse(txtBoxX.Text, out x) ? x : 854,
                winSizeY = int.TryParse(txtBoxY.Text, out y) ? y : 480,
                Xmx = int.Parse(txtBoxXmx.Text)
            };
            return file;
        }
        private void butSave_Click(object sender, RoutedEventArgs e)
        {
            Profile txt;
            if ((txt = outText()) != null) {
                xmlLoader.rewriteProfile(_path, txt);
                loadProfile(_path);
            }
        }

        private void butNew_Click(object sender, RoutedEventArgs e)
        {
            xmlLoader.writeProfile(_path, outNewText());
            loadProfile(_path);
        }

        private void butDel_Click(object sender, RoutedEventArgs e)
        {
            xmlLoader.deleteProfile(_path, outText());
            loadProfile(_path);
        }
    }
}
