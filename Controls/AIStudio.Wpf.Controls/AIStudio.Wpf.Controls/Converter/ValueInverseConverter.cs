using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    #region
    internal class ValueInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doublevalue)
            {
                return 0 - doublevalue;
            }
            if (value is int intvalue)
            {
                return 0 - intvalue;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
    #endregion
}
