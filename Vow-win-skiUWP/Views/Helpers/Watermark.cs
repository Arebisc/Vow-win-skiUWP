using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Vow_win_skiUWP.Views.Helpers
{
    class Watermark
    {
        public static void SetUp(TextBox textbox, string info)
        {
            textbox.Foreground = new SolidColorBrush(Colors.DarkGray);
            textbox.FontStyle = FontStyle.Italic;
            textbox.Text = info;
        }

        public static void Clear(TextBox textbox)
        {
            textbox.Foreground = new SolidColorBrush(Colors.White);
            textbox.FontStyle = FontStyle.Normal;
            textbox.Text = String.Empty;
        }

        public static void OnFocused(TextBox selectedTextbox, string watermark)
        {
            if (selectedTextbox.Text == watermark)
                Clear(selectedTextbox);
        }

        public static void OnFocusLost(TextBox selectedTextbox, string watermark)
        {
            if (selectedTextbox.Text == String.Empty)
                SetUp(selectedTextbox, watermark);
        }

        public static void ClearTextboxWithWatermark(TextBox textBox, string watermark)
        {
            if (textBox.Text == watermark)
                Watermark.Clear(textBox);
        }

        public static void SetWatermarkIfEmptyTextBox(TextBox textbox, string watermark)
        {
            if (textbox.Text == String.Empty)
                Watermark.SetUp(textbox, watermark);
        }
    }
}
