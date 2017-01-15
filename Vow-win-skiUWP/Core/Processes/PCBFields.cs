using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Annotations;
using Vow_win_skiUWP.Core.CPU;
using Vow_win_skiUWP.Core.MemoryModule;

namespace Vow_win_skiUWP.Core.Processes
{
    public enum ProcessState
    {
        New,

        /// <summary>
        /// Proces gotowy, oczekujący w kolejce na uruchomienie
        /// </summary>
        Ready,

        Running,

        /// <summary>
        /// Proces jest wstrzymany i oczekuje na coś, do czasu wznowienia nie będzie wykonywany
        /// </summary>
        Waiting,

        Terminated
    }


    public partial class PCB : INotifyPropertyChanged
    {
        private int _currentPriority = 7;
        /// <remarks>0 - najwyższy priorytet, 7 - najniższy</remarks>
        public int CurrentPriority
        {
            get { return _currentPriority; }
            set
            {
                _currentPriority = value; 
                OnPropertyChanged("CurrentPriority");
            }
        }

        private int _startPriority = 7;

        public int StartPriority
        {
            get { return _startPriority; }
            set
            {
                _startPriority = value;
                OnPropertyChanged("StartPriority");
            }
        }

        private int _waitingForProcessorTime = 1;

        public int WaitingForProcessorTime
        {
            get { return _waitingForProcessorTime; }
            set
            {
                _waitingForProcessorTime = value;
                OnPropertyChanged("WaitingForProcessorTime");
            }
        }

        public Register Registers = new Register();

        private string _name = string.Empty;
        /// <summary>
        /// Nazwa procesu, musi być unikalna
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Identyfikator procesu, musi być unikalny
        /// </summary>
        private int _PID = 0;

        public byte PID
        {
            get
            {
                return (byte)_PID;
            }
        }

        private ProcessState _state = ProcessState.Terminated;

        public ProcessState State {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }
        }

        public int InstructionCounter = 0;

        public ProcessPages MemoryBlocks = null;

        public int MaxMemory = 0;

        /// <summary>
        /// 1, jeśli proces został uśpiony z powodu oczekiwania na wiadomość
        /// </summary>
        public int ReceiverMessageLock = 0;

        /// <summary>
        /// Zamek oczekiwania na zatrzymanie - jeśli zatrzymywany proces
        /// ma stan inny niż Running, proces zatrzymujący blokuje się
        /// pod tym zamkiem i odblokowuje dopiero po zamknięciu procesu
        /// </summary>
        Lockers StopperLock = new Lockers();

        /// <summary>
        /// Proces zamykający ten proces
        /// </summary>
        PCB ClosingProcess = null;

        /// <summary>
        /// True, jeśli podczas zamykania procesu proces miał stan inny niż Running
        /// </summary>
        private bool WaitingForStopping = false;

        //private PipeClient client = null;

        public SourceOfCode Source = SourceOfCode.WindowsDisc;


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
