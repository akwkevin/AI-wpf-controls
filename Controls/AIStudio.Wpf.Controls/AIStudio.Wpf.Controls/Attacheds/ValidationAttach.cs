using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public static class ValidationAttach
    {
        #region ShowOnFocusProperty

        /// <summary>
        /// The hint property
        /// </summary>
        public static readonly DependencyProperty OnlyShowOnFocusProperty = DependencyProperty.RegisterAttached(
            "OnlyShowOnFocus",
            typeof(bool),
            typeof(ValidationAttach),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static bool GetOnlyShowOnFocus(DependencyObject element)
        {
            return (bool)element.GetValue(OnlyShowOnFocusProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetOnlyShowOnFocus(DependencyObject element, bool value)
        {
            element.SetValue(OnlyShowOnFocusProperty, value);
        }

        #endregion

        #region UsePopupProperty

        /// <summary>
        /// The hint property
        /// </summary>
        public static readonly DependencyProperty UsePopupProperty = DependencyProperty.RegisterAttached(
            "UsePopup",
            typeof(bool),
            typeof(ValidationAttach),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static bool GetUsePopup(DependencyObject element)
        {
            return (bool)element.GetValue(UsePopupProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetUsePopup(DependencyObject element, bool value)
        {
            element.SetValue(UsePopupProperty, value);
        }

        #endregion

        /// <summary>
        /// The hint property
        /// </summary>
        public static readonly DependencyProperty PopupPlacementProperty = DependencyProperty.RegisterAttached(
            "PopupPlacement",
            typeof(PlacementMode),
            typeof(ValidationAttach),
            new FrameworkPropertyMetadata(PlacementMode.Bottom, FrameworkPropertyMetadataOptions.Inherits));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static PlacementMode GetPopupPlacement(DependencyObject element)
        {
            return (PlacementMode)element.GetValue(PopupPlacementProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetPopupPlacement(DependencyObject element, PlacementMode value)
        {
            element.SetValue(PopupPlacementProperty, value);
        }

        /// <summary>
        /// Framework use only.
        /// </summary>
        public static readonly DependencyProperty SuppressProperty = DependencyProperty.RegisterAttached(
            "Suppress", typeof(bool), typeof(ValidationAttach), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Framework use only.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetSuppress(DependencyObject element, bool value)
        {
            element.SetValue(SuppressProperty, value);
        }

        /// <summary>
        /// Framework use only.
        /// </summary>
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static bool GetSuppress(DependencyObject element)
        {
            return (bool)element.GetValue(SuppressProperty);
        }

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached(
            "Background", typeof(Brush), typeof(ValidationAttach), new PropertyMetadata(default(Brush)));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetBackground(DependencyObject element, Brush value)
        {
            element.SetValue(BackgroundProperty, value);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static Brush GetBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(BackgroundProperty);
        }



        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.RegisterAttached("FontSize", typeof(double), typeof(ValidationAttach), new PropertyMetadata(10.0));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetFontSize(DependencyObject element, double value)
        {
            element.SetValue(FontSizeProperty, value);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static double GetFontSize(DependencyObject element)
        {
            return (double)element.GetValue(FontSizeProperty);
        }

        public static readonly DependencyProperty HasErrorProperty = DependencyProperty.RegisterAttached(
            "HasError",
            typeof(bool),
            typeof(ValidationAttach),
            new PropertyMetadata(default(bool)));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetHasError(DependencyObject element, bool value)
        {
            element.SetValue(HasErrorProperty, value);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static bool GetHasError(DependencyObject element)
        {
            return (bool)element.GetValue(HasErrorProperty);
        }

        public static readonly DependencyProperty HorizontalAlignmentProperty = DependencyProperty.RegisterAttached(
            "HorizontalAlignment", typeof(HorizontalAlignment), typeof(ValidationAttach), new PropertyMetadata(HorizontalAlignment.Left));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetHorizontalAlignment(DependencyObject element, HorizontalAlignment value) => element.SetValue(HorizontalAlignmentProperty, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))] 
        public static HorizontalAlignment GetHorizontalAlignment(DependencyObject element) => (HorizontalAlignment)element.GetValue(HorizontalAlignmentProperty);
    }
}
