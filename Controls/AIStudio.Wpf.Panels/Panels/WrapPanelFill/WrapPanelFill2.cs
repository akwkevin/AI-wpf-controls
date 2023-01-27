using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AIStudio.Wpf.Panels.Panels.WrapPanelFill2
{
    public class WrapPanelFill2 : WrapPanel
    {
        // ******************************************************************
        public static readonly DependencyProperty UseToFillProperty = DependencyProperty.RegisterAttached("UseToFill", typeof(Boolean),
            typeof(WrapPanelFill2), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        // ******************************************************************
        public static void SetUseToFill(UIElement element, Boolean value)
        {
            element.SetValue(UseToFillProperty, value);
        }
        // ******************************************************************
        public static Boolean GetUseToFill(UIElement element)
        {
            return (Boolean)element.GetValue(UseToFillProperty);
        }

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
            UVSize curLineSize = new UVSize(Orientation);
            UVSize panelSize = new UVSize(Orientation);
            UVSize uvConstraint = new UVSize(Orientation, constraint.Width, constraint.Height);
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
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
                UVSize sz = new UVSize(
                    Orientation,
                    (itemWidthSet ? itemWidth : child.DesiredSize.Width),
                    (itemHeightSet ? itemHeight : child.DesiredSize.Height));

                if (DoubleGreaterThan(curLineSize.U + sz.U, uvConstraint.U)) //need to switch to another line 
                {
                    // EO
                    currentLineInfo.Size = curLineSize;
                    _lineInfos.Add(currentLineInfo);

                    panelSize.U = Math.Max(curLineSize.U, panelSize.U);
                    panelSize.V += curLineSize.V;
                    curLineSize = sz;

                    // EO
                    currentLineInfo = new LineInfo();
                    var feChild = child as FrameworkElement;
                    if (GetUseToFill(feChild))
                    {
                        currentLineInfo.ElementsWithNoWidthSet.Add(feChild);
                        _atLeastOneElementCanHasItsWidthExpanded = true;
                    }

                    if (DoubleGreaterThan(sz.U, uvConstraint.U)) //the element is wider then the constrint - give it a separate line
                    {
                        currentLineInfo = new LineInfo();

                        panelSize.U = Math.Max(sz.U, panelSize.U);
                        panelSize.V += sz.V;
                        curLineSize = new UVSize(Orientation);
                    }
                }
                else //continue to accumulate a line
                {
                    curLineSize.U += sz.U;
                    curLineSize.V = Math.Max(sz.V, curLineSize.V);

                    // EO
                    var feChild = child as FrameworkElement;
                    if (GetUseToFill(feChild))
                    {
                        currentLineInfo.ElementsWithNoWidthSet.Add(feChild);
                        _atLeastOneElementCanHasItsWidthExpanded = true;
                    }
                }
            }

            if (curLineSize.U > 0)
            {
                currentLineInfo.Size = curLineSize;
                _lineInfos.Add(currentLineInfo);
            }

            //the last line size, if any should be added 
            panelSize.U = Math.Max(curLineSize.U, panelSize.U);
            panelSize.V += curLineSize.V;

            // EO
            if (_atLeastOneElementCanHasItsWidthExpanded)
            {
                return new Size(constraint.Width, panelSize.Height);
            }

            //go from UV space to W/H space
            return new Size(panelSize.Width, panelSize.Height);
        }

        // ************************************************************************
        private struct UVSize
        {
            internal UVSize(Orientation orientation, double width, double height)
            {
                U = V = 0d;
                _orientation = orientation;
                Width = width;
                Height = height;
            }

            internal UVSize(Orientation orientation)
            {
                U = V = 0d;
                _orientation = orientation;
            }

            internal double U;
            internal double V;
            private Orientation _orientation;

            internal double Width
            {
                get
                {
                    return (_orientation == Orientation.Horizontal ? U : V);
                }
                set
                {
                    if (_orientation == Orientation.Horizontal) U = value; else V = value;
                }
            }
            internal double Height
            {
                get
                {
                    return (_orientation == Orientation.Horizontal ? V : U);
                }
                set
                {
                    if (_orientation == Orientation.Horizontal) V = value; else U = value;
                }
            }
        }

        // ************************************************************************
        private class LineInfo
        {
            public List<UIElement> ElementsWithNoWidthSet = new List<UIElement>();
            //			public double SpaceLeft = 0;
            //			public double WidthCorrectionPerElement = 0;
            public UVSize Size;
            public double Correction = 0;
        }

        private List<LineInfo> _lineInfos = new List<LineInfo>();

        protected override Size ArrangeOverride(Size finalSize)
        {
            int lineIndex = 0;
            int firstInLine = 0;
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            double accumulatedV = 0;
            double itemU = (Orientation == Orientation.Horizontal ? itemWidth : itemHeight);
            UVSize curLineSize = new UVSize(Orientation);
            UVSize uvFinalSize = new UVSize(Orientation, finalSize.Width, finalSize.Height);
            bool itemWidthSet = !Double.IsNaN(itemWidth);
            bool itemHeightSet = !Double.IsNaN(itemHeight);
            bool useItemU = (Orientation == Orientation.Horizontal ? itemWidthSet : itemHeightSet);

            UIElementCollection children = InternalChildren;

            for (int i = 0, count = children.Count; i < count; i++)
            {
                UIElement child = children[i] as UIElement;
                if (child == null) continue;

                UVSize sz = new UVSize(
                    Orientation,
                    (itemWidthSet ? itemWidth : child.DesiredSize.Width),
                    (itemHeightSet ? itemHeight : child.DesiredSize.Height));

                if (DoubleGreaterThan(curLineSize.U + sz.U, uvFinalSize.U)) //need to switch to another line 
                {
                    arrangeLine(lineIndex, accumulatedV, curLineSize.V, firstInLine, i, useItemU, itemU, uvFinalSize);
                    lineIndex++;

                    accumulatedV += curLineSize.V;
                    curLineSize = sz;

                    if (DoubleGreaterThan(sz.U, uvFinalSize.U)) //the element is wider then the constraint - give it a separate line 
                    {
                        //switch to next line which only contain one element 
                        arrangeLine(lineIndex, accumulatedV, sz.V, i, ++i, useItemU, itemU, uvFinalSize);

                        accumulatedV += sz.V;
                        curLineSize = new UVSize(Orientation);
                    }
                    firstInLine = i;
                }
                else //continue to accumulate a line
                {
                    curLineSize.U += sz.U;
                    curLineSize.V = Math.Max(sz.V, curLineSize.V);
                }
            }

            //arrange the last line, if any
            if (firstInLine < children.Count)
            {
                arrangeLine(lineIndex, accumulatedV, curLineSize.V, firstInLine, children.Count, useItemU, itemU, uvFinalSize);
            }

            return finalSize;
        }

        // ************************************************************************
        private void arrangeLine(int lineIndex, double v, double lineV, int start, int end, bool useItemU, double itemU, UVSize uvFinalSize)
        {
            double u = 0;
            bool isHorizontal = (Orientation == Orientation.Horizontal);

            Debug.Assert(lineIndex < _lineInfos.Count);

            LineInfo lineInfo = _lineInfos[lineIndex];
            double lineSpaceAvailableForCorrection = Math.Max(uvFinalSize.U - lineInfo.Size.U, 0);
            double perControlCorrection = 0;
            if (lineSpaceAvailableForCorrection > 0 && lineInfo.Size.U > 0)
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
                    UVSize childSize = new UVSize(Orientation, child.DesiredSize.Width, child.DesiredSize.Height);
                    double layoutSlotU = (useItemU ? itemU : childSize.U);

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

    }
}
