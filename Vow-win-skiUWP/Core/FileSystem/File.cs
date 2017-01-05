using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Core.FileSystem
{
    class File
    {
        public string FileName { get; private set; }
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

    }
}
