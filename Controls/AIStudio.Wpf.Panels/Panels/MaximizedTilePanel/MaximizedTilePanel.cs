using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AIStudio.Wpf.Panels.Helpers;

namespace AIStudio.Wpf.Panels
{
    public class MaximizedTilePanel : Panel
    {
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
            DependencyProperty.Register("MaximizedTileLocation", typeof(object), typeof(MaximizedTilePanel), new FrameworkPropertyMetadata(Location.Left, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(MaximizedTileLocationChangedCallback)));

        static void MaximizedTileLocationChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as MaximizedTilePanel).MaximizedTileState = TileState.Minimized;
        }

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

        public static readonly DependencyProperty MaximizedRatioProperty =
            DependencyProperty.Register("MaximizedRatio", typeof(object), typeof(MaximizedTilePanel), new FrameworkPropertyMetadata(0.7, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(MaximizedRatioChangedCallback)));

        static void MaximizedRatioChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }

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
            DependencyProperty.Register("TileMargin", typeof(Thickness), typeof(MaximizedTilePanel), new FrameworkPropertyMetadata(new Thickness(2), FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// 可调整元素为最大
        /// </summary>
        public bool AutoMaximizedLocation
        {
            get
            {
                return (bool)GetValue(FixMaximizedLocationProperty);
            }
            set
            {
                SetValue(FixMaximizedLocationProperty, value);
            }
        }
        /// <summary>
        /// 可调整元素为最大
        /// </summary>
        public static readonly DependencyProperty FixMaximizedLocationProperty =
            DependencyProperty.Register("AutoMaximizedLocation", typeof(bool), typeof(MaximizedTilePanel), new FrameworkPropertyMetadata(true));

        private FrameworkElement ActiveTile;
        public TileState MaximizedTileState { get; set; } = TileState.Minimized;
        #region Overrides

        protected override Size MeasureOverride(Size constraint)
        {
            var sz = base.MeasureOverride(constraint);

            if (!AutoMaximizedLocation)
            {
                if (MaximizedTileLocation == Location.Top || MaximizedTileLocation == Location.Left)
                {
                    if (this.Children.Count > 0)
                        ActiveTile = this.Children[0] as FrameworkElement;
                }
                else if (MaximizedTileLocation == Location.Bottom || MaximizedTileLocation == Location.Right)
                {
                    if (this.Children.Count > 0)
                        ActiveTile = this.Children[this.Children.Count - 1] as FrameworkElement;
                }
            }
            else
            {
                if (this.Children.Count > 0)
                    ActiveTile = this.Children.OfType<FrameworkElement>().Where(p => p.EqualsPropertyValue("TileState", TileState.Maximized)).OrderByDescending(p => p.GetPropertyValue("TileTime")).FirstOrDefault();
            }

            if (ActiveTile == null)
            {
                NormalMeasureOverride(constraint);
                MaximizedTileState = TileState.Normal;
            }
            else
            {
                MaximizedTileMeasureOverride(constraint);
                MaximizedTileState = TileState.Maximized;
            }

            return sz;
        }

        private void MaximizedTileMeasureOverride(Size constraint)
        {
            if (AutoMaximizedLocation)
            {
                if (!ActiveTile.EqualsPropertyValue("TileState", TileState.Maximized))
                    ActiveTile.SetPropertyValue("TileState", TileState.Maximized);

                this.Children.OfType<FrameworkElement>().Where(p => p != ActiveTile && p.EqualsPropertyValue("TileState", TileState.Maximized)).ToList().ForEach(p => {
                    //重置上一个最大化元素大小
                    p.SetPropertyValue("TileState", TileState.Normal);
                    if (MaximizedTileLocation == Location.Top || MaximizedTileLocation == Location.Bottom)
                    {
                        p.Height = double.NaN;
                        p.Width = MDIItem.GetNormalWidth(p);
                    }
                    else if (MaximizedTileLocation == Location.Left || MaximizedTileLocation == Location.Right)
                    {
                        p.Width = double.NaN;
                        p.Height = MDIItem.GetNormalHeight(p);
                    }
                });

                //布局改变，就重置元素大小
                if (MaximizedTileState != TileState.Maximized)
                {
                    if (MaximizedTileLocation == Location.Top || MaximizedTileLocation == Location.Bottom)
                    {
                        this.Children.OfType<FrameworkElement>().ToList().ForEach(p => {
                            p.Height = double.NaN;
                            if (!p.EqualsPropertyValue("TileState", TileState.Maximized))
                            {
                                p.Width = MDIItem.GetNormalWidth(p);
                            }
                        });
                    }
                    else if (MaximizedTileLocation == Location.Left || MaximizedTileLocation == Location.Right)
                    {
                        this.Children.OfType<FrameworkElement>().ToList().ForEach(p => {
                            p.Width = double.NaN;
                            if (!p.EqualsPropertyValue("TileState", TileState.Maximized))
                            {
                                p.Height = MDIItem.GetNormalHeight(p);
                            }
                        });
                    }
                }
            }

            if (MaximizedTileLocation == Location.Top || MaximizedTileLocation == Location.Bottom)
            {

                double availableWidth = constraint.Width - 2 * (TileMargin.Left + TileMargin.Right);
                double availableHeight = constraint.Height - 3 * (TileMargin.Top + TileMargin.Bottom);

                double activeHeight = MaximizedRatio * availableHeight;
                double inactiveHeight = availableHeight - activeHeight;
                //double inactiveWidth = (availableWidth - ((this.Children.Count - 2) * PADDING)) / (this.Children.Count - 1);

                foreach (FrameworkElement tile in this.Children)
                {
                    tile.Measure(
                        new Size(
                        tile == ActiveTile ? availableWidth : constraint.Width,//inactiveWidth,
                        tile == ActiveTile ? activeHeight : inactiveHeight
                        ));
                }
            }
            else if (MaximizedTileLocation == Location.Left || MaximizedTileLocation == Location.Right)
            {
                double availableWidth = constraint.Width - 3 * (TileMargin.Left + TileMargin.Right);
                double availableHeight = constraint.Height - 2 * (TileMargin.Top + TileMargin.Bottom);

                double activeWidth = MaximizedRatio * availableWidth;
                double inactiveWidth = availableWidth - activeWidth;
                //double inactiveHeight = (availableHeight - ((this.Children.Count - 2) * PADDING)) / (this.Children.Count - 1);

                foreach (FrameworkElement tile in this.Children)
                {
                    tile.Measure(
                        new Size(
                        tile == ActiveTile ? activeWidth : inactiveWidth,
                        tile == ActiveTile ? availableHeight : constraint.Height//inactiveHeight
                        ));
                }
            }
        }

        private void NormalMeasureOverride(Size constraint)
        {
            //布局改变，就重置元素大小
            if (MaximizedTileState != TileState.Normal)
            {
                this.Children.OfType<FrameworkElement>().ToList().ForEach(p => {
                    p.Width = MDIItem.GetNormalWidth(p);
                    p.Height = MDIItem.GetNormalHeight(p);
                });
            }

            foreach (UIElement element in this.Children)
            {
                element.Measure(constraint);
            }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (ActiveTile == null)
            {
                NormalArrangeOverride(arrangeBounds);
            }
            else
            {
                MaximizedTileArrangeOverride(arrangeBounds);
            }

            return arrangeBounds;
        }

        private void MaximizedTileArrangeOverride(Size arrangeBounds)
        {
            if (MaximizedTileLocation == Location.Top || MaximizedTileLocation == Location.Bottom)
            {
                double availableWidth = arrangeBounds.Width - 2 * (TileMargin.Left + TileMargin.Right);
                double availableHeight = arrangeBounds.Height - 3 * (TileMargin.Top + TileMargin.Bottom);

                double activeHeight = MaximizedRatio * availableHeight;
                double inactiveHeight = availableHeight - activeHeight;
                //double inactiveWidth = (availableWidth - ((this.Children.Count - 2) * PADDING)) / (this.Children.Count - 1);

                double x = TileMargin.Left + TileMargin.Right;
                double y = 0;
                if (MaximizedTileLocation == Location.Top)
                {
                    y = 2 * (TileMargin.Top + TileMargin.Bottom) + activeHeight;
                }
                else if (MaximizedTileLocation == Location.Bottom)
                {
                    y = 2 * (TileMargin.Top + TileMargin.Bottom) + inactiveHeight;
                }

                foreach (FrameworkElement tile in this.Children)
                {
                    if (MaximizedTileLocation == Location.Top)
                    {
                        var rect = new Rect(
                            ActiveTile == tile ? TileMargin.Left + TileMargin.Right : x,
                            ActiveTile == tile ? TileMargin.Top + TileMargin.Bottom : y,
                            ActiveTile == tile ? availableWidth : tile.DesiredSize.Width,// inactiveWidth,
                            ActiveTile == tile ? activeHeight : inactiveHeight
                            );

                        tile.Arrange(rect);
                    }
                    else if (MaximizedTileLocation == Location.Bottom)
                    {
                        var rect = new Rect(
                            ActiveTile == tile ? TileMargin.Left + TileMargin.Right : x,
                            ActiveTile == tile ? y : TileMargin.Top + TileMargin.Bottom,
                            ActiveTile == tile ? availableWidth : tile.DesiredSize.Width,//inactiveWidth,
                            ActiveTile == tile ? activeHeight : inactiveHeight
                            );

                        tile.Arrange(rect);
                    }

                    if (ActiveTile != tile)
                        x += tile.DesiredSize.Width + TileMargin.Left + TileMargin.Right;//inactiveWidth + PADDING; 
                }
            }
            else if (MaximizedTileLocation == Location.Left || MaximizedTileLocation == Location.Right)
            {
                double availableWidth = arrangeBounds.Width - 3 * (TileMargin.Left + TileMargin.Right);
                double availableHeight = arrangeBounds.Height - 2 * (TileMargin.Top + TileMargin.Bottom);

                double activeWidth = MaximizedRatio * availableWidth;
                double inactiveWidth = availableWidth - activeWidth;
                //double inactiveHeight = (availableHeight - ((this.Children.Count - 2) * PADDING)) / (this.Children.Count - 1);

                double x = 0;
                double y = TileMargin.Top + TileMargin.Bottom;
                if (MaximizedTileLocation == Location.Left)
                {
                    x = 2 * (TileMargin.Left + TileMargin.Right) + activeWidth;
                }
                else if (MaximizedTileLocation == Location.Right)
                {
                    x = 2 * (TileMargin.Left + TileMargin.Right) + inactiveWidth;
                }

                foreach (FrameworkElement tile in Children)
                {
                    if (MaximizedTileLocation == Location.Left)
                    {
                        var rect = new Rect(
                            ActiveTile == tile ? (TileMargin.Left + TileMargin.Right) : x,
                            ActiveTile == tile ? (TileMargin.Top + TileMargin.Bottom) : y,
                            ActiveTile == tile ? activeWidth : inactiveWidth,
                            ActiveTile == tile ? availableHeight : tile.DesiredSize.Height//inactiveHeight
                            );

                        tile.Arrange(rect);
                    }
                    else if (MaximizedTileLocation == Location.Right)
                    {
                        var rect = new Rect(
                            ActiveTile == tile ? x : (TileMargin.Left + TileMargin.Right),
                            ActiveTile == tile ? (TileMargin.Top + TileMargin.Bottom) : y,
                            ActiveTile == tile ? activeWidth : inactiveWidth,
                            ActiveTile == tile ? availableHeight : tile.DesiredSize.Height//inactiveHeight
                            );

                        tile.Arrange(rect);
                    }

                    if (ActiveTile != tile)
                        y += tile.DesiredSize.Height + (TileMargin.Top + TileMargin.Bottom); // inactiveHeight + PADDING;
                }
            }
        }

        private void NormalArrangeOverride(Size arrangeBounds)
        {
            int firstInLine = 0;

            Size currentLineSize = new Size();

            double accumulatedHeight = TileMargin.Top;

            UIElementCollection elements = base.InternalChildren;
            for (int i = 0; i < elements.Count; i++)
            {

                Size desiredSize = elements[i].DesiredSize;
                desiredSize.Width += (TileMargin.Left + TileMargin.Right);
                desiredSize.Height += (TileMargin.Top + TileMargin.Bottom);

                if (currentLineSize.Width + desiredSize.Width > arrangeBounds.Width) //need to switch to another line
                {
                    arrangeLine(accumulatedHeight, currentLineSize.Height, firstInLine, i);

                    accumulatedHeight += currentLineSize.Height;
                    currentLineSize = desiredSize;

                    if (desiredSize.Width > arrangeBounds.Width) //the element is wider then the constraint - give it a separate line                    
                    {
                        arrangeLine(accumulatedHeight, desiredSize.Height, i, ++i);
                        accumulatedHeight += desiredSize.Height;
                        currentLineSize = new Size();
                    }
                    firstInLine = i;
                }
                else //continue to accumulate a line
                {
                    currentLineSize.Width += desiredSize.Width;
                    currentLineSize.Height = Math.Max(desiredSize.Height, currentLineSize.Height);
                }
            }

            if (firstInLine < elements.Count)
                arrangeLine(accumulatedHeight, currentLineSize.Height, firstInLine, elements.Count);
        }

        private void arrangeLine(double y, double lineHeight, int start, int end)
        {
            double x = TileMargin.Left;
            UIElementCollection children = InternalChildren;
            for (int i = start; i < end; i++)
            {
                UIElement child = children[i];
                child.Arrange(new Rect(x, y, child.DesiredSize.Width, lineHeight - (TileMargin.Top + TileMargin.Bottom)));
                x += (child.DesiredSize.Width + (TileMargin.Left + TileMargin.Right));
            }
        }
        #endregion


    }

    public enum Location
    {
        Left,
        Top,
        Right,
        Bottom,
    }
}
