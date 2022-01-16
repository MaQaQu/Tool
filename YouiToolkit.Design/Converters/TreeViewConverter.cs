﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace YouiToolkit.Design
{
    internal class TreeViewVerticalChainConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Thickness(0, 0, 0, (double)value / 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

}
