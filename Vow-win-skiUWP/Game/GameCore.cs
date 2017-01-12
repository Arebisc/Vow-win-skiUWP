using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Game
{
    public class GameCore
    {
        private static GameCore gameInstance;
        private int[,] gameBoard;
        private int circle = 1, sharp = 0;
        private int currentMove = 1;
        private int currentRound = 1;
        private int currentPlayer = 0;

        public static GameCore GetInstance
        {
            get
            {
                if (gameInstance == null)
                {
                    gameInstance = new GameCore();
                }
                return gameInstance;
            }
        }

        private GameCore()
        {
            GameBoardInit();
        }


        public void GameBoardInit()
        {
            gameBoard = new int[3, 3];
            ClearBoard();
        }

        public int SetBoardsValue(int[] position)
        {
            if (currentMove % 2 != 0)
            {
                gameBoard[position[0], position[1]] = circle;
                return circle;
            }
            else
            {
                gameBoard[position[0], position[1]] = sharp;
                return sharp;
            }
        }

        public void ChangeCurrentPlayer()
        {
            currentPlayer = currentPlayer == circle ? sharp : circle;
        }

        public void GameMain()
        {

        }

        public void EndGame()
        {
            int horizonWin = 0;
            int verticalWin = 0;
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    horizonWin += gameBoard[i, j];
                    verticalWin += gameBoard[j, i];
                }

                if (horizonWin == 0 | horizonWin == 3 | verticalWin ==0 | verticalWin ==3)
                {
                    break;
                }
                else
                {
                    horizonWin = 0;
                    verticalWin = 0;
                }
            }
        }

        public void ClearBoard()
        {
            for (var i = 0; i < gameBoard.Length; i++)
            {
                for (var j = 0; j < gameBoard.Length; j++)
                {
                    gameBoard[i, j] = 2;
                }
            }
        }
    }
}
