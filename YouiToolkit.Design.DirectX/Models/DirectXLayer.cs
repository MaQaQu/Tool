using SuperMath;
using System.Windows;
using System.Windows.Input;
using PointF = System.Drawing.PointF;

namespace YouiToolkit.Design.DirectX
{
    public abstract class DirectXLayer
    {
        public Matrix ToLayerMatrix = null;
        public Matrix FromLayerMatrix = null;

        public DirectXLayer()
        {
            ToLayerMatrix = new Matrix(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });
            FromLayerMatrix = Matrix.Inverse(ToLayerMatrix);
        }

        public PointF ToLayerPoint(Point src)
        {
            Matrix input = new Matrix(new double[,] { { src.X }, { src.Y }, { 1 } });
            Matrix output = ToLayerMatrix * input;
            return new PointF((float)output[0, 0], (float)output[1, 0]);
        }

        public Point FromLayerPoint(PointF src)
        {
            Matrix input = new Matrix(new double[,] { { src.X }, { src.Y }, { 1 } });
            Matrix output = FromLayerMatrix * input;
            return new Point((float)output[0, 0], (float)output[1, 0]);
        }

        public virtual void UpdateTransMatrix(Matrix toLayerMatrix, Matrix fromLayerMatrix)
        {
            ToLayerMatrix = toLayerMatrix;
            FromLayerMatrix = fromLayerMatrix;
        }

        public virtual bool OnGotFocus() => false;

        public virtual bool OnLostFocus() => false;

        public virtual bool OnMouseMove(Point p) => false;

        public virtual bool OnMouseEnter(Point p) => false;

        public virtual bool OnMouseLeave(Point p) => false;

        public virtual bool OnMouseDown(Point p, bool leftPressed, bool middlePressed, bool rightPressed, int clickCount = 1) => false;

        public virtual bool OnMouseUp(Point p, bool leftReleased, bool middleReleased, bool rightReleased) => false;

        public virtual bool OnMouseWheel(Point p, int delta) => false;

        public virtual bool OnKeyDown(Key key, ModifierKeys modifiers) => false;

        public virtual bool OnKeyUp(Key key, ModifierKeys modifiers) => false;
    }
}
