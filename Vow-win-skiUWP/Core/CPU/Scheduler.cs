using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Core.Processes;

namespace Vow_win_skiUWP.Core.CPU
{
    public sealed class Scheduler
    {
        private static volatile Scheduler _instance;
        private static readonly object SyncRoot = new object();
        private ObservableCollection<PCB> WaitingForProcessor { get; set; }

        private Scheduler()
        {
            WaitingForProcessor = new ObservableCollection<PCB>();
        }

        public static Scheduler GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new Scheduler();
                    }
                }

                return _instance;
            }
        }

        public bool ListEmpty()
        {
            if (WaitingForProcessor.Count > 1)
                return false;
            return true;
        }

        public void AddProcess(PCB process)
        {
            WaitingForProcessor.Add(process);
        }

        public void RemoveProcess(PCB process)
        {
            //WaitingForProcessor.RemoveAt(element => element.PID == process.PID);
        }

        //public PCB SearchForProcessInList(PCB process)
        //{
        //    //return WaitingForProcessor.Find(element => element.PID == process.PID);
        //}

        public void PrintList()
        {
            Console.WriteLine(ToString());
        }

        public override string ToString()
        {
            string result = string.Empty;

            foreach (var elem in WaitingForProcessor)
            {
                result += (elem.PID + " " + elem.Name + "\n");
            }
            return result;
        }

        public PCB PriorityAlgorithm()
        {
            var process = WaitingForProcessor
                .Aggregate((elem1, elem2) =>
                    (elem1.CurrentPriority < elem2.CurrentPriority ? elem1 : elem2));

            return process;
        }

        public PCB GetRunningPCB()
        {
            return WaitingForProcessor
                .SingleOrDefault(x => x.State == ProcessState.Running);
        }

        public void AgingWaitingForProcesorTime()
        {
            if (!ListEmpty())
            {
                foreach (var pcb in WaitingForProcessor)
                {
                    if (pcb.State != ProcessState.Running && !pcb.IsIdleProcess())
                        pcb.WaitingForProcessorTime++;
                }
            }
        }

        public void NegatedAgingWaitingForProcesorTime()
        {
            if (!ListEmpty())
            {
                foreach (var pcb in WaitingForProcessor)
                {
                    if (pcb.State != ProcessState.Running && !pcb.IsIdleProcess())
                        pcb.WaitingForProcessorTime--;
                }
            }
        }

        public void AgingProcessPriority()
        {
            if (!ListEmpty())
            {
                foreach (var pcb in WaitingForProcessor)
                {
                    if (pcb.WaitingForProcessorTime % 3 == 0 && pcb.State != ProcessState.Running &&
                        !pcb.IsIdleProcess() && pcb.CurrentPriority - 1 >= 0)
                    {
                        pcb.CurrentPriority--;
                        Console.WriteLine("Postarzono proces: " + pcb.Name);
                    }
                }
            }
        }

        public void RejuvenationCurrentProcess()
        {
            if (!ListEmpty())
            {
                if (GetRunningPCB() != null)
                {
                    var runningPcb = GetRunningPCB();
                    if (runningPcb.CurrentPriority < runningPcb.StartPriority &&
                        !runningPcb.IsIdleProcess() && CPU.GetInstance.OrderTime % 3 == 0)
                    {
                        runningPcb.CurrentPriority++;
                        Console.WriteLine("Odmłodzono process: " + runningPcb.Name);
                    }
                }
            }
        }

        public bool CheckIfOtherProcessShouldGetCPU()
        {
            if (GetRunningPCB() == PriorityAlgorithm())
                return false;
            return true;
        }

        public void RevriteRegistersFromCPU()
        {
            GetRunningPCB().Registers.A = CPU.GetInstance.Register.A;
            GetRunningPCB().Registers.B = CPU.GetInstance.Register.B;
            GetRunningPCB().Registers.C = CPU.GetInstance.Register.C;
            GetRunningPCB().Registers.D = CPU.GetInstance.Register.D;
        }

        public void RevriteRegistersToCPU()
        {
            CPU.GetInstance.Register.A = GetRunningPCB().Registers.A;
            CPU.GetInstance.Register.B = GetRunningPCB().Registers.B;
            CPU.GetInstance.Register.C = GetRunningPCB().Registers.C;
            CPU.GetInstance.Register.D = GetRunningPCB().Registers.D;
        }

        public void SwitchCPUForOtherProcess()
        {
            CPU.GetInstance.OrderTime = 1;
            RevriteRegistersFromCPU();
            GetInstance.GetRunningPCB().WaitForScheduling();
            GetInstance.PriorityAlgorithm().RunReadyProcess();
            GetRunningPCB().WaitingForProcessorTime = 1;
            RevriteRegistersToCPU();
        }

        public ObservableCollection<PCB> GetProcessList()
        {
            return WaitingForProcessor;
        }
    }
}
