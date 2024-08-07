﻿namespace AIStudio.Wpf.Controls
{
    using System.Windows;

    /// <summary>
    /// The HamburgerTreeMenu is based on a SplitView control. By default it contains a PART_HamburgerButton and a ListView to display menu items.
    /// </summary>
    public partial class HamburgerTreeMenu
    {
        /// <summary>
        /// Identifies the <see cref="HamburgerWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HamburgerWidthProperty = DependencyProperty.Register(nameof(HamburgerWidth), typeof(double), typeof(HamburgerTreeMenu), new PropertyMetadata(28.0));

        /// <summary>
        /// Identifies the <see cref="HamburgerHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HamburgerHeightProperty = DependencyProperty.Register(nameof(HamburgerHeight), typeof(double), typeof(HamburgerTreeMenu), new PropertyMetadata(28.0));

        /// <summary>
        /// Identifies the <see cref="HamburgerMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HamburgerMarginProperty = DependencyProperty.Register(nameof(HamburgerMargin), typeof(Thickness), typeof(HamburgerTreeMenu), new PropertyMetadata(new Thickness(12.0)));

        /// <summary>
        /// Identifies the <see cref="HamburgerMenuTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HamburgerMenuTemplateProperty = DependencyProperty.Register(nameof(HamburgerMenuTemplate), typeof(DataTemplate), typeof(HamburgerTreeMenu), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the hamburger icon.
        /// </summary>
        public DataTemplate HamburgerMenuTemplate
        {
            get
            {
                return (DataTemplate)GetValue(HamburgerMenuTemplateProperty);
            }
            set
            {
                SetValue(HamburgerMenuTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets main button's _width
        /// </summary>
        public double HamburgerWidth
        {
            get
            {
                return (double)GetValue(HamburgerWidthProperty);
            }
            set
            {
                SetValue(HamburgerWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets main button's height
        /// </summary>
        public double HamburgerHeight
        {
            get
            {
                return (double)GetValue(HamburgerHeightProperty);
            }
            set
            {
                SetValue(HamburgerHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets main button's margin
        /// </summary>
        public Thickness HamburgerMargin
        {
            get
            {
                return (Thickness)GetValue(HamburgerMarginProperty);
            }
            set
            {
                SetValue(HamburgerMarginProperty, value);
            }
        }
    }
}
