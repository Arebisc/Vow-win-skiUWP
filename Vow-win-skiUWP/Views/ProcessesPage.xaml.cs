using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Streaming.Adaptive;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Vow_win_skiUWP.Annotations;
using Vow_win_skiUWP.Core.MemoryModule;
using Vow_win_skiUWP.Core.Processes;
using Vow_win_skiUWP.Views.Helpers;
using Page = Windows.UI.Xaml.Controls.Page;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Vow_win_skiUWP.Views
{
    public class ProcessesPageModel : INotifyPropertyChanged
    {
        private PCB _spcb = null;
        public ObservableCollection<PCB> _list { get; set; } = PCB.GetPcbsList();

        public PCB SPCB
        {
            get { return _spcb; }
            set
            {
                _spcb = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PCB> list
        {
            get { return _list; }
            set
            {
                _list = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProcessesPage : Page
    {
        private ProcessesPageModel model;

        public ProcessesPage()
        {
            model = new ProcessesPageModel();
            this.InitializeComponent();
            this.DataContext = model;
        }

        private void ProcessList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            int selectedItemIndex = listView.SelectedIndex;
            var selectedItem = listView.ContainerFromIndex(selectedItemIndex) as ListViewItem;
            if (selectedItem != null)
            {
                PCB selectedPCB = selectedItem.Content as PCB;
                model.SPCB = selectedPCB;
                SPCBStack.Visibility = Visibility.Visible;
            }
        }

        private async void MemoryBlocks_OnClick(object sender, RoutedEventArgs e)
        {
            var popup = new MemoryPopupDialog("Memory blocks", Memory.GetInstance.DisplayPageList(model.SPCB.PID));
            await popup.ShowAsync();
        }

        private async void CPButton_Clicked(object sender, RoutedEventArgs e)
        {
            var popup = new Helpers.CPDialog();
            await popup.ShowAsync();
        }

        private async void CppButton_OnClick(object sender, RoutedEventArgs e)
        {
            var popup = new Helpers.CppDialog();
            await popup.ShowAsync();
        }

        private void NprAllButton_OnClick(object sender, RoutedEventArgs e)
        {
            PCB.RunAllNewProcesses();
        }

        private void StartProcess_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            UserInterface.RunNewProcess(model.SPCB.Name);
        }

        private void StopProcess_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            UserInterface.StopProcess(model.SPCB.Name);
        }

        private void SleepProcess_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            UserInterface.SleepProcess(model.SPCB.Name);
        }

        private async void MemoryProgram_OnClicked(object sender, RoutedEventArgs e)
        {
            var popup = new MemoryPopupDialog("Program", Memory.GetInstance.DisplayProgram(model.SPCB.PID));
            await popup.ShowAsync();
        }

        private void WakeupProcess_Click(object sender, RoutedEventArgs e)
        {
            if(model.SPCB != null)
                UserInterface.ResumeProcess(model.SPCB.Name);
        }
    }
}
