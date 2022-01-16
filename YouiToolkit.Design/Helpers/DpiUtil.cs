using Microsoft.Win32;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using YouiToolkit.Logging;

namespace YouiToolkit.Design
{
    public static class DpiUtil
    {
        public static DpiScale Dpi { get; private set; } = InitDpiScale();

        static DpiUtil()
        {
            SystemEvents.DisplaySettingsChanged += (s, e) =>
            {
                DpiScale dpiPrev = Dpi;
                Dpi = InitDpiScale();
                Logger.Warn($"[DpiUpdate] From {dpiPrev.PixelsPerDip} to {Dpi.PixelsPerDip}.");
            };
        }

        public static DpiScale InitDpiScale()
        {
            if (Application.Current.MainWindow != null)
            {
                DpiScale dpi = VisualTreeHelper.GetDpi(Application.Current.MainWindow);
                return dpi;
            }
            return default;
        }

        public static double GetDpiScale(this Visual visual)
        {
            PresentationSource source = PresentationSource.FromVisual(visual);
            return source.CompositionTarget.TransformToDevice.M11;
        }

        public static Dpi GetDpiFromVisual(Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);

            var dpiX = 96d;
            var dpiY = 96d;

            if (source?.CompositionTarget != null)
            {
                dpiX = 96d * source.CompositionTarget.TransformToDevice.M11;
                dpiY = 96d * source.CompositionTarget.TransformToDevice.M22;
            }

            return new Dpi(dpiX, dpiY);
        }

        public static Dpi GetDpiByGraphics()
        {
            using var graphics = Graphics.FromHwnd(IntPtr.Zero);
            double dpiX = graphics.DpiX;
            double dpiY = graphics.DpiY;

            return new Dpi(dpiX, dpiY);
        }

        public static Dpi GetDpiBySystemParameters()
        {
            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;

            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", bindingFlags);
            var dpiYProperty = typeof(SystemParameters).GetProperty("DpiY", bindingFlags);

            var dpiX = 96.0;
            var dpiY = 96.0;

            if (dpiXProperty != null)
            {
                dpiX = (double)dpiXProperty.GetValue(null, null);
            }

            if (dpiYProperty != null)
            {
                dpiY = (double)dpiYProperty.GetValue(null, null);
            }

            return new Dpi(dpiX, dpiY);
        }
    }

    public struct Dpi
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Dpi(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
