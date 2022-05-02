using System;
using System.Globalization;
using System.Windows.Data;

namespace AIStudio.Wpf.Panels.Converters
{
    public class StateToCollapsededGlyphVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return System.Windows.Visibility.Visible;
            }

            TileState result = TileState.Normal;

            if (Enum.TryParse(value.ToString(), out result))
            {
                if (result == TileState.Normal)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }

            return System.Windows.Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
