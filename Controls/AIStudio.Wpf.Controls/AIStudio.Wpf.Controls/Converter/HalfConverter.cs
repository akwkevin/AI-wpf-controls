using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    #region Half 
    internal class HalfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = (value as double?).GetValueOrDefault();

            return doubleValue / 2;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
    #endregion
}
