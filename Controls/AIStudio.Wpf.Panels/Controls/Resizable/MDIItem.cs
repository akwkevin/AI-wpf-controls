using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using AIStudio.Wpf.Panels.Helpers;

namespace AIStudio.Wpf.Panels
{
    [TemplatePart(Name = PART_ButtonBar_MinimizeButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_ButtonBar_MaximizeButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_ButtonBar_CloseButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_RootGrid, Type = typeof(Grid))]
    [TemplatePart(Name = PART_Tumblr, Type = typeof(Image))]
    public class MDIItem : ResizableItem
    {
        private const string PART_ButtonBar_MinimizeButton = "PART_ButtonBar_MinimizeButton";
        private const string PART_ButtonBar_MaximizeButton = "PART_ButtonBar_MaximizeButton";
        private const string PART_ButtonBar_CloseButton = "PART_ButtonBar_CloseButton";
        private const string PART_RootGrid = "PART_RootGrid";
        private const string PART_Tumblr = "PART_Tumblr";

        private ButtonBase _buttonBar_MinimizeButton;
        private ButtonBase _buttonBar_MaximizeButton;
        private ButtonBase _buttonBar_CloseButton;
        private Grid _rootGrid;
        private Image _tumblr;

        internal TileState PreviousTileState
        {
            get; set;
        }
        internal double PreviousWidth { get; set; } = double.NaN;
        internal double PreviousHeight { get; set; } = double.NaN;


        #region TileState
        /// <summary>
        /// 当前瓦片状态
        /// </summary>
        public TileState TileState
        {
            get
            {
                return (TileState)GetValue(TileStateProperty);
            }
            set
            {
                SetValue(TileStateProperty, value);
            }
        }

        ///// <summary>
        ///// 当前瓦片状态
        ///// </summary>
        public static readonly DependencyProperty TileStateProperty =
            DependencyProperty.Register("TileState", typeof(TileState), typeof(MDIItem), new FrameworkPropertyMetadata(TileState.Normal, FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                (d, e) => {
                    if (d is MDIItem)
                    {
                        (d as MDIItem).OnTileStateChanged((TileState)e.OldValue, (TileState)e.NewValue);
                    }
                }));
        #endregion

        #region TileTime
        /// <summary>
        /// TileTime
        /// </summary>
        public DateTime TileTime
        {
            get;
            set;
        }

        #endregion

        #region MaximizedTileState
        /// <summary>
        /// 辨别是否
        /// </summary>
        public TileState MaximizedTileState
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 当前瓦片状态
        /// </summary>
        public object Title
        {
            get
            {
                return (object)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for CurrentTileState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(MDIItem), new PropertyMetadata(null));

        public DataTemplate TitleTemplate
        {
            get
            {
                return (DataTemplate)GetValue(TitleTemplateProperty);
            }
            set
            {
                SetValue(TitleTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(MDIItem));

        #region MaximizedTile

        /// <summary>
        /// 最大化元素的高度
        /// </summary>
        public double MaximizedHeight
        {
            get
            {
                return (double)GetValue(MaximizedHeightProperty);
            }
            set
            {
                SetValue(MaximizedHeightProperty, value);
            }
        }
        /// <summary>
        /// 容器内元素的高度
        /// </summary>
        public static readonly DependencyProperty MaximizedHeightProperty =
            DependencyProperty.Register("MaximizedHeight", typeof(double), typeof(MDIItem), new FrameworkPropertyMetadata(200d));
        /// <summary>
        /// 最大化元素的宽度
        /// </summary>
        public double MaximizedWidth
        {
            get
            {
                return (double)GetValue(MaximizedWidthProperty);
            }
            set
            {
                SetValue(MaximizedWidthProperty, value);
            }
        }
        /// <summary>
        /// 最大化元素的宽度
        /// </summary>
        public static readonly DependencyProperty MaximizedWidthProperty =
            DependencyProperty.Register("MaximizedWidth", typeof(double), typeof(MDIItem), new FrameworkPropertyMetadata(200d));


        public static double GetNormalHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(NormalHeightProperty);
        }

        public static void SetNormalHeight(DependencyObject obj, TileState value)
        {
            obj.SetValue(NormalHeightProperty, value);
        }

        /// <summary>
        /// 正常元素的高度
        /// </summary>
        public double NormalHeight
        {
            get
            {
                return (double)GetValue(NormalHeightProperty);
            }
            set
            {
                SetValue(NormalHeightProperty, value);
            }
        }

        /// <summary>
        /// 容器内元素的高度
        /// </summary>
        public static readonly DependencyProperty NormalHeightProperty =
            DependencyProperty.Register("NormalHeight", typeof(double), typeof(MDIItem), new FrameworkPropertyMetadata(100d));

        public static double GetNormalWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(NormalWidthProperty);
        }

        public static void SetNormalWidth(DependencyObject obj, TileState value)
        {
            obj.SetValue(NormalWidthProperty, value);
        }

