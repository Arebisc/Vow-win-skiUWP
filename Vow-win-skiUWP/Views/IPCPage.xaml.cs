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
using Vow_win_skiUWP.Core.IPC;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Vow_win_skiUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IPCPage : Page
    {
        public IPCPage()
        {                    
            var collection = new MessageCollection();           
            this.InitializeComponent();
            this.DataContext = collection;            
        }
    }

    public class MessageCollection
    {
        
        public ObservableCollection<Message> WaitingMessages { get; set; } = PipeServer.GetServer.ShowWaiting();
        public ObservableCollection<Message> ReceivedMessages { get; set; } = PipeServer.GetServer.ShowHistory();

    }
    
        
}
