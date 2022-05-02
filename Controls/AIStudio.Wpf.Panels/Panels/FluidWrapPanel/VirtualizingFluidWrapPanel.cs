#region File Header

// -------------------------------------------------------------------------------
// 
// This file is part of the WPFSpark project: http://wpfspark.codeplex.com/
//
// Author: Ratish Philip
// 
// WPFSpark v1.1
//
// -------------------------------------------------------------------------------

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AIStudio.Wpf.Panels
{
    /// <summary>
    /// Interactions for the VirtualizingFluidWrapPanel
    /// </summary>
    public class VirtualizingFluidWrapPanel : VirtualizingPanel, IScrollInfo, IFluidWrapPanel
    {
        #region Constants

        private const double NORMAL_SCALE = 1.0d;
        private const double DRAG_SCALE_DEFAULT = 1.3d;
        private const double NORMAL_OPACITY = 1.0d;
        private const double DRAG_OPACITY_DEFAULT = 0.6d;
        private const double OPACITY_MIN = 0.1d;
        private const Int32 Z_INDEX_NORMAL = 0;
        private const Int32 Z_INDEX_INTERMEDIATE = 1;
        private const Int32 Z_INDEX_DRAG = 10;
        private static TimeSpan DEFAULT_ANIMATION_TIME_WITHOUT_EASING = TimeSpan.FromMilliseconds(200);
        private static TimeSpan DEFAULT_ANIMATION_TIME_WITH_EASING = TimeSpan.FromMilliseconds(400);
        private static TimeSpan FIRST_TIME_ANIMATION_DURATION = TimeSpan.FromMilliseconds(320);

        private TranslateTransform trans = new TranslateTransform();//htzk123
        private ScrollViewer owner;
        private bool canHScroll = false;
        private bool canVScroll = false;
        private Size extent = new Size(0, 0);
        private Size viewport = new Size(0, 0);
        private Point offset;
        private DoubleAnimation transAnimation;
        private IEasingFunction easingFunction;
        #endregion

        #region Fields

        Point dragStartPoint = new Point();
        UIElement dragElement = null;
        UIElement lastDragElement = null;
        List<UIElement> fluidElements = null;
        FluidLayoutManager layoutManager = null;
        bool isInitializeArrangeRequired = false;
        #endregion

        #region Dependency Properties

        #region DragEasing

        /// <summary>
        /// DragEasing Dependency Property
        /// </summary>
        public static readonly DependencyProperty DragEasingProperty =
            DependencyProperty.Register("DragEasing", typeof(EasingFunctionBase), typeof(VirtualizingFluidWrapPanel),
                new FrameworkPropertyMetadata((new PropertyChangedCallback(OnDragEasingChanged))));

        /// <summary>
        /// Gets or sets the DragEasing property. This dependency property 
        /// indicates the Easing function to be used when the user stops dragging the child and releases it.
        /// </summary>
        public EasingFunctionBase DragEasing
        {
            get
            {
                return (EasingFunctionBase)GetValue(DragEasingProperty);
            }
            set
            {
                SetValue(DragEasingProperty, value);
            }
        }

        /// <summary>
        /// Handles changes to the DragEasing property.
        /// </summary>
        /// <param name="d">VirtualizingFluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnDragEasingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingFluidWrapPanel panel = (VirtualizingFluidWrapPanel)d;
            EasingFunctionBase oldDragEasing = (EasingFunctionBase)e.OldValue;
            EasingFunctionBase newDragEasing = panel.DragEasing;
            panel.OnDragEasingChanged(oldDragEasing, newDragEasing);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the DragEasing property.
        /// </summary>
        /// <param name="oldDragEasing">Old Value</param>
        /// <param name="newDragEasing">New Value</param>
        protected virtual void OnDragEasingChanged(EasingFunctionBase oldDragEasing, EasingFunctionBase newDragEasing)
        {

        }

        #endregion

        #region DragOpacity

        /// <summary>
        /// DragOpacity Dependency Property
        /// </summary>
        public static readonly DependencyProperty DragOpacityProperty =
            DependencyProperty.Register("DragOpacity", typeof(double), typeof(VirtualizingFluidWrapPanel),
                new FrameworkPropertyMetadata(DRAG_OPACITY_DEFAULT,
                                              new PropertyChangedCallback(OnDragOpacityChanged),
                                              new CoerceValueCallback(CoerceDragOpacity)));

        /// <summary>
        /// Gets or sets the DragOpacity property. This dependency property 
        /// indicates the opacity of the child being dragged.
        /// </summary>
        public double DragOpacity
        {
            get
            {
                return (double)GetValue(DragOpacityProperty);
            }
            set
            {
                SetValue(DragOpacityProperty, value);
            }
        }


        /// <summary>
        /// Coerces the FluidDrag Opacity to an acceptable value
        /// </summary>
        /// <param name="d">Dependency Object</param>
        /// <param name="value">Value</param>
        /// <returns>Coerced Value</returns>
        private static object CoerceDragOpacity(DependencyObject d, object value)
        {
            double opacity = (double)value;

            if (opacity < OPACITY_MIN)
            {
                opacity = OPACITY_MIN;
            }
            else if (opacity > NORMAL_OPACITY)
            {
                opacity = NORMAL_OPACITY;
            }

            return opacity;
        }

        /// <summary>
        /// Handles changes to the DragOpacity property.
        /// </summary>
        /// <param name="d">VirtualizingFluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnDragOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingFluidWrapPanel panel = (VirtualizingFluidWrapPanel)d;
            double oldDragOpacity = (double)e.OldValue;
            double newDragOpacity = panel.DragOpacity;
            panel.OnDragOpacityChanged(oldDragOpacity, newDragOpacity);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the DragOpacity property.
        /// </summary>
        /// <param name="oldDragOpacity">Old Value</param>
        /// <param name="newDragOpacity">New Value</param>
        protected virtual void OnDragOpacityChanged(double oldDragOpacity, double newDragOpacity)
        {

        }

        #endregion

        #region DragScale

        /// <summary>
        /// DragScale Dependency Property
        /// </summary>
        public static readonly DependencyProperty DragScaleProperty =
            DependencyProperty.Register("DragScale", typeof(double), typeof(VirtualizingFluidWrapPanel),
                new FrameworkPropertyMetadata(DRAG_SCALE_DEFAULT, new PropertyChangedCallback(OnDragScaleChanged)));

        /// <summary>
        /// Gets or sets the DragScale property. This dependency property 
        /// indicates the factor by which the child should be scaled when it is dragged.
        /// </summary>
        public double DragScale
        {
            get
            {
                return (double)GetValue(DragScaleProperty);
            }
            set
            {
                SetValue(DragScaleProperty, value);
            }
        }

        /// <summary>
        /// Handles changes to the DragScale property.
        /// </summary>
        /// <param name="d">VirtualizingFluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnDragScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingFluidWrapPanel panel = (VirtualizingFluidWrapPanel)d;
            double oldDragScale = (double)e.OldValue;
            double newDragScale = panel.DragScale;
            panel.OnDragScaleChanged(oldDragScale, newDragScale);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the DragScale property.
        /// </summary>
        /// <param name="oldDragScale">Old Value</param>
        /// <param name="newDragScale">New Value</param>
        protected virtual void OnDragScaleChanged(double oldDragScale, double newDragScale)
        {

        }

        #endregion

        #region ElementEasing

        /// <summary>
        /// ElementEasing Dependency Property
        /// </summary>
        public static readonly DependencyProperty ElementEasingProperty =
            DependencyProperty.Register("ElementEasing", typeof(EasingFunctionBase), typeof(VirtualizingFluidWrapPanel),
                new FrameworkPropertyMetadata((new PropertyChangedCallback(OnElementEasingChanged))));

        /// <summary>
        /// Gets or sets the ElementEasing property. This dependency property 
        /// indicates the Easing Function to be used when the elements are rearranged.
        /// </summary>
        public EasingFunctionBase ElementEasing
        {
            get
            {
                return (EasingFunctionBase)GetValue(ElementEasingProperty);
            }
            set
            {
                SetValue(ElementEasingProperty, value);
            }
        }

        /// <summary>
        /// Handles changes to the ElementEasing property.
        /// </summary>
        /// <param name="d">VirtualizingFluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnElementEasingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingFluidWrapPanel panel = (VirtualizingFluidWrapPanel)d;
            EasingFunctionBase oldElementEasing = (EasingFunctionBase)e.OldValue;
            EasingFunctionBase newElementEasing = panel.ElementEasing;
            panel.OnElementEasingChanged(oldElementEasing, newElementEasing);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ElementEasing property.
        /// </summary>
        /// <param name="oldElementEasing">Old Value</param>
        /// <param name="newElementEasing">New Value</param>
        /// 
        protected virtual void OnElementEasingChanged(EasingFunctionBase oldElementEasing, EasingFunctionBase newElementEasing)
        {

        }

        #endregion

        #region IsComposing

        /// <summary>
        /// IsComposing Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsComposingProperty =
            DependencyProperty.Register("IsComposing", typeof(bool), typeof(VirtualizingFluidWrapPanel),
                new FrameworkPropertyMetadata((new PropertyChangedCallback(OnIsComposingChanged))));

        /// <summary>
        /// Gets or sets the IsComposing property. This dependency property 
        /// indicates if the VirtualizingFluidWrapPanel is in Composing mode.
        /// </summary>
        public bool IsComposing
        {
            get
            {
                return (bool)GetValue(IsComposingProperty);
            }
            set
            {
                SetValue(IsComposingProperty, value);
            }
        }

        /// <summary>
        /// Handles changes to the IsComposing property.
        /// </summary>
        /// <param name="d">VirtualizingFluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnIsComposingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingFluidWrapPanel panel = (VirtualizingFluidWrapPanel)d;
            bool oldIsComposing = (bool)e.OldValue;
            bool newIsComposing = panel.IsComposing;
            panel.OnIsComposingChanged(oldIsComposing, newIsComposing);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IsComposing property.
        /// </summary>
        /// <param name="oldIsComposing">Old Value</param>
        /// <param name="newIsComposing">New Value</param>
        protected virtual void OnIsComposingChanged(bool oldIsComposing, bool newIsComposing)
        {

        }

        #endregion

        #region ItemHeight

        /// <summary>
        /// ItemHeight Dependency Property
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(VirtualizingFluidWrapPanel),
                new FrameworkPropertyMetadata(0.0,
                    new PropertyChangedCallback(OnItemHeightChanged)));

        /// <summary>
        /// Gets or sets the ItemHeight property. This dependency property 
        /// indicates the height of each item.
        /// </summary>
        public double ItemHeight
        {
            get
            {
                return (double)GetValue(ItemHeightProperty);
            }
            set
            {
                SetValue(ItemHeightProperty, value);
            }
        }

        /// <summary>
        /// Handles changes to the ItemHeight property.
        /// </summary>
        /// <param name="d">VirtualizingFluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingFluidWrapPanel fwPanel = (VirtualizingFluidWrapPanel)d;
            double oldItemHeight = (double)e.OldValue;
            double newItemHeight = fwPanel.ItemHeight;
            fwPanel.OnItemHeightChanged(oldItemHeight, newItemHeight);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ItemHeight property.
        /// </summary>
        /// <param name="oldItemHeight">Old Value</param>
        /// <param name="newItemHeight">New Value</param>
        protected void OnItemHeightChanged(double oldItemHeight, double newItemHeight)
        {

        }

        #endregion

        #region ItemWidth

        /// <summary>
        /// ItemWidth Dependency Property
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(VirtualizingFluidWrapPanel),
                new FrameworkPropertyMetadata(0.0,
                    new PropertyChangedCallback(OnItemWidthChanged)));

        /// <summary>
        /// Gets or sets the ItemWidth property. This dependency property 
        /// indicates the width of each item.
        /// </summary>
        public double ItemWidth
        {
            get
            {
                return (double)GetValue(ItemWidthProperty);
            }
            set
            {
                SetValue(ItemWidthProperty, value);
            }
        }

        /// <summary>
        /// Handles changes to the ItemWidth property.
        /// </summary>
        /// <param name="d">VirtualizingFluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingFluidWrapPanel fwPanel = (VirtualizingFluidWrapPanel)d;
            double oldItemWidth = (double)e.OldValue;
            double newItemWidth = fwPanel.ItemWidth;
            fwPanel.OnItemWidthChanged(oldItemWidth, newItemWidth);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ItemWidth property.
        /// </summary>
        /// <param name="oldItemWidth">Old Value</param>
        /// <param name="newItemWidth">New Value</param>
        protected void OnItemWidthChanged(double oldItemWidth, double newItemWidth)
        {

        }

        #endregion

        #region ItemsSource

        /// <summary>
        /// ItemsSource Dependency Property
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(VirtualizingFluidWrapPanel),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnItemsSourceChanged)));

        /// <summary>
        /// Gets or sets the ItemsSource property. This dependency property 
        /// indicates the bindable collection.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return (ObservableCollection<UIElement>)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Handles changes to the ItemsSource property.
        /// </summary>
        /// <param name="d">VirtualizingFluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingFluidWrapPanel panel = (VirtualizingFluidWrapPanel)d;
            IEnumerable oldItemsSource = (ObservableCollection<UIElement>)e.OldValue;
            IEnumerable newItemsSource = panel.ItemsSource;
            panel.OnItemsSourceChanged(oldItemsSource, newItemsSource);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the ItemsSource property.
        /// </summary>
        /// <param name="oldItemsSource">Old Value</param>
        /// <param name="newItemsSource">New Value</param>
        protected void OnItemsSourceChanged(IEnumerable oldItemsSource, IEnumerable newItemsSource)
        {
            if (oldItemsSource is INotifyCollectionChanged)
            {
                (oldItemsSource as INotifyCollectionChanged).CollectionChanged -= ItemsSourceCollectionChanged;
            }

            if (newItemsSource is INotifyCollectionChanged)
            {
                (newItemsSource as INotifyCollectionChanged).CollectionChanged += ItemsSourceCollectionChanged;

            }

            // Clear the previous items in the Children property
            this.ClearItemsSource();

            // Add the new children
            foreach (UIElement child in newItemsSource)
            {
                Children.Add(child);
            }

            isInitializeArrangeRequired = true;

            InvalidateVisual();
        }

        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    fluidElements.Remove((UIElement)item);
                }
            }
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    // Check if the child is already added to the fluidElements collection
                    if (!fluidElements.Contains((UIElement)item))
                    {
                        AddChildToFluidElements((UIElement)item);
                    }
                }
            }
        }

        #endregion

        #region Orientation

        /// <summary>
        /// Orientation Dependency Property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(VirtualizingFluidWrapPanel),
                new FrameworkPropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Gets or sets the Orientation property. This dependency property 
        /// indicates the orientation of arrangement of items in the panel.
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

        /// <summary>
        /// Handles changes to the Orientation property.
        /// </summary>
        /// <param name="d">VirtualizingFluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualizingFluidWrapPanel panel = (VirtualizingFluidWrapPanel)d;
            Orientation oldOrientation = (Orientation)e.OldValue;
            Orientation newOrientation = panel.Orientation;
            panel.OnOrientationChanged(oldOrientation, newOrientation);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Orientation property.
        /// </summary>
        /// <param name="oldOrientation">Old Value</param>
        /// <param name="newOrientation">New Value</param>
        protected virtual void OnOrientationChanged(Orientation oldOrientation, Orientation newOrientation)
        {
            InvalidateVisual();
        }

        #endregion

        #region ScrollOffset
        //鼠标每一次滚动 UI上的偏移//htzk123
        public static readonly DependencyProperty ScrollOffsetProperty = DependencyProperty.RegisterAttached("ScrollOffset", typeof(int), typeof(VirtualizingFluidWrapPanel), new PropertyMetadata(10));
        public int ScrollOffset
        {
            get
            {
                return Convert.ToInt32(GetValue(ScrollOffsetProperty));
            }
            set
            {
                SetValue(ScrollOffsetProperty, value);
            }
        }
        #endregion

        #region
        public WrapPanelAlignment HorizontalContentAlignment
        {
            get
            {
                return (WrapPanelAlignment)GetValue(HorizontalContentAlignmentProperty);
            }
            set
            {
                SetValue(HorizontalContentAlignmentProperty, value);
            }
        }

        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.RegisterAttached(nameof(HorizontalContentAlignment), typeof(WrapPanelAlignment), typeof(VirtualizingFluidWrapPanel), new FrameworkPropertyMetadata(WrapPanelAlignment.Left, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
        #endregion
        #endregion

        #region Overrides

        /// <summary>
        /// Override for the Measure Layout Phase
        /// </summary>
        /// <param name="availableSize">Available Size</param>
        /// <returns>Size required by the panel</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            try
            {
                this.UpdateScrollInfo(availableSize);

                // Figure out range that's visible based on layout algorithm
                int firstVisibleItemIndex = 0;
                int lastVisibleItemIndex = 0;
                GetVisibleRange(ref firstVisibleItemIndex, ref lastVisibleItemIndex);

                // We need to access InternalChildren before the generator to work around a bug
                UIElementCollection children = this.InternalChildren;
                IItemContainerGenerator generator = this.ItemContainerGenerator;

                // Get the generator position of the first visible data item
                GeneratorPosition startPos = generator.GeneratorPositionFromIndex(firstVisibleItemIndex);

                // Get index where we'd insert the child for this position. If the item is realized
                // (position.Offset == 0), it's just position.Index, otherwise we have to add one to
                // insert after the corresponding child
                int childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;

                using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
                {
                    int itemIndex = firstVisibleItemIndex;
                    while (itemIndex <= lastVisibleItemIndex)
                    {
                        bool newlyRealized = false;

                        // Get or create the child
                        UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
                        if (newlyRealized)
                        {
                            // Figure out if we need to insert the child at the end or somewhere in the middle
                            if (childIndex >= children.Count)
                            {
                                base.AddInternalChild(child);

                                //Check if the child is already added to the fluidElements collection
                                if (!fluidElements.Contains(child))
                                {
                                    AddChildToFluidElements(child);
                                }
                            }
                            else
                            {
                                base.InsertInternalChild(childIndex, child);

                                //Check if the child is already added to the fluidElements collection
                                if (!fluidElements.Contains(child))
                                {
                                    InsertChildToFluidElements(childIndex, child);
                                }
                            }
                            generator.PrepareItemContainer(child);
                        }
                        else
                        {
                            // The child has already been created, let's be sure it's in the right spot
                            Debug.Assert(child.Equals(children[childIndex]), "Wrong child was generated");
                        }

                        //Measurements will depend on layout algorithm
                        //child.Measure(GetChildSize());
                        Size availableItemSize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
                        child.Measure(availableItemSize);

                        itemIndex += 1;
                        childIndex += 1;
                    }
                }


                // Note: this could be deferred to idle time for efficiency
                CleanUpItems(firstVisibleItemIndex, lastVisibleItemIndex);

                //for (int i = 0; i < InternalChildren.Count; i++)
                //{
                //    UIElement child = InternalChildren[i];
                //    if (child != null)
                //    {
                //        //Check if the child is already added to the fluidElements collection
                //        if (!fluidElements.Contains(child))
                //        {
                //            AddChildToFluidElements(child);
                //        }
                //    }
                //}

                List<UIElement> dirtyElements = new List<UIElement>();
                foreach (var element in fluidElements)
                {
                    if (!InternalChildren.Contains(element))
                    {
                        dirtyElements.Add(element);
                    }
                }

                foreach (var item in dirtyElements)
                {
                    fluidElements.Remove(item);
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                // No idea if we can ignore this
            }
            catch (NullReferenceException)
            {
                // No idea if we can ignore this
            }

            // Guard against possible infinity if exiting measure early
            return new Size(double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width, double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height);
        }

        /// <summary>
        /// Override for the Arrange Layout Phase
        /// </summary>
        /// <param name="finalSize">Available size provided by the VirtualizingFluidWrapPanel</param>
        /// <returns>Size taken up by the Panel</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            //IItemContainerGenerator generator = this.ItemContainerGenerator;

            //UpdateScrollInfo(finalSize);

            //for (int i = 0; i <= this.Children.Count - 1; i++)
            //{
            //    UIElement child = this.Children[i];

            //    // Map the child offset to an item offset
            //    int itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));

            //    ArrangeChild(itemIndex, child, finalSize);
            //}

            //return finalSize;

            UpdateScrollInfo(finalSize);

            if (layoutManager == null)
                layoutManager = new FluidLayoutManager();

            //Initialize the LayoutManager
            layoutManager.Initialize(finalSize.Width, finalSize.Height, ItemWidth, ItemHeight, Orientation);

            bool isEasingRequired = !isInitializeArrangeRequired;

            //If the children are newly added, then set their initial location before the panel loads
            if ((isInitializeArrangeRequired) && (this.Children.Count > 0))
            {
                InitializeArrange();
                isInitializeArrangeRequired = false;
            }

            //Update the Layout
            UpdateFluidLayout(isEasingRequired);

            //Return the size taken up by the Panel's Children
            //return layoutManager.GetArrangedSize(fluidElements.Count, finalSize);//htzk123虚拟化后，删除

            return finalSize;
        }
        #endregion

        #region Construction / Initialization

        /// <summary>
        /// Ctor
        /// </summary>
        public VirtualizingFluidWrapPanel()
        {
            this.RenderTransform = trans;//构造函数
            this.easingFunction = new SineEase() { EasingMode = EasingMode.EaseOut };
            this.transAnimation = new DoubleAnimation()
            {
                Duration = Constants.SmoothScrollingDuration,
                EasingFunction = this.easingFunction,
                FillBehavior = FillBehavior.Stop
            };
            fluidElements = new List<UIElement>();
            layoutManager = new FluidLayoutManager();
            isInitializeArrangeRequired = true;
        }

        #endregion       

        #region Helpers

        /// <summary>
        /// Adds the child to the fluidElements collection and initializes its RenderTransform.
        /// </summary>
        /// <param name="child">UIElement</param>
        private void AddChildToFluidElements(UIElement child)
        {
            // Add the child to the fluidElements collection
            fluidElements.Add(child);
            // Initialize its RenderTransform
            child.RenderTransform = layoutManager.CreateTransform(-ItemWidth, -ItemHeight, NORMAL_SCALE, NORMAL_SCALE);
        }

        /// <summary>
        /// Adds the child to the fluidElements collection and initializes its RenderTransform.
        /// </summary>
        /// <param name="child">UIElement</param>
        private void InsertChildToFluidElements(int index, UIElement child)
        {
            // Add the child to the fluidElements collection
            fluidElements.Insert(index, child);
            // Initialize its RenderTransform
            child.RenderTransform = layoutManager.CreateTransform(-ItemWidth, -ItemHeight, NORMAL_SCALE, NORMAL_SCALE);
        }

        /// <summary>
        /// Intializes the arrangement of the children
        /// </summary>
        private void InitializeArrange()
        {
            foreach (UIElement child in fluidElements)
            {
                // Get the child's index in the fluidElements
                int index = fluidElements.IndexOf(child);

                // Get the initial location of the child
                Point pos = layoutManager.GetInitialLocationOfChild(index);

                // Initialize the appropriate Render Transform for the child
                child.RenderTransform = layoutManager.CreateTransform(pos.X, pos.Y, NORMAL_SCALE, NORMAL_SCALE);
            }
        }

        /// <summary>
        /// Iterates through all the fluid elements and animate their
        /// movement to their new location.
        /// </summary>
        private void UpdateFluidLayout(bool showEasing = true)
        {
            // Iterate through all the fluid elements and animate their
            // movement to their new location.
            var validElement = fluidElements.Where(o => o != null && o.Visibility == Visibility.Visible).ToList();

            for (int index = 0; index < validElement.Count; index++)
            {
                UIElement element = validElement[index];
                //if (element == null||element.Visibility==Visibility.Collapsed)
                //    continue;

                // If an child is currently being dragged, then no need to animate it
                if (dragElement != null && index == validElement.IndexOf(dragElement))
                    continue;

                element.Arrange(new Rect(0, 0, element.DesiredSize.Width,
                      element.DesiredSize.Height));

                // Get the cell position of the current index
                Point pos = layoutManager.GetPointFromIndex(index);
                pos.Y += ((int)(VerticalOffset / ItemHeight)) * ItemHeight;//htzk123,虚拟化修正pos，当VerticalOffset > ItemHeight时虚拟化启动，偏移需加被虚拟化的ItemHeight

                Storyboard transition;
                // Is the child being animated the same as the child which was last dragged?
                if (element == lastDragElement)
                {
                    if (!showEasing)
                    {
                        // Create the Storyboard for the transition
                        transition = layoutManager.CreateTransition(element, pos, FIRST_TIME_ANIMATION_DURATION, null);
                    }
                    else
                    {
                        // Is easing function specified for the animation?
                        TimeSpan duration = (DragEasing != null) ? DEFAULT_ANIMATION_TIME_WITH_EASING : DEFAULT_ANIMATION_TIME_WITHOUT_EASING;
                        // Create the Storyboard for the transition
                        transition = layoutManager.CreateTransition(element, pos, duration, DragEasing);
                    }

                    // When the user releases the drag child, it's Z-Index is set to 1 so that 
                    // during the animation it does not go below other elements.
                    // After the animation has completed set its Z-Index to 0
                    transition.Completed += (s, e) => {
                        if (lastDragElement != null)
                        {
                            lastDragElement.SetValue(Canvas.ZIndexProperty, 0);
                            lastDragElement = null;
                        }
                    };
                }
                else // It is a non-dragElement
                {
                    if (!showEasing)
                    {
                        // Create the Storyboard for the transition
                        transition = layoutManager.CreateTransition(element, pos, FIRST_TIME_ANIMATION_DURATION, null);
                    }
                    else
                    {
                        // Is easing function specified for the animation?
                        TimeSpan duration = (ElementEasing != null) ? DEFAULT_ANIMATION_TIME_WITH_EASING : DEFAULT_ANIMATION_TIME_WITHOUT_EASING;
                        // Create the Storyboard for the transition
                        transition = layoutManager.CreateTransition(element, pos, duration, ElementEasing);
                    }
                }

                // Start the animation
                transition.Begin();
            }
        }

        /// <summary>
        /// Moves the dragElement to the new Index
        /// </summary>
        /// <param name="newIndex">Index of the new location</param>
        /// <returns>True-if dragElement was moved otherwise False</returns>
        private bool UpdateDragElementIndex(int newIndex)
        {
            // Check if the dragElement is being moved to its current place
            // If yes, then no need to proceed further. (Improves efficiency!)
            int dragCellIndex = fluidElements.IndexOf(dragElement);
            if (dragCellIndex == newIndex)
                return false;

            fluidElements.RemoveAt(dragCellIndex);
            fluidElements.Insert(newIndex, dragElement);

            //int childrenPerRow = CalculateChildrenPerRow(this.extent);//没有成功
            //var posIndex = ((int)(VerticalOffset / ItemHeight)) * childrenPerRow;

            ////this.Children.RemoveAt(dragCellIndex + posIndex);
            ////this.Children.Insert(newIndex+ posIndex, dragElement);
            //IItemContainerGenerator generator = this.ItemContainerGenerator;
            //GeneratorPosition childGeneratorPos = new GeneratorPosition(dragCellIndex, 0);
            //int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);
            //generator.Remove(childGeneratorPos, 1);
            //this.RemoveInternalChildRange(dragCellIndex, 1);
            //this.InsertInternalChild(newIndex, dragElement);
            //generator.PrepareItemContainer(dragElement);

            return true;
        }

        /// <summary>
        /// Removes all the children from the VirtualizingFluidWrapPanel
        /// </summary>
        private void ClearItemsSource()
        {
            fluidElements.Clear();
            Children.Clear();
        }

        #endregion

        #region FluidDrag Event Handlers

        /// <summary>
        /// Handler for the event when the user starts dragging the dragElement.
        /// </summary>
        /// <param name="child">UIElement being dragged</param>
        /// <param name="position">Position in the child where the user clicked</param>
        public void BeginFluidDrag(UIElement child, Point position)
        {
            if ((child == null) || (!IsComposing))
                return;

            // Call the event handler core on the Dispatcher. (Improves efficiency!)
            Dispatcher.BeginInvoke(new Action(() => {
                child.Opacity = DragOpacity;
                child.SetValue(Canvas.ZIndexProperty, Z_INDEX_DRAG);
                // Capture further mouse events
                child.CaptureMouse();
                dragElement = child;
                lastDragElement = null;

                // Since we are scaling the dragElement by DragScale, the clickPoint also shifts
                dragStartPoint = new Point(position.X * DragScale, position.Y * DragScale);
            }));
        }

        /// <summary>
        /// Handler for the event when the user drags the dragElement.
        /// </summary>
        /// <param name="child">UIElement being dragged</param>
        /// <param name="position">Position where the user clicked w.r.t. the UIElement being dragged</param>
        /// <param name="positionInParent">Position where the user clicked w.r.t. the VirtualizingFluidWrapPanel (the parentFWPanel of the UIElement being dragged</param>
        public void FluidDrag(UIElement child, Point position, Point positionInParent)
        {
            if ((child == null) || (!IsComposing))
                return;

            // Call the event handler core on the Dispatcher. (Improves efficiency!)
            Dispatcher.BeginInvoke(new Action(() => {
                if ((dragElement != null) && (layoutManager != null))
                {
                    dragElement.RenderTransform = layoutManager.CreateTransform(positionInParent.X - dragStartPoint.X,
                                                                                  positionInParent.Y - dragStartPoint.Y,
                                                                                  DragScale,
                                                                                  DragScale);

                    // Get the index in the fluidElements list corresponding to the current mouse location
                    Point currentPt = positionInParent;
                    int index = layoutManager.GetIndexFromPoint(currentPt);

                    // If no valid cell index is obtained, add the child to the end of the 
                    // fluidElements list.
                    if ((index == -1) || (index >= fluidElements.Count))
                    {
                        index = fluidElements.Count - 1;
                    }

                    // If the dragElement is moved to a new location, then only
                    // call the updation of the layout.
                    if (UpdateDragElementIndex(index))
                    {
                        UpdateFluidLayout();
                    }
                }
            }));
        }

        /// <summary>
        /// Handler for the event when the user stops dragging the dragElement and releases it.
        /// </summary>
        /// <param name="child">UIElement being dragged</param>
        /// <param name="position">Position where the user clicked w.r.t. the UIElement being dragged</param>
        /// <param name="positionInParent">Position where the user clicked w.r.t. the VirtualizingFluidWrapPanel (the parentFWPanel of the UIElement being dragged</param>
        public void EndFluidDrag(UIElement child, Point position, Point positionInParent)
        {
            if ((child == null) || (!IsComposing))
                return;

            // Call the event handler core on the Dispatcher. (Improves efficiency!)
            Dispatcher.BeginInvoke(new Action(() => {
                if ((dragElement != null) && (layoutManager != null))
                {
                    dragElement.RenderTransform = layoutManager.CreateTransform(positionInParent.X - dragStartPoint.X,
                                                                                positionInParent.Y - dragStartPoint.Y,
                                                                                DragScale,
                                                                                DragScale);

                    child.Opacity = NORMAL_OPACITY;
                    // Z-Index is set to 1 so that during the animation it does not go below other elements.
                    child.SetValue(Canvas.ZIndexProperty, Z_INDEX_INTERMEDIATE);
                    // Release the mouse capture
                    child.ReleaseMouseCapture();

                    // Reference used to set the Z-Index to 0 during the UpdateFluidLayout
                    lastDragElement = dragElement;

                    dragElement = null;
                }

                UpdateFluidLayout();
            }));
        }

        #endregion

        #region my123
        /// <summary>
        /// When items are removed, remove the corresponding UI if necessary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    break;
                case NotifyCollectionChangedAction.Move:
                    RemoveInternalChildRange(args.OldPosition.Index, args.ItemUICount);
                    break;
            }
        }

        /// <summary>
        /// Makes sure the Vertical scroll Offset is updated when the size changes.
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.SetVerticalOffset(this.VerticalOffset);
        }

        /// <summary>
        /// Makes sure that the Vertical scroll Offset is reset to 0 when all children are removed.
        /// </summary>
        protected override void OnClearChildren()
        {
            base.OnClearChildren();
            this.SetVerticalOffset(0);
        }

        protected override void BringIndexIntoView(int index)
        {
            if (index < 0 || index >= Children.Count)
            {
                return;
            }

            int childrenPerRow = CalculateChildrenPerRow(RenderSize);
            int row = index / childrenPerRow;
            SetVerticalOffset(row * ItemHeight);
        }

        /// <summary>
        /// Revirtualize items that are no longer visible,将不在可视区域内的item 移除
        /// </summary>
        /// <param name="minDesiredGenerated">first item index that should be visible</param>
        /// <param name="maxDesiredGenerated">last item index that should be visible</param>
        private void CleanUpItems(int minDesiredGenerated, int maxDesiredGenerated)
        {
            UIElementCollection children = this.InternalChildren;
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            int itemCount = GetItemCount();
            int childrenPerRow = CalculateChildrenPerRow(this.extent);
            //if (minDesiredGenerated - 2 * childrenPerRow > 0)//去掉上两行的缓冲区,改为1行
            //    minDesiredGenerated -= 2 * childrenPerRow;
            //if (maxDesiredGenerated + 2 * childrenPerRow < itemCount)//去掉下两行的缓冲区
            //    maxDesiredGenerated += 2 * childrenPerRow;

            for (int i = children.Count - 1; i >= 0; i--)
            {
                GeneratorPosition childGeneratorPos = new GeneratorPosition(i, 0);
                int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);
                //if ((itemIndex > 2 * childrenPerRow - 1 && itemIndex < minDesiredGenerated) ||
                //    (itemIndex < itemCount - 2 * childrenPerRow - 1 && itemIndex > maxDesiredGenerated))
                if (itemIndex < minDesiredGenerated || itemIndex > maxDesiredGenerated)
                {
                    generator.Remove(childGeneratorPos, 1);
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

        // I've isolated the layout specific code to this region. If you want to do something other than tiling, this is
        // where you'll make your changes

        /// <summary>
        /// width不超过availableSize的情况下，自身实际需要的Size(高度可能会超出availableSize)
        /// </summary>
        /// <param name="availableSize">available size</param>
        /// <param name="itemCount">number of data items</param>
        /// <returns></returns>
        private Size CalculateExtent(Size availableSize, int itemCount)
        {
            int childrenPerRow = CalculateChildrenPerRow(availableSize);//现有宽度下 一行可以最多容纳多少个

            // See how big we are
            return new Size(childrenPerRow * this.ItemWidth, this.ItemHeight * Math.Ceiling(Convert.ToDouble(itemCount) / childrenPerRow));
        }

        /// <summary>
        /// Get the range of children that are visible
        /// 更新滚动条，获取所有item，在可视区域内第一个item和最后一个item的索引
        /// </summary>
        /// <param name="firstVisibleItemIndex">The item index of the first visible item</param>
        /// <param name="lastVisibleItemIndex">The item index of the last visible item</param>
        private void GetVisibleRange(ref int firstVisibleItemIndex, ref int lastVisibleItemIndex)
        {
            int childrenPerRow = CalculateChildrenPerRow(this.extent);

            try
            {
                firstVisibleItemIndex = Convert.ToInt32(Math.Floor(this.offset.Y / this.ItemHeight)) * childrenPerRow;
                lastVisibleItemIndex = Convert.ToInt32(Math.Ceiling((this.offset.Y + this.viewport.Height) / this.ItemHeight)) * childrenPerRow - 1;

                int itemCount = GetItemCount();
                if (lastVisibleItemIndex >= itemCount)
                {
                    lastVisibleItemIndex = itemCount - 1;
                }
            }
            catch (OverflowException)
            {
                // No idea if we can ignore this
            }
        }

        /// <summary>
        /// Get the size of the children. We assume they are all the same
        /// </summary>
        /// <returns>The size</returns>
        private Size GetChildSize()
        {
            return new Size(this.ItemWidth, this.ItemHeight);
        }

        /// <summary>
        /// Position a child
        /// </summary>
        /// <param name="itemIndex">The data item index of the child</param>
        /// <param name="child">The element to position</param>
        /// <param name="finalSize">The size of the panel</param>
        private void ArrangeChild(int itemIndex, UIElement child, Size finalSize)
        {
            int childrenPerRow = CalculateChildrenPerRow(finalSize);

            int row = itemIndex / childrenPerRow;
            int column = itemIndex % childrenPerRow;

            double xCoordForItem = 0;
            if (HorizontalContentAlignment == WrapPanelAlignment.Left)
            {
                xCoordForItem = column * this.ItemWidth;
            }
            else // alignment is WrapPanelAlignment.Center or WrapPanelAlignment.Right
            {
                if (childrenPerRow > this.Children.Count)
                {
                    childrenPerRow = this.Children.Count;
                }
                double widthOfRow = childrenPerRow * this.ItemWidth;
                double startXForRow = finalSize.Width - widthOfRow;
                if (HorizontalContentAlignment == WrapPanelAlignment.Center)
                {
                    startXForRow /= 2;
                }
                xCoordForItem = startXForRow + (column * this.ItemWidth);
            }

            child.Arrange(new Rect(xCoordForItem, row * this.ItemHeight, this.ItemWidth, this.ItemHeight));
            //child.Arrange(new Rect(xCoordForItem, row * this.ItemHeight, child.DesiredSize.Width, child.DesiredSize.Height));
        }

        /// <summary>
        /// Helper function for tiling layout
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns></returns>
        private int CalculateChildrenPerRow(Size availableSize)
        {
            // Figure out how many children fit on each row
            int childrenPerRow = 0;

            if (availableSize.Width == double.PositiveInfinity)
            {
                childrenPerRow = this.Children.Count;
            }
            else
            {
                try
                {
                    childrenPerRow = Math.Max(1, Convert.ToInt32(Math.Floor(availableSize.Width / this.ItemWidth)));
                }
                catch (OverflowException)
                {
                    // No idea if we can ignore this
                }
            }
            return childrenPerRow;
        }

        private int GetItemCount()
        {
            // See how many items there are
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            int itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;

            return itemCount;
        }

        //更新滚动条
        // See Ben Constable's series of posts at http://blogs.msdn.com/bencon/
        private void UpdateScrollInfo(Size availableSize)
        {
            int itemCount = GetItemCount();

            Size extent = CalculateExtent(availableSize, itemCount);//extent 自己实际需要
            // Update extent
            if (extent != this.extent)
            {
                this.extent = extent;
                if (this.owner != null)
                {
                    this.owner.InvalidateScrollInfo();
                }
            }

            // Update viewport
            if (availableSize != this.viewport)
            {
                this.viewport = availableSize;
                if (this.owner != null)
                {
                    this.owner.InvalidateScrollInfo();
                }
            }
        }

        public ScrollViewer ScrollOwner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

        public bool CanHorizontallyScroll
        {
            get
            {
                return this.canHScroll;
            }
            set
            {
                this.canHScroll = value;
            }
        }

        public bool CanVerticallyScroll
        {
            get
            {
                return this.canVScroll;
            }
            set
            {
                this.canVScroll = value;
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return this.offset.X;
            }
        }

        public double VerticalOffset
        {
            get
            {
                return this.offset.Y;
            }
        }

        public double ExtentHeight
        {
            get
            {
                return this.extent.Height;
            }
        }

        public double ExtentWidth
        {
            get
            {
                return this.extent.Width;
            }
        }

        public double ViewportHeight
        {
            get
            {
                return this.viewport.Height;
            }
        }

        public double ViewportWidth
        {
            get
            {
                return this.viewport.Width;
            }
        }

        public void LineUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - (this.ScrollOffset > 0 ? this.ScrollOffset : this.ItemHeight));
        }

        public void LineDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + (this.ScrollOffset > 0 ? this.ScrollOffset : this.ItemHeight));
        }

        public void PageUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.viewport.Height);
        }

        public void PageDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.viewport.Height);
        }

        public void MouseWheelUp()
        {
            this.EasingSetVerticalOffset(this.VerticalOffset - (this.ScrollOffset > 0 ? this.ScrollOffset : this.ItemHeight));
        }

        public void MouseWheelDown()
        {
            this.EasingSetVerticalOffset(this.VerticalOffset + (this.ScrollOffset > 0 ? this.ScrollOffset : this.ItemHeight));
        }

        public void LineLeft()
        {
            throw new InvalidOperationException();
        }

        public void LineRight()
        {
            throw new InvalidOperationException();
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return new Rect();
        }

        public void MouseWheelLeft()
        {
            throw new InvalidOperationException();
        }

        public void MouseWheelRight()
        {
            throw new InvalidOperationException();
        }

        public void PageLeft()
        {
            throw new InvalidOperationException();
        }

        public void PageRight()
        {
            throw new InvalidOperationException();
        }

        public void SetHorizontalOffset(double offset)
        {
            throw new InvalidOperationException();
        }

        public void EasingSetVerticalOffset(double offset)
        {
            AdjustVerticalOffset(ref offset);

            transAnimation.From = trans.Y;
            transAnimation.To = -offset;
            this.trans.BeginAnimation(TranslateTransform.YProperty, transAnimation);
            this.trans.Y = -offset;

            // Force us to realize the correct children
            this.InvalidateMeasure();
        }

        public void SetVerticalOffset(double offset)
        {
            AdjustVerticalOffset(ref offset);

            this.trans.Y = -offset;

            // Force us to realize the correct children
            this.InvalidateMeasure();
        }

        private void AdjustVerticalOffset(ref double offset)
        {
            if (offset < 0 || this.viewport.Height >= this.extent.Height)
            {
                offset = 0;
            }
            else
            {
                if (offset + this.viewport.Height >= this.extent.Height)
                {
                    offset = this.extent.Height - this.viewport.Height;
                }
            }

            this.offset.Y = offset;

            if (this.owner != null)
            {
                this.owner.InvalidateScrollInfo();
            }
        }
        #endregion
    }

}