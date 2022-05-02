using System;
using System.Windows.Data;
using System.Windows.Media;

namespace AIStudio.Wpf.ComeCapture.Converters
{
    public class ColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(value.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
