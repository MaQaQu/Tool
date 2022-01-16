using System;

namespace SuperMath
{
    public struct GraphPointA : IEquatable<GraphPointA>
    {
        private double x, y, angle;

        public double X
        {
            get => x;
            set => x = value;
        }
        public double Y
        {
            get => y;
            set => y = value;
        }
        public double Angle
        {
            get => angle;
            set => angle = value;
        }

        public GraphPointA(double x, double y, double angle)
        {
            this.x = x;
            this.y = y;
            this.angle = angle;
        }

        public VectorPoint Transform()
        {
            return new VectorPoint(X, Y, angle);
        }

        public bool Equals(GraphPointA other)
        {
            return X == other.X && Y == other.Y && Angle == other.Angle;
        }

        public bool Equals(GraphPoint other)
        {
            return X == other.X && Y == other.Y ;
        }

        public override string ToString()
        {
            return $"{X},{Y},{Angle}";
        }
    }
}
