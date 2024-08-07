﻿using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace AIStudio.Wpf.GridControls
{
    #region ListViewDragDropManager

    /// <summary>
    /// Manages the dragging and dropping of ListViewItems in a ListBox.
    /// The ItemType type parameter indicates the type of the objects in
    /// the ListBox's items source.  The ListBox's ItemsSource must be 
    /// set to an instance of ObservableCollection of ItemType, or an 
    /// Exception will be thrown.
    /// </summary>
    /// <typeparam name="ItemType">The type of the ListBox's items.</typeparam>
    public class ListBoxDragDropManager<ItemType> where ItemType : class
    {
        #region Data

        bool canInitiateDrag;
        DragAdorner dragAdorner;
        double dragAdornerOpacity;
        int indexToSelect;
        bool isDragInProgress;
        ItemType itemUnderDragCursor;
        ListBox listBox;
        Point ptMouseDown;
        bool showDragAdorner;

        #endregion // Data

        #region Constructors

        /// <summary>
        /// Initializes a new instance of ListViewDragManager.
        /// </summary>
        public ListBoxDragDropManager()
        {
            this.canInitiateDrag = false;
            this.dragAdornerOpacity = 0.7;
            this.indexToSelect = -1;
            this.showDragAdorner = true;
        }

        /// <summary>
        /// Initializes a new instance of ListViewDragManager.
        /// </summary>
        /// <param name="listBox"></param>
        public ListBoxDragDropManager(ListBox listBox)
            : this()
        {
            this.ListBox = listBox;
        }

        /// <summary>
        /// Initializes a new instance of ListViewDragManager.
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="dragAdornerOpacity"></param>
        public ListBoxDragDropManager(ListBox listBox, double dragAdornerOpacity)
            : this(listBox)
        {
            this.DragAdornerOpacity = dragAdornerOpacity;
        }

        /// <summary>
        /// Initializes a new instance of ListViewDragManager.
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="showDragAdorner"></param>
        public ListBoxDragDropManager(ListBox listBox, bool showDragAdorner)
            : this(listBox)
        {
            this.ShowDragAdorner = showDragAdorner;
        }

        #endregion // Constructors

        #region Public Interface

        #region DragAdornerOpacity

        /// <summary>
        /// Gets/sets the opacity of the drag adorner.  This property has no
        /// effect if ShowDragAdorner is false. The default value is 0.7
        /// </summary>
        public double DragAdornerOpacity
        {
            get
            {
                return this.dragAdornerOpacity;
            }
            set
            {
                if (this.IsDragInProgress)
                    throw new InvalidOperationException("Cannot set the DragAdornerOpacity property during a drag operation.");

                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException("DragAdornerOpacity", value, "Must be between 0 and 1.");

                this.dragAdornerOpacity = value;
            }
        }

        #endregion // DragAdornerOpacity

        #region IsDragInProgress

        /// <summary>
        /// Returns true if there is currently a drag operation being managed.
        /// </summary>
        public bool IsDragInProgress
        {
            get
            {
                return this.isDragInProgress;
            }
            private set
            {
                this.isDragInProgress = value;
            }
        }

        #endregion // IsDragInProgress

        #region ListBox

        /// <summary>
        /// Gets/sets the ListBox whose dragging is managed.  This property
        /// can be set to null, to prevent drag management from occuring.  If
        /// the ListBox's AllowDrop property is false, it will be set to true.
        /// </summary>
        public ListBox ListBox
        {
            get
            {
                return listBox;
            }
            set
            {
                if (this.IsDragInProgress)
                    throw new InvalidOperationException("Cannot set the ListBox property during a drag operation.");

                if (this.listBox != null)
                {
                    #region Unhook Events

                    this.listBox.PreviewMouseLeftButtonDown -= listBox_PreviewMouseLeftButtonDown;
                    this.listBox.PreviewMouseMove -= listBox_PreviewMouseMove;
                    this.listBox.DragOver -= listBox_DragOver;
                    this.listBox.DragLeave -= listBox_DragLeave;
                    this.listBox.DragEnter -= listBox_DragEnter;
                    this.listBox.Drop -= listBox_Drop;

                    #endregion // Unhook Events
                }

                this.listBox = value;

                if (this.listBox != null)
                {
                    if (!this.listBox.AllowDrop)
                        this.listBox.AllowDrop = true;

                    #region Hook Events

                    this.listBox.PreviewMouseLeftButtonDown += listBox_PreviewMouseLeftButtonDown;
                    this.listBox.PreviewMouseMove += listBox_PreviewMouseMove;
                    this.listBox.DragOver += listBox_DragOver;
                    this.listBox.DragLeave += listBox_DragLeave;
                    this.listBox.DragEnter += listBox_DragEnter;
                    this.listBox.Drop += listBox_Drop;

                    #endregion // Hook Events
                }
            }
        }

        #endregion // ListBox

        #region ProcessDrop [event]

        /// <summary>
        /// Raised when a drop occurs.  By default the dropped item will be moved
        /// to the target index.  Handle this event if relocating the dropped item
        /// requires custom behavior.  Note, if this event is handled the default
        /// item dropping logic will not occur.
        /// </summary>
        public event EventHandler<ProcessDropEventArgs<ItemType>> ProcessDrop;

        public event EventHandler<ProcessDropBeforeMoveEventArgs> ProcessDropBeforeMove;

        #endregion // ProcessDrop [event]

        #region ShowDragAdorner

        /// <summary>
        /// Gets/sets whether a visual representation of the ListBoxItem being dragged
        /// follows the mouse cursor during a drag operation.  The default value is true.
        /// </summary>
        public bool ShowDragAdorner
        {
            get
            {
                return this.showDragAdorner;
            }
            set
            {
                if (this.IsDragInProgress)
                    throw new InvalidOperationException("Cannot set the ShowDragAdorner property during a drag operation.");

                this.showDragAdorner = value;
            }
        }

        #endregion // ShowDragAdorner

        #endregion // Public Interface

        #region Event Handling Methods

        #region listBox_PreviewMouseLeftButtonDown

        void listBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseOverScrollbar)
            {
                // 4/13/2007 - Set the flag to false when cursor is over scrollbar.
                this.canInitiateDrag = false;
                return;
            }

            int index = this.IndexUnderDragCursor;
            this.canInitiateDrag = index > -1;

            if (this.canInitiateDrag)
            {
                // Remember the location and index of the ListBoxItem the user clicked on for later.
                this.ptMouseDown = MouseUtilities.GetMousePosition(this.listBox);
                this.indexToSelect = index;
            }
            else
            {
                this.ptMouseDown = new Point(-10000, -10000);
                this.indexToSelect = -1;
            }
        }

        #endregion // listBox_PreviewMouseLeftButtonDown

        #region listBox_PreviewMouseMove

        void listBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!this.CanStartDragOperation)
                return;

            // Select the item the user clicked on.
            if (this.listBox.SelectedIndex != this.indexToSelect)
                this.listBox.SelectedIndex = this.indexToSelect;

            // If the item at the selected index is null, there's nothing
            // we can do, so just return;
            if (this.listBox.SelectedItem == null)
                return;

            ListBoxItem itemToDrag = this.GetListBoxItem(this.listBox.SelectedIndex);
            if (itemToDrag == null)
                return;

            AdornerLayer adornerLayer = this.ShowDragAdornerResolved ? this.InitializeAdornerLayer(itemToDrag) : null;

            this.InitializeDragOperation(itemToDrag);
            this.PerformDragOperation();
            this.FinishDragOperation(itemToDrag, adornerLayer);
        }

        #endregion // listBox_PreviewMouseMove

        #region listBox_DragOver

        void listBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;

            if (this.ShowDragAdornerResolved)
                this.UpdateDragAdornerLocation();

            // Update the item which is known to be currently under the drag cursor.
            int index = this.IndexUnderDragCursor;
            this.ItemUnderDragCursor = index < 0 ? null : this.ListBox.Items[index] as ItemType;
        }

        #endregion // listBox_DragOver

        #region listBox_DragLeave

        void listBox_DragLeave(object sender, DragEventArgs e)
        {
            if (!this.IsMouseOver(this.listBox))
            {
                if (this.ItemUnderDragCursor != null)
                    this.ItemUnderDragCursor = null;

                if (this.dragAdorner != null)
                    this.dragAdorner.Visibility = Visibility.Collapsed;
            }
        }

        #endregion // listBox_DragLeave

        #region listBox_DragEnter

        void listBox_DragEnter(object sender, DragEventArgs e)
        {
            if (this.dragAdorner != null && this.dragAdorner.Visibility != Visibility.Visible)
            {
                // Update the location of the adorner and then show it.				
                this.UpdateDragAdornerLocation();
                this.dragAdorner.Visibility = Visibility.Visible;
            }
        }

        #endregion // listBox_DragEnter

        #region listBox_Drop

        void listBox_Drop(object sender, DragEventArgs e)
        {
            if (this.ItemUnderDragCursor != null)
                this.ItemUnderDragCursor = null;

            e.Effects = DragDropEffects.None;

            if (!e.Data.GetDataPresent(typeof(ItemType)))
                return;

            // Get the data object which was dropped.
            ItemType data = e.Data.GetData(typeof(ItemType)) as ItemType;
            if (data == null)
                return;

            // Get the ObservableCollection<ItemType> which contains the dropped data object.
            ObservableCollection<ItemType> itemsSource = this.listBox.ItemsSource as ObservableCollection<ItemType>;
            if (itemsSource == null)
                throw new Exception(
                    "A ListBox managed by ListViewDragManager must have its ItemsSource set to an ObservableCollection<ItemType>.");

            int oldIndex = itemsSource.IndexOf(data);
            int newIndex = this.IndexUnderDragCursor;

            if (newIndex < 0)
            {
                // The drag started somewhere else, and our ListBox is empty
                // so make the new item the first in the list.
                if (itemsSource.Count == 0)
                    newIndex = 0;

                // The drag started somewhere else, but our ListBox has items
                // so make the new item the last in the list.
                else if (oldIndex < 0)
                    newIndex = itemsSource.Count;

                // The user is trying to drop an item from our ListBox into
                // our ListBox, but the mouse is not over an item, so don't
                // let them drop it.
                else
                    return;
            }

            // Dropping an item back onto itself is not considered an actual 'drop'.
            if (oldIndex == newIndex)
                return;

            if (this.ProcessDrop != null)
            {
                // Let the client code process the drop.
                ProcessDropEventArgs<ItemType> args = new ProcessDropEventArgs<ItemType>(itemsSource, data, oldIndex, newIndex, e.AllowedEffects);
                this.ProcessDrop(this, args);
                e.Effects = args.Effects;
            }
            else
            {
                // Move the dragged data object from it's original index to the
                // new index (according to where the mouse cursor is).  If it was
                // not previously in the ListBox, then insert the item.
                if (oldIndex > -1)
                {
                    if (ProcessDropBeforeMove != null)
                    {
                        ProcessDropBeforeMove(this, new ProcessDropBeforeMoveEventArgs(oldIndex, newIndex));
                    }
                    itemsSource.Move(oldIndex, newIndex);
                }
                else
                    itemsSource.Insert(newIndex, data);

                // Set the Effects property so that the call to DoDragDrop will return 'Move'.
                e.Effects = DragDropEffects.Move;
            }
        }

        #endregion // listBox_Drop

        #endregion // Event Handling Methods

        #region Private Helpers

        #region CanStartDragOperation

        bool CanStartDragOperation
        {
            get
            {
                if (Mouse.LeftButton != MouseButtonState.Pressed)
                    return false;

                if (!this.canInitiateDrag)
                    return false;

                if (this.indexToSelect == -1)
                    return false;

                if (!this.HasCursorLeftDragThreshold)
                    return false;

                return true;
            }
        }

        #endregion // CanStartDragOperation

        #region FinishDragOperation

        void FinishDragOperation(ListBoxItem draggedItem, AdornerLayer adornerLayer)
        {
            // Let the ListBoxItem know that it is not being dragged anymore.
            ListViewItemDragState.SetIsBeingDragged(draggedItem, false);

            this.IsDragInProgress = false;

            if (this.ItemUnderDragCursor != null)
                this.ItemUnderDragCursor = null;

            // Remove the drag adorner from the adorner layer.
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this.dragAdorner);
                this.dragAdorner = null;
            }
        }

        #endregion // FinishDragOperation

        #region GetListViewItem

        ListBoxItem GetListBoxItem(int index)
        {
            if (this.listBox.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;

            return this.listBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
        }

        ListBoxItem GetListBoxItem(ItemType dataItem)
        {
            if (this.listBox.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;

            return this.listBox.ItemContainerGenerator.ContainerFromItem(dataItem) as ListBoxItem;
        }

        #endregion // GetListViewItem

        #region HasCursorLeftDragThreshold

        bool HasCursorLeftDragThreshold
        {
            get
            {
                if (this.indexToSelect < 0)
                    return false;

                ListBoxItem item = this.GetListBoxItem(this.indexToSelect);
                Rect bounds = VisualTreeHelper.GetDescendantBounds(item);
                Point ptInItem = this.listBox.TranslatePoint(this.ptMouseDown, item);

                // In case the cursor is at the very top or bottom of the ListBoxItem
                // we want to make the vertical threshold very small so that dragging
                // over an adjacent item does not select it.
                double topOffset = Math.Abs(ptInItem.Y);
                double btmOffset = Math.Abs(bounds.Height - ptInItem.Y);
                double vertOffset = Math.Min(topOffset, btmOffset);

                double width = SystemParameters.MinimumHorizontalDragDistance * 2;
                double height = Math.Min(SystemParameters.MinimumVerticalDragDistance, vertOffset) * 2;
                Size szThreshold = new Size(width, height);

                Rect rect = new Rect(this.ptMouseDown, szThreshold);
                rect.Offset(szThreshold.Width / -2, szThreshold.Height / -2);
                Point ptInListView = MouseUtilities.GetMousePosition(this.listBox);
                return !rect.Contains(ptInListView);
            }
        }

        #endregion // HasCursorLeftDragThreshold

        #region IndexUnderDragCursor

        /// <summary>
        /// Returns the index of the ListBoxItem underneath the
        /// drag cursor, or -1 if the cursor is not over an item.
        /// </summary>
        int IndexUnderDragCursor
        {
            get
            {
                int index = -1;
                for (int i = 0; i < this.listBox.Items.Count; ++i)
                {
                    ListBoxItem item = this.GetListBoxItem(i);
                    if (this.IsMouseOver(item))
                    {
                        index = i;
                        break;
                    }
                }
                return index;
            }
        }

        #endregion // IndexUnderDragCursor

        #region InitializeAdornerLayer
        AdornerLayer InitializeAdornerLayer(ListBoxItem itemToDrag)
        {
            // Create a brush which will paint the ListBoxItem onto
            // a visual in the adorner layer.
            VisualBrush brush = new VisualBrush(itemToDrag);

            // Create an element which displays the source item while it is dragged.
            this.dragAdorner = new DragAdorner(this.listBox, itemToDrag.RenderSize, brush);
            // Set the drag adorner's opacity.		
            this.dragAdorner.Opacity = this.DragAdornerOpacity;

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this.listBox);
            layer.Add(dragAdorner);

            // Save the location of the cursor when the left mouse button was pressed.
            this.ptMouseDown = MouseUtilities.GetMousePosition(this.listBox);

            return layer;
        }

        #endregion // InitializeAdornerLayer

        #region InitializeDragOperation

        void InitializeDragOperation(ListBoxItem itemToDrag)
        {
            // Set some flags used during the drag operation.
            this.IsDragInProgress = true;
            this.canInitiateDrag = false;

            // Let the ListBoxItem know that it is being dragged.
            ListViewItemDragState.SetIsBeingDragged(itemToDrag, true);
        }

        #endregion // InitializeDragOperation

        #region IsMouseOver

        bool IsMouseOver(Visual target)
        {
            // We need to use MouseUtilities to figure out the cursor
            // coordinates because, during a drag-drop operation, the WPF
            // mechanisms for getting the coordinates behave strangely.

            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            Point mousePos = MouseUtilities.GetMousePosition(target);
            return bounds.Contains(mousePos);
        }

        #endregion // IsMouseOver

        #region IsMouseOverScrollbar

        /// <summary>
        /// Returns true if the mouse cursor is over a scrollbar in the ListBox.
        /// </summary>
        bool IsMouseOverScrollbar
        {
            get
            {
                Point ptMouse = MouseUtilities.GetMousePosition(this.listBox);
                HitTestResult res = VisualTreeHelper.HitTest(this.listBox, ptMouse);
                if (res == null)
                    return false;

                DependencyObject depObj = res.VisualHit;
                while (depObj != null)
                {
                    if (depObj is ScrollBar)
                        return true;

                    // VisualTreeHelper works with objects of type Visual or Visual3D.
                    // If the current object is not derived from Visual or Visual3D,
                    // then use the LogicalTreeHelper to find the parent element.
                    if (depObj is Visual || depObj is System.Windows.Media.Media3D.Visual3D)
                        depObj = VisualTreeHelper.GetParent(depObj);
                    else
                        depObj = LogicalTreeHelper.GetParent(depObj);
                }

                return false;
            }
        }

        #endregion // IsMouseOverScrollbar

        #region ItemUnderDragCursor

        ItemType ItemUnderDragCursor
        {
            get
            {
                return this.itemUnderDragCursor;
            }
            set
            {
                if (this.itemUnderDragCursor == value)
                    return;

                // The first pass handles the previous item under the cursor.
                // The second pass handles the new one.
                for (int i = 0; i < 2; ++i)
                {
                    if (i == 1)
                        this.itemUnderDragCursor = value;

                    if (this.itemUnderDragCursor != null)
                    {
                        ListBoxItem listBoxItem = this.GetListBoxItem(this.itemUnderDragCursor);
                        if (listBoxItem != null)
                            ListViewItemDragState.SetIsUnderDragCursor(listBoxItem, i == 1);
                    }
                }
            }
        }

        #endregion // ItemUnderDragCursor

        #region PerformDragOperation

        void PerformDragOperation()
        {
            ItemType selectedItem = this.listBox.SelectedItem as ItemType;
            DragDropEffects allowedEffects = DragDropEffects.Move | DragDropEffects.Move | DragDropEffects.Link;
            if (DragDrop.DoDragDrop(this.listBox, selectedItem, allowedEffects) != DragDropEffects.None)
            {
                // The item was dropped into a new location,
                // so make it the new selected item.
                this.listBox.SelectedItem = selectedItem;
            }
        }

        #endregion // PerformDragOperation

        #region ShowDragAdornerResolved

        bool ShowDragAdornerResolved
        {
            get
            {
                return this.ShowDragAdorner && this.DragAdornerOpacity > 0.0;
            }
        }

        #endregion // ShowDragAdornerResolved

        #region UpdateDragAdornerLocation

        void UpdateDragAdornerLocation()
        {
            if (this.dragAdorner != null)
            {
                Point ptCursor = MouseUtilities.GetMousePosition(this.ListBox);

                // 4/13/2007 - Made the top offset relative to the item being dragged.
                ListBoxItem itemBeingDragged = this.GetListBoxItem(this.indexToSelect);
                Point itemLoc = itemBeingDragged.TranslatePoint(new Point(0, 0), this.ListBox);
                double left = itemLoc.X + ptCursor.X - this.ptMouseDown.X;
                double top = itemLoc.Y + ptCursor.Y - this.ptMouseDown.Y;

                this.dragAdorner.SetOffsets(left, top);
            }
        }

        #endregion // UpdateDragAdornerLocation

        #endregion // Private Helpers
    }

    #endregion // ListViewDragDropManager

    #region ListViewItemDragState

    /// <summary>
    /// Exposes attached properties used in conjunction with the ListViewDragDropManager class.
    /// Those properties can be used to allow triggers to modify the appearance of ListViewItems
    /// in a ListBox during a drag-drop operation.
    /// </summary>
    public static class ListViewItemDragState
    {
        #region IsBeingDragged

        /// <summary>
        /// Identifies the ListViewItemDragState's IsBeingDragged attached property.  
        /// This field is read-only.
        /// </summary>
        public static readonly DependencyProperty IsBeingDraggedProperty =
            DependencyProperty.RegisterAttached(
                "IsBeingDragged",
                typeof(bool),
                typeof(ListViewItemDragState),
                new UIPropertyMetadata(false));

        /// <summary>
        /// Returns true if the specified ListBoxItem is being dragged, else false.
        /// </summary>
        /// <param name="item">The ListBoxItem to check.</param>
        public static bool GetIsBeingDragged(ListBoxItem item)
        {
            return (bool)item.GetValue(IsBeingDraggedProperty);
        }

        /// <summary>
        /// Sets the IsBeingDragged attached property for the specified ListBoxItem.
        /// </summary>
        /// <param name="item">The ListBoxItem to set the property on.</param>
        /// <param name="value">Pass true if the element is being dragged, else false.</param>
        internal static void SetIsBeingDragged(ListBoxItem item, bool value)
        {
            item.SetValue(IsBeingDraggedProperty, value);
        }

        #endregion // IsBeingDragged

        #region IsUnderDragCursor

        /// <summary>
        /// Identifies the ListViewItemDragState's IsUnderDragCursor attached property.  
        /// This field is read-only.
        /// </summary>
        public static readonly DependencyProperty IsUnderDragCursorProperty =
            DependencyProperty.RegisterAttached(
                "IsUnderDragCursor",
                typeof(bool),
                typeof(ListViewItemDragState),
                new UIPropertyMetadata(false));

        /// <summary>
        /// Returns true if the specified ListBoxItem is currently underneath the cursor 
        /// during a drag-drop operation, else false.
        /// </summary>
        /// <param name="item">The ListBoxItem to check.</param>
        public static bool GetIsUnderDragCursor(ListBoxItem item)
        {
            return (bool)item.GetValue(IsUnderDragCursorProperty);
        }

        /// <summary>
        /// Sets the IsUnderDragCursor attached property for the specified ListBoxItem.
        /// </summary>
        /// <param name="item">The ListBoxItem to set the property on.</param>
        /// <param name="value">Pass true if the element is underneath the drag cursor, else false.</param>
        internal static void SetIsUnderDragCursor(ListBoxItem item, bool value)
        {
            item.SetValue(IsUnderDragCursorProperty, value);
        }

        #endregion // IsUnderDragCursor
    }

    #endregion // ListViewItemDragState

    #region ProcessDropEventArgs

    /// <summary>
    /// Event arguments used by the ListViewDragDropManager.ProcessDrop event.
    /// </summary>
    /// <typeparam name="ItemType">The type of data object being dropped.</typeparam>
    public class ProcessDropEventArgs<ItemType> : EventArgs where ItemType : class
    {
        #region Data

        ObservableCollection<ItemType> itemsSource;
        ItemType dataItem;
        int oldIndex;
        int newIndex;
        DragDropEffects allowedEffects = DragDropEffects.None;
        DragDropEffects effects = DragDropEffects.None;

        #endregion // Data

        #region Constructor

        internal ProcessDropEventArgs(
            ObservableCollection<ItemType> itemsSource,
            ItemType dataItem,
            int oldIndex,
            int newIndex,
            DragDropEffects allowedEffects)
        {
            this.itemsSource = itemsSource;
            this.dataItem = dataItem;
            this.oldIndex = oldIndex;
            this.newIndex = newIndex;
            this.allowedEffects = allowedEffects;
        }

        #endregion // Constructor

        #region Public Properties

        /// <summary>
        /// The items source of the ListBox where the drop occurred.
        /// </summary>
        public ObservableCollection<ItemType> ItemsSource
        {
            get
            {
                return this.itemsSource;
            }
        }

        /// <summary>
        /// The data object which was dropped.
        /// </summary>
        public ItemType DataItem
        {
            get
            {
                return this.dataItem;
            }
        }

        /// <summary>
        /// The current index of the data item being dropped, in the ItemsSource collection.
        /// </summary>
        public int OldIndex
        {
            get
            {
                return this.oldIndex;
            }
        }

        /// <summary>
        /// The target index of the data item being dropped, in the ItemsSource collection.
        /// </summary>
        public int NewIndex
        {
            get
            {
                return this.newIndex;
            }
        }

        /// <summary>
        /// The drag drop effects allowed to be performed.
        /// </summary>
        public DragDropEffects AllowedEffects
        {
            get
            {
                return allowedEffects;
            }
        }

        /// <summary>
        /// The drag drop effect(s) performed on the dropped item.
        /// </summary>
        public DragDropEffects Effects
        {
            get
            {
                return effects;
            }
            set
            {
                effects = value;
            }
        }

        #endregion // Public Properties
    }

    #endregion // ProcessDropEventArgs

    #region ProcessDropBeforeMoveEventArgs
    /// <summary>
    /// Event arguments used by the ListViewDragDropManager.ProcessDropBeforeMove event.
    /// </summary>
    /// <typeparam name="ItemType">The type of data object being dropped.</typeparam>
    public class ProcessDropBeforeMoveEventArgs : EventArgs
    {
        #region Data

        int oldIndex;
        int newIndex;

        #endregion // Data

        #region Constructor

        internal ProcessDropBeforeMoveEventArgs(
            int oldIndex,
            int newIndex)
        {
            this.oldIndex = oldIndex;
            this.newIndex = newIndex;
        }

        #endregion // Constructor

        #region Public Properties

        /// <summary>
        /// The current index of the data item being dropped, in the ItemsSource collection.
        /// </summary>
        public int OldIndex
        {
            get
            {
                return this.oldIndex;
            }
        }

        /// <summary>
        /// The target index of the data item being dropped, in the ItemsSource collection.
        /// </summary>
        public int NewIndex
        {
            get
            {
                return this.newIndex;
            }
        }

        #endregion // Public Properties
    }

    #endregion // ProcessDropBeforeMoveEventArgs

    /// <summary>
    /// 实现了属性更改通知的基类
    /// </summary>
    public class BaseNotifyPropertyChanged : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// 属性值变化时发生
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 属性值变化时发生
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = (propertyExpression.Body as MemberExpression).Member.Name;
            this.OnPropertyChanged(propertyName);
        }

        public virtual event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    }

    public class DragDropListViewData : BaseNotifyPropertyChanged
    {
        #region IsEnabled 是否可用

        private bool _isEnabled;
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(() => IsEnabled);
                }
            }
        }

        #endregion

        #region IsChecked 是否选中（列表通过此属性判断是否显示）

        private bool _isChecked;
        /// <summary>
        /// 是否选中（列表通过此属性判断是否显示）
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(() => IsChecked);
                }
            }
        }

        #endregion

        #region DisplayName 显示名称

        private string _displayName;
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    OnPropertyChanged(() => DisplayName);
                }
            }
        }

        #endregion

        #region DisplayIndex 显示的索引

        private int _displayIndex;
        /// <summary>
        /// 显示的索引
        /// </summary>
        public int DisplayIndex
        {
            get
            {
                return _displayIndex;
            }
            set
            {
                if (_displayIndex != value)
                {
                    _displayIndex = value;
                    OnPropertyChanged(() => DisplayIndex);
                }
            }
        }

        #endregion
    }
}
