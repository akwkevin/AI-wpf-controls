using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace AIStudio.Wpf.GridControls.Converters
{
    public class DynamicResourceWithConverterExtension : DynamicResourceExtension
    {
        public DynamicResourceWithConverterExtension()
        {
        }

        public DynamicResourceWithConverterExtension(object resourceKey)
                : base(resourceKey)
        {
        }

        public IValueConverter Converter
        {
            get; set;
        }
        public object ConverterParameter
        {
            get; set;
        }

        public override object ProvideValue(IServiceProvider provider)
        {
            object value = base.ProvideValue(provider);
            if (value != this && Converter != null)
            {
                Type targetType = null;
                var target = (IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget));
                if (target != null)
                {
                    DependencyProperty targetDp = target.TargetProperty as DependencyProperty;
                    if (targetDp != null)
                    {
                        targetType = targetDp.PropertyType;
                    }
                }
                if (targetType != null)
                    return Converter.Convert(value, targetType, ConverterParameter, CultureInfo.CurrentCulture);
            }

            return value;
        }
    }
}
