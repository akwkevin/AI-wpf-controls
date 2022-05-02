using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Event;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_OverflowButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = PART_HeaderPanel, Type = typeof(TabPanel))]
    [TemplatePart(Name = PART_OverflowScrollviewer, Type = typeof(ScrollViewer))]
    public class TabControlEx : System.Windows.Controls.TabControl
    {
        private const string PART_OverflowButton = "PART_OverflowButton";
        private const string PART_HeaderPanel = "PART_HeaderPanel";
        private const string PART_OverflowScrollviewer = "PART_OverflowScrollviewer";

        private ToggleButton _buttonOverflow;
        internal TabPanel HeaderPanel
        {
            get; private set;
        }
        private ScrollViewer _scrollViewerOverflow;

        /// <summary>
        ///     是否为内部操作
        /// </summary>
        internal bool IsInternalAction;


        /// <summary>
        ///     是否可以拖动
        /// </summary>
        public static readonly DependencyProperty IsDraggableProperty = DependencyProperty.Register(
            nameof(IsDraggable), typeof(bool), typeof(TabControlEx), new PropertyMetadata(false));

        /// <summary>
        ///     是否可以拖动
        /// </summary>
        public bool IsDraggable
        {
            get => (bool)GetValue(IsDraggableProperty);
            set => SetValue(IsDraggableProperty, value);
        }

        /// <summary>
        ///     是否显示关闭按钮
        /// </summary>
        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.RegisterAttached(
            nameof(ShowCloseButton), typeof(bool), typeof(TabControlEx), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetShowCloseButton(DependencyObject element, bool value)
            => element.SetValue(ShowCloseButtonProperty, value);

        public static bool GetShowCloseButton(DependencyObject element)
            => (bool)element.GetValue(ShowCloseButtonProperty);

        /// <summary>
        ///     是否显示关闭按钮
        /// </summary>
        public bool ShowCloseButton
        {
            get => (bool)GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, value);
        }

        /// <summary>
        ///     TabHeader显示类型
        /// </summary>
        public static readonly DependencyProperty ControlStyleProperty = DependencyProperty.RegisterAttached(
            nameof(ControlStyle), typeof(TabControlStyle), typeof(TabControlEx), new FrameworkPropertyMetadata(TabControlStyle.Card, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetControlStyle(DependencyObject element, TabControlStyle value)
            => element.SetValue(ControlStyleProperty, value);

        public static TabControlStyle GetControlStyle(DependencyObject element)
            => (TabControlStyle)element.GetValue(ControlStyleProperty);

        /// <summary>
        ///     TabHeader显示类型
        /// </summary>
        public TabControlStyle ControlStyle
        {
            get => (TabControlStyle)GetValue(ControlStyleProperty);
            set => SetValue(ControlStyleProperty, value);
        }

        /// <summary>
        ///     是否显示上下文菜单
        /// </summary>
        public static readonly DependencyProperty ShowContextMenuProperty = DependencyProperty.RegisterAttached(
            nameof(ShowContextMenu), typeof(bool), typeof(TabControlEx), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetShowContextMenu(DependencyObject element, bool value)
            => element.SetValue(ShowContextMenuProperty, value);

        public static bool GetShowContextMenu(DependencyObject element)
            => (bool)element.GetValue(ShowContextMenuProperty);

        /// <summary>
        ///     是否显示上下文菜单
        /// </summary>
        public bool ShowContextMenu
        {
            get => (bool)GetValue(ShowContextMenuProperty);
            set => SetValue(ShowContextMenuProperty, value);
        }

        /// <summary>
        ///     是否将标签填充
        /// </summary>
        public static readonly DependencyProperty IsTabFillEnabledProperty = DependencyProperty.Register(
            nameof(IsTabFillEnabled), typeof(bool), typeof(TabControlEx), new PropertyMetadata(false));

        /// <summary>
        ///     是否将标签填充
        /// </summary>
        public bool IsTabFillEnabled
        {
            get => (bool)GetValue(IsTabFillEnabledProperty);
            set => SetValue(IsTabFillEnabledProperty, value);
        }

        /// <summary>
        ///     标签宽度
        /// </summary>
        public static readonly DependencyProperty TabItemWidthProperty = DependencyProperty.Register(
            nameof(TabItemWidth), typeof(double), typeof(TabControlEx), new PropertyMetadata(double.NaN));

        /// <summary>
        ///     标签宽度
        /// </summary>
        public double TabItemWidth
        {
            get => (double)GetValue(TabItemWidthProperty);
            set => SetValue(TabItemWidthProperty, value);
        }

        /// <summary>
        ///     标签高度
        /// </summary>
        public static readonly DependencyProperty TabItemHeightProperty = DependencyProperty.Register(
            nameof(TabItemHeight), typeof(double), typeof(TabControlEx), new PropertyMetadata(double.NaN));

        /// <summary>
        ///     标签高度
        /// </summary>
        public double TabItemHeight
        {
            get => (double)GetValue(TabItemHeightProperty);
            set => SetValue(TabItemHeightProperty, value);
        }

        /// <summary>
        ///     是否显示溢出按钮
        /// </summary>
        public static readonly DependencyProperty ShowOverflowButtonProperty = DependencyProperty.Register(
            nameof(ShowOverflowButton), typeof(bool), typeof(TabControlEx), new PropertyMetadata(true));

        /// <summary>
        ///     是否显示溢出按钮
        /// </summary>
        public bool ShowOverflowButton
        {
            get => (bool)GetValue(ShowOverflowButtonProperty);
            set => SetValue(ShowOverflowButtonProperty, value);
        }


        /// <summary>
        ///     可见的标签数量
        /// </summary>
        private int _itemShowCount;

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            UpdateOverflowButton();
        }

        static TabControlEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControlEx), new FrameworkPropertyMetadata(typeof(TabControlEx)));
        }

        public override void OnApplyTemplate()
        {
            if (_buttonOverflow != null)
            {
                if (_buttonOverflow.ContextMenu != null)
                {
                    _buttonOverflow.ContextMenu.Closed -= Menu_Closed;
                    _buttonOverflow.ContextMenu = null;
                }

                _buttonOverflow.Click -= ButtonOverflow_Click;
            }

            if (_scrollViewerOverflow != null)
            {
                _scrollViewerOverflow.PreviewMouseWheel -= _scrollViewerOverflow_MouseWheel;
            }

            base.OnApplyTemplate();
            HeaderPanel = GetTemplateChild(PART_HeaderPanel) as TabPanel;

            if (IsTabFillEnabled) return;

            _buttonOverflow = GetTemplateChild(PART_OverflowButton) as ToggleButton;
            _scrollViewerOverflow = GetTemplateChild(PART_OverflowScrollviewer) as ScrollViewer;

            if (_scrollViewerOverflow != null)
            {
                _scrollViewerOverflow.PreviewMouseWheel += _scrollViewerOverflow_MouseWheel;
            }

            if (_buttonOverflow != null)
            {
                var menu = new System.Windows.Controls.ContextMenu
                {
                    Placement = PlacementMode.Bottom,
                    PlacementTarget = _buttonOverflow
                };
                menu.Closed += Menu_Closed;
                _buttonOverflow.ContextMenu = menu;
                _buttonOverflow.Click += ButtonOverflow_Click;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateOverflowButton();
        }

        private void UpdateOverflowButton()
        {
            if (!IsTabFillEnabled)
            {
                _itemShowCount = (int)(ActualWidth / TabItemWidth);
                if (_buttonOverflow != null)
                {
                    _buttonOverflow.Visibility = (ShowOverflowButton && Items.Count > 0 && Items.Count >= _itemShowCount) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private void Menu_Closed(object sender, RoutedEventArgs e) => _buttonOverflow.IsChecked = false;

        private void _scrollViewerOverflow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var _totalHorizontalOffset = Math.Min(Math.Max(0, GetHorizontalOffset() - e.Delta), _scrollViewerOverflow.ScrollableWidth);
            _scrollViewerOverflow.ScrollToHorizontalOffset(_totalHorizontalOffset);
        }

        internal double GetHorizontalOffset() => _scrollViewerOverflow?.HorizontalOffset ?? 0;

        private void ButtonOverflow_Click(object sender, RoutedEventArgs e)
        {
            if (_buttonOverflow.IsChecked == true)
            {
                _buttonOverflow.ContextMenu.Items.Clear();
                for (var i = 0; i < Items.Count; i++)
                {
                    if (!(ItemContainerGenerator.ContainerFromIndex(i) is TabItemEx item)) continue;

                    var menuItem = new System.Windows.Controls.MenuItem
                    {
                        HeaderStringFormat = ItemStringFormat,
                        HeaderTemplate = ItemTemplate,
                        HeaderTemplateSelector = ItemTemplateSelector,
                        Header = item.Header,
                        Width = TabItemWidth,
                        IsChecked = item.IsSelected,
                        IsCheckable = true
                    };

                    menuItem.Click += delegate {
                        _buttonOverflow.IsChecked = false;

                        var list = GetActualList();
                        if (list == null) return;

                        var actualItem = ItemContainerGenerator.ItemFromContainer(item);
                        if (actualItem == null) return;
                        item.IsSelected = true;

                        item.BringIntoView();
                    };
                    _buttonOverflow.ContextMenu.Items.Add(menuItem);
                }

                _buttonOverflow.ContextMenu.PlacementTarget = _buttonOverflow;
                _buttonOverflow.ContextMenu.IsOpen = true;
            }
            else
            {
                _buttonOverflow.ContextMenu.IsOpen = false;
            }
        }

        internal void UpdateScroll() => _scrollViewerOverflow?.RaiseEvent(new MouseWheelEventArgs(Mouse.PrimaryDevice, Environment.TickCount, 0)
        {
            RoutedEvent = MouseWheelEvent
        });

        internal void CloseAllItems() => CloseOtherItems(null);

        internal void CloseOtherItems(TabItemEx currentItem)
        {
            var actualItem = currentItem != null ? ItemContainerGenerator.ItemFromContainer(currentItem) : null;

            var list = GetActualList();
            if (list == null) return;

            IsInternalAction = true;

            for (var i = 0; i < Items.Count; i++)
            {
                var item = list[i];
                if (!Equals(item, actualItem) && item != null)
                {
                    var argsClosing = new CancelRoutedEventArgs(TabItemEx.ClosingEvent, item);

                    if (!(ItemContainerGenerator.ContainerFromItem(item) is TabItemEx tabItem)) continue;

                    tabItem.RaiseEvent(argsClosing);
                    if (argsClosing.Cancel) return;

                    tabItem.RaiseEvent(new RoutedEventArgs(TabItemEx.ClosedEvent, item));
                    list.Remove(item);

                    i--;
                }
            }

            SetCurrentValue(SelectedIndexProperty, Items.Count == 0 ? -1 : 0);
        }

        internal IList GetActualList()
        {
            IList list;
            if (ItemsSource != null)
            {
                list = ItemsSource as IList;
            }
            else
            {
                list = Items;
            }

            return list;
        }

        protected override bool IsItemItsOwnContainerOverride(object item) => item is TabItemEx;

        protected override DependencyObject GetContainerForItemOverride() => new TabItemEx();
    }
}