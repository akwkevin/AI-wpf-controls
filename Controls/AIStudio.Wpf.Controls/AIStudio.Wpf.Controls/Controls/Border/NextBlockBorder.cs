using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class NextBlockBorder : Decorator
    {
        public NextBlockBorder()
        {

        }

        #region 依赖属性
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(NextBlockBorder)
                , new PropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 255, 255))));
        /// <summary>
        /// 背景色，默认值为#FFFFFF，白色
        /// </summary>
        public Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }
            set
            {
                SetValue(BackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(NextBlockBorder)
                , new PropertyMetadata(new Thickness(0, 0, 0, 0)));
        /// <summary>
        /// 内边距
        /// </summary>
        public Thickness Padding
        {
            get
            {
                return (Thickness)GetValue(PaddingProperty);
            }
            set
            {
                SetValue(PaddingProperty, value);
            }
        }

        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(NextBlockBorder)
                , new PropertyMetadata(default(Brush)));
        /// <summary>
        /// 边框颜色
        /// </summary>
        public Brush BorderBrush
        {
            get
            {
                return (Brush)GetValue(BorderBrushProperty);
            }
            set
            {
                SetValue(BorderBrushProperty, value);
            }
        }

        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(nameof(BorderThickness), typeof(Thickness), typeof(NextBlockBorder), new PropertyMetadata(new Thickness(0d)));
        /// <summary>
        /// 边框大小
        /// </summary>
        public Thickness BorderThickness
        {
            get
            {
                return (Thickness)GetValue(BorderThicknessProperty);
            }
            set
            {
                SetValue(BorderThicknessProperty, value);
            }
        }
        #endregion

        #region 方法重写
        private double insideheight = 6;
        private double insidewidth = 21;
        private double insideoffset = 14;
        private double minwidth = 86;
        private double minheight = 34;
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

                result.Width = Math.Max(minwidth, Child.DesiredSize.Width + padding.Left + padding.Right);
                result.Height = Math.Max(minheight, Child.DesiredSize.Height + padding.Top + padding.Bottom);
            }
            else
            {
                result.Width = minwidth;
                result.Height = minheight;
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
                    new Size(Math.Max(minwidth, Child.DesiredSize.Width + padding.Left + padding.Right), Math.Max(minheight, Child.DesiredSize.Height + padding.Top + padding.Bottom))));
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

            Geometry cg = CreateGeometry(Math.Max(minwidth, Child?.DesiredSize.Width??0 + padding.Left + padding.Right), Math.Max(minheight, Child?.DesiredSize.Height??0 + padding.Top + padding.Bottom));
            Brush brush = CreateFillBrush();

            GuidelineSet guideLines = new GuidelineSet();
            drawingContext.PushGuidelineSet(guideLines);
            drawingContext.DrawGeometry(brush, pen, cg);

        }
        #endregion
        #region 私有方法
        private Geometry CreateGeometry(double x, double y)
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

            QuadraticBezierSegment seg4 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + 4, 6), Point2 = new Point(insideoffset + 5, 6) };
            pf.Segments.Add(seg4);

            LineSegment seg5 = new LineSegment() { Point = new Point(insideoffset + insidewidth - 5, 6) };
            pf.Segments.Add(seg5);

            QuadraticBezierSegment seg6 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + insidewidth - 4, 6), Point2 = new Point(insideoffset + insidewidth - 2, 2) };
            pf.Segments.Add(seg6);

            QuadraticBezierSegment seg7 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + insidewidth - 1, 0), Point2 = new Point(insideoffset + insidewidth, 0) };
            pf.Segments.Add(seg7);

            //第三横线
            LineSegment seg8 = new LineSegment() { Point = new Point(x - 2, 0) };
            pf.Segments.Add(seg8);

            //第四竖线
            ArcSegment seg9 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x, 2) };
            pf.Segments.Add(seg9);

            LineSegment seg10 = new LineSegment() { Point = new Point(x, y - 2) };
            pf.Segments.Add(seg10);

            //第五横线
            ArcSegment seg11 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x - 2, y) };
            pf.Segments.Add(seg11);

            LineSegment seg12 = new LineSegment() { Point = new Point(insideoffset + insidewidth, y) };
            pf.Segments.Add(seg12);

            //第六in
            QuadraticBezierSegment seg13 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + insidewidth - 1, y), Point2 = new Point(insideoffset + insidewidth - 2, y + 2) };
            pf.Segments.Add(seg13);

            QuadraticBezierSegment seg14 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + insidewidth - 4, y + insideheight), Point2 = new Point(insideoffset + insidewidth - 5, y + insideheight) };
            pf.Segments.Add(seg14);

            LineSegment seg15 = new LineSegment() { Point = new Point(insideoffset + 5, y + insideheight) };
            pf.Segments.Add(seg15);

            QuadraticBezierSegment seg16 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + 4, y + insideheight), Point2 = new Point(insideoffset + 2, y + 2) };
            pf.Segments.Add(seg16);

            QuadraticBezierSegment seg17 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + 1, y), Point2 = new Point(insideoffset, y) };
            pf.Segments.Add(seg17);

            //第七横线
            LineSegment seg18 = new LineSegment() { Point = new Point(2, y) };
            pf.Segments.Add(seg18);

            ArcSegment seg19 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(0, y - 2) };
            pf.Segments.Add(seg19);

            PathGeometry g1 = new PathGeometry();
            g1.Figures.Add(pf);
            #endregion

            return g1;
        }

        private Brush CreateFillBrush()
        {
            Brush result = null;

            GradientStopCollection gsc = new GradientStopCollection();
            gsc.Add(new GradientStop(((SolidColorBrush)this.Background).Color, 0));
            LinearGradientBrush backGroundBrush = new LinearGradientBrush(gsc, new Point(0, 0), new Point(0, 1));
            result = backGroundBrush;

            return result;
        }
        #endregion
    }
}
