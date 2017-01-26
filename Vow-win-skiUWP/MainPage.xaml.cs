using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Vow_win_skiUWP.Core;
using Vow_win_skiUWP.Core.CPU;
using Vow_win_skiUWP.Core.Processes;
using Vow_win_skiUWP.Log;
using Vow_win_skiUWP.Views.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Vow_win_skiUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            IconsListBox.SelectedItem = ProcessesListBoxItem;
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
                case "IPCListBoxItem":
                    RootFrame.Navigate(typeof(Views.IPCPage));
                    break;
                case "LockersListBoxItem":
                    RootFrame.Navigate(typeof(Views.LockersPage));
                    break;
                case "CPUListBoxItem":
                    RootFrame.Navigate(typeof(Views.CPUPage));
                    break;
                case "DiscListBoxItem":
                    RootFrame.Navigate(typeof(Views.DiscPage));
                    break;
            }
        }

        private void NextOrderButton_OnClick(object sender, RoutedEventArgs e)
        {
            Interpreter.GetInstance.InterpretOrder();
        }

        private void RunToEnd_OnClick(object sender, RoutedEventArgs e)
        {
            while (!Scheduler.GetInstance.ListEmpty())
                Interpreter.GetInstance.InterpretOrder();
        }
    }
}
