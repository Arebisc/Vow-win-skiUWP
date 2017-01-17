using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Core.MemoryModule
{
    public class FreeFramesList
    {
        public int FreeFramesCount;
        public List<int> _freeFrames;

        public FreeFramesList(int framesCount)
        {
            FreeFramesCount = framesCount;
            _freeFrames = new List<int>();
            for (int i = 0; i < framesCount; i++)
                _freeFrames.Add(i);
        }
        public int RemoveFromList()
        {
            int number = _freeFrames[0];
            _freeFrames = _freeFrames
                .Select(x => x)
                .Skip(1)
                .ToList();
            FreeFramesCount = _freeFrames.Count;
            return number;
        }
        public void AddToList(int frameNumber)
        {

            _freeFrames.Add(frameNumber);
            FreeFramesCount = _freeFrames.Count;
        }

        public string DisplayFreeFrames()
        {
            StringBuilder displayStringBuilder = new StringBuilder();
            foreach (var frame in _freeFrames)
            {
                //Console.WriteLine(frame);
                displayStringBuilder.Append(frame+"\n");
            }
            return displayStringBuilder.ToString();
        }
    }
}
