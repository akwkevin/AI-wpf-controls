using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using AIStudio.Wpf.Controls.Bindings;

namespace AIStudio.Wpf.GridControls.Demo.Models
{
    public abstract class BaseControlItem : BindableBase
    {
        public object Header
        {
            get; set;
        }

        public string PropertyName
        {
            get; set;
        }

        private object _value;
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaisePropertyChanged("Value");
                }
            }
        }

        private Visibility _visibility;
        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                if (_visibility != value)
                {
                    _visibility = value;
                    RaisePropertyChanged("Visibility");
                }
            }
        }

        public ControlType ControlType
        {
            get; set;
        }

        public ObservableCollection<SelectOption> ItemSource
        {
            get; set;
        }

        public bool IsRequired
        {
            get; set;
        }

        public string StringFormat
        {
            get; set;
        }

        public static bool GetControlItem(PropertyInfo property, BaseControlItem queryConditionItem)
        {
            var attribute = ColumnHeaderAttribute.GetPropertyAttribute(property);
            if (attribute != null)
            {
                if (attribute.Ignore)
                {
                    return false;
                }

                queryConditionItem.Header = attribute.DisplayName ?? property.Name;
                queryConditionItem.ControlType = attribute.ControlType;
                queryConditionItem.IsRequired = attribute.IsRequired;
                queryConditionItem.StringFormat = attribute.StringFormat;
            }
            else
            {
                queryConditionItem.Header = property.Name;
                queryConditionItem.ControlType = ControlType.TextBox;
            }

            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(long))
            {
                if (string.IsNullOrEmpty(queryConditionItem.StringFormat))
                {
                    queryConditionItem.StringFormat = "n0";
                }
                queryConditionItem.ControlType = ControlType.IntegerUpDown;
            }
            else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(float))
            {
                if (string.IsNullOrEmpty(queryConditionItem.StringFormat))
                {
                    queryConditionItem.StringFormat = "f3";
                }
                queryConditionItem.ControlType = ControlType.NumericUpDown;
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                if (string.IsNullOrEmpty(queryConditionItem.StringFormat))
                {
                    queryConditionItem.StringFormat = "yyyy-MM-dd HH:mm:ss";
                }
                queryConditionItem.ControlType = ControlType.DateTimeUpDown;
            }

            queryConditionItem.PropertyName = property.Name;

            return true;
        }

        public static void ObjectToList<T>(object value, IEnumerable<T> items) where T : BaseControlItem
        {
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.PropertyName))
                    continue;

                item.Value = value.GetType().GetProperty(item.PropertyName).GetValue(value);
            }
        }

        public static void ListToObject<T>(object value, IEnumerable<T> items) where T : BaseControlItem
        {
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.PropertyName))
                    continue;

                try
                {

                    var propertyInfo = value.GetType().GetProperty(item.PropertyName);
                    //object objvalue = item.Value;
                    //if (propertyInfo.PropertyType == typeof(double))
                    //{
                    //    objvalue = double.Parse(objvalue?.ToString());
                    //}
                    //else if (propertyInfo.PropertyType == typeof(float))
                    //{
                    //    objvalue = float.Parse(objvalue?.ToString());
                    //}
                    //else if (propertyInfo.PropertyType == typeof(int))
                    //{
                    //    objvalue = int.Parse(objvalue?.ToString());
                    //}
                    //else if (propertyInfo.PropertyType == typeof(long))
                    //{
                    //    objvalue = long.Parse(objvalue?.ToString());
                    //}
                    propertyInfo.SetValue(value, item.Value);
                }
                catch { }
            }
        }

        public static Dictionary<string, string> ListToDictionary<T>(IEnumerable<T> items) where T : BaseControlItem
        {
            Dictionary<string, string> keyValue = new Dictionary<string, string>();
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.PropertyName))
                    continue;
                if (item.Value == null || item.Value.ToString() == string.Empty)
                    continue;

                keyValue.Add(item.PropertyName, item.Value?.ToString());
            }

            return keyValue;
        }
    }
}
