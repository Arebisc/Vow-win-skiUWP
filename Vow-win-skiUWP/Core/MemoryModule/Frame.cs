using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Core.MemoryModule
{
    public class MemoryAllocationUnit
    {
        private int _frameSize;
        private readonly char[] _fields;
        public int Offset;

        public MemoryAllocationUnit(int frameSize)
        {
            _frameSize = frameSize;
            _fields = new char[_frameSize];
            ClearAllocationUnit();
            Offset = 0;
        }
        public void WriteAllocationUnit(char[] data)
        {
            Offset = data.Length;
            for (int i = 0; i < data.Length; i++)
            {
                _fields[i] = data[i];
            }
        }
        public char[] ReadAllocationUnit()
        {
            return _fields.Take(Offset).ToArray();
        }
        public void ClearAllocationUnit()
        {
            Offset = 0;
            for (int i = 0; i < _fields.Length; i++)
            {
                _fields[i] = '0';
            }
        }

        public string ShowAllocationUnit()
        {
            StringBuilder frame = new StringBuilder();
            for (int i = 0; i < _fields.Length; i++)
            {
                if (_fields[i] == '\r')
                {
                    //Console.Write("\\r ");
                    frame.Append("\\r ");
                }
                else if (_fields[i] == '\n')
                {
                    //Console.Write("\\n ");
                    frame.Append("\\n ");
                }
                else
                {
                    //Console.Write(_fields[i] + " ");
                    frame.Append(_fields[i] + " ");
                }
            }
            //Console.WriteLine("");
            frame.Append("\n");
            return frame.ToString();
        }

        public char GetByte(int index)
        {
            return _fields[index];
        }

        public void ChangeByte(int index, char data)
        {
            _fields[index] = data;
        }
    }
}
