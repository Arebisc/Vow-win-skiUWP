using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Core.FileSystem
{
    class Block
    {
        private readonly int _blockSize;
        public byte[] BlockData { get; set; }

        public Block()
        {
            _blockSize = 32;
            BlockData = new byte[_blockSize];
            SetBlank();
        }

        /// <summary>
        /// Fills block with maximum value (255)
        /// </summary>
        public void SetBlank()
        {
            for (int i = 0; i < BlockData.Length; i++)
            {
                BlockData[i] = 255;
            }
        }

    }
}
