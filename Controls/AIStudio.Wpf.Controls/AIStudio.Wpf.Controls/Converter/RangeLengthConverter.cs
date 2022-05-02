using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    public class RangeLengthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 4 || values.Any(v => v == null))
                return Binding.DoNothing;

            double min, max, value, containerLength;
            if (!double.TryParse(values[0].ToString(), out min)
                || !double.TryParse(values[1].ToString(), out max)
                || !double.TryParse(values[2].ToString(), out value)
                || !double.TryParse(values[3].ToString(), out containerLength))

                return Binding.DoNothing;

            var percent = (value - min) / (max - min);
            var length = percent * containerLength;

            return length > containerLength ? containerLength : length;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
