// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    /// <summary>
    /// Converts a Thickness to a new Thickness. It's possible to ignore a side with the IgnoreThicknessSide property.
    /// </summary>
    [ValueConversion(typeof(Thickness), typeof(Thickness), ParameterType = typeof(ThicknessSideType))]
    public class ThicknessBindingConverter : IValueConverter
    {
        public ThicknessSideType IgnoreThicknessSide { get; set; } = ThicknessSideType.None;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                var ignoreThickness = this.IgnoreThicknessSide;

                // yes, we can override it with the parameter value
                if (parameter is ThicknessSideType sideType)
                {
                    ignoreThickness = sideType;
                }

                switch (ignoreThickness)
                {
                    case ThicknessSideType.Left: return new Thickness(0, thickness.Top, thickness.Right, thickness.Bottom);
                    case ThicknessSideType.Top: return new Thickness(thickness.Left, 0, thickness.Right, thickness.Bottom);
                    case ThicknessSideType.Right: return new Thickness(thickness.Left, thickness.Top, 0, thickness.Bottom);
                    case ThicknessSideType.Bottom: return new Thickness(thickness.Left, thickness.Top, thickness.Right, 0);
                    case ThicknessSideType.OnlyLeft: return new Thickness(thickness.Left, 0, 0, 0);
                    case ThicknessSideType.OnlyTop: return new Thickness(0, thickness.Top, 0, 0);
                    case ThicknessSideType.OnlyRight: return new Thickness(0, 0, thickness.Right, 0);
                    case ThicknessSideType.OnlyBottom: return new Thickness(0, 0, 0, thickness.Bottom);
                    case ThicknessSideType.Half: return new Thickness(thickness.Left / 2, thickness.Top / 2, thickness.Right / 2, thickness.Bottom / 2);
                };
            }

            return default(Thickness);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public enum ThicknessSideType
    {
        /// <summary>
        /// Use all sides.
        /// </summary>
        None,
        /// <summary>
        /// Ignore the left side.
        /// </summary>
        Left,
        /// <summary>
        /// Ignore the top side.
        /// </summary>
        Top,
        /// <summary>
        /// Ignore the right side.
        /// </summary>
        Right,
        /// <summary>
        /// Ignore the bottom side.
        /// </summary>
        Bottom,
        /// <summary>
        /// Ignore the left side.
        /// </summary>
        OnlyLeft,
        /// <summary>
        /// Ignore the top side.
        /// </summary>
        OnlyTop,
        /// <summary>
        /// Ignore the right side.
        /// </summary>
        OnlyRight,
        /// <summary>
        /// Ignore the bottom side.
        /// </summary>
        OnlyBottom,
        /// <summary>
        /// Half
        /// </summary>
        Half,
    }
}