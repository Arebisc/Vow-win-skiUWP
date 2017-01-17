using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Core.MemoryModule
{
    class ExchangeFile
    {
        private List<ExchangeFileProcess> _takenProcesses;


        public ExchangeFile()
        {
            _takenProcesses = new List<ExchangeFileProcess>();
        }
        public void PlaceIntoMemory(ExchangeFileProcess data)
        {
            _takenProcesses.Add(data);
        }

        public void RemoveFromMemory(int id)
        {
            _takenProcesses = _takenProcesses
                .Select(x => x)
                .Where(x => x.TakenProcessPages.Id != id)
                .ToList();
        }

        public char[] ReadFromExchangeFile(int id, int pageNumber)
        {
            var data = _takenProcesses.Select(x => x).SingleOrDefault(x => x.TakenProcessPages.Id == id);
            return data?.TakenFrames[pageNumber].ReadAllocationUnit();
        }

        public void UpdateData(int processId, int pageNumber, char[] data)
        {
            foreach (var exchangeFileProcess in _takenProcesses)
            {
                if (exchangeFileProcess.TakenProcessPages.Id == processId)
                {
                    exchangeFileProcess.TakenFrames[pageNumber].WriteAllocationUnit(data);
                }
            }
        }
    }
}
