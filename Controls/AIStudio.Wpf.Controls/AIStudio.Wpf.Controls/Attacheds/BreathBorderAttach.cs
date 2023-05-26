using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace AIStudio.Wpf.Controls
{
    public static class BreathBorderAttach
    {
        /// <summary>
        /// 点击后呼吸动画
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(AppName.AIStudio)]
        public static void SetIsBreath(DependencyObject element, bool value)
        {
            element.SetValue(IsBreathProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(AppName.AIStudio)]

        public static bool GetIsBreath(DependencyObject element)
        {
            return (bool)element.GetValue(IsBreathProperty);
        }

        public static readonly DependencyProperty IsBreathProperty = DependencyProperty.RegisterAttached(
            "IsBreath", typeof(bool), typeof(BreathBorderAttach), new FrameworkPropertyMetadata(default(bool), OnIsBreathPropertyChanged));


        private static void OnIsBreathPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (depObj is FrameworkElement element)
            {
                if ((bool)e.NewValue == true)
                {
                    element.PreviewMouseLeftButtonDown -= Element_PreviewMouseLeftButtonDown;
                    element.PreviewMouseLeftButtonDown += Element_PreviewMouseLeftButtonDown;
                }
                else
                {
                    element.PreviewMouseLeftButtonDown -= Element_PreviewMouseLeftButtonDown;
                }
            }
        }

        private static void Element_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                ScaleTransform scale = new ScaleTransform();
                TransformGroup group = new TransformGroup();
                group.Children.Add(scale);
                element.RenderTransform = group;

                //定义圆心位置
                element.RenderTransformOrigin = new Point(0.5, 0.5);
                EasingFunctionBase easeFunction = new SineEase()
                {
                    EasingMode = EasingMode.EaseOut,
                };

                // 动画参数
                DoubleAnimation scaleAnimation = new DoubleAnimation()
                {
                    From = 0.8,
                    To = 1.1,
                    EasingFunction = easeFunction,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 1, 0)),
                    FillBehavior = FillBehavior.Stop
                };

                element.Effect = new DropShadowEffect() { BlurRadius = 25, Color = Colors.Red, ShadowDepth = 0, };

                EventHandler completionHandler = null;
                completionHandler = (sender2, args) => {
                    element.Effect = null;
                    scaleAnimation.Completed -= completionHandler;
                };

                scaleAnimation.Completed += completionHandler;

                // 执行动画
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

            }
        }


    }
}
