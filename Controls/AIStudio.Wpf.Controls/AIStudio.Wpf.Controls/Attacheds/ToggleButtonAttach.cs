﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    public static class ToggleButtonAttach
    {
        #region CheckedElement
        /// <summary>
        ///     选中时展示的元素
        /// </summary>
        public static readonly DependencyProperty CheckedElementProperty = DependencyProperty.RegisterAttached(
            "CheckedElement", typeof(object), typeof(ToggleButtonAttach), new PropertyMetadata(default(object)));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        public static void SetCheckedElement(DependencyObject element, object value) => element.SetValue(CheckedElementProperty, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        public static object GetCheckedElement(DependencyObject element) => element.GetValue(CheckedElementProperty);

        /// <summary>
        /// Allows an on (IsChecked) template to be provided on supporting <see cref="ToggleButton"/> styles.
        /// </summary>
        public static readonly DependencyProperty CheckedElementTemplateProperty = DependencyProperty.RegisterAttached(
            "CheckedElementTemplate", typeof(DataTemplate), typeof(ToggleButtonAttach), new PropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// Allows an on (IsChecked) template to be provided on supporting <see cref="ToggleButton"/> styles.
        /// </summary>
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        public static void SetCheckedElementTemplate(DependencyObject element, DataTemplate value)
            => element.SetValue(CheckedElementTemplateProperty, value);

        /// <summary>
        /// Allows an on (IsChecked) template to be provided on supporting <see cref="ToggleButton"/> styles.
        /// </summary>
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        public static DataTemplate GetCheckedElementTemplate(DependencyObject element)
            => (DataTemplate)element.GetValue(CheckedElementTemplateProperty);

        /// <summary>
        ///     是否隐藏元素
        /// </summary>
        public static readonly DependencyProperty HideUncheckedElementProperty = DependencyProperty.RegisterAttached(
            "HideUncheckedElement", typeof(bool), typeof(ToggleButtonAttach), new PropertyMetadata(false));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        public static void SetHideUncheckedElement(DependencyObject element, bool value) => element.SetValue(HideUncheckedElementProperty, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        public static bool GetHideUncheckedElement(DependencyObject element) => (bool)element.GetValue(HideUncheckedElementProperty);
        #endregion
    }
}
