using System.Windows;
using System.Windows.Media;

namespace AIStudio.Wpf.Panels.Controls
{
    using System.Windows.Controls.Primitives;

    public class WindowButton : ButtonBase
    {
        static WindowButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowButton), new FrameworkPropertyMetadata(typeof(WindowButton)));
        }

        public Brush Icon
        {
            get
            {
                return (Brush)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(Brush), typeof(WindowButton), new UIPropertyMetadata(Brushes.Transparent));
    }
}
