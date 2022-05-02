using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Effects;

namespace AIStudio.Wpf.Controls.Converter
{
    public class ShadowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DropShadowEffect depth)
            {
                return Clone(depth);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();

        private static DropShadowEffect Clone(DropShadowEffect dropShadowEffect)
        {
            if (dropShadowEffect is null) return null;
            return new DropShadowEffect()
            {
                BlurRadius = dropShadowEffect.BlurRadius,
                Color = dropShadowEffect.Color,
                Direction = dropShadowEffect.Direction,
                Opacity = dropShadowEffect.Opacity,
                RenderingBias = dropShadowEffect.RenderingBias,
                ShadowDepth = dropShadowEffect.ShadowDepth
            };
        }
    }
}
