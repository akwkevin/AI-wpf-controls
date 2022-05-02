using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace AIStudio.Wpf.Controls.Converter
{
    public class StringToImageSourceConverter : IValueConverter
    {
        #region Converter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;
            if (string.IsNullOrEmpty(path))
            {
                return value;
            }
            return BitmapFrame.Create(new Uri(path), BitmapCreateOptions.None, BitmapCacheOption.Default);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion

    }
}
