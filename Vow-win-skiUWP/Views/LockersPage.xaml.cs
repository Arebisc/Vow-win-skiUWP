using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Vow_win_skiUWP.Core.Processes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Vow_win_skiUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LockersPage : Page
    {
        public LockersPage()
        {
            var WaitingList = new LockersCollection();
            this.InitializeComponent();
            this.DataContext = WaitingList;

           // WaitingList.LockerProces= new PCB("Proces0",1);
            WaitingList.LockersWaiting.Add(new PCB("Proces1",1));
            WaitingList.LockersWaiting.Add(new PCB("Proces2", 1));
            WaitingList.LockersWaiting.Add(new PCB("Proces3", 1));
            WaitingList.LockersWaiting.Add(new PCB("Proces4", 1));
        }   
             
    }

    public class LockersCollection
    {
        public ObservableCollection<PCB> LockersWaiting { get; set; } /*= LockersHolder.GetInstance.GetCollection();*/
        public PCB LockerProces { get; set; }

        public LockersCollection()
        {
            LockersWaiting = new ObservableCollection<PCB>();
            LockerProces = new PCB("Proces0",1);
            //if (LockersHolder.GetInstance.GetCollection().First() != null)
            //{
            //    LockerProces = LockersHolder.GetInstance.GetCollection().First();
            //}
        }
    }
}
