using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    public class WindowBaseTitleBar : Control
    {
        #region LeftWindowCommands 标题栏左侧
        public static readonly DependencyProperty LeftWindowCommandsProperty = DependencyProperty.Register(nameof(LeftWindowCommands), typeof(object), typeof(WindowBaseTitleBar), new PropertyMetadata(null, OnLeftWindowCommandsChanged));

        /// <summary>
        /// Gets/sets the left window commands that hosts the user commands.
        /// </summary>
        public object LeftWindowCommands
        {
            get
            {
                return (object)GetValue(LeftWindowCommandsProperty);
            }
            set
            {
                SetValue(LeftWindowCommandsProperty, value);
            }
        }

        private static void OnLeftWindowCommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WindowBaseTitleBar element = d as WindowBaseTitleBar;
            if (element != null)
            {
                element.Loaded += LeftElement_Loaded;
            }
        }

        private static void LeftElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is WindowBaseTitleBar titleBar)
            {
                WindowBase window = titleBar.TryFindParent<WindowBase>();
                if (window != null)
                {
                    if (DesignerProperties.GetIsInDesignMode((sender as WindowBaseTitleBar)) == true)
                    {
                        window.LeftWindowCommands = new Button() { Content = "LeftWindowCommands" };
                    }
                    else
                    {
                        window.LeftWindowCommands = (sender as WindowBaseTitleBar).LeftWindowCommands;
                    }

                    if (window.LeftWindowCommands is FrameworkElement element)
                    {
                        element.DataContext = (sender as WindowBaseTitleBar).DataContext;
                    }
                }
            }
        }
        #endregion

        #region RightWindowCommands 标题栏右侧
        public static readonly DependencyProperty RightWindowCommandsProperty = DependencyProperty.Register(nameof(RightWindowCommands), typeof(object), typeof(WindowBaseTitleBar), new PropertyMetadata(null, OnRightWindowCommandsChanged));

        /// <summary>
        /// Gets/sets the right window commands that hosts the user commands.
        /// </summary>
        public object RightWindowCommands
        {
            get
            {
                return (object)GetValue(RightWindowCommandsProperty);
            }
            set
            {
                SetValue(RightWindowCommandsProperty, value);
            }
        }

        private static void OnRightWindowCommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WindowBaseTitleBar element = d as WindowBaseTitleBar;
            if (element != null)
            {
                element.Loaded += RightElement_Loaded;
            }
        }

        private static void RightElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is WindowBaseTitleBar titleBar)
            {
                WindowBase window = titleBar.TryFindParent<WindowBase>();
                if (window != null)
                {
                    if (DesignerProperties.GetIsInDesignMode((sender as WindowBaseTitleBar)) == true)
                    {
                        window.RightWindowCommands = new Button() { Content = "RightWindowCommands" };
                    }
                    else
                    {
                        window.RightWindowCommands = (sender as WindowBaseTitleBar).RightWindowCommands;
                    }

                    if (window.RightWindowCommands is FrameworkElement element)
                    {
                        element.DataContext = (sender as WindowBaseTitleBar).DataContext;
                    }
                }
            }
        }
        #endregion
    }
}
