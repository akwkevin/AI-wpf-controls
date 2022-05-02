using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    public class ToolBarOverflowButtonVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var overflowMode = (OverflowMode)values[0];
            var hasOverflowItems = (bool)values[1];

            if (overflowMode == OverflowMode.AsNeeded && hasOverflowItems)
            {
                return Visibility.Visible;
            }
            else if (overflowMode == OverflowMode.Always)
            {
                return Visibility.Visible;
            }
            else if (overflowMode == OverflowMode.Never)
            {
                return Visibility.Hidden;
            }
            else
            {
                return Visibility.Hidden;
            }
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
