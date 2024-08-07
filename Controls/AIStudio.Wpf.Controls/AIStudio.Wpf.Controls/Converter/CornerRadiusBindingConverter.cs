﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls.Converter
{
    /// <summary>
    /// Converts a CornerRadius to a new CornerRadius. It's possible to ignore a side with the IgnoreRadius property.
    /// </summary>
    [ValueConversion(typeof(CornerRadius), typeof(CornerRadius), ParameterType = typeof(RadiusType))]
    public class CornerRadiusBindingConverter : IValueConverter
    {
        public RadiusType IgnoreRadius { get; set; } = RadiusType.None;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CornerRadius cornerRadius)
            {
                var ignoreRadius = this.IgnoreRadius;

                // yes, we can override it with the parameter value
                if (parameter is RadiusType radiusType)
                {
                    ignoreRadius = radiusType;
                }

                switch (ignoreRadius)
                {
                    case RadiusType.Left: return new CornerRadius(0, cornerRadius.TopRight, cornerRadius.BottomRight, 0);
                    case RadiusType.Top: return new CornerRadius(0, 0, cornerRadius.BottomRight, cornerRadius.BottomLeft);
                    case RadiusType.Right: return new CornerRadius(cornerRadius.TopLeft, 0, 0, cornerRadius.BottomLeft);
                    case RadiusType.Bottom: return new CornerRadius(cornerRadius.TopLeft, cornerRadius.TopRight, 0, 0);
                    case RadiusType.TopLeft: return new CornerRadius(0, cornerRadius.TopRight, cornerRadius.BottomRight, cornerRadius.BottomLeft);
                    case RadiusType.TopRight: return new CornerRadius(cornerRadius.TopLeft, 0, cornerRadius.BottomRight, cornerRadius.BottomLeft);
                    case RadiusType.BottomRight: return new CornerRadius(cornerRadius.TopLeft, cornerRadius.TopRight, 0, cornerRadius.BottomLeft);
                    case RadiusType.BottomLeft: return new CornerRadius(cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomRight, 0);
                };
            }

            return default(CornerRadius);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // for now no back converting
            return DependencyProperty.UnsetValue;
        }
    }

    public enum RadiusType
    {
        /// <summary>
        /// Use the radius of all corners.
        /// </summary>
        None,
        /// <summary>
        /// Ignore the radius of the top-left and bottom-left corner.
        /// </summary>
        Left,
        /// <summary>
        /// Ignore the radius of the top-left and top-right corner.
        /// </summary>
        Top,
        /// <summary>
        /// Ignore the radius of the top-right and bottom-right corner.
        /// </summary>
        Right,
        /// <summary>
        /// Ignore the radius of the bottom-left and bottom-right corner.
        /// </summary>
        Bottom,
        /// <summary>
        /// Ignore the radius of the top-left corner.
        /// </summary>
        TopLeft,
        /// <summary>
        /// Ignore radius of the top-right corner.
        /// </summary>
        TopRight,
        /// <summary>
        /// Ignore the radius of the bottom-right corner.
        /// </summary>
        BottomRight,
        /// <summary>
        /// Ignore the radius of the bottom-left corner.
        /// </summary>
        BottomLeft
    }
}