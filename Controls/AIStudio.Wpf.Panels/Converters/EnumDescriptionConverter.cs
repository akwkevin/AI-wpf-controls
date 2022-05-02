using System;
using System.Windows.Data;
using AIStudio.Wpf.Panels.Helpers;

namespace AIStudio.Wpf.Panels.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumDescriptionConverter"/> class.
        /// </summary>
        public EnumDescriptionConverter()
        {
        }

        #endregion

        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (value is Enum)
                {
                    return ((Enum)value).GetDescription();
                }
                else
                {
                    return value.ToString();
                }
            }
            else
            {
                return string.Empty;
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
