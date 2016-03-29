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
using System.Windows.Shapes;

namespace MTMCL.Accents
{
    /// <summary>
    /// AccentWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AccentWindow
    {
        public AccentWindow()
        {
            InitializeComponent();
        }

        private void butOK_Click(object sender, RoutedEventArgs e)
        {
            AccentHelper.CreateAccent(!string.IsNullOrWhiteSpace(txtName.Text) ? txtName.Text : ((SolidColorBrush)butAccent.Background).Color.ToString(), ((SolidColorBrush)butHighLight.Background).Color, ((SolidColorBrush)butAccent.Background).Color);
            Close();
        }

        private void butHighLight_Click(object sender, RoutedEventArgs e)
        {
            var color = new System.Windows.Forms.ColorDialog()
            {
                Color = System.Drawing.Color.FromArgb(((SolidColorBrush)butHighLight.Background).Color.R, ((SolidColorBrush)butHighLight.Background).Color.G, ((SolidColorBrush)butHighLight.Background).Color.B),
                AnyColor = true,
                FullOpen = true,
                ShowHelp = true,
                SolidColorOnly = true
            };
            color.ShowDialog();
            ((SolidColorBrush)butHighLight.Background).Color = Color.FromArgb(0xFF, color.Color.R, color.Color.G, color.Color.B);
        }

        private void butAccent_Click(object sender, RoutedEventArgs e)
        {
            var color = new System.Windows.Forms.ColorDialog()
            {
                Color = System.Drawing.Color.FromArgb(((SolidColorBrush)butAccent.Background).Color.R, ((SolidColorBrush)butAccent.Background).Color.G, ((SolidColorBrush)butAccent.Background).Color.B),
                AnyColor = true,
                FullOpen = true,
                ShowHelp = true,
                SolidColorOnly = true
            };
            color.ShowDialog();
            ((SolidColorBrush)butAccent.Background).Color = Color.FromArgb(0xCC, color.Color.R, color.Color.G, color.Color.B);
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtName.Text = txtName.Text
                .Replace(@"\", "")
                .Replace(@"/", "")
                .Replace(@"|", "")
                .Replace("\"", "")
                .Replace(@"*", "")
                .Replace(@"?", "")
                .Replace(@":", "")
                .Replace(@"<", "")
                .Replace(@">", "");
        }
    }
}
