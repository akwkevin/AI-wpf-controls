using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// TreeSelect.xaml 的交互逻辑
    /// </summary>
    [TemplatePart(Name = "PART_TreeView", Type = typeof(TreeView))]
    public partial class TreeSelect : ComboBox
    {
        public bool IsMulti
        {
            get
            {
                return (bool)GetValue(IsMultiProperty);
            }
            set
            {
                SetValue(IsMultiProperty, value);
            }
        }

        public static readonly DependencyProperty IsMultiProperty =
            DependencyProperty.Register("IsMulti", typeof(bool), typeof(TreeSelect), new PropertyMetadata(false));

        public IList SelectedItems
        {
            get
            {
                return (IList)GetValue(SelectedItemsProperty);
            }
            set
            {
                SetValue(SelectedItemsProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList), typeof(TreeSelect), new PropertyMetadata(OnSelectedItemsChanged));

        private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TreeSelect)sender).UpdateSelectedItems(e.NewValue as IList, e.OldValue as IList);
        }

        public new string DisplayMemberPath
        {
            get
            {
                return (string)GetValue(DisplayMemberPathProperty);
            }
            set
            {
                SetValue(DisplayMemberPathProperty, value);
            }
        }

        public new static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(TreeSelect));

        public new string SelectedValuePath
        {
            get
            {
                return (string)GetValue(SelectedValuePathProperty);
            }
            set
            {
                SetValue(SelectedValuePathProperty, value);
            }
        }

        public new static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(TreeSelect));

        /// <summary>
        /// Selected item of the TreeView
        /// </summary>
        public new object SelectedItem
        {
            get
            {
                return (object)GetValue(SelectedItemProperty);
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        public new static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TreeSelect), new PropertyMetadata(null, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TreeSelect)sender).UpdateSelectedItem();
        }

        public new object SelectedValue
        {
            get
            {
                return (object)GetValue(SelectedValueProperty);
            }
            set
            {
                SetValue(SelectedValueProperty, value);
            }
        }

        public new static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(TreeSelect), new PropertyMetadata(null, OnSelectedValueChanged));

        private static void OnSelectedValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TreeSelect)sender).UpdateSelectedValue();
        }

        public new string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public static new readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TreeSelect));

        /// <summary>
        /// Gets or sets text separator.
        /// </summary>
        public string TextSeparator
        {
            get
            {
                return (string)GetValue(TextSeparatorProperty);
            }
            set
            {
                SetValue(TextSeparatorProperty, value);
            }
        }

        public static readonly DependencyProperty TextSeparatorProperty =
            DependencyProperty.Register("TextSeparator", typeof(string), typeof(TreeSelect), new PropertyMetadata(","));

        static TreeSelect()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeSelect), new FrameworkPropertyMetadata(typeof(TreeSelect)));
        }

        public TreeSelect()
        {
            Loaded -= TreeSelect_Loaded;
            Loaded += TreeSelect_Loaded;
        }

        private ExtendedTreeView _treeView;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._treeView = Template.FindName("PART_TreeView", this) as ExtendedTreeView;
            if (this._treeView != null)
            {
                //this._treeView.SelectedItemChanged += _TreeView_SelectedItemChanged;
                _treeView.OnHierarchyMouseUp += OnTreeViewHierarchyMouseUp;
                _treeView.AddHandler(System.Windows.Controls.TreeViewItem.SelectedEvent, new System.Windows.RoutedEventHandler(treeview_Selected));
            }
        }

        //protected override void OnDropDownClosed(EventArgs e)
        //{
        //    base.OnDropDownClosed(e);
        //    this.SelectedItem = _treeView.SelectedItem;
        //    this.UpdateText();
        //}

        //protected override void OnDropDownOpened(EventArgs e)
        //{
        //    base.OnDropDownOpened(e);
        //    this.UpdateText();
        //}

        private bool _interChanged = false;
        /// <summary>
        /// Handles clicks on any item in the tree view
        /// </summary>
        private void OnTreeViewHierarchyMouseUp(object sender, RoutedEventArgs e)
        {
            if (IsMulti)
            {
                _interChanged = true;

                if (SelectedItems != null)
                {
                    SelectedItems.Clear();
                    foreach (var item in GenerateMultiObject(Items))
                    {
                        SelectedItems.Add(item);
                    }
                }
                else
                {
                    SelectedItems = new ObservableCollection<object>(GenerateMultiObject(Items));
                }
                _interChanged = false;
            }
            else
            {
                //This line isn't obligatory because it is executed in the OnDropDownClosed method, but be it so
                this.SelectedItem = _treeView.SelectedItem;
                base.SelectedItem = this.SelectedItem;
                this.IsDropDownOpen = false;
            }
            this.UpdateText();
        }

        private void treeview_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (e.OriginalSource as TreeViewItem);
            if (item != null)
            {
                item.BringIntoView();
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ComboBoxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            if (item is ComboBoxItem)
                return true;
            else
                return false;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var uie = element as FrameworkElement;

            if (!(item is ComboBoxItem))
            {
                var textBinding = new Binding(DisplayMemberPath);
                textBinding.Source = item;
                uie.SetBinding(ContentPresenter.ContentProperty, textBinding);
            }

            base.PrepareContainerForItemOverride(element, item);
        }

        private void TreeSelect_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateText();
        }

        private void UpdateSelectedValue()
        {
            if (_interSelectedValueChanged == true) return;

            SelectedItem = GetTreeModel(Items, SelectedValue);
        }

        private bool _interSelectedValueChanged;
        private void UpdateSelectedItem()
        {
            _interSelectedValueChanged = true;
            if (this.SelectedItem == null || string.IsNullOrEmpty(this.SelectedValuePath))
            {
                SelectedValue = null;
            }
            else
            {
                SelectedValue = this.SelectedItem.GetType().GetProperty(SelectedValuePath).GetValue(this.SelectedItem, null);
            }
            _interSelectedValueChanged = false;
            base.SelectedItem = this.SelectedItem;

            UpdateText();
        }

        private void UpdateSelectedItems(IList newitem, IList olditem)
        {
            if (olditem != null)
            {
                foreach (var item in olditem)
                {
                    if (item.GetType().GetProperty("IsChecked") != null)
                    {
                        item.GetType().GetProperty("IsChecked").SetValue(item, false);
                    }
                }

                ((INotifyCollectionChanged)olditem).CollectionChanged -= TreeSelect_CollectionChanged;
            }

            if (newitem != null)
            {
                foreach (var item in newitem)
                {
                    if (item.GetType().GetProperty("IsChecked") != null)
                    {
                        item.GetType().GetProperty("IsChecked").SetValue(item, true);
                    }
                }

                ((INotifyCollectionChanged)newitem).CollectionChanged += TreeSelect_CollectionChanged;
            }

            UpdateText();
        }

        //主要是外部改变的时候更新数据
        private void TreeSelect_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_interChanged) return;

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item.GetType().GetProperty("IsCheckedOnlySelf") != null)
                    {
                        item.GetType().GetProperty("IsCheckedOnlySelf").SetValue(item, true);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item.GetType().GetProperty("IsCheckedOnlySelf") != null)
                    {
                        item.GetType().GetProperty("IsCheckedOnlySelf").SetValue(item, false);
                    }
                }
            }

            UpdateText();
        }

        private void _TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateText();
            base.SelectedItem = this.SelectedItem;
        }

        private void UpdateText()
        {
            if (!IsLoaded)
                return;

            if (IsMulti == false)
            {
                Text = GenerateText(SelectedItem);
            }
            else
            {
                Text = GenerateMultiText();
            }
        }

        public string GenerateText(object selectedItem)
        {
            var text = "";
            if (selectedItem == null)
            {
                text = "";
            }
            else if (selectedItem is ComboBoxItem)
            {
                var msi = selectedItem as ComboBoxItem;
                text += msi.Content.ToString();
            }
            else
            {
                if (!string.IsNullOrEmpty(DisplayMemberPath) && selectedItem.GetType().GetProperty(DisplayMemberPath) != null)
                    text += selectedItem.GetType().GetProperty(DisplayMemberPath).GetValue(selectedItem, null).ToString();
                else
                    text += selectedItem.ToString();

                if (selectedItem.GetType().GetProperty("IsSelected") != null)
                {
                    selectedItem.GetType().GetProperty("IsSelected").SetValue(selectedItem, true);
                }
            }

            return text;
        }

        private string GenerateMultiText()
        {
            var text = "";
            var isFirst = true;

            if (SelectedItems != null)
            {
                foreach (var item in SelectedItems)
                {
                    string txt = null;
                    if (item is ComboBoxItem)//这个还未支持多选，按单选处理
                    {
                        var msi = item as ComboBoxItem;
                        txt = msi.Content.ToString();
                    }
                    else
                    {
                        if (item.GetType().GetProperty("IsChecked") != null && item.GetType().GetProperty("IsChecked").GetValue(item, null).ToString() == "True")
                        {
                            if (!string.IsNullOrEmpty(DisplayMemberPath) && item.GetType().GetProperty(DisplayMemberPath) != null)
                                txt = item.GetType().GetProperty(DisplayMemberPath).GetValue(item, null).ToString();
                            else
                                txt = item.ToString();
                        }
                    }

                    if (!isFirst)
                        text += TextSeparator;
                    else
                        isFirst = false;

                    text += txt;                  
                }
            }
            return text;
        }

        private List<object> GenerateMultiObject(System.Collections.IEnumerable items)
        {
            List<object> objs = new List<object>();
            if (items != null)
            {
                foreach (var item in items)
                {
                    object obj = null;
                    if (item is ComboBoxItem)//这个还未支持多选，按单选处理
                    {
                        var msi = item as ComboBoxItem;
                        if (msi.IsSelected)
                        {
                            obj = item;
                        }
                    }
                    else
                    {
                        if (item.GetType().GetProperty("IsChecked") != null && item.GetType().GetProperty("IsChecked").GetValue(item, null).ToString() == "True")
                        {
                            obj = item;
                        }
                    }

                    if (obj != null)
                        objs.Add(obj);

                    if (item.GetType().GetProperty("Children") != null)
                    {
                        objs.AddRange(GenerateMultiObject(item.GetType().GetProperty("Children").GetValue(item, null) as System.Collections.IEnumerable));
                    }
                }
            }
            return objs;
        }

        public object GetTreeModel(IList trees, object id)
        {
            object treemodel = null;
            if (trees != null && id != null)
            {
                foreach (var tree in trees)
                {
                    if (tree.GetType().GetProperty(SelectedValuePath).GetValue(tree)?.ToString() == id?.ToString())
                    {
                        treemodel = tree;
                        break;
                    }
                    else
                    {
                        var children = tree.GetType().GetProperty("Children").GetValue(tree) as IList;
                        if (children != null)
                        {
                            treemodel = GetTreeModel(children, id);
                            if (treemodel != null)
                                break;
                        }                        
                    }
                }
            }
            return treemodel;
        }
    }
}
