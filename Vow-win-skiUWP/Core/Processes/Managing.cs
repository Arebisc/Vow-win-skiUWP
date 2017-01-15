using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Core.MemoryModule;
using Vow_win_skiUWP.Log;

namespace Vow_win_skiUWP.Core.Processes
{
    public enum ReasonOfProcessTerminating
    {
        Ended = 1,
        ThrownError = 2,
        UserClosed = 3,
        CriticalError = 4,
        KilledByOther = 5,
        ClosingSystem = 6
    }

    public enum SourceOfCode
    {
        WindowsDisc,    //Plik z Windows
        SystemDisc      //Dysk systemu
    }

    public partial class PCB
    {

        private static LinkedList<PCB> _CreatedPCBs = new LinkedList<PCB>();
        private static int _NextPID = -1;

        /// <summary>
        /// Pusty konstruktor do testow
        /// </summary>
        public PCB()
        {
            _PID = ++_NextPID;
        }

        /// <summary>
        /// RUSZ TEN KONSTRUKTOR JESZCZE RAZ TO ZABIJE!!!!
        /// </summary>
        /// <remarks>WAL SIE</remarks>
        public PCB(string name, int priority)
        {
            _PID = ++_NextPID;
            this.Name = name;
            this.CurrentPriority = priority;
            this.State = ProcessState.Ready;
            this.StartPriority = 7;
        }

        /// <summary>
        /// Tworzy nowy proces bez uruchamiania go (stan procesu = New)
        /// </summary>
        /// <param name="Name_">Nazwa procesu, musi być unikalna</param>
        /// <param name="ProgramFilePath">Ścieżka do pliku z programem (z której zostanie wczytany kod programu)</param>
        /// <remarks>Utworzenie procesu - XC</remarks>
        public PCB(string Name_, int Priority, string ProgramFilePath, SourceOfCode Source = SourceOfCode.WindowsDisc)
        {

            //Wczytaj program
            string Program;
            this.Source = Source;

            if (Source == SourceOfCode.WindowsDisc)
            {

                try
                {
                    Program = System.IO.File.ReadAllText(ProgramFilePath);
                }
                catch
                {
                    Console.WriteLine("Nie udalo sie utworzyc procesu: w Windows nie znaleziono pliku o nazwie " + ProgramFilePath);
                    State = ProcessState.Terminated;
                    return;
                }
            }
            else
            {
                Program = FileSystem.Disc.GetDisc.GetFileData(ProgramFilePath);
                if (Program == null)
                {
                    Console.WriteLine("Nie udalo sie utworzyc procesu: na dysku systemu nie znaleziono pliku o nazwie " + ProgramFilePath);
                    State = ProcessState.Terminated;
                    return;
                }
            }

            //Nazwa procesu
            if (IsProcessNameUsed(Name_))
            {

                int i = 1;
                while (IsProcessNameUsed(Name_ + i.ToString())) i++;

                Name = Name_ + i.ToString();
                Console.WriteLine("Podana nazwa [" + Name_ + "] jest juz uzywana, proces otrzymal nazwe " + Name + ".");

            }
            else
            {
                Name = Name_;
                Console.WriteLine("Proces otrzymal nazwe " + Name + ".");
            }

            //Utwórz PCB
            _PID = ++_NextPID;

            if (Priority < 0 || Priority > 7)
            {
                Console.WriteLine("Priorytet musi miescic sie w zakresie 0 - 7. Proces otrzymal priorytet 7.");
                CurrentPriority = 7;
                StartPriority = 7;
            }
            else
            {
                CurrentPriority = Priority;
                StartPriority = Priority;
            }

            State = ProcessState.New;


            Memory.GetInstance.AllocateMemory(this, Program);

            _CreatedPCBs.AddLast(this);
            Console.WriteLine("Utworzono proces: " + this.ToString());
        }

        public static PCB CreateIdleProcess()
        {
            Console.WriteLine("Tworzenie procesu bezczynnosci systemu...");
            PCB Idle = new PCB("ProcesBezczynnosci", 7, "idle.txt", SourceOfCode.WindowsDisc);
            Idle._PID = 0;
            Idle.StartPriority = 8;
            Idle.CurrentPriority = 8;
            Idle.RunNewProcess();
            Idle.RunReadyProcess();
            return Idle;
        }

        /// <summary>Zwraca blok kontrolny procesu o podanym identyfikatorze</summary>
        /// <remarks>Znalezienie bloku PCB o danej nazwie - XN</remarks>
        public static PCB GetPCB(int PID)
        {
            LinkedList<PCB>.Enumerator en = _CreatedPCBs.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Current.PID == PID)
                {
                    Console.WriteLine("Znaleziono proces o podanym PID: " + en.Current.ToString());
                    return en.Current;
                }
            }
            Console.WriteLine("Znaleziono proces o podanym PID: " + en.Current.ToString());
            return null;
        }

        /// <summary>Zwraca blok kontrolny procesu o podanym identyfikatorze</summary>
        /// <remarks>Znalezienie bloku PCB o danej nazwie - XN</remarks>
        public static PCB GetPCB(string Name)
        {
            LinkedList<PCB>.Enumerator en = _CreatedPCBs.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Current.Name == Name)
                {
                    Console.WriteLine("Znaleziono proces o podanej nazwie: " + en.Current.ToString());
                    return en.Current;
                }
            }

            Console.WriteLine("Nie znaleziono procesu o nazwie " + Name + ".");
            return null;
        }

        public static void PrintAllPCBs()
        {
            LinkedList<PCB>.Enumerator en = _CreatedPCBs.GetEnumerator();

            Console.WriteLine("Procesy aktualnie obecne w systemie:");

            while (en.MoveNext())
            {
                Console.WriteLine(en.Current.ToString());
            }

            Console.WriteLine();

        }

        public static void RunAllNewProcesses()
        {
            LinkedList<PCB>.Enumerator en = _CreatedPCBs.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Current.State == ProcessState.New) en.Current.RunNewProcess();
            }
        }

        private bool IsProcessNameUsed(string Name)
        {
            LinkedList<PCB>.Enumerator en = _CreatedPCBs.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Current.Name == Name) return true;
            }

            return false;
        }

    }
}
