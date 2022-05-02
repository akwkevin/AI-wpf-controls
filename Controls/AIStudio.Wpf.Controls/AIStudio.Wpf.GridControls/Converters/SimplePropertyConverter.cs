using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace AIStudio.Wpf.GridControls.Converters
{
    /// <summary>
    /// Provide IValueConverter for common ValueTypes.
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(String))]
    [ValueConversion(typeof(String), typeof(String))]
    [ValueConversion(typeof(int), typeof(String))]
    [ValueConversion(typeof(Int64), typeof(String))]
    [ValueConversion(typeof(long), typeof(String))]
    [ValueConversion(typeof(double), typeof(String))]
    [ValueConversion(typeof(Enum), typeof(String))]
    [ValueConversion(typeof(bool), typeof(String))]
    public class SimplePropertyConverter : IValueConverter
    {
        private static SimplePropertyConverter _This = new SimplePropertyConverter();

        private static CultureInfo GetCulture(FrameworkElement element)
        {
            CultureInfo culture;
            if (element != null && DependencyPropertyHelper.GetValueSource(element, FrameworkElement.LanguageProperty).BaseValueSource != BaseValueSource.Default)
            {
                culture = GetCultureInfo(element);
            }
            else
            {
                culture = CultureInfo.CurrentCulture;
            }
            return culture;
        }
        private static CultureInfo GetCultureInfo(DependencyObject element)
        {
            XmlLanguage language = (XmlLanguage)element.GetValue(FrameworkElement.LanguageProperty);
            try
            {
                return language.GetSpecificCulture();
            }
            catch (InvalidOperationException)
            {
                // We default to en-US if no part of the language tag is recognized.
                return CultureInfo.ReadOnly(new CultureInfo("en-us", false));
            }
        }

        public static SimplePropertyConverter This
        {
            get
            {
                return _This;
            }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(targetType);
            try
            {
                if (converter.CanConvertTo(null, typeof(string)))
                {
                    //return converter.ConvertTo(value, typeof(string));
                    return converter.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(string));
                }
                else
                {
                    return value.ToString();
                }
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(targetType);
            try
            {

                if (converter.CanConvertFrom(null, value.GetType()))
                {
                    return converter.ConvertFrom(null, CultureInfo.CurrentCulture, value);
                }
                else
                {
                    return converter.ConvertFrom(value.ToString());
                }
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}
