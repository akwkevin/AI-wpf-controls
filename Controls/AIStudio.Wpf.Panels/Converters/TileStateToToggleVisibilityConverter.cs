using System;
using System.Globalization;
using System.Windows.Data;


namespace AIStudio.Wpf.Panels.Converters
{
    public class TileStateToToggleVisibilityConverter : IValueConverter
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
                switch (result)
                {
                    case TileState.Maximized:
                        {
                            return System.Windows.Visibility.Collapsed;
                        }
                    case TileState.Normal:
                    case TileState.Minimized:
                    //case TileState.MinimizedExpanded:
                    default:
                        {
                            return System.Windows.Visibility.Visible;
                        }
                }
            }

            return System.Windows.Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
