using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = "PART_Rect", Type = typeof(Rectangle))]
    public class AnimatedRectangle : Control
    {
        private Rectangle _rect;

        static AnimatedRectangle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedRectangle), new FrameworkPropertyMetadata(typeof(AnimatedRectangle)));
        }

        public AnimatedRectangle()
        {
            Loaded += AnimatedRectangle_Loaded;
        }

        private void AnimatedRectangle_Loaded(object sender, RoutedEventArgs e)
        {
            Animation();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (_rect != null && _rect.StrokeThickness > 0)
            {
                double length = constraint.Width * 2 + constraint.Height * 2;
                double value = length / _rect.StrokeThickness;
                _rect.StrokeDashArray = new DoubleCollection() { value, value + 10 };
            }
            return base.MeasureOverride(constraint);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _rect = GetTemplateChild("PART_Rect") as Rectangle;
        }

        private void Animation()
        {
            if (_rect.StrokeThickness > 0)
            {
                double length = _rect.ActualWidth * 2 + _rect.ActualHeight * 2;
                double value = length / _rect.StrokeThickness;
                _rect.StrokeDashArray = new DoubleCollection() { value, value + 10 };

                DoubleAnimation offsetAnimation = new DoubleAnimation();
                offsetAnimation.From = value;
                offsetAnimation.To = 0;
                Storyboard.SetTarget(offsetAnimation, _rect);
                Storyboard.SetTargetProperty(offsetAnimation, new PropertyPath(Rectangle.StrokeDashOffsetProperty));

                Storyboard sb = new Storyboard();
                sb.Children.Add(offsetAnimation);
                sb.Begin();
            }
        }
    }
}
