using AIStudio.Wpf.GridControls.Initializer;
using AIStudio.Wpf.GridControls.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace AIStudio.Wpf.GridControls
{
    /// <summary>
    /// Represent BolapanControls.PropertyFilter.ColumnFilter control, which show filter View for associated Model.
    /// If defined as part of System.Windows.Controls.Primitives.DataGridColumnHeader template, represent filter View for current column.
    /// </summary>
    [TemplateVisualState(GroupName = "FilterState", Name = "Disable")]
    [TemplateVisualState(GroupName = "FilterState", Name = "Enable")]
    [TemplateVisualState(GroupName = "FilterState", Name = "Active")]
    [TemplateVisualState(GroupName = "FilterState", Name = "Open")]
    [TemplateVisualState(GroupName = "FilterState", Name = "OpenActive")]
    [TemplatePart(Name = ColumnFilter.PART_FiltersView, Type = typeof(Popup))]
    public class ColumnFilter : FilterControl
    {
        internal const string PART_FiltersView = "PART_FilterView";
        #region BindingPath

        /// <summary>
        /// BindingPath Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty BindingPathProperty =
            DependencyProperty.RegisterAttached("BindingPath", typeof(string), typeof(ColumnFilter),
                new FrameworkPropertyMetadata((string)null));

        /// <summary>
        /// Gets the BindingPath property. This dependency property 
        /// indicates path to the property in ParentCollection.
        /// </summary>
        public static string GetBindingPath(DependencyObject d)
        {
            return (string)d.GetValue(BindingPathProperty);
        }

        /// <summary>
        /// Sets the BindingPath property. This dependency property 
        /// indicates path to the property in ParentCollection.
        /// </summary>
        public static void SetBindingPath(DependencyObject d, string value)
        {
            d.SetValue(BindingPathProperty, value);
        }

        #endregion
        #region Initializers

        /// <summary>
        /// Initializers Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty InitializersProperty =
            DependencyProperty.RegisterAttached("Initializers", typeof(FilterInitializersManager), typeof(ColumnFilter),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the FilterInitializersManager  that used for generate ColumnFilter.Model. 
        /// </summary>
        public static FilterInitializersManager GetInitializers(DependencyObject d)
        {
            return (FilterInitializersManager)d.GetValue(InitializersProperty);
        }

        /// <summary>
        /// Sets the FilterInitializersManager that used for generate ColumnFilter.Model.
        /// </summary>
        public static void SetInitializers(DependencyObject d, FilterInitializersManager value)
        {
            d.SetValue(InitializersProperty, value);
        }

        #endregion


        static ColumnFilter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColumnFilter), new FrameworkPropertyMetadata(typeof(ColumnFilter)));
            CommandManager.RegisterClassCommandBinding(typeof(ColumnFilter),
                new CommandBinding(FilterCommand.Show, DoShowFilter, CanShowFilter));
        }

        private Popup partFilterView;
        /// <summary>
        /// Create new instance of BolapanControls.PropertyFilter.ColumnFilter class.
        /// </summary>
        public ColumnFilter()
        {
            //CommandBindings.Add(new CommandBinding(FilterCommand.Show,DoShowFilter,CanShowFilter));
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            partFilterView = GetTemplateChild(PART_FiltersView) as Popup;
        }

        /// <summary>
        /// Initializes the filter view model.
        /// </summary>
        protected override FilterControlVm CreateModel()
        {
            FilterControlVm vm = null;
            filterPresenter = Parent == null ? null : FilterPresenter.TryGet(ParentCollection);
            if (filterPresenter != null)
            {
                if (Key == null)
                {
                    DataGridColumnHeader columnHeader = this.GetParent<DataGridColumnHeader>();
                    if (columnHeader == null)
                        return null;
                    DataGridColumn column = columnHeader.Column;
                    if (column == null)
                    {
                        return null;
                    }
                    DataGrid dataGrid = columnHeader.GetParent<DataGrid>();
                    if (dataGrid == null)
                    {
                        return null;
                    }
                    if (column.DisplayIndex >= dataGrid.Columns.Count)
                    {
                        return null;
                    }
                    IEnumerable<FilterInitializer> initializers = GetInitializers(column) ?? FilterInitializersManager;
                    string key = Key ?? GetColumnKey(column);
                    vm = filterPresenter.TryGetFilterControlVm(key, initializers);
                    
                    columnHeader.MouseEnter -= ColumnHeader_MouseEnter;
                    columnHeader.MouseEnter += ColumnHeader_MouseEnter;

                    columnHeader.MouseLeave -= ColumnHeader_MouseLeave;
                    columnHeader.MouseLeave += ColumnHeader_MouseLeave;
                }
                else
                {
                    IEnumerable<FilterInitializer> initializers = FilterInitializersManager;
                    vm = filterPresenter.TryGetFilterControlVm(Key, initializers);
                }
                if (vm != null)
                    vm.IsEnable = true;
            }
            return vm;
        }

        private void ColumnHeader_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Model != null)
            {
                Model.IsMouseOver = true;
            }
        }

        private void ColumnHeader_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Model != null && Model.State == State.MouseOver)
            {
                Model.IsMouseOver = false;
            }
        }



        //
        // Summary:
        //     Invoked when an unhandled System.Windows.UIElement.MouseLeftButtonDown routed
        //     event is raised on this element. Implement this method to add class handling
        //     for this event.
        //
        // Parameters:
        //   e:
        //     The System.Windows.Input.MouseButtonEventArgs that contains the event data.
        //     The event data reports that the left mouse button was pressed.
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            base.OnMouseLeftButtonDown(e);

        }

        private static void CanShowFilter(object sender, CanExecuteRoutedEventArgs e)
        {
            ColumnFilter filter = (ColumnFilter)sender;
            e.CanExecute = filter.Model != null && filter.Model.IsEnable && filter.partFilterView != null;
        }

        private static void DoShowFilter(object sender, ExecutedRoutedEventArgs e)
        {
            ((ColumnFilter)sender).Model.IsOpen = true;
        }

        private string GetColumnKey(DataGridColumn column)
        {
            string attachedBinding = GetBindingPath(column) as string;
            if (attachedBinding != null)
            {
                return String.IsNullOrWhiteSpace(attachedBinding) ? (string)null : attachedBinding;
                ;
            }

            string bindingPath = null;
            if (column is DataGridBoundColumn)
            {
                DataGridBoundColumn columnBound = column as DataGridBoundColumn;
                Binding binding = columnBound.Binding as Binding;
                if (binding != null)
                {
                    bindingPath = binding.Path.Path;
                }
            }
            else if (column is DataGridTemplateColumn)
            {
                DataGridTemplateColumn templateColumn = column as DataGridTemplateColumn;
                string header = templateColumn.Header as string;
                if (header == null)
                {
                    return null;
                }
                bindingPath = header;
            }
            else if (column is DataGridComboBoxColumn)
            {
                DataGridComboBoxColumn comboBoxColumn = column as DataGridComboBoxColumn;
                Binding binding = comboBoxColumn.SelectedItemBinding as Binding;
                bindingPath = binding == null ? null : binding.Path.Path;
            }

            if (bindingPath == null || string.IsNullOrEmpty(bindingPath))
            {
                return null;
            }
            if (bindingPath.Contains("."))
                return bindingPath;
            return bindingPath;
        }



    }
}
