using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Core.FileSystem
{
    public class Folder
    {
        public string FolderName { get; private set; }
        public ObservableCollection<File> FilesInDirectory { get; private set; }

        /// <summary>
        /// Creates root folder
        /// </summary>
        public Folder()
        {
            FolderName = "root";
            FilesInDirectory = new ObservableCollection<File>();
        }
    }
}
