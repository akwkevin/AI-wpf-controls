﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    public sealed class MathMultipleConverter : IMultiValueConverter
    {
        public MathOperation Operation
        {
            get; set;
        }

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.Length < 2 || value[0] == null || value[1] == null) return Binding.DoNothing;

            double value1, value2;
            if (!double.TryParse(value[0].ToString(), out value1) || !double.TryParse(value[1].ToString(), out value2))
                return 0;

            switch (Operation)
            {
                default:
                    // (case MathOperation.Add:)
                    return value1 + value2;
                case MathOperation.Divide:
                    return value1 / value2;
                case MathOperation.Multiply:
                    return value1 * value2;
                case MathOperation.Subtract:
                    return value1 - value2;
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
