using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AIStudio.Wpf.Controls
{
    public static class ScrollViewerAttach
    {
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(ScrollViewerAttach), new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

        public static void SetVerticalOffset(FrameworkElement target, double value)
        {
            target.SetValue(VerticalOffsetProperty, value);
        }
        public static double GetVerticalOffset(FrameworkElement target)
        {
            return (double)target.GetValue(VerticalOffsetProperty);
        }
        private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = target as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }

        public static readonly DependencyProperty MouseWheelProperty = DependencyProperty.RegisterAttached("MouseWheel", typeof(bool), typeof(ScrollViewerAttach), new UIPropertyMetadata(false, OnMouseWheelChanged));

        public static void SetMouseWheel(FrameworkElement target, bool value)
        {
            target.SetValue(MouseWheelProperty, value);
        }
        public static bool GetMouseWheel(FrameworkElement target)
        {
            return (bool)target.GetValue(MouseWheelProperty);
        }
        private static void OnMouseWheelChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fElement = target as FrameworkElement;
            if (fElement != null)
            {
                if ((bool)e.NewValue == true)
                {
                    fElement.PreviewMouseWheel += (sender, args) => {
                        var eventArg = new MouseWheelEventArgs(args.MouseDevice, args.Timestamp, args.Delta);
                        eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                        eventArg.Source = sender;
                        fElement.RaiseEvent(eventArg);
                    };
                }
            }
        }

        internal static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.RegisterAttached(
           "SyncHorizontalOffset", typeof(double), typeof(ScrollViewerAttach), new PropertyMetadata(default(double), OnSyncHorizontalOffsetChanged));

        private static void OnSyncHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;
            scrollViewer?.ScrollToHorizontalOffset((double)e.NewValue);
        }

        internal static void SetSyncHorizontalOffset(DependencyObject element, double value)
        {
            element.SetValue(HorizontalOffsetProperty, value);
        }

        internal static double GetSyncHorizontalOffset(DependencyObject element)
        {
            return (double)element.GetValue(HorizontalOffsetProperty);
        }

        public static readonly DependencyProperty IsAutoHideEnabledProperty = DependencyProperty.RegisterAttached(
            "IsAutoHideEnabled", typeof(bool), typeof(ScrollViewerAttach), new PropertyMetadata(default(bool)));

        public static void SetIsAutoHideEnabled(DependencyObject element, bool value)
        {
            element.SetValue(IsAutoHideEnabledProperty, value);
        }

        public static bool GetIsAutoHideEnabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsAutoHideEnabledProperty);
        }

        public static readonly DependencyProperty CornerRectangleVisibilityProperty = DependencyProperty.RegisterAttached(
            "CornerRectangleVisibility", typeof(Visibility), typeof(ScrollViewerAttach), new PropertyMetadata(default(Visibility)));

        public static void SetCornerRectangleVisibility(DependencyObject element, Visibility value)
        {
            element.SetValue(CornerRectangleVisibilityProperty, value);
        }

        public static Visibility GetCornerRectangleVisibility(DependencyObject element)
        {
            return (Visibility)element.GetValue(CornerRectangleVisibilityProperty);
        }

        public static readonly DependencyProperty ShowSeparatorsProperty = DependencyProperty.RegisterAttached(
            "ShowSeparators", typeof(bool), typeof(ScrollViewerAttach), new PropertyMetadata(default(bool)));

        public static void SetShowSeparators(DependencyObject element, bool value)
        {
            element.SetValue(ShowSeparatorsProperty, value);
        }

        public static bool GetShowSeparators(DependencyObject element)
        {
            return (bool)element.GetValue(ShowSeparatorsProperty);
        }

        public static readonly DependencyProperty IgnorePaddingProperty = DependencyProperty.RegisterAttached(
            "IgnorePadding", typeof(bool), typeof(ScrollViewerAttach), new PropertyMetadata(true));

        public static void SetIgnorePadding(DependencyObject element, bool value) => element.SetValue(IgnorePaddingProperty, value);
        public static bool GetIgnorePadding(DependencyObject element) => (bool)element.GetValue(IgnorePaddingProperty);
    }
}
