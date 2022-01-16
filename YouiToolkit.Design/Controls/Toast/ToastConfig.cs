using System;
using System.Windows;
using System.Windows.Media;

namespace YouiToolkit.Design
{
    public class ToastConfig
    {
        public const int FastTime = 1500;
        public const int NormalTime = 2000;
        public const int SlowTime = 3000;

        public double Height { get; set; } = 35;
        public int Time { get; set; } = NormalTime;
        public MessageBoxIcon MessageBoxIcon { get; set; } = MessageBoxIcon.None;
        public ToastLocation Location { get; set; } = ToastLocation.TopCenter;
        public Brush Foreground { get; set; } = Brushes.Black;
        public FontStyle FontStyle { get; set; } = SystemFonts.MessageFontStyle;
        public FontStretch FontStretch { get; set; } = FontStretches.Normal;
        public double FontSize { get; set; } = SystemFonts.MessageFontSize;
        public FontFamily FontFamily { get; set; } = SystemFonts.MessageFontFamily;
        public FontWeight FontWeight { get; set; } = SystemFonts.MenuFontWeight;
        public double IconSize { get; set; } = 26;
        public CornerRadius CornerRadius { get; set; } = new CornerRadius(5);
        public Brush BorderBrush { get; set; } = (Brush)new BrushConverter().ConvertFromString("#E1E1E1");
        public Thickness BorderThickness { get; set; } = new Thickness(1);
        public Brush Background { get; set; } = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");
        public HorizontalAlignment HorizontalContentAlignment { get; set; } = HorizontalAlignment.Left;
        public VerticalAlignment VerticalContentAlignment { get; set; } = VerticalAlignment.Center;
        public Thickness OffsetMargin { get; set; } = new Thickness(15);

        public ToastConfig()
        {
        }

        public ToastConfig(MessageBoxIcon icon, ToastLocation location, Thickness offsetMargin, int time) : this()
        {
            MessageBoxIcon = icon;
            Location = location;
            if (offsetMargin != default)
            {
                OffsetMargin = offsetMargin;
            }
            Time = time;
        }
    }

    public enum ToastLocation
    {
        Center,
        Left,
        Right,
        TopLeft,
        TopCenter,
        TopRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
    }
}
