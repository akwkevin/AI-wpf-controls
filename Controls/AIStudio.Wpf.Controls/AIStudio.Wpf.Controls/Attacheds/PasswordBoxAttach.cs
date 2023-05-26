using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AIStudio.Wpf.Controls
{
    public static class PasswordBoxAttach
    {
        public static readonly DependencyProperty CapsLockIconProperty
            = DependencyProperty.RegisterAttached(
                "CapsLockIcon",
                typeof(object),
                typeof(PasswordBoxAttach),
                new PropertyMetadata("!", OnCapsLockIconChanged));

        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [Category(AppName.AIStudio)]
        public static object GetCapsLockIcon(PasswordBox element)
        {
            return element.GetValue(CapsLockIconProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [Category(AppName.AIStudio)]
        public static void SetCapsLockIcon(PasswordBox element, object value)
        {
            element.SetValue(CapsLockIconProperty, value);
        }

        private static void OnCapsLockIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                PasswordBox pb = (PasswordBox)d;

                pb.KeyDown -= RefreshCapsLockStatus;
                pb.GotFocus -= RefreshCapsLockStatus;
                pb.PreviewGotKeyboardFocus -= RefreshCapsLockStatus;
                pb.LostFocus -= HandlePasswordBoxLostFocus;

                if (e.NewValue != null)
                {
                    pb.KeyDown += RefreshCapsLockStatus;
                    pb.GotFocus += RefreshCapsLockStatus;
                    pb.PreviewGotKeyboardFocus += RefreshCapsLockStatus;
                    pb.LostFocus += HandlePasswordBoxLostFocus;
                }
            }
        }

        private static void RefreshCapsLockStatus(object sender, RoutedEventArgs e)
        {
            var fe = FindCapsLockIndicator(sender as Control);
            if (fe != null)
            {
                fe.Visibility = Keyboard.IsKeyToggled(Key.CapsLock) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private static void HandlePasswordBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var fe = FindCapsLockIndicator(sender as Control);
            if (fe != null)
            {
                fe.Visibility = Visibility.Collapsed;
            }
        }

        private static FrameworkElement FindCapsLockIndicator(Control pb)
        {
            return pb?.Template?.FindName("PART_CapsLockIndicator", pb) as FrameworkElement;
        }

        public static readonly DependencyProperty CapsLockWarningToolTipProperty
            = DependencyProperty.RegisterAttached(
                "CapsLockWarningToolTip",
                typeof(object),
                typeof(PasswordBoxAttach),
                new PropertyMetadata("Caps lock is on"));

        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static object GetCapsLockWarningToolTip(PasswordBox element)
        {
            return element.GetValue(CapsLockWarningToolTipProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static void SetCapsLockWarningToolTip(PasswordBox element, object value)
        {
            element.SetValue(CapsLockWarningToolTipProperty, value);
        }

        public static readonly DependencyProperty RevealButtonContentProperty
            = DependencyProperty.RegisterAttached(
                "RevealButtonContent",
                typeof(object),
                typeof(PasswordBoxAttach),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the content of the RevealButton.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static object GetRevealButtonContent(DependencyObject d)
        {
            return (object)d.GetValue(RevealButtonContentProperty);
        }

        /// <summary>
        /// Sets the content of the RevealButton.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static void SetRevealButtonContent(DependencyObject obj, object value)
        {
            obj.SetValue(RevealButtonContentProperty, value);
        }

        public static readonly DependencyProperty RevealButtonContentTemplateProperty
            = DependencyProperty.RegisterAttached(
                "RevealButtonContentTemplate",
                typeof(DataTemplate),
                typeof(PasswordBoxAttach),
                new FrameworkPropertyMetadata(null));

        /// <summary> 
        /// Gets the data template used to display the content of the RevealButton.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static DataTemplate GetRevealButtonContentTemplate(DependencyObject d)
        {
            return (DataTemplate)d.GetValue(RevealButtonContentTemplateProperty);
        }

        /// <summary> 
        /// Sets the data template used to display the content of the RevealButton.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static void SetRevealButtonContentTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(RevealButtonContentTemplateProperty, value);
        }













        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static bool GetShowClearButton(PasswordBox element)
        {
            return (bool)element.GetValue(ShowClearButtonProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static void SetShowClearButton(PasswordBox element, bool value)
        {
            element.SetValue(ShowClearButtonProperty, value);
        }

        /// <summary>
        ///     是否显示清除按钮
        /// </summary>
        public static readonly DependencyProperty ShowClearButtonProperty = DependencyProperty.RegisterAttached(
            "ShowClearButton", typeof(bool), typeof(PasswordBoxAttach), new PropertyMetadata(false));


        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static bool GetShowEyeButton(PasswordBox element)
        {
            return (bool)element.GetValue(ShowEyeButtonProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static void SetShowEyeButton(PasswordBox element, bool value)
        {
            element.SetValue(ShowEyeButtonProperty, value);
        }
        public static readonly DependencyProperty ShowEyeButtonProperty = DependencyProperty.RegisterAttached(
            "ShowEyeButton", typeof(bool), typeof(PasswordBoxAttach), new PropertyMetadata(false));

    }
}
