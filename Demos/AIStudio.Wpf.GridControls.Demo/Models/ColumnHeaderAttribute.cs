using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace AIStudio.Wpf.GridControls.Demo.Models
{
    public class ColumnHeaderAttribute : DisplayNameAttribute
    {
        public ColumnHeaderAttribute()
        {

        }

        public ColumnHeaderAttribute(string displayName) : base(displayName) { }


        public Visibility Visibility
        {
            get; set;
        } = Visibility.Visible;

        public bool DisplayIndex
        {
            get; set;
        }

        public string StringFormat
        {
            get; set;
        }

        public bool Ignore
        {
            get; set;
        }

        public Type Converter
        {
            get; set;
        }

        public object ConverterParameter
        {
            get; set;
        }

        public string ForegroundExpression
        {
            get; set;
        }

        public string BackgroundExpression
        {
            get; set;
        }

        public bool IsPin
        {
            get; set;
        }

        public bool IsRequired
        {
            get; set;
        }

        public bool IsReadOnly
        {
            get; set;
        }

        public ControlType ControlType
        {
            get; set;
        } = ControlType.TextBox;

        public static DataGridColumnCustom GetDataGridColumnCustom(PropertyInfo property)
        {
            DataGridColumnCustom dataGridColumnCustom = new DataGridColumnCustom();
            dataGridColumnCustom.Binding = property.Name;

            var attribute = ColumnHeaderAttribute.GetPropertyAttribute(property);
            if (attribute != null)
            {
                if (attribute.Ignore)
                {
                    return null;
                }

                dataGridColumnCustom.Header = attribute.DisplayName ?? property.Name;
                dataGridColumnCustom.StringFormat = attribute.StringFormat;
                dataGridColumnCustom.Visibility = attribute.Visibility;
                if (attribute.Converter != null)
                {
                    dataGridColumnCustom.Converter = Activator.CreateInstance(attribute.Converter) as IValueConverter;
                    dataGridColumnCustom.ConverterParameter = attribute.ConverterParameter;
                }
                dataGridColumnCustom.ForegroundExpression = attribute.ForegroundExpression;
                dataGridColumnCustom.BackgroundExpression = attribute.BackgroundExpression;
            }
            else
            {
                dataGridColumnCustom.Header = property.Name;
            }
            return dataGridColumnCustom;
        }

        public static ColumnHeaderAttribute GetPropertyAttribute(object descriptor)
        {
            PropertyDescriptor pd = descriptor as PropertyDescriptor;
            if (pd != null)
            {
                ColumnHeaderAttribute attr = pd.Attributes[typeof(ColumnHeaderAttribute)] as ColumnHeaderAttribute;
                if ((attr != null) && (attr != ColumnHeaderAttribute.Default))
                {
                    return attr;
                }
            }
            else
            {
                PropertyInfo pi = descriptor as PropertyInfo;
                if (pi != null)
                {
                    Object[] attrs = pi.GetCustomAttributes(typeof(ColumnHeaderAttribute), true);
                    foreach (var att in attrs)
                    {
                        ColumnHeaderAttribute attribute = att as ColumnHeaderAttribute;
                        if ((attribute != null) && (attribute != ColumnHeaderAttribute.Default))
                        {
                            return attribute;
                        }
                    }
                }
            }
            return null;
        }
    }
}
