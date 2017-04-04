using System;
using System.Collections.Generic;
using System.Windows;

namespace MTMCL.Lang
{
    static class LangManager
    {
        private static readonly Dictionary<string, LangType> Languages = new Dictionary<string, LangType>();
        private static readonly Dictionary<string, string> DisplayToName = new Dictionary<string, string>();
        static private readonly ResourceDictionary DefaultLanguage = LoadLangFromResource("pack://application:,,,/Lang/en.xaml");
        //static private readonly ResourceDictionary DefaultLanguageInJson = LoadLangFromResource("pack://application:,,,/Lang/en.json");
        static public void Add(string languageName,string languageUrl)
        {
            if (Languages.ContainsKey(languageName))
            {
                Languages[languageName] = new LangType(languageName,languageUrl);
                return;
            }
            Languages.Add(languageName, new LangType(languageName, languageUrl));

        }
        static public void Clear() {
            Languages.Clear();
        }
        static public string GetLocalized(string key)
        {
            if (Application.Current.Resources.Contains(key))
                return Application.Current.Resources[key] as string;
            if (DefaultLanguage.Contains(key))
                return DefaultLanguage[key] as string;
            return key;
        }
        static public void SetLocalizedContent(this FrameworkElement element, string key) {
            SetLocalizedContent(element, key, System.Windows.Controls.ContentControl.ContentProperty);
        }

        static public void SetLocalizedContent(this FrameworkElement element, string key, DependencyProperty property)
        {
            if (Application.Current.Resources.Contains(key))
                element.SetResourceReference(property, key);
            else element.SetValue(property, key);
        }

        static public ResourceDictionary LoadLangFromResource(string path)
        {
            var lang = new ResourceDictionary();
            lang.Source = new Uri(path);
            return lang;
        }
        static public void UseLanguage(string languageName)
        {
            if (!Languages.ContainsKey(languageName))
            {
                ReplaceDictationaries();
                Application.Current.Resources.MergedDictionaries.Add(DefaultLanguage);
                return;
            }
            var langType = Languages[languageName];
            if (langType != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(Languages[MeCore.Config.Lang].Language);
                Application.Current.Resources.MergedDictionaries.Add(langType.Language);
                //ReplaceDictationaries();
            }
        }

        public static string[] ListLanuage()
        {
            var langs = new string[Languages.Count];
            var i = 0;
            foreach (var lang in Languages)
            {
                langs[i] = (string)lang.Value.Language["DisplayName"];
                i++;
            }
            return langs;
        }
        private static void ReplaceDictationaries()
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml") });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml") });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml") });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Green.xaml") });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml") });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Resources/Icons.xaml") });
        }
    }
}
