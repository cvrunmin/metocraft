using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMCL.Accents
{
    public class AccentHelper
    {
        public const string sample = 
            "<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n"+
            "        xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n"+
            "        xmlns:options=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation/options\"\n"+
            "        xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"\n"+
            "        mc:Ignorable=\"options\">\n"+
            "\n"+
            "    <!--ACCENT COLORS-->\n"+
            "    <Color x:Key=\"HighlightColor\">#FF{Highlight}</Color>\n"+
            "\n"+
            "    <Color x:Key=\"AccentBaseColor\">#FF{Accent}</Color>\n"+
            "\n"+
            "    <!--80%-->\n"+
            "    <Color x:Key=\"AccentColor\">#CC{Accent}</Color>\n"+
            "    <!--60%-->\n"+
            "    <Color x:Key=\"AccentColor2\">#99{Accent}</Color>\n"+
            "    <!--40%-->\n"+
            "    <Color x:Key=\"AccentColor3\">#66{Accent}</Color>\n"+
            "    <!--20%-->\n"+
            "    <Color x:Key=\"AccentColor4\">#33{Accent}</Color>\n"+
            "\n"+
            "    <!-- re-set brushes too -->"+
            "    <SolidColorBrush x:Key=\"HighlightBrush\" Color=\"{StaticResource HighlightColor}\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"AccentBaseColorBrush\" Color=\"{StaticResource AccentBaseColor}\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"AccentColorBrush\" Color=\"{StaticResource AccentColor}\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"AccentColorBrush2\" Color=\"{StaticResource AccentColor2}\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"AccentColorBrush3\" Color=\"{StaticResource AccentColor3}\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"AccentColorBrush4\" Color=\"{StaticResource AccentColor4}\" options:Freeze=\"True\" />\n"+
            "\n"+
            "    <SolidColorBrush x:Key=\"WindowTitleColorBrush\" Color=\"{StaticResource AccentColor}\" options:Freeze=\"True\" />\n"+
            "\n"+
            "    <LinearGradientBrush x:Key=\"ProgressBrush\" EndPoint=\"0.001,0.5\" StartPoint=\"1.002,0.5\" options:Freeze=\"True\">\n"+
            "        <GradientStop Color = \"{StaticResource HighlightColor}\" Offset=\"0\" />\n"+
            "        <GradientStop Color = \"{StaticResource AccentColor3}\" Offset=\"1\" />\n"+
            "    </LinearGradientBrush>\n"+
            "\n"+
            "    <SolidColorBrush x:Key=\"CheckmarkFill\" Color=\"{StaticResource AccentColor}\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"RightArrowFill\" Color=\"{StaticResource AccentColor}\" options:Freeze=\"True\" />\n"+
            "\n"+
            "    <Color x:Key=\"IdealForegroundColor\">White</Color>\n"+
            "    <SolidColorBrush x:Key=\"IdealForegroundColorBrush\" Color=\"{StaticResource IdealForegroundColor}\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"IdealForegroundDisabledBrush\" Color=\"{StaticResource IdealForegroundColor}\" Opacity=\"0.4\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"AccentSelectedColorBrush\" Color=\"{StaticResource IdealForegroundColor}\" options:Freeze=\"True\" />\n"+
            "\n"+
            "    <!-- DataGrid brushes -->\n"+
            "    <SolidColorBrush x:Key=\"MetroDataGrid.HighlightBrush\" Color=\"{StaticResource AccentColor}\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"MetroDataGrid.HighlightTextBrush\" Color=\"{StaticResource IdealForegroundColor}\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"MetroDataGrid.MouseOverHighlightBrush\" Color=\"{StaticResource AccentColor3}\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"MetroDataGrid.FocusBorderBrush\" Color=\"{StaticResource AccentColor}\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"MetroDataGrid.InactiveSelectionHighlightBrush\" Color=\"{StaticResource AccentColor2}\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"MetroDataGrid.InactiveSelectionHighlightTextBrush\" Color=\"{StaticResource IdealForegroundColor}\" options:Freeze=\"True\" />\n"+
            "\n"+
            "    <SolidColorBrush x:Key=\"MahApps.Metro.Brushes.ToggleSwitchButton.OnSwitchBrush.Win10\" Color=\"{StaticResource AccentColor}\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"MahApps.Metro.Brushes.ToggleSwitchButton.OnSwitchMouseOverBrush.Win10\" Color=\"{StaticResource AccentColor2}\" options:Freeze=\"True\" />\n"+
            "    <SolidColorBrush x:Key=\"MahApps.Metro.Brushes.ToggleSwitchButton.ThumbIndicatorCheckedBrush.Win10\" Color=\"{StaticResource IdealForegroundColor}\" options:Freeze=\"True\" />\n"+
            "</ResourceDictionary>";
        public static void CreateAccent(string name, System.Windows.Media.Color highlight, System.Windows.Media.Color accent) {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.File.Create(System.IO.Path.Combine(MeCore.DataDirectory, "Color", name + ".xaml")));
            System.Threading.Tasks.Task.WaitAll(sw.WriteAsync(sample.Replace("{Highlight}", BitConverter.ToString(new byte[] { highlight.R, highlight.G, highlight.B }).Replace("-", "")).Replace("{Accent}", BitConverter.ToString(new byte[] { accent.R,accent.G,accent.B}).Replace("-", ""))));
            sw.Close();
        }
    }
}
