using System;

namespace seunghoon
{
    [Serializable]
    public class BallPositionModel
    {
        public double x;
        public double y;
        public double z;

        BallPositionModel(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}