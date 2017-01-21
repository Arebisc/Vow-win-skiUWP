using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Annotations;

namespace Vow_win_skiUWP.Core.Processes
{
    public class Lockers : INotifyPropertyChanged
    {
        private static Lockers _instance;
        private PCB _proces = null;
        private byte open = 0;
        public ObservableCollection<PCB> waiting { get; set; }
        private string Name;

        public static void InitLockers()
        {
            _instance = new Lockers();
        }

        public static Lockers GetInstance() => _instance;



        public PCB proces
        {
            get
            {
                return _proces;
            }
            set
            {
                _proces = value;
                OnPropertyChanged(nameof(proces));
            }
        }

        private Lockers()
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
                Proces.WaitForSomething();
                Proces.InstructionCounter--;
            }
        }

        public void Unlock(string name)
        {
            if (!Check())
            {
                if (waiting.Count() > 0)
                {
                    if (!Check(name))
                    {
                        foreach (var i in waiting)
                        {
                            if (name == i.Name)
                            {
                                i.StopWaiting();
                                waiting.Remove(i);
                                break;
                            }
                        }
                    }
                    else
                    {
                        proces.StopWaiting();
                        proces = waiting[0];
                        waiting.Remove(proces);
                        this.Name = proces.Name;
                    }
                }
                else if (waiting.Count() == 0)
                {
                    if (Check(name))
                    {
                        proces.StopWaiting();
                        this.proces = null;
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
