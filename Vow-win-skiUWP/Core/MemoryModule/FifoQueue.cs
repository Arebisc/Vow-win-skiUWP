using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Annotations;

namespace Vow_win_skiUWP.Core.MemoryModule
{
    public class FrameData
    {
        public int Id;
        public int FrameNumber;
    }
    public class FifoQueue : INotifyPropertyChanged
    {
        private List<FrameData> _queue;
        public int Size;
        private string _queueState;

        public string QueueState
        {
            get
            {
                return _queueState;
            }
            set
            {
                _queueState = value;
                OnPropertyChanged();
            }
        }


        public FifoQueue()
        {
            _queue = new List<FrameData>();
            Size = 0;
        }
        public void AddFrame(FrameData data)
        {
            _queue.Add(data);
            Size = _queue.Count;
        }

        public FrameData RemoveFrame()
        {
            FrameData removeData = _queue[0];
            _queue.RemoveAt(0);
            Size = _queue.Count;
            return removeData;
        }

        public void RemoveChoosenProcess(int id)
        {
            _queue = _queue
                .Select(x => x)
                .Where(x => x.Id != id)
                .ToList();
            Size = _queue.Count;
        }

        public string DisplayQueue()
        {
            var display = _queue.Select(x => x.FrameNumber).Reverse().ToList();
            StringBuilder queue = new StringBuilder();
            //Console.WriteLine("Kolejka FIFO");
        
            foreach (var field in display)
            {
                //Console.Write(field + " ");
                queue.Append(field + "\n");
            }
            return queue.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
