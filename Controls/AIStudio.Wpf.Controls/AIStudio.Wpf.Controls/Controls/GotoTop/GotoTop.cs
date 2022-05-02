using System;
using System.Windows;
using System.Windows.Controls;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    public class GotoTop : Button
    {
        private Action _gotoTopAction;

        private System.Windows.Controls.ScrollViewer _scrollViewer;

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            nameof(Target), typeof(DependencyObject), typeof(GotoTop), new PropertyMetadata(default(DependencyObject)));

        public DependencyObject Target
        {
            get => (DependencyObject)GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        public GotoTop() => Loaded += (s, e) => CreateGotoAction(Target);

        public virtual void CreateGotoAction(DependencyObject obj)
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
            }
            _scrollViewer = VisualHelper.GetChild<System.Windows.Controls.ScrollViewer>(obj);
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;

                _gotoTopAction = (() => {
                    //_scrollViewer.ScrollToTop();
                    _scrollViewer.AnimateScrollIntoVerticalOffset(0, AnimationTime);
                });
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (AutoHiding)
            {
                this.Visibility = (e.VerticalOffset >= HidingHeight) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static readonly DependencyProperty AnimatedProperty = DependencyProperty.Register(
            nameof(Animated), typeof(bool), typeof(GotoTop), new PropertyMetadata(true));

        public bool Animated
        {
            get => (bool)GetValue(AnimatedProperty);
            set => SetValue(AnimatedProperty, value);
        }

        public static readonly DependencyProperty AnimationTimeProperty = DependencyProperty.Register(
            nameof(AnimationTime), typeof(double), typeof(GotoTop), new PropertyMetadata(200d));

        public double AnimationTime
        {
            get => (double)GetValue(AnimationTimeProperty);
            set => SetValue(AnimationTimeProperty, value);
        }

        public static readonly DependencyProperty HidingHeightProperty = DependencyProperty.Register(
            nameof(HidingHeight), typeof(double), typeof(GotoTop), new PropertyMetadata(default(double)));

        public double HidingHeight
        {
            get => (double)GetValue(HidingHeightProperty);
            set => SetValue(HidingHeightProperty, value);
        }

        public static readonly DependencyProperty AutoHidingProperty = DependencyProperty.Register(
            nameof(AutoHiding), typeof(bool), typeof(GotoTop), new PropertyMetadata(true));

        public bool AutoHiding
        {
            get => (bool)GetValue(AutoHidingProperty);
            set => SetValue(AutoHidingProperty, value);
        }

        protected override void OnClick()
        {
            base.OnClick();

            _gotoTopAction?.Invoke();
        }
    }
}