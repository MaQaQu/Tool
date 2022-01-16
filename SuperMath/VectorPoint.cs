using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMath
{
    public class VectorPoint
    {
        private double _X, _Y, _Angle;

        public double X { get { return _X; } set { _X = value; } }
        public double Y { get { return _Y; } set { _Y = value; } }
        public double Angle { get { return _Angle; } set { _Angle = value; } }

        public VectorPoint(double x, double y)
            : this(x, y, 0)
        {

        }

        public VectorPoint(double x, double y, double angle)
        {
            _X = x;
            _Y = y;
            _Angle = angle;
        }

        public GraphPoint Transform()
        {
            return new GraphPoint(X, Y);
        }

        public bool Equals(VectorPoint other)
        {
            return X == other.X & Y == other.Y & Angle == other.Angle;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}, {2}", X, Y);
        }
    }
}
