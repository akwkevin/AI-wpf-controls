namespace AIStudio.Wpf.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The HamburgerTreeMenu is based on a SplitView control. By default it contains a PART_HamburgerButton and a ListView to display menu items.
    /// </summary>
    public partial class HamburgerTreeMenu
    {
        /// <summary>
        /// Identifies the <see cref="OptionsItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OptionsItemsSourceProperty = DependencyProperty.Register(nameof(OptionsItemsSource), typeof(object), typeof(HamburgerTreeMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="OptionsItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OptionsItemTemplateProperty = DependencyProperty.Register(nameof(OptionsItemTemplate), typeof(DataTemplate), typeof(HamburgerTreeMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="OptionsItemTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OptionsItemTemplateSelectorProperty = DependencyProperty.Register(nameof(OptionsItemTemplateSelector), typeof(DataTemplateSelector), typeof(HamburgerTreeMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="OptionsVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OptionsVisibilityProperty = DependencyProperty.Register(nameof(OptionsVisibility), typeof(Visibility), typeof(HamburgerTreeMenu), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <see cref="SelectedOptionsItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedOptionsItemProperty = DependencyProperty.Register(nameof(SelectedOptionsItem), typeof(object), typeof(HamburgerTreeMenu), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies the <see cref="SelectedOptionsIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedOptionsIndexProperty = DependencyProperty.Register(nameof(SelectedOptionsIndex), typeof(int), typeof(HamburgerTreeMenu), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

        /// <summary>
        ///     Gets or sets an object source used to generate the content of the options.
        /// </summary>
        public object OptionsItemsSource
        {
            get
            {
                return GetValue(OptionsItemsSourceProperty);
            }
            set
            {
                SetValue(OptionsItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used to display each item in the options.
        /// </summary>
        public DataTemplate OptionsItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(OptionsItemTemplateProperty);
            }
            set
            {
                SetValue(OptionsItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplateSelector used to display each item in the options.
        /// </summary>
        public DataTemplateSelector OptionsItemTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(OptionsItemTemplateSelectorProperty);
            }
            set
            {
                SetValue(OptionsItemTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection used to generate the content of the option list.
        /// </summary>
        /// <exception cref="Exception">
        /// Exception thrown if PART_OptionsListView is not yet defined.
        /// </exception>
        public ItemCollection OptionsItems
        {
            get
            {
                if (_optionsListView == null)
                {
                    throw new Exception("PART_OptionsListView is not defined yet. Please use OptionsItemsSource instead.");
                }

                return _optionsListView?.Items;
            }
        }

        /// <summary>
        /// Gets or sets options' visibility.
        /// </summary>
        public Visibility OptionsVisibility
        {
            get
            {
                return (Visibility)GetValue(OptionsVisibilityProperty);
            }
            set
            {
                SetValue(OptionsVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected options menu item.
        /// </summary>
        public object SelectedOptionsItem
        {
            get
            {
                return GetValue(SelectedOptionsItemProperty);
            }
            set
            {
                SetValue(SelectedOptionsItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected options menu index.
        /// </summary>
        public int SelectedOptionsIndex
        {
            get
            {
                return (int)GetValue(SelectedOptionsIndexProperty);
            }
            set
            {
                SetValue(SelectedOptionsIndexProperty, value);
            }
        }
    }
}
