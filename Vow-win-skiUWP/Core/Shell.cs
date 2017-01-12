using System;
using System.Linq;
using System.Threading.Tasks;
using Vow_win_skiUWP.Core.CPU;
using Vow_win_skiUWP.Core.FileSystem;
using Vow_win_skiUWP.Core.IPC;
using Vow_win_skiUWP.Core.MemoryModule;
using Vow_win_skiUWP.Core.Processes;
using Vow_win_skiUWP.Log;

namespace Vow_win_skiUWP.Core
{
    public class Shell
    {
        private Reporter reporter;
        private Shell()
        { }

        private static volatile Shell _instance;
        private static object _syncRoot = new object();
        public static Shell GetShell
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new Shell();
                        }
                    }
                }
                return _instance;
            }
        }

        public void OpenShell()
        {
            reporter = new Reporter();
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine();
                Console.Write("root\\>");
                reporter.AddLog("\nroot\\>");
                string cmdline = Console.ReadLine();

                // ReSharper disable once PossibleNullReferenceException
                string cmd = cmdline.Split(' ')[0];
                string p1 = "";
                if (cmdline.Contains(" ")) p1 = cmdline.Split(' ')[1];
                string p2 = string.Join(" ", cmdline.Split(' ').Skip(2));

                cmd = cmd.ToUpper();
                switch (cmd)
                {
                    case "":
                        break;
                    case "HELP":
                        ShowHelp();
                        break;
                    case "EX":
                        if (p1 == "-all")
                        {
                            while (!Scheduler.GetInstance.ListEmpty())
                                Interpreter.GetInstance.InterpretOrder();
                        }
                        else
                            Interpreter.GetInstance.InterpretOrder();
                        break;
                    case "QUIT":
                        Console.WriteLine("Zamykanie systemu...");
                        reporter.AddLog("Zamykanie systemu..." );
                        Task.Delay(2500).Wait();
                        exit = true;
                        break;
                    case "BS":
                        throw new Exception(p1 == "" ? "Wyjątek wywołany przez użytkownika" : p1);
                    //===================================================
                    case "SRP":
                        Scheduler.GetInstance.PrintList();
                        break;
                    case "SRG":
                        CPU.CPU.GetInstance.Register.PrintRegisters();
                        break;
                    //===================================================
                    case "CP":
                        UserInterface.CreateProcess(p1, p2);
                        break;
                    case "CPD":
                        UserInterface.CreateProcessFromDisc(p1, p2);
                        break;
                    case "CPP":
                        UserInterface.ChangePriority(p1, p2);
                        break;
                    case "NPR":
                        UserInterface.RunNewProcess(p1);
                        break;
                    case "HP":
                        UserInterface.StopProcess(p1);
                        break;
                    case "SAP":
                        UserInterface.ShowAllProcesses();
                        break;
                    case "SPCB":
                        UserInterface.ShowPCB(p1);
                        break;
                    case "WP":
                        UserInterface.SleepProcess(p1);
                        break;
                    case "RP":
                        UserInterface.ResumeProcess(p1);
                        break;
                    //===================================================
                    case "SPL":
                        if (PCB.GetPCB(p1) != null)
                            Memory.GetInstance.DisplayPageList(PCB.GetPCB(p1).PID);
                        break;
                    case "SPC":
                        if (PCB.GetPCB(p1) == null) break;
                        int nr;
                        if (Int32.TryParse(p2, out nr))
                            Memory.GetInstance.DisplayPageContent(PCB.GetPCB(p1).PID, nr);
                        else
                            Console.WriteLine("Błąd: Nieprawidłowy numer strony.");
                            reporter.AddLog("Błąd: Nieprawidłowy numer strony.");
                        break;
                    case "SEP":
                        Memory.GetInstance.DisplayFreeFrames();
                        break;
                    case "SM":
                        Memory.GetInstance.DisplayPhysicalMemory();
                        break;
                    case "SFIFO":
                        Memory.GetInstance.DisplayFifoQueue();
                        break;
                    case "SLM":
                        Console.WriteLine(Memory.GetInstance.ReadMessage() + "\n");
                        reporter.AddLog(Memory.GetInstance.ReadMessage() + "\n");
                        break;
                    //===================================================
                    case "SAM":
                        PipeServer.GetServer.Show();
                        break;
                    case "SMH":
                        PipeServer.GetServer.ShowHistory();
                        break;
                    //===================================================
                    case "SW":
                        LockersHolder.ProcLock.Show();
                        break;
                    //===================================================
                    case "DIR":
                    case "LS":
                        Disc.GetDisc.ShowDirectory();
                        break;
                    case "CF":
                        Disc.GetDisc.CreateFile(p1, p2);
                        break;
                    case "CW":
                        Disc.GetDisc.CreateFromWindows(p1, p2);
                        break;
                    case "TYPE":
                        Console.WriteLine(Disc.GetDisc.GetFileData(p1) ?? "Błąd czytania pliku");
                        reporter.AddLog(Disc.GetDisc.GetFileData(p1) ?? "Błąd czytania pliku");
                        break;
                    case "DF":
                        Disc.GetDisc.DeleteFile(p1);
                        break;
                    case "APP":
                        Disc.GetDisc.AppendToFile(p1, p2);
                        break;
                    case "SDB":
                        Disc.GetDisc.ShowDataBlocks(p1);
                        break;
                    default:
                        Console.WriteLine("Nieznane polecenie\nWpisz \"help\" aby wyświetlić listę dostępnych poleceń");
                        reporter.AddLog("Nieznane polecenie\nWpisz \"help\" aby wyświetlić listę dostępnych poleceń");
                        break;
                }
            }

        }

        private void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine("Parametry: [opcjonalny] {wymagany}");
            Console.WriteLine();
            Console.WriteLine("Polecenia\t\t   Opis");
            reporter.AddLog("\nParametry: [opcjonalny] {wymagany}\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-------------------------------Ogólne---------------------------------");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("HELP\t\t   Wyświetla tę listę");
            Console.WriteLine("EX [-all]\t   Wykonuje kolejny rozkaz, [-all] - wszystkie rozkazy");
            Console.WriteLine("QUIT\t\t   Zamyka system");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("------------------------------Procesor--------------------------------");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("SRP\t\t   Wyświetla listę procesów Ready");
            Console.WriteLine("SRG\t\t   Wyświetla rejestry procesora");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-------------------------------Procesy--------------------------------");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("CP {nazwa} {prog}  Tworzy proces {nazwa} z programu {prog} na dysku Windows");
            Console.WriteLine("CPD {nazwa} {prog} Tworzy proces {nazwa} z programu {prog} na dysku systemu");
            Console.WriteLine("CPP {nazwa} {pr}   Ustawia priorytet procesu {nazwa} na {pr}");
            Console.WriteLine("NPR {nazwa/\"-all\"} Uruchom nowy proces {nazwa}, {-all} - wszystkie procesy");
            Console.WriteLine("HP {nazwa}\t   Zatrzymuje proces {nazwa}");
            Console.WriteLine("SAP\t\t   Wyświetla listę wszystkich procesów");
            Console.WriteLine("SPCB {nazwa}\t   Wyświetla listę PCB procesu {nazwa}");
            Console.WriteLine("WP {nazwa}\t   Usypia uruchomiony proces {nazwa}");
            Console.WriteLine("RP {nazwa}\t   Wznawia uśpiony proces {nazwa}");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-------------------------------Pamięć---------------------------------");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("SPL {nazwa}\t   Wyświetla listę stron procesu {nazwa}");
            Console.WriteLine("SPC {nazwa} {nr}   Wyświetla zawartość strony {nr} procesu {nazwa}");
            Console.WriteLine("SEP\t\t   Wyświetla puste stron");
            Console.WriteLine("SM\t\t   Wyświetla całą pamięć");
            Console.WriteLine("SLM\t\t   Wyświetla ostatnią wiadomość z pamięci");
            Console.WriteLine("SFIFO\t\t   Wyświetla kolejke FIFO");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-----------------------------Komunikacja------------------------------");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("SAM\t\t   Wyświetla wszystkie oczekujące komunikaty");
            Console.WriteLine("SMH\t\t   Wyświetla historię komunikatów");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("----------------------------Synchronizacja----------------------------");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("SW\t\t   Wyświetla procesy oczekujące pod zamkiem komunikatów");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("--------------------------------Dysk----------------------------------");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("DIR/LS\t\t   Wyświetla listę plików");
            Console.WriteLine("CW {nazwa} {plik}  Tworzy plik {nazwa} i wypełnia danymi z {plik} Windows");
            Console.WriteLine("CF {nazwa} [dane]  Tworzy plik {nazwa} i wypełnia [dane]");
            Console.WriteLine("APP {nazwa} [dane] Dołącza [dane] do pliku {nazwa}");
            Console.WriteLine("TYPE {nazwa}\t   Wyświetla dane z pliku {nazwa}");
            Console.WriteLine("DF {nazwa}\t   Usuwa plik {nazwa}");
            Console.WriteLine("SDB [nr]\t   Wyświetla dane wszystkich bloków, [nr] bloków na ekran");
        }
    }
}
