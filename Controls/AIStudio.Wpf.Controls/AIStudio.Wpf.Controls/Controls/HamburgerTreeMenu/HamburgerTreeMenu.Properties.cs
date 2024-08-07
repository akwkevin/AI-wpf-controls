﻿namespace AIStudio.Wpf.Controls
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// The HamburgerTreeMenu is based on a SplitView control. By default it contains a PART_HamburgerButton and a ListView to display menu items.
    /// </summary>
    public partial class HamburgerTreeMenu
    {
        /// <summary>
        /// Identifies the <see cref="OpenPaneLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpenPaneLengthProperty = DependencyProperty.Register(nameof(OpenPaneLength), typeof(double), typeof(HamburgerTreeMenu), new PropertyMetadata(240.0));

        /// <summary>
        /// Identifies the <see cref="PanePlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanePlacementProperty = DependencyProperty.Register(nameof(PanePlacement), typeof(SplitViewPanePlacement), typeof(HamburgerTreeMenu), new PropertyMetadata(SplitViewPanePlacement.Left));

        /// <summary>
        /// Identifies the <see cref="DisplayMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(nameof(DisplayMode), typeof(SplitViewDisplayMode), typeof(HamburgerTreeMenu), new PropertyMetadata(SplitViewDisplayMode.CompactInline));

        /// <summary>
        /// Identifies the <see cref="CompactPaneLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CompactPaneLengthProperty = DependencyProperty.Register(nameof(CompactPaneLength), typeof(double), typeof(HamburgerTreeMenu), new PropertyMetadata(48.0));

        /// <summary>
        /// Identifies the <see cref="PaneBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaneBackgroundProperty = DependencyProperty.Register(nameof(PaneBackground), typeof(Brush), typeof(HamburgerTreeMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsPaneOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPaneOpenProperty = DependencyProperty.Register(nameof(IsPaneOpen), typeof(bool), typeof(HamburgerTreeMenu), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(HamburgerTreeMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(HamburgerTreeMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MenuItemTemplateProperty = DependencyProperty.Register(nameof(MenuItemTemplate), typeof(DataTemplate), typeof(HamburgerTreeMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(HamburgerTreeMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectedItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(HamburgerTreeMenu), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies the <see cref="SelectedIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(HamburgerTreeMenu), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

        /// <summary>
        /// Identifies the <see cref="ContentTransition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTransitionProperty = DependencyProperty.Register(nameof(ContentTransition), typeof(TransitionType), typeof(HamburgerTreeMenu), new FrameworkPropertyMetadata(TransitionType.Normal));

        /// <summary>
        /// Gets or sets the _width of the pane when it's fully expanded.
        /// </summary>
        public double OpenPaneLength
        {
            get
            {
                return (double)GetValue(OpenPaneLengthProperty);
            }
            set
            {
                SetValue(OpenPaneLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies whether the pane is shown on the right or left side of the control.
        /// </summary>
        public SplitViewPanePlacement PanePlacement
        {
            get
            {
                return (SplitViewPanePlacement)GetValue(PanePlacementProperty);
            }
            set
            {
                SetValue(PanePlacementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets gets of sets a value that specifies how the pane and content areas are shown.
        /// </summary>
        public SplitViewDisplayMode DisplayMode
        {
            get
            {
                return (SplitViewDisplayMode)GetValue(DisplayModeProperty);
            }
            set
            {
                SetValue(DisplayModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the _width of the pane in its compact display mode.
        /// </summary>
        public double CompactPaneLength
        {
            get
            {
                return (double)GetValue(CompactPaneLengthProperty);
            }
            set
            {
                SetValue(CompactPaneLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Brush to apply to the background of the Pane area of the control.
        /// </summary>
        public Brush PaneBackground
        {
            get
            {
                return (Brush)GetValue(PaneBackgroundProperty);
            }
            set
            {
                SetValue(PaneBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value that specifies whether the pane is expanded to its full _width.
        /// </summary>
        public bool IsPaneOpen
        {
            get
            {
                return (bool)GetValue(IsPaneOpenProperty);
            }
            set
            {
                SetValue(IsPaneOpenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an object source used to generate the content of the menu.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used to display each item.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemTemplateProperty);
            }
            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplateSelector used to display each item.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty);
            }
            set
            {
                SetValue(ItemTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used to display each item.
        /// </summary>
        public DataTemplate MenuItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(MenuItemTemplateProperty);
            }
            set
            {
                SetValue(MenuItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection used to generate the content of the items list.
        /// </summary>
        /// <exception cref="Exception">
        /// Exception thrown if PART_ButtonsListView is not yet defined.
        /// </exception>
        public ItemCollection Items
        {
            get
            {
                if (_buttonsListView == null)
                {
                    throw new Exception("PART_ButtonsListView is not defined yet. Please use ItemsSource instead.");
                }

                return _buttonsListView.Items;
            }
        }

        /// <summary>
        /// Gets or sets the selected menu item.
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected menu index.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }
            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }

        public TransitionType ContentTransition
        {
            get
            {
                return (TransitionType)this.GetValue(ContentTransitionProperty);
            }
            set
            {
                this.SetValue(ContentTransitionProperty, value);
            }
        }
    }
}
