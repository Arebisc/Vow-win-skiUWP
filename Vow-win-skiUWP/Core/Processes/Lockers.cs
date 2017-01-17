using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Core.Processes
{
    public class Lockers
    {
        private byte open = 0;
        public ObservableCollection<PCB> waiting { get; set; }
        private string Name;
        public PCB proces { get; set; }

        public Lockers()
        {
            waiting = new ObservableCollection<PCB>();
        }

        public void Lock(PCB Proces)
        {
            if (Check())
            {
                proces = Proces;
                this.Name = proces.Name;
                open = 1;
                proces.WaitForSomething();
                Proces.InstructionCounter--;
            }
            else
            {
                waiting.Add(Proces);
                proces.WaitForSomething();
                Proces.InstructionCounter--;
            }
        }

        public void Unlock(string name)
        {
            if (!Check())
            {
                if (waiting.Count() > 0)
                {
                    if (Check(name))
                    {
                        foreach (var i in waiting)
                        {
                            if (name == i.Name)
                            {
                                proces = i;
                                break;
                            }
                        }
                        proces.StopWaiting();
                        this.Name = proces.Name;
                    }
                }
                else if (waiting.Count() == 0)
                {
                    if (Check(name))
                    {
                        foreach (var i in waiting)
                        {
                            if (name == i.Name)
                            {
                                proces = i;
                                break;
                            }
                        }
                        proces.StopWaiting();
                        open = 0;
                    }
                }
            }
        }

        public void Show()
        {
            if (waiting.Count > 0)
            {
                foreach (var i in waiting)
                {
                    Console.WriteLine(i.PID + "\t" + i.Name);
                }
            }
            else
            {
                Console.WriteLine("Brak procesów pod zamkien.");
            }
        }

        public bool Check()
        {
            if (open == 0)
                return true;
            else
                return false;
        }

        public bool Check(string name)
        {
            if (this.Name == name)
                return true;
            else
                return false;
        }
    }
}
