using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Core.FileSystem;
using Vow_win_skiUWP.Core.Processes;
using Vow_win_skiUWP.Log;

namespace Vow_win_skiUWP.Core.CPU
{
    public sealed class Interpreter
    {
        private Reporter reporter;

        private static readonly object SyncRoot = new object();
        private static volatile Interpreter _instance;

        private Interpreter()
        {
            reporter = new Reporter();
        }

        public static Interpreter GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new Interpreter();
                    }
                }
                return _instance;
            }
        }

        public void InterpretOrder()
        {
            if (Scheduler.GetInstance.CheckIfOtherProcessShouldGetCPU())
            {
                if (Scheduler.GetInstance.GetRunningPCB() != null)
                    Scheduler.GetInstance.SwitchCPUForOtherProcess();
                else
                {
                    CPU.GetInstance.OrderTime = 1;
                    var result = Scheduler.GetInstance.PriorityAlgorithm().RunReadyProcess();
                    if (result == 2)
                    {
                        Console.WriteLine("Proces został zakończony");
                        reporter.AddLog("Proces został zakończony");
                        return;
                    }
                    Scheduler.GetInstance.GetRunningPCB().WaitingForProcessorTime = 1;
                    Scheduler.GetInstance.RevriteRegistersToCPU();
                }
                Console.WriteLine("Przełączono CPU na process: " + Scheduler.GetInstance.GetRunningPCB().Name);
                reporter.AddLog("Przełączono CPU na process: " + Scheduler.GetInstance.GetRunningPCB().Name);
                Scheduler.GetInstance.NegatedAgingWaitingForProcesorTime();
                return;
            }

            var order = GetOrderFromMemory(Scheduler.GetInstance.GetRunningPCB());

            if (order.TrimEnd().EndsWith(":"))
            {
                Console.WriteLine("Etykieta o nazwie: " + order.TrimEnd().TrimEnd(':'));
                reporter.AddLog("Etykieta o nazwie: " + order.TrimEnd().TrimEnd(':'));
            }
            else
            {
                switch (GetOrderName(order))
                {
                    case "HLT":
                        HLTOrder();
                        break;
                    case "XC":
                        XCOrder(GetOrderFirstArgument(order), GetOrderSecondArgument(order));
                        break;
                    case "XD":
                        XDOrder(GetOrderFirstArgument(order));
                        break;
                    case "XR":
                        XROrder();
                        break;
                    case "XS":
                        XSOrder(GetOrderFirstArgument(order), GetOrderSecondArgument(order));
                        break;
                    case "XN":
                        XNOrder(GetOrderFirstArgument(order));
                        break;
                    case "XY":
                        XYOrder(GetOrderFirstArgument(order));
                        break;
                    case "XZ":
                        XZOrder(GetOrderFirstArgument(order));
                        break;
                    case "AD":
                        ADOrder(GetOrderFirstArgument(order), GetOrderSecondArgument(order));
                        break;
                    case "SB":
                        SBOrder(GetOrderFirstArgument(order), GetOrderSecondArgument(order));
                        break;
                    case "MU":
                        MUOrder(GetOrderFirstArgument(order), GetOrderSecondArgument(order));
                        break;
                    case "JM":
                        JMOrder(GetOrderFirstArgument(order));
                        break;
                    case "MV":
                        MVOrder(GetOrderFirstArgument(order), GetOrderSecondArgument(order));
                        break;
                    case "MN":
                        MNOrder(GetOrderFirstArgument(order), Int32.Parse(GetOrderSecondArgument(order)));
                        break;
                    case "MF":
                        MFOrder(GetOrderFirstArgument(order));
                        break;
                    case "WF":
                        WFOrder(GetOrderFirstArgument(order), GetOrderSecondArgument(order));
                        break;
                    case "WR":
                        WROrder(GetOrderFirstArgument(order), GetOrderSecondArgument(order));
                        break;
                    case "DF":
                        DFOrder(GetOrderFirstArgument(order));
                        break;
                    case "PO":
                        POOrder(GetOrderFirstArgument(order));
                        break;
                    default:
                        Console.WriteLine("Nie rozpoznano rozkazu: " + order);
                        reporter.AddLog("Nie rozpoznano rozkazu: " + order);
                        break;
                }
            }
            SchedulerMenager();
        }

        public string GetOrderFromMemory(PCB runningPCB)
        {
            var order = String.Empty;
            int iterator = 0;

            for (int i = 0; i < runningPCB.InstructionCounter; i++)
            {
                while (runningPCB.MemoryBlocks.ReadByte(iterator) != '\n' &&
                        runningPCB.MemoryBlocks.ReadByte(iterator) != '\r' &&
                        iterator <= runningPCB.MaxMemory)
                {
                    iterator++;
                }
                if (runningPCB.MemoryBlocks.ReadByte(iterator) == '\r')
                    iterator++;
                if (runningPCB.MemoryBlocks.ReadByte(iterator) == '\n')
                    iterator++;
            }

            while (runningPCB.MemoryBlocks.ReadByte(iterator) != '\n' &&
                        runningPCB.MemoryBlocks.ReadByte(iterator) != '\r' &&
                        iterator <= runningPCB.MaxMemory)
            {
                order += runningPCB.MemoryBlocks.ReadByte(iterator);
                iterator++;
            }
            if (runningPCB.MemoryBlocks.ReadByte(iterator) == '\r')
                iterator++;
            if (runningPCB.MemoryBlocks.ReadByte(iterator) == '\n')
                iterator++;
            runningPCB.InstructionCounter++;

            return order;
        }

        public string GetOrderName(string order)
        {
            string orderName = String.Empty;

            for (int i = 0; i < order.Length; i++)
            {
                if (order[i] != ' ')
                    orderName += order[i];
                else break;
            }

            return orderName;
        }

        public string GetOrderFirstArgument(string order)
        {
            var orderName = GetOrderName(order);

            var trimmedOrderName = order.TrimStart(orderName.ToCharArray());
            var trimmedWhiteSpaces = trimmedOrderName.Trim();

            if (!trimmedWhiteSpaces.Contains(','))
                return trimmedWhiteSpaces;

            var result = String.Empty;
            for (int i = 0; i < trimmedWhiteSpaces.Length; i++)
            {
                if (trimmedWhiteSpaces[i] != ',')
                    result += trimmedWhiteSpaces[i];
                else break;
            }

            return result;
        }

        public string GetOrderSecondArgument(string order)
        {
            var trimmedOrderName = order.TrimStart(GetOrderName(order).ToCharArray());
            var trimmedWhiteSpaces = trimmedOrderName.Trim();

            var trimmedFirstOrder = trimmedWhiteSpaces.TrimStart(GetOrderFirstArgument(order).ToCharArray());
            var result = trimmedFirstOrder.TrimStart(',');

            return result;
        }

        public void POOrder(string register)
        {
            Console.WriteLine("Rozkaz PO z parametrem " + register);
            reporter.AddLog("Rozkaz PO z parametrem " + register);

            Console.WriteLine(register);
            reporter.AddLog(register);
        }

        public void DFOrder(string fileName)
        {
            Console.WriteLine("Rozkaz DF z parametrem " + " " + fileName);
            reporter.AddLog("Rozkaz DF z parametrem " + " " + fileName);

            Disc.GetDisc.DeleteFile(fileName);
        }

        public void WROrder(string fileName, string register)
        {
            Console.WriteLine("Rozkaz WR z parametrem " + fileName + " " + register);
            reporter.AddLog("Rozkaz WR z parametrem " + fileName + " " + register);

            Disc.GetDisc.AppendToFile(fileName, CPU.GetInstance.Register.GetRegisterValueByName(register).ToString());
        }

        public void WFOrder(string fileName, string content)
        {
            Console.WriteLine("Rozkaz WF z parametrem " + fileName + " " + content);
            reporter.AddLog("Rozkaz WF z parametrem " + fileName + " " + content);

            Disc.GetDisc.AppendToFile(fileName, content);
        }

        public void MFOrder(string fileName)
        {
            Console.WriteLine("Rozkaz MF z parametrem " + " " + fileName);
            reporter.AddLog("Rozkaz MF z parametrem " + " " + fileName);

            Disc.GetDisc.CreateFile(fileName, String.Empty);
        }

        public void MNOrder(string register, int number)
        {
            Console.WriteLine("Rozkaz MN z parametrem " + register + " " + number);
            reporter.AddLog("Rozkaz MN z parametrem " + register + " " + number);

            CPU.GetInstance.Register.SetRegisterValueByName(register, number);
        }

        public void MVOrder(string register1, string register2)
        {
            Console.WriteLine("Rozkaz MV z parametrem " + register1 + " " + register2);
            reporter.AddLog("Rozkaz MV z parametrem " + register1 + " " + register2);

            CPU.GetInstance.Register.SetRegisterValueByName(register1,
                CPU.GetInstance.Register.GetRegisterValueByName(register2));
        }

        public void JMOrder(string tag)
        {
            Console.WriteLine("Rozkaz JM z parametrem " + tag);
            reporter.AddLog("Rozkaz JM z parametrem " + tag);
            CPU.GetInstance.Register.C--;

            if (CPU.GetInstance.Register.C != 0)
            {
                var runningPCB = Scheduler.GetInstance.GetRunningPCB();
                runningPCB.InstructionCounter = 0;

                var order = String.Empty;
                int iterator = 0;
                bool foundFlag = false;

                while (foundFlag != true)
                {
                    while (runningPCB.MemoryBlocks.ReadByte(iterator) != '\n' &&
                        runningPCB.MemoryBlocks.ReadByte(iterator) != '\r' &&
                        iterator <= runningPCB.MaxMemory)
                    {
                        order += runningPCB.MemoryBlocks.ReadByte(iterator);
                        iterator++;
                    }

                    if (runningPCB.MemoryBlocks.ReadByte(iterator) == '\r')
                        iterator++;
                    if (runningPCB.MemoryBlocks.ReadByte(iterator) == '\n')
                        iterator++;
                    runningPCB.InstructionCounter++;

                    if (order.TrimEnd().TrimEnd(':') == tag)
                        foundFlag = true;
                    order = String.Empty;
                }
            }
        }

        public void MUOrder(string register1, string register2)
        {
            Console.WriteLine("Rozkaz MV z parametrem " + register1 + " " + register2);
            reporter.AddLog("Rozkaz MV z parametrem " + register1 + " " + register2);
            if (!IsNumeric(register2))
            {
                var oldReg1Value = CPU.GetInstance.Register.GetRegisterValueByName(register1);
                CPU.GetInstance.Register.SetRegisterValueByName(
                    register1,
                     CPU.GetInstance.Register.GetRegisterValueByName(register2) * oldReg1Value);
            }
            else
            {
                CPU.GetInstance.Register.SetRegisterValueByName(
                    register1,
                    CPU.GetInstance.Register.GetRegisterValueByName(register1) * Int32.Parse(register2));
            }
        }

        public void SBOrder(string register1, string register2)
        {
            Console.WriteLine("Rozkaz SB z parametrem " + register1 + " " + register2);
            reporter.AddLog("Rozkaz SB z parametrem " + register1 + " " + register2);
            if (!IsNumeric(register2))
            {
                var oldReg1Value = CPU.GetInstance.Register.GetRegisterValueByName(register1);
                CPU.GetInstance.Register.SetRegisterValueByName(
                    register1,
                     CPU.GetInstance.Register.GetRegisterValueByName(register2) - oldReg1Value);
            }
            else
            {
                CPU.GetInstance.Register.SetRegisterValueByName(
                    register1,
                    CPU.GetInstance.Register.GetRegisterValueByName(register1) - Int32.Parse(register2));
            }
        }

        public void ADOrder(string register1, string register2)
        {
            Console.WriteLine("Rozkaz AD z parametrem " + register1 + " " + register2);
            reporter.AddLog("Rozkaz AD z parametrem " + register1 + " " + register2);
            if (!IsNumeric(register2))
            {
                var oldReg1Value = CPU.GetInstance.Register.GetRegisterValueByName(register1);
                CPU.GetInstance.Register.SetRegisterValueByName(
                    register1,
                     CPU.GetInstance.Register.GetRegisterValueByName(register2) + oldReg1Value);
            }
            else
            {
                CPU.GetInstance.Register.SetRegisterValueByName(
                    register1,
                    CPU.GetInstance.Register.GetRegisterValueByName(register1) + Int32.Parse(register2));
            }

        }

        public bool IsNumeric(string text)
        {
            int n;
            if (int.TryParse(text, out n))
                return true;
            return false;
        }

        public void XZOrder(string processName)
        {
            Console.WriteLine("Rozkaz XZ z parametrem " + processName);
            reporter.AddLog("Rozkaz XZ z parametrem " + processName);

            PCB.GetPCB(processName).TerminateProcess(ReasonOfProcessTerminating.Ended);
        }

        public void XYOrder(string processName)
        {
            Console.WriteLine("Rozkaz XY z parametrem " + processName);
            reporter.AddLog("Rozkaz XY z parametrem " + processName);

            PCB.GetPCB(processName).RunNewProcess();
        }

        public void XNOrder(string processName)
        {
            Console.WriteLine("Rozkaz XN z parametrem " + processName);
            reporter.AddLog("Rozkaz XN z parametrem " + processName);

            var pcbID = PCB.GetPCB(processName).PID;
            CPU.GetInstance.Register.SetRegisterValueByName("A", pcbID);
        }

        public void XSOrder(string processName, string communicate)
        {
            Console.WriteLine("Rozkaz XS z parametrem " + processName + " " + communicate);
            reporter.AddLog("Rozkaz XS z parametrem " + processName + " " + communicate);
            Scheduler.GetInstance.GetRunningPCB().Send(processName, communicate);
        }

        public void XROrder()
        {
            Console.WriteLine("Rozkaz XR");
            reporter.AddLog("Rozkaz XR");
            Scheduler.GetInstance.GetRunningPCB().Receive();
        }

        public void XDOrder(string processName)
        {
            Console.WriteLine("Rozkaz XD z parametrem " + processName);
            reporter.AddLog("Rozkaz XD z parametrem " + processName);

            var flag = PCB.GetPCB(processName).TerminateProcess(ReasonOfProcessTerminating.KilledByOther,
                Scheduler.GetInstance.GetRunningPCB());

            if (flag == 0)
                PCB.GetPCB(processName).RemoveProcess();
        }

        public void XCOrder(string processName, string fileName)
        {
            Console.WriteLine("Rozkaz XC z parametrem " + processName + " " + fileName);
            reporter.AddLog("Rozkaz XC z parametrem " + processName + " " + fileName);

            UserInterface.CreateProcessFromDisc(processName, fileName);
        }

        public void HLTOrder()
        {
            Console.WriteLine("Rozkaz HLT");
            reporter.AddLog("Rozkaz HLT");
            Scheduler.GetInstance.GetRunningPCB().TerminateProcess(ReasonOfProcessTerminating.Ended);

            if (!Scheduler.GetInstance.ListEmpty())
            {
                Scheduler.GetInstance.PriorityAlgorithm().RunReadyProcess();
                Scheduler.GetInstance.RevriteRegistersToCPU();
            }
        }

        public void SchedulerMenager()
        {
            Scheduler.GetInstance.AgingWaitingForProcesorTime();
            Scheduler.GetInstance.RejuvenationCurrentProcess();
            Scheduler.GetInstance.AgingProcessPriority();
            CPU.GetInstance.OrderTime++;
        }
    }
}
