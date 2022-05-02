using System.Windows;
using System.Windows.Controls.Primitives;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// popup帮助类
    /// 实现popup与PlacementTarget关联控件的联动，PlacementTarget可以是Button等等
    /// 使用方式：
    /// 1、xaml添加PopupAttachHelper的引用
    /// 2、popup 使用PopupAttachHelper.IsAutoSetPopup设置为True
    /// </summary>
    public class PopupAttachHelper : DependencyObject
    {

        private static Popup GetWindowPopup(DependencyObject obj)
        {
            return (Popup)obj.GetValue(WindowPopupProperty);
        }

        private static void SetWindowPopup(DependencyObject obj, Popup value)
        {
            obj.SetValue(WindowPopupProperty, value);
        }

        private static readonly DependencyProperty WindowPopupProperty = DependencyProperty.RegisterAttached("WindowPopup", typeof(Popup), typeof(PopupAttachHelper));

        public static bool GetIsAutoSetPopup(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAutoSetPopupProperty);
        }

        public static void SetIsAutoSetPopup(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAutoSetPopupProperty, value);
        }

        public bool IsAutoSetPopup
        {
            get
            {
                return (bool)GetValue(IsAutoSetPopupProperty);
            }
            set
            {
                SetValue(IsAutoSetPopupProperty, value);
            }
        }

        public static readonly DependencyProperty IsAutoSetPopupProperty =
            DependencyProperty.RegisterAttached("IsAutoSetPopup", typeof(bool), typeof(PopupAttachHelper), new UIPropertyMetadata(false, IsAutoSetPopupChanged));

        private static void IsAutoSetPopupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Popup popup = d as Popup;
            if (popup == null) return;
            popup.Loaded += Popup_Loaded;
        }

        private static void Popup_Loaded(object sender, RoutedEventArgs e)
        {
            var popup = (Popup)sender;
            popup.StaysOpen = true;
            popup.PreviewMouseUp -= Popup_PreviewMouseUp;
            popup.PreviewMouseUp += Popup_PreviewMouseUp;
            var target = GetPlacementTarget(popup);
            if (target == null) return;
            SetWindowPopup(target, popup);
            //点击PlacementTarget控件设置popup弹出与关闭
            target.PreviewMouseLeftButtonDown -= Target_PreviewMouseLeftButtonDown;
            target.PreviewMouseLeftButtonDown += Target_PreviewMouseLeftButtonDown;
            target.LostFocus -= Target_LostFocus;
            target.LostFocus += Target_LostFocus;
            //控件样式发生变化更新pop位置（窗体移动，大小变化，控件移动）
            target.LayoutUpdated += (a, b) => {
                if (!popup.IsOpen)
                {
                    return;
                }
                var methodInfo = typeof(Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                methodInfo?.Invoke(popup, null);
            };
        }

        private static void Popup_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (sender as Popup).IsOpen = false;
        }

        private static void Target_LostFocus(object sender, RoutedEventArgs e)
        {
            var popup = GetWindowPopup((DependencyObject)sender);
            if (popup.IsOpen)
            {
                popup.IsOpen = false;
            }
        }

        private static void Target_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var popup = GetWindowPopup((DependencyObject)sender);
            if (popup.IsOpen)
            {
                popup.IsOpen = false;
            }
            else
            {
                popup.IsOpen = true;
            }
            UIElement target = sender as UIElement;
            Window window = Window.GetWindow(target);
            SetWindowPopupPlacementTarget(window, target);
            window.MouseUp -= Window_Mouseup;
            window.MouseUp += Window_Mouseup;
        }

        private static void Window_Mouseup(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var target = GetWindowPopupPlacementTarget((DependencyObject)sender);
            var popup = GetWindowPopup(target);
            if (popup.IsOpen)
            {
                popup.IsOpen = false;
            }
        }

        /// <summary>
        /// 获取popup的PlacementTarget对象
        /// </summary>
        /// <param name="popup"></param>
        /// <returns></returns>
        private static UIElement GetPlacementTarget(Popup popup)
        {
            return popup.PlacementTarget;
        }

        private static UIElement GetWindowPopupPlacementTarget(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(WindowPopupPlacementTargetProperty);
        }

        private static void SetWindowPopupPlacementTarget(DependencyObject obj, UIElement value)
        {
            obj.SetValue(WindowPopupPlacementTargetProperty, value);
        }

        private static readonly DependencyProperty WindowPopupPlacementTargetProperty = DependencyProperty.RegisterAttached("WindowPopupPlacementTarget", typeof(UIElement), typeof(PopupAttachHelper));

    }
}
