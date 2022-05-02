using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using AIStudio.Wpf.MDIContainer.Events;

namespace AIStudio.Wpf.MDIContainer
{
    public sealed class MDIContainer : System.Windows.Controls.Primitives.Selector
    {
        private IList InternalItemSource { get; set; }
        internal int MinimizedWindowsCount { get; private set; }

        public bool LastChildrenFill
        {
            get { return (bool)GetValue(LastChildrenFillProperty); }
            set { SetValue(LastChildrenFillProperty, value); }
        }

        public static readonly DependencyProperty LastChildrenFillProperty =
            DependencyProperty.Register("LastChildrenFill", typeof(bool), typeof(MDIContainer), new PropertyMetadata(false));

        static MDIContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIContainer), new FrameworkPropertyMetadata(typeof(MDIContainer)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MDIWindow();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var window = element as MDIWindow;
            if (window != null)
            {
                window.IsCloseButtonEnabled = this.InternalItemSource != null;
                window.FocusChanged += OnWindowFocusChanged;
                window.Closing += OnWindowClosing;
                window.WindowStateChanged += OnWindowStateChanged;
                window.Initialize(this);

                Canvas.SetTop(window, 32);
                Canvas.SetLeft(window, 32);

                window.Focus();
            }

            base.PrepareContainerForItemOverride(element, item);
        }

        private void OnWindowStateChanged(object sender, WindowStateChangedEventArgs e)
        {
            if (e.NewValue == WindowState.Minimized)
            {
                this.MinimizedWindowsCount++;
            }
            else if (e.OldValue == WindowState.Minimized)
            {
                this.MinimizedWindowsCount--;
            }
        }

        private void OnWindowClosing(object sender, RoutedEventArgs e)
        {
            var window = sender as MDIWindow;
            if (window != null && window.DataContext != null)
            {
                if (this.InternalItemSource != null)
                {
                    this.InternalItemSource.Remove(window.DataContext);
                }

                // clear
                window.FocusChanged -= OnWindowFocusChanged;
                window.Closing -= OnWindowClosing;
                window.WindowStateChanged -= OnWindowStateChanged;
                window.DataContext = null;
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (newValue != null && newValue is IList)
            {
                this.InternalItemSource = newValue as IList;
            }
        }

        private void OnWindowFocusChanged(object sender, RoutedEventArgs e)
        {
            if (((MDIWindow)sender).IsFocused)
            {
                this.SelectedItem = e.OriginalSource;
            }
        }
    }
}
