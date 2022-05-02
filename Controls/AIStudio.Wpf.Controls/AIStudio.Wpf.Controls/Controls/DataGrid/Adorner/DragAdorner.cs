using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// 拖拽时DataRowRow的装饰层
    /// </summary>
    public class DragAdorner : Adorner
    {
        #region Data

        private Rectangle child = null;
        private double offsetLeft = 0;
        private double offsetTop = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// 创建装饰层对象
        /// </summary>
        /// <param name="adornedElement">DataGridRow对象</param>
        /// <param name="size">装饰层size</param>
        /// <param name="brush">装饰层颜色</param>
        public DragAdorner(UIElement adornedElement, Size size, Brush brush)
            : base(adornedElement)
        {
            Rectangle rect = new Rectangle();
            rect.Fill = brush;
            rect.Width = size.Width;
            rect.Height = size.Height;
            rect.IsHitTestVisible = false;
            this.child = rect;
        }

        #endregion

        #region GetDesiredTransform

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.offsetLeft, this.offsetTop));
            return result;
        }

        #endregion

        #region OffsetLeft

        /// <summary>
        /// 
        /// </summary>
        public double OffsetLeft
        {
            get
            {
                return this.offsetLeft;
            }
            set
            {
                this.offsetLeft = value;
                UpdateLocation();
            }
        }

        #endregion

        #region SetOffsets

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void SetOffsets(double left, double top)
        {
            this.offsetLeft = left;
            this.offsetTop = top;
            this.UpdateLocation();
        }

        #endregion

        #region OffsetTop

        /// <summary>
        /// 
        /// </summary>
        public double OffsetTop
        {
            get
            {
                return this.offsetTop;
            }
            set
            {
                this.offsetTop = value;
                UpdateLocation();
            }
        }

        #endregion

        #region Protected Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.child.Measure(constraint);
            return this.child.DesiredSize;
        }

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.child;
        }

        /// <summary>
        /// Override.  Always returns 1.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        #endregion // Protected Overrides

        #region Private Helpers

        private void UpdateLocation()
        {
            AdornerLayer adornerLayer = this.Parent as AdornerLayer;
            if (adornerLayer != null)
                adornerLayer.Update(this.AdornedElement);
        }

        #endregion
    }
}
