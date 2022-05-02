using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    public class GridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var length = value?.ToString();
            try
            {
                return new GridLength(double.Parse(length), GridUnitType.Pixel);
            }
            catch
            {
                return new GridLength(1, GridUnitType.Auto);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class GridLengthAutoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var length = value?.ToString();
            try
            {
                if (Regex.IsMatch(length, @"^\d+(\.\d+)?$"))
                {
                    return new GridLength(double.Parse(length), GridUnitType.Pixel);
                }
                else if (length == "*")
                {
                    return new GridLength(1, GridUnitType.Star);
                }
                else if (Regex.IsMatch(length, @"^\d+(\.\d+)?\*$"))
                {
                    return new GridLength(double.Parse(length.Substring(0, length.Length - 1)), GridUnitType.Star);
                }
                else
                {
                    return new GridLength(1, GridUnitType.Auto);
                }
            }
            catch
            {
                return new GridLength(1, GridUnitType.Auto);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
