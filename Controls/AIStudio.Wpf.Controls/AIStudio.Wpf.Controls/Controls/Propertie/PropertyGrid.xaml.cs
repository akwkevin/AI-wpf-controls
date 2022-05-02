using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.Controls.Event;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_ItemsControl, Type = typeof(ItemsControl))]
    [TemplatePart(Name = PART_SearchBar, Type = typeof(SearchBar))]
    public class PropertyGrid : Control
    {
        #region GotFocusProperty 
        /// <summary>
        /// Border圆角
        /// </summary>
        public static readonly DependencyProperty GotFocusPropertyProperty = DependencyProperty.RegisterAttached(
            "GotFocusProperty", typeof(PropertyGrid), typeof(PropertyGrid), new FrameworkPropertyMetadata(null, OnGotFocusPropertyChanged));

        private static void OnGotFocusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is PropertyGrid oldvalue)
            {
                if (d is Panel panel)
                {
                    foreach (var child in VisualHelper.GetPanelChildren(panel).OfType<UIElement>())
                    {
                        child.GotFocus -= Child_GotFocus;
                    }
                }
            }

            if (e.NewValue is PropertyGrid newvalue)
            {
                if (d is Panel panel)
                {
                    foreach (var child in VisualHelper.GetPanelChildren(panel).OfType<UIElement>())
                    {
                        PropertyGrid.SetGotFocusProperty(child, newvalue);

                        child.GotFocus -= Child_GotFocus;
                        child.GotFocus += Child_GotFocus;
                    }
                }
            }
        }



        private static void Child_GotFocus(object sender, RoutedEventArgs e)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                var property = PropertyGrid.GetGotFocusProperty(element);
                if (property.IsVisible)
                {
                    property.SelectedObject = element;
                }
            }
        }

        public static PropertyGrid GetGotFocusProperty(DependencyObject d)
        {
            return (PropertyGrid)d.GetValue(GotFocusPropertyProperty);
        }

        public static void SetGotFocusProperty(DependencyObject obj, PropertyGrid value)
        {
            obj.SetValue(GotFocusPropertyProperty, value);
        }
        #endregion

        private const string PART_ItemsControl = "PART_ItemsControl";
        private const string PART_SearchBar = "PART_SearchBar";

        private ItemsControl _itemsControl;

        private ICollectionView _dataView;

        private SearchBar _searchBar;

        private string _searchKey;

        static PropertyGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

        public PropertyGrid()
        {
            CommandBindings.Add(new CommandBinding(ControlCommands.SortByCategory, SortByCategory, (s, e) => e.CanExecute = ShowSortButton));
            CommandBindings.Add(new CommandBinding(ControlCommands.SortByName, SortByName, (s, e) => e.CanExecute = ShowSortButton));

            this.IsVisibleChanged += PropertyGrid_IsVisibleChanged;
        }

        public virtual PropertyResolver PropertyResolver { get; } = new PropertyResolver();

        public static readonly RoutedEvent SelectedObjectChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(SelectedObjectChanged), RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<object>), typeof(PropertyGrid));

        public event RoutedPropertyChangedEventHandler<object> SelectedObjectChanged
        {
            add => AddHandler(SelectedObjectChangedEvent, value);
            remove => RemoveHandler(SelectedObjectChangedEvent, value);
        }

        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register(
            nameof(SelectedObject), typeof(object), typeof(PropertyGrid), new PropertyMetadata(default, OnSelectedObjectChanged));

        private static void OnSelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (PropertyGrid)d;
            ctl.OnSelectedObjectChanged(e.OldValue, e.NewValue);
        }

        public object SelectedObject
        {
            get => GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }

        protected virtual void OnSelectedObjectChanged(object oldValue, object newValue)
        {
            UpdateItems(newValue);
            RaiseEvent(new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, SelectedObjectChangedEvent));
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description), typeof(string), typeof(PropertyGrid), new PropertyMetadata(default(string)));

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty TitleWidthProperty = DependencyProperty.Register(
            nameof(TitleWidth), typeof(double), typeof(PropertyGrid), new PropertyMetadata(100d));

        public double TitleWidth
        {
            get => (double)GetValue(TitleWidthProperty);
            set => SetValue(TitleWidthProperty, value);
        }

        public static readonly DependencyProperty ShowSortButtonProperty = DependencyProperty.Register(
            nameof(ShowSortButton), typeof(bool), typeof(PropertyGrid), new PropertyMetadata(true));

        public bool ShowSortButton
        {
            get => (bool)GetValue(ShowSortButtonProperty);
            set => SetValue(ShowSortButtonProperty, value);
        }

        public override void OnApplyTemplate()
        {
            if (_searchBar != null)
            {
                _searchBar.SearchStarted -= SearchBar_SearchStarted;
            }

            base.OnApplyTemplate();

            _itemsControl = GetTemplateChild(PART_ItemsControl) as ItemsControl;
            _searchBar = GetTemplateChild(PART_SearchBar) as SearchBar;

            if (_searchBar != null)
            {
                _searchBar.SearchStarted += SearchBar_SearchStarted;
            }

            UpdateItems(SelectedObject);
        }

        private void PropertyGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateItems(SelectedObject);
        }

        private object _lastSelectedObject;

        private void UpdateItems(object obj)
        {
            if (obj == null || _itemsControl == null)
                return;

            if (IsVisible == false)
                return;

            if (_lastSelectedObject == obj)
                return;

            _dataView = CollectionViewSource.GetDefaultView(TypeDescriptor.GetProperties(obj.GetType()).OfType<PropertyDescriptor>()
                .Where(item => PropertyResolver.ResolveIsBrowsable(item)).Select(CreatePropertyItem)
                .Do(item => item.InitElement()));

            SortByCategory(null, null);
            _itemsControl.ItemsSource = _dataView;
            _lastSelectedObject = obj;
        }

        private void SortByCategory(object sender, ExecutedRoutedEventArgs e)
        {
            if (_dataView == null) return;

            using (_dataView.DeferRefresh())
            {
                _dataView.GroupDescriptions.Clear();
                _dataView.SortDescriptions.Clear();
                _dataView.SortDescriptions.Add(new SortDescription(PropertyItem.CategoryProperty.Name, ListSortDirection.Ascending));
                _dataView.SortDescriptions.Add(new SortDescription(PropertyItem.DisplayNameProperty.Name, ListSortDirection.Ascending));
                _dataView.GroupDescriptions.Add(new PropertyGroupDescription(PropertyItem.CategoryProperty.Name));
            }
        }

        private void SortByName(object sender, ExecutedRoutedEventArgs e)
        {
            if (_dataView == null) return;

            using (_dataView.DeferRefresh())
            {
                _dataView.GroupDescriptions.Clear();
                _dataView.SortDescriptions.Clear();
                _dataView.SortDescriptions.Add(new SortDescription(PropertyItem.PropertyNameProperty.Name, ListSortDirection.Ascending));
            }
        }

        private void SearchBar_SearchStarted(object sender, FunctionEventArgs<string> e)
        {
            if (_dataView == null) return;

            _searchKey = e.Info;
            if (string.IsNullOrEmpty(_searchKey))
            {
                foreach (UIElement item in _dataView)
                {
                    item.Visibility = Visibility.Visible;
                }
            }
            else
            {
                foreach (PropertyItem item in _dataView)
                {
                    item.Visibility = (item.PropertyName.ToLower().Contains(_searchKey.ToLower()) || item.DisplayName.ToLower().Contains(_searchKey.ToLower())) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        protected virtual PropertyItem CreatePropertyItem(PropertyDescriptor propertyDescriptor) => new PropertyItem()
        {
            Category = PropertyResolver.ResolveCategory(propertyDescriptor),
            DisplayName = PropertyResolver.ResolveDisplayName(propertyDescriptor),
            Description = PropertyResolver.ResolveDescription(propertyDescriptor),
            IsReadOnly = PropertyResolver.ResolveIsReadOnly(propertyDescriptor),
            DefaultValue = PropertyResolver.ResolveDefaultValue(propertyDescriptor),
            Editor = PropertyResolver.ResolveEditor(propertyDescriptor),
            Value = SelectedObject,
            PropertyName = propertyDescriptor.Name,
            PropertyType = propertyDescriptor.PropertyType,
            PropertyTypeName = $"{propertyDescriptor.PropertyType.Namespace}.{propertyDescriptor.PropertyType.Name}"
        };
    }

    public static class EnumerableExtension
    {
        public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> predicate)
        {
            var enumerable = source as IList<TSource> ?? source.ToList();
            foreach (var item in enumerable)
            {
                predicate.Invoke(item);
            }

            return enumerable;
        }

        public static IEnumerable<TSource> DoWithIndex<TSource>(this IEnumerable<TSource> source, Action<TSource, int> predicate)
        {
            var enumerable = source as IList<TSource> ?? source.ToList();
            for (var i = 0; i < enumerable.Count; i++)
            {
                predicate.Invoke(enumerable[i], i);
            }

            return enumerable;
        }
    }
}
