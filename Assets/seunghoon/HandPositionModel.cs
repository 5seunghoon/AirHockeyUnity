using System;

namespace seunghoon
{
    [Serializable]
    public class HandPositionModel
    {
        public float x;
        public float y;
        public float z;

        HandPositionModel(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}