using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMath
{
    public struct GraphPoint : IEquatable<GraphPoint>
    {
        private double _X, _Y;

        public double X { get { return _X; } set { _X = value; } }
        public double Y { get { return _Y; } set { _Y = value; } }

        public GraphPoint(double x, double y)
        {
            _X = x;
            _Y = y;
        }

        public VectorPoint Transform(double angle)
        {
            return new VectorPoint(X, Y, angle);
        }

        public bool Equals(GraphPoint other)
        {
            return X == other.X & Y == other.Y;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", X, Y);
        }
    }
}
