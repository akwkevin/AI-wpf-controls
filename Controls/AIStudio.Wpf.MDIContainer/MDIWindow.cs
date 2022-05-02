using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using AIStudio.Wpf.MDIContainer.Extensions;
using AIStudio.Wpf.MDIContainer.WindowControls;
using AIStudio.Wpf.MDIContainer.Events;

namespace AIStudio.Wpf.MDIContainer
{
    using System.ComponentModel;

    [TemplatePart(Name = "PART_Border", Type = typeof(Border))]
    [TemplatePart(Name = "PART_BorderGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_Header", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ButtonBar", Type = typeof(StackPanel))]
    [TemplatePart(Name = "PART_ButtonBar_CloseButton", Type = typeof(WindowButton))]
    [TemplatePart(Name = "PART_ButtonBar_MaximizeButton", Type = typeof(WindowButton))]
    [TemplatePart(Name = "PART_ButtonBar_MinimizeButton", Type = typeof(WindowButton))]
    [TemplatePart(Name = "PART_BorderContent", Type = typeof(Border))]
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_MoverThumb", Type = typeof(MoveThumb))]
    [TemplatePart(Name = "PART_ResizerThumb", Type = typeof(ResizeThumb))]
    [TemplatePart(Name = "PART_Thumblr", Type = typeof(Image))]
    public sealed class MDIWindow : ContentControl
    {
        internal double LastTop { get; set; }
        internal double LastLeft { get; set; }
        internal double LastWidth { get; set; }
        internal double LastHeight { get; set; }

        internal MDIContainer Container { get; private set; }
        internal WindowState PreviousWindowState { get; set; }

        public Image Tumblr { get; private set; }

        private WindowButton closeButton;
        private WindowButton maximizeButton;
        private WindowButton minimizeButton;

        static MDIWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MDIWindow), new FrameworkPropertyMetadata(typeof(MDIWindow)));
        }
        public MDIWindow()
        {
            Height = 200;
            Width = 200;
        }

        internal void Initialize(MDIContainer container)
        {
            this.Container = container;
            this.Container.SizeChanged += OnContainerSizeChanged;

            this.LastHeight = this.ActualHeight;
            this.LastWidth = this.ActualWidth;
        }

        private void OnContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.Width += e.NewSize.Width - e.PreviousSize.Width;
                this.Height += e.NewSize.Height - e.PreviousSize.Height;

                this.RemoveWindowLock();
            }

            if (this.WindowState == WindowState.Minimized)
            {
                Canvas.SetTop(this, this.Container.ActualHeight - 32);
            }
        }

        public override void OnApplyTemplate()
        {
            this.closeButton = this.GetTemplateChild("PART_ButtonBar_CloseButton") as WindowButton;
            if (this.closeButton != null)
            {
                this.closeButton.Click += CloseWindow;
            }

            this.maximizeButton = this.GetTemplateChild("PART_ButtonBar_MaximizeButton") as WindowButton;
            if (this.maximizeButton != null)
            {
                this.maximizeButton.Click += ToggleMaximizeWindow;
            }

            this.minimizeButton = this.GetTemplateChild("PART_ButtonBar_MinimizeButton") as WindowButton;
            if (this.minimizeButton != null)
            {
                this.minimizeButton.Click += ToggleMinimizeWindow;
            }

            this.Tumblr = this.GetTemplateChild("PART_Tumblr") as Image;
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MDIWindow), new UIPropertyMetadata(false));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MDIWindow), new PropertyMetadata(string.Empty));

        public WindowState WindowState
        {
            get { return (WindowState)GetValue(WindowStateProperty); }
            set { SetValue(WindowStateProperty, value); }
        }

        public static readonly DependencyProperty WindowStateProperty =
            DependencyProperty.Register("WindowState", typeof(WindowState), typeof(MDIWindow), new PropertyMetadata(WindowState.Normal, OnWindowStateChanged));

        public static readonly RoutedEvent ClosingEvent = EventManager.RegisterRoutedEvent(
            "Closing", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MDIWindow));

        [Bindable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsCloseButtonEnabled
        {
            get { return (bool)GetValue(IsCloseButtonEnabledProperty); }
            set { SetValue(IsCloseButtonEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsCloseButtonEnabledProperty =
            DependencyProperty.Register("IsCloseButtonEnabled", typeof(bool), typeof(MDIWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.NotDataBindable));

        public event RoutedEventHandler Closing
        {
            add { AddHandler(ClosingEvent, value); }
            remove { RemoveHandler(ClosingEvent, value); }
        }

        public static readonly RoutedEvent FocusChangedEvent = EventManager.RegisterRoutedEvent(
           "FocusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MDIWindow));

        public event RoutedEventHandler FocusChanged
        {
            add { AddHandler(FocusChangedEvent, value); }
            remove { RemoveHandler(FocusChangedEvent, value); }
        }

        public bool HasDropShadow
        {
            get { return (bool)GetValue(HasDropShadowProperty); }
            set { SetValue(HasDropShadowProperty, value); }
        }

        public static readonly DependencyProperty HasDropShadowProperty =
            DependencyProperty.Register("HasDropShadow", typeof(bool), typeof(MDIWindow), new UIPropertyMetadata(true));

        public delegate void WindowStateChangedRoutedEventHandler(object sender, WindowStateChangedEventArgs e);

        public static readonly RoutedEvent WindowStateChangedEvent = EventManager.RegisterRoutedEvent(
           "WindowStateChanged", RoutingStrategy.Bubble, typeof(WindowStateChangedRoutedEventHandler), typeof(MDIWindow));

        public event WindowStateChangedRoutedEventHandler WindowStateChanged
        {
            add { AddHandler(WindowStateChangedEvent, value); }
            remove { RemoveHandler(WindowStateChangedEvent, value); }
        }

        public bool CanClose
        {
            get { return (bool)GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.Register("CanClose", typeof(bool), typeof(MDIWindow), new FrameworkPropertyMetadata(true));

        public bool IsResizable
        {
            get { return (bool)GetValue(IsResizableProperty); }
            set { SetValue(IsResizableProperty, value); }
        }

        public static readonly DependencyProperty IsResizableProperty =
            DependencyProperty.Register("IsResizable", typeof(bool), typeof(MDIWindow), new UIPropertyMetadata(true));


        private static void OnWindowStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var window = obj as MDIWindow;
            if (window != null)
            {
                window.PreviousWindowState = (WindowState)e.OldValue;

                var args = new WindowStateChangedEventArgs(WindowStateChangedEvent, (WindowState)e.OldValue, (WindowState)e.NewValue);
                window.RaiseEvent(args);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            this.Focus();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);

            this.IsSelected = false;
            Panel.SetZIndex(this, 0);

            RaiseEvent(new RoutedEventArgs(FocusChangedEvent, this.DataContext));
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            this.IsSelected = true;
            Panel.SetZIndex(this, 1);

            RaiseEvent(new RoutedEventArgs(FocusChangedEvent, this.DataContext));
        }

        private void ToggleMaximizeWindow(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.ToggleMaximize();
        }

        private void ToggleMinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.ToggleMinimize();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            var canCloseBinding = BindingOperations.GetBindingExpression(this, CanCloseProperty);
            if (canCloseBinding != null)
            {
                canCloseBinding.UpdateTarget();
            }

            if (this.CanClose)
            {
                RaiseEvent(new RoutedEventArgs(ClosingEvent));
            }
        }
    }
}
