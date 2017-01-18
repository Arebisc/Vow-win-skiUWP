using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Annotations;

namespace Vow_win_skiUWP.Core.FileSystem
{
    public class File : INotifyPropertyChanged
    {
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        public int FileSize { get; private set; }
        public DateTime CreationDateTime { get; private set; }
        public int DataBlockPointer { get; private set; }


        public File(string name, int size, int dataBlockPointer)
        {
            FileName = name;
            CreationDateTime = DateTime.Now;
            FileSize = size;
            DataBlockPointer = dataBlockPointer;
        }

        public void Append(int size)
        {
            FileSize += size;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
