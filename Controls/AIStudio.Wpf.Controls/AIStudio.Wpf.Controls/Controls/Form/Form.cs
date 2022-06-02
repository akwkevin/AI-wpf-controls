using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    public class Form : Selector
    {
        #region AttachedProperty : HeaderWidthProperty
        public static readonly DependencyProperty HeaderWidthProperty
            = DependencyProperty.RegisterAttached("HeaderWidth", typeof(GridLength), typeof(Form), new FrameworkPropertyMetadata(new GridLength(80d, GridUnitType.Pixel), FrameworkPropertyMetadataOptions.Inherits));

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
            = DependencyProperty.RegisterAttached("ItemMargin", typeof(Thickness), typeof(Form), new FrameworkPropertyMetadata(new Thickness(3), FrameworkPropertyMetadataOptions.Inherits));
        public static Thickness GetItemMargin(DependencyObject element) => (Thickness)element.GetValue(ItemMarginProperty);
        public static void SetItemMargin(DependencyObject element, Thickness value) => element.SetValue(ItemMarginProperty, value);
        #endregion     

        #region AttachedProperty : HeaderMarginProperty
        public static readonly DependencyProperty HeaderMarginProperty
            = DependencyProperty.RegisterAttached("HeaderMargin", typeof(Thickness), typeof(Form), new FrameworkPropertyMetadata(new Thickness(0, 0, 3, 0), FrameworkPropertyMetadataOptions.Inherits));

        public static Thickness GetHeaderMargin(DependencyObject element) => (Thickness)element.GetValue(HeaderMarginProperty);
        public static void SetHeaderMargin(DependencyObject element, Thickness value) => element.SetValue(HeaderMarginProperty, value);
        #endregion

        #region AttachedProperty : BodyMarginProperty
        public static readonly DependencyProperty BodyMarginProperty
            = DependencyProperty.RegisterAttached("BodyMargin", typeof(Thickness), typeof(Form), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.Inherits));

        public static Thickness GetBodyMargin(DependencyObject element) => (Thickness)element.GetValue(BodyMarginProperty);
        public static void SetBodyMargin(DependencyObject element, Thickness value) => element.SetValue(BodyMarginProperty, value);
        #endregion

        //#region AttachedProperty : ItemHeightProperty
        //public static readonly DependencyProperty ItemHeightProperty
        //    = DependencyProperty.RegisterAttached("ItemHeight", typeof(double), typeof(Form), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.Inherits));

        //public static double GetItemHeight(DependencyObject element) => (double)element.GetValue(ItemHeightProperty);
        //public static void SetItemHeight(DependencyObject element, double value) => element.SetValue(ItemHeightProperty, value);
        //#endregion

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(Form), new FrameworkPropertyMetadata(true));
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

        /// <summary>
        ///     The Property for the Delay property.
        ///     Flags:              Can be used in style rules
        ///     Default Value:      Depend on SPI_GETKEYBOARDDELAY from SystemMetrics
        /// </summary>
        public static readonly DependencyProperty DelayProperty
            = DependencyProperty.Register("Delay", typeof(int), typeof(Form),
                                          new FrameworkPropertyMetadata(GetKeyboardDelay()),
                                          new ValidateValueCallback(IsDelayValid));

        /// <summary>
        ///     Specifies the amount of time, in milliseconds, to wait before repeating begins.
        /// Must be non-negative
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public int Delay
        {
            get
            {
                return (int)GetValue(DelayProperty);
            }
            set
            {
                SetValue(DelayProperty, value);
            }
        }

        private static bool IsDelayValid(object value)
        {
            return ((int)value) >= 0;
        }

        /// <summary>
        ///     The Property for the Interval property.
        ///     Flags:              Can be used in style rules
        ///     Default Value:      Depend on SPI_GETKEYBOARDSPEED from SystemMetrics
        /// </summary>
        public static readonly DependencyProperty IntervalProperty
            = DependencyProperty.Register("Interval", typeof(int), typeof(Form),
                                          new FrameworkPropertyMetadata(GetKeyboardSpeed()),
                                          new ValidateValueCallback(IsIntervalValid));

        /// <summary>
        ///     Specifies the amount of time, in milliseconds, between repeats once repeating starts.
        /// Must be non-negative
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public int Interval
        {
            get
            {
                return (int)GetValue(IntervalProperty);
            }
            set
            {
                SetValue(IntervalProperty, value);
            }
        }

        private static bool IsIntervalValid(object value)
        {
            return ((int)value) > 0;
        }

        public static readonly DependencyProperty PanelTypeProperty =
          DependencyProperty.Register("PanelType", typeof(FormPanelType), typeof(Form), new PropertyMetadata(FormPanelType.StackPanel, OnPanelTypeChanged));

        public FormPanelType PanelType
        {
            get
            {
                return (FormPanelType)GetValue(PanelTypeProperty);
            }
            set
            {
                SetValue(PanelTypeProperty, value);
            }
        }

        private static void OnPanelTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Form form)
            {
                ItemsPanelTemplate panel = new ItemsPanelTemplate();
                if ((FormPanelType)e.NewValue == FormPanelType.StackPanel)
                {
                    FrameworkElementFactory factory = new FrameworkElementFactory(typeof(StackPanel));
                    factory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
                    panel.VisualTree = factory;
                }
                else if ((FormPanelType)e.NewValue == FormPanelType.WrapPanel)
                {
                    FrameworkElementFactory factory = new FrameworkElementFactory(typeof(WrapPanel));
                    panel.VisualTree = factory;
                }
                else if ((FormPanelType)e.NewValue == FormPanelType.UniformGrid)
                {
                    FrameworkElementFactory factory = new FrameworkElementFactory(typeof(UniformGridEx));
                    factory.SetValue(UniformGridEx.ColumnsProperty, form.PanelColumns);
                    factory.SetValue(UniformGridEx.VerticalAlignmentProperty, VerticalAlignment.Top);
                    panel.VisualTree = factory;
                }

                form.ItemsPanel = panel;
            }

        }

        public static readonly DependencyProperty PanelColumnsProperty =
          DependencyProperty.Register("PanelColumns", typeof(int), typeof(Form), new PropertyMetadata(3, OnPanelColumnsChanged), new ValidateValueCallback(IsPanelColumnsValid));

        public int PanelColumns
        {
            get
            {
                return (int)GetValue(PanelColumnsProperty);
            }
            set
            {
                SetValue(PanelColumnsProperty, value);
            }
        }

        private static void OnPanelColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Form form)
            {               
                if (form.PanelType == FormPanelType.UniformGrid)
                {
                    ItemsPanelTemplate panel = new ItemsPanelTemplate();
                    FrameworkElementFactory factory = new FrameworkElementFactory(typeof(UniformGridEx));
                    factory.SetValue(UniformGridEx.ColumnsProperty, form.PanelColumns);
                    factory.SetValue(UniformGridEx.VerticalAlignmentProperty, VerticalAlignment.Top);
                    panel.VisualTree = factory;
                    form.ItemsPanel = panel;
                }               
            }
        }

        private static bool IsPanelColumnsValid(object value)
        {
            return ((int)value) > 0;
        }

        public Form()
        {
            this.Background = Brushes.Transparent;

            this.Drop += Form_Drop;
            this.DragOver += Form_DragOver;

        }

        internal void NotifyListItemClicked(FormItem item, MouseButton mouseButton)
        {
            if (IsReadOnly == true)
                return;

            if (!item.IsSelected)
            {
                item.SetCurrentValue(IsSelectedProperty, true);
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                item.SetCurrentValue(IsSelectedProperty, false);
            }

            if (item.IsSelected)
            {

                bool isItemsSource;
                var list = GetActualList(out isItemsSource);
                int index = list.IndexOf(isItemsSource ? item.DataContext : item);
                SetCurrentValue(SelectedIndexProperty, index);
            }
            else
            {
                SetCurrentValue(SelectedIndexProperty, -1);
            }
        }

        private DispatcherTimer _timer;
        private FormItem _dragItem;


        /// <summary>
        /// This is the method that responds to the MouseButtonEvent event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (AllowDrop == false)
            {
                e.Handled = true;
                return;
            }

            var pos = e.GetPosition(this);
            HitTestResult result = VisualTreeHelper.HitTest(this, pos);
            if (result == null)
            {
                return;
            }
            _dragItem = VisualHelper.FindParent<FormItem>(result.VisualHit);
            if (_dragItem == null)
            {
                return;
            }

            StartTimer();

            //DragDrop.DoDragDrop(this, _dragItem, DragDropEffects.Move);
        }

        /// <summary>
        /// This is the method that responds to the MouseButtonEvent event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            _dragItem = null;
            StopTimer();

        }

        /// <summary>
        /// Starts a _timer ticking
        /// </summary>
        private void StartTimer()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Tick += new EventHandler(OnTimeout);
            }
            else if (_timer.IsEnabled)
                return;

            _timer.Interval = TimeSpan.FromMilliseconds(Delay);
            _timer.Start();
        }

        /// <summary>
        /// Stops a _timer that has already started
        /// </summary>
        private void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// This is the handler for when the repeat _timer expires. All we do
        /// is invoke a click.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void OnTimeout(object sender, EventArgs e)
        {
            TimeSpan interval = TimeSpan.FromMilliseconds(Interval);
            if (_timer.Interval != interval)
                _timer.Interval = interval;


            if (_dragItem != null)
            {
                DragDrop.DoDragDrop(this, _dragItem, DragDropEffects.Move);
                _dragItem = null;
            }

        }

        /// <summary>
        /// Retrieves the keyboard repeat-delay setting, which is a value in the range from 0
        /// (approximately 250 ms delay) through 3 (approximately 1 second delay).
        /// The actual delay associated with each value may vary depending on the hardware.
        /// </summary>
        /// <returns></returns>
        internal static int GetKeyboardDelay()
        {
            int delay = SystemParameters.KeyboardDelay;
            // SPI_GETKEYBOARDDELAY 0,1,2,3 correspond to 250,500,750,1000ms
            if (delay < 0 || delay > 3)
                delay = 0;
            return (delay + 1) * 250;
        }

        /// <summary>
        /// Retrieves the keyboard repeat-speed setting, which is a value in the range from 0
        /// (approximately 2.5 repetitions per second) through 31 (approximately 30 repetitions per second).
        /// The actual repeat rates are hardware-dependent and may vary from a linear scale by as much as 20%
        /// </summary>
        /// <returns></returns>
        internal static int GetKeyboardSpeed()
        {
            int speed = SystemParameters.KeyboardSpeed;
            // SPI_GETKEYBOARDSPEED 0,...,31 correspond to 1000/2.5=400,...,1000/30 ms
            if (speed < 0 || speed > 31)
                speed = 31;
            return (31 - speed) * (400 - 1000 / 30) / 31 + 1000 / 30;
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
            else if (sourceItem.ParentForm != this)
            {
                InsertItem(sourceItem, targetItem);
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
            object source;
            if (isItemsSource)
            {
                source = sourceItem.DataContext;
            }
            else
            {
                if (sourceItem.ParentSelector != this)
                {
                    string xaml = System.Windows.Markup.XamlWriter.Save(sourceItem);
                    source = System.Windows.Markup.XamlReader.Parse(xaml) as FormItem;
                }
                else
                {
                    source = sourceItem;
                }
            }

            list.Add(source);
        }

        private void InsertItem(FormItem sourceItem, FormItem targetItem)
        {
            bool isItemsSource;
            var list = GetActualList(out isItemsSource);
            var target = isItemsSource ? targetItem.DataContext : targetItem;
            int indexTarget = list.IndexOf(target);

            object source;
            if (isItemsSource)
            {
                source = sourceItem.DataContext;
            }
            else
            {
                if (sourceItem.ParentSelector != this)
                {
                    string xaml = System.Windows.Markup.XamlWriter.Save(sourceItem);
                    source = System.Windows.Markup.XamlReader.Parse(xaml) as FormItem;
                    if (source is FormItem item)
                        item.IsSelected = false;
                }
                else
                {
                    source = sourceItem;
                }
            }

            list.Insert(indexTarget, source);
        }

        public void CopySelectItem()
        {
            if (SelectedItem == null)
                return;

            bool isItemsSource;
            var list = GetActualList(out isItemsSource);

            object source;
            if (isItemsSource)
            {
                source = SelectedItem;
            }
            else
            {
                string xaml = System.Windows.Markup.XamlWriter.Save(SelectedItem);
                source = System.Windows.Markup.XamlReader.Parse(xaml) as FormItem;
                if (source is FormItem item)
                    item.IsSelected = false;
            }

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
    }
}
