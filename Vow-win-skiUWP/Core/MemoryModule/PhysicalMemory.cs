using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Core.MemoryModule
{
    public class PhysicalMemory
    {
        private int _framesCount;
        private int _framesSize;
        private readonly List<MemoryAllocationUnit> _memory;

        public PhysicalMemory(int framesCount, int framesSize)
        {
            _framesCount = framesCount;
            _framesSize = framesSize;
            _memory = new List<MemoryAllocationUnit>();
            for (int i = 0; i < _framesCount; i++)
            {
                _memory.Add(new MemoryAllocationUnit(_framesSize));
            }
        }

        public MemoryAllocationUnit GetFrame(int index)
        {
            return _memory[index];
        }

        public void ShowMemory()
        {
            Console.WriteLine("Tutaj");
            foreach (var frame in _memory)
            {
                frame.ShowAllocationUnit();
            }
        }

        public string ShowFrame(int number)
        {
            return _memory[number].ShowAllocationUnit();
        }

        public void SetFrame(int index, char[] data)
        {
            _memory[index].ClearAllocationUnit();
            _memory[index].WriteAllocationUnit(data);
        }
    }
}
