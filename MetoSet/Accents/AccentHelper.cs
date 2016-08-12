using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MTMCL.Accents
{
    public class AccentHelper
    {
        public static void CreateAccent(string name, Color highlight, Color accent) {
            using (var sw = new System.IO.StreamWriter(System.IO.File.Create(System.IO.Path.Combine(MeCore.DataDirectory, "Color", name + ".xaml")))) {
                string s = System.Windows.Markup.XamlWriter.Save(createResourceDictionary(highlight, accent));
                System.Threading.Tasks.Task.WaitAll(sw.WriteAsync(s));
            }
        }
        #region Create ResourceDictionary
        /// <summary>
        /// From:
        /// https://github.com/punker76/code-samples/blob/master/MahAppsMetroThemesSample/MahAppsMetroThemesSample/ThemeManagerHelper.cs
        /// </summary>
        /// <param name="highlight"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static ResourceDictionary createResourceDictionary(Color highlight, Color color) {
            var resourceDictionary = new ResourceDictionary();

            resourceDictionary.Add("HighlightColor", highlight);
            resourceDictionary.Add("AccentBaseColor", color);
            resourceDictionary.Add("AccentColor", Color.FromArgb(0xCC, color.R, color.G, color.B));
            resourceDictionary.Add("AccentColor2", Color.FromArgb(0x99, color.R, color.G, color.B));
            resourceDictionary.Add("AccentColor3", Color.FromArgb(0x66, color.R, color.G, color.B));
            resourceDictionary.Add("AccentColor4", Color.FromArgb(0x33, color.R, color.G, color.B));

            resourceDictionary.Add("HighlightBrush", GetSolidColorBrush((Color)resourceDictionary["HighlightColor"]));
            resourceDictionary.Add("AccentBaseColorBrush", GetSolidColorBrush((Color)resourceDictionary["AccentBaseColor"]));
            resourceDictionary.Add("AccentColorBrush", GetSolidColorBrush((Color)resourceDictionary["AccentColor"]));
            resourceDictionary.Add("AccentColorBrush2", GetSolidColorBrush((Color)resourceDictionary["AccentColor2"]));
            resourceDictionary.Add("AccentColorBrush3", GetSolidColorBrush((Color)resourceDictionary["AccentColor3"]));
            resourceDictionary.Add("AccentColorBrush4", GetSolidColorBrush((Color)resourceDictionary["AccentColor4"]));
            resourceDictionary.Add("WindowTitleColorBrush", GetSolidColorBrush((Color)resourceDictionary["AccentColor"]));

            resourceDictionary.Add("ProgressBrush", new LinearGradientBrush(
                new GradientStopCollection(new[]
                {
                    new GradientStop((Color)resourceDictionary["HighlightColor"], 0),
                    new GradientStop((Color)resourceDictionary["AccentColor3"], 1)
                }),
                new Point(0.001, 0.5), new Point(1.002, 0.5)));

            resourceDictionary.Add("CheckmarkFill", GetSolidColorBrush((Color)resourceDictionary["AccentColor"]));
            resourceDictionary.Add("RightArrowFill", GetSolidColorBrush((Color)resourceDictionary["AccentColor"]));

            resourceDictionary.Add("IdealForegroundColor", IdealTextColor(color));
            resourceDictionary.Add("IdealForegroundColorBrush", GetSolidColorBrush((Color)resourceDictionary["IdealForegroundColor"]));
            resourceDictionary.Add("IdealForegroundDisabledBrush", GetSolidColorBrush((Color)resourceDictionary["IdealForegroundColor"], 0.4));
            resourceDictionary.Add("AccentSelectedColorBrush", GetSolidColorBrush((Color)resourceDictionary["IdealForegroundColor"]));

            resourceDictionary.Add("MetroDataGrid.HighlightBrush", GetSolidColorBrush((Color)resourceDictionary["AccentColor"]));
            resourceDictionary.Add("MetroDataGrid.HighlightTextBrush", GetSolidColorBrush((Color)resourceDictionary["IdealForegroundColor"]));
            resourceDictionary.Add("MetroDataGrid.MouseOverHighlightBrush", GetSolidColorBrush((Color)resourceDictionary["AccentColor3"]));
            resourceDictionary.Add("MetroDataGrid.FocusBorderBrush", GetSolidColorBrush((Color)resourceDictionary["AccentColor"]));
            resourceDictionary.Add("MetroDataGrid.InactiveSelectionHighlightBrush", GetSolidColorBrush((Color)resourceDictionary["AccentColor2"]));
            resourceDictionary.Add("MetroDataGrid.InactiveSelectionHighlightTextBrush", GetSolidColorBrush((Color)resourceDictionary["IdealForegroundColor"]));

            resourceDictionary.Add("MahApps.Metro.Brushes.ToggleSwitchButton.OnSwitchBrush.Win10", GetSolidColorBrush((Color)resourceDictionary["AccentColor"]));
            resourceDictionary.Add("MahApps.Metro.Brushes.ToggleSwitchButton.OnSwitchMouseOverBrush.Win10", GetSolidColorBrush((Color)resourceDictionary["AccentColor2"]));
            resourceDictionary.Add("MahApps.Metro.Brushes.ToggleSwitchButton.ThumbIndicatorCheckedBrush.Win10", GetSolidColorBrush((Color)resourceDictionary["IdealForegroundColor"]));
            return resourceDictionary;
        }
        /// <summary>
        /// Determining Ideal Text Color Based on Specified Background Color
        /// http://www.codeproject.com/KB/GDI-plus/IdealTextColor.aspx
        /// </summary>
        /// <param name = "color">The bg.</param>
        /// <returns></returns>
        private static Color IdealTextColor(Color color)
        {
            const int nThreshold = 105;
            var bgDelta = System.Convert.ToInt32((color.R * 0.299) + (color.G * 0.587) + (color.B * 0.114));
            var foreColor = (255 - bgDelta < nThreshold) ? Colors.Black : Colors.White;
            return foreColor;
        }
        private static SolidColorBrush GetSolidColorBrush(Color color, double opacity = 1d)
        {
            var brush = new SolidColorBrush(color) { Opacity = opacity };
            brush.Freeze();
            return brush;
        }
        #endregion
    }
}
