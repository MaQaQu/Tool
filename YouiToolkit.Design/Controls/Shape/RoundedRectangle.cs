using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace YouiToolkit.Design
{
    public class RoundedRectangle : Shape
    {
        public static readonly DependencyProperty RadiusXProperty;
        public static readonly DependencyProperty RadiusYProperty;

        public static readonly DependencyProperty RoundTopLeftProperty;
        public static readonly DependencyProperty RoundTopRightProperty;
        public static readonly DependencyProperty RoundBottomLeftProperty;
        public static readonly DependencyProperty RoundBottomRightProperty;

        public int RadiusX
        {
            get => (int)GetValue(RadiusXProperty);
            set => SetValue(RadiusXProperty, value);
        }

        public int RadiusY
        {
            get => (int)GetValue(RadiusYProperty);
            set => SetValue(RadiusYProperty, value);
        }

        public bool RoundTopLeft
        {
            get => (bool)GetValue(RoundTopLeftProperty);
            set => SetValue(RoundTopLeftProperty, value);
        }

        public bool RoundTopRight
        {
            get => (bool)GetValue(RoundTopRightProperty);
            set => SetValue(RoundTopRightProperty, value);
        }

        public bool RoundBottomLeft
        {
            get => (bool)GetValue(RoundBottomLeftProperty);
            set => SetValue(RoundBottomLeftProperty, value);
        }

        public bool RoundBottomRight
        {
            get => (bool)GetValue(RoundBottomRightProperty);
            set => SetValue(RoundBottomRightProperty, value);
        }

        static RoundedRectangle()
        {
            RadiusXProperty = DependencyProperty.Register("RadiusX", typeof(int), typeof(RoundedRectangle));
            RadiusYProperty = DependencyProperty.Register("RadiusY", typeof(int), typeof(RoundedRectangle));

            RoundTopLeftProperty = DependencyProperty.Register("RoundTopLeft", typeof(bool), typeof(RoundedRectangle), new PropertyMetadata(true));
            RoundTopRightProperty = DependencyProperty.Register("RoundTopRight", typeof(bool), typeof(RoundedRectangle), new PropertyMetadata(true));
            RoundBottomLeftProperty = DependencyProperty.Register("RoundBottomLeft", typeof(bool), typeof(RoundedRectangle), new PropertyMetadata(true));
            RoundBottomRightProperty = DependencyProperty.Register("RoundBottomRight", typeof(bool), typeof(RoundedRectangle), new PropertyMetadata(true));
        }

        public RoundedRectangle()
        {
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                Geometry result = new RectangleGeometry(new Rect(0, 0, Width, Height), RadiusX, RadiusY);
                double halfWidth = Width / 2d;
                double halfHeight = Height / 2d;
                
                if (!RoundTopLeft)
                    result = new CombinedGeometry(GeometryCombineMode.Union, result, new RectangleGeometry(new Rect(0, 0, halfWidth, halfHeight)));
                if (!RoundTopRight)
                    result = new CombinedGeometry(GeometryCombineMode.Union, result, new RectangleGeometry(new Rect(halfWidth, 0, halfWidth, halfHeight)));
                if (!RoundBottomLeft)
                    result = new CombinedGeometry(GeometryCombineMode.Union, result, new RectangleGeometry(new Rect(0, halfHeight, halfWidth, halfHeight)));
                if (!RoundBottomRight)
                    result = new CombinedGeometry(GeometryCombineMode.Union, result, new RectangleGeometry(new Rect(halfWidth, halfHeight, halfWidth, halfHeight)));

                return result;
            }
        }
    }
}
