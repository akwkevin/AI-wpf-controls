using System;
using System.Globalization;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    public class DateTimeToStringConverter : IValueConverter
    {
        #region Converter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion

    }
}
