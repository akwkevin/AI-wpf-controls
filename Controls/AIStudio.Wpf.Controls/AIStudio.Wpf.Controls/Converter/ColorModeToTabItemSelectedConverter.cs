using System;
using System.Globalization;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    public class ColorModeToTabItemSelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorMode = (ColorMode)value;
            return (colorMode == ColorMode.ColorPalette) ? 0 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var index = (int)value;
            return (index == 0) ? ColorMode.ColorPalette : ColorMode.ColorCanvas;
        }
    }
}
