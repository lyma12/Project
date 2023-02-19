

using System;
using System.Windows;

namespace Tic_Tac_Toe
{
    public class GameState
    {
        private int turnPass { get; set; }
        public Player winer { get; private set; }
        public WinType inForWingame { get; private set; }
        public Player playerCurrent { get; private set; }
        public Player[,] playerGrid { get; private set; }
        public event Action<int, int> OnMoveMadeState;
        public event Action<WinType, int , int> EndGame;
        public event Action GameNoWin;
     

        public GameState()
        {
            turnPass = 0;
           
            playerCurrent = Player.X;
            playerGrid = new Player[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    playerGrid[i, j] = Player.none;
                }
            }
        }

        public void Reset()
        {
            turnPass = 0;
            playerCurrent = Player.X;
            playerGrid = new Player[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    playerGrid[i, j] = Player.none;
                }
            }
        }


        public void GameOver(int r, int c)
        {
            if (GameWin(r, c))
            {
                winer = playerGrid[r, c];
                EndGame?.Invoke(this.inForWingame, r, c);
                
            }
            else if (turnPass == 9)
            {
                winer = Player.none;
                GameNoWin?.Invoke();
                
            }
            
            
            return;
            
        }


        private bool GameWin(int r, int c)
        {
            if (playerGrid[0, c] == playerGrid[1, c] && playerGrid[1, c] == playerGrid[2, c] && playerGrid[0,c] != Player.none)
            {
                this.inForWingame = WinType.columns;
                return true;
            }
            if (playerGrid[r, 0] == playerGrid[r, 1] && playerGrid[r, 2] == playerGrid[r, 1]&& playerGrid[r,0] != Player.none)
            {
                this.inForWingame = WinType.row;
                return true;
            }
            if (playerGrid[0, 0] == playerGrid[1, 1] && playerGrid[1, 1] == playerGrid[2, 2]&& playerGrid[0, 0] != Player.none)
            {
                this.inForWingame = WinType.diag1;
                return true;
            }
            if (playerGrid[0, 2] == playerGrid[1, 1] && playerGrid[1, 1] == playerGrid[2, 0]&& playerGrid[0, 2] != Player.none)
            {
                this.inForWingame = WinType.diag2;
                return true;
            }
            return false;
        }

        public void MakeMove(int r, int c)
        {
            if (playerGrid[r, c] == Player.none )
            {
                playerGrid[r, c] = playerCurrent;
                playerCurrent = playerCurrent == Player.X ? Player.O : Player.X;
                turnPass++;
                OnMoveMadeState?.Invoke(r, c);
                GameOver(r, c);
            }
        }

    }
}

