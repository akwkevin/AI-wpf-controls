﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    #region TreeView
    public enum SelectMode
    {
        Any,
        ChildOnly,
        Disabled
    }

    public enum TreeExpandMode
    {
        DoubleClick,
        SingleClick
    }

    public enum ExpandBehaviour
    {
        Any,
        OnlyOne,
    }
    #endregion

    public static class TreeViewAttach
    {
        #region ControlStyle
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static TreeViewControlStyle GetControlStyle(DependencyObject obj)
        {
            return (TreeViewControlStyle)obj.GetValue(ControlStyleProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static void SetControlStyle(DependencyObject obj, TreeViewControlStyle value)
        {
            obj.SetValue(ControlStyleProperty, value);
        }

        public static readonly DependencyProperty ControlStyleProperty =
            DependencyProperty.RegisterAttached("ControlStyle", typeof(TreeViewControlStyle), typeof(TreeViewAttach), new PropertyMetadata(TreeViewControlStyle.Standard));
        #endregion

        #region SelectMode
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static SelectMode GetSelectMode(DependencyObject obj)
        {
            return (SelectMode)obj.GetValue(SelectModeProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static void SetSelectMode(DependencyObject obj, SelectMode value)
        {
            obj.SetValue(SelectModeProperty, value);
        }

        public static readonly DependencyProperty SelectModeProperty =
            DependencyProperty.RegisterAttached("SelectMode", typeof(SelectMode), typeof(TreeViewAttach), new PropertyMetadata(SelectMode.Any));
        #endregion

        #region ExpandMode
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static TreeExpandMode GetExpandMode(DependencyObject obj)
        {
            return (TreeExpandMode)obj.GetValue(ExpandModeProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static void SetExpandMode(DependencyObject obj, TreeExpandMode value)
        {
            obj.SetValue(ExpandModeProperty, value);
        }

        public static readonly DependencyProperty ExpandModeProperty =
            DependencyProperty.RegisterAttached("ExpandMode", typeof(TreeExpandMode), typeof(TreeViewAttach), new PropertyMetadata(TreeExpandMode.DoubleClick, OnExpandModeChanged));

        private static void OnExpandModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var treeView = d as TreeView;

            treeView.RemoveHandler(TreeViewItem.PreviewMouseDownEvent, new RoutedEventHandler(OnExpandModeItemSelected));

            if ((TreeExpandMode)e.NewValue == TreeExpandMode.SingleClick)
            {
                treeView.AddHandler(TreeViewItem.PreviewMouseDownEvent, new RoutedEventHandler(OnExpandModeItemSelected));
            }
        }

        private static void OnExpandModeItemSelected(object sender, RoutedEventArgs e)
        {
            var treeView = sender as TreeView;
            TreeViewItem treeViewItem = null;


            if (e.Source is TreeViewItem)
            {
                treeViewItem = e.Source as TreeViewItem;
            }
            else
            {
                treeViewItem = GetTreeViewItemFromChild(treeView, e.OriginalSource as UIElement);
            }

            if (treeViewItem != null && treeViewItem.HasItems)
            {
                treeViewItem.IsExpanded = !treeViewItem.IsExpanded;
            }
        }

        private static TreeViewItem GetTreeViewItemFromChild(TreeView treeView, UIElement child)
        {
            try
            {
                UIElement target = child;

                while ((target != null) && !(target is TreeViewItem))
                    target = VisualTreeHelper.GetParent(target) as UIElement;

                return target as TreeViewItem;
            }
            catch
            {
                return null;
            }

        }
        #endregion

        #region ExpandBehaviour
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static ExpandBehaviour GetExpandBehaviour(DependencyObject obj)
        {
            return (ExpandBehaviour)obj.GetValue(ExpandBehaviourProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static void SetExpandBehaviour(DependencyObject obj, ExpandBehaviour value)
        {
            obj.SetValue(ExpandBehaviourProperty, value);
        }

        public static readonly DependencyProperty ExpandBehaviourProperty =
            DependencyProperty.RegisterAttached("ExpandBehaviour", typeof(ExpandBehaviour), typeof(TreeViewAttach), new PropertyMetadata(OnExpandBehaviourChanged));

        private static void OnExpandBehaviourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var treeView = d as TreeView;

            treeView.RemoveHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(OnExpandBehaviourItemSelected));

            if ((ExpandBehaviour)e.NewValue == ExpandBehaviour.OnlyOne)
            {
                treeView.AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(OnExpandBehaviourItemSelected));
            }
        }

        private static void OnExpandBehaviourItemSelected(object sender, RoutedEventArgs e)
        {
            var treeView = sender as TreeView;
            TreeViewItem treeViewItem = null;

            if (e.Source is TreeViewItem)
            {
                treeViewItem = e.Source as TreeViewItem;
            }
            else
            {
                treeViewItem = GetTreeViewItemFromChild(treeView, e.OriginalSource as UIElement);
            }

            if (treeViewItem != null && treeViewItem.HasItems)
            {
                var lastTreeViewItem = GetLastExpandedItem(treeView);
                if (lastTreeViewItem != null)
                {
                    if (lastTreeViewItem == treeViewItem)
                        return;

                    var parentnew = GetParentTreeViewItems(treeViewItem);
                    var parentold = GetParentTreeViewItems(lastTreeViewItem);
                    foreach (var old in parentold)
                    {
                        if (!parentnew.Contains(old))
                        {
                            old.IsExpanded = false;
                        }
                    }
                }

                SetLastExpandedItem(treeView, treeViewItem);
            }
        }

        public static List<TreeViewItem> GetParentTreeViewItems(TreeViewItem treeViewItem)
        {
            List<TreeViewItem> items = new List<TreeViewItem>();
            items.Add(treeViewItem);
            DependencyObject parent = VisualTreeHelper.GetParent(treeViewItem);
            while (parent != null)
            {
                if (parent is TreeViewItem)
                {
                    items.Add(parent as TreeViewItem);
                }
                parent = VisualTreeHelper.GetParent(parent);
            }

            return items;
        }

        #endregion

        #region ExpandedBrush
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static Brush GetExpandedBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(ExpandedBrushProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static void SetExpandedBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(ExpandedBrushProperty, value);
        }

        public static readonly DependencyProperty ExpandedBrushProperty =
            DependencyProperty.RegisterAttached("ExpandedBrush", typeof(Brush), typeof(TreeViewAttach));
        #endregion

        #region SelectedBackground
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static Brush GetSelectedBackground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(SelectedBackgroundProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static void SetSelectedBackground(DependencyObject obj, Brush value)
        {
            obj.SetValue(SelectedBackgroundProperty, value);
        }

        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.RegisterAttached("SelectedBackground", typeof(Brush), typeof(TreeViewAttach));
        #endregion

        #region SelectedForeground
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static Brush GetSelectedForeground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(SelectedForegroundProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static void SetSelectedForeground(DependencyObject obj, Brush value)
        {
            obj.SetValue(SelectedForegroundProperty, value);
        }

        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.RegisterAttached("SelectedForeground", typeof(Brush), typeof(TreeViewAttach));
        #endregion

        #region ItemHeight
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static double GetItemHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(ItemHeightProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static void SetItemHeight(DependencyObject obj, double value)
        {
            obj.SetValue(ItemHeightProperty, value);
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.RegisterAttached("ItemHeight", typeof(double), typeof(TreeViewAttach));
        #endregion

        #region ItemIcon
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static object GetItemIcon(DependencyObject obj)
        {
            return obj.GetValue(ItemIconProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static void SetItemIcon(DependencyObject obj, object value)
        {
            obj.SetValue(ItemIconProperty, value);
        }

        public static readonly DependencyProperty ItemIconProperty =
            DependencyProperty.RegisterAttached("ItemIcon", typeof(object), typeof(TreeViewAttach));
        #endregion

        #region ItemPadding
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static Thickness GetItemPadding(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(ItemPaddingProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        public static void SetItemPadding(DependencyObject obj, Thickness value)
        {
            obj.SetValue(ItemPaddingProperty, value);
        }

        public static readonly DependencyProperty ItemPaddingProperty =
            DependencyProperty.RegisterAttached("ItemPadding", typeof(Thickness), typeof(TreeViewAttach));
        #endregion

        #region (Internal) LastSelectedItem
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        internal static TreeViewItem GetLastSelectedItem(DependencyObject obj)
        {
            return (TreeViewItem)obj.GetValue(LastSelecteedItemProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        internal static void SetLastSelecteedItem(DependencyObject obj, TreeViewItem value)
        {
            obj.SetValue(LastSelecteedItemProperty, value);
        }

        internal static readonly DependencyProperty LastSelecteedItemProperty =
            DependencyProperty.RegisterAttached("LastSelecteedItem", typeof(TreeViewItem), typeof(TreeViewAttach));
        #endregion

        #region (Internal) LastExpandedItem
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        internal static TreeViewItem GetLastExpandedItem(DependencyObject obj)
        {
            return (TreeViewItem)obj.GetValue(LastExpandedItemProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(TreeView))]
        internal static void SetLastExpandedItem(DependencyObject obj, TreeViewItem value)
        {
            obj.SetValue(LastExpandedItemProperty, value);
        }

        internal static readonly DependencyProperty LastExpandedItemProperty =
            DependencyProperty.RegisterAttached("LastExpandedItem", typeof(TreeViewItem), typeof(TreeViewAttach));
        #endregion

        #region (Internal) TreeViewHook
        private static void TreeView_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                var treeView = sender as TreeView;
                var selectMode = GetSelectMode(treeView);

                if (selectMode == SelectMode.Any)
                    return;

                var sourceData = treeView.SelectedItem;
                if (sourceData is TreeViewItem)
                {
                    if (selectMode == SelectMode.ChildOnly && ((TreeViewItem)sourceData).HasItems)
                        e.Handled = true;
                    else if (selectMode == SelectMode.Disabled)
                        e.Handled = true;
                }
                else
                {
                    if (selectMode == SelectMode.Disabled)
                        e.Handled = true;
                    else if (selectMode == SelectMode.ChildOnly)
                    {
                        if (!(treeView.ItemTemplate is HierarchicalDataTemplate))
                            return;
                        var itemsPath = ((System.Windows.Data.Binding)((HierarchicalDataTemplate)treeView.ItemTemplate)?.ItemsSource)?.Path?.Path;
                        if (string.IsNullOrEmpty(itemsPath))
                            return;

                        var propertyInfo = sourceData.GetType().GetProperty(itemsPath);
                        if (propertyInfo == null)
                            return;

                        var children = propertyInfo.GetValue(sourceData, null) as ICollection;
                        if (children == null)
                            return;

                        if (children != null && children.Count != 0)
                            e.Handled = true;
                    }
                }
            }
            catch { }
        }

        private static void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                var treeView = sender as TreeView;

                var selectMode = GetSelectMode(treeView);

                if (selectMode == SelectMode.Any)
                    return;

                if (e.OriginalSource is TreeViewItem)
                {
                    var treeViewItem = e.OriginalSource as TreeViewItem;

                    var oldItem = GetLastSelectedItem(treeView);
                    if ((selectMode == SelectMode.ChildOnly && treeViewItem.HasItems) || selectMode == SelectMode.Disabled)
                    {
                        treeViewItem.IsSelected = false;

                        if (oldItem != null && !oldItem.IsSelected)
                        {
                            SetLastSelecteedItem(treeView, null);
                            oldItem.IsSelected = true;
                        }
                    }
                    else
                    {
                        if (oldItem != null)
                            oldItem.IsSelected = false;
                        SetLastSelecteedItem(treeView, treeViewItem);
                    }
                }
            }
            catch { }
        }

        #endregion
    }
}
