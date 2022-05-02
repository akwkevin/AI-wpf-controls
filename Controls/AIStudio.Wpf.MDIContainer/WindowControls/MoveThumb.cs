using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using AIStudio.Wpf.MDIContainer.Extensions;

namespace AIStudio.Wpf.MDIContainer.WindowControls
{
    public sealed class MoveThumb : Thumb
    {
        static MoveThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MoveThumb), new FrameworkPropertyMetadata(typeof(MoveThumb)));
        }

        public MoveThumb()
        {
            this.DragDelta += this.OnMoveThumbDragDelta;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            var window = VisualTreeExtension.FindMDIWindow(this);

            if (window != null)
            {
                window.Focus();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            var window = VisualTreeExtension.FindMDIWindow(this);

            if (window != null && window.Container != null)
            {
                switch (window.WindowState)
                {
                    case WindowState.Maximized:
                        window.Normalize();
                        break;
                    case WindowState.Normal:
                        window.Maximize();
                        break;
                    case WindowState.Minimized:
                        window.Normalize();
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported WindowsState mode");
                }
            }

            e.Handled = true;
        }

        private void OnMoveThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var window = VisualTreeExtension.FindMDIWindow(this);

            if (window != null)
            {
                if (window.WindowState == WindowState.Maximized)
                {
                    window.Normalize();
                }

                if (window.WindowState != WindowState.Minimized)
                {
                    window.LastLeft = Canvas.GetLeft(window);
                    window.LastTop = Canvas.GetTop(window);

                    Canvas.SetLeft(window, window.LastLeft + e.HorizontalChange);
                    Canvas.SetTop(window, window.LastTop + e.VerticalChange);
                }
            }
        }
    }
}
