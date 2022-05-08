using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Windows;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.GridControls.Demo.Attributes;

namespace AIStudio.Wpf.GridControls.Demo.Commons
{
    public class EditFormItem : BaseControlItem
    {
        public static EditFormItem GetEditFormItem(PropertyInfo property)
        {
            EditFormItem editFormItem = new EditFormItem();
            if (GetControlItem(property, editFormItem) == false)
                return null;

            var attribute = ColumnHeaderAttribute.GetPropertyAttribute(property);
            if (attribute != null)
            {
                editFormItem.IsReadOnly = attribute.IsReadOnly;
                editFormItem.Visibility = attribute.Visibility;
            }
            else
            {
                editFormItem.Visibility = Visibility.Visible;
            }

            return editFormItem;
        }


    }
}
