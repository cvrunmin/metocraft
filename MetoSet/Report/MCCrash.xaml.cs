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
using MetoCraft.util;

namespace MetoCraft
{
    /// <summary>
    /// MCCrash.xaml 的互動邏輯
    /// </summary>
    public partial class MCCrash : Window
    {
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
    }
}
