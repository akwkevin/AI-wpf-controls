using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    public static class TabAttach
    {
        #region HeaderContent
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static object GetHeaderContent(DependencyObject obj)
        {
            return (object)obj.GetValue(HeaderContentProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderContent(DependencyObject obj, object value)
        {
            obj.SetValue(HeaderContentProperty, value);
        }

        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.RegisterAttached("HeaderContent", typeof(object), typeof(TabAttach), new PropertyMetadata(null));
        #endregion

        #region ControlStyle
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static TabControlStyle GetControlStyle(DependencyObject obj)
        {
            return (TabControlStyle)obj.GetValue(ControlStyleProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetControlStyle(DependencyObject obj, TabControlStyle value)
        {
            obj.SetValue(ControlStyleProperty, value);
        }

        public static readonly DependencyProperty ControlStyleProperty =
            DependencyProperty.RegisterAttached("ControlStyle", typeof(TabControlStyle), typeof(TabAttach), new PropertyMetadata(TabControlStyle.Standard));
        #endregion
    }



    public class TabItemAttach
    {
        #region SelectMode
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TabItemAttach))]
        public static bool GetIsCanClose(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCanCloseProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TabItemAttach))]
        public static void SetIsCanClose(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCanCloseProperty, value);
        }

        public static readonly DependencyProperty IsCanCloseProperty =
            DependencyProperty.RegisterAttached("IsCanClose", typeof(bool), typeof(TabItemAttach), new PropertyMetadata(true));
        #endregion

        /// <summary>
        /// 清除输入框Text值按钮行为开关
        /// </summary>
        public static readonly DependencyProperty CloseButtonProperty = DependencyProperty.RegisterAttached("CloseButton"
            , typeof(bool), typeof(TabItemAttach), new FrameworkPropertyMetadata(false, OnCloseButtonChanged));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TabItemAttach))]
        public static bool GetCloseButton(DependencyObject d)
        {
            return (bool)d.GetValue(CloseButtonProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TabItemAttach))]
        public static void SetCloseButton(DependencyObject obj, bool value)
        {
            obj.SetValue(CloseButtonProperty, value);
        }

        /// <summary>
        /// 绑定清除Text操作的按钮事件
        /// </summary>
        private static void OnCloseButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as System.Windows.Controls.Button;
            if (button != null)
            {
                button.Click -= Button_Click;
                if ((bool)e.NewValue == true)
                {
                    button.Click += Button_Click;
                }
            }
        }

        private static void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TabItem tabitem = VisualHelper.TryFindParent<System.Windows.Controls.TabItem>(sender as Button);
            System.Windows.Controls.TabControl tabControl = VisualHelper.TryFindParent<System.Windows.Controls.TabControl>(tabitem);

            var item = tabControl.ItemContainerGenerator.ItemFromContainer(tabitem);

            IList list;
            if (tabControl.ItemsSource != null)
            {
                list = tabControl.ItemsSource as IList;
            }
            else
            {
                list = tabControl.Items;
            }

            list.Remove(item);
        }
    }
}
