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
    public class File : INotifyPropertyChanged, IEquatable<File>
    {
        private string _fileName;
        private int _fileSize;
        private DateTime _creationDateTime;
        private int _dataBlockPointer;

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        public int FileSize
        {
            get { return _fileSize; }
            private set
            {
                _fileSize = value;
                OnPropertyChanged();
            }
        }
        public DateTime CreationDateTime
        {
            get { return _creationDateTime; }
            private set
            {
                _creationDateTime = value;
                OnPropertyChanged();
            }
        }
        public int DataBlockPointer
        {
            get { return _dataBlockPointer; }
            private set
            {
                _dataBlockPointer = value;
                OnPropertyChanged();
            }
        }


        public File(string name, int size, int dataBlockPointer)
        {
            FileName = name;
            CreationDateTime = DateTime.Now;
            FileSize = size;
            DataBlockPointer = dataBlockPointer;
        }

        public void SetSize(int size)
        {
            FileSize = size;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static bool operator ==(File a, File b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            if (a.FileName == b.FileName)
                return true;
            return false;
        }

        public static bool operator !=(File a, File b)
        {
            return !(a == b);
        }

        public bool Equals(File other)
        {
            return this == other;
        }
    }
}
