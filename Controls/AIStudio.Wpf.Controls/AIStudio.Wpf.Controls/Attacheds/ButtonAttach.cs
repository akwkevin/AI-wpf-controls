﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public static class ButtonAttach
    {
        private const double DefaultMaximum = 100.0;

        #region AttachedProperty : MinimumProperty
        public static readonly DependencyProperty MinimumProperty
            = DependencyProperty.RegisterAttached("Minimum", typeof(double), typeof(ButtonAttach), new FrameworkPropertyMetadata(default(double)));

        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static double GetMinimum(ButtonBase element) => (double)element.GetValue(MinimumProperty);
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static void SetMinimum(ButtonBase element, double value) => element.SetValue(MinimumProperty, value);
        #endregion

        #region AttachedProperty : MaximumProperty
        public static readonly DependencyProperty MaximumProperty
            = DependencyProperty.RegisterAttached("Maximum", typeof(double), typeof(ButtonAttach), new FrameworkPropertyMetadata(DefaultMaximum));

        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static double GetMaximum(ButtonBase element) => (double)element.GetValue(MaximumProperty);
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static void SetMaximum(ButtonBase element, double value) => element.SetValue(MaximumProperty, value);
        #endregion

        #region AttachedProperty : ValueProperty
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.RegisterAttached("Value", typeof(double), typeof(ButtonAttach), new FrameworkPropertyMetadata(default(double)));

        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static double GetValue(ButtonBase element) => (double)element.GetValue(ValueProperty);
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static void SetValue(ButtonBase element, double value) => element.SetValue(ValueProperty, value);
        #endregion

        #region AttachedProperty : IsIndeterminate
        public static readonly DependencyProperty IsIndeterminateProperty
            = DependencyProperty.RegisterAttached("IsIndeterminate", typeof(bool), typeof(ButtonAttach), new FrameworkPropertyMetadata(default(bool)));

        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static bool GetIsIndeterminate(ButtonBase element) => (bool)element.GetValue(IsIndeterminateProperty);
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static void SetIsIndeterminate(ButtonBase element, bool isIndeterminate) => element.SetValue(IsIndeterminateProperty, isIndeterminate);
        #endregion

        #region AttachedProperty : IndicatorForegroundProperty
        public static readonly DependencyProperty IndicatorForegroundProperty
            = DependencyProperty.RegisterAttached("IndicatorForeground", typeof(Brush), typeof(ButtonAttach), new FrameworkPropertyMetadata(default(Brush)));

        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static Brush GetIndicatorForeground(ButtonBase element) => (Brush)element.GetValue(IndicatorForegroundProperty);
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static void SetIndicatorForeground(ButtonBase element, Brush indicatorForeground) => element.SetValue(IndicatorForegroundProperty, indicatorForeground);
        #endregion

        #region AttachedProperty : IndicatorBackgroundProperty
        public static readonly DependencyProperty IndicatorBackgroundProperty
            = DependencyProperty.RegisterAttached("IndicatorBackground", typeof(Brush), typeof(ButtonAttach), new FrameworkPropertyMetadata(default(Brush)));

        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static Brush GetIndicatorBackground(ButtonBase element) => (Brush)element.GetValue(IndicatorBackgroundProperty);
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static void SetIndicatorBackground(ButtonBase element, Brush indicatorBackground) => element.SetValue(IndicatorBackgroundProperty, indicatorBackground);
        #endregion

        #region AttachedProperty : IsIndicatorVisibleProperty
        public static readonly DependencyProperty IsIndicatorVisibleProperty
            = DependencyProperty.RegisterAttached("IsIndicatorVisible", typeof(bool), typeof(ButtonAttach), new FrameworkPropertyMetadata(default(bool)));

        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static bool GetIsIndicatorVisible(ButtonBase element) => (bool)element.GetValue(IsIndicatorVisibleProperty);
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static void SetIsIndicatorVisible(ButtonBase element, bool isIndicatorVisible) => element.SetValue(IsIndicatorVisibleProperty, isIndicatorVisible);
        #endregion

        #region AttachedProperty : OpacityProperty
        public static readonly DependencyProperty OpacityProperty
            = DependencyProperty.RegisterAttached("Opacity", typeof(double), typeof(ButtonAttach), new FrameworkPropertyMetadata(default(double)));

        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static double GetOpacity(ButtonBase element) => (double)element.GetValue(OpacityProperty);
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        [Category(AppName.AIStudio)]
        public static void SetOpacity(ButtonBase element, double opacity) => element.SetValue(OpacityProperty, opacity);
        #endregion


    }
}
