using System.Windows.Controls;
using System.Windows.Media;

namespace YouiToolkit.Design
{
    public static class BrushUtils
    {
        public static Brush GetBrush(string color)
        {
            BrushConverter converter = new BrushConverter();
            return (converter.ConvertFromString(color) as Brush)!;
        }

        public static Brush GetBrushHighlight(Brush brush, int level = 20)
        {
            byte red = 0xFF;
            byte green = 0xFF;
            byte blue = 0xFF;

            if (brush is SolidColorBrush solidBrush)
            {
                red = (byte)(solidBrush.Color.R + level);
                green = (byte)(solidBrush.Color.G + level);
                blue = (byte)(solidBrush.Color.B + level);
            }
            return new SolidColorBrush(Color.FromRgb(red, green, blue));
        }

        public static VisualBrush GetVisualBrush(Brush background, Brush foreground, double opacity)
        {
            return new VisualBrush(new Border()
            {
                Width = 1,
                Height = 1,
                Background = background,

                Child = new Border()
                {
                    Background = foreground,
                    Opacity = opacity
                }
            });
        }
        public static SolidColorBrush ToBrush(this Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

        public static SolidColorBrush ToBrush(this Color? color)
        {
            if (color == null)
                return new SolidColorBrush(Colors.Transparent);
            return new SolidColorBrush((Color)color);
        }

        public static Color ToColor(this string color)
        {
            return (Color)ColorConverter.ConvertFromString(color);
        }

        public static Color ToColor(this SolidColorBrush brush)
        {
            if (brush == null)
                return Colors.Transparent;
            return brush.Color;
        }

        public static Color ToColor(this Brush brush)
        {
            if (brush == null)
                return Colors.Transparent;
            if (brush is SolidColorBrush)
                return (brush as SolidColorBrush).Color;
            else if (brush is LinearGradientBrush)
                return (brush as LinearGradientBrush).GradientStops[0].Color;
            else if (brush is RadialGradientBrush)
                return (brush as RadialGradientBrush).GradientStops[0].Color;
            else
                return Colors.Transparent;
        }

        public static string ToHexString(this Color color, bool withAlpha = true)
        {
            if (withAlpha)
                return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
            else
                return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        public static string ToHexString(this Color? color, bool withAlpha = true)
        {
            if (color == null)
                return string.Empty;

            var realColor = (Color)color;
            if (withAlpha)
                return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", realColor.A, realColor.R, realColor.G, realColor.B);
            else
                return string.Format("#{0:X2}{1:X2}{2:X2}", realColor.R, realColor.G, realColor.B);
        }

        public static LinearGradientBrush ToBrush(this Color[] colors)
        {
            var lcb = new LinearGradientBrush();
            for (int i = 0; i < colors.Length; i++)
            {
                lcb.GradientStops.Add(new GradientStop()
                {
                    Color = colors[i],
                    Offset = i * 1.0 / colors.Length
                });
            }
            return lcb;
        }

        public static bool IsEqual(this Color thisColor, Color color)
            => thisColor.A == color.A
            && thisColor.R == color.R
            && thisColor.G == color.G
            && thisColor.B == color.B;
    }
}
