using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AIStudio.Wpf.Panels.Helpers;

namespace AIStudio.Wpf.Panels
{
    public class ResizableItemsControl : ItemsControl
    {
        #region BindingWidthAndHeight

        /// <summary>
        /// BindingWidthAndHeight Dependency Property
        /// </summary>
        public static readonly DependencyProperty BindingWidthAndHeightProperty =
            DependencyProperty.Register("BindingWidthAndHeight", typeof(bool), typeof(ResizableItemsControl),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets the BindingWidthAndHeight property. This dependency property 
        /// indicates if the ResizableItemsControl is in Composing mode.
        /// </summary>
        public bool BindingWidthAndHeight
        {
            get
            {
                return (bool)GetValue(BindingWidthAndHeightProperty);
            }
            set
            {
                SetValue(BindingWidthAndHeightProperty, value);
            }
        }

        #endregion

        #region Tile
        /// <summary>
        /// 容器内元素的高度
        /// </summary>
        public int TileHeight
        {
            get
            {
                return (int)GetValue(TileHeightProperty);
            }
            set
            {
                SetValue(TileHeightProperty, value);
            }
        }
        /// <summary>
        /// 容器内元素的高度
        /// </summary>
        public static readonly DependencyProperty TileHeightProperty =
            DependencyProperty.Register("TileHeight", typeof(int), typeof(ResizableItemsControl), new FrameworkPropertyMetadata(100, FrameworkPropertyMetadataOptions.AffectsMeasure));
        /// <summary>
        /// 容器内元素的宽度
        /// </summary>
        public int TileWidth
        {
            get
            {
                return (int)GetValue(TileWidthProperty);
            }
            set
            {
                SetValue(TileWidthProperty, value);
            }
        }
        /// <summary>
        /// 容器内元素的宽度
        /// </summary>
        public static readonly DependencyProperty TileWidthProperty =
            DependencyProperty.Register("TileWidth", typeof(int), typeof(ResizableItemsControl), new FrameworkPropertyMetadata(100, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 容器内元素的高度
        /// </summary>
        public int RowNum
        {
            get
            {
                return (int)GetValue(RowNumProperty);
            }
            set
            {
                SetValue(RowNumProperty, value);
            }
        }
        /// <summary>
        /// 容器内元素的高度
        /// </summary>
        public static readonly DependencyProperty RowNumProperty =
            DependencyProperty.Register("RowNum", typeof(int), typeof(ResizableItemsControl), new FrameworkPropertyMetadata(0, (d, e) => {
                ResizableItemsControl control = d as ResizableItemsControl;
                if (control.IsLoaded)
                {
                    control.SetTileHeight();
                }
            }));



        /// <summary>
        /// 容器内元素的宽度
        /// </summary>
        public int ColumnNum
        {
            get
            {
                return (int)GetValue(ColumnNumProperty);
            }
            set
            {
                SetValue(ColumnNumProperty, value);
            }
        }
        /// <summary>
        /// 容器内元素的宽度
        /// </summary>
        public static readonly DependencyProperty ColumnNumProperty =
            DependencyProperty.Register("ColumnNum", typeof(int), typeof(ResizableItemsControl), new FrameworkPropertyMetadata(0, (d, e) => {
                ResizableItemsControl control = d as ResizableItemsControl;
                if (control.IsLoaded)
                {
                    control.SetTileWidth();
                }
            }));

        /// <summary>
        /// 格子数量
        /// </summary>
        public int TileCount
        {
            get
            {
                return (int)GetValue(TileCountProperty);
            }
            set
            {
                SetValue(TileCountProperty, value);
            }
        }
        /// <summary>
        /// 格子数量
        /// </summary>
        public static readonly DependencyProperty TileCountProperty =
            DependencyProperty.Register("TileCount", typeof(int), typeof(ResizableItemsControl), new PropertyMetadata(4));

        /// <summary>
        /// Tile之间的间距
        /// </summary>
        public Thickness TileMargin
        {
            get
            {
                return (Thickness)GetValue(TileMarginProperty);
            }
            set
            {
                SetValue(TileMarginProperty, value);
            }
        }
        /// <summary>
        /// Tile之间的间距
        /// </summary>
        public static readonly DependencyProperty TileMarginProperty =
            DependencyProperty.Register("TileMargin", typeof(Thickness), typeof(ResizableItemsControl), new FrameworkPropertyMetadata(new Thickness(2)));
        #endregion

        #region PanelType

        /// <summary>
        /// PanelType Dependency Property
        /// </summary>
        public static readonly DependencyProperty PanelTypeProperty =
            DependencyProperty.Register("PanelType", typeof(PanelType), typeof(ResizableItemsControl),
                new FrameworkPropertyMetadata(PanelType.None));

        /// <summary>
        /// Gets or sets the BindingWidthAndHeight property. This dependency property 
        /// indicates if the ResizableItemsControl is in Composing mode.
        /// </summary>
        public PanelType PanelType
        {
            get
            {
                return (PanelType)GetValue(PanelTypeProperty);
            }
            set
            {
                SetValue(PanelTypeProperty, value);
            }
        }

        #endregion

        #region Orientation
        /// <summary>
        /// 排列方向
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ResizableItemsControl), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));
        /// <summary>
        /// 排列方向
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }
        #endregion

