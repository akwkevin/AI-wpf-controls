using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    public class WrapPanelFill : WrapPanel
    {
        // ******************************************************************
        private static readonly DependencyProperty UseToFillProperty = DependencyProperty.RegisterAttached("UseToFill", typeof(Boolean),
            typeof(WrapPanelFill), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        // ******************************************************************
        private static void SetUseToFill(UIElement element, Boolean value)
        {
            element.SetValue(UseToFillProperty, value);
        }
        // ******************************************************************
        private static Boolean GetUseToFill(UIElement element)
        {
            return (Boolean)element.GetValue(UseToFillProperty);
        }


        // ******************************************************************
        public static readonly DependencyProperty FillDefaultProperty = DependencyProperty.Register(nameof(FillDefault), typeof(Boolean),
            typeof(WrapPanelFill), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool FillDefault
        {
            get => (bool)GetValue(FillDefaultProperty);
            set => SetValue(FillDefaultProperty, value);
        }

        public bool CanWrap { get; set; } = true;

        // ******************************************************************
        const double DBL_EPSILON = 2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */

        // ******************************************************************
        private static bool DoubleAreClose(double value1, double value2)
        {
            //in case they are Infinities (then epsilon check does not work)
            if (value1 == value2) return true;
            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        // ******************************************************************
        private static bool DoubleGreaterThan(double value1, double value2)
        {
            return (value1 > value2) && !DoubleAreClose(value1, value2);
        }

        // ******************************************************************
        private bool _atLeastOneElementCanHasItsWidthExpanded = false;

        // ******************************************************************
        /// <summary> 
        /// <see cref="FrameworkElement.MeasureOverride"/>
        /// </summary> 
        protected override Size MeasureOverride(Size constraint)
        {
            XYSize curLineSize = new XYSize(Orientation);
            XYSize panelSize = new XYSize(Orientation);
            XYSize uvConstraint = new XYSize(Orientation, constraint.Width, constraint.Height);
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            if (CanWrap == false && FillDefault == true && !Double.IsNaN(constraint.Width))
            {
                itemWidth = constraint.Width / InternalChildren.Count;
            }

            bool itemWidthSet = !Double.IsNaN(itemWidth);
            bool itemHeightSet = !Double.IsNaN(itemHeight);

            Size childConstraint = new Size(
                (itemWidthSet ? itemWidth : constraint.Width),
                (itemHeightSet ? itemHeight : constraint.Height));

            UIElementCollection children = InternalChildren;

            // EO
            LineInfo currentLineInfo = new LineInfo(); // EO, the way it works it is always like we are on the current line
            _lineInfos.Clear();
            _atLeastOneElementCanHasItsWidthExpanded = false;

            for (int i = 0, count = children.Count; i < count; i++)
            {
                UIElement child = children[i] as UIElement;
                if (child == null) continue;

                //Flow passes its own constrint to children 
                child.Measure(childConstraint);

                //this is the size of the child in UV space 
                XYSize sz = new XYSize(
                    Orientation,
                    (itemWidthSet ? itemWidth : child.DesiredSize.Width),
                    (itemHeightSet ? itemHeight : child.DesiredSize.Height));

                if (DoubleGreaterThan(curLineSize.X + sz.X, uvConstraint.X) && CanWrap) //need to switch to another line 
                {
                    // EO
                    currentLineInfo.Size = curLineSize;
                    _lineInfos.Add(currentLineInfo);

                    panelSize.X = Math.Max(curLineSize.X, panelSize.X);
                    panelSize.Y += curLineSize.Y;
                    curLineSize = sz;

                    // EO
                    currentLineInfo = new LineInfo();
                    var feChild = child as FrameworkElement;
                    if (FillDefault || GetUseToFill(feChild))
                    {
                        currentLineInfo.ElementsWithNoWidthSet.Add(feChild);
                        _atLeastOneElementCanHasItsWidthExpanded = true;
                    }

                    if (DoubleGreaterThan(sz.X, uvConstraint.X) && CanWrap) //the element is wider then the constrint - give it a separate line
                    {
                        currentLineInfo = new LineInfo();

                        panelSize.X = Math.Max(sz.X, panelSize.X);
                        panelSize.Y += sz.Y;
                        curLineSize = new XYSize(Orientation);
                    }
                }
                else //continue to accumulate a line
                {
                    curLineSize.X += sz.X;
                    curLineSize.Y = Math.Max(sz.Y, curLineSize.Y);

                    // EO
                    var feChild = child as FrameworkElement;
                    if (FillDefault || GetUseToFill(feChild))
                    {
                        currentLineInfo.ElementsWithNoWidthSet.Add(feChild);
                        _atLeastOneElementCanHasItsWidthExpanded = true;
                    }
                }
            }

            if (curLineSize.X > 0)
            {
                currentLineInfo.Size = curLineSize;
                _lineInfos.Add(currentLineInfo);
            }

            //the last line size, if any should be added 
            panelSize.X = Math.Max(curLineSize.X, panelSize.X);
            panelSize.Y += curLineSize.Y;

            // EO
            if (_atLeastOneElementCanHasItsWidthExpanded)
            {
                if (Orientation == Orientation.Horizontal)
                    return new Size(constraint.Width, panelSize.Height);
                else
                    return new Size(panelSize.Width, constraint.Height);
            }

            //go from UV space to W/H space
            return new Size(panelSize.Width, panelSize.Height);
        }

        // ************************************************************************
        private struct XYSize
        {
            internal XYSize(Orientation orientation, double width, double height)
            {
                X = Y = 0d;
                _orientation = orientation;
                Width = width;
                Height = height;
            }

            internal XYSize(Orientation orientation)
            {
                X = Y = 0d;
                _orientation = orientation;
            }

            internal double X;
            internal double Y;
            private Orientation _orientation;

            internal double Width
            {
                get
                {
                    return (_orientation == Orientation.Horizontal ? X : Y);
                }
                set
                {
                    if (_orientation == Orientation.Horizontal) X = value; else Y = value;
                }
            }
            internal double Height
            {
                get
                {
                    return (_orientation == Orientation.Horizontal ? Y : X);
                }
                set
                {
                    if (_orientation == Orientation.Horizontal) Y = value; else X = value;
                }
            }
        }

        // ************************************************************************
        private class LineInfo
        {
            public List<UIElement> ElementsWithNoWidthSet = new List<UIElement>();
            //			public double SpaceLeft = 0;
            //			public double WidthCorrectionPerElement = 0;
            public XYSize Size;
            public double Correction = 0;
        }

        private List<LineInfo> _lineInfos = new List<LineInfo>();

        // ************************************************************************
        /// <summary>
        /// <see cref="FrameworkElement.ArrangeOverride"/> 
        /// </summary> 
        protected override Size ArrangeOverride(Size finalSize)
        {
            int lineIndex = 0;
            int firstInLine = 0;
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            if (CanWrap == false && FillDefault == true && !Double.IsNaN(finalSize.Width))
            {
                itemWidth = finalSize.Width / InternalChildren.Count;
            }

            double accumulatedV = 0;
            double itemU = (Orientation == Orientation.Horizontal ? itemWidth : itemHeight);
            XYSize curLineSize = new XYSize(Orientation);
            XYSize uvFinalSize = new XYSize(Orientation, finalSize.Width, finalSize.Height);
            bool itemWidthSet = !Double.IsNaN(itemWidth);
            bool itemHeightSet = !Double.IsNaN(itemHeight);
            bool useItemU = (Orientation == Orientation.Horizontal ? itemWidthSet : itemHeightSet);

            UIElementCollection children = InternalChildren;

            for (int i = 0, count = children.Count; i < count; i++)
            {
                UIElement child = children[i] as UIElement;
                if (child == null) continue;

                XYSize sz = new XYSize(
                    Orientation,
                    (itemWidthSet ? itemWidth : child.DesiredSize.Width),
                    (itemHeightSet ? itemHeight : child.DesiredSize.Height));

                if (DoubleGreaterThan(curLineSize.X + sz.X, uvFinalSize.X) && CanWrap) //need to switch to another line 
                {
                    arrangeLine(lineIndex, accumulatedV, curLineSize.Y, firstInLine, i, useItemU, itemU, uvFinalSize);
                    lineIndex++;

                    accumulatedV += curLineSize.Y;
                    curLineSize = sz;

                    if (DoubleGreaterThan(sz.X, uvFinalSize.X) && CanWrap) //the element is wider then the constraint - give it a separate line 
                    {
                        //switch to next line which only contain one element 
                        arrangeLine(lineIndex, accumulatedV, sz.Y, i, ++i, useItemU, itemU, uvFinalSize);

                        accumulatedV += sz.Y;
                        curLineSize = new XYSize(Orientation);
                    }
                    firstInLine = i;
                }
                else //continue to accumulate a line
                {
                    curLineSize.X += sz.X;
                    curLineSize.Y = Math.Max(sz.Y, curLineSize.Y);
                }
            }

            //arrange the last line, if any
            if (firstInLine < children.Count)
            {
                arrangeLine(lineIndex, accumulatedV, curLineSize.Y, firstInLine, children.Count, useItemU, itemU, uvFinalSize);
            }

            return finalSize;
        }

        // ************************************************************************
        private void arrangeLine(int lineIndex, double v, double lineV, int start, int end, bool useItemU, double itemU, XYSize uvFinalSize)
        {
            double u = 0;
            bool isHorizontal = (Orientation == Orientation.Horizontal);

            Debug.Assert(lineIndex < _lineInfos.Count);

            LineInfo lineInfo = _lineInfos[lineIndex];
            double lineSpaceAvailableForCorrection = Math.Max(uvFinalSize.X - lineInfo.Size.X, 0);
            double perControlCorrection = 0;
            if (lineSpaceAvailableForCorrection > 0 && lineInfo.Size.X > 0)
            {
                perControlCorrection = lineSpaceAvailableForCorrection / lineInfo.ElementsWithNoWidthSet.Count;
                if (double.IsInfinity(perControlCorrection))
                {
                    perControlCorrection = 0;
                }
            }
            int indexOfControlToAdjustSizeToFill = 0;
            UIElement uIElementToAdjustNext = indexOfControlToAdjustSizeToFill < lineInfo.ElementsWithNoWidthSet.Count ? lineInfo.ElementsWithNoWidthSet[indexOfControlToAdjustSizeToFill] : null;

            UIElementCollection children = InternalChildren;
            for (int i = start; i < end; i++)
            {
                UIElement child = children[i] as UIElement;
                if (child != null)
                {
                    XYSize childSize = new XYSize(Orientation, child.DesiredSize.Width, child.DesiredSize.Height);
                    double layoutSlotU = (useItemU ? itemU : childSize.X);

                    if (perControlCorrection > 0 && child == uIElementToAdjustNext)
                    {
                        layoutSlotU += perControlCorrection;

                        indexOfControlToAdjustSizeToFill++;
                        uIElementToAdjustNext = indexOfControlToAdjustSizeToFill < lineInfo.ElementsWithNoWidthSet.Count ? lineInfo.ElementsWithNoWidthSet[indexOfControlToAdjustSizeToFill] : null;
                    }

                    child.Arrange(new Rect(
                        (isHorizontal ? u : v),
                        (isHorizontal ? v : u),
                        (isHorizontal ? layoutSlotU : lineV),
                        (isHorizontal ? lineV : layoutSlotU)));
                    u += layoutSlotU;
                }
            }
        }

        // ************************************************************************

    }
}
