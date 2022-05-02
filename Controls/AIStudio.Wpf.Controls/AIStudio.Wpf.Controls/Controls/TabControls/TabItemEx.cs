using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.Controls.Event;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_BtnClose, Type = typeof(Button))]
    [TemplatePart(Name = PART_Thumb, Type = typeof(Thumb))]
    public class TabItemEx : System.Windows.Controls.TabItem
    {

        private const string PART_BtnClose = "PART_BtnClose";
        private const string PART_Thumb = "PART_Thumb";

        private Button _btnClose;
        private Thumb _thumb;

        /// <summary>
        ///     是否显示关闭按钮
        /// </summary>
        public static readonly DependencyProperty ShowCloseButtonProperty =
            TabControlEx.ShowCloseButtonProperty.AddOwner(typeof(TabItemEx));

        /// <summary>
        ///     是否显示关闭按钮
        /// </summary>
        public bool ShowCloseButton
        {
            get => (bool)GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, value);
        }

        public static void SetShowCloseButton(DependencyObject element, bool value)
            => element.SetValue(ShowCloseButtonProperty, value);

        public static bool GetShowCloseButton(DependencyObject element)
            => (bool)element.GetValue(ShowCloseButtonProperty);

        /// <summary>
        ///     TabHeader显示类型
        /// </summary>
        public static readonly DependencyProperty ControlStyleProperty =
            TabControlEx.ControlStyleProperty.AddOwner(typeof(TabItemEx), new FrameworkPropertyMetadata(TabControlStyle.Card));

        /// <summary>
        ///     TabHeader显示类型
        /// </summary>
        public TabControlStyle ControlStyle
        {
            get => (TabControlStyle)GetValue(ControlStyleProperty);
            set => SetValue(ControlStyleProperty, value);
        }

        public static void SetControlStyle(DependencyObject element, TabControlStyle value)
            => element.SetValue(ControlStyleProperty, value);

        public static TabControlStyle GetControlStyle(DependencyObject element)
            => (TabControlStyle)element.GetValue(ControlStyleProperty);

        /// <summary>
        ///     是否显示上下文菜单
        /// </summary>
        public static readonly DependencyProperty ShowContextMenuProperty =
            TabControlEx.ShowContextMenuProperty.AddOwner(typeof(TabItemEx), new FrameworkPropertyMetadata(true));

        /// <summary>
        ///     是否显示上下文菜单
        /// </summary>
        public bool ShowContextMenu
        {
            get => (bool)GetValue(ShowContextMenuProperty);
            set => SetValue(ShowContextMenuProperty, value);
        }

        public static void SetShowContextMenu(DependencyObject element, bool value)
            => element.SetValue(ShowContextMenuProperty, value);

        public static bool GetShowContextMenu(DependencyObject element)
            => (bool)element.GetValue(ShowContextMenuProperty);

        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(
            "Menu", typeof(System.Windows.Controls.ContextMenu), typeof(TabItemEx), new PropertyMetadata(default(System.Windows.Controls.ContextMenu)));

        public System.Windows.Controls.ContextMenu Menu
        {
            get => (System.Windows.Controls.ContextMenu)GetValue(MenuProperty);
            set => SetValue(MenuProperty, value);
        }

        public TabItemEx()
        {
            CommandBindings.Add(new CommandBinding(ControlCommands.Close, (s, e) => Close()));
            CommandBindings.Add(new CommandBinding(ControlCommands.CloseAll,
                (s, e) => { TabControlParent.CloseAllItems(); }));
            CommandBindings.Add(new CommandBinding(ControlCommands.CloseOther,
                (s, e) => { TabControlParent.CloseOtherItems(this); }));

            this.Drop += TabItemEx_Drop;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (_btnClose != null)
            {
                _btnClose.Click -= (s, e) => Close();
            }
            if (_thumb != null)
            {
                _thumb.DragStarted -= ThumbOnDragStarted;
                _thumb.DragDelta -= ThumbOnDragDelta;
                _thumb.DragCompleted -= ThumbOnDragCompleted;
            }

            _btnClose = GetTemplateChild(PART_BtnClose) as Button;
            if (_btnClose != null)
            {
                _btnClose.Click += (s, e) => Close();
            }
            _thumb = GetTemplateChild(PART_Thumb) as Thumb;
            if (_thumb != null)
            {
                _thumb.DragStarted += ThumbOnDragStarted;
                _thumb.DragDelta += ThumbOnDragDelta;
                _thumb.DragCompleted += ThumbOnDragCompleted;
            }
        }

        private TabControlEx TabControlParent => System.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(this) as TabControlEx;

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            IsSelected = true;
            Focus();
        }

        internal void Close()
        {
            var parent = TabControlParent;
            if (parent == null) return;

            var item = parent.ItemContainerGenerator.ItemFromContainer(this);

            var argsClosing = new CancelRoutedEventArgs(ClosingEvent, item);
            RaiseEvent(argsClosing);
            if (argsClosing.Cancel) return;

            parent.IsInternalAction = true;
            RaiseEvent(new RoutedEventArgs(ClosedEvent, item));

            var list = parent.GetActualList();
            list?.Remove(item);
        }

        private void ThumbOnDragCompleted(object sender, DragCompletedEventArgs dragCompletedEventArgs)
        {

        }

        private void ThumbOnDragDelta(object sender, DragDeltaEventArgs dragDeltaEventArgs)
        {
            DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
        }

        private void ThumbOnDragStarted(object sender, DragStartedEventArgs dragStartedEventArgs)
        {
            this.IsSelected = true;
        }

        private void TabItemEx_Drop(object sender, DragEventArgs e)
        {
            var parent = TabControlParent;
            if (parent == null) return;

            //查找元数据
            var sourceItem = e.Data.GetData(typeof(TabItemEx)) as TabItemEx;
            if (sourceItem == null)
            {
                return;
            }

            var targetItem = this;
            if (sourceItem == targetItem)
            {
                return;
            }

            var source = parent.ItemContainerGenerator.ItemFromContainer(sourceItem);
            var target = parent.ItemContainerGenerator.ItemFromContainer(targetItem);

            var list = parent.GetActualList();

            int indexTarget = list.IndexOf(target);

            list.Remove(source);
            list.Insert(indexTarget, source);

            parent.SelectedIndex = indexTarget;
        }

        public static readonly RoutedEvent ClosingEvent = EventManager.RegisterRoutedEvent(nameof(Closing), RoutingStrategy.Bubble, typeof(EventHandler), typeof(TabItemEx));

        public event EventHandler Closing
        {
            add => AddHandler(ClosingEvent, value);
            remove => RemoveHandler(ClosingEvent, value);
        }

        public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent(nameof(Closed), RoutingStrategy.Bubble, typeof(EventHandler), typeof(TabItemEx));

        public event EventHandler Closed
        {
            add => AddHandler(ClosedEvent, value);
            remove => RemoveHandler(ClosedEvent, value);
        }
    }
}