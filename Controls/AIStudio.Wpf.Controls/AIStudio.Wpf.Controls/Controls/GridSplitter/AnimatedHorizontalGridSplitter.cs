using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for AnimatedHorizontalGridSplitter.xaml
    /// </summary>
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_LeftEllipse", Type = typeof(Ellipse))]
    [TemplatePart(Name = "PART_Line", Type = typeof(Line))]
    [TemplatePart(Name = "PART_RightEllipse", Type = typeof(Ellipse))]
    public partial class AnimatedHorizontalGridSplitter : GridSplitter
    {
        private Grid _root;
        private Ellipse _leftEllipse;
        private Line _Line;
        private Ellipse _rightEllipse;

        static AnimatedHorizontalGridSplitter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedHorizontalGridSplitter), new FrameworkPropertyMetadata(typeof(AnimatedHorizontalGridSplitter)));
        }

        public AnimatedHorizontalGridSplitter()
        {
            Loaded += AnimatedHorizontalGridSplitter_Loaded;
        }

        private void AnimatedHorizontalGridSplitter_Loaded(object sender, RoutedEventArgs e)
        {
            Animation();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _root = GetTemplateChild("PART_Root") as Grid;
            _leftEllipse = GetTemplateChild("PART_LeftEllipse") as Ellipse;
            _Line = GetTemplateChild("PART_Line") as Line;
            _rightEllipse = GetTemplateChild("PART_RightEllipse") as Ellipse;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (constraint.Width > 0)
            {
                TransformGroup tg = _leftEllipse.RenderTransform as TransformGroup;
                if (tg != null && !tg.IsFrozen)
                {
                    TranslateTransform tf = new TranslateTransform();
                    tf.X = -(constraint.Width / 2 - 6);
                    tg.Children[0] = tf;
                }

                TransformGroup tg2 = _rightEllipse.RenderTransform as TransformGroup;
                if (tg2 != null && !tg2.IsFrozen)
                {
                    TranslateTransform tf = new TranslateTransform();
                    tf.X = constraint.Width / 2 - 6;
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
            Storyboard.SetTargetProperty(lineAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));

            DoubleAnimation leftAnimation = new DoubleAnimation();
            leftAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            leftAnimation.From = 0;
            leftAnimation.To = -(this.ActualWidth / 2 - 6);
            leftAnimation.EasingFunction = easing;
            Storyboard.SetTarget(leftAnimation, _leftEllipse);
            Storyboard.SetTargetProperty(leftAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));

            DoubleAnimation rightAnimation = new DoubleAnimation();
            rightAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            rightAnimation.From = 0;
            rightAnimation.To = this.ActualWidth / 2 - 6;
            rightAnimation.EasingFunction = easing;
            Storyboard.SetTarget(rightAnimation, _rightEllipse);
            Storyboard.SetTargetProperty(rightAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));

            Storyboard sb = new Storyboard();
            sb.Children.Add(lineAnimation);
            sb.Children.Add(leftAnimation);
            sb.Children.Add(rightAnimation);
            sb.Begin();
        }
    }
}
