using System;
using System.Windows.Data;

namespace AIStudio.Wpf.ComeCapture.Converters
{
    public class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return int.Parse(value.ToString()) == int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                return parameter;
            }
            else
            {
                return null;
            }
        }
    }
}
