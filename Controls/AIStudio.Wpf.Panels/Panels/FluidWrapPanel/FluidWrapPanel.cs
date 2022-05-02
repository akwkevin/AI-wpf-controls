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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AIStudio.Wpf.Panels
{
    /// <summary>
    /// Interactions for the FluidWrapPanel
    /// </summary>
    public class FluidWrapPanel : Panel, IFluidWrapPanel
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

        #endregion

        #region Fields

        Point dragStartPoint = new Point();
        UIElement dragElement = null;
        UIElement lastDragElement = null;
        //List<UIElement> FluidElements = null;
        FluidLayoutManager layoutManager = null;
        bool isInitializeArrangeRequired = false;

        #endregion

        #region Dependency Properties

        #region DragEasing

        /// <summary>
        /// DragEasing Dependency Property
        /// </summary>
        public static readonly DependencyProperty FluidElementsProperty =
            DependencyProperty.Register("FluidElements", typeof(List<UIElement>), typeof(FluidWrapPanel),
                new FrameworkPropertyMetadata(new List<UIElement>(), new PropertyChangedCallback(OnFluidElementsChanged)));

        /// <summary>
        /// Gets or sets the DragEasing property. This dependency property 
        /// indicates the Easing function to be used when the user stops dragging the child and releases it.
        /// </summary>
        public List<UIElement> FluidElements
        {
            get
            {
                return (List<UIElement>)GetValue(FluidElementsProperty);
            }
            set
            {
                SetValue(FluidElementsProperty, value);
            }
        }

        private static void OnFluidElementsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FluidWrapPanel fwPanel = (FluidWrapPanel)d;
            List<UIElement> oldItemWidth = (List<UIElement>)e.OldValue;
            List<UIElement> newItemWidth = (List<UIElement>)e.NewValue;
            fwPanel.OnFluidElementsChanged(oldItemWidth, newItemWidth);
        }

        protected virtual void OnFluidElementsChanged(List<UIElement> oldDragEasing, List<UIElement> newDragEasing)
        {

        }
        #endregion

        #region DragEasing

        /// <summary>
        /// DragEasing Dependency Property
        /// </summary>
        public static readonly DependencyProperty DragEasingProperty =
            DependencyProperty.Register("DragEasing", typeof(EasingFunctionBase), typeof(FluidWrapPanel),
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
        /// <param name="d">FluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnDragEasingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FluidWrapPanel panel = (FluidWrapPanel)d;
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
            DependencyProperty.Register("DragOpacity", typeof(double), typeof(FluidWrapPanel),
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
        /// <param name="d">FluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnDragOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FluidWrapPanel panel = (FluidWrapPanel)d;
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
            DependencyProperty.Register("DragScale", typeof(double), typeof(FluidWrapPanel),
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
        /// <param name="d">FluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnDragScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FluidWrapPanel panel = (FluidWrapPanel)d;
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
            DependencyProperty.Register("ElementEasing", typeof(EasingFunctionBase), typeof(FluidWrapPanel),
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
        /// <param name="d">FluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnElementEasingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FluidWrapPanel panel = (FluidWrapPanel)d;
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
            DependencyProperty.Register("IsComposing", typeof(bool), typeof(FluidWrapPanel),
                new FrameworkPropertyMetadata((new PropertyChangedCallback(OnIsComposingChanged))));

        /// <summary>
        /// Gets or sets the IsComposing property. This dependency property 
        /// indicates if the FluidWrapPanel is in Composing mode.
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
        /// <param name="d">FluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnIsComposingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FluidWrapPanel panel = (FluidWrapPanel)d;
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
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(FluidWrapPanel),
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
        /// <param name="d">FluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FluidWrapPanel fwPanel = (FluidWrapPanel)d;
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
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(FluidWrapPanel),
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
        /// <param name="d">FluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FluidWrapPanel fwPanel = (FluidWrapPanel)d;
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
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(FluidWrapPanel),
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
        /// <param name="d">FluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FluidWrapPanel panel = (FluidWrapPanel)d;
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
                    FluidElements.Remove((UIElement)item);
                }
            }
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    // Check if the child is already added to the FluidElements collection
                    if (!FluidElements.Contains((UIElement)item))
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
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FluidWrapPanel),
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
        /// <param name="d">FluidWrapPanel</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FluidWrapPanel panel = (FluidWrapPanel)d;
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

        #endregion

        #region Overrides

        /// <summary>
        /// Override for the Measure Layout Phase
        /// </summary>
        /// <param name="availableSize">Available Size</param>
        /// <returns>Size required by the panel</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size availableItemSize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
            double rowWidth = 0.0;
            double maxRowHeight = 0.0;
            double colHeight = 0.0;
            double maxColWidth = 0.0;
            double totalColumnWidth = 0.0;
            double totalRowHeight = 0.0;

            // Iterate through all the UIElements in the Children collection
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i];
                if (child != null)
                {
                    // Ask the child how much size it needs
                    child.Measure(availableItemSize);
                    //Check if the child is already added to the FluidElements collection
                    if (!FluidElements.Contains(child))
                    {
                        AddChildToFluidElements(child);
                    }

                    if (this.Orientation == Orientation.Horizontal)
                    {
                        // Will the child fit in the current row?
                        //if (rowWidth + child.DesiredSize.Width > availableSize.Width)
                        if (rowWidth + this.ItemWidth > availableSize.Width)
                        {
                            // Wrap to next row
                            totalRowHeight += maxRowHeight;

                            // Is the current row width greater than the previous row widths
                            if (rowWidth > totalColumnWidth)
                                totalColumnWidth = rowWidth;

                            rowWidth = 0.0;
                            maxRowHeight = 0.0;
                        }

                        //rowWidth += child.DesiredSize.Width;
                        rowWidth += this.ItemWidth;
                        //if (child.DesiredSize.Height > maxRowHeight)
                        //    maxRowHeight = child.DesiredSize.Height;
                        if (this.ItemHeight > maxRowHeight)
                            maxRowHeight = this.ItemHeight;
                    }
                    else // Vertical Orientation
                    {
                        // Will the child fit in the current column?
                        //if (colHeight + child.DesiredSize.Height > availableSize.Height)
                        if (colHeight + this.ItemHeight > availableSize.Height)
                        {
                            // Wrap to next column
                            totalColumnWidth += maxColWidth;

                            // Is the current column height greater than the previous column heights
                            if (colHeight > totalRowHeight)
                                totalRowHeight = colHeight;

                            colHeight = 0.0;
                            maxColWidth = 0.0;
                        }

                        //colHeight += child.DesiredSize.Height;
                        colHeight += this.ItemHeight;
                        //if (child.DesiredSize.Width > maxColWidth)
                        //    maxColWidth = child.DesiredSize.Width;
                        if (this.ItemWidth > maxColWidth)
                            maxColWidth = this.ItemWidth;
                    }
                }
            }

            List<UIElement> dirtyElements = new List<UIElement>();
            foreach (var element in FluidElements)
            {
                if (!InternalChildren.Contains(element))
                {
                    dirtyElements.Add(element);
                }
            }

            foreach (var item in dirtyElements)
            {
                FluidElements.Remove(item);
            }

            if (this.Orientation == Orientation.Horizontal)
            {
                // Add the height of the last row
                totalRowHeight += maxRowHeight;
                // If there is only one row, take its width as the total width
                if (totalColumnWidth == 0.0)
                {
                    totalColumnWidth = rowWidth;
                }
            }
            else
            {
                // Add the width of the last column
                totalColumnWidth += maxColWidth;
                // If there is only one column, take its height as the total height
                if (totalRowHeight == 0.0)
                {
                    totalRowHeight = colHeight;
                }
            }

            Size resultSize = new Size(totalColumnWidth, totalRowHeight);

            return resultSize;
        }

        /// <summary>
        /// Override for the Arrange Layout Phase
        /// </summary>
        /// <param name="finalSize">Available size provided by the FluidWrapPanel</param>
        /// <returns>Size taken up by the Panel</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (layoutManager == null)
                layoutManager = new FluidLayoutManager();

            // Initialize the LayoutManager
            layoutManager.Initialize(finalSize.Width, finalSize.Height, ItemWidth, ItemHeight, Orientation);

            bool isEasingRequired = !isInitializeArrangeRequired;

            // If the children are newly added, then set their initial location before the panel loads
            if ((isInitializeArrangeRequired) && (this.Children.Count > 0))
            {
                InitializeArrange();
                isInitializeArrangeRequired = false;
            }

            // Update the Layout
            UpdateFluidLayout(isEasingRequired);

            // Return the size taken up by the Panel's Children
            return layoutManager.GetArrangedSize(FluidElements.Count, finalSize);
        }

        #endregion

        #region Construction / Initialization

        /// <summary>
        /// Ctor
        /// </summary>
        public FluidWrapPanel()
        {
            if (FluidElements == null)
            {
                FluidElements = new List<UIElement>();
            }
            layoutManager = new FluidLayoutManager();
            isInitializeArrangeRequired = true;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Adds the child to the FluidElements collection and initializes its RenderTransform.
        /// </summary>
        /// <param name="child">UIElement</param>
        private void AddChildToFluidElements(UIElement child)
        {
            // Add the child to the FluidElements collection
            FluidElements.Add(child);
            // Initialize its RenderTransform
            child.RenderTransform = layoutManager.CreateTransform(-ItemWidth, -ItemHeight, NORMAL_SCALE, NORMAL_SCALE);
        }

        /// <summary>
        /// Intializes the arrangement of the children
        /// </summary>
        private void InitializeArrange()
        {
            foreach (UIElement child in FluidElements)
            {
                // Get the child's index in the FluidElements
                int index = FluidElements.IndexOf(child);

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
            var validElement = FluidElements.Where(o => o != null && o.Visibility == Visibility.Visible).ToList();

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
            int dragCellIndex = FluidElements.IndexOf(dragElement);
            if (dragCellIndex == newIndex)
                return false;

            FluidElements.RemoveAt(dragCellIndex);
            FluidElements.Insert(newIndex, dragElement);

            return true;
        }

        /// <summary>
        /// Removes all the children from the FluidWrapPanel
        /// </summary>
        private void ClearItemsSource()
        {
            FluidElements.Clear();
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
        /// <param name="positionInParent">Position where the user clicked w.r.t. the FluidWrapPanel (the parentFWPanel of the UIElement being dragged</param>
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

                    // Get the index in the FluidElements list corresponding to the current mouse location
                    Point currentPt = positionInParent;
                    int index = layoutManager.GetIndexFromPoint(currentPt);

                    // If no valid cell index is obtained, add the child to the end of the 
                    // FluidElements list.
                    if ((index == -1) || (index >= FluidElements.Count))
                    {
                        index = FluidElements.Count - 1;
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
        /// <param name="positionInParent">Position where the user clicked w.r.t. the FluidWrapPanel (the parentFWPanel of the UIElement being dragged</param>
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
    }
}
