using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Core.CPU
{
    public sealed class CPU
    {
        private static volatile CPU _instance;
        private static readonly object SyncRoot = new object();
        public Register Register;
        public int OrderTime;

        private CPU()
        {
            this.Register = new Register();
            OrderTime = 0;
        }

        public static CPU GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new CPU();
                    }
                }

                return _instance;
            }
        }
    }
}
