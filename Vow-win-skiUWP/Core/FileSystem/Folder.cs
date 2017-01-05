using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Core.FileSystem
{
    class Folder
    {
        public string FolderName { get; private set; }
        public List<File> FilesInDirectory { get; private set; }

        /// <summary>
        /// Creates root folder
        /// </summary>
        public Folder()
        {
            FolderName = "root";
            FilesInDirectory = new List<File>();
        }
    }
}
