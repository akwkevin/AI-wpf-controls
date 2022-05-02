using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace AIStudio.Wpf.Controls
{
    public enum BadgePlacementMode
    {
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left
    }

    [TemplatePart(Name = PART_BadgeContainer, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_RectWave, Type = typeof(Rectangle))]
    [TemplatePart(Name = PART_ScaleTransform, Type = typeof(ScaleTransform))]
    public class BadgedEx : ContentControl
    {
        public const string PART_BadgeContainer = "PART_BadgeContainer";
        public const string PART_RectWave = "PART_RectWave";
        public const string PART_ScaleTransform = "PART_ScaleTransform";

        protected FrameworkElement _badgeContainer;
        protected Rectangle _rectangle;
        protected ScaleTransform _scaleTransform;

        public static readonly DependencyProperty BadgeProperty = DependencyProperty.Register(
            nameof(Badge), typeof(object), typeof(BadgedEx), new FrameworkPropertyMetadata(default(object), FrameworkPropertyMetadataOptions.AffectsArrange, OnBadgeChanged));

        public object Badge
        {
            get
            {
                return (object)GetValue(BadgeProperty);
            }
            set
            {
                SetValue(BadgeProperty, value);
            }
        }

        public static readonly DependencyProperty BadgeBackgroundProperty = DependencyProperty.Register(
            nameof(BadgeBackground), typeof(Brush), typeof(BadgedEx), new PropertyMetadata(default(Brush)));

        public Brush BadgeBackground
        {
            get
            {
                return (Brush)GetValue(BadgeBackgroundProperty);
            }
            set
            {
                SetValue(BadgeBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty BadgeForegroundProperty = DependencyProperty.Register(
            nameof(BadgeForeground), typeof(Brush), typeof(BadgedEx), new PropertyMetadata(default(Brush)));

        public Brush BadgeForeground
        {
            get
            {
                return (Brush)GetValue(BadgeForegroundProperty);
            }
            set
            {
                SetValue(BadgeForegroundProperty, value);
            }
        }

        public static readonly DependencyProperty BadgeBorderBrushProperty = DependencyProperty.Register(
            nameof(BadgeBorderBrush), typeof(Brush), typeof(BadgedEx), new PropertyMetadata(default(Brush)));

        public Brush BadgeBorderBrush
        {
            get
            {
                return (Brush)GetValue(BadgeBorderBrushProperty);
            }
            set
            {
                SetValue(BadgeBorderBrushProperty, value);
            }
        }

        public static readonly DependencyProperty BadgeBorderThicknessProperty = DependencyProperty.Register(
             nameof(BadgeBorderThickness), typeof(Thickness), typeof(BadgedEx), new PropertyMetadata(default(Thickness)));

        public Thickness BadgeBorderThickness
        {
            get
            {
                return (Thickness)GetValue(BadgeBorderThicknessProperty);
            }
            set
            {
                SetValue(BadgeBorderThicknessProperty, value);
            }
        }

        public static readonly DependencyProperty BadgePaddingProperty = DependencyProperty.Register(
            nameof(BadgePadding), typeof(Thickness), typeof(BadgedEx), new PropertyMetadata(new Thickness(3)));

        public Thickness BadgePadding
        {
            get
            {
                return (Thickness)GetValue(BadgePaddingProperty);
            }
            set
            {
                SetValue(BadgePaddingProperty, value);
            }
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
           nameof(CornerRadius), typeof(CornerRadius), typeof(BadgedEx), new PropertyMetadata(new CornerRadius(11)));

        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        public static readonly DependencyProperty BadgePlacementModeProperty = DependencyProperty.Register(
            nameof(BadgePlacementMode), typeof(BadgePlacementMode), typeof(BadgedEx), new PropertyMetadata(default(BadgePlacementMode)));

        public BadgePlacementMode BadgePlacementMode
        {
            get
            {
                return (BadgePlacementMode)GetValue(BadgePlacementModeProperty);
            }
            set
            {
                SetValue(BadgePlacementModeProperty, value);
            }
        }

        public static readonly RoutedEvent BadgeChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(BadgeChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<object>),
                typeof(BadgedEx));

        public event RoutedPropertyChangedEventHandler<object> BadgeChanged
        {
            add
            {
                AddHandler(BadgeChangedEvent, value);
            }
            remove
            {
                RemoveHandler(BadgeChangedEvent, value);
            }
        }

        private static readonly DependencyPropertyKey IsBadgeSetPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsBadgeSet), typeof(bool), typeof(BadgedEx),
                new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsBadgeSetProperty =
            IsBadgeSetPropertyKey.DependencyProperty;

        public bool IsBadgeSet
        {
            get
            {
                return (bool)GetValue(IsBadgeSetProperty);
            }
            private set
            {
                SetValue(IsBadgeSetPropertyKey, value);
            }
        }

        private static void OnBadgeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (BadgedEx)d;

            instance.IsBadgeSet = !string.IsNullOrWhiteSpace(e.NewValue as string) || (e.NewValue != null && !(e.NewValue is string));

            var args = new RoutedPropertyChangedEventArgs<object>(
                e.OldValue,
                e.NewValue)
            {
                RoutedEvent = BadgeChangedEvent
            };
            instance.RaiseEvent(args);
        }

        public bool IsWaving
        {
            get
            {
                return (bool)GetValue(IsWavingProperty);
            }
            set
            {
                SetValue(IsWavingProperty, value);
            }
        }

        public static readonly DependencyProperty IsWavingProperty =
            DependencyProperty.Register(nameof(IsWaving), typeof(bool), typeof(BadgedEx), new PropertyMetadata(OnIsWavingChanged));

        private static void OnIsWavingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var badge = d as BadgedEx;
            badge.ChangeWaving();
        }

        public static readonly DependencyProperty IsDotProperty =
            DependencyProperty.Register(nameof(IsDot), typeof(bool), typeof(BadgedEx), new PropertyMetadata(false));

        public bool IsDot
        {
            get => (bool)GetValue(IsDotProperty);
            set => SetValue(IsDotProperty, value);
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(nameof(Status), typeof(ControlStatus), typeof(BadgedEx), new PropertyMetadata(ControlStatus.Default));

        public ControlStatus Status
        {
            get => (ControlStatus)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        private void ChangeWaving()
        {
            if (_rectangle == null)
                return;
            if (IsWaving)
            {
                _rectangle.Visibility = Visibility.Visible;
                BeginWave();
            }
            else
            {
                _rectangle.Visibility = Visibility.Collapsed;
                StopWave();
            }
        }

        #region Function

        private void BeginWave()
        {
            var anima1 = new DoubleAnimation()
            {
                From = Badge == null ? 0.4 : 1,
                To = 2,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = TimeSpan.FromSeconds(1.5),
            };
            _scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, anima1);

            var anima2 = new DoubleAnimation()
            {
                From = Badge == null ? 0.4 : 1,
                To = 2,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = TimeSpan.FromSeconds(1.5),
            };
            _scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, anima2);

            var anima3 = new DoubleAnimation()
            {
                To = 0,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = TimeSpan.FromSeconds(1.5),
            };
            _rectangle.BeginAnimation(OpacityProperty, anima3);
        }

        private void StopWave()
        {
            _scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            _scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            _rectangle.BeginAnimation(OpacityProperty, null);
        }
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _badgeContainer = GetTemplateChild(PART_BadgeContainer) as FrameworkElement;

            _rectangle = GetTemplateChild(PART_RectWave) as Rectangle;

            _scaleTransform = _rectangle.RenderTransform as ScaleTransform;

            ChangeWaving();
        }

        public BadgedEx()
        {

        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var result = base.ArrangeOverride(arrangeBounds);
            if (HasContent == false) return result;
            if (_badgeContainer == null) return result;

            var containerDesiredSize = _badgeContainer.DesiredSize;
            if ((containerDesiredSize.Width <= 0.0 || containerDesiredSize.Height <= 0.0)
                && !double.IsNaN(_badgeContainer.ActualWidth) && !double.IsInfinity(_badgeContainer.ActualWidth)
                && !double.IsNaN(_badgeContainer.ActualHeight) && !double.IsInfinity(_badgeContainer.ActualHeight))
            {
                containerDesiredSize = new Size(_badgeContainer.ActualWidth, _badgeContainer.ActualHeight);
            }

            var h = 0 - containerDesiredSize.Width / 2;
            var v = 0 - containerDesiredSize.Height / 2;
            _badgeContainer.Margin = new Thickness(0);
            _badgeContainer.Margin = new Thickness(h, v, h, v);

            return result;
        }
    }

}
