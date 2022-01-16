using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace YouiToolkit.Design
{
    public class SliderValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[1] is string valueFormat)
            {
                return string.Format(valueFormat, values[0]);
            }
            return $"{values[0]:0}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { DependencyProperty.UnsetValue };
        }
    }
}
