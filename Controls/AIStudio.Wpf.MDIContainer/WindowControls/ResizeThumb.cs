using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using AIStudio.Wpf.MDIContainer.Extensions;

namespace AIStudio.Wpf.MDIContainer.WindowControls
{
    public sealed class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            this.DragDelta += this.OnResizing;
        }

        private void OnResizing(object sender, DragDeltaEventArgs e)
        {
            var window = VisualTreeExtension.FindMDIWindow(this);

            if (window != null)
            {
                if (window.IsFocused == false)
                {
                    window.Focus();
                }

                window.Height = window.ActualHeight;
                window.Width = window.ActualWidth;

                switch (this.VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        var deltaVertical = Math.Min(-e.VerticalChange, window.ActualHeight - window.MinHeight);
                        window.Height -= deltaVertical;
                        break;
                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, window.ActualHeight - window.MinHeight);
                        Canvas.SetTop(window, Canvas.GetTop(window) + deltaVertical);
                        window.Height -= deltaVertical;
                        break;
                    default:
                        break;
                }

                switch (this.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        var deltaHorizontal = Math.Min(e.HorizontalChange, window.ActualWidth - window.MinWidth);
                        Canvas.SetLeft(window, Canvas.GetLeft(window) + deltaHorizontal);
                        window.Width -= deltaHorizontal;
                        break;
                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange, window.ActualWidth - window.MinWidth);
                        window.Width -= deltaHorizontal;
                        break;
                    default:
                        break;
                }
            }

            e.Handled = true;
        }
    }
}
