using System;
using System.Globalization;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    /// <summary>
    /// 在0~9前面加0
    /// </summary>
    public class NumberConvert : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null)
            {
                return "";
            }
            int number = 2;
            if (values[1] != null)
            {
                int.TryParse(System.Convert.ToString(values[1]), out number);
            }

            return values[0].ToString().PadLeft(number, '0');
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
