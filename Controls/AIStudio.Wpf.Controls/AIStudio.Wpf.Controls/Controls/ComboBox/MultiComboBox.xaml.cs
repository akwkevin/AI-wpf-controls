using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class MultiComboBox : MultiSelector
    {
        #region Identifier
        private PropertyInfo _displayMemberPropertyInfo;
        #endregion

        private ObservableCollection<object> bindingList = new ObservableCollection<object>();//数据源绑定List
        private ICollectionView _view;

        /// <summary>
        /// 注册依赖事件
        /// </summary>
        public new static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(MultiComboBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(ItemsSourceChanged)));
        /// <summary>
        /// 数据源改变，添加数据源到绑定数据源
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiComboBox ecb = d as MultiComboBox;
            ecb.bindingList.Clear();
            //遍历循环操作
            foreach (var item in ecb.ItemsSource)
            {
                ecb.bindingList.Add(item);
            }
        }
        /// <summary>
        /// 设置或获取ComboBox的数据源
        /// </summary>
        public new IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public new IList SelectedItems
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
            DependencyProperty.Register("SelectedItems", typeof(IList), typeof(MultiComboBox), new PropertyMetadata(OnSelectedItemsChanged));

        private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //同步到基类的SelectedItems
            var listBox = sender as MultiSelector;
            if (listBox != null)
            {
                if (e.OldValue != null) { listBox.SelectionChanged -= OnlistBoxSelectionChanged; }
                IList collection = e.NewValue as IList;
                listBox.SelectedItems.Clear();
                if (collection != null)
                {
                    foreach (var item in collection)
                    {
                        listBox.SelectedItems.Add(item);
                    }
                    listBox.SelectionChanged += OnlistBoxSelectionChanged;
                }
            }
        }

        static void OnlistBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //更新到重写的SelectedItems
            var listBox = sender as MultiComboBox;          //添加用户选中的当前项.      
            var dataSource = listBox.SelectedItems;
            foreach (var item in e.AddedItems)
            {
                dataSource.Add(item);
            }
            //删除用户取消选中的当前项            
            foreach (var item in e.RemovedItems)
            {
                dataSource.Remove(item);
            }
        }

        #region Constructor
        static MultiComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiComboBox), new FrameworkPropertyMetadata(typeof(MultiComboBox)));
        }

        public MultiComboBox()
        {
            Loaded -= MultiComboBox_Loaded;
            Loaded += MultiComboBox_Loaded;
        }

        #endregion
        /// <summary>
        /// 重写初始化
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            base.ItemsSource = bindingList;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            SetView();
            _view.Refresh();
        }

        private void SetView()
        {
            if (base.ItemsSource != null)
            {
                _view = CollectionViewSource.GetDefaultView(base.ItemsSource);
                _view.Filter = (o) => {
                    if (string.IsNullOrEmpty(SearchText))
                    {
                        return true;
                    }

                    if (o is string obj)
                    {
                        return obj.ToLower().Contains(SearchText.ToLower());
                    }
                    else
                    {
                        if (_displayMemberPropertyInfo == null || _displayMemberPropertyInfo.Name != DisplayMemberPath)
                            _displayMemberPropertyInfo = o.GetType().GetProperty(DisplayMemberPath);

                        if (_displayMemberPropertyInfo != null)
                        {
                            var text = _displayMemberPropertyInfo.GetValue(o, null)?.ToString();
                            return text.ToLower().Contains(SearchText.ToLower());
                        }
                        else
                        {
                            return false;
                        }
                    }
                };
            }
        }

        public virtual string GenerateText(IList selectedItems)
        {
            var text = "";
            var isFirst = true;

            foreach (var item in selectedItems)
            {
                if (!isFirst)
                    text += TextSeparator;
                else
                    isFirst = false;

                if (item is ListBoxItem)
                {
                    var msi = item as ListBoxItem;
                    text += msi.Content.ToString();
                }
                else
                {
                    if (_displayMemberPropertyInfo == null || _displayMemberPropertyInfo.Name != DisplayMemberPath)
                        _displayMemberPropertyInfo = item.GetType().GetProperty(DisplayMemberPath);

                    if (_displayMemberPropertyInfo != null)
                        text += _displayMemberPropertyInfo.GetValue(item, null).ToString();
                    else
                        text += item.ToString();
                }
            }
            return text;
        }


        #region Property

        /// <summary>
        /// Gets or sets max drop down height.
        /// </summary>
        public double MaxDropDownHeight
        {
            get
            {
                return (double)GetValue(MaxDropDownHeightProperty);
            }
            set
            {
                SetValue(MaxDropDownHeightProperty, value);
            }
        }
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(MultiComboBox));

        /// <summary>
        /// Gets or sets is drop down open.
        /// </summary>
        public bool IsDropDownOpen
        {
            get
            {
                return (bool)GetValue(IsDropDownOpenProperty);
            }
            set
            {
                SetValue(IsDropDownOpenProperty, value);
            }
        }
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(MultiComboBox));

        /// <summary>
        /// Gets or sets stays open.
        /// </summary>
        public bool StaysOpen
        {
            get
            {
                return (bool)GetValue(StaysOpenProperty);
            }
            set
            {
                SetValue(StaysOpenProperty, value);
            }
        }

        public static readonly DependencyProperty StaysOpenProperty =
            DependencyProperty.Register("StaysOpen", typeof(bool), typeof(MultiComboBox));

        /// <summary>
        /// Gets or sets text.
        /// </summary>
        public string Text
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
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MultiComboBox));

        /// <summary>
        /// Gets or sets checkbox style.
        /// </summary>
        public Style CheckBoxStyle
        {
            get
            {
                return (Style)GetValue(CheckBoxStyleProperty);
            }
            set
            {
                SetValue(CheckBoxStyleProperty, value);
            }
        }
        public static readonly DependencyProperty CheckBoxStyleProperty =
            DependencyProperty.Register(nameof(CheckBoxStyle), typeof(Style), typeof(MultiComboBox));

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
            DependencyProperty.Register(nameof(TextSeparator), typeof(string), typeof(MultiComboBox), new PropertyMetadata(","));


        /// <summary>
        /// Gets or sets is search textbox visible.
        /// </summary>
        public bool IsSearchTextBoxVisible
        {
            get
            {
                return (bool)GetValue(IsSearchTextBoxVisibleProperty);
            }
            set
            {
                SetValue(IsSearchTextBoxVisibleProperty, value);
            }
        }

        public static readonly DependencyProperty IsSearchTextBoxVisibleProperty =
            DependencyProperty.Register("IsSearchTextBoxVisible", typeof(bool), typeof(MultiComboBox));


        /// <summary>
        /// Gets or sets search text.
        /// </summary>
        public string SearchText
        {
            get
            {
                return (string)GetValue(SearchTextProperty);
            }
            set
            {
                SetValue(SearchTextProperty, value);
            }
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(MultiComboBox), new PropertyMetadata(OnSearchTextChanged));

        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = d as MultiComboBox;
            selector.SearchTextChanged();
        }

        private void SearchTextChanged()
        {
            _view.Refresh();
        }

        /// <summary>
        /// Gets or sets search text box watermark.
        /// </summary>
        public string SearchTextBoxWatermark
        {
            get
            {
                return (string)GetValue(SearchTextBoxWatermarkProperty);
            }
            set
            {
                SetValue(SearchTextBoxWatermarkProperty, value);
            }
        }

        public static readonly DependencyProperty SearchTextBoxWatermarkProperty =
            DependencyProperty.Register("SearchTextBoxWatermark", typeof(string), typeof(MultiComboBox));

        /// <summary>
        /// Gets or sets is search textbox visible.
        /// </summary>
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
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(MultiComboBox));
        #endregion


        #region EventHandler
        private void MultiComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateText();
            SetView();
        }
        #endregion

        #region Override
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ListBoxItem();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            UpdateText();
            base.OnSelectionChanged(e);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            if (item is ListBoxItem)
                return true;
            else
                return false;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var uie = element as FrameworkElement;

            if (!(item is ListBoxItem))
            {
                var textBinding = new Binding(DisplayMemberPath);
                textBinding.Source = item;
                uie.SetBinding(ContentPresenter.ContentProperty, textBinding);
            }

            base.PrepareContainerForItemOverride(element, item);
        }

        #endregion

        #region Function
        private void UpdateText()
        {
            Text = GenerateText(base.SelectedItems);
        }
        #endregion      
    }

}
