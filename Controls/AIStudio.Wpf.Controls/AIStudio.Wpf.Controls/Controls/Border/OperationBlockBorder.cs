using System;
using System.Windows;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class OperationBlockBorder : BlockDecorator
    {
        public OperationBlockBorder()
        {

        }

        #region 方法重写
        public double Minwidth { get; set; } = 100;
        public double Minheight { get; set; } = 30;

        /// <summary>
        /// 该方法用于测量整个控件的大小
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns>控件的大小</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Thickness padding = this.Padding;

            Size result = new Size();
            if (Child != null)
            {
                //测量子控件的大小
                Child.Measure(constraint);

                result.Width = Math.Max(Minwidth, Child.DesiredSize.Width + padding.Left + padding.Right);
                result.Height = Math.Max(Minheight, Child.DesiredSize.Height + padding.Top + padding.Bottom);
            }
            else
            {
                result.Width = Minwidth;
                result.Height = Minheight;
            }
            return result;
        }

        /// <summary>
        /// 设置子控件的大小与位置
        /// </summary>
        /// <param name="arrangeSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Thickness padding = this.Padding;
            if (Child != null)
            {
                Child.Arrange(new Rect(new Point(padding.Left, padding.Top),
                    new Size(Math.Max(Minwidth - padding.Left - padding.Right, Child.DesiredSize.Width), Math.Max(Minheight - padding.Top - padding.Bottom, Child.DesiredSize.Height))));
            }
            return arrangeSize;
        }

        /// <summary>
        /// 绘制控件
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            Thickness padding = this.Padding;

            Pen pen = new Pen();
            pen.Brush = this.BorderBrush;
            pen.Thickness = this.BorderThickness.Left;// NextForBlockBorder.RoundLayoutValue(BorderThickness.Left, DoubleUtil.DpiScaleX);

            Geometry cg = CreateGeometry(Math.Max(Minwidth, Child?.DesiredSize.Width ?? 0 + padding.Left + padding.Right), Math.Max(Minheight, Child?.DesiredSize.Height ?? 0 + padding.Top + padding.Bottom));
            Brush brush = CreateFillBrush();

            GuidelineSet guideLines = new GuidelineSet();
            drawingContext.PushGuidelineSet(guideLines);
            drawingContext.DrawGeometry(brush, pen, cg);
        }
        #endregion

        #region 私有方法
        //按Element的大小计算
        private Geometry CreateGeometry(double x, double y)
        {
            #region 
            PathFigure pf = new PathFigure();
            pf.IsClosed = true;
            pf.StartPoint = new Point(0, y/ 2 + 1);


            LineSegment seg1 = new LineSegment() { Point = new Point(y / 2 + 1, 0) };
            pf.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment() { Point = new Point(x - y / 2 - 1, 0) };
            pf.Segments.Add(seg2);

            LineSegment seg3 = new LineSegment() { Point = new Point(x, y / 2) };
            pf.Segments.Add(seg3);

            LineSegment seg4 = new LineSegment() { Point = new Point(x - y / 2 - 1, y) };
            pf.Segments.Add(seg4);

            LineSegment seg5 = new LineSegment() { Point = new Point(y / 2 + 1, y) };
            pf.Segments.Add(seg5);

            PathGeometry g1 = new PathGeometry();
            g1.Figures.Add(pf);
            #endregion

            return g1;
        }
        #endregion
    }
}
