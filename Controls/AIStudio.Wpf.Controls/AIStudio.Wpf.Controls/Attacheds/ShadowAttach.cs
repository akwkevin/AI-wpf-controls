using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace AIStudio.Wpf.Controls
{
    public static class ShadowAttach
    {
        #region 阴影
        [Flags]
        public enum ShadowEdges
        {
            None = 0,
            Left = 1,
            Top = 2,
            Right = 4,
            Bottom = 8,
            All = Left | Top | Right | Bottom
        }

        public static readonly DependencyProperty DropShadowEffectProperty = DependencyProperty.RegisterAttached(
          "DropShadowEffect", typeof(DropShadowEffect), typeof(ShadowAttach), new FrameworkPropertyMetadata(default(DropShadowEffect), FrameworkPropertyMetadataOptions.AffectsRender));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetDropShadowEffect(DependencyObject element, DropShadowEffect value)
        {
            element.SetValue(DropShadowEffectProperty, value);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static DropShadowEffect GetDropShadowEffect(DependencyObject element)
        {
            return (DropShadowEffect)element.GetValue(DropShadowEffectProperty);
        }

        private static readonly DependencyPropertyKey LocalInfoPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "LocalInfo", typeof(ShadowLocalInfo), typeof(ShadowAttach), new PropertyMetadata(default(ShadowLocalInfo)));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        private static void SetLocalInfo(DependencyObject element, ShadowLocalInfo value)
            => element.SetValue(LocalInfoPropertyKey, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        private static ShadowLocalInfo GetLocalInfo(DependencyObject element)
            => (ShadowLocalInfo)element.GetValue(LocalInfoPropertyKey.DependencyProperty);

        public static readonly DependencyProperty DarkenProperty = DependencyProperty.RegisterAttached(
            "Darken", typeof(bool), typeof(ShadowAttach), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.AffectsRender, DarkenPropertyChangedCallback));

        private static void DarkenPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var uiElement = dependencyObject as UIElement;
            var dropShadowEffect = uiElement?.Effect as DropShadowEffect;

            if (dropShadowEffect == null) return;

            if ((bool)dependencyPropertyChangedEventArgs.NewValue)
            {
                SetLocalInfo(dependencyObject, new ShadowLocalInfo(dropShadowEffect.Opacity));

                var doubleAnimation = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(350)))
                {
                    FillBehavior = FillBehavior.HoldEnd
                };
                dropShadowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, doubleAnimation);
            }
            else
            {
                var shadowLocalInfo = GetLocalInfo(dependencyObject);
                if (shadowLocalInfo == null) return;

                var doubleAnimation = new DoubleAnimation(shadowLocalInfo.StandardOpacity, new Duration(TimeSpan.FromMilliseconds(350)))
                {
                    FillBehavior = FillBehavior.HoldEnd
                };
                dropShadowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, doubleAnimation);
            }
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetDarken(DependencyObject element, bool value)
        {
            element.SetValue(DarkenProperty, value);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static bool GetDarken(DependencyObject element)
        {
            return (bool)element.GetValue(DarkenProperty);
        }

        public static readonly DependencyProperty CacheModeProperty = DependencyProperty.RegisterAttached(
            "CacheMode", typeof(CacheMode), typeof(ShadowAttach), new FrameworkPropertyMetadata(new BitmapCache { EnableClearType = true, SnapsToDevicePixels = true }, FrameworkPropertyMetadataOptions.Inherits));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetCacheMode(DependencyObject element, CacheMode value)
        {
            element.SetValue(CacheModeProperty, value);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static CacheMode GetCacheMode(DependencyObject element)
        {
            return (CacheMode)element.GetValue(CacheModeProperty);
        }

        public static readonly DependencyProperty ShadowEdgesProperty = DependencyProperty.RegisterAttached(
            "ShadowEdges", typeof(ShadowEdges), typeof(ShadowAttach), new PropertyMetadata(ShadowEdges.All));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetShadowEdges(DependencyObject element, ShadowEdges value)
        {
            element.SetValue(ShadowEdgesProperty, value);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static ShadowEdges GetShadowEdges(DependencyObject element)
        {
            return (ShadowEdges)element.GetValue(ShadowEdgesProperty);
        }
        #endregion
    }

    internal class ShadowLocalInfo
    {
        public ShadowLocalInfo(double standardOpacity)
        {
            StandardOpacity = standardOpacity;
        }

        public double StandardOpacity
        {
            get;
        }
    }
}
