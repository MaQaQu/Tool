using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace YouiToolkit.Design
{
    public partial class LightView : UserControl
    {

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public static readonly DependencyProperty BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(Color), typeof(LightView), new PropertyMetadata(null));

        public Color FillColor
        {
            get { return (Color)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }
        public static readonly DependencyProperty FillColorProperty = DependencyProperty.Register("FillColor", typeof(Color), typeof(LightView), new PropertyMetadata(null));


        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(LightView), new PropertyMetadata(20d, OnRadiusChanged));


        public bool IsShowText
        {
            get => (bool)GetValue(IsShowTextProperty);
            set => SetValue(IsShowTextProperty, value);
        }
        public static readonly DependencyProperty IsShowTextProperty = DependencyProperty.Register("IsShowText", typeof(bool), typeof(LightView), new PropertyMetadata(false, OnIsShowTextChanged));

        public LightView()
        {
            InitializeComponent();
        }

        private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LightView lightView && e.NewValue is double radius)
            {

            }
        }

        private static void OnIsShowTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LightView lightView && e.NewValue is bool isShowText)
            {
                lightView.textBlock.Visibility = isShowText ? Visibility.Visible : Visibility.Collapsed;
            }
        }

    }
}
