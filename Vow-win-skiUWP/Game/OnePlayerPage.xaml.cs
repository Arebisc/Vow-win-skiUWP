using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Vow_win_skiUWP.Game
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OnePlayerPage : Page
    {
        public OnePlayerPage()
        {
            this.InitializeComponent();
        }

        private void _1x1Button_Click(object sender, RoutedEventArgs e)
        {
            int[] position = { 1, 1 };
            if (GameCore.GetInstance.SetBoardsValue(position) != 0)
            {
                this._1x1Button.Content = "O";
            }
            else
            {
                this._1x1Button.Content = "X";
            }
        }
        private void _1x2Button_Click(object sender, RoutedEventArgs e)
        {
            int[] position = { 1, 2 };
            if (GameCore.GetInstance.SetBoardsValue(position) != 0)
            {
                this._1x2Button.Content = "O";
            }
            else
            {
                this._1x2Button.Content = "X";
            }
        }

        private void _1x3Button_Click(object sender, RoutedEventArgs e)
        {
            int[] position = { 1, 3 };
            if (GameCore.GetInstance.SetBoardsValue(position) != 0)
            {
                this._1x3Button.Content = "O";
            }
            else
            {
                this._1x3Button.Content = "X";
            }
        }

        private void _2x1Button_Click(object sender, RoutedEventArgs e)
        {
            int[] position = { 2, 1 };
            if (GameCore.GetInstance.SetBoardsValue(position) != 0)
            {
                this._2x1Button.Content = "O";
            }
            else
            {
                this._2x1Button.Content = "X";
            }
        }

        private void _2x2Button_Click(object sender, RoutedEventArgs e)
        {
            int[] position = { 2, 2 };
            if (GameCore.GetInstance.SetBoardsValue(position) != 0)
            {
                this._2x2Button.Content = "O";
            }
            else
            {
                this._2x2Button.Content = "X";
            }
        }

        private void _2x3Button_Click(object sender, RoutedEventArgs e)
        {
            int[] position = { 2, 3 };
            if (GameCore.GetInstance.SetBoardsValue(position) != 0)
            {
                this._2x3Button.Content = "O";
            }
            else
            {
                this._2x3Button.Content = "X";
            }
        }

        private void _3x1Button_Click(object sender, RoutedEventArgs e)
        {
            int[] position = { 3, 1 };
            if (GameCore.GetInstance.SetBoardsValue(position) != 0)
            {
                this._3x1Button.Content = "O";
            }
            else
            {
                this._3x1Button.Content = "X";
            }
        }

        private void _3x2Button_Click(object sender, RoutedEventArgs e)
        {
            int[] position = { 3, 2 };
            if (GameCore.GetInstance.SetBoardsValue(position) != 0)
            {
                this._3x2Button.Content = "O";
            }
            else
            {
                this._3x2Button.Content = "X";
            }
        }

        private void _3x3Button_Click(object sender, RoutedEventArgs e)
        {
            int[] position = { 3, 3 };
            if (GameCore.GetInstance.SetBoardsValue(position) != 0)
            {
                this._3x3Button.Content = "O";
            }
            else
            {
                this._3x3Button.Content = "X";
            }
        }
    }
}
