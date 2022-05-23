using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    public class Form : ItemsControl
    {
        #region AttachedProperty : HeaderWidthProperty
        public static readonly DependencyProperty HeaderWidthProperty
            = DependencyProperty.RegisterAttached("HeaderWidth", typeof(GridLength), typeof(Form), new FrameworkPropertyMetadata(new GridLength(50d, GridUnitType.Pixel), FrameworkPropertyMetadataOptions.Inherits));

        public static GridLength GetHeaderWidth(DependencyObject element) => (GridLength)element.GetValue(HeaderWidthProperty);
        public static void SetHeaderWidth(DependencyObject element, GridLength value) => element.SetValue(HeaderWidthProperty, value);
        #endregion

        #region AttachedProperty : BodyWidthProperty
        public static readonly DependencyProperty BodyWidthProperty
            = DependencyProperty.RegisterAttached("BodyWidth", typeof(GridLength), typeof(Form), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), FrameworkPropertyMetadataOptions.Inherits));

        public static GridLength GetBodyWidth(DependencyObject element) => (GridLength)element.GetValue(BodyWidthProperty);
        public static void SetBodyWidth(DependencyObject element, GridLength value) => element.SetValue(BodyWidthProperty, value);
        #endregion

        #region AttachedProperty : OrientationProperty
        public static readonly DependencyProperty OrientationProperty
            = DependencyProperty.RegisterAttached("Orientation", typeof(Orientation), typeof(Form), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.Inherits));

        public static Orientation GetOrientation(DependencyObject element) => (Orientation)element.GetValue(OrientationProperty);
        public static void SetOrientation(DependencyObject element, Orientation value) => element.SetValue(OrientationProperty, value);
        #endregion

        #region AttachedProperty: ItemMarginProperty
        public static readonly DependencyProperty ItemMarginProperty
            = DependencyProperty.RegisterAttached("ItemMargin", typeof(Thickness), typeof(Form), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.Inherits));
        public static Thickness GetItemMargin(DependencyObject element) => (Thickness)element.GetValue(ItemMarginProperty);
        public static void SetItemMargin(DependencyObject element, Thickness value) => element.SetValue(ItemMarginProperty, value);
        #endregion     

        #region AttachedProperty : HeaderMarginProperty
        public static readonly DependencyProperty HeaderMarginProperty
            = DependencyProperty.RegisterAttached("HeaderMargin", typeof(Thickness), typeof(Form), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.Inherits));

        public static Thickness GetHeaderMargin(DependencyObject element) => (Thickness)element.GetValue(HeaderMarginProperty);
        public static void SetHeaderMargin(DependencyObject element, Thickness value) => element.SetValue(HeaderMarginProperty, value);
        #endregion

        #region AttachedProperty : BodyMarginProperty
        public static readonly DependencyProperty BodyMarginProperty
            = DependencyProperty.RegisterAttached("BodyMargin", typeof(Thickness), typeof(Form), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.Inherits));

        public static Thickness GetBodyMargin(DependencyObject element) => (Thickness)element.GetValue(BodyMarginProperty);
        public static void SetBodyMargin(DependencyObject element, Thickness value) => element.SetValue(BodyMarginProperty, value);
        #endregion

        #region AttachedProperty: AllowDropProperty
        public new static readonly DependencyProperty AllowDropProperty
            = DependencyProperty.RegisterAttached("AllowDrop", typeof(bool), typeof(Form), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, OnAllowDropChanged));

        private static void OnAllowDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                RoutedEventArgs arg = new RoutedEventArgs(Form.AllowDropEvent, element);
                element.RaiseEvent(arg);
            }
        }

        public static bool GetAllowDrop(DependencyObject element) => (bool)element.GetValue(AllowDropProperty);
        public static void SetAllowDrop(DependencyObject element, bool value) => element.SetValue(AllowDropProperty, value);
        #endregion

        // 声明并定义路由事件
        public static readonly RoutedEvent AllowDropEvent = EventManager.RegisterRoutedEvent
            ("AllowDrop", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(Form));

        internal IList GetActualList()
        {
            IList list;
            if (ItemsSource != null)
            {
                list = ItemsSource as IList;
            }
            else
            {
                list = Items;
            }

            return list;
        }
    }
}
