using SuperMath;

namespace YouiToolkit.Assist
{
    public class AssistPoseGraph
    {
        public int Index { get; private set; }
        public int Count { get; private set; }
        public GraphPoint[] BasePoints { get; private set; }
        public GraphPoint[] MapPoints { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double A { get; private set; }
        public int CurrVersion { get; set; }
        public int MapVersion { get; set; }

        public AssistPoseGraph(int index)
        {
            Index = index;
            Count = 0;
            BasePoints = null;
            MapPoints = null;
            X = 0;
            Y = 0;
            A = 0;
            CurrVersion = -1;
            MapVersion = -1;
        }

        public bool SetGraphCount(int count)
        {
            if (Count != count)
            {
                Count = count;
                BasePoints = new GraphPoint[Count];
                MapPoints = new GraphPoint[Count];
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetGraphPose(double x, double y, double a)
        {
            if (X != x || Y != y || A != a)
            {
                X = x;
                Y = y;
                A = a;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
