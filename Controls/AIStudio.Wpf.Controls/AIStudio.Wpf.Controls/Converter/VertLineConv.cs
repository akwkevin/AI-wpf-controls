using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    public class VertLineConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TreeViewItem item = (TreeViewItem)value;
            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(item);
            int index = ic.ItemContainerGenerator.IndexFromContainer(item);

            if ((string)parameter == "top")
            {
                if (ic is TreeView)
                    return 0;
                else
                    return 1;
            }
            else // assume "bottom"
            {
                if (item.HasItems == false)
                    return 0;
                else
                    return 1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }
}
