using System;

namespace seunghoon
{
    [Serializable]
    public class ScoreModel
    {
        public string p1Score;
        public string p2Score;

        ScoreModel(string p1Score, string p2Score)
        {
            this.p1Score = p1Score;
            this.p2Score = p2Score;
        }
        
    }
}