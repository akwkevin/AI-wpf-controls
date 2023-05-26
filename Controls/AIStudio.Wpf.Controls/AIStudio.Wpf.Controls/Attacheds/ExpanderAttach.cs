using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public static class ExpanderAttach
    {
        private static readonly Thickness DefaultHorizontalHeaderPadding = new Thickness(5);
        private static readonly Thickness DefaultVerticalHeaderPadding = new Thickness(5);

        #region AttachedProperty : HorizontalHeaderPaddingProperty
        public static readonly DependencyProperty HorizontalHeaderPaddingProperty
            = DependencyProperty.RegisterAttached("HorizontalHeaderPadding", typeof(Thickness), typeof(ExpanderAttach),
                new FrameworkPropertyMetadata(DefaultHorizontalHeaderPadding, FrameworkPropertyMetadataOptions.Inherits));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Thickness GetHorizontalHeaderPadding(Expander element)
            => (Thickness)element.GetValue(HorizontalHeaderPaddingProperty);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static void SetHorizontalHeaderPadding(Expander element, Thickness value)
            => element.SetValue(HorizontalHeaderPaddingProperty, value);
        #endregion

        #region AttachedProperty : VerticalHeaderPaddingProperty
        public static readonly DependencyProperty VerticalHeaderPaddingProperty
            = DependencyProperty.RegisterAttached("VerticalHeaderPadding", typeof(Thickness), typeof(ExpanderAttach),
                new FrameworkPropertyMetadata(DefaultVerticalHeaderPadding, FrameworkPropertyMetadataOptions.Inherits));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Thickness GetVerticalHeaderPadding(Expander element)
            => (Thickness)element.GetValue(VerticalHeaderPaddingProperty);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static void SetVerticalHeaderPadding(Expander element, Thickness value)
            => element.SetValue(VerticalHeaderPaddingProperty, value);
        #endregion

        #region AttachedProperty : HeaderIsEnabledProperty
        public static readonly DependencyProperty HeaderIsEnabledProperty
            = DependencyProperty.RegisterAttached("HeaderIsEnabled", typeof(bool), typeof(ExpanderAttach),
                new FrameworkPropertyMetadata(true));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static bool GetHeaderIsEnabled(Expander element)
            => (bool)element.GetValue(HeaderIsEnabledProperty);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static void SetHeaderIsEnabled(Expander element, bool value)
            => element.SetValue(HeaderIsEnabledProperty, value);
        #endregion

        #region AttachedProperty : HeaderBackgroundProperty
        public static readonly DependencyProperty HeaderBackgroundProperty
            = DependencyProperty.RegisterAttached("HeaderBackground", typeof(Brush), typeof(ExpanderAttach));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Brush GetHeaderBackground(Expander element)
            => (Brush)element.GetValue(HeaderBackgroundProperty);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static void SetHeaderBackground(Expander element, Brush value)
            => element.SetValue(HeaderBackgroundProperty, value);
        #endregion

        #region AttachedProperty : HeaderForegroundProperty
        public static readonly DependencyProperty HeaderForegroundProperty
            = DependencyProperty.RegisterAttached("HeaderForeground", typeof(Brush), typeof(ExpanderAttach));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Brush GetHeaderForeground(Expander element)
            => (Brush)element.GetValue(HeaderForegroundProperty);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static void SetHeaderForeground(Expander element, Brush value)
            => element.SetValue(HeaderForegroundProperty, value);
        #endregion

        #region ExpanderAttach
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static bool GetAutoExpander(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoExpanderProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static void SetAutoExpander(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoExpanderProperty, value);
        }

        public static readonly DependencyProperty AutoExpanderProperty =
            DependencyProperty.RegisterAttached("AutoExpander", typeof(bool), typeof(ExpanderAttach), new PropertyMetadata(false, OnAutoExpanderChanged));
        private static void OnAutoExpanderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Panel)
            {
                (sender as Panel).Loaded -= ExpanderGridHelper_Loaded;
                if ((bool)e.NewValue == true)
                {
                    (sender as Panel).Loaded += ExpanderGridHelper_Loaded;
                }
            }
        }

        private static void ExpanderGridHelper_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Panel)
            {
                var expanderlist = new List<Expander>();
                foreach (UIElement element in (sender as Panel).Children)
                {
                    if (element is Expander)
                    {
                        Expander expander = element as Expander;
                        SetHeaderIsEnabled(expander, !expander.IsExpanded);
                        expander.Expanded -= expander_Expanded;
                        expander.Expanded += expander_Expanded;
                        expanderlist.Add(expander);
                    }
                }
                SetAutoExpanderList((sender as Panel), expanderlist);
            }
        }

        private static void expander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander expander1 = sender as Expander;
            Panel panel = expander1.Parent as Panel;
            if (expander1 != null && panel != null)
            {
                var expanderlist = GetAutoExpanderList(panel);
                if (expanderlist != null)
                {
                    foreach (var expander in expanderlist)
                    {
                        expander.IsExpanded = (sender == expander) ? true : false;
                        SetHeaderIsEnabled(expander, !expander.IsExpanded);
                        if (expander1.Parent is Grid)
                        {
                            int row = Grid.GetRow(expander);
                            if (expander.IsExpanded == true)
                            {
                                (panel as Grid).RowDefinitions[row].Height = new GridLength((panel as Grid).RowDefinitions[row].Height.Value, GridUnitType.Star);
                            }
                            else
                            {
                                (panel as Grid).RowDefinitions[row].Height = new GridLength((panel as Grid).RowDefinitions[row].Height.Value, GridUnitType.Auto);
                            }
                        }
                    }
                }
            }
        }

        public static List<Expander> GetAutoExpanderList(DependencyObject obj)
        {
            return (List<Expander>)obj.GetValue(AutoExpanderListProperty);
        }

        public static void SetAutoExpanderList(DependencyObject obj, List<Expander> value)
        {
            obj.SetValue(AutoExpanderListProperty, value);
        }

        private static readonly DependencyProperty AutoExpanderListProperty =
            DependencyProperty.RegisterAttached("AutoExpanderList", typeof(List<Expander>), typeof(ExpanderAttach), new PropertyMetadata(null));
        #endregion

        public static readonly DependencyProperty HeaderUpStyleProperty = DependencyProperty.RegisterAttached("HeaderUpStyle", typeof(Style), typeof(ExpanderAttach), new FrameworkPropertyMetadata((Style)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the toggle button style used for the ExpandDirection Up.
        /// </summary>
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Style GetHeaderUpStyle(UIElement element)
        {
            return (Style)element.GetValue(HeaderUpStyleProperty);
        }

        /// <summary>
        /// Sets the toggle button style used for the ExpandDirection Up.
        /// </summary>
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static void SetHeaderUpStyle(UIElement element, Style value)
        {
            element.SetValue(HeaderUpStyleProperty, value);
        }

        public static readonly DependencyProperty HeaderDownStyleProperty = DependencyProperty.RegisterAttached("HeaderDownStyle", typeof(Style), typeof(ExpanderAttach), new FrameworkPropertyMetadata((Style)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the toggle button style used for the ExpandDirection Down.
        /// </summary>
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Style GetHeaderDownStyle(UIElement element)
        {
            return (Style)element.GetValue(HeaderDownStyleProperty);
        }

        /// <summary>
        /// Sets the toggle button style used for the ExpandDirection Down.
        /// </summary>
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static void SetHeaderDownStyle(UIElement element, Style value)
        {
            element.SetValue(HeaderDownStyleProperty, value);
        }

        public static readonly DependencyProperty HeaderLeftStyleProperty = DependencyProperty.RegisterAttached("HeaderLeftStyle", typeof(Style), typeof(ExpanderAttach), new FrameworkPropertyMetadata((Style)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the toggle button style used for the ExpandDirection Left.
        /// </summary>
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Style GetHeaderLeftStyle(UIElement element)
        {
            return (Style)element.GetValue(HeaderLeftStyleProperty);
        }

        /// <summary>
        /// Sets the toggle button style used for the ExpandDirection Left.
        /// </summary>
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static void SetHeaderLeftStyle(UIElement element, Style value)
        {
            element.SetValue(HeaderLeftStyleProperty, value);
        }

        public static readonly DependencyProperty HeaderRightStyleProperty = DependencyProperty.RegisterAttached("HeaderRightStyle", typeof(Style), typeof(ExpanderAttach), new FrameworkPropertyMetadata((Style)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the toggle button style used for the ExpandDirection Right.
        /// </summary>
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Style GetHeaderRightStyle(UIElement element)
        {
            return (Style)element.GetValue(HeaderRightStyleProperty);
        }

        /// <summary>
        /// Sets the toggle button style used for the ExpandDirection Right.
        /// </summary>
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static void SetHeaderRightStyle(UIElement element, Style value)
        {
            element.SetValue(HeaderRightStyleProperty, value);
        }

        //上下模式的时候使用，箭头和文字是否居右
        public static readonly DependencyProperty IsHeaderRightProperty = DependencyProperty.RegisterAttached("IsHeaderRight", typeof(bool), typeof(ExpanderAttach), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets the toggle button style used for the ExpandDirection Right.
        /// </summary>
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static bool GetIsHeaderRight(UIElement element)
        {
            return (bool)element.GetValue(IsHeaderRightProperty);
        }

        /// <summary>
        /// Sets the toggle button style used for the ExpandDirection Right.
        /// </summary>
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static void SetIsHeaderRight(UIElement element, bool value)
        {
            element.SetValue(IsHeaderRightProperty, value);
        }
    }
}
