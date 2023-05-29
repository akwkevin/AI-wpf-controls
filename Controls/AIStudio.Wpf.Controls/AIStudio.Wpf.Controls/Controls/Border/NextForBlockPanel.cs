using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{

    public class NextForBlockPanel : BlockPanel
    {
        public NextForBlockPanel()
        {

        }

        #region 方法重写
        private UIElement Header
        {
            get
            {
                return InternalChildren.Count >= 0 ? InternalChildren[0] : null;
            }
        }

        private UIElement Content
        {
            get
            {
                return InternalChildren.Count >= 1 ? InternalChildren[1] : null;
            }
        }

        private UIElement Footer
        {
            get
            {
                return InternalChildren.Count >= 2 ? InternalChildren[2] : null;
            }
        }

        private double insideheight = 6;
        private double insidewidth = 21;
        private double insideoffset = 14;
        private double leftpanelwidth = 10;
        private double minwidth = 109;
        private double minheaderheight = 34;
        private double mincontentheight = 16;
        private double minfooterheight = 26;


        private double _width;
        private double _headerheight;
        private double _contentheight;
        private double _footerheight;
        /// <summary>
        /// 该方法用于测量整个控件的大小
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns>控件的大小</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Thickness padding = this.Padding;

            _width = minwidth;
            if (Header != null)
            {
                //测量子控件的大小
                Header.Measure(constraint);
                _width = Math.Max(minwidth, Header.DesiredSize.Width + padding.Left + padding.Right);
                _headerheight = Math.Max(minheaderheight, Header.DesiredSize.Height + padding.Top + padding.Bottom);
            }
            else
            {
                _headerheight = minheaderheight;
            }

            if (Content != null)
            {
                //测量子控件的大小
                Content.Measure(constraint);
                _width = Math.Max(minwidth, Content.DesiredSize.Width + padding.Left + padding.Right + leftpanelwidth);
                _contentheight = Math.Max(mincontentheight, Content.DesiredSize.Height + padding.Top + padding.Bottom);
            }
            else
            {
                _contentheight = mincontentheight;
            }

            if (Footer != null)
            {
                //测量子控件的大小
                Footer.Measure(constraint);
                _width = Math.Max(minwidth, Footer.DesiredSize.Width + padding.Left + padding.Right);
                _footerheight = Math.Max(minfooterheight, Footer.DesiredSize.Height + padding.Top + padding.Bottom);
            }
            else
            {
                _footerheight = minfooterheight;
            }

            return new Size(_width, _headerheight + _contentheight + _footerheight);
        }

        /// <summary>
        /// 设置子控件的大小与位置
        /// </summary>
        /// <param name="arrangeSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Thickness padding = this.Padding;

            if (Header != null)
            {
                Header.Arrange(new Rect(new Point(padding.Left, padding.Top), new Size(_width, _headerheight)));             
            }


            if (Content != null)
            {
                Content.Arrange(new Rect(new Point(padding.Left + leftpanelwidth, padding.Top + _headerheight), new Size(_width - leftpanelwidth,_contentheight)));
            }


            if (Footer != null)
            {
                Footer.Arrange(new Rect(new Point(padding.Left + leftpanelwidth, padding.Top + _headerheight + _contentheight), new Size(_width, _footerheight)));
            }


            return base.ArrangeOverride(arrangeSize);
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

            Geometry cg = CreateGeometry(_width, _headerheight, _contentheight, _footerheight);
            Brush brush = CreateFillBrush();

            GuidelineSet guideLines = new GuidelineSet();
            drawingContext.PushGuidelineSet(guideLines);
            drawingContext.DrawGeometry(brush, pen, cg);

            base.OnRender(drawingContext);
        }
        #endregion      

        #region 私有方法
        private Geometry CreateGeometry(double x, double y0, double y1, double y2)
        {
            #region 
            PathFigure pf = new PathFigure();
            pf.IsClosed = true;
            pf.StartPoint = new Point(0, 2);

            //第一横线
            ArcSegment seg1 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(2, 0) };
            pf.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment() { Point = new Point(insideoffset, 0) };
            pf.Segments.Add(seg2);

            //第二in
            QuadraticBezierSegment seg3 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + 1, 0), Point2 = new Point(insideoffset + 2, 2) };
            pf.Segments.Add(seg3);

            QuadraticBezierSegment seg4 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + 4, insideheight), Point2 = new Point(insideoffset + 5, insideheight) };
            pf.Segments.Add(seg4);

            LineSegment seg5 = new LineSegment() { Point = new Point(insideoffset + insidewidth - 5, insideheight) };
            pf.Segments.Add(seg5);

            QuadraticBezierSegment seg6 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + insidewidth - 4, insideheight), Point2 = new Point(insideoffset + insidewidth - 2, 2) };
            pf.Segments.Add(seg6);

            QuadraticBezierSegment seg7 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + insidewidth - 1, 0), Point2 = new Point(insideoffset + insidewidth, 0) };
            pf.Segments.Add(seg7);

            //第三横线
            LineSegment seg8 = new LineSegment() { Point = new Point(x - 2, 0) };
            pf.Segments.Add(seg8);

            //第四竖线
            ArcSegment seg9 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x, 2) };
            pf.Segments.Add(seg9);

            LineSegment seg10 = new LineSegment() { Point = new Point(x, y0 - 2) };
            pf.Segments.Add(seg10);

            //第五横线
            ArcSegment seg11 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x - 2, y0) };
            pf.Segments.Add(seg11);

            LineSegment seg12 = new LineSegment() { Point = new Point(leftpanelwidth + insideoffset + insidewidth, y0) };
            pf.Segments.Add(seg12);

            //第六in
            QuadraticBezierSegment seg13 = new QuadraticBezierSegment() { Point1 = new Point(leftpanelwidth + insideoffset + insidewidth - 1, y0), Point2 = new Point(leftpanelwidth + insideoffset + insidewidth - 2, y0 + 2) };
            pf.Segments.Add(seg13);

            QuadraticBezierSegment seg14 = new QuadraticBezierSegment() { Point1 = new Point(leftpanelwidth + insideoffset + insidewidth - 4, y0 + insideheight), Point2 = new Point(leftpanelwidth + insideoffset + insidewidth - 5, y0 + insideheight) };
            pf.Segments.Add(seg14);

            LineSegment seg15 = new LineSegment() { Point = new Point(leftpanelwidth + insideoffset + 5, y0 + insideheight) };
            pf.Segments.Add(seg15);

            QuadraticBezierSegment seg16 = new QuadraticBezierSegment() { Point1 = new Point(leftpanelwidth + insideoffset + 4, y0 + insideheight), Point2 = new Point(leftpanelwidth + insideoffset + 2, y0 + 2) };
            pf.Segments.Add(seg16);

            QuadraticBezierSegment seg17 = new QuadraticBezierSegment() { Point1 = new Point(leftpanelwidth + insideoffset + 1, y0), Point2 = new Point(insideoffset, y0) };
            pf.Segments.Add(seg17);

            //第七横线
            LineSegment seg18 = new LineSegment() { Point = new Point(leftpanelwidth + 2, y0) };
            pf.Segments.Add(seg18);

            ArcSegment seg19 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Counterclockwise, Point = new Point(leftpanelwidth, y0 + 2) };
            pf.Segments.Add(seg19);

            //第八竖线
            LineSegment seg20 = new LineSegment() { Point = new Point(leftpanelwidth, y0 + y1 - 2) };
            pf.Segments.Add(seg20);

            ArcSegment seg21 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Counterclockwise, Point = new Point(leftpanelwidth + 2, y0 + y1) };
            pf.Segments.Add(seg21);

            //第九横线
            LineSegment seg22 = new LineSegment() { Point = new Point(leftpanelwidth + insideoffset, y0 + y1) };
            pf.Segments.Add(seg22);

            //第十in
            QuadraticBezierSegment seg23 = new QuadraticBezierSegment() { Point1 = new Point(leftpanelwidth + insideoffset + 1, y0 + y1), Point2 = new Point(leftpanelwidth + insideoffset + 2, y0 + y1 + 2) };
            pf.Segments.Add(seg23);

            QuadraticBezierSegment seg24 = new QuadraticBezierSegment() { Point1 = new Point(leftpanelwidth + insideoffset + 4, y0 + y1 + insideheight), Point2 = new Point(leftpanelwidth + insideoffset + 5, y0 + y1 + insideheight) };
            pf.Segments.Add(seg24);

            LineSegment seg25 = new LineSegment() { Point = new Point(leftpanelwidth + insideoffset + insidewidth - 5, y0 + y1 + insideheight) };
            pf.Segments.Add(seg25);

            QuadraticBezierSegment seg26 = new QuadraticBezierSegment() { Point1 = new Point(leftpanelwidth + insideoffset + insidewidth - 4, y0 + y1 + insideheight), Point2 = new Point(leftpanelwidth + insideoffset + insidewidth - 2, y0 + y1 + 2) };
            pf.Segments.Add(seg26);

            QuadraticBezierSegment seg27 = new QuadraticBezierSegment() { Point1 = new Point(leftpanelwidth + insideoffset + insidewidth - 1, y0 + y1), Point2 = new Point(leftpanelwidth + insideoffset + insidewidth, y0 + y1) };
            pf.Segments.Add(seg27);

            //第十一横线
            LineSegment seg28 = new LineSegment() { Point = new Point(x - 2, y0 + y1) };
            pf.Segments.Add(seg28);

            //第十二竖线
            ArcSegment seg29 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x, y0 + y1 + 2) };
            pf.Segments.Add(seg29);

            LineSegment seg30 = new LineSegment() { Point = new Point(x, y0 + y1 + y2 - 2) };
            pf.Segments.Add(seg30);

            //第十三横线
            ArcSegment seg31 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x - 2, y0 + y1 + y2) };
            pf.Segments.Add(seg31);

            LineSegment seg32 = new LineSegment() { Point = new Point(insideoffset + insidewidth, y0 + y1 + y2) };
            pf.Segments.Add(seg32);

            //第十四in
            QuadraticBezierSegment seg33 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + insidewidth - 1, y0 + y1 + y2), Point2 = new Point(insideoffset + insidewidth - 2, y0 + y1 + y2 + 2) };
            pf.Segments.Add(seg33);

            QuadraticBezierSegment seg34 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + insidewidth - 4, y0 + y1 + y2 + insideheight), Point2 = new Point(insideoffset + insidewidth - 5, y0 + y1 + y2 + insideheight) };
            pf.Segments.Add(seg34);

            LineSegment seg35 = new LineSegment() { Point = new Point(insideoffset + 5, y0 + y1 + y2 + insideheight) };
            pf.Segments.Add(seg35);

            QuadraticBezierSegment seg36 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + 4, y0 + y1 + y2 + insideheight), Point2 = new Point(insideoffset + 2, y0 + y1 + y2 + 2) };
            pf.Segments.Add(seg36);

            QuadraticBezierSegment seg37 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + 1, y0 + y1 + y2), Point2 = new Point(insideoffset, y0 + y1 + y2) };
            pf.Segments.Add(seg37);

            //第十五横线
            LineSegment seg38 = new LineSegment() { Point = new Point(2, y0 + y1 + y2) };
            pf.Segments.Add(seg38);

            ArcSegment seg39 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(0, y0 + y1 + y2 - 2) };
            pf.Segments.Add(seg39);

            PathGeometry g1 = new PathGeometry();
            g1.Figures.Add(pf);
            #endregion

            return g1;
        }

        #endregion
    }
}
