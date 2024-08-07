﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public static class IconAttach
    {
        #region Icon
        public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached(
          "Icon", typeof(string), typeof(IconAttach), new FrameworkPropertyMetadata(default(string)));


        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetIcon(DependencyObject element, string value)
            => element.SetValue(IconProperty, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static string GetIcon(DependencyObject element)
            => (string)element.GetValue(IconProperty);

        public static readonly DependencyProperty KindProperty = DependencyProperty.RegisterAttached(
            "Kind", typeof(string), typeof(IconAttach), new FrameworkPropertyMetadata(default(string)));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetKind(DependencyObject element, string value)
            => element.SetValue(KindProperty, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static string GetKind(DependencyObject element)
            => (string)element.GetValue(KindProperty);

        public static readonly DependencyProperty GeometryProperty = DependencyProperty.RegisterAttached(
           "Geometry", typeof(Geometry), typeof(IconAttach), new FrameworkPropertyMetadata(default(Geometry)));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetGeometry(DependencyObject element, Geometry value)
            => element.SetValue(GeometryProperty, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static Geometry GetGeometry(DependencyObject element)
            => (Geometry)element.GetValue(GeometryProperty);

        public static readonly DependencyProperty GeometrySelectedProperty = DependencyProperty.RegisterAttached(
            "GeometrySelected", typeof(Geometry), typeof(IconAttach), new FrameworkPropertyMetadata(default(Geometry)));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetGeometrySelected(DependencyObject element, Geometry value)
        {
            element.SetValue(GeometrySelectedProperty, value);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static Geometry GetGeometrySelected(DependencyObject element)
        {
            return (Geometry)element.GetValue(GeometrySelectedProperty);
        }

        public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached(
            "Width", typeof(double), typeof(IconAttach), new FrameworkPropertyMetadata(10d));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetWidth(DependencyObject element, double value)
            => element.SetValue(WidthProperty, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static double GetWidth(DependencyObject element)
            => (double)element.GetValue(WidthProperty);

        public static readonly DependencyProperty HeightProperty = DependencyProperty.RegisterAttached(
            "Height", typeof(double), typeof(IconAttach), new FrameworkPropertyMetadata(10d));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetHeight(DependencyObject element, double value)
            => element.SetValue(HeightProperty, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static double GetHeight(DependencyObject element)
            => (double)element.GetValue(HeightProperty);

        public static readonly DependencyProperty DockProperty = DependencyProperty.RegisterAttached(
           "Dock", typeof(Dock), typeof(IconAttach), new FrameworkPropertyMetadata(Dock.Left));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetDock(DependencyObject element, Dock value)
            => element.SetValue(DockProperty, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static Dock GetDock(DependencyObject element)
            => (Dock)element.GetValue(DockProperty);


        public static readonly DependencyProperty AllowsAnimationProperty = DependencyProperty.RegisterAttached(
           "AllowsAnimation", typeof(bool), typeof(IconAttach), new FrameworkPropertyMetadata(false));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static void SetAllowsAnimation(DependencyObject element, bool value)
            => element.SetValue(AllowsAnimationProperty, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static bool GetAllowsAnimation(DependencyObject element)
            => (bool)element.GetValue(AllowsAnimationProperty);
        #endregion
    }
}
