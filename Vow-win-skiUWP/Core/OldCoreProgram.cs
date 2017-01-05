using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Core.FileSystem;
using Vow_win_skiUWP.Core.IPC;
using Vow_win_skiUWP.Core.Processes;

namespace Vow_win_skiUWP.Core
{
    class OldCoreProgram
    {
        static void InitSystemResources(string[] args)
        {
            LockersHolder.InitLockers();
            PipeServer.InitServer();
            PCB.CreateIdleProcess();
            if (args.Length > 0)
                Disc.InitDisc(args[0]);
            else
                Disc.InitDisc();
        }


        static void CoreMain(string[] args)
        {
            Console.WriteLine("Uruchamianie systemu...");
            Directory.SetCurrentDirectory(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString());

            InitSystemResources(args);

            try
            {
                Shell.GetShell.OpenShell();
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + ("Coś... coś się popsuło :(".Length / 2)) + "}", "Coś... coś się popsuło :("));
                Console.WriteLine();
                Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + ((e.GetType() + ":").Length / 2)) + "}", e.GetType() + ":"));
                Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (e.Message.Length / 2)) + "}", e.Message));
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.Write(String.Format("{0," + ((Console.WindowWidth / 2) + ("Naciśnij dowolny klawisz...".Length / 2)) + "}", "Naciśnij dowolny klawisz..."));
                Console.ReadKey();
                Console.ResetColor();
                Console.Clear();
            }
        }
    }
}
