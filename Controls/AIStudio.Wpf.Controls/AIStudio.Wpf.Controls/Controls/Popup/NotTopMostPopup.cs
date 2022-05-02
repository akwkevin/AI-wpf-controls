using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace AIStudio.Wpf.Controls
{
    public class NotTopMostPopup : Popup
    {
        private System.Windows.Window _window;
        protected override void OnOpened(EventArgs e)
        {
            var hwnd = ((HwndSource)PresentationSource.FromVisual(this.Child)).Handle;
            RECT rect;

            if (GetWindowRect(hwnd, out rect))
            {
                SetWindowPos(hwnd, -2, rect.Left, rect.Top, (int)this.Width, (int)this.Height, 0);
            }

            _window = System.Windows.Window.GetWindow(this);
            _window.PreviewMouseDown -= Window_PreviewMouseDown;
            _window.PreviewMouseDown += Window_PreviewMouseDown;
            _window.LocationChanged -= Window_LocationChanged;
            _window.LocationChanged += Window_LocationChanged;

            RaiseOpening();

            if (AutoClose)
            {
                FrameworkElement target = this.PlacementTarget as FrameworkElement;
                if (target != null)
                {
                    target.MouseLeave += (s, ev) => {
                        if (timerCloseRecentPopup == null)
                        {
                            timerCloseRecentPopup = new DispatcherTimer();
                            timerCloseRecentPopup.Interval = new TimeSpan(0, 0, 1);
                            timerCloseRecentPopup.Tag = this;
                            timerCloseRecentPopup.Tick += closePopup;
                        }
                        timerCloseRecentPopup.Stop();
                        timerCloseRecentPopup.Start();
                    };
                }
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            var offset = HorizontalOffset;
            HorizontalOffset = offset + 1;
            HorizontalOffset = offset;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (_window != null)
            {
                _window.PreviewMouseDown -= Window_PreviewMouseDown;
            }
            RaiseClosing();
        }

        /// <summary>
        /// 定时关闭 更多 popup窗口
        /// </summary>
        private DispatcherTimer timerCloseRecentPopup;
        private void closePopup(object state, EventArgs e)
        {
            Popup pop = timerCloseRecentPopup.Tag as Popup;
            if (pop == null)
            {
                //todo timer里面的Assert没有对话框出来
                Debug.Assert(true, "pop==null");
                return;
            }

            FrameworkElement target = pop.PlacementTarget as FrameworkElement;

            if (!pop.IsMouseOver)
            {
                if (target != null)
                {
                    if (!target.IsMouseOver)
                    {
                        pop.IsOpen = false;
                        timerCloseRecentPopup.Stop();
                    }

                }
                else
                {
                    pop.IsOpen = false;
                    timerCloseRecentPopup.Stop();
                }
            }

        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = PlacementTarget as FrameworkElement;
            if (element != null && !IsMouseOver && !element.IsMouseOver)
                IsOpen = false;
        }

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
            DependencyProperty.Register("AutoClose", typeof(bool), typeof(NotTopMostPopup), new PropertyMetadata(false));

        #region Event
        public static readonly RoutedEvent OpeningEvent = EventManager.RegisterRoutedEvent("Opening", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Control));
        public event RoutedEventHandler Opening
        {
            add
            {
                AddHandler(OpeningEvent, value);
            }
            remove
            {
                RemoveHandler(OpeningEvent, value);
            }
        }
        void RaiseOpening()
        {
            var arg = new RoutedEventArgs(OpeningEvent);
            RaiseEvent(arg);
        }

        public static readonly RoutedEvent ClosingEvent = EventManager.RegisterRoutedEvent("Closing", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Control));
        public event RoutedEventHandler Closing
        {
            add
            {
                AddHandler(ClosingEvent, value);
            }
            remove
            {
                RemoveHandler(ClosingEvent, value);
            }
        }
        void RaiseClosing()
        {
            var arg = new RoutedEventArgs(ClosingEvent);
            RaiseEvent(arg);
        }
        #endregion

        #region P/Invoke imports & definitions

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        #endregion
    }
}
