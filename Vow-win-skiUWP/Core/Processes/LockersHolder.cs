using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Log;

namespace Vow_win_skiUWP.Core.Processes
{
    public sealed class LockersHolder
    {
        private Reporter reporter;
        private static volatile LockersHolder _instance;
        public static Lockers MemoLock;
        public static Lockers ProcLock;

        public static void InitLockers()
        {
            _instance = new LockersHolder();
        }

        private LockersHolder()
        {
            reporter = new Reporter();
            Console.WriteLine();
            Console.WriteLine("Tworzenie mechanizmów synchronizacji LOCKS.");
            reporter.AddLog("\nTworzenie mechanizmów synchronizacji LOCKS.");
            MemoLock = new Lockers();
            ProcLock = new Lockers();
        }

        public static LockersHolder GetInstance => _instance;
    }
}
