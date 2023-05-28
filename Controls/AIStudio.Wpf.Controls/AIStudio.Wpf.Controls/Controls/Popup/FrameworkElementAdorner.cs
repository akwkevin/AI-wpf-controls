using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class FrameworkElementAdorner : Adorner
    {
        #region Fields
        private const double BaseVerticalOffset = 0.0;
        private const double BaseHorizontalOffset = 0.0;

        private FrameworkElement _child = null;
        private Border _bgBorder = null;
        private AdornerPopup _adorner;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(FrameworkElementAdorner), new PropertyMetadata(0.0));

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(FrameworkElementAdorner), new PropertyMetadata(0.0));
        #endregion

        #region Properties
        public double VerticalOffset
        {
            get
            {
                return (double)GetValue(VerticalOffsetProperty);
            }
            set
            {
                SetValue(VerticalOffsetProperty, value);
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return (double)GetValue(HorizontalOffsetProperty);
            }
            set
            {
                SetValue(HorizontalOffsetProperty, value);
            }
        }
        #endregion

        #region Constructors 
        public FrameworkElementAdorner(FrameworkElement adornerChildElement, FrameworkElement adornedElement,
            AdornerPopup adorner,
            double horizontalOffset
            , double verticalOffset) : base(adornedElement)
        {
            this._child = adornerChildElement;
            this._adorner = adorner;
            this._bgBorder = CreateBackgroundBorder(adornerChildElement);

            this.HorizontalOffset = horizontalOffset;
            this.VerticalOffset = verticalOffset;

            base.AddLogicalChild(_bgBorder);
            base.AddVisualChild(_bgBorder);
        }
        #endregion

        #region Override Methods

        protected override Size MeasureOverride(Size constraint)
        {
            double measureWidth = constraint.Width;
            double measureHeight = constraint.Height;
            this._bgBorder.Measure(constraint);

            return new Size(this._bgBorder.DesiredSize.Width, this._bgBorder.DesiredSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double left = DetermineLeft(finalSize);
            double top = DetermineTop(finalSize);
            //double _width = DetermineWidth();
            //double height = DetermineHeight();
            double width = finalSize.Width;
            double height = finalSize.Height;

            this._bgBorder.Arrange(new Rect(left, top, width, height));
            return finalSize;
        }

        private double DetermineLeft(Size finalSize)
        {
            double left = BaseHorizontalOffset + HorizontalOffset;

            switch (_adorner.Placement)
            {
                case AdornerPopupPlacement.Top:
                    return left;
                case AdornerPopupPlacement.Right:
                    return left + AdornedElement.ActualWidth;
                case AdornerPopupPlacement.Left:
                    return left + finalSize.Width * -1;
                case AdornerPopupPlacement.Bottom:
                    return left;
                default:
                    return left;
            }
        }

        private double DetermineTop(Size finalSize)
        {
            double top = BaseVerticalOffset + VerticalOffset;

            switch (_adorner.Placement)
            {
                case AdornerPopupPlacement.Top:
                    return top + finalSize.Height * -1;
                case AdornerPopupPlacement.Right:
                    return top;
                case AdornerPopupPlacement.Left:
                    return top;
                case AdornerPopupPlacement.Bottom:
                    return top + AdornedElement.ActualHeight;
                default:
                    return top;
            }
        }

        protected override Int32 VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Visual GetVisualChild(Int32 index)
        {
            return this._bgBorder;
        }
        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                list.Add(this._bgBorder);
                return (IEnumerator)list.GetEnumerator();
            }
        }

        public new FrameworkElement AdornedElement
        {
            get
            {
                return (FrameworkElement)base.AdornedElement;
            }
        }
        #endregion

        #region Public Methods
        public void DisconnectChild()
        {
            _bgBorder.Child = null;
            base.RemoveLogicalChild(_bgBorder);
            base.RemoveVisualChild(_bgBorder);
            _bgBorder = null;
        }
        #endregion

        #region Private Methods 
        private Border CreateBackgroundBorder(FrameworkElement child)
        {
            Border br = new Border();
            br.SetBinding(Control.BackgroundProperty, new Binding("Background") { Source = _adorner, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            br.SetBinding(Control.BorderThicknessProperty, new Binding("BorderThickness") { Source = _adorner, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            br.SetBinding(Control.BorderBrushProperty, new Binding("BorderBrush") { Source = _adorner, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            br.Child = child;
            return br;
        }

        private double DetermineWidth()
        {
            if (_child == null)
            {
                return _adorner.ActualWidth;
            }

            switch (_child.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    {
                        return this._bgBorder.DesiredSize.Width;
                    }
                case HorizontalAlignment.Right:
                    {
                        return this._bgBorder.DesiredSize.Width;
                    }
                case HorizontalAlignment.Center:
                    {
                        return this._bgBorder.DesiredSize.Width;
                    }
                case HorizontalAlignment.Stretch:
                    {
                        return _adorner.ActualWidth;
                    }
            }

            return 0.0;
        }

        private double DetermineHeight()
        {
            if (_child == null)
            {
                return _adorner.ActualHeight;
            }

            switch (_child.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    {
                        return this._bgBorder.DesiredSize.Height;
                    }
                case VerticalAlignment.Bottom:
                    {
                        return this._bgBorder.DesiredSize.Height;
                    }
                case VerticalAlignment.Center:
                    {
                        return this._bgBorder.DesiredSize.Height;
                    }
                case VerticalAlignment.Stretch:
                    {
                        return _adorner.ActualHeight;
                    }
            }

            return 0.0;
        }
        #endregion
    }
}
