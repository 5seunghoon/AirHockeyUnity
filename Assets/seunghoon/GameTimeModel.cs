using System;

namespace seunghoon
{
    [Serializable]
    public class GameTimeModel
    {
        public string timeMst;
        public string timeSst;

        GameTimeModel(string timeMst, string timeSst)
        {
            this.timeMst = timeMst;
            this.timeSst = timeSst;
        }

        public override string ToString()
        {
            return timeMst + " : " + timeSst;
        }
    }
}