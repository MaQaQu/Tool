using System.Windows.Controls;
using System.Windows.Media;

namespace YouiToolkit.Design
{
    public class ColorImage : Image
    {
        public new ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public ColorImage()
        {
        }
    }
}
