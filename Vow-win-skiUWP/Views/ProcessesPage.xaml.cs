using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
using Vow_win_skiUWP.Core.Processes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Vow_win_skiUWP.Views
{
    public class ProcessesPageModel
    {
        public ObservableCollection<PCB> list { get; set; } = PCB.GetPcbsList();
        public PCB SPCB { get; set; } = null;
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

            UserInterface.CreateProcess("Nazwa", "silnia12.txt");
            UserInterface.CreateProcess("Nazwa1", "idle.txt");
            UserInterface.CreateProcess("Nazwa2", "nadawca.txt");
        }

        private void ProcessList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

            model.SPCB = PCB.GetPCB((sender as PCB).PID);
        }
    }
}
