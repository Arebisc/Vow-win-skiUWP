using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Log;

namespace Vow_win_skiUWP.Core.MemoryModule
{
    public class PhysicalMemory
    {
        private Reporter reporter;
        private int _framesCount;
        private int _framesSize;
        private readonly List<Frame> _memory;

        public PhysicalMemory(int framesCount, int framesSize)
        {
            reporter = new Reporter();
            _framesCount = framesCount;
            _framesSize = framesSize;
            _memory = new List<Frame>();
            for (int i = 0; i < _framesCount; i++)
            {
                _memory.Add(new Frame(_framesSize));
            }
        }

        public Frame GetFrame(int index)
        {
            return _memory[index];
        }

        public void ShowMemory()
        {
            Console.WriteLine("Tutaj");
            reporter.AddLog("Tutaj");
            foreach (var frame in _memory)
            {
                frame.ShowFrame();
            }
        }

        public void ShowFrame(int number)
        {
            _memory[number].ShowFrame();
        }

        public void SetFrame(int index, char[] data)
        {
            _memory[index].ClearFrame();
            _memory[index].WriteFrame(data);
        }
    }
}
