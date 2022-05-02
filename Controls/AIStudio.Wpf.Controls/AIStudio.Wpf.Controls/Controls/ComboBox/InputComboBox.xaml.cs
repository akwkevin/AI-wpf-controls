using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_EditableTextBox2, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    [TemplatePart(Name = PART_DropDownToggle, Type = typeof(ToggleButton))]
    public class InputComboBox : ComboBox
    {
        public const string PART_EditableTextBox2 = "PART_EditableTextBox2";
        public const string PART_Popup = "PART_Popup";
        public const string PART_DropDownToggle = "PART_DropDownToggle";

        internal TextBox _textBox;
        internal Popup _popup;
        internal ToggleButton _toggleButton;
        private ObservableCollection<object> bindingList = new ObservableCollection<object>();//数据源绑定List

        /// <summary>
        /// 注册依赖事件
        /// </summary>
        public new static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(InputComboBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(ItemsSourceChanged)));
        /// <summary>
        /// 数据源改变，添加数据源到绑定数据源
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InputComboBox ecb = d as InputComboBox;
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


        public static new readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(InputComboBox), new PropertyMetadata("", (p, d) => {
            InputComboBox inputComboBox = p as InputComboBox;
            if (inputComboBox._textBox != null && (string)d.NewValue != inputComboBox._textBox.Text)
            {
                inputComboBox._textBox.Text = (string)d.NewValue;
            }
        }));
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


        public static readonly DependencyProperty NullShowAllProperty = DependencyProperty.Register("NullShowAll", typeof(bool), typeof(InputComboBox), new PropertyMetadata(false));
        public bool NullShowAll
        {
            get
            {
                return (bool)GetValue(NullShowAllProperty);
            }
            set
            {
                SetValue(NullShowAllProperty, value);
            }
        }

        public static readonly DependencyProperty TextFilterProperty = DependencyProperty.Register("TextFilter", typeof(bool), typeof(InputComboBox), new PropertyMetadata(true));
        public bool TextFilter
        {
            get
            {
                return (bool)GetValue(TextFilterProperty);
            }
            set
            {
                SetValue(TextFilterProperty, value);
            }
        }

        public static readonly DependencyProperty ItemFilterProperty = DependencyProperty.Register("ItemFilter", typeof(Func<object, string, bool>), typeof(InputComboBox), new PropertyMetadata(null));
        public Func<object, string, bool> ItemFilter
        {
            get
            {
                return (Func<object, string, bool>)GetValue(ItemFilterProperty);
            }
            set
            {
                SetValue(ItemFilterProperty, value);
            }
        }

        public static readonly DependencyProperty IsFocusTextProperty = DependencyProperty.Register("IsFocusText", typeof(bool), typeof(InputComboBox), new PropertyMetadata(true, (p, d) => {
            InputComboBox inputComboBox = p as InputComboBox;
            if ((bool)d.NewValue)
            {
                Keyboard.Focus(inputComboBox._textBox);
            }
        }));
        public bool IsFocusText
        {
            get
            {
                return (bool)GetValue(IsFocusTextProperty);
            }
            set
            {
                SetValue(IsFocusTextProperty, value);
            }
        }

        #region Routed Event
        public static readonly RoutedEvent ResultChangedEvent = EventManager.RegisterRoutedEvent("ResultChanged", RoutingStrategy.Bubble, typeof(EventHandler), typeof(InputComboBox));
        public event EventHandler ResultChanged
        {
            add
            {
                AddHandler(ResultChangedEvent, value);
            }
            remove
            {
                RemoveHandler(ResultChangedEvent, value);
            }
        }
        void RaiseResultChanged(object result)
        {
            var arg = new RoutedEventArgs(ResultChangedEvent, result);
            RaiseEvent(arg);
        }
        #endregion

        private ICollectionView _view;
        private PropertyInfo _displayMemberPropertyInfo;

        static InputComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InputComboBox), new FrameworkPropertyMetadata(typeof(InputComboBox)));
        }

        public InputComboBox()
        {
            Loaded += InputComboBox_Loaded;
        }

        private void InputComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            SetView();
        }

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

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            base.OnSelectionChanged(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Enter)
            {
                IsDropDownOpen = false;
                if (SelectedItem != null)
                {
                    if (SelectedItem is string obj)
                    {
                        _textBox.Text = obj;
                    }
                    else
                    {
                        if (_displayMemberPropertyInfo == null || _displayMemberPropertyInfo.Name != DisplayMemberPath)
                            _displayMemberPropertyInfo = SelectedItem.GetType().GetProperty(DisplayMemberPath);

                        if (_displayMemberPropertyInfo != null)
                        {
                            var text = _displayMemberPropertyInfo.GetValue(SelectedItem, null)?.ToString();
                            _textBox.Text = text;
                        }
                    }
                    _textBox.SelectionStart = _textBox.Text.Length;

                    RaiseResultChanged(SelectedItem);
                }
            }
        }

        private void SetView()
        {
            if (base.ItemsSource != null)
            {
                _view = CollectionViewSource.GetDefaultView(base.ItemsSource);
                _view.Filter = (o) => {
                    if (string.IsNullOrEmpty(_textBox?.Text))
                    {
                        return NullShowAll;
                    }

                    if (ItemFilter != null)
                    {
                        return ItemFilter(o, _textBox?.Text);
                    }
                    else if (TextFilter)
                    {
                        if (o is string obj)
                        {
                            return obj.ToLower().Contains(_textBox?.Text.ToLower());
                        }
                        else
                        {
                            if (_displayMemberPropertyInfo == null || _displayMemberPropertyInfo.Name != DisplayMemberPath)
                                _displayMemberPropertyInfo = o.GetType().GetProperty(DisplayMemberPath);

                            if (_displayMemberPropertyInfo != null)
                            {
                                var text = _displayMemberPropertyInfo.GetValue(o, null)?.ToString();
                                return text.ToLower().Contains(_textBox?.Text.ToLower());
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                };
            }
        }

        //private 

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBox = GetTemplateChild(PART_EditableTextBox2) as TextBox;
            _popup = GetTemplateChild(PART_Popup) as Popup;
            _toggleButton = GetTemplateChild(PART_DropDownToggle) as ToggleButton;
            _textBox.PreviewKeyDown += _textBox_PreviewKeyDown;
            _textBox.TextChanged += _textBox_TextChanged;
            _textBox.GotKeyboardFocus += _textBox_GotKeyboardFocus;
            _textBox.LostTouchCapture += _textBox_LostTouchCapture;
        }

        private void _textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = _textBox.Text;

            if (TextFilter == true || ItemFilter != null)
            {
                _view.Refresh();
                if (SelectedIndex == -1)
                {
                    SelectedIndex = 0;
                }
            }

            if (FindFirst(_view) != null)
                IsDropDownOpen = true;
        }

        private void _textBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (FindFirst(_view) != null)
                IsDropDownOpen = true;
            //_popup.StaysOpen = true;
        }

        private void _textBox_LostTouchCapture(object sender, TouchEventArgs e)
        {
            //_popup.StaysOpen = false;
        }

        private void _textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Down)
            {
                SelectedIndex++;
            }
            else if (e.Key == Key.Up)
            {
                if (SelectedIndex > 0)
                    SelectedIndex--;
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);
        }

        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnPreviewGotKeyboardFocus(e);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            if (FindFirst(_view) != null)
                IsDropDownOpen = true;

            DependencyObject dep = (DependencyObject)e.OriginalSource;

            var comboBoxItem = VisualHelper.TryFindParent<ComboBoxItem>(dep);
            if (comboBoxItem != null && comboBoxItem.DataContext != null)
            {
                if (comboBoxItem.DataContext is string obj)
                {
                    _textBox.Text = obj;
                }
                else
                {
                    if (_displayMemberPropertyInfo == null || _displayMemberPropertyInfo.Name != DisplayMemberPath)
                        _displayMemberPropertyInfo = comboBoxItem.DataContext.GetType().GetProperty(DisplayMemberPath);

                    if (_displayMemberPropertyInfo != null)
                    {
                        var text = _displayMemberPropertyInfo.GetValue(comboBoxItem.DataContext, null)?.ToString();
                        _textBox.Text = text;
                    }
                }
                _textBox.SelectionStart = _textBox.Text.Length;

                RaiseResultChanged(comboBoxItem.DataContext);
            }
        }

        public static T FindFirst<T>(IEnumerable<T> source) where T : class
        {
            T current;
            T t;
            if (source != null)
            {
                IEnumerator<T> enumerator = source.GetEnumerator();
                try
                {
                    if (enumerator.MoveNext())
                    {
                        current = enumerator.Current;
                        return current;
                    }
                }
                finally
                {
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                t = default(T);
                current = t;
            }
            else
            {
                t = default(T);
                current = t;
            }
            return current;
        }

        public static object FindFirst(System.Collections.IEnumerable source)
        {
            object current;
            if (source != null)
            {
                System.Collections.IEnumerator enumerator = source.GetEnumerator();
                try
                {
                    if (enumerator.MoveNext())
                    {
                        current = enumerator.Current;
                        return current;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                current = null;
            }
            else
            {
                current = null;
            }
            return current;
        }


    }
}
