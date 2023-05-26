using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AIStudio.Wpf.Controls
{
    public static class TipAttach
    {
        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.RegisterAttached(
            "Visibility", typeof(Visibility), typeof(TipAttach), new PropertyMetadata(Visibility.Collapsed));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Popup))]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        public static void SetVisibility(DependencyObject element, Visibility value) => element.SetValue(VisibilityProperty, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Popup))]
        [AttachedPropertyBrowsableForType(typeof(Slider))] public static Visibility GetVisibility(DependencyObject element) => (Visibility)element.GetValue(VisibilityProperty);

        public static readonly DependencyProperty PlacementProperty = DependencyProperty.RegisterAttached(
            "Placement", typeof(PlacementType), typeof(TipAttach), new PropertyMetadata(default(PlacementType)));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Popup))]
        [AttachedPropertyBrowsableForType(typeof(Slider))] public static void SetPlacement(DependencyObject element, PlacementType value) => element.SetValue(PlacementProperty, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Popup))]
        [AttachedPropertyBrowsableForType(typeof(Slider))] public static PlacementType GetPlacement(DependencyObject element) => (PlacementType)element.GetValue(PlacementProperty);

        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.RegisterAttached(
            "StringFormat", typeof(string), typeof(TipAttach), new PropertyMetadata("#0.0"));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Popup))]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        public static void SetStringFormat(DependencyObject element, string value)
            => element.SetValue(StringFormatProperty, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Popup))]
        [AttachedPropertyBrowsableForType(typeof(Slider))]
        public static string GetStringFormat(DependencyObject element)
            => (string)element.GetValue(StringFormatProperty);

        public static CustomPopupPlacementCallback CustomPopupPlacementCallback => CustomPopupPlacementCallbackImpl;

        public static CustomPopupPlacement[] CustomPopupPlacementCallbackImpl(Size popupSize, Size targetSize, Point offset)
        {
            return new[]
            {
                new CustomPopupPlacement(new Point(targetSize.Width/2 - popupSize.Width/2, targetSize.Height + 14), PopupPrimaryAxis.Horizontal)
            };
        }
    }

    public enum PlacementType
    {
        /// <summary>
        /// 左上
        /// </summary>
        LeftTop,

        /// <summary>
        /// 左
        /// </summary>
        Left,

        /// <summary>
        /// 左下
        /// </summary>
        LeftBottom,

        /// <summary>
        /// 上左
        /// </summary>
        TopLeft,

        /// <summary>
        /// 上
        /// </summary>
        Top,

        /// <summary>
        /// 上右
        /// </summary>
        TopRight,

        /// <summary>
        /// 右上
        /// </summary>
        RightTop,

        /// <summary>
        /// 右
        /// </summary>
        Right,

        /// <summary>
        /// 右下
        /// </summary>
        RightBottom,

        /// <summary>
        /// 下左
        /// </summary>
        BottomLeft,

        /// <summary>
        /// 下
        /// </summary>
        Bottom,

        /// <summary>
        /// 下右
        /// </summary>
        BottomRight,
    }
}
