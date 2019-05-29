using System;

namespace seunghoon
{
    [Serializable]
    public class BallPositionModel
    {
        public float x;
        public float y;
        public float z;
        public long tick;

        BallPositionModel(float x, float y, float z, long tick)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.tick = tick;
        }
    }
}