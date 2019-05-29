using System;

namespace seunghoon
{
    [Serializable]
    public class GameEndModel
    {
        public string winner;

        GameEndModel(string winner)
        {
            this.winner = winner;
        }

        public bool IsDraw()
        {
            if (winner != null && winner == "DRAW") return true;
            else return false;
        }
        
    }
}