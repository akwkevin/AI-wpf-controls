using System;
using System.Windows;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    public class IntToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double thickValue = 0;
            if (value != null)
                thickValue = System.Convert.ToDouble(value.ToString());

            if (parameter != null)
            {
                if (parameter.ToString().ToUpper() == "LEFT")
                    return new Thickness(thickValue, 0, 0, 0);
                else if (parameter.ToString().ToUpper() == "TOP")
                    return new Thickness(0, thickValue, 0, 0);
                else if (parameter.ToString().ToUpper() == "RIGHT")
                    return new Thickness(0, 0, thickValue, 0);
                else if (parameter.ToString().ToUpper() == "BOTTOM")
                    return new Thickness(0, 0, 0, thickValue);
            }

            return new Thickness(thickValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
