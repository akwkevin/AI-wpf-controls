using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    public class IsLastItemInContainerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                DependencyObject item = (DependencyObject)value;
                ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(item);
                if (ic != null)
                {
                    return ic.ItemContainerGenerator.IndexFromContainer(item) == ic.Items.Count - 1;
                }
            }
            catch
            {

            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