        #region Maximized

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
            DependencyProperty.Register("MaximizedHeight", typeof(double), typeof(ResizableItemsControl), new FrameworkPropertyMetadata(200d));
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
            DependencyProperty.Register("MaximizedWidth", typeof(double), typeof(ResizableItemsControl), new FrameworkPropertyMetadata(200d));
        #endregion

        #region Normal

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
            DependencyProperty.Register("NormalHeight", typeof(double), typeof(ResizableItemsControl), new FrameworkPropertyMetadata(100d));
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
            DependencyProperty.Register("NormalWidth", typeof(double), typeof(ResizableItemsControl), new FrameworkPropertyMetadata(100d));
        #endregion

        #region Title
        /// <summary>
        /// 显示标题
        /// </summary>
        public static readonly DependencyProperty ShowTitleProperty =
            DependencyProperty.Register("ShowTitle", typeof(bool), typeof(ResizableItemsControl), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// 显示标题
        /// </summary>
        public bool ShowTitle
        {
            get
            {
                return (bool)GetValue(ShowTitleProperty);
            }
            set
            {
                SetValue(ShowTitleProperty, value);
            }
        }

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

        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(ResizableItemsControl));
        #endregion

        #region MaximizedTile
        public Location MaximizedTileLocation
        {
            get
            {
                return (Location)GetValue(MaximizedTileLocationProperty);
            }
            set
            {
                SetValue(MaximizedTileLocationProperty, value);
            }
        }

        public static readonly DependencyProperty MaximizedTileLocationProperty =
            DependencyProperty.Register("MaximizedTileLocation", typeof(object), typeof(ResizableItemsControl), new FrameworkPropertyMetadata(Location.Left));

        public static readonly DependencyProperty MaximizedRatioProperty =
            DependencyProperty.Register("MaximizedRatio", typeof(object), typeof(ResizableItemsControl), new FrameworkPropertyMetadata(0.7));

