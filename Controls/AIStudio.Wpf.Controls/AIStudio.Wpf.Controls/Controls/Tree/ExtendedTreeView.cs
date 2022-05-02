using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace AIStudio.Wpf.Controls
{
    public class ExtendedTreeView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            var childItem = ExtendedTreeViewItem.CreateItemWithBinding();

            childItem.OnHierarchyMouseUp += OnChildItemMouseLeftButtonUp;

            return childItem;
        }

        private void OnChildItemMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            if (this.OnHierarchyMouseUp != null)
            {
                this.OnHierarchyMouseUp(this, e);
            }
        }

        public event RoutedEventHandler OnHierarchyMouseUp;
    }

    public class ExtendedTreeViewItem : TreeViewItem
    {
        public ExtendedTreeViewItem()
        {
            //this.MouseLeftButtonUp += OnMouseLeftButtonUp;
            AddHandler(TreeViewItem.MouseLeftButtonUpEvent, new RoutedEventHandler(OnMouseLeftButtonUp), true);
        }

        private void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            if (this.OnHierarchyMouseUp != null)
            {
                this.OnHierarchyMouseUp(this, e);
            }
        }
        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.OnHierarchyMouseUp != null)
            {
                this.OnHierarchyMouseUp(this, e);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var childItem = CreateItemWithBinding();

            //childItem.MouseLeftButtonUp += OnMouseLeftButtonUp;
            childItem.AddHandler(TreeViewItem.MouseLeftButtonUpEvent, new RoutedEventHandler(OnMouseLeftButtonUp), true);

            return childItem;
        }

        public static ExtendedTreeViewItem CreateItemWithBinding()
        {
            var tvi = new ExtendedTreeViewItem();

            var expandedBinding = new Binding("IsExpanded");
            expandedBinding.Mode = BindingMode.TwoWay;
            tvi.SetBinding(ExtendedTreeViewItem.IsExpandedProperty, expandedBinding);

            var selectedBinding = new Binding("IsSelected");
            selectedBinding.Mode = BindingMode.TwoWay;
            tvi.SetBinding(ExtendedTreeViewItem.IsSelectedProperty, selectedBinding);

            return tvi;
        }

        public event RoutedEventHandler OnHierarchyMouseUp;
    }
}
