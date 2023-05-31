using System;
using System.Windows;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class StartBlockBorder : BlockDecorator
    {
        public StartBlockBorder()
        {

        }

        #region 方法重写
        public double Outsideheight { get; set; } = 13;
        public double Outsidewidth { get; set; } = 70;
        public double Insideheight { get; set; } = 6;
        public double Insidewidth { get; set; } = 21;
        public double Insideoffset { get; set; } = 14;
        public double Minwidth { get; set; } = 86;
        public double Minheight { get; set; } = 47;

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
                result.Height = Math.Max(Minheight, Child.DesiredSize.Height + padding.Top + padding.Bottom + Outsideheight);
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
                Child.Arrange(new Rect(new Point(padding.Left, Outsideheight + padding.Top),
                    new Size(Math.Max(Minwidth - padding.Left - padding.Right, Child.DesiredSize.Width), Math.Max(Minheight - Outsideheight - padding.Top - padding.Bottom, Child.DesiredSize.Height ))));
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

            Geometry cg = CreateGeometry(Math.Max(Minwidth, Child?.DesiredSize.Width ?? 0 + padding.Left + padding.Right), Math.Max(Minheight - Outsideheight, Child?.DesiredSize.Height ?? 0 + padding.Top + padding.Bottom));
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
            pf.StartPoint = new Point(0, Outsideheight);

            //第一横线
            QuadraticBezierSegment seg1 = new QuadraticBezierSegment() { Point1 = new Point(Outsidewidth / 2, 3 - Outsideheight), Point2 = new Point(Outsidewidth, Outsideheight) };
            pf.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment() { Point = new Point(x - 2, Outsideheight) };
            pf.Segments.Add(seg2);

            //第二竖线
            ArcSegment seg3 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x, Outsideheight + 2) };
            pf.Segments.Add(seg3);

            LineSegment seg4 = new LineSegment() { Point = new Point(x, Outsideheight + y - 2) };
            pf.Segments.Add(seg4);

            //第三横线
            ArcSegment seg5 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x - 2, Outsideheight + y) };
            pf.Segments.Add(seg5);

            LineSegment seg6 = new LineSegment() { Point = new Point(Insideoffset + Insidewidth, Outsideheight + y) };
            pf.Segments.Add(seg6);

            //第四in
            QuadraticBezierSegment seg7 = new QuadraticBezierSegment() { Point1 = new Point(Insideoffset + Insidewidth - 1, Outsideheight + y), Point2 = new Point(Insideoffset + Insidewidth - 2, Outsideheight + y + 2) };
            pf.Segments.Add(seg7);

            QuadraticBezierSegment seg8 = new QuadraticBezierSegment() { Point1 = new Point(Insideoffset + Insidewidth - 4, Outsideheight + y + Insideheight), Point2 = new Point(Insideoffset + Insidewidth - 5, Outsideheight + y + Insideheight) };
            pf.Segments.Add(seg8);

            LineSegment seg9 = new LineSegment() { Point = new Point(Insideoffset + 5, Outsideheight + y + Insideheight) };
            pf.Segments.Add(seg9);

            QuadraticBezierSegment seg10 = new QuadraticBezierSegment() { Point1 = new Point(Insideoffset + 4, Outsideheight + y + Insideheight), Point2 = new Point(Insideoffset + 2, Outsideheight + y + 2) };
            pf.Segments.Add(seg10);

            QuadraticBezierSegment seg11 = new QuadraticBezierSegment() { Point1 = new Point(Insideoffset + 1, Outsideheight + y), Point2 = new Point(Insideoffset, Outsideheight + y) };
            pf.Segments.Add(seg11);

            //第五横线
            LineSegment seg12 = new LineSegment() { Point = new Point(2, Outsideheight + y) };
            pf.Segments.Add(seg12);

            ArcSegment seg13 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(0, Outsideheight + y - 2) };
            pf.Segments.Add(seg13);

            PathGeometry g1 = new PathGeometry();
            g1.Figures.Add(pf);
            #endregion

            return g1;
        }
        #endregion
    }
}
