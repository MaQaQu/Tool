using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace YouiToolkit.Design
{
    public abstract class DataGridButtonGroupColumnStyle
    {
        public abstract CornerRadius CornerRadius { get; set; }
        public abstract IList<Brush> Backgrounds { get; set; }
        public abstract IList<Brush> Foregrounds { get; set; }
        public abstract IList<FontFamily> FontFamilies { get; set; }
        public abstract IList<Brush> HoverBrushes  { get; set; }
        public virtual IList<int> Widths { get; set; }
        public virtual double Margin { get; set; } = 5;

        public virtual void ApplyStyle(DataGridButtonGroupColumnButton button, int index)
        {
            if (index < 0)
            {
                return;
            }
            if (index != 0)
            {
                button.Margin = new Thickness(Margin, 0, 0, 0);
            }
            if (Widths != null && index < Widths.Count)
            {
                if (Widths[index] >= 0)
                {
                    button.Width = Widths[index];
                }
            }
            if (CornerRadius != null)
            {
                ButtonHelper.SetCornerRadius(button, CornerRadius);
            }
            if (Backgrounds != null && index < Backgrounds.Count)
            {
                button.Background = Backgrounds[index];
            }
            if (Foregrounds != null && index < Foregrounds.Count)
            {
                button.Foreground = Foregrounds[index];
            }
            if (FontFamilies != null && index < FontFamilies.Count)
            {
                button.FontFamily = FontFamilies[index];
            }
            if (HoverBrushes != null && index < HoverBrushes.Count)
            {
                ButtonHelper.SetHoverBrush(button, HoverBrushes[index]);
            }
        }
    }
}
