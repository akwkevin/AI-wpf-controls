﻿using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
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

    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

    }
}
