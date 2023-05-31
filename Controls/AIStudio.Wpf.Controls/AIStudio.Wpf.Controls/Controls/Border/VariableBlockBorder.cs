using System;
using System.Windows;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class VariableBlockBorder : BlockDecorator
    {
        public VariableBlockBorder()
        {

        }

        #region 方法重写
        public double Minwidth { get; set; } = 64;
        public double Minheight { get; set; } = 28;

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
            pf.StartPoint = new Point(y / 2, 0);


            LineSegment seg1 = new LineSegment() { Point = new Point(x - y / 2, 0) };
            pf.Segments.Add(seg1);

            ArcSegment seg2= new ArcSegment() { Size = new Size(y / 2, y / 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x - y / 2, y) };
            pf.Segments.Add(seg2);

            LineSegment seg3 = new LineSegment() { Point = new Point(y / 2, y) };
            pf.Segments.Add(seg3);

            ArcSegment seg4 = new ArcSegment() { Size = new Size(y / 2, y / 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(y / 2, 0) };
            pf.Segments.Add(seg4);           

            PathGeometry g1 = new PathGeometry();
            g1.Figures.Add(pf);
            #endregion

            return g1;
        }
        #endregion
    }
}
