using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace YouiToolkit.Design
{
    public static class MeasureUtil
    {
        public static Size MeasureString(string text, double fontSize, FontFamily fontFamily, FontStyle? fontStyle = null, FontWeight? fontWeight = null, FontStretch? fontStretch = null, double pixelsPerDip = 1d)
        {
            fontStyle ??= FontStyles.Normal;
            fontWeight ??= FontWeights.Normal;
            fontStretch ??= FontStretches.Normal;
            var typeface = new Typeface(fontFamily, (FontStyle)fontStyle, (FontWeight)fontWeight, (FontStretch)fontStretch);
            var formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black, pixelsPerDip);

            return new Size(formattedText.Width, formattedText.Height);
        }

        public static Size MeasureString(this Visual visual, string text, double fontSize, FontFamily fontFamily, FontStyle? fontStyle = null, FontWeight? fontWeight = null, FontStretch? fontStretch = null)
        {
            fontStyle ??= FontStyles.Normal;
            fontWeight ??= FontWeights.Normal;
            fontStretch ??= FontStretches.Normal;

            DpiScale dpi = VisualTreeHelper.GetDpi(visual);
            var typeface = new Typeface(fontFamily, (FontStyle)fontStyle, (FontWeight)fontWeight, (FontStretch)fontStretch);
            var formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black, dpi.PixelsPerDip);

            return new Size(formattedText.Width, formattedText.Height);
        }

        public static Size MeasureString(this TextBlock textBlock, string text)
        {
            return textBlock.MeasureString(text, textBlock.FontSize, textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
        }
    }
}
