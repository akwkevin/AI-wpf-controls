using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls.Converter
{
    public class DropDownBorderPathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var width = values[0] as double? ?? 0;
            var height = values[1] as double? ?? 0;

            var contentWidth = values[2] as double? ?? 0;
            var contentHeight = values[3] as double? ?? 0;

            var placement = values[4] as PlacementMode? ?? PlacementMode.Bottom;
            var radius = values[5] as CornerRadius? ?? new CornerRadius();
            var isAngleVisible = values[6] as bool? ?? false;

            var path = "";
            if (isAngleVisible)
            {
                switch (placement)
                {
                    case PlacementMode.Bottom:
                        path = $"M 1,{radius.TopLeft + 7} A{radius.TopLeft},{radius.TopLeft} 0 0 1 {radius.TopLeft + 1}, 7 H {width - contentWidth / 2 - 5} L {width - contentWidth / 2},1 L {width - contentWidth / 2 + 5},7   H {width - radius.TopRight - 1} A{radius.TopRight},{radius.TopRight} 0 0 1 {width - 1}, {radius.TopRight + 7}" +
                            $"V {height - radius.BottomRight - 1} A{ radius.BottomRight},{ radius.BottomRight} 0 0 1 {width - radius.BottomRight - 1}, {height - 1} H {radius.BottomLeft + 1} A{radius.BottomLeft},{radius.BottomLeft} 0 0 1 1, {height - radius.BottomLeft - 1} Z";
                        break;
                    case PlacementMode.Top:
                        path = $"M 1,{radius.TopLeft + 1} A{radius.TopLeft},{radius.TopLeft} 0 0 1 {radius.TopLeft + 1}, 1 H {width - radius.TopRight - 1} A{radius.TopRight},{radius.TopRight} 0 0 1 {width - 1}, {radius.TopRight + 1}" +
                             $"V {height - radius.BottomRight - 7} A{radius.BottomRight},{radius.BottomRight} 0 0 1 {width - radius.BottomRight - 1}, {height - 7} H {width - contentWidth / 2 + 5} L {width - contentWidth / 2}, {height - 1} L {width - contentWidth / 2 - 5}, {height - 7} H {radius.BottomLeft + 1} A{radius.BottomLeft},{radius.BottomLeft} 0 0 1 1, {height - radius.BottomLeft - 7} Z";
                        break;
                    case PlacementMode.Left:
                        path = $"M 1,{radius.TopLeft + 1} A{radius.TopLeft},{radius.TopLeft} 0 0 1 {radius.TopLeft + 1}, 1 H {width - radius.TopRight - 7} A{radius.TopRight },{radius.TopRight } 0 0 1 {width - 7}, {radius.TopRight + 1}" +
                            $"V {height / 2 - 5} L {width - 1},{height / 2} L {width - 7},{height / 2 + 5} V {height - radius.BottomRight - 1} A{radius.BottomRight},{radius.BottomRight} 0 0 1 {width - radius.BottomRight - 7}, {height - 1} H {radius.BottomLeft + 1} A{radius.BottomLeft},{radius.BottomLeft} 0 0 1 1, {height - radius.BottomLeft - 1} Z";
                        break;
                    case PlacementMode.Right:
                        path = $"M 7,{radius.TopLeft + 1} A{radius.TopLeft},{radius.TopLeft} 0 0 1 {radius.TopLeft + 7}, 1 H {width - radius.TopRight - 1} A{radius.TopRight},{radius.TopRight} 0 0 1 {width - 1}, {radius.TopRight + 1}" +
                            $"V {height - radius.BottomRight - 1} A{radius.BottomRight},{radius.BottomRight} 0 0 1 {width - radius.BottomRight - 1}, {height - 1} H {radius.BottomLeft + 7} A{radius.BottomLeft},{radius.BottomLeft} 0 0 1 7, {height - radius.BottomLeft - 1} V {height / 2 + 5} L {1},{height / 2} L {7},{height / 2 - 5} Z";
                        break;
                    default:
                        path = $"M 1,{radius.TopLeft + 1} A{radius.TopLeft},{radius.TopLeft} 0 0 1 {radius.TopLeft + 1}, 1 H {width - radius.TopRight - 1} A{radius.TopRight},{radius.TopRight} 0 0 1 {width - 1}, {radius.TopRight + 1}" +
                                           $"V {height - radius.BottomRight - 1} A{radius.BottomRight},{radius.BottomRight} 0 0 1 {width - radius.BottomRight - 1}, {height - 1} H {radius.BottomLeft + 1} A{radius.BottomLeft},{radius.BottomLeft} 0 0 1 1, {height - radius.BottomLeft - 1} Z";
                        break;
                }
            }
            else
            {
                path = $"M 1,{radius.TopLeft + 1} A{radius.TopLeft},{radius.TopLeft} 0 0 1 {radius.TopLeft + 1}, 1 H {width - radius.TopRight - 1} A{radius.TopRight},{radius.TopRight} 0 0 1 {width - 1}, {radius.TopRight + 1}" +
                    $"V {height - radius.BottomRight - 1} A{radius.BottomRight},{radius.BottomRight} 0 0 1 {width - radius.BottomRight - 1}, {height - 1} H {radius.BottomLeft + 1} A{radius.BottomLeft},{radius.BottomLeft} 0 0 1 1, {height - radius.BottomLeft - 1} Z";
            }

            return Geometry.Parse(path);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { DependencyProperty.UnsetValue, DependencyProperty.UnsetValue };
        }
    }
}
