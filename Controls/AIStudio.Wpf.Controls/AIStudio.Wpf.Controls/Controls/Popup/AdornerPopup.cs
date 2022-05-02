using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    [DefaultProperty("Child")]
    [ContentProperty("Child")]
    public class AdornerPopup : Control
    {
        #region Fields
        private Window _adornerOfWindow = null;
        private AdornerLayer _adornerLayer = null;
        private FrameworkElementAdorner _adorner = null;
        #endregion

        #region Constructors
        static AdornerPopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AdornerPopup), new FrameworkPropertyMetadata(typeof(AdornerPopup)));
            FocusableProperty.OverrideMetadata(typeof(AdornerPopup), new FrameworkPropertyMetadata(false));
        }

        public AdornerPopup()
        {
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(AdornerPopup_DataContextChanged);
        }
        #endregion

        #region Dependency Properties
        /// <summary>
        /// Popup
        /// </summary>
        public FrameworkElement Child
        {
            get
            {
                return (FrameworkElement)GetValue(ChildProperty);
            }
            set
            {
                SetValue(ChildProperty, value);
            }
        }

        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(FrameworkElement), typeof(AdornerPopup), new PropertyMetadata(null));

        /// <summary>
        /// 是否打开Popup
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return (bool)GetValue(IsOpenProperty);
            }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(AdornerPopup), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsOpenCallback));

        public FrameworkElement PlacementTarget
        {
            get
            {
                return (FrameworkElement)GetValue(PlacementTargetProperty);
            }
            set
            {
                SetValue(PlacementTargetProperty, value);
            }
        }

        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.Register("PlacementTarget", typeof(FrameworkElement), typeof(AdornerPopup), new PropertyMetadata(null, PlacementTargetChangedCallback));


        /// <summary>
        /// 鼠标点击非Popup内容时是否隐藏
        /// </summary>
        public bool StaysOpen
        {
            get
            {
                return (bool)GetValue(StaysOpenProperty);
            }
            set
            {
                SetValue(StaysOpenProperty, value);
            }
        }

        public static readonly DependencyProperty StaysOpenProperty =
            DependencyProperty.Register("StaysOpen", typeof(bool), typeof(AdornerPopup), new PropertyMetadata(true, StaysOpenCallback));

        /// <summary>
        /// 在StaysOpen为False的情况下, 当点击PlacementTarget对象时忽略鼠标点击不关闭Popup
        /// </summary>
        public bool IgnoreTargetEvent
        {
            get
            {
                return (bool)GetValue(IgnoreTargetEventProperty);
            }
            set
            {
                SetValue(IgnoreTargetEventProperty, value);
            }
        }

        public static readonly DependencyProperty IgnoreTargetEventProperty =
            DependencyProperty.Register("IgnoreTargetEvent", typeof(bool), typeof(AdornerPopup), new PropertyMetadata(false));

        public double HorizontalOffset
        {
            get
            {
                return (double)GetValue(HorizontalOffsetProperty);
            }
            set
            {
                SetValue(HorizontalOffsetProperty, value);
            }
        }

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(AdornerPopup), new PropertyMetadata(0.0));

        public double VerticalOffset
        {
            get
            {
                return (double)GetValue(VerticalOffsetProperty);
            }
            set
            {
                SetValue(VerticalOffsetProperty, value);
            }
        }

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(AdornerPopup), new PropertyMetadata(0.0));

        public AdornerPopupPlacement Placement
        {
            get
            {
                return (AdornerPopupPlacement)GetValue(PlacementProperty);
            }
            set
            {
                SetValue(PlacementProperty, value);
            }
        }

        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(AdornerPopupPlacement), typeof(AdornerPopup), new PropertyMetadata(AdornerPopupPlacement.Bottom));

        /// <summary>
        /// AutoClose
        /// </summary>
        public bool AutoClose
        {
            get
            {
                return (bool)GetValue(AutoCloseProperty);
            }
            set
            {
                SetValue(AutoCloseProperty, value);
            }
        }

        public static readonly DependencyProperty AutoCloseProperty =
            DependencyProperty.Register("AutoClose", typeof(bool), typeof(AdornerPopup), new PropertyMetadata(false));
        #endregion

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        #region Event Handlers

        private static void PlacementTargetChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdornerPopup uc = d as AdornerPopup;
            if (uc == null)
                return;

            uc.ReleasePlacementTargetEventHandler(e.OldValue as FrameworkElement);
            uc.RegisterPlacementTargetEventHandler(e.NewValue as FrameworkElement);
        }

        private void OnPlacementTarget_LostFocus(object sender, RoutedEventArgs e)
        {
            if (StaysOpen)
                return;
            if (this.IsOpen)
            {
                Point pos = Mouse.GetPosition(_adorner);
                HitTestResult hitResult = VisualTreeHelper.HitTest(_adorner, pos);

                if (hitResult == null)
                {
                    SetIsOpenCore(false);
                    return;
                }

                // 如果点击对象对Child则返回
                if (VisualHelper.IsDescendantOf(hitResult.VisualHit, _adorner))
                {
                    return;
                }

                pos = Mouse.GetPosition(PlacementTarget);
                hitResult = VisualTreeHelper.HitTest(PlacementTarget, pos);
                if (hitResult == null)
                {
                    SetIsOpenCore(false);
                    return;
                }
                // 如果点击对象PlacementTarget则返回
                if (IgnoreTargetEvent && VisualHelper.IsDescendantOf(hitResult.VisualHit, PlacementTarget))
                {
                    return;
                }

                SetIsOpenCore(false);
            }
        }

        private static void IsOpenCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdornerPopup uc = d as AdornerPopup;
            if (uc == null)
                return;

            if (uc.PlacementTarget == null)
            {
                throw new InvalidOperationException("PlacementTarget is null");
            }

            bool isShow = (bool)e.NewValue;
            if (isShow)
            {
                uc.ShowPopup();
            }
            else
            {
                uc.HidePopup();
            }
        }

        private static void StaysOpenCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdornerPopup uc = d as AdornerPopup;
            if (uc == null)
                return;

            bool newValue = (bool)e.NewValue;

            if (newValue)
            {

                // 如果当前Popup正在和显示中释放鼠标事件
                if (uc.IsOpen)
                {
                    uc.ReleaseMouseDownHandler();
                }
            }
            else
            {
                if (uc.IsOpen)
                {
                    uc.RegisterMouseDownHandler();
                }
            }

        }

        private void AdornerPopup_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateAdornerDataContext();
        }

        private void OnAdornerOfWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (StaysOpen)
                return;

            if (_adorner == null)
                return;
            Point pos = e.GetPosition(_adorner);
            HitTestResult hitResult = VisualTreeHelper.HitTest(_adorner, pos);

            if (hitResult == null)
            {
                SetIsOpenCore(false);
                return;
            }

            // 如果点击对象对Child则返回
            if (VisualHelper.IsDescendantOf(hitResult.VisualHit, _adorner))
            {
                return;
            }

            // 如果点击对象PlacementTarget则返回
            if (IgnoreTargetEvent && VisualHelper.IsDescendantOf(hitResult.VisualHit, PlacementTarget))
            {
                return;
            }

            SetIsOpenCore(false);
            e.Handled = true;
        }

        #endregion

        #region Private Methods
        private void SetIsOpenCore(bool isOpen)
        {
            SetCurrentValue(IsOpenProperty, isOpen);

            // Just notify the source property
            BindingExpression binding = GetBindingExpression(IsOpenProperty);
            if (binding == null) return;

            // Note: IsOpenProperty must be TwoWay or OneWayToSource 
            binding.UpdateSource();
        }

        private void ShowPopup()
        {
            if (_adornerLayer == null)
            {
                _adornerLayer = AdornerLayer.GetAdornerLayer(this);
            }

            if (_adornerLayer != null)
            {
                _adorner = new FrameworkElementAdorner(this.Child, PlacementTarget, this, HorizontalOffset, VerticalOffset);
                _adornerLayer.Add(_adorner);

                _adorner.MouseLeave += _adorner_MouseLeave;

                ScaleTransform scale = null;
                this.Child.RenderTransform = scale = new ScaleTransform();

                DoubleAnimation myDoubleAnimation = new DoubleAnimation();
                myDoubleAnimation.FillBehavior = FillBehavior.HoldEnd;
                myDoubleAnimation.From = 0;
                myDoubleAnimation.To = 1;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                myDoubleAnimation.AutoReverse = false;

                scale.ScaleX = 1;
                scale.ScaleY = 0;
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, myDoubleAnimation);

                if (!StaysOpen)
                {
                    RegisterMouseDownHandler();
                }
            }

        }

        private void _adorner_MouseLeave(object sender, MouseEventArgs e)
        {
            if (AutoClose)
            {
                if (PlacementTarget == null)
                {
                    SetIsOpenCore(false);
                }
                else if (PlacementTarget != null && !PlacementTarget.IsMouseOver)
                {
                    SetIsOpenCore(false);
                }
            }
        }

        private void HidePopup()
        {
            ReleaseMouseDownHandler();
            if (_adorner != null)
            {
                _adorner.DisconnectChild();
                if (_adornerLayer != null)
                {
                    _adornerLayer.Remove(_adorner);
                    _adornerLayer = null;
                }
                this.RemoveLogicalChild(_adorner);
                _adorner = null;
            }
        }

        private void RegisterMouseDownHandler()
        {
            _adornerOfWindow = VisualHelper.GetAncestor<Window>(_adorner);
            if (_adornerOfWindow != null)
            {
                Mouse.AddMouseDownHandler(_adornerOfWindow, OnAdornerOfWindow_PreviewMouseDown);
            }
        }

        private void ReleaseMouseDownHandler()
        {
            if (_adornerOfWindow != null)
            {
                Mouse.RemoveMouseDownHandler(_adornerOfWindow, OnAdornerOfWindow_PreviewMouseDown);
            }
        }

        private void ReleasePlacementTargetEventHandler(FrameworkElement frameworkElement)
        {
            if (frameworkElement == null) return;
            frameworkElement.LostFocus -= OnPlacementTarget_LostFocus;
        }

        private void RegisterPlacementTargetEventHandler(FrameworkElement frameworkElement)
        {
            if (frameworkElement == null) return;
            frameworkElement.LostFocus += OnPlacementTarget_LostFocus;
        }

        private void UpdateAdornerDataContext()
        {
            if (this.Child != null)
            {
                this.Child.DataContext = this.DataContext;
            }
        }
        #endregion

        #region Override Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        #endregion
    }

    public enum AdornerPopupPlacement
    {
        Top,
        Right,
        Left,
        Bottom,
    }
}
