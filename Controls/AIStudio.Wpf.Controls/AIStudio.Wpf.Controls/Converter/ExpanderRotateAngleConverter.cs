using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    internal class ExpanderRotateAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double factor = 1.0;
            if (parameter != null)
            {
                if (!double.TryParse(parameter.ToString(), out factor))
                {
                    factor = 1.0;
                }
            }
            if (object.Equals(value, ExpandDirection.Left))
            {
                return 90 * factor;
            }
            else if (object.Equals(value, ExpandDirection.Right))
            {
                return -90 * factor;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
