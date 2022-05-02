using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public static class ImageAttach
    {
        //图片圆角       
        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }
        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }
        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...     
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ImageAttach), new PropertyMetadata(new CornerRadius(0), OnCornerRadiusChanged));

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Image img)
            {
                if (img.OpacityMask == null)
                {
                    Border border = new Border();
                    border.Background = Brushes.White;
                    VisualBrush brush = new VisualBrush();
                    brush.Visual = border;
                    img.OpacityMask = brush;
                    img.SizeChanged += delegate {
                        border.Width = img.ActualWidth;
                        border.Height = img.ActualHeight;
                    };
                }
                if (img.OpacityMask is VisualBrush vb)
                    if (vb.Visual is Border bd)
                        bd.CornerRadius = (CornerRadius)e.NewValue;
            }
        }
    }
}
