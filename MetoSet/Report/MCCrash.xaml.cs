using MTMCL.util;
using System.Windows;

namespace MTMCL
{
    /// <summary>
    /// MCCrash.xaml 的互動邏輯
    /// </summary>
    public partial class MCCrash : Window
    {
        public string path { get; private set; }
        public MCCrash()
        {
            InitializeComponent();
            txtblktitle.AddContentFromSpecficString(Lang.LangManager.GetLangFromResource("MCCrashTitle"));
        }
        public MCCrash(string content) {
            InitializeComponent();
            txtContent.Text = content;
            if ((!content.Contains("Is Modded: Probably not")) && (!content.Contains("Is Modded: Unknown")))
            {
                txtblktitle.AddContentFromSpecficString(Lang.LangManager.GetLangFromResource("MCCrashTitleModded"));
            }
            else
            {
                txtblktitle.AddContentFromSpecficString(Lang.LangManager.GetLangFromResource("MCCrashTitle"));
            }
        }
        public MCCrash(string content, string path) : this(content) {
            this.path = path;
            butOpen.IsEnabled = true;
        }
        private void butOpen_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(path);
        }
    }
}
