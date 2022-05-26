using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    public class Form : ListBox
    {
        #region AttachedProperty : HeaderWidthProperty
        public static readonly DependencyProperty HeaderWidthProperty
            = DependencyProperty.RegisterAttached("HeaderWidth", typeof(GridLength), typeof(Form), new FrameworkPropertyMetadata(new GridLength(50d, GridUnitType.Pixel), FrameworkPropertyMetadataOptions.Inherits));

        public static GridLength GetHeaderWidth(DependencyObject element) => (GridLength)element.GetValue(HeaderWidthProperty);
        public static void SetHeaderWidth(DependencyObject element, GridLength value) => element.SetValue(HeaderWidthProperty, value);
        #endregion

        #region AttachedProperty : BodyWidthProperty
        public static readonly DependencyProperty BodyWidthProperty
            = DependencyProperty.RegisterAttached("BodyWidth", typeof(GridLength), typeof(Form), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), FrameworkPropertyMetadataOptions.Inherits));

        public static GridLength GetBodyWidth(DependencyObject element) => (GridLength)element.GetValue(BodyWidthProperty);
        public static void SetBodyWidth(DependencyObject element, GridLength value) => element.SetValue(BodyWidthProperty, value);
        #endregion

        #region AttachedProperty : OrientationProperty
        public static readonly DependencyProperty OrientationProperty
            = DependencyProperty.RegisterAttached("Orientation", typeof(Orientation), typeof(Form), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.Inherits));

        public static Orientation GetOrientation(DependencyObject element) => (Orientation)element.GetValue(OrientationProperty);
        public static void SetOrientation(DependencyObject element, Orientation value) => element.SetValue(OrientationProperty, value);
        #endregion

        #region AttachedProperty: ItemMarginProperty
        public static readonly DependencyProperty ItemMarginProperty
            = DependencyProperty.RegisterAttached("ItemMargin", typeof(Thickness), typeof(Form), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.Inherits));
        public static Thickness GetItemMargin(DependencyObject element) => (Thickness)element.GetValue(ItemMarginProperty);
        public static void SetItemMargin(DependencyObject element, Thickness value) => element.SetValue(ItemMarginProperty, value);
        #endregion     

        #region AttachedProperty : HeaderMarginProperty
        public static readonly DependencyProperty HeaderMarginProperty
            = DependencyProperty.RegisterAttached("HeaderMargin", typeof(Thickness), typeof(Form), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.Inherits));

        public static Thickness GetHeaderMargin(DependencyObject element) => (Thickness)element.GetValue(HeaderMarginProperty);
        public static void SetHeaderMargin(DependencyObject element, Thickness value) => element.SetValue(HeaderMarginProperty, value);
        #endregion

        #region AttachedProperty : BodyMarginProperty
        public static readonly DependencyProperty BodyMarginProperty
            = DependencyProperty.RegisterAttached("BodyMargin", typeof(Thickness), typeof(Form), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.Inherits));

        public static Thickness GetBodyMargin(DependencyObject element) => (Thickness)element.GetValue(BodyMarginProperty);
        public static void SetBodyMargin(DependencyObject element, Thickness value) => element.SetValue(BodyMarginProperty, value);
        #endregion

        public bool IsReadOnly
        {
            get
            {
                return (bool)GetValue(IsReadOnlyProperty);
            }
            set
            {
                SetValue(IsReadOnlyProperty, value);
            }
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(Form));

        public bool IsCodeMode
        {
            get
            {
                return (bool)GetValue(IsCodeModeProperty);
            }
            set
            {
                SetValue(IsCodeModeProperty, value);
            }
        }

        public static readonly DependencyProperty IsCodeModeProperty =
            DependencyProperty.Register("IsCodeMode", typeof(bool), typeof(Form));

        public Form()
        {
            this.Drop += Form_Drop;
            this.DragOver += Form_DragOver;
            this.MouseLeftButtonDown += Form_MouseLeftButtonDown;
        }

        private void Form_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);
            HitTestResult result = VisualTreeHelper.HitTest(this, pos);
            if (result == null)
            {
                return;
            }
            var dragItem = VisualHelper.FindParent<FormItem>(result.VisualHit);
            if (dragItem == null)
            {
                return;
            }

            SelectedItem = dragItem;
            DragDrop.DoDragDrop(this, dragItem, DragDropEffects.Move);
        }

        private void Form_DragOver(object sender, DragEventArgs e)
        {
            if (IsReadOnly)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.Link;
            }
        }

        private void Form_Drop(object sender, DragEventArgs e)
        {
            if (IsReadOnly)
                return;

            var pos = e.GetPosition(this);
            var result = VisualTreeHelper.HitTest(this, pos);
            if (result == null)
            {
                return;
            }

            //查找元数据
            var sourceItem = (e.Data.GetData(typeof(FormItem)) ?? e.Data.GetData(typeof(FormCodeItem))) as FormItem;
            if (sourceItem == null)
            {
                return;
            }

            //查找目标数据
            var targetItem = VisualHelper.FindParent<FormItem>(result.VisualHit);
            if (sourceItem == targetItem)
            {
                return;
            }

            if (targetItem == null)
            {
                AddItem(sourceItem);
            }
            else
            {
                ChangedItem(sourceItem, targetItem);
            }

            this.Items.Refresh();
        }

        private void ChangedItem(FormItem sourceItem, FormItem targetItem)
        {
            bool isItemsSource;
            var list = GetActualList(out isItemsSource);

            var target = isItemsSource ? targetItem.DataContext : targetItem;
            var source = isItemsSource ? sourceItem.DataContext : sourceItem;

            int indexTarget = list.IndexOf(target);

            // If no valid cell index is obtained, add the child to the end of the 
            // fluidElements list.
            if ((indexTarget == -1) || (indexTarget >= Items.Count))
            {
                indexTarget = Items.Count - 1;
            }

            list.Remove(source);
            list.Insert(indexTarget, source);
        }

        private void AddItem(FormItem sourceItem)
        {
            bool isItemsSource;
            var list = GetActualList(out isItemsSource);
            var source = isItemsSource ? sourceItem.DataContext : sourceItem;

            list.Remove(source);
            list.Add(source);
        }

        internal IList GetActualList(out bool isItemsSource)
        {
            IList list;
            if (ItemsSource != null)
            {
                isItemsSource = true;
                list = ItemsSource as IList;
            }
            else
            {
                isItemsSource = false;
                list = Items;
            }

            return list;
        }


        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            if (IsCodeMode)
            {
                return item is FormCodeItem;
            }
            else
            {
                return item is FormItem;
            }            
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            if (IsCodeMode)
            {
                return new FormCodeItem();
            }
            else
            {
                return new FormItem();
            }
        }
    }
}
