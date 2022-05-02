using AIStudio.Wpf.MDIContainer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AIStudio.Wpf.MDIContainer.Extensions
{
    internal static class WindowBehaviorExtension
    {
        public static void Maximize(this MDIWindow window)
        {
            if (window.IsResizable)
            {
                Canvas.SetTop(window, 0.0);
                Canvas.SetLeft(window, 0.0);

                AnimateResize(window, window.Container.ActualWidth - 4, window.Container.ActualHeight - 4, true);

                window.WindowState = WindowState.Maximized;
            }
        }

        public static void Normalize(this MDIWindow window)
        {
            Canvas.SetTop(window, window.LastTop);
            Canvas.SetLeft(window, window.LastLeft);

            AnimateResize(window, window.LastWidth, window.LastHeight, false);

            window.WindowState = WindowState.Normal;
        }

        public static void Minimize(this MDIWindow window)
        {
            //var index = window.Container.MinimizedWindowsCount;
            int index = 0;
            foreach (var child in VisualTreeExtension.GetChildObjects<MDIWindow>(window.Container).Where(p => p.WindowState == WindowState.Minimized).OrderBy(p => Canvas.GetLeft(p)).ToList())
            {
                if (Canvas.GetLeft(child) != index * 205)
                {
                    break;
                }
                index++;
            }

            window.LastWidth = window.ActualWidth;
            window.LastHeight = window.ActualHeight;
            Canvas.SetTop(window, window.Container.ActualHeight - 32);
            Canvas.SetLeft(window, index * 205);

            RemoveWindowLock(window);
            AnimateResize(window, 200, 32, true);

            window.WindowState = WindowState.Minimized;

            window.Tumblr.Source = window.CreateSnapshot();
        }

        public static void AnimateResize(MDIWindow window, double newWidth, double newHeight, bool lockWindow)
        {
            window.LayoutTransform = new ScaleTransform();

            var widthAnimation = new DoubleAnimation(window.ActualWidth, newWidth, new Duration(TimeSpan.FromMilliseconds(10)));
            var heightAnimation = new DoubleAnimation(window.ActualHeight, newHeight, new Duration(TimeSpan.FromMilliseconds(10)));

            if (lockWindow == false)
            {
                widthAnimation.Completed += (s, e) => window.BeginAnimation(FrameworkElement.WidthProperty, null);
                heightAnimation.Completed += (s, e) => window.BeginAnimation(FrameworkElement.HeightProperty, null);
            }

            window.BeginAnimation(FrameworkElement.WidthProperty, widthAnimation, HandoffBehavior.Compose);
            window.BeginAnimation(FrameworkElement.HeightProperty, heightAnimation, HandoffBehavior.Compose);
        }

        public static void ToggleMaximize(this MDIWindow window)
        {
            if (window.WindowState == WindowState.Maximized)
            {
                window.Normalize();
            }
            else
            {
                window.Maximize();
            }
        }

        public static void ToggleMinimize(this MDIWindow window)
        {
            if (window.WindowState != WindowState.Minimized)
            {
                window.Minimize();
            }
            else
            {
                switch (window.PreviousWindowState)
                {
                    case WindowState.Maximized:
                        window.Maximize();
                        break;
                    case WindowState.Normal:
                        window.Normalize();
                        break;
                    default:
                        throw new NotSupportedException("Invalid WindowState");
                }
            }
        }

        public static void RemoveWindowLock(this MDIWindow window)
        {
            window.BeginAnimation(FrameworkElement.WidthProperty, null);
            window.BeginAnimation(FrameworkElement.HeightProperty, null);
        }
    }

    public static class MDIContainerExtension
    {
        public static void Rank(this MDIContainer container, MdiLayout mdiLayout)
        {
            var windows = VisualTreeExtension.GetChildObjects<MDIWindow>(container).ToArray();
            if (windows.Length == 0) return;

            switch (mdiLayout)
            {
                case MdiLayout.Cascade:
                    {
                        double titleheight = 28;
                        double width = Math.Max(0, container.ActualWidth - (windows.Length - 1) * titleheight - 4);
                        double height = Math.Max(0, container.ActualHeight - (windows.Length - 1) * titleheight - 4);
                        for (int i = 0; i < windows.Length; i++)
                        {
                            Canvas.SetTop(windows[i], i * titleheight);
                            Canvas.SetLeft(windows[i], i * titleheight);
                            WindowBehaviorExtension.AnimateResize(windows[i], width, height, true);
                            windows[i].WindowState = WindowState.Normal;
                        }
                        break;
                    }
                case MdiLayout.TileHorizontal:
                    {
                        int col = (int)Math.Ceiling(Math.Sqrt(windows.Length));
                        int row = (int)Math.Ceiling(((double)windows.Length / col));
                        double width = Math.Max(0, (container.ActualWidth - 4) / col);
                        double height = Math.Max(0, (container.ActualHeight - 4) / row);
                        for (int i = 0; i < row; i++)
                        {
                            for (int j = 0; j < col; j++)
                            {
                                int index = i * col + j;
                                if (index >= windows.Length)
                                    break;
                  
                                Canvas.SetTop(windows[index], height * i);
                                Canvas.SetLeft(windows[index], width * j);

                                if (index == windows.Length - 1 && container.LastChildrenFill)
                                { 
                                    WindowBehaviorExtension.AnimateResize(windows[index], container.ActualWidth - 4 - width * j, height, true);
                                }
                                else
                                {
                                    WindowBehaviorExtension.AnimateResize(windows[index], width, height, true);
                                }
                                windows[index].WindowState = WindowState.Normal;
                            }
                        }
                        break;
                    }
                case MdiLayout.TileVertical:
                    {
                        int row = (int)Math.Ceiling(Math.Sqrt(windows.Length));
                        int col = (int)Math.Ceiling(((double)windows.Length / row));
                        double width = Math.Max(0, (container.ActualWidth - 4) / col);
                        double height = Math.Max(0, (container.ActualHeight - 4) / row);
                        for (int i = 0; i < col; i++)
                        {
                            for (int j = 0; j < row; j++)
                            {
                                int index = i * row + j;
                                if (index >= windows.Length)
                                    break;
                                Canvas.SetTop(windows[index], height * j);
                                Canvas.SetLeft(windows[index], width * i);
                                if (index == windows.Length - 1 && container.LastChildrenFill)
                                {
                                    WindowBehaviorExtension.AnimateResize(windows[index], width, container.ActualHeight - 4 - height * j, true);
                                }
                                else
                                {
                                    WindowBehaviorExtension.AnimateResize(windows[index], width, height, true);
                                }
                                windows[index].WindowState = WindowState.Normal;
                            }
                        }
                        break;
                    }
            }
        }
    }
}