using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    public class DisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || parameter is null)
                return value;

            Type type = value.GetType();
            if (type.GetProperty(parameter.ToString()) != null)
            {
                var prop = type.GetProperty(parameter.ToString());

                string name = string.Empty;
                var displayAttr = prop.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                if (displayAttr.Length > 0)
                {
                    name = (displayAttr[0] as DisplayNameAttribute).DisplayName;
                }
                else
                {
                    name = prop.Name;
                }

                return name;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
