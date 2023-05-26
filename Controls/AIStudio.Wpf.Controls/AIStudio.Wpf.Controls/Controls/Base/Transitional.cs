using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows;

namespace AIStudio.Wpf.Controls
{

    /// <summary>
    /// 转换动画
    /// </summary>
    public class Transitional : ContentControl
    {
        static Transitional()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Transitional), new FrameworkPropertyMetadata(typeof(Transitional)));
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Loaded += Transition_Loaded;
        }

        private void Transition_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsShow && Type != TransitionalType.None)
            {
                ShowAnimation(this);

                if (!PlayEveryTime)
                {
                    Loaded -= Transition_Loaded;
                }
            }
        }

        #region 命令

        /// <summary>
        /// 显示完命令
        /// </summary>
        public static readonly DependencyProperty ShowedCommandProperty =
            DependencyProperty.Register(nameof(ShowedCommand), typeof(ICommand), typeof(Transitional), new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// 显示完命令
        /// </summary>
        public ICommand ShowedCommand
        {
            get
            {
                return (ICommand)GetValue(ShowedCommandProperty);
            }
            set
            {
                SetValue(ShowedCommandProperty, value);
            }
        }

        /// <summary>
        /// 关闭完命令
        /// </summary>
        public static readonly DependencyProperty ClosedCommandProperty =
            DependencyProperty.Register(nameof(ClosedCommand), typeof(ICommand), typeof(Transitional), new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// 关闭完命令
        /// </summary>
        public ICommand ClosedCommand
        {
            get
            {
                return (ICommand)GetValue(ClosedCommandProperty);
            }
            set
            {
                SetValue(ClosedCommandProperty, value);
            }
        }

        #endregion 命令

        #region 事件

        /// <summary>
        /// 显示完事件
        /// </summary>
        public static readonly RoutedEvent ShowedEvent =
           EventManager.RegisterRoutedEvent(nameof(Showed), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Transitional));

        /// <summary>
        /// 显示完事件 handler
        /// </summary>
        public event RoutedEventHandler Showed
        {
            add
            {
                AddHandler(ShowedEvent, value);
            }
            remove
            {
                RemoveHandler(ShowedEvent, value);
            }
        }

        /// <summary>
        /// 关闭完事件
        /// </summary>
        public static readonly RoutedEvent ClosedEvent =
          EventManager.RegisterRoutedEvent(nameof(Closed), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Transitional));

        /// <summary>
        /// 关闭完事件 handler
        /// </summary>
        public event RoutedEventHandler Closed
        {
            add
            {
                AddHandler(ClosedEvent, value);
            }
            remove
            {
                RemoveHandler(ClosedEvent, value);
            }
        }

        #endregion 事件

        #region 依赖属性

        /// <summary>
        /// 是否显示
        /// </summary>
        public static readonly DependencyProperty IsShowProperty =
            DependencyProperty.Register(nameof(IsShow), typeof(bool), typeof(Transitional), new PropertyMetadata(false, OnIsShowChanged));

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow
        {
            get
            {
                return (bool)GetValue(IsShowProperty);
            }
            set
            {
                SetValue(IsShowProperty, value);
            }
        }

        /// <summary>
        /// 每次加载显示动画
        /// </summary>
        public static readonly DependencyProperty PlayEveryTimeProperty =
            DependencyProperty.Register(nameof(PlayEveryTime), typeof(bool), typeof(Transitional), new PropertyMetadata(false));

        /// <summary>
        /// 每次加载显示动画
        /// </summary>
        public bool PlayEveryTime
        {
            get
            {
                return (bool)GetValue(PlayEveryTimeProperty);
            }
            set
            {
                SetValue(PlayEveryTimeProperty, value);
            }
        }

        /// <summary>
        /// 转换类型
        /// </summary>
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(nameof(Type), typeof(TransitionalType), typeof(Transitional), new PropertyMetadata(default(TransitionalType)));

        /// <summary>
        /// 转换类型
        /// </summary>
        public TransitionalType Type
        {
            get
            {
                return (TransitionalType)GetValue(TypeProperty);
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }

        /// <summary>
        /// 动画时长
        /// </summary>
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register(nameof(Duration), typeof(Duration), typeof(Transitional), new PropertyMetadata(default(Duration)));

        /// <summary>
        /// 动画时长
        /// </summary>
        public Duration Duration
        {
            get
            {
                return (Duration)GetValue(DurationProperty);
            }
            set
            {
                SetValue(DurationProperty, value);
            }
        }

        /// <summary>
        /// 动画位置偏移
        /// </summary>
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register(nameof(Offset), typeof(double), typeof(Transitional), new PropertyMetadata(default(double)));

        /// <summary>
        /// 动画位置偏移
        /// </summary>
        public double Offset
        {
            get
            {
                return (double)GetValue(OffsetProperty);
            }
            set
            {
                SetValue(OffsetProperty, value);
            }
        }

        /// <summary>
        /// 动画初始缩放
        /// </summary>
        public static readonly DependencyProperty InitialScaleProperty =
            DependencyProperty.Register(nameof(InitialScale), typeof(double), typeof(Transitional), new PropertyMetadata(default(double)));

        /// <summary>
        /// 动画初始缩放
        /// </summary>
        public double InitialScale
        {
            get
            {
                return (double)GetValue(InitialScaleProperty);
            }
            set
            {
                SetValue(InitialScaleProperty, value);
            }
        }

        /// <summary>
        /// 显示缓动
        /// </summary>
        public static readonly DependencyProperty ShowEasingFunctionProperty =
            DependencyProperty.Register(nameof(ShowEasingFunction), typeof(IEasingFunction), typeof(Transitional), new PropertyMetadata(default(IEasingFunction)));

        /// <summary>
        /// 显示缓动
        /// </summary>
        public IEasingFunction ShowEasingFunction
        {
            get
            {
                return (IEasingFunction)GetValue(ShowEasingFunctionProperty);
            }
            set
            {
                SetValue(ShowEasingFunctionProperty, value);
            }
        }

        /// <summary>
        /// 关闭缓动
        /// </summary>
        public static readonly DependencyProperty CloseEasingFunctionProperty =
            DependencyProperty.Register(nameof(CloseEasingFunction), typeof(IEasingFunction), typeof(Transitional), new PropertyMetadata(default(IEasingFunction)));

        /// <summary>
        /// 关闭缓动
        /// </summary>
        public IEasingFunction CloseEasingFunction
        {
            get
            {
                return (IEasingFunction)GetValue(CloseEasingFunctionProperty);
            }
            set
            {
                SetValue(CloseEasingFunctionProperty, value);
            }
        }

        /// <summary>
        /// 动画进度
        /// 显示完成 = 1
        /// 隐藏完成 = 0
        /// </summary>
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register(nameof(Progress), typeof(double), typeof(Transitional), new PropertyMetadata(default(double)));

        /// <summary>
        /// 动画进度
        /// 显示完成 = 1
        /// 隐藏完成 = 0
        /// </summary>
        public double Progress
        {
            get
            {
                return (double)GetValue(ProgressProperty);
            }
            set
            {
                SetValue(ProgressProperty, value);
            }
        }

        /// <summary>
        /// 是否淡入淡出
        /// </summary>
        public static readonly DependencyProperty IsFadeProperty =
            DependencyProperty.Register(nameof(IsFade), typeof(bool), typeof(Transitional), new PropertyMetadata(false));

        /// <summary>
        /// 是否淡入淡出
        /// </summary>
        public bool IsFade
        {
            get
            {
                return (bool)GetValue(IsFadeProperty);
            }
            set
            {
                SetValue(IsFadeProperty, value);
            }
        }

        /// <summary>
        /// 折叠后的大小
        /// </summary>
        public static readonly DependencyProperty CollapsedSizeProperty =
            DependencyProperty.Register(nameof(CollapsedSize), typeof(double), typeof(Transitional), new PropertyMetadata(default(double), OnIsShowChanged));

        /// <summary>
        /// 折叠后的大小
        /// </summary>
        public double CollapsedSize
        {
            get
            {
                return (double)GetValue(CollapsedSizeProperty);
            }
            set
            {
                SetValue(CollapsedSizeProperty, value);
            }
        }

        #endregion 依赖属性

        /// <summary>
        /// 关闭转换器
        /// </summary>
        /// <returns>关闭是否完成</returns>
        public async Task<bool> Close()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            this.Closed += (a, b) => taskCompletionSource.TrySetResult(true);
            return await taskCompletionSource.Task;
        }

        private static void OnIsShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var transition = d as Transitional;
            ChangeTransitionVisual(transition);
        }

        private static void ChangeTransitionVisual(Transitional transition)
        {
            if (transition.IsLoaded && transition.Type != TransitionalType.None)
            {
                if (transition.IsShow)
                {
                    ShowAnimation(transition);
                }
                else
                {
                    CloseAnimation(transition);
                }
            }
            else if (transition.IsInitialized || transition.Type == TransitionalType.None)
            {
                transition.BeginAnimation(Transitional.ProgressProperty, null);

                if (transition.IsShow)
                {
                    transition.Progress = 1;
                }
                else
                {
                    transition.Progress = 0;
                }
            }
        }

        /// <summary>
        /// 开始显示动画
        /// </summary>
        /// <param name="transition">转换</param>
        public static void ShowAnimation(Transitional transition)
        {
            if (transition.Type == TransitionalType.None)
            {
                return;
            }

            Storyboard storyboard = new Storyboard();
            storyboard.Completed += (sender, e) => {
                var args = new RoutedEventArgs();
                args.RoutedEvent = Transitional.ShowedEvent;
                transition.RaiseEvent(args);
                transition.ShowedCommand?.Execute(null);
            };

            DoubleAnimation progressAnimation;
            if (transition.Type >= TransitionalType.CollapseUp && transition.Type <= TransitionalType.CollapseRight)
            {
                progressAnimation = GetProgressAnimation(transition, 1, transition.ShowEasingFunction);
            }
            else
            {
                progressAnimation = GetProgressAnimation(transition, 1, transition.ShowEasingFunction, from: 0);
            }

            switch (transition.Type)
            {
                case TransitionalType.Fade:
                case TransitionalType.CollapseUp:
                case TransitionalType.CollapseDown:
                case TransitionalType.CollapseLeft:
                case TransitionalType.CollapseRight:
                default:
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.FadeLeft:
                    DoubleAnimation translateAnimation = GetTranslateXAnimation(transition, transition.Offset, 0, transition.ShowEasingFunction);
                    storyboard.Children.Add(translateAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.FadeRight:
                    translateAnimation = GetTranslateXAnimation(transition, -transition.Offset, 0, transition.ShowEasingFunction);
                    storyboard.Children.Add(translateAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.FadeUp:
                    translateAnimation = GetTranslateYAnimation(transition, transition.Offset, 0, transition.ShowEasingFunction);
                    storyboard.Children.Add(translateAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.FadeDown:
                    translateAnimation = GetTranslateYAnimation(transition, -transition.Offset, 0, transition.ShowEasingFunction);
                    storyboard.Children.Add(translateAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.Zoom:
                    DoubleAnimation scaleXAnimation = GetScaleXAnimation(transition, transition.InitialScale, 1, transition.ShowEasingFunction);
                    DoubleAnimation scaleYAnimation = GetScaleYAnimation(transition, transition.InitialScale, 1, transition.ShowEasingFunction);
                    storyboard.Children.Add(scaleXAnimation);
                    storyboard.Children.Add(scaleYAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.ZoomX:
                    scaleXAnimation = GetScaleXAnimation(transition, transition.InitialScale, 1, transition.ShowEasingFunction);
                    storyboard.Children.Add(scaleXAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.ZoomY:
                    scaleYAnimation = GetScaleYAnimation(transition, transition.InitialScale, 1, transition.ShowEasingFunction);
                    storyboard.Children.Add(scaleYAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.ZoomLeft:
                    transition.RenderTransformOrigin = new Point(0, 0.5);
                    scaleXAnimation = GetScaleXAnimation(transition, transition.InitialScale, 1, transition.ShowEasingFunction);
                    storyboard.Children.Add(scaleXAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.ZoomRight:
                    transition.RenderTransformOrigin = new Point(1, 0.5);
                    scaleXAnimation = GetScaleXAnimation(transition, transition.InitialScale, 1, transition.ShowEasingFunction);
                    storyboard.Children.Add(scaleXAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.ZoomUp:
                    transition.RenderTransformOrigin = new Point(0.5, 0);
                    scaleYAnimation = GetScaleYAnimation(transition, transition.InitialScale, 1, transition.ShowEasingFunction);
                    storyboard.Children.Add(scaleYAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.ZoomDown:
                    transition.RenderTransformOrigin = new Point(0.5, 1);
                    scaleYAnimation = GetScaleYAnimation(transition, transition.InitialScale, 1, transition.ShowEasingFunction);
                    storyboard.Children.Add(scaleYAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;
            }

            storyboard.Begin();
        }

        /// <summary>
        /// 开始关闭动画
        /// </summary>
        /// <param name="transition">转换</param>
        public static void CloseAnimation(Transitional transition)
        {
            if (transition.Type == TransitionalType.None)
            {
                return;
            }

            Storyboard storyboard = new Storyboard();
            storyboard.Completed += (sender, e) => {
                var args = new RoutedEventArgs();
                args.RoutedEvent = Transitional.ClosedEvent;
                transition.RaiseEvent(args);
                transition.ClosedCommand?.Execute(null);
            };

            DoubleAnimation progressAnimation;
            if (transition.Type >= TransitionalType.CollapseUp && transition.Type <= TransitionalType.CollapseRight)
            {
                var progress = GetCollapsedProgress(transition);
                progressAnimation = GetProgressAnimation(transition, progress, transition.CloseEasingFunction);
            }
            else
            {
                progressAnimation = GetProgressAnimation(transition, 0, transition.CloseEasingFunction, from: 1);
            }

            switch (transition.Type)
            {
                case TransitionalType.Fade:
                case TransitionalType.CollapseUp:
                case TransitionalType.CollapseDown:
                case TransitionalType.CollapseLeft:
                case TransitionalType.CollapseRight:
                default:
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.FadeLeft:
                    DoubleAnimation translateAnimation = GetTranslateXAnimation(transition, 0, transition.Offset, transition.CloseEasingFunction);
                    storyboard.Children.Add(translateAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.FadeRight:
                    translateAnimation = GetTranslateXAnimation(transition, 0, -transition.Offset, transition.CloseEasingFunction);
                    storyboard.Children.Add(translateAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.FadeUp:
                    translateAnimation = GetTranslateYAnimation(transition, 0, transition.Offset, transition.CloseEasingFunction);
                    storyboard.Children.Add(translateAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.FadeDown:
                    translateAnimation = GetTranslateYAnimation(transition, 0, -transition.Offset, transition.CloseEasingFunction);
                    storyboard.Children.Add(translateAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.Zoom:
                    DoubleAnimation scaleXAnimation = GetScaleXAnimation(transition, 1, transition.InitialScale, transition.CloseEasingFunction);
                    DoubleAnimation scaleYAnimation = GetScaleYAnimation(transition, 1, transition.InitialScale, transition.CloseEasingFunction);
                    storyboard.Children.Add(scaleXAnimation);
                    storyboard.Children.Add(scaleYAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.ZoomX:
                    scaleXAnimation = GetScaleXAnimation(transition, 1, transition.InitialScale, transition.CloseEasingFunction);
                    storyboard.Children.Add(scaleXAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.ZoomY:
                    scaleYAnimation = GetScaleYAnimation(transition, 1, transition.InitialScale, transition.CloseEasingFunction);
                    storyboard.Children.Add(scaleYAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.ZoomLeft:
                    transition.RenderTransformOrigin = new Point(0, 0.5);
                    scaleXAnimation = GetScaleXAnimation(transition, 1, transition.InitialScale, transition.CloseEasingFunction);
                    storyboard.Children.Add(scaleXAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.ZoomRight:
                    transition.RenderTransformOrigin = new Point(1, 0.5);
                    scaleXAnimation = GetScaleXAnimation(transition, 1, transition.InitialScale, transition.CloseEasingFunction);
                    storyboard.Children.Add(scaleXAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.ZoomUp:
                    transition.RenderTransformOrigin = new Point(0.5, 0);
                    scaleYAnimation = GetScaleYAnimation(transition, 1, transition.InitialScale, transition.CloseEasingFunction);
                    storyboard.Children.Add(scaleYAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;

                case TransitionalType.ZoomDown:
                    transition.RenderTransformOrigin = new Point(0.5, 1);
                    scaleYAnimation = GetScaleYAnimation(transition, 1, transition.InitialScale, transition.CloseEasingFunction);
                    storyboard.Children.Add(scaleYAnimation);
                    storyboard.Children.Add(progressAnimation);
                    break;
            }

            storyboard.Begin();
        }

        private static DoubleAnimation GetProgressAnimation(Transitional transition, double to, IEasingFunction easing, double? from = null)
        {

            DoubleAnimation progressAnimation = new DoubleAnimation()
            {
                From = transition.Progress == to ? (to > 0 ? 0 : 1) : from,
                To = to,
                Duration = transition.Duration,
                EasingFunction = easing,
            };

            if (from != null)
            {
                progressAnimation.From = from;
            }

            Storyboard.SetTargetProperty(progressAnimation, new PropertyPath(ProgressProperty));
            Storyboard.SetTarget(progressAnimation, transition);

            return progressAnimation;
        }

        private static DoubleAnimation GetTranslateXAnimation(Transitional transition, double from, double to, IEasingFunction easing)
        {
            DoubleAnimation transformAnimation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = transition.Duration,
                EasingFunction = easing,
            };

            Storyboard.SetTargetProperty(transformAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"));
            Storyboard.SetTarget(transformAnimation, transition);
            return transformAnimation;
        }

        private static DoubleAnimation GetTranslateYAnimation(Transitional transition, double from, double to, IEasingFunction easing)
        {
            DoubleAnimation transformAnimation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = transition.Duration,
                EasingFunction = easing,
            };

            Storyboard.SetTargetProperty(transformAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));
            Storyboard.SetTarget(transformAnimation, transition);
            return transformAnimation;
        }

        private static DoubleAnimation GetScaleXAnimation(Transitional transition, double from, double to, IEasingFunction easing)
        {
            DoubleAnimation transformAnimation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = transition.Duration,
                EasingFunction = easing,
            };

            Storyboard.SetTargetProperty(transformAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            Storyboard.SetTarget(transformAnimation, transition);
            return transformAnimation;
        }

        private static DoubleAnimation GetScaleYAnimation(Transitional transition, double from, double to, IEasingFunction easing)
        {
            DoubleAnimation transformAnimation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = transition.Duration,
                EasingFunction = easing,
            };

            Storyboard.SetTargetProperty(transformAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
            Storyboard.SetTarget(transformAnimation, transition);
            return transformAnimation;
        }

        private static double GetCollapsedProgress(Transitional transition)
        {
            switch (transition.Type)
            {
                case TransitionalType.CollapseUp:
                case TransitionalType.CollapseDown:
                    return transition.ActualHeight == 0 ? 0 : transition.CollapsedSize / transition.ActualHeight;

                case TransitionalType.CollapseLeft:
                case TransitionalType.CollapseRight:
                    return transition.ActualWidth == 0 ? 0 : transition.CollapsedSize / transition.ActualWidth;

                default:
                    return 0;
            }
        }
    }

    /// <summary>
    /// 转换类型
    /// </summary>
    public enum TransitionalType
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 淡入
        /// </summary>
        Fade,

        /// <summary>
        /// 向左淡入
        /// </summary>
        FadeLeft,

        /// <summary>
        /// 向右淡入
        /// </summary>
        FadeRight,

        /// <summary>
        /// 向上淡入
        /// </summary>
        FadeUp,

        /// <summary>
        /// 向下淡入
        /// </summary>
        FadeDown,

        /// <summary>
        /// 缩放
        /// </summary>
        Zoom,

        /// <summary>
        /// X 轴缩放
        /// </summary>
        ZoomX,

        /// <summary>
        /// Y 轴缩放
        /// </summary>
        ZoomY,

        /// <summary>
        /// 向左缩放
        /// </summary>
        ZoomLeft,

        /// <summary>
        /// 向右缩放
        /// </summary>
        ZoomRight,

        /// <summary>
        /// 向上缩放
        /// </summary>
        ZoomUp,

        /// <summary>
        /// 向下缩放
        /// </summary>
        ZoomDown,

        /// <summary>
        /// 向上折叠
        /// </summary>
        CollapseUp,

        /// <summary>
        /// 向下折叠
        /// </summary>
        CollapseDown,

        /// <summary>
        /// 向左折叠
        /// </summary>
        CollapseLeft,

        /// <summary>
        /// 向右折叠
        /// </summary>
        CollapseRight,
    }
}
