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
    }
}
