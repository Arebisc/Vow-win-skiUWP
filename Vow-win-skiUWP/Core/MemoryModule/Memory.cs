using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Core.Processes;

namespace Vow_win_skiUWP.Core.MemoryModule
{
    public class Memory
    {
        private const int FramesCount = 16;
        private const int FramesSize = 16;
        private int _messageLength;
        private ExchangeFile _exchangeFile;
        private FifoQueue _fifoQueue;
        private PhysicalMemory _physicalMemory;
        private FreeFramesList _freeFramesList;
        public List<ProcessPages> ProcessPages;

        private static volatile Memory _instance;
        private static object syncRoot = new Object();

        private Memory()
        {
            _exchangeFile = new ExchangeFile();
            _fifoQueue = new FifoQueue();
            _physicalMemory = new PhysicalMemory(FramesCount, FramesSize);
            _freeFramesList = new FreeFramesList(FramesCount );
            ProcessPages = new List<ProcessPages>();
            _messageLength = 0;
        }

        public static Memory GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new Memory();
                        }
                    }
                }
                return _instance;
            }
        }

        private void RemoveFrame(int id, int frameNumber)
        {
            foreach (ProcessPages processPages in ProcessPages)
            {
                if (processPages.Id == id)
                {
                    foreach (Page page in processPages.TakenPages)
                    {
                        if (page.GetFrameNumber() == frameNumber)
                        {
                            page.VaildInVaild = false;
                            if (page.IsModified)
                            {
                                UploadChanges(id, frameNumber,
                                    processPages.TakenPages.IndexOf(page));
                            }
                            break;
                        }
                    }
                    processPages.RemoveFrame(frameNumber);

                    break;
                }
            }
        }

        public void UploadChanges(int id, int frameNumber, int pageNumber)
        {
            //pobranie danych z danej ramki
            MemoryAllocationUnit tempdata = _physicalMemory.GetFrame(frameNumber);
            char[] data = tempdata.ReadAllocationUnit();

            //wpisanie ich do pliku wymiany
            _exchangeFile.UpdateData(id, pageNumber, data);
        }

        private char GetByte(int id, int index)
        {
            //Ustalenie numeru strony i offsetu
            int pages = index / FramesSize;
            int offset = index % FramesSize;
            //Poszukiwanie tablicy stron do danego procesu
            var process = ProcessPages.Select(x => x).SingleOrDefault(x => x.Id == id);
            //sprawdzenie czy dana strona znajduje sie w pamieci
            if (process.IsPageInMemory(pages))
            {
                //Zwrocenie danego bajtu
                return _physicalMemory.GetFrame(process.ReadFrameNumber(pages)).GetByte(offset);
            }
            else
            {
                if (_freeFramesList.FreeFramesCount == 0)
                {
                    //pobranie najstarszego numeru ramki
                    var indexprocessdata = _fifoQueue.RemoveFrame();
                    //przypisanie do tablicy stron z ktorego pobrano ramke ze juz nie ma jej w pamieci
                    RemoveFrame(indexprocessdata.Id, indexprocessdata.FrameNumber);
                    //dodanie ramki do list
                    _freeFramesList.AddToList(indexprocessdata.FrameNumber);
                    //pobranie z listy wolnych ramek najstarszej ramki
                    int newFrameIndex = _freeFramesList.RemoveFromList();
                    //dodanie do kolejki FIFO nowego numeru ramki
                    _fifoQueue.AddFrame(new FrameData()
                    {
                        FrameNumber = indexprocessdata.FrameNumber,
                        Id = id
                    });
                    //wpisanie do pamieci nowej strony
                    _physicalMemory.SetFrame(newFrameIndex, _exchangeFile.ReadFromExchangeFile(id, pages));
                    //wpisanie do tablicy stron,ze strona znajduje sie w pamieci
                    foreach (var processPage in ProcessPages)
                    {
                        if (processPage.Id == id)
                            processPage.AddFrame(pages, newFrameIndex);
                    }
                    //zwrocenie bajtu
                    return _physicalMemory.GetFrame(newFrameIndex).GetByte(offset);
                }
                else
                {
                    //pobranie nowej strony
                    var newFrameIndex = _freeFramesList.RemoveFromList();
                    //dodanie do kolejki FIFO nowego numeru ramki
                    _fifoQueue.AddFrame(new FrameData()
                    {
                        FrameNumber = newFrameIndex,
                        Id = id
                    });
                    //wpisanie do pamieci nowej strony
                    _physicalMemory.SetFrame(newFrameIndex, _exchangeFile.ReadFromExchangeFile(id, pages));
                    //wpisanie do tablicy stron,ze strona znajduje sie w pamieci
                    foreach (var processPage in ProcessPages)
                    {
                        if (processPage.Id == id)
                            processPage.AddFrame(pages, newFrameIndex);
                    }
                    //zwrocenie bajtu
                    return _physicalMemory.GetFrame(newFrameIndex).GetByte(offset);


                }
            }
        }

        public void AllocateMemory(PCB processData, string program)
        {
            //obliczenie ilosci stron 
            int pagesCount = (int)Math.Ceiling((double)program.Length / FramesSize);

            //przypisnie Stron procesu i delegatow do Listy stron i do PCB
            ProcessPages temp = new ProcessPages(processData.PID, pagesCount);
            temp.GetChar = GetByte;
            temp.ChangeByteDel = ChangeByte;
            ProcessPages.Add(temp);
            processData.MemoryBlocks = temp;
            processData.MaxMemory = program.Length - 1;

            //uzupelnienie stron
            var pages = new List<MemoryAllocationUnit>();
            for (int i = 0; i < pagesCount; i++)
            {
                pages.Add(new MemoryAllocationUnit(FramesSize));
                pages[i].WriteAllocationUnit(program.Select(x => x)
                    .Skip(FramesSize * i)
                    .Take((program.Length - FramesSize * i < FramesSize) ? program.Length - FramesSize * i : FramesSize)
                    .ToArray());
            }

            //Dodanie danych do pliku wymiany
            ExchangeFileProcess newProcess = new ExchangeFileProcess()
            {
                TakenProcessPages = temp,
                TakenFrames = pages
            };
            _exchangeFile.PlaceIntoMemory(newProcess);

            //sprawdzenie czy mozna dodac pierwsza strone do pamieci
            if (_freeFramesList.FreeFramesCount >= 1)
            {
                //przypisanie indexu ramki do ktorej zostanie wprowadzona strona
                int index = _freeFramesList.RemoveFromList();
                //dodanie numeru ramki do kolejki 
                _fifoQueue.AddFrame(new FrameData()
                {
                    FrameNumber = index,
                    Id = processData.PID
                });
                //wprowadzenie 0 strony do pamieci fizycznej
                _physicalMemory.SetFrame(index, pages[0].ReadAllocationUnit());
                //wprowadzenie do tablicy stron ze strona 0 znajduje sie w danym miejscu
                foreach (ProcessPages process in ProcessPages)
                {
                    if (process.Id == processData.PID)
                        process.AddFrame(0, index);
                }
            }
            else
            {
                //zdjecie z kolejki ramki w ktorej znajduje sie najstarsza strona
                var indexprocessdata = _fifoQueue.RemoveFrame();
                //wpisanie do tablicy stron ze dana strona zostala zdjeta
                RemoveFrame(indexprocessdata.Id, indexprocessdata.FrameNumber);
                //dodanie do listy wolnych ramek
                _freeFramesList.AddToList(indexprocessdata.FrameNumber);
                //zabranie ze listy wolnych ramek pierwszej wolnej ramki
                int index = _freeFramesList.RemoveFromList();
                //dodanie strony do kolejki 
                _fifoQueue.AddFrame(new FrameData()
                {
                    FrameNumber = index,
                    Id = processData.PID
                });
                //wprowadzenie do pamieci fizycznej danej strony
                _physicalMemory.SetFrame(index, pages[0].ReadAllocationUnit());
                //wpisanie do tablicy stron ze dana strona znajduje sie w pamieci
                foreach (var processPage in ProcessPages)
                {
                    if (processPage.Id == processData.PID)
                        processPage.AddFrame(0, index);
                }
            }
        }

        public void RemoveFromMemory(PCB processData)
        {
            //tablica ramek do ktorych zostaly wpisane strony
            int[] frames = null;
            //przypisanie do nich wartosci
            for (int i = 0; i < ProcessPages.Count; i++)
            {
                if (ProcessPages[i].Id == processData.PID)
                {
                    frames = ProcessPages[i].ReadFrameNumbers();
                    ProcessPages.RemoveAt(i);
                    break;
                }
            }
            //zabezpieczenie przed exceptionem
            if (frames != null)
            {
                //wprowadzenie zwolnionych ramek do listy wolnych ramek
                //oczyszczenie ramek w pamieci fizycznej
                foreach (var frame in frames)
                {
                    _freeFramesList.AddToList(frame);
                    //_physicalMemory.GetFrame(frame).ClearFrame();
                }
            }
            //usuniecie z kolejki procesu
            _fifoQueue.RemoveChoosenProcess(processData.PID);
            //usuniecie danych z pliku wymiany
            _exchangeFile.RemoveFromMemory(processData.PID);
        }

        public void ChangeByte(int id, int index, char data)
        {
            //Ustalenie numeru strony i offsetu
            int pages = index / FramesSize;
            int offset = index % FramesSize;
            //Poszukiwanie tablicy stron do danego procesu
            var process = ProcessPages.Select(x => x).SingleOrDefault(x => x.Id == id);
            //sprawdzenie czy dana strona znajduje sie w pamieci
            if (process.IsPageInMemory(pages))
            {
                //Zwrocenie danego bajtu
                process.SetModified(pages);
                _physicalMemory.GetFrame(process.ReadFrameNumber(pages)).ChangeByte(offset, data);

            }
            else
            {
                if (_freeFramesList.FreeFramesCount == 0)
                {
                    //pobranie najstarszego numeru ramki
                    var indexprocessdata = _fifoQueue.RemoveFrame();
                    //przypisanie do tablicy stron z ktorego pobrano ramke ze juz nie ma jej w pamieci
                    RemoveFrame(indexprocessdata.Id, indexprocessdata.FrameNumber);
                    //dodanie ramki do list
                    _freeFramesList.AddToList(indexprocessdata.FrameNumber);
                    //pobranie z listy wolnych ramek najstarszej ramki
                    int newFrameIndex = _freeFramesList.RemoveFromList();
                    //dodanie do kolejki FIFO nowego numeru ramki
                    _fifoQueue.AddFrame(new FrameData()
                    {
                        FrameNumber = indexprocessdata.FrameNumber,
                        Id = id
                    });
                    //wpisanie do pamieci nowej strony
                    _physicalMemory.SetFrame(newFrameIndex, _exchangeFile.ReadFromExchangeFile(id, pages));
                    //wpisanie do tablicy stron,ze strona znajduje sie w pamieci
                    foreach (var processPage in ProcessPages)
                    {
                        if (processPage.Id == id)
                        {
                            processPage.AddFrame(pages, newFrameIndex);
                            processPage.SetModified(pages);
                        }
                    }
                    //zwrocenie bajtu

                    _physicalMemory.GetFrame(newFrameIndex).ChangeByte(offset, data);
                }
                else
                {
                    //pobranie nowej strony
                    var newFrameIndex = _freeFramesList.RemoveFromList();
                    //dodanie do kolejki FIFO nowego numeru ramki
                    _fifoQueue.AddFrame(new FrameData()
                    {
                        FrameNumber = newFrameIndex,
                        Id = id
                    });
                    //wpisanie do pamieci nowej strony
                    _physicalMemory.SetFrame(newFrameIndex, _exchangeFile.ReadFromExchangeFile(id, pages));
                    //wpisanie do tablicy stron,ze strona znajduje sie w pamieci
                    foreach (var processPage in ProcessPages)
                    {
                        if (processPage.Id == id)
                        {
                            processPage.AddFrame(pages, newFrameIndex);
                            //ustawienie ze ramka byla modyfikowana
                            processPage.SetModified(pages);
                        }
                    }
                    //zwrocenie bajtu
                    _physicalMemory.GetFrame(newFrameIndex).ChangeByte(offset, data);
                }
            }
        }

        public string DisplayPageList(int id)
        {
            try
            {
                StringBuilder pageList = new StringBuilder();
                ProcessPages pages = ProcessPages.SingleOrDefault(x => x.Id == id);

                for (int i = 0; i < pages.PagesCount; i++)
                {
                    if (pages.IsPageInMemory(i))
                    {
                        Console.WriteLine("Strona " + i + " znajduje się w ramce nr " + pages.ReadFrameNumber(i));
                        pageList.Append("Strona " + i + " znajduje się w ramce nr " + pages.ReadFrameNumber(i)+"\n");
                    }
                    else
                    {
                        //Console.WriteLine("Strona " + i + " nie ma przypisanej ramki.");
                        pageList.Append("Strona " + i + " nie ma przypisanej ramki.\n");
                    }
                }
                return pageList.ToString();
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Tego procesu nie ma w pamięci");
                return "Tego procesu nie ma w pamięci.\n";
            }
        }

        public string DisplayPageContent(int id, int number)
        {
            try
            {
                ProcessPages pages = ProcessPages.FirstOrDefault(x => x.Id == id);
                StringBuilder pageContent = new StringBuilder();

                if (pages.IsPageInMemory(number))
                {
                    //Console.WriteLine("Zawarość ramki nr: " + number);
                    //pageContent.Append("Zawartość ramki nr " + number+": ");
                    pageContent.Append(_physicalMemory.GetFrame(pages.ReadFrameNumber(number)).ShowAllocationUnit()+"\n");
                    return pageContent.ToString();
                }
                else
                {
                    //Console.WriteLine("Danej strony nie ma w pamięci.");
                    pageContent.Append("Danej strony nie ma w pamięci.\n");
                    return pageContent.ToString();
                }

            }
            catch (Exception)
            {
                //Console.WriteLine("Nie ma danego procesu w pamieci.");
                return "Nie ma danego procesu w pamieci.\n";
            }
        }

        public string DisplayFreeFrames()
        {
            if (_freeFramesList.FreeFramesCount == 0)
            {
                //Console.WriteLine("Brak wolnych ramek.");
                return "";
            }
            else
            {
                //Console.WriteLine("Lista wolnych ramek.");
                return _freeFramesList.DisplayFreeFrames();
            }
        }

        public string DisplayPhysicalMemory()
        {
            StringBuilder physicalMemory = new StringBuilder();
            //Console.WriteLine("Wyświetlenie całej pamięci.");
            for (int i = 0; i < FramesCount; i++)
            {
                //Console.Write("Ramka nr " + i + ": ");
               // physicalMemory.Append("Ramka nr " + i + ": ");
                physicalMemory.Append(_physicalMemory.ShowFrame(i));
                physicalMemory.Append("\n");
            }
            return physicalMemory.ToString();
        }

        public string DisplayFifoQueue()
        {
            return _fifoQueue.DisplayQueue();
        }

        public void TestFillMemory(PCB testProcessData)
        {
            string data = "abcdefghijklmnop" +
                         "rstuvwxyzabcdefg" +
                         "hijklmnoprstuvwx" +
                         "yzabcdefghijklmn" +
                         "oprstuvwxyzabcde" +
                         "fghijklmnoprstuv" +
                         "wxyzabcdefghijkl" +
                         "mnoprstuvwxyzabc" +
                         "defghijklmnoprst" +
                         "uvwxyzabcdefghij" +
                         "klmnoprstuvwxyza" +
                         "bcdefghijklmnopr" +
                         "stuvwxyzabcdefgh" +
                         "ijklmnoprstuvwxy";
            AllocateMemory(testProcessData, data);
            for (int i = 0; i < FramesCount - 2; i++)
            {
                testProcessData.MemoryBlocks.ReadByte(i * FramesSize);
            }

        }

        public void TestCleanMemory()
        {
            //przejscie po liscie list stron procesow
            for (int i = 0; i < ProcessPages.Count; i++)
            {
                //przypisanie zajetych ramek przez dany proces
                var frames = ProcessPages[i].ReadFrameNumbers();

                //jezeli nie zajmuje nic przejdz dalej
                if (frames == null) continue;
                //usuniecie danych ramek ze wszyskich miejsc
                foreach (var frame in frames)
                {
                    _freeFramesList.AddToList(frame);
                    _physicalMemory.GetFrame(frame).ClearAllocationUnit();
                    _fifoQueue.RemoveChoosenProcess(ProcessPages[i].Id);
                    _exchangeFile.RemoveFromMemory(ProcessPages[i].Id);
                }
            }
            //wyczyszczenie listy procesow
            ProcessPages = new List<ProcessPages>();
        }

        public void PlaceMessage(string message)
        {
            if (message.Length > 32)
            {
                message = message.Take(2 * FramesSize).ToString();
            }
            //wpisanie dlugosci wiadomosci
            _messageLength = message.Length;


            //dane do zapelnienia nieuzywanej ramki
            //wprowadzenie tylko do jednej strony
            if (_messageLength < 16)
            {
                _physicalMemory.SetFrame(FramesCount - 2,
                    message.Take(_messageLength).ToArray());
                _physicalMemory.GetFrame(FramesCount - 1).ClearAllocationUnit();
                // Console.WriteLine(message.Take(_messageLength).ToArray());

            }
            //wprowadzenie do 2 stron
            else
            {
                _physicalMemory.SetFrame(FramesCount - 2,
                    message.Take(16).ToArray());
                _physicalMemory.SetFrame(FramesCount - 1,
                    message.Skip(16).Take(_messageLength - 16).ToArray());
                //  Console.WriteLine(message.Take(16).ToArray());
                // Console.WriteLine(message.Skip(16).Take(_messageLength - 16).ToArray());
            }
        }

        public string ReadMessage()
        {
            if (_messageLength > 0)
            {
                string temp = new string(_physicalMemory.GetFrame(FramesCount - 2).ReadAllocationUnit());
                string temp1 = new string(_physicalMemory.GetFrame(FramesCount - 1).ReadAllocationUnit());

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(temp);
                stringBuilder.Append(temp1);

                return stringBuilder.ToString();
            }
            else
            {
                return "Nie ma jeszcze wiadomosci w pamięci.";
            }

        }


    }
}
