using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Vow_win_skiUWP.Annotations;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Vow_win_skiUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MemoryPage : Page
    {
        public MemoryPage()
        {
            var memoryCollection = new MemoryCollection();
            this.InitializeComponent();
            this.DataContext = memoryCollection;

        }

       
    }

    public class MemoryCollection : INotifyPropertyChanged
    {
       

        private string _queueState;
        private string _freeFramesList;
        private string _physicalMemory;

        public string QueueState
        {
            get
            {
                return _queueState;
            }
            set
            {
                _queueState = value;
                OnPropertyChanged();
            }
        }

        public string FreeFramesList
        {
            get
            {
                return _freeFramesList;
            }
            set
            {
                _freeFramesList = value;
                OnPropertyChanged();
            }
        }

        public string PhysicalMemory
        {
            get
            {
                return _physicalMemory;
            }
            set
            {
                _physicalMemory = value;
                OnPropertyChanged();
            }
        }


        public MemoryCollection()
        {
            QueueState = Core.MemoryModule.Memory.GetInstance.DisplayFifoQueue();
            FreeFramesList = Core.MemoryModule.Memory.GetInstance.DisplayFreeFrames();
            PhysicalMemory = Core.MemoryModule.Memory.GetInstance.DisplayPhysicalMemory();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
