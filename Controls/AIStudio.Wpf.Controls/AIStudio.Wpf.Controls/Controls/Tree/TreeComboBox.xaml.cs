using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_TreeView, Type = typeof(ExtendedTreeView))]
    [TemplatePart(Name = PART_EditableTextBox, Type = typeof(TextBox))]    
    public class TreeComboBox : ComboBox
    {
        private const string PART_ContentPresenter = "PART_ContentPresenter";
        private const string PART_TreeView = "PART_TreeView";
        private const string PART_EditableTextBox = "PART_EditableTextBox";
        private ExtendedTreeView _treeView;
        private ContentPresenter _contentPresenter;
        /// <summary>
        /// 可输入的TextBox
        /// </summary>
        private System.Windows.Controls.TextBox popupTextBox;

        public TreeComboBox()
        {
            this.DefaultStyleKey = typeof(TreeComboBox);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            //don't call the method of the base class
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _treeView = this.GetTemplateChild(PART_TreeView) as ExtendedTreeView;
            popupTextBox = this.GetTemplateChild(PART_EditableTextBox) as TextBox;
            _treeView.OnHierarchyMouseUp += OnTreeViewHierarchyMouseUp;
            _treeView.AddHandler(System.Windows.Controls.TreeViewItem.SelectedEvent, new System.Windows.RoutedEventHandler(treeview_Selected));

            _contentPresenter = this.GetTemplateChild(PART_ContentPresenter) as ContentPresenter;

            this.SetSelectedItemToHeader();
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            this.SelectedItem = _treeView.SelectedItem;
            this.SetSelectedItemToHeader();
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
            this.SetSelectedItemToHeader();
        }

        /// <summary>
        /// Handles clicks on any item in the tree view
        /// </summary>
        private void OnTreeViewHierarchyMouseUp(object sender, RoutedEventArgs e)
        {
            //This line isn't obligatory because it is executed in the OnDropDownClosed method, but be it so
            this.SelectedItem = _treeView.SelectedItem;

            this.IsDropDownOpen = false;
        }

        private void treeview_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (e.OriginalSource as TreeViewItem);
            if (item != null)
            {
                item.BringIntoView();
            }
        }

        public new IEnumerable<IBaseTreeItemViewModel> ItemsSource
        {
            get
            {
                return (IEnumerable<IBaseTreeItemViewModel>)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public static readonly new DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<IBaseTreeItemViewModel>), typeof(TreeComboBox), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TreeComboBox)sender).UpdateItemsSource();
        }

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
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(TreeComboBox), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TreeComboBox)sender).UpdateSelectedItem();
        }

        /// <summary>
        /// Selected hierarchy of the treeview
        /// </summary>
        public IEnumerable<string> SelectedHierarchy
        {
            get
            {
                return (IEnumerable<string>)GetValue(SelectedHierarchyProperty);
            }
            set
            {
                SetValue(SelectedHierarchyProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedHierarchyProperty =
            DependencyProperty.Register("SelectedHierarchy", typeof(IEnumerable<string>), typeof(TreeComboBox), new PropertyMetadata(null, OnSelectedHierarchyChanged));

        private static void OnSelectedHierarchyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TreeComboBox)sender).UpdateSelectedHierarchy();
        }

        public string DisplayPath
        {
            get
            {
                return (string)GetValue(DisplayPathProperty);
            }
            set
            {
                SetValue(DisplayPathProperty, value);
            }
        }

        public static readonly DependencyProperty DisplayPathProperty =
            DependencyProperty.Register("DisplayPath", typeof(string), typeof(TreeComboBox));

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
            DependencyProperty.Register("TextSeparator", typeof(string), typeof(TreeComboBox), new PropertyMetadata("."));

        private void UpdateItemsSource()
        {
            var allItems = new List<IBaseTreeItemViewModel>();

            Action<IEnumerable<IBaseTreeItemViewModel>> selectAllItemsRecursively = null;
            selectAllItemsRecursively = items => {
                if (items == null)
                {
                    return;
                }

                foreach (var item in items)
                {
                    allItems.Add(item);
                    selectAllItemsRecursively(item.GetChildren());
                }
            };

            selectAllItemsRecursively(this.ItemsSource);

            base.ItemsSource = allItems.Count > 0 ? allItems : null;
        }

        private void UpdateSelectedItem()
        {
            if (this.SelectedItem is TreeViewItem)
            {
                //I would rather use a correct object instead of TreeViewItem
                this.SelectedItem = ((TreeViewItem)this.SelectedItem).DataContext;
            }
            else
            {
                //Update the selected hierarchy and displays
                var model = this.SelectedItem as IBaseTreeItemViewModel;
                if (model != null)
                {
                    if (!string.IsNullOrEmpty(DisplayPath))
                    {
                        this.SelectedHierarchy = model.GetHierarchy().Select(h => h.GetType().GetProperty(DisplayPath).GetValue(h, null).ToString()).ToList();
                    }
                    else
                    {
                        this.SelectedHierarchy = model.GetHierarchy().Select(h => h.Name).ToList();
                    }
                }

                this.SetSelectedItemToHeader();

                base.SelectedItem = this.SelectedItem;
            }
        }

        private void UpdateSelectedHierarchy()
        {
            if (ItemsSource != null && this.SelectedHierarchy != null)
            {
                //Find corresponding items and expand or select them
                var source = this.ItemsSource.OfType<IBaseTreeItemViewModel>();
                var item = SelectItem(source, this.SelectedHierarchy, DisplayPath);
                this.SelectedItem = item;
            }
        }

        /// <summary>
        /// Searches the items of the hierarchy inside the items source and selects the last found item
        /// </summary>
        private static IBaseTreeItemViewModel SelectItem(IEnumerable<IBaseTreeItemViewModel> items, IEnumerable<string> selectedHierarchy, string displayMemberPath)
        {
            if (items == null || selectedHierarchy == null || !items.Any() || !selectedHierarchy.Any())
            {
                return null;
            }

            var hierarchy = selectedHierarchy.ToList();
            var currentItems = items;
            IBaseTreeItemViewModel selectedItem = null;

            for (int i = 0; i < hierarchy.Count; i++)
            {
                IBaseTreeItemViewModel currentItem = null;
                if (!string.IsNullOrEmpty(displayMemberPath))
                {
                    // get next item in the hierarchy from the collection of child items
                    currentItem = currentItems.FirstOrDefault(ci => ci.GetType().GetProperty(displayMemberPath).GetValue(ci, null).ToString() == hierarchy[i]);
                }
                else
                {
                    // get next item in the hierarchy from the collection of child items
                    currentItem = currentItems.FirstOrDefault(ci => ci.Name == hierarchy[i]);
                }


                if (currentItem == null)
                {
                    break;
                }

                selectedItem = currentItem;

                // rewrite the current collection of child items
                currentItems = selectedItem.GetChildren();
                if (currentItems == null)
                {
                    break;
                }

                // the intermediate items will be expanded
                if (i != hierarchy.Count - 1)
                {
                    selectedItem.IsExpanded = true;
                }
            }

            if (selectedItem != null)
            {
                selectedItem.IsSelected = true;
            }

            return selectedItem;
        }

        /// <summary>
        /// Gets the hierarchy of the selected tree item and displays it at the combobox header
        /// </summary>
        private void SetSelectedItemToHeader()
        {
            string content = null;

            var item = this.SelectedItem as IBaseTreeItemViewModel;
            if (item != null)
            {
                string[] hierarchy;
                //Get hierarchy and display it as the selected item
                if (!string.IsNullOrEmpty(DisplayPath))
                {
                    hierarchy = item.GetHierarchy().Select(i => i.GetType().GetProperty(DisplayPath).GetValue(i, null).ToString()).ToArray();
                }
                else
                {
                    hierarchy = item.GetHierarchy().Select(i => i.Name).ToArray();
                }
                if (hierarchy.Length > 0)
                {
                    content = string.Join(TextSeparator ?? ".", hierarchy);
                }
            }

            this.SetContentAsTextBlock(content);
        }

        /// <summary>
        /// Gets the combobox header and displays the specified content there
        /// </summary>
        private void SetContentAsTextBlock(string content)
        {
            if (_contentPresenter == null)
            {
                return;
            }

            var tb = _contentPresenter.Content as TextBlock;
            if (tb == null)
            {
                _contentPresenter.Content = tb = new TextBlock();
            }
            tb.Text = content ?? ' '.ToString();
            _contentPresenter.ContentTemplate = null;

            Text = content;

        }
    }

}
