using System.Windows;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class ClipAssists
    {
        public static double GetSize(DependencyObject obj)
        {
            return (double)obj.GetValue(SizeProperty);
        }

        public static void SetSize(DependencyObject obj, double value)
        {
            obj.SetValue(SizeProperty, value);
        }

        // Using a DependencyProperty as the backing store for Size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.RegisterAttached("Size", typeof(double), typeof(ClipAssists), new PropertyMetadata(0.0, SizePropertyChanged));

        private static void SizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (double.TryParse(e.NewValue?.ToString(), out double size))
            {
                if (d is EllipseGeometry ellipse)
                {
                    ellipse.RadiusX = ellipse.RadiusY = size / 2;
                    ellipse.Center = new Point(ellipse.RadiusX, ellipse.RadiusY);

                }
            }
        }
    }
}
