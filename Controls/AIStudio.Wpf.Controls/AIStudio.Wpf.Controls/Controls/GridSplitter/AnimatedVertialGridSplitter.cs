using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for AnimatedVertialGridSplitter.xaml
    /// </summary>
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_UpperEllipse", Type = typeof(Ellipse))]
    [TemplatePart(Name = "PART_Line", Type = typeof(Line))]
    [TemplatePart(Name = "PART_LowerEllipse", Type = typeof(Ellipse))]
    public partial class AnimatedVertialGridSplitter : GridSplitter
    {
        private Grid _root;
        private Ellipse _upperEllipse;
        private Line _Line;
        private Ellipse _lowerEllipse;

        static AnimatedVertialGridSplitter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedVertialGridSplitter), new FrameworkPropertyMetadata(typeof(AnimatedVertialGridSplitter)));
        }

        public AnimatedVertialGridSplitter()
        {
            Loaded += AnimatedGridSplitter_Loaded;
        }

        private void AnimatedGridSplitter_Loaded(object sender, RoutedEventArgs e)
        {
            Animation();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _root = GetTemplateChild("PART_Root") as Grid;
            _upperEllipse = GetTemplateChild("PART_UpperEllipse") as Ellipse;
            _Line = GetTemplateChild("PART_Line") as Line;
            _lowerEllipse = GetTemplateChild("PART_LowerEllipse") as Ellipse;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (constraint.Height > 0)
            {
                TransformGroup tg = _upperEllipse.RenderTransform as TransformGroup;
                if (tg != null && !tg.IsFrozen)
                {
                    TranslateTransform tf = new TranslateTransform();
                    tf.Y = -(constraint.Height / 2 - 6);
                    tg.Children[0] = tf;
                }

                TransformGroup tg2 = _lowerEllipse.RenderTransform as TransformGroup;
                if (tg2 != null && !tg2.IsFrozen)
                {
                    TranslateTransform tf = new TranslateTransform();
                    tf.Y = constraint.Height / 2 - 6;
                    tg2.Children[0] = tf;
                }
            }
            return base.MeasureOverride(constraint);
        }

        private void Animation()
        {
            CircleEase easing = new CircleEase();
            easing.EasingMode = EasingMode.EaseOut;

            DoubleAnimation lineAnimation = new DoubleAnimation();
            lineAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            lineAnimation.From = 0;
            lineAnimation.To = 1;
            lineAnimation.EasingFunction = easing;
            Storyboard.SetTarget(lineAnimation, _Line);
            Storyboard.SetTargetProperty(lineAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));

            DoubleAnimation upperAnimation = new DoubleAnimation();
            upperAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            upperAnimation.From = 0;
            upperAnimation.To = -(this.ActualHeight / 2 - 6);
            upperAnimation.EasingFunction = easing;
            Storyboard.SetTarget(upperAnimation, _upperEllipse);
            Storyboard.SetTargetProperty(upperAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)"));

            DoubleAnimation lowerAnimation = new DoubleAnimation();
            lowerAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            lowerAnimation.From = 0;
            lowerAnimation.To = this.ActualHeight / 2 - 6;
            lowerAnimation.EasingFunction = easing;
            Storyboard.SetTarget(lowerAnimation, _lowerEllipse);
            Storyboard.SetTargetProperty(lowerAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)"));

            Storyboard sb = new Storyboard();
            sb.Children.Add(lineAnimation);
            sb.Children.Add(upperAnimation);
            sb.Children.Add(lowerAnimation);
            sb.Begin();
        }
    }
}
