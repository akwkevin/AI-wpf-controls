using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Windows;
using AIStudio.Wpf.Controls.Bindings;

namespace AIStudio.Wpf.GridControls.Demo.Models
{
    public class EditFormItem : QueryConditionItem
    {
        public bool IsReadOnly
        {
            get; set;
        }

        public static EditFormItem GetEditFormItem(PropertyInfo property)
        {
            EditFormItem editFormItem = new EditFormItem();

            var attribute = ColumnHeaderAttribute.GetPropertyAttribute(property);
            if (attribute != null)
            {
                if (attribute.Ignore)
                {
                    return null;
                }

                editFormItem.Header = attribute.DisplayName ?? property.Name;
                editFormItem.IsRequired = attribute.IsRequired;
                editFormItem.ControlType = attribute.ControlType;
                editFormItem.IsReadOnly = attribute.IsReadOnly;
                editFormItem.StringFormat = attribute.StringFormat;
            }
            else
            {
                editFormItem.Header = property.Name;
                editFormItem.Visibility = Visibility.Collapsed;
                editFormItem.ControlType = ControlType.TextBox;
            }


            if (property.PropertyType == typeof(int))
            {
                if (string.IsNullOrEmpty(editFormItem.StringFormat))
                {
                    editFormItem.StringFormat = "n0";
                }
                editFormItem.ControlType = ControlType.NumericUpDown;
            }
            else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(float))
            {
                if (string.IsNullOrEmpty(editFormItem.StringFormat))
                {
                    editFormItem.StringFormat = "f3";
                }
                editFormItem.ControlType = ControlType.NumericUpDown;
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                if (string.IsNullOrEmpty(editFormItem.StringFormat))
                {
                    editFormItem.StringFormat = "yyyy-MM-dd HH:mm:ss";
                }
                editFormItem.ControlType = ControlType.DateTimeUpDown;
            }

            editFormItem.PropertyName = property.Name;

            return editFormItem;
        }


    }
}
