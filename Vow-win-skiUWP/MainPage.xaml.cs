using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Vow_win_skiUWP.Views.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Vow_win_skiUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Dictionary<string, string> _defaultWatermarkDictionary;

        public MainPage()
        {
            this.InitializeComponent();
            InitializeWatermarkDictionary();
            SetUpAllWatermarks();
        }

        private void InitializeWatermarkDictionary()
        {
            _defaultWatermarkDictionary = new Dictionary<string, string>
            {
                {"CmdTb", "Wpisz komendę shella:"},
            };
        }

        private void SetUpAllWatermarks()
        {
            Watermark.SetWatermarkIfEmptyTextBox(CmdTb, _defaultWatermarkDictionary[CmdTb.Name]);
        }

        private void TextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox selectedTextbox = sender as TextBox;
            Watermark.OnFocused(selectedTextbox, _defaultWatermarkDictionary[selectedTextbox.Name]);
        }

        private void TextBox_OnFocusLost(object sender, RoutedEventArgs e)
        {
            TextBox selectedTextbox = sender as TextBox;
            Watermark.OnFocusLost(selectedTextbox, _defaultWatermarkDictionary[selectedTextbox.Name]);
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void IconsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((sender as ListBox).SelectedItem as ListBoxItem).Name)
            {
                case "ProcessesListBoxItem":
                    RootFrame.Navigate(typeof(Views.ProcessesPage));
                    break;
                case "MemoryListBoxItem":
                    RootFrame.Navigate(typeof(Views.MemoryPage));
                    break;
                case "SyncAndIPCListBoxItem":
                    RootFrame.Navigate(typeof(Views.SyncAndIPCPage));
                    break;
                case "CPUListBoxItem":
                    RootFrame.Navigate(typeof(Views.CPUPage));
                    break;
                case "DiscListBoxItem":
                    RootFrame.Navigate(typeof(Views.DiscPage));
                    break;
            }
        }

        private void Cmd_EnterPressed(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == VirtualKey.Enter)
                Cmd_ExecuteCommand(CmdTb.Text);
        }
        //TODO
        private async void Cmd_ExecuteCommand(string command)
        {
            var dialog = new MessageDialog(command);
            await dialog.ShowAsync();
            CmdTb.Text = "";
            Watermark.OnFocusLost(CmdTb, _defaultWatermarkDictionary[CmdTb.Name]);
        }
    }
}
