﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace AIStudio.Wpf.GridControls.Demo.Converter
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type typeTarget, object param, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is double))
            {
                return "Black";
            }

            if (System.Convert.ToDouble(value) >= 100.0 && System.Convert.ToDouble(value) < 200)
            {
                return "Black";
            }

            if (System.Convert.ToDouble(value) >= 200.0 && System.Convert.ToDouble(value) < 300)
            {
                return "Green";
            }

            if (System.Convert.ToDouble(value) >= 300.0)
            {
                return "Red";
            }

            return "Black";
        }
        public object ConvertBack(object value, Type typeTarget, object param, System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }
}
