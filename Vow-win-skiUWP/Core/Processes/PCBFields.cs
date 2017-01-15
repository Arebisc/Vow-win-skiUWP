using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


    public partial class PCB
    {
        /// <remarks>0 - najwyższy priorytet, 7 - najniższy</remarks>
        public int CurrentPriority { get; set; } = 7;

        public int StartPriority = 7;

        public int WaitingForProcessorTime = 1;

        public Register Registers = new Register();

        /// <summary>
        /// Nazwa procesu, musi być unikalna
        /// </summary>
        public string Name { get; set; } = "";

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

        public ProcessState State { get; set; } = ProcessState.Terminated;

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

    }
}
