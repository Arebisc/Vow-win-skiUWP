using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Core.MemoryModule
{
    public class Frame
    {
        private int _frameSize;
        private readonly char[] _fields;
        public int Offset;

        public Frame(int frameSize)
        {
            _frameSize = frameSize;
            _fields = new char[_frameSize];
            ClearFrame();
            Offset = 0;
        }
        public void WriteFrame(char[] data)
        {
            Offset = data.Length;
            for (int i = 0; i < data.Length; i++)
            {
                _fields[i] = data[i];
            }
        }
        public char[] ReadFrame()
        {
            return _fields.Take(Offset).ToArray();
        }
        public void ClearFrame()
        {
            Offset = 0;
            for (int i = 0; i < _fields.Length; i++)
            {
                _fields[i] = '0';
            }
        }

        public void ShowFrame()
        {
            for (int i = 0; i < _fields.Length; i++)
            {
                if (_fields[i] == '\r')
                {
                    Console.Write("\\r ");
                }
                else if (_fields[i] == '\n')
                {
                    Console.Write("\\n ");
                }
                else
                {
                    Console.Write(_fields[i] + " ");
                }
            }
            Console.WriteLine("");
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