        public double MaximizedRatio
        {
            get
            {
                return (double)GetValue(MaximizedRatioProperty);
            }
            set
            {
                SetValue(MaximizedRatioProperty, value);
            }
        }
        #endregion

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            if (ShowTitle)
            {
                return item is MDIItem;
            }
            else
            {
                return item is ResizableItem;
            }
        }

        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            if (ShowTitle)
            {
                return new MDIItem();
            }
            else
            {
                return new ResizableItem();
            }
        }

        static ResizableItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizableItemsControl), new FrameworkPropertyMetadata(typeof(ResizableItemsControl)));
        }
        #region 构造函数
        public ResizableItemsControl()
            : base()
        {
            //获取资源文件信息
            //ResourceDictionary rd = new ResourceDictionary();
            //rd.Source = new Uri("/AIStudio.Wpf.Panels;component/Themes/Generic.xaml", UriKind.Relative);
            //if (!this.Resources.MergedDictionaries.Any(p => p.Source == rd.Source))
            //{
            //    this.Resources.MergedDictionaries.Add(rd);
            //}
            //this.ItemContainerStyle = this.FindResource("ResizableItemItemContainerBindingWidthAndHeightStyle") as Style;
            //this.Style = this.FindResource("AIStudio.Styles.ResizableItemsControl") as Style;

            this.Drop += ResizableItemsControl_Drop;
            this.MouseLeftButtonDown += ResizableItemsControl_MouseLeftButtonDown;
            this.Loaded += ResizableItemsControl_Loaded;
            this.SizeChanged += ResizableItemsControl_SizeChanged;
        }

        private void ResizableItemsControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);
            HitTestResult result = VisualTreeHelper.HitTest(this, pos);
            if (result == null)
            {
                return;
            }
            var dragItem = VisualTreeExtension.FindVisualParent<ResizableItem>(result.VisualHit);
            if (dragItem == null)
            {
                return;
            }
            DragDrop.DoDragDrop(this, dragItem, DragDropEffects.Move);
        }

        private void ResizableItemsControl_Drop(object sender, DragEventArgs e)
        {
            var pos = e.GetPosition(this);
            var result = VisualTreeHelper.HitTest(this, pos);
            if (result == null)
            {
                return;
            }

            //查找元数据
            var sourceItem = (e.Data.GetData(typeof(ResizableItem)) ?? e.Data.GetData(typeof(MDIItem))) as ResizableItem;
            if (sourceItem == null)
            {
                return;
            }

            //查找目标数据
            var targetItem = VisualTreeExtension.FindVisualParent<ResizableItem>(result.VisualHit);
            if (targetItem == null)
            {
                return;
            }

            if (sourceItem == targetItem)
            {
                return;
            }
            int indexTarget = Items.IndexOf(targetItem.DataContext);

            // If no valid cell index is obtained, add the child to the end of the 
            // fluidElements list.
            if ((indexTarget == -1) || (indexTarget >= Items.Count))
            {
                indexTarget = Items.Count - 1;
            }

            if (ShowTitle && PanelType == PanelType.MaximizedTilePanel)
            {
                var targettilestatus = targetItem.GetPropertyValue("TileState");
                var sourcetilestatus = sourceItem.GetPropertyValue("TileState");
                targetItem.SetPropertyValue("TileState", sourcetilestatus);
                sourceItem.SetPropertyValue("TileState", targettilestatus);
            }

            var source = sourceItem.DataContext;
            ((IList)this.ItemsSource).Remove(source);
            ((IList)this.ItemsSource).Insert(indexTarget, source);

            this.Items.Refresh();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var resizableitem = element as ResizableItem;
            if (resizableitem != null)
            {
                resizableitem.Container = this;
            }

            var mdiitem = element as MDIItem;
            if (mdiitem != null)
            {
                if (PanelType == PanelType.MaximizedTilePanel)
                {
                    mdiitem.MaximizedWidth = double.NaN;
                    mdiitem.MaximizedHeight = double.NaN;
                    mdiitem.NormalWidth = this.NormalWidth;
                    mdiitem.NormalHeight = this.NormalHeight;
                }
                else if (PanelType == PanelType.StackPanel)
                {
                    if (Orientation == Orientation.Vertical)
                    {
                        mdiitem.MaximizedWidth = double.NaN;
                        mdiitem.MaximizedHeight = this.MaximizedHeight;
                    }
                    else
                    {
                        mdiitem.MaximizedWidth = this.MaximizedWidth;
                        mdiitem.MaximizedHeight = double.NaN;
                    }
                }
                else if (PanelType == PanelType.WaterfallPanel)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        mdiitem.MaximizedWidth = double.NaN;
                        mdiitem.MaximizedHeight = this.MaximizedHeight;
                    }
                    else
                    {
                        mdiitem.MaximizedWidth = this.MaximizedWidth;
                        mdiitem.MaximizedHeight = double.NaN;
                    }
                }
                else
                {
                    mdiitem.MaximizedWidth = this.MaximizedWidth;
                    mdiitem.MaximizedHeight = this.MaximizedHeight;
                }

                mdiitem.IsCloseButtonEnabled = this.Items != null;
                mdiitem.Closing += Mdiitem_Closing; ;
                mdiitem.TileStateChanged += Mdiitem_TileStateChanged; ;

                mdiitem.Focus();
            }

            base.PrepareContainerForItemOverride(element, item);
        }

        private void Mdiitem_Closing(object sender, RoutedEventArgs e)
        {
            var mdiitem = sender as MDIItem;
            if (mdiitem != null && mdiitem.DataContext != null)
            {
                // clear
                mdiitem.Closing -= Mdiitem_Closing;
                mdiitem.TileStateChanged -= Mdiitem_TileStateChanged;

                ((IList)this.ItemsSource).Remove(mdiitem.DataContext);
                this.Items.Refresh();
            }
        }

        private void Mdiitem_TileStateChanged(object sender, TileStateChangedEventArgs e)
        {
            if (PanelType == PanelType.MaximizedTilePanel)
            {
                if (e.NewValue == TileState.Maximized)
                {

                }
            }
        }

        protected override void OnItemsPanelChanged(ItemsPanelTemplate oldItemsPanel, ItemsPanelTemplate newItemsPanel)
        {
            base.OnItemsPanelChanged(oldItemsPanel, newItemsPanel);
            if (this.PanelType == PanelType.TilePanel)
            {
                SetTileWidth();
                SetTileHeight();
            }
        }

        private void ResizableItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.PanelType == PanelType.TilePanel)
            {
                SetTileWidth();
                SetTileHeight();
            }
        }

        private void ResizableItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.PanelType == PanelType.TilePanel)
            {
                SetTileWidth();
                SetTileHeight();
            }
        }

        private void SetTileWidth()
        {
            this.TileWidth = (int)(this.ActualWidth / this.ColumnNum - this.TileMargin.Left - this.TileMargin.Right);
            if (this.Orientation == Orientation.Horizontal)
            {
                this.TileCount = this.ColumnNum;
                this.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden);
            }
        }

        private void SetTileHeight()
        {
            this.TileHeight = (int)(this.ActualHeight / this.RowNum - this.TileMargin.Top - this.TileMargin.Bottom);
            if (this.Orientation == Orientation.Vertical)
            {
                this.TileCount = this.RowNum;
                this.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden);
            }
        }

        #endregion
    }



}
