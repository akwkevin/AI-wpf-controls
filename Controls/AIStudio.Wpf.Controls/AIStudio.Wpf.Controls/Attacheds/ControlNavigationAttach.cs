using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public static class ControlNavigationAttach
    {
        /// <summary>
        /// 是否使用上下键导航
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(AppName.AIStudio)]
        public static void SetNavWithUpDownKey(DependencyObject element, string value)
        {
            element.SetValue(NavWithUpDownKeyProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(AppName.AIStudio)]
        public static string GetNavWithUpDownKey(DependencyObject element)
        {
            return (string)element.GetValue(NavWithUpDownKeyProperty);
        }

        public static readonly DependencyProperty NavWithUpDownKeyProperty = DependencyProperty.RegisterAttached(
            "NavWithUpDownKey", typeof(string), typeof(ControlNavigationAttach), new FrameworkPropertyMetadata("Left^Right^Up^Down^Tab^Return", FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// 默认焦点控件
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(AppName.AIStudio)]
        public static double GetNavWithUpDownDefaultIndex(DependencyObject cc)
        {
            return (double)cc.GetValue(NavWithUpDownDefaultIndexProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(AppName.AIStudio)]
        public static void SetNavWithUpDownDefaultIndex(DependencyObject cc, double value)
        {
            cc.SetValue(NavWithUpDownDefaultIndexProperty, value);
        }

        public static readonly DependencyProperty NavWithUpDownDefaultIndexProperty = DependencyProperty.RegisterAttached("NavWithUpDownDefaultIndex", typeof(double), typeof(ControlNavigationAttach),
            new UIPropertyMetadata(0d));

        /// <summary>
        /// 是否使用上下键导航
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(AppName.AIStudio)]
        public static void SetNavWithUpDown(DependencyObject element, bool value)
        {
            element.SetValue(NavWithUpDownProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(AppName.AIStudio)]
        public static bool GetNavWithUpDown(DependencyObject element)
        {
            return (bool)element.GetValue(NavWithUpDownProperty);
        }

        public static readonly DependencyProperty NavWithUpDownProperty = DependencyProperty.RegisterAttached(
            "NavWithUpDown", typeof(bool), typeof(ControlNavigationAttach), new FrameworkPropertyMetadata(default(bool), OnNavWithUpDownPropertyChanged));


        private static void OnNavWithUpDownPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (depObj is FrameworkElement element && (bool)e.NewValue == true)
            {
                if (element.IsLoaded)
                {
                    InitControlNavigation(element);
                }
                else
                {
                    element.Loaded -= Element_Loaded;
                    element.Loaded += Element_Loaded;
                }
            }

        }

        private static void Element_Loaded(object sender, RoutedEventArgs e)
        {
            InitControlNavigation(sender as FrameworkElement);
        }

        private static void InitControlNavigation(FrameworkElement element)
        {
            List<Control> controlList = new List<Control>();

            controlList = NavigationIndexCalculate(element, GetRealNavigationIndex(element), GetNavigationMultiple(element));
            controlList = controlList.OrderBy(p => GetRealNavigationIndex(p)).ToList();
            foreach (var control in controlList)
            {
                SetNavigationControlList(control, controlList);

                if (GetRealNavigationIndex(control) == GetNavWithUpDownDefaultIndex(element))
                    control.Focus();
            }
        }

        private static List<Control> NavigationIndexCalculate(FrameworkElement element, double baseoffset, double multiple)
        {
            List<Control> controlList = new List<Control>();
            //遍历所有子对象
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); ++i)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                if (child == null)
                    continue;

                if (GetNavigationIndex(child) != null)
                {
                    if (child is Control control)
                    {
                        if (control.Focusable == true)
                        {
                            SetNavigationMultiple(control, multiple);
                            SetNavigationOffset(control, baseoffset);
                            controlList.Add(control);
                            //System.Diagnostics.Debug.WriteLine(GetRealNavigationIndex(control));

                            control.PreviewKeyDown -= Control_PreviewKeyDown;
                            control.PreviewKeyDown += Control_PreviewKeyDown;
                            continue;
                        }
                    }

                    controlList.AddRange(NavigationIndexCalculate(child, GetNavigationIndex(child).Value * multiple + baseoffset, GetNavigationMultiple(child) * multiple));
                }
                else
                {
                    controlList.AddRange(NavigationIndexCalculate(child, baseoffset, multiple));
                }

            }

            return controlList;
        }

        private static void Control_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var keyword = GetNavWithUpDownKey(sender as DependencyObject);
            string[] keys = new string[] { };
            if (!string.IsNullOrEmpty(keyword))
            {
                keys = keyword.Split('^');
            }

            if (keys.Contains(e.Key.ToString()) && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (e.Key == Key.Left || e.Key == Key.Up)
                {
                    e.Handled = MoveFocusLeft(sender as Control);
                }
                else if (e.Key == Key.Right || e.Key == Key.Down || e.Key == Key.Tab || e.Key == Key.Return)
                {
                    e.Handled = MoveFocusRight(sender as Control);
                }
            }
        }

        private static bool MoveFocusLeft(Control control)
        {
            var controlList = GetNavigationControlList(control);
            if (controlList != null)
            {
                int index = controlList.IndexOf(control);
                if (index == 0)
                {
                    return FocusLeft(controlList[controlList.Count - 1]);
                }
                else
                {
                    return FocusLeft(controlList[index - 1]);
                }
            }
            return false;
        }
        private static bool FocusLeft(Control control)
        {
            if (control.IsVisible)
            {
                control.Focus();
                return true;
            }

            return MoveFocusLeft(control);
        }

        private static bool MoveFocusRight(Control control)
        {
            var controlList = GetNavigationControlList(control);
            if (controlList != null)
            {
                int index = controlList.IndexOf(control);
                if (controlList.Count - index - 1 == 0)
                {
                    return FocusRight(controlList[0]);
                }
                else
                {
                    return FocusRight(controlList[index + 1]);
                }
            }
            return false;
        }
        private static bool FocusRight(Control control)
        {
            if (control.IsVisible)
            {
                control.Focus();
                return true;
            }

            return MoveFocusRight(control);
        }

        public static double GetRealNavigationIndex(DependencyObject control)
        {
            return (GetNavigationIndex(control) ?? 0d) * GetNavigationMultiple(control) + GetNavigationOffset(control);
        }
        public static double? GetNavigationIndex(DependencyObject cc)
        {
            return (double?)cc.GetValue(NavigationIndexProperty);
        }

        public static void SetNavigationIndex(DependencyObject cc, double? value)
        {
            cc.SetValue(NavigationIndexProperty, value);
        }

        public static readonly DependencyProperty NavigationIndexProperty = DependencyProperty.RegisterAttached("NavigationIndex", typeof(double?), typeof(ControlNavigationAttach),
            new UIPropertyMetadata(null));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(AppName.AIStudio)]
        public static double GetNavigationMultiple(DependencyObject cc)
        {
            return (double)cc.GetValue(NavigationMultipleProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(AppName.AIStudio)]
        public static void SetNavigationMultiple(DependencyObject cc, double value)
        {
            cc.SetValue(NavigationMultipleProperty, value);
        }

        public static readonly DependencyProperty NavigationMultipleProperty = DependencyProperty.RegisterAttached("NavigationMultiple", typeof(double), typeof(ControlNavigationAttach),
            new UIPropertyMetadata(1d));

        private static double GetNavigationOffset(DependencyObject cc)
        {
            return (double)cc.GetValue(NavigationOffsetProperty);
        }

        private static void SetNavigationOffset(DependencyObject cc, double value)
        {
            cc.SetValue(NavigationOffsetProperty, value);
        }

        private static readonly DependencyProperty NavigationOffsetProperty = DependencyProperty.RegisterAttached("NavigationOffset", typeof(double), typeof(ControlNavigationAttach),
            new UIPropertyMetadata(0d));


        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(AppName.AIStudio)]
        private static List<Control> GetNavigationControlList(DependencyObject cc)
        {
            return (List<Control>)cc.GetValue(NavigationControlListProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        [Category(AppName.AIStudio)]
        private static void SetNavigationControlList(DependencyObject cc, List<Control> value)
        {
            cc.SetValue(NavigationControlListProperty, value);
        }

        private static readonly DependencyProperty NavigationControlListProperty = DependencyProperty.RegisterAttached("NavigationControlList", typeof(List<Control>), typeof(ControlNavigationAttach),
            new UIPropertyMetadata(null));
    }
}
