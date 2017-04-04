using MTMCL.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTMCL.Guide
{
    /// <summary>
    /// PageGuideLang.xaml 的互動邏輯
    /// </summary>
    public partial class PageGuideLang
    {
        public PageGuideLang()
        {
            InitializeComponent();
        }

        private void butUse_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Guide\\PageGuideFast.xaml", UriKind.Relative));
            //((Grid)Parent).Children.Clear();
            //((Grid)Parent).Children.Add(new PageGuideFast());
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            RefreshLangList();
        }
        public void RefreshLangList()
        {
            comboLang.Items.Clear();
            var langs = LangManager.ListLanuage();
            foreach (var lang in langs)
            {
                comboLang.Items.Add(lang);
            }
            comboLang.SelectedItem = LangManager.GetLocalized("DisplayName");
        }

        private void comboLang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboLang.SelectedItem as string != null)
                if (MeCore.Language.ContainsKey(comboLang.SelectedItem as string))
                {
                    LangManager.UseLanguage(MeCore.Language[comboLang.SelectedItem as string] as string);
                    MeCore.Config.QuickChange("Lang", LangManager.GetLocalized("LangName"));
                }
        }
    }
}
