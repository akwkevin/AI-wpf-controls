using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Windows;
using AIStudio.Wpf.Controls.Bindings;

namespace AIStudio.Wpf.GridControls.Demo.Models
{
    public class QueryConditionItem : BindableBase
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

        public static QueryConditionItem GetQueryConditionItem(PropertyInfo property)
        {
            QueryConditionItem queryConditionItem = new QueryConditionItem();

            var attribute = ColumnHeaderAttribute.GetPropertyAttribute(property);
            if (attribute != null)
            {
                if (attribute.Ignore)
                {
                    return null;
                }

                queryConditionItem.Header = attribute.DisplayName ?? property.Name;
                queryConditionItem.Visibility = attribute.IsPin ? Visibility.Visible : Visibility.Collapsed;
                queryConditionItem.IsRequired = attribute.IsRequired;
                queryConditionItem.ControlType = attribute.ControlType;
            }
            else
            {
                queryConditionItem.Header = property.Name;
                queryConditionItem.Visibility = Visibility.Collapsed;
                queryConditionItem.ControlType = ControlType.TextBox;
            }
            queryConditionItem.PropertyName = property.Name;

            return queryConditionItem;
        }

        public static void ObjectToList<T>(object value, IEnumerable<T> items) where T : QueryConditionItem
        {
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.PropertyName))
                    continue;

                item.Value = value.GetType().GetProperty(item.PropertyName).GetValue(value);
            }
        }


        public static void ListToObject<T>(object value, IEnumerable<T> items) where T : QueryConditionItem
        {
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.PropertyName))
                    continue;

                value.GetType().GetProperty(item.PropertyName).SetValue(value, item.Value);
            }
        }

        public static Dictionary<string, string> ListToDictionary<T>(IEnumerable<T> items) where T : QueryConditionItem
        {
            Dictionary<string, string> keyValue = new Dictionary<string, string>();
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.PropertyName))
                    continue;
                if (item.Value == null)
                    continue;

                keyValue.Add(item.PropertyName, item.Value?.ToString());
            }

            return keyValue;
        }
    }
}
