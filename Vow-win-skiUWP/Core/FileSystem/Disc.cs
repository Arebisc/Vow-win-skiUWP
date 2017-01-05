using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Core.FileSystem
{
    public class Disc
    {
        private static Disc _instance;

        private readonly int _blockSize = 32;
        private static int _numberOfBlocks;
        private Block[] _blocks;
        private BitArray _occupiedBlocksArray;
        private Folder _rootFolder;


        public static void InitDisc(string numberOfBlocks)
        {
            if (int.TryParse(numberOfBlocks, out _numberOfBlocks))
            {
                if (_numberOfBlocks >= 2 && _numberOfBlocks < 255)
                {
                    _instance = new Disc(_numberOfBlocks);
                    return;
                }
            }
            Console.WriteLine("Parametr tworzenia dysku jest nieprawidłowy.");
            _instance = new Disc();
        }

        public static void InitDisc()
        {
            _instance = new Disc();
        }

        public static Disc GetDisc => _instance;

        private Disc()
        {
            Console.WriteLine("Tworzenie dysku z domyślną ilością bloków (32).");
            _numberOfBlocks = 32;
            _blocks = new Block[_numberOfBlocks].Select(b => new Block()).ToArray(); //initialize elements in array
            _occupiedBlocksArray = new BitArray(_numberOfBlocks);
            _rootFolder = new Folder();
        }

        private Disc(int numberOfblocks)
        {
            Console.WriteLine("Tworzenie dysku z niestandardową ilością bloków: " + numberOfblocks);
            _numberOfBlocks = numberOfblocks;
            _blocks = new Block[_numberOfBlocks].Select(b => new Block()).ToArray(); //initialize elements in array
            _occupiedBlocksArray = new BitArray(_numberOfBlocks);
            _rootFolder = new Folder();
        }

        //=================================================================================================================================

        public void ShowDirectory()
        {
            Console.WriteLine("Zawartość folderu root\\\n");
            if (_rootFolder.FilesInDirectory.Count == 0)
            {
                Console.WriteLine("Folder jest pusty.");
                return;
            }

            Console.WriteLine("Nazwa pliku".PadRight(17) + "Rozmiar\t" + "Data utworzenia\t\t" + "Nr bloku indeksowego");
            foreach (var file in _rootFolder.FilesInDirectory)
            {
                Console.WriteLine(file.FileName.PadRight(17) + file.FileSize + " B\t\t" + file.CreationDateTime + "\t" + file.DataBlockPointer);
            }
        }

        //=================================================================================================================================

        public void ShowDataBlocks(string modulo)
        {
            int mod, temp = 0;
            if (!int.TryParse(modulo, out mod))
                mod = _numberOfBlocks;

            int freeblocks = (from bool bit in _occupiedBlocksArray where !bit select bit).Count();
            int occupiedblocks = (from bool bit in _occupiedBlocksArray where bit select bit).Count();

            Console.WriteLine("\nIlość bloków: " + _numberOfBlocks + "\tRozmiar bloku: " + _blockSize + " B");
            Console.WriteLine("Pojemność: " + _numberOfBlocks * _blockSize + " B");
            Console.WriteLine("Wolne miejsce: " + freeblocks * _blockSize + " B (" +
                              (float)freeblocks / _numberOfBlocks * 100 + "%)\tZajęte miejsce: " +
                              occupiedblocks * _blockSize + " B (" + (float)occupiedblocks / _numberOfBlocks * 100 + "%)");
            Console.WriteLine("Wolne bloki: " + freeblocks + "\t\tZajęte bloki: " + occupiedblocks + "\n");

            for (var i = 0; i < _numberOfBlocks; i++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[Blok nr " + i + "]: ");
                Console.Write(_occupiedBlocksArray[i] ? "Zajęty\n" : "Wolny\n");
                Console.ForegroundColor = ConsoleColor.Gray;

                foreach (byte t in _blocks[i].BlockData)
                {
                    Console.Write(t.ToString().PadRight(5));
                }
                temp++;
                if (temp == mod)
                {
                    if (mod == _numberOfBlocks) continue;
                    temp = 0;
                    Console.Write("Naciśnij dowolny klawisz...");
                    Console.ReadKey();
                    Console.WriteLine();
                }
            }
        }

        //=================================================================================================================================
        public bool CreateFile(string nameForNewFile, string data)
        {

            if (data.Any(d => d > 255))
            {
                Console.WriteLine("Błąd: dane zawierają niedozwolony znak");
                return false;
            }

            int newFileSize = data.Length;

            if (_rootFolder.FilesInDirectory.Any(file => file.FileName == nameForNewFile))
            {
                Console.WriteLine("Błąd: Plik \"" + nameForNewFile + "\" już istnieje");
                return false;
            }

            if (nameForNewFile.Length == 0 || nameForNewFile.Length > 15 || nameForNewFile.Contains("\\") ||
                nameForNewFile.Contains("/") || nameForNewFile.Contains("\t") || nameForNewFile == "root")
            {
                Console.WriteLine("Błąd: Nieprawidłowa nazwa pliku");
                Console.WriteLine("Nazwa pliku musi składać się z 1 do 15 znaków i nie może zawierać:");
                Console.WriteLine("'\t' '/' '\\' 'root'");
                return false;
            }

            int blocksneeded = newFileSize / _blockSize + 1;
            if (newFileSize % _blockSize > 0) blocksneeded++;
            if (blocksneeded > _blockSize + 1)
            {
                Console.WriteLine("Błąd: Przekroczono maksymalny rozmiar pliku");
                Console.WriteLine("Wymagany rozmiar: " + newFileSize + " B\tMaksymalny dozwolony rozmiar: " +
                                  _blockSize * _blockSize + " B");
                return false;
            }

            int freeBlocks = (from bool bit in _occupiedBlocksArray where !bit select bit).Count();
            if (blocksneeded > freeBlocks)
            {
                Console.WriteLine("Za mało wolnych bloków by utworzyć nowy plik");
                Console.WriteLine("Wymagane: " + blocksneeded + "\tDostępne: " + freeBlocks);
                return false;
            }

            int[] blocksToBeOccupied = new int[blocksneeded];
            for (int i = 0; i < blocksneeded; i++)
            {
                for (int j = 0; j < _numberOfBlocks; j++)
                {
                    if (_occupiedBlocksArray[j] == false)
                    {
                        blocksToBeOccupied[i] = j;
                        _occupiedBlocksArray[j] = true;
                        break;
                    }
                }
            }

            //fills index block
            _blocks[blocksToBeOccupied[0]].SetBlank();
            for (int i = 0, j = 1; j < blocksneeded; i++, j++)
            {
                _blocks[blocksToBeOccupied[0]].BlockData[i] = Convert.ToByte(blocksToBeOccupied[j]);
            }

            //fills data blocks
            int inBlockPointer = 0, blockPointer = 1;
            while (data != "")
            {
                if (inBlockPointer == _blockSize)
                {
                    inBlockPointer = 0;
                    blockPointer++;
                }
                _blocks[blocksToBeOccupied[blockPointer]].BlockData[inBlockPointer] = Convert.ToByte(data[0]);
                inBlockPointer++;

                data = data.Substring(1);
            }

            _rootFolder.FilesInDirectory.Add(new File(nameForNewFile, newFileSize, blocksToBeOccupied[0]));
            Console.WriteLine("Nowy plik \"" + nameForNewFile + "\" został utworzony w folderze root\\");
            return true;
        }

        //=================================================================================================================================

        public string GetFileData(string fileToOpen)
        {
            if (_rootFolder.FilesInDirectory.All(x => x.FileName != fileToOpen))
            {
                Console.WriteLine("Błąd: Wskazany plik \"" + fileToOpen + "\" nie istnieje w folderze root\\");
                return null;
            }
            int indexBlock = _rootFolder.FilesInDirectory.Single(x => x.FileName == fileToOpen).DataBlockPointer;
            int length = _rootFolder.FilesInDirectory.Single(x => x.FileName == fileToOpen).FileSize;
            List<byte> dataBlocksToRead = new List<byte>();

            dataBlocksToRead.AddRange(_blocks[indexBlock].BlockData.Where(x => x != 255));

            List<byte> dataList = new List<byte>();
            int inBlockPointer = 0, blockPointer = 0;
            while (length != 0)
            {
                if (inBlockPointer == _blockSize)
                {
                    inBlockPointer = 0;
                    blockPointer++;
                }
                dataList.Add(_blocks[dataBlocksToRead[blockPointer]].BlockData[inBlockPointer]);
                inBlockPointer++;

                length--;
            }
            return Encoding.ASCII.GetString(dataList.ToArray());
        }

        //=================================================================================================================================

        public bool DeleteFile(string filenameToDelete)
        {
            if (_rootFolder.FilesInDirectory.All(x => x.FileName != filenameToDelete))
            {
                Console.WriteLine("Błąd: Wskazany plik \"" + filenameToDelete + "\" nie istnieje w folderze root\\");
                return false;
            }
            File fileToDelete = _rootFolder.FilesInDirectory.Single(x => x.FileName == filenameToDelete);

            _occupiedBlocksArray[fileToDelete.DataBlockPointer] = false;
            foreach (var datablocknr in _blocks[fileToDelete.DataBlockPointer].BlockData)
            {
                if (datablocknr == 255) break;
                _occupiedBlocksArray[datablocknr] = false;
            }
            _rootFolder.FilesInDirectory.RemoveAll(x => x.FileName == filenameToDelete);

            Console.WriteLine("Plik \"" + filenameToDelete + "\" został usunięty");
            return true;
        }

        //=================================================================================================================================

        public bool CreateFromWindows(string nameForNewFile, string windowsPath)
        {
            string data;
            try
            {
                data = System.IO.File.ReadAllText(windowsPath);
            }
            catch (Exception e)
            {
                Console.Write("Błąd: " + e.Message);
                return false;
            }
            return CreateFile(nameForNewFile, data);
        }

        //=================================================================================================================================

        public bool AppendToFile(string filenameToAppend, string data)
        {
            if (_rootFolder.FilesInDirectory.All(x => x.FileName != filenameToAppend))
            {
                Console.WriteLine("Błąd: Wskazany plik \"" + filenameToAppend + "\" nie istnieje w folderze root\\");
                return false;
            }
            File fileToAppend = _rootFolder.FilesInDirectory.Single(x => x.FileName == filenameToAppend);

            if (data.Any(d => d > 255))
            {
                Console.WriteLine("Błąd: dane zawierają niedozwolony znak");
                return false;
            }

            int allBytesToAppend = data.Length;
            int blocksNeeded = 0;

            //jezeli ostatni blok jest pelny
            if (fileToAppend.FileSize % _blockSize == 0)
            {
                if (allBytesToAppend > 0)
                {
                    blocksNeeded = 1;
                }
            }

            int bytesNeeded = data.Length - (_blockSize - fileToAppend.FileSize % _blockSize);
            if (bytesNeeded < 0) bytesNeeded = 0;
            blocksNeeded += bytesNeeded / _blockSize;
            if (bytesNeeded % _blockSize > 0) blocksNeeded++;

            int freeBlocks = (from bool bit in _occupiedBlocksArray where !bit select bit).Count();
            if (blocksNeeded > freeBlocks)
            {
                Console.WriteLine("Błąd: Za mało wolnych bloków by dopisać dane do pliku");
                Console.WriteLine("Wymagane: " + blocksNeeded + "\tDostępne: " + freeBlocks);
                return false;
            }

            int freePointers = 32;
            for (var i = 0; _blocks[fileToAppend.DataBlockPointer].BlockData[i] != 255; i++)
            {
                freePointers--;
            }

            if (blocksNeeded > freePointers)
            {
                Console.WriteLine("Błąd: Przekroczono maksymalny rozmiar pliku");
                Console.WriteLine("Wymagany rozmiar: " + fileToAppend.FileSize + data.Length + " B\tMaksymalny dozwolony rozmiar: " + _blockSize * _blockSize + " B");
                return false;
            }

            int[] blocksToBeOccupied = new int[blocksNeeded];
            for (int i = 0; i < blocksNeeded; i++)
            {
                for (int j = 0; j < _numberOfBlocks; j++)
                {
                    if (_occupiedBlocksArray[j] == false)
                    {
                        blocksToBeOccupied[i] = j;
                        _occupiedBlocksArray[j] = true;
                        break;
                    }
                }
            }

            //fills index block
            int inBlockPointer = _blockSize - freePointers;
            foreach (var b in blocksToBeOccupied)
            {
                _blocks[fileToAppend.DataBlockPointer].BlockData[inBlockPointer] = Convert.ToByte(b);
            }

            //fills last data block to the end
            if (fileToAppend.FileSize % 32 != 0)
            {
                inBlockPointer = fileToAppend.FileSize % 32;
                while (inBlockPointer < _blockSize && data.Length > 0)
                {
                    _blocks[_blocks[fileToAppend.DataBlockPointer].BlockData[31 - freePointers]].BlockData[inBlockPointer]
                        = Convert.ToByte(data[0]);
                    data = data.Substring(1);
                    inBlockPointer++;
                }
            }
            //fills new data blocks
            inBlockPointer = 0;
            int blockPointer = 0;
            while (data != "")
            {
                if (inBlockPointer == _blockSize)
                {
                    inBlockPointer = 0;
                    blockPointer++;
                }
                _blocks[blocksToBeOccupied[blockPointer]].BlockData[inBlockPointer] = Convert.ToByte(data[0]);
                inBlockPointer++;

                data = data.Substring(1);
            }
            fileToAppend.Append(allBytesToAppend);
            Console.WriteLine(allBytesToAppend + " B zostało dopisane do pliku \"" + filenameToAppend + "\"");
            return true;
        }

        //=================================================================================================================================
    }
}
