using System;
using System.Globalization;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    public class StringJoinConverter : IMultiValueConverter
    {
        public string Split { get; set; } = "/";

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.Length < 2 || value[0] == null || value[1] == null) return Binding.DoNothing;

            double value1, value2;
            if (!double.TryParse(value[0].ToString(), out value1) || !double.TryParse(value[1].ToString(), out value2))
                return 0;

            return string.Join(Split, value);

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
