using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{

    public class NextFor2BlockPanel : BlockPanel
    {
        public NextFor2BlockPanel()
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

        private UIElement Content1
        {
            get
            {
                return InternalChildren.Count >= 1 ? InternalChildren[1] : null;
            }
        }

        private UIElement Footer1
        {
            get
            {
                return InternalChildren.Count >= 2 ? InternalChildren[2] : null;
            }
        }

        private UIElement Content2
        {
            get
            {
                return InternalChildren.Count >= 3 ? InternalChildren[3] : null;
            }
        }

        private UIElement Footer2
        {
            get
            {
                return InternalChildren.Count >= 4 ? InternalChildren[4] : null;
            }
        }

        public double Insideheight { get; set; } = 6;
        public double Insidewidth { get; set; } = 21;
        public double insideoffset { get; set; } = 14;
        public double Leftpanelwidth { get; set; } = 10;
        public double Minwidth { get; set; } = 109;
        public double Minheaderheight { get; set; } = 34;
        public double Mincontentheight { get; set; } = 16;
        public double Minfooterheight { get; set; } =  26;


        private double _width;
        private double _headerheight;
        private double _content1height;
        private double _footer1height;
        private double _content2height;
        private double _footer2height;
        /// <summary>
        /// 该方法用于测量整个控件的大小
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns>控件的大小</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Thickness padding = this.Padding;

            _width = Minwidth;
            if (Header != null)
            {
                //测量子控件的大小
                Header.Measure(constraint);
                _width = Math.Max(Minwidth, Header.DesiredSize.Width + padding.Left + padding.Right);
                _headerheight = Math.Max(Minheaderheight, Header.DesiredSize.Height + padding.Top + padding.Bottom);
            }
            else
            {
                _headerheight = Minheaderheight;
            }

            if (Content1 != null)
            {
                //测量子控件的大小
                Content1.Measure(constraint);
                _width = Math.Max(Minwidth, Content1.DesiredSize.Width + padding.Left + padding.Right + Leftpanelwidth);
                _content1height = Math.Max(Mincontentheight, Content1.DesiredSize.Height + padding.Top + padding.Bottom);
            }
            else
            {
                _content1height = Mincontentheight;
            }

            if (Footer1 != null)
            {
                //测量子控件的大小
                Footer1.Measure(constraint);
                _width = Math.Max(Minwidth, Footer1.DesiredSize.Width + padding.Left + padding.Right);
                _footer1height = Math.Max(Minfooterheight, Footer1.DesiredSize.Height + padding.Top + padding.Bottom);
            }
            else
            {
                _footer1height = Minfooterheight;
            }

            if (Content2!= null)
            {
                //测量子控件的大小
                Content2.Measure(constraint);
                _width = Math.Max(Minwidth, Content2.DesiredSize.Width + padding.Left + padding.Right + Leftpanelwidth);
                _content2height = Math.Max(Mincontentheight, Content2.DesiredSize.Height + padding.Top + padding.Bottom);
            }
            else
            {
                _content2height = Mincontentheight;
            }

            if (Footer2 != null)
            {
                //测量子控件的大小
                Footer2.Measure(constraint);
                _width = Math.Max(Minwidth, Footer2.DesiredSize.Width + padding.Left + padding.Right);
                _footer2height = Math.Max(Minfooterheight, Footer2.DesiredSize.Height + padding.Top + padding.Bottom);
            }
            else
            {
                _footer2height = Minfooterheight;
            }

            return new Size(_width, _headerheight + _content1height + _footer1height + _content2height + _footer2height);
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
                Header.Arrange(new Rect(new Point(padding.Left, padding.Top), new Size(_width - padding.Left - padding.Right, _headerheight - padding.Top - padding.Bottom)));             
            }


            if (Content1 != null)
            {
                Content1.Arrange(new Rect(new Point(padding.Left + Leftpanelwidth, padding.Top + _headerheight), new Size(_width - Leftpanelwidth - padding.Left - padding.Right, _content1height - padding.Top - padding.Bottom)));
            }


            if (Footer1 != null)
            {
                Footer1.Arrange(new Rect(new Point(padding.Left, padding.Top + _headerheight + _content1height), new Size(_width - padding.Left - padding.Right, _footer1height - padding.Top - padding.Bottom)));
            }

            if (Content2 != null)
            {
                Content2.Arrange(new Rect(new Point(padding.Left + Leftpanelwidth, padding.Top + _headerheight + _content1height + _footer1height), new Size(_width - Leftpanelwidth - padding.Left - padding.Right, _content2height - padding.Top - padding.Bottom)));
            }


            if (Footer2 != null)
            {
                Footer2.Arrange(new Rect(new Point(padding.Left, padding.Top + _headerheight + _content1height + _footer1height + _content2height), new Size(_width - padding.Left - padding.Right, _footer2height - padding.Top - padding.Bottom)));
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

            Geometry cg = CreateGeometry(_width, _headerheight, _content1height, _footer1height, _content2height, _footer2height);
            Brush brush = CreateFillBrush();

            GuidelineSet guideLines = new GuidelineSet();
            drawingContext.PushGuidelineSet(guideLines);
            drawingContext.DrawGeometry(brush, pen, cg);

            base.OnRender(drawingContext);
        }
        #endregion      

        #region 私有方法
        private Geometry CreateGeometry(double x, double y0, double y1, double y2, double y3, double y4)
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

            QuadraticBezierSegment seg4 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + 4, Insideheight), Point2 = new Point(insideoffset + 5, Insideheight) };
            pf.Segments.Add(seg4);

            LineSegment seg5 = new LineSegment() { Point = new Point(insideoffset + Insidewidth - 5, Insideheight) };
            pf.Segments.Add(seg5);

            QuadraticBezierSegment seg6 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + Insidewidth - 4, Insideheight), Point2 = new Point(insideoffset + Insidewidth - 2, 2) };
            pf.Segments.Add(seg6);

            QuadraticBezierSegment seg7 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + Insidewidth - 1, 0), Point2 = new Point(insideoffset + Insidewidth, 0) };
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

            LineSegment seg12 = new LineSegment() { Point = new Point(Leftpanelwidth + insideoffset + Insidewidth, y0) };
            pf.Segments.Add(seg12);

            //第六in
            QuadraticBezierSegment seg13 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + Insidewidth - 1, y0), Point2 = new Point(Leftpanelwidth + insideoffset + Insidewidth - 2, y0 + 2) };
            pf.Segments.Add(seg13);

            QuadraticBezierSegment seg14 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + Insidewidth - 4, y0 + Insideheight), Point2 = new Point(Leftpanelwidth + insideoffset + Insidewidth - 5, y0 + Insideheight) };
            pf.Segments.Add(seg14);

            LineSegment seg15 = new LineSegment() { Point = new Point(Leftpanelwidth + insideoffset + 5, y0 + Insideheight) };
            pf.Segments.Add(seg15);

            QuadraticBezierSegment seg16 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + 4, y0 + Insideheight), Point2 = new Point(Leftpanelwidth + insideoffset + 2, y0 + 2) };
            pf.Segments.Add(seg16);

            QuadraticBezierSegment seg17 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + 1, y0), Point2 = new Point(insideoffset, y0) };
            pf.Segments.Add(seg17);

            //第七横线
            LineSegment seg18 = new LineSegment() { Point = new Point(Leftpanelwidth + 2, y0) };
            pf.Segments.Add(seg18);

            ArcSegment seg19 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Counterclockwise, Point = new Point(Leftpanelwidth, y0 + 2) };
            pf.Segments.Add(seg19);

            //第八竖线
            LineSegment seg20 = new LineSegment() { Point = new Point(Leftpanelwidth, y0 + y1 - 2) };
            pf.Segments.Add(seg20);

            ArcSegment seg21 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Counterclockwise, Point = new Point(Leftpanelwidth + 2, y0 + y1) };
            pf.Segments.Add(seg21);

            //第九横线
            LineSegment seg22 = new LineSegment() { Point = new Point(Leftpanelwidth + insideoffset, y0 + y1) };
            pf.Segments.Add(seg22);

            //第十in
            QuadraticBezierSegment seg23 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + 1, y0 + y1), Point2 = new Point(Leftpanelwidth + insideoffset + 2, y0 + y1 + 2) };
            pf.Segments.Add(seg23);

            QuadraticBezierSegment seg24 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + 4, y0 + y1 + Insideheight), Point2 = new Point(Leftpanelwidth + insideoffset + 5, y0 + y1 + Insideheight) };
            pf.Segments.Add(seg24);

            LineSegment seg25 = new LineSegment() { Point = new Point(Leftpanelwidth + insideoffset + Insidewidth - 5, y0 + y1 + Insideheight) };
            pf.Segments.Add(seg25);

            QuadraticBezierSegment seg26 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + Insidewidth - 4, y0 + y1 + Insideheight), Point2 = new Point(Leftpanelwidth + insideoffset + Insidewidth - 2, y0 + y1 + 2) };
            pf.Segments.Add(seg26);

            QuadraticBezierSegment seg27 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + Insidewidth - 1, y0 + y1), Point2 = new Point(Leftpanelwidth + insideoffset + Insidewidth, y0 + y1) };
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



            LineSegment seg32 = new LineSegment() { Point = new Point(Leftpanelwidth +  insideoffset + Insidewidth, y0 + y1 + y2) };
            pf.Segments.Add(seg32);

            //第十四in
            QuadraticBezierSegment seg33 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + Insidewidth - 1, y0 + y1 + y2), Point2 = new Point(Leftpanelwidth + insideoffset + Insidewidth - 2, y0 + y1 + y2 + 2) };
            pf.Segments.Add(seg33);

            QuadraticBezierSegment seg34 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + Insidewidth - 4, y0 + y1 + y2 + Insideheight), Point2 = new Point(Leftpanelwidth + insideoffset + Insidewidth - 5, y0 + y1 + y2 + Insideheight) };
            pf.Segments.Add(seg34);

            LineSegment seg35 = new LineSegment() { Point = new Point(Leftpanelwidth + insideoffset + 5, y0 + y1 + y2 + Insideheight) };
            pf.Segments.Add(seg35);

            QuadraticBezierSegment seg36 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + 4, y0 + y1 + y2 + Insideheight), Point2 = new Point(Leftpanelwidth + insideoffset + 2, y0 + y1 + y2 + 2) };
            pf.Segments.Add(seg36);

            QuadraticBezierSegment seg37 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + 1, y0 + y1 + y2), Point2 = new Point(Leftpanelwidth + insideoffset, y0 + y1 + y2) };
            pf.Segments.Add(seg37);

            //第十五横线
            LineSegment seg38 = new LineSegment() { Point = new Point(Leftpanelwidth + 2, y0 + y1 + y2) };
            pf.Segments.Add(seg38);

            ArcSegment seg39 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Counterclockwise, Point = new Point(Leftpanelwidth, y0 + y1 + y2 + 2) };
            pf.Segments.Add(seg39);

            //第十六竖向
            LineSegment seg40 = new LineSegment() { Point = new Point(Leftpanelwidth, y0 + y1 + y2 + y3 - 2) };
            pf.Segments.Add(seg40);

            ArcSegment seg41 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Counterclockwise, Point = new Point(Leftpanelwidth + 2, y0 + y1 + y2 + y3) };
            pf.Segments.Add(seg41);

            //第十七横线
            LineSegment seg42 = new LineSegment() { Point = new Point(Leftpanelwidth + insideoffset, y0 + y1 + y2 + y3) };
            pf.Segments.Add(seg42);

            //第十八in
            QuadraticBezierSegment seg43 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + 1, y0 + y1 + y2 + y3), Point2 = new Point(Leftpanelwidth + insideoffset + 2, y0 + y1 + y2 + y3 + 2) };
            pf.Segments.Add(seg43);

            QuadraticBezierSegment seg44 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + 4, y0 + y1 + y2 + y3 + Insideheight), Point2 = new Point(Leftpanelwidth + insideoffset + 5, y0 + y1 + y2 + y3 + Insideheight) };
            pf.Segments.Add(seg44);

            LineSegment seg45 = new LineSegment() { Point = new Point(Leftpanelwidth + insideoffset + Insidewidth - 5, y0 + y1 + y2 + y3 + Insideheight) };
            pf.Segments.Add(seg45);

            QuadraticBezierSegment seg46 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + Insidewidth - 4, y0 + y1 + y2 + y3 + Insideheight), Point2 = new Point(Leftpanelwidth + insideoffset + Insidewidth - 2, y0 + y1 + y2 + y3 + 2) };
            pf.Segments.Add(seg46);

            QuadraticBezierSegment seg47 = new QuadraticBezierSegment() { Point1 = new Point(Leftpanelwidth + insideoffset + Insidewidth - 1, y0 + y1 + y2 + y3), Point2 = new Point(Leftpanelwidth + insideoffset + Insidewidth, y0 + y1 + y2 + y3) };
            pf.Segments.Add(seg47);

            //第十一横线
            LineSegment seg48 = new LineSegment() { Point = new Point(x - 2, y0 + y1 + y2 + y3) };
            pf.Segments.Add(seg48);

            //第十二竖线
            ArcSegment seg49 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x, y0 + y1 + y2 + y3 + 2) };
            pf.Segments.Add(seg49);

            LineSegment seg50 = new LineSegment() { Point = new Point(x, y0 + y1 + y2 + y3 + y4 - 2) };
            pf.Segments.Add(seg50);

            //第十三横线
            ArcSegment seg51 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x - 2, y0 + y1 + y2 + y3 + y4) };
            pf.Segments.Add(seg51);



            LineSegment seg52 = new LineSegment() { Point = new Point(insideoffset + Insidewidth, y0 + y1 + y2 + y3 + y4) };
            pf.Segments.Add(seg52);

            //第十四in
            QuadraticBezierSegment seg53 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + Insidewidth - 1, y0 + y1 + y2 + y3 + y4), Point2 = new Point(insideoffset + Insidewidth - 2, y0 + y1 + y2 + y3 + y4 + 2) };
            pf.Segments.Add(seg53);

            QuadraticBezierSegment seg54 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + Insidewidth - 4, y0 + y1 + y2 + y3 + y4 + Insideheight), Point2 = new Point(insideoffset + Insidewidth - 5, y0 + y1 + y2 + y3 + y4 + Insideheight) };
            pf.Segments.Add(seg54);

            LineSegment seg55 = new LineSegment() { Point = new Point(insideoffset + 5, y0 + y1 + y2 + y3 + y4 + Insideheight) };
            pf.Segments.Add(seg55);

            QuadraticBezierSegment seg56 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + 4, y0 + y1 + y2 + y3 + y4 + Insideheight), Point2 = new Point(insideoffset + 2, y0 + y1 + y2 + y3 + y4 + 2) };
            pf.Segments.Add(seg56);

            QuadraticBezierSegment seg57 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + 1, y0 + y1 + y2 + y3 + y4), Point2 = new Point(insideoffset, y0 + y1 + y2 + y3 + y4) };
            pf.Segments.Add(seg57);

            //第十五横线
            LineSegment seg58 = new LineSegment() { Point = new Point(2, y0 + y1 + y2 + y3 + y4) };
            pf.Segments.Add(seg58);

            ArcSegment seg59 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Counterclockwise, Point = new Point(0, y0 + y1 + y2 + y3 + y4 - 2) };
            pf.Segments.Add(seg59);


            PathGeometry g1 = new PathGeometry();
            g1.Figures.Add(pf);
            #endregion

            return g1;
        }

        #endregion
    }
}
