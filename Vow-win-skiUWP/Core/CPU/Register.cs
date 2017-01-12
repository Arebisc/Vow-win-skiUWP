using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Log;

namespace Vow_win_skiUWP.Core.CPU
{
    public class Register : IEquatable<Register>, INotifyPropertyChanged
    {
        private Reporter reporter;
        private int _A;
        private int _B;
        private int _C;
        private int _D;

        public int A
        {
            get { return _A; }
            set
            {
                _A = value;
                OnPropertyChanged("A");
            }
        }

        public int B
        {
            get { return _B; }
            set
            {
                _B = value;
                OnPropertyChanged("B");
            }
        }

        public int C
        {
            get { return _C; }
            set
            {
                _C = value;
                OnPropertyChanged("C");
            }
        }

        public int D
        {
            get { return _D; }
            set
            {
                _D = value;
                OnPropertyChanged("D");
            }
        }

        public Register()
        {
            A = 0;
            B = 0;
            C = 0;
            D = 0;
            reporter = new Reporter();
        }

        public Register(int a, int b, int c, int d)
            : this()
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public Register(Register register)
            : this()
        {
            A = register.A;
            B = register.B;
            C = register.C;
            D = register.D;
        }

        public override string ToString()
        {
            String result = String.Format("A: {0}; B: {1}; C: {2}; D: {3}", A, B, C, D);

            return result;
        }

        public void PrintRegisters()
        {
            Console.WriteLine(this.ToString());
            reporter.AddLog(this.ToString());
        }

        public static bool operator ==(Register a, Register b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            if (a.A == b.A)
                if (a.B == b.B)
                    if (a.C == b.C)
                        if (a.D == b.D)
                            return true;
            return false;
        }

        public static bool operator !=(Register a, Register b)
        {
            return !(a == b);
        }

        public bool Equals(Register other)
        {
            return this == other;
        }

        public PropertyInfo GetRegisterByName(string name)
        {
            return CPU.GetInstance.Register.GetType().GetProperty(name);
        }

        public int GetRegisterValueByName(string name)
        {
            return (int)(GetRegisterByName(name).GetValue(CPU.GetInstance.Register));
        }

        public void SetRegisterValueByName(string name, int value)
        {
            GetRegisterByName(name).SetValue(CPU.GetInstance.Register, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
