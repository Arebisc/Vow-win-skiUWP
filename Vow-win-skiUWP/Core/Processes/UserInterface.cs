using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Log;

namespace Vow_win_skiUWP.Core.Processes
{
    static class UserInterface
    {
        private static Reporter reporter = new Reporter();
        public static void CreateProcess(string Name, string Path)
        {
            Name = Name.Trim();
            Path = Path.Trim();

            if (Name == "")
            {
                Console.WriteLine("Nie podano nazwy procesu.");
                reporter.AddLog("Nie podano nazwy procesu.");
                return;
            }
            if (Path == "")
            {
                Console.WriteLine("Nie podano sciezki do programu.");
                reporter.AddLog("Nie podano sciezki do programu.");
                return;
            }

            new PCB(Name, new Random().Next(0, 7), Path, SourceOfCode.WindowsDisc);
        }

        public static void CreateProcessFromDisc(string Name, string Path)
        {

            Name = Name.Trim();
            Path = Path.Trim();

            if (Name == "")
            {
                Console.WriteLine("Nie podano nazwy procesu.");
                reporter.AddLog("Nie podano nazwy procesu.");
                return;
            }
            if (Path == "")
            {
                Console.WriteLine("Nie podano sciezki do programu.");
                reporter.AddLog("Nie podano sciezki do programu.");
                return;
            }

            new PCB(Name, new Random().Next(0, 7), Path, SourceOfCode.SystemDisc);
        }

        public static void RunNewProcess(string Name)
        {
            if (Name == "-all")
            {
                PCB.RunAllNewProcesses();
            }
            else
            {
                PCB pcb = PCB.GetPCB(Name);
                if (pcb != null) pcb.RunNewProcess();
            }
        }

        public static void StopProcess(string Name)
        {
            PCB pcb = PCB.GetPCB(Name);
            if (pcb != null) pcb.TerminateProcess(ReasonOfProcessTerminating.UserClosed);
        }

        public static void ShowAllProcesses()
        {
            PCB.PrintAllPCBs();
        }

        public static void ShowPCB(string Name)
        {
            PCB pcb = PCB.GetPCB(Name);
            if (pcb != null) pcb.PrintAllFields();
        }

        public static void SleepProcess(string Name)
        {
            PCB pcb = PCB.GetPCB(Name);
            if (pcb != null) pcb.WaitForSomething();
        }

        public static void ResumeProcess(string Name)
        {
            PCB pcb = PCB.GetPCB(Name);
            if (pcb != null) pcb.StopWaiting();
        }

        public static void ChangePriority(string Name, string Priority)
        {

            int NewPriority = 0;

            if (!int.TryParse(Priority, out NewPriority))
            {
                Console.WriteLine("Podana wartosc nie jest liczba.");
                reporter.AddLog("Podana wartosc nie jest liczba.");
                return;
            }

            if (NewPriority >= 0 && NewPriority <= 7)
            {
                PCB pcb = PCB.GetPCB(Name);
                if (pcb != null)
                {
                    pcb.CurrentPriority = NewPriority;
                    pcb.StartPriority = NewPriority;
                    pcb.WaitingForProcessorTime = 1;
                    Console.WriteLine("Ustawiono nowy priorytet (" + NewPriority + ") dla procesu " + Name + ".");
                    reporter.AddLog("Ustawiono nowy priorytet (" + NewPriority + ") dla procesu " + Name + ".");
                }
            }
            else
            {
                Console.WriteLine("Priorytet procesu musi miescic sie w zakresie od 0 do 7.");
                reporter.AddLog("Priorytet procesu musi miescic sie w zakresie od 0 do 7.");
            }
        }

    }
}
