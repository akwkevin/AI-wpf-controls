using System;
using System.Windows.Data;

namespace AIStudio.Wpf.GridControls.Converters
{
    [ValueConversion(typeof(DateTime), typeof(double))]
    public class DateTimeDoubleConverter : IValueConverter
    {
        /// <summary>
        /// Converts a DateTime Value to a Double Value using the Ticks of the DateTime instance.
        /// </summary>
        /// <param name="value">Instance of the DateTime class.</param>
        /// <param name="targetType">Target Type, which should be a Double.</param>
        /// <param name="parameter">Parameter used in the conversion.</param>
        /// <param name="culture">Globalization culture instance.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dt = DateTime.Parse(value.ToString());
            return dt.Ticks;
        }

        /// <summary>
        /// Converts a Double Value to a DateTime Value assuming the Double represents the amount of Ticks for a DateTime instance.
        /// </summary>
        /// <param name="value">Instance of the Double Class.</param>
        /// <param name="targetType">Target Type, which should be a DateTime</param>
        /// <param name="parameter">Parameter used in the conversion.</param>
        /// <param name="culture">Globalization culture instance.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double d = double.Parse(value.ToString());
            return new DateTime((long)d);
        }
    }
}