        /// <summary>
        /// 正常元素的宽度
        /// </summary>
        public double NormalWidth
        {
            get
            {
                return (double)GetValue(NormalWidthProperty);
            }
            set
            {
                SetValue(NormalWidthProperty, value);
            }
        }
        /// <summary>
        /// 最大化元素的宽度
        /// </summary>
        public static readonly DependencyProperty NormalWidthProperty =
            DependencyProperty.Register("NormalWidth", typeof(double), typeof(MDIItem), new FrameworkPropertyMetadata(100d));
        #endregion

        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MDIItem), new UIPropertyMetadata(false));

        public static readonly RoutedEvent ClosingEvent = EventManager.RegisterRoutedEvent(
           "Closing", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MDIItem));

        [Bindable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsCloseButtonEnabled
        {
            get
            {
                return (bool)GetValue(IsCloseButtonEnabledProperty);
            }
            set
            {
                SetValue(IsCloseButtonEnabledProperty, value);
            }
        }

        public static readonly DependencyProperty IsCloseButtonEnabledProperty =
            DependencyProperty.Register("IsCloseButtonEnabled", typeof(bool), typeof(MDIItem), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.NotDataBindable));

        public event RoutedEventHandler Closing
        {
            add
            {
                AddHandler(ClosingEvent, value);
            }
            remove
            {
                RemoveHandler(ClosingEvent, value);
            }
        }

        public bool CanClose
        {
            get
            {
                return (bool)GetValue(CanCloseProperty);
            }
            set
            {
                SetValue(CanCloseProperty, value);
            }
        }

        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.Register("CanClose", typeof(bool), typeof(MDIItem), new FrameworkPropertyMetadata(true));

        public delegate void TileStateChangedRoutedEventHandler(object sender, TileStateChangedEventArgs e);

        public static readonly RoutedEvent TileStateChangedEvent = EventManager.RegisterRoutedEvent("TileStateChanged", RoutingStrategy.Bubble, typeof(TileStateChangedRoutedEventHandler), typeof(MDIItem));

        public event TileStateChangedRoutedEventHandler TileStateChanged
        {
            add
            {
                AddHandler(TileStateChangedEvent, value);
            }
            remove
            {
                RemoveHandler(TileStateChangedEvent, value);
            }
        }


        public MDIItem()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._buttonBar_MinimizeButton = this.GetTemplateChild(PART_ButtonBar_MinimizeButton) as ButtonBase;
            if (this._buttonBar_MinimizeButton != null)
            {
                this._buttonBar_MinimizeButton.Click += MinimizeButton_Click;
            }

            this._buttonBar_MaximizeButton = this.GetTemplateChild(PART_ButtonBar_MaximizeButton) as ButtonBase;
            if (this._buttonBar_MaximizeButton != null)
            {
                this._buttonBar_MaximizeButton.Click += MaximizeButton_Click;
            }

            this._buttonBar_CloseButton = this.GetTemplateChild(PART_ButtonBar_CloseButton) as ButtonBase;
            if (this._buttonBar_CloseButton != null)
            {
                this._buttonBar_CloseButton.Click += CloseButton_Click; ;
            }


            this._rootGrid = this.GetTemplateChild(PART_RootGrid) as Grid;
            this._tumblr = this.GetTemplateChild(PART_Tumblr) as Image;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }

        public override void InitWidthAndHeight()
        {
            if (Container.PanelType == PanelType.MaximizedTilePanel)
            {
                var maximizedTile = VisualTreeExtension.FindVisualParent<MaximizedTilePanel>(this);
                if (maximizedTile.MaximizedTileState == TileState.Maximized)
                {
                    if (Container.MaximizedTileLocation == Location.Left || Container.MaximizedTileLocation == Location.Right)
                    {
                        if (this.TileState == TileState.Maximized)
                        {
                            this.Width = double.NaN;
                            this.Height = double.NaN;
                        }
                        else
                        {
                            this.Width = double.NaN;
                            this.Height = this.NormalHeight;
                        }
                    }
                    else if (Container.MaximizedTileLocation == Location.Top || Container.MaximizedTileLocation == Location.Bottom)
                    {
                        if (this.TileState == TileState.Maximized)
                        {
                            this.Width = double.NaN;
                            this.Height = double.NaN;
                        }
                        else
                        {
                            this.Width = this.NormalWidth;
                            this.Height = double.NaN;
                        }
                    }
                }
                else
                {
                    this.Width = this.NormalWidth;
                    this.Height = this.NormalHeight;
                }
            }
        }

        #region 拖拽使用
        protected override void SetWidthChanged(double width)
        {
            base.SetWidthChanged(width);
        }

        protected override void SetHeightChanged(double height)
        {
            base.SetHeightChanged(height);
        }

        protected override void SetWidthMaximizedRatio(double width)
        {
            if (Container.PanelType == PanelType.MaximizedTilePanel)
            {
                var maximizedTile = VisualTreeExtension.FindVisualParent<MaximizedTilePanel>(this);
                if (maximizedTile.MaximizedTileState == TileState.Maximized)
                {
                    if (Container.MaximizedTileLocation == Location.Left || Container.MaximizedTileLocation == Location.Right)
                    {
                        if (this.TileState == TileState.Maximized)
                        {
                            Container.MaximizedRatio = width / (Container.ActualWidth - (Container.TileMargin.Left + Container.TileMargin.Right) * 3);
                        }
                        else
                        {
                            Container.MaximizedRatio = 1 - width / (Container.ActualWidth - (Container.TileMargin.Left + Container.TileMargin.Right) * 3);
                        }
                        this.Width = double.NaN;
                    }
                }
            }
        }


        protected override void SetHeightMaximizedRatio(double height)
        {
            if (Container.PanelType == PanelType.MaximizedTilePanel)
            {
                var maximizedTile = VisualTreeExtension.FindVisualParent<MaximizedTilePanel>(this);
                if (maximizedTile.MaximizedTileState == TileState.Maximized)
                {
                    if (Container.MaximizedTileLocation == Location.Top || Container.MaximizedTileLocation == Location.Bottom)
                    {
                        if (this.TileState == TileState.Maximized)
                        {
                            Container.MaximizedRatio = height / (Container.ActualHeight - (Container.TileMargin.Top + Container.TileMargin.Bottom) * 3);
                        }
                        else
                        {
                            Container.MaximizedRatio = 1 - height / (Container.ActualHeight - (Container.TileMargin.Top + Container.TileMargin.Bottom) * 3);
                        }
                        this.Height = double.NaN;
                    }
                }
            }
        }
        #endregion

        #region 焦点
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.Focus();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            this.IsSelected = false;
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            this.IsSelected = true;
        }
        #endregion

        #region TileState控制
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.ToggleMaximize();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.ToggleMinimize();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var canCloseBinding = BindingOperations.GetBindingExpression(this, CanCloseProperty);
            if (canCloseBinding != null)
            {
                canCloseBinding.UpdateTarget();
            }

            if (this.CanClose)
            {
                RaiseEvent(new RoutedEventArgs(ClosingEvent));
            }
        }

        public void ToggleMaximize()
        {
            if (TileState == TileState.Maximized)
            {
                Normalize();
            }
            else
            {
                Maximize();
            }
        }

        public void ToggleMinimize()
        {
            if (TileState != TileState.Minimized)
            {
                Minimize();
            }
            else
            {
                switch (PreviousTileState)
                {
                    case TileState.Maximized:
                        Maximize();
                        break;
                    case TileState.Normal:
                        Normalize();
                        break;
                    default:
                        throw new NotSupportedException("Invalid WindowState");
                }
            }
        }

        public void Maximize()
        {
            if (!double.IsNaN(this.Width))
                PreviousWidth = this.Width;
            this.Width = Math.Max(this.MaximizedWidth, !double.IsNaN(this.Width) ? this.Width : 0);
            //Tile使用
            if (!double.IsNaN(this.Width))
                SetWidthPixChanged(this.Width);

            if (!double.IsNaN(this.Height))
                PreviousHeight = this.Height;
            this.Height = Math.Max(this.MaximizedHeight, !double.IsNaN(this.Height) ? this.Height : 0);
            //Tile使用
            if (!double.IsNaN(this.Height))
                SetHeightPixChanged(this.Height);

            SetState(TileState.Maximized);
        }

        public void Normalize()
        {
            if (_rootGrid != null)
            {
                _rootGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
            }

            if (TileState == TileState.Maximized)
            {
                this.Width = PreviousWidth;
                //Tile使用
                if (!double.IsNaN(this.Width))
                    SetWidthPixChanged(this.Width);

                this.Height = PreviousHeight;
                //Tile使用
                if (!double.IsNaN(this.Height))
                    SetHeightPixChanged(this.Height);
            }

            SetState(TileState.Normal);
        }

        public void Minimize()
        {
            if (_rootGrid != null)
            {
                _rootGrid.RowDefinitions[1].Height = new GridLength(0.1, GridUnitType.Pixel);
            }
            if (_tumblr != null)
            {
                _tumblr.Source = this.CreateSnapshot();
            }

            SetState(TileState.Minimized);
        }

        public void SetState(TileState newvalue)
        {
            TileTime = DateTime.Now;
            //在属性上绑定了，需要给DataContext赋值
            this.SetPropertyValue("TileState", newvalue);
        }

        public void OnTileStateChanged(TileState oldvalue, TileState newvalue)
        {
            PreviousTileState = oldvalue;
            var args = new TileStateChangedEventArgs(TileStateChangedEvent, oldvalue, newvalue);

            RaiseEvent(args);
        }

        #endregion

    }
}