using System;

namespace seunghoon
{
    [Serializable]
    public class BallPositionModel
    {
        public float x;
        public float y;
        public float z;

        BallPositionModel(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}