using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class StartBlockBorder : Decorator
    {
        public StartBlockBorder()
        {

        }

        #region 依赖属性
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(StartBlockBorder)
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
            DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(StartBlockBorder)
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
            DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(StartBlockBorder)
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
            DependencyProperty.Register(nameof(BorderThickness), typeof(Thickness), typeof(StartBlockBorder), new PropertyMetadata(new Thickness(0d)));
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
        private double outsideheight = 13;
        private double outsidewidth = 70;
        private double insideheight = 6;
        private double insidewidth = 21;
        private double insideoffset = 14;
        private double minwidth = 86;
        private double minheight = 47;

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
                result.Height = Math.Max(minheight, Child.DesiredSize.Height + padding.Top + padding.Bottom + outsideheight);
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
                Child.Arrange(new Rect(new Point(padding.Left, outsideheight + padding.Top),
                    new Size(Math.Max(minwidth, Child.DesiredSize.Width + padding.Left + padding.Right), Math.Max(minheight - outsideheight, Child.DesiredSize.Height + padding.Top + padding.Bottom))));
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

            Geometry cg = CreateGeometry(Math.Max(minwidth, Child?.DesiredSize.Width ?? 0 + padding.Left + padding.Right), Math.Max(minheight - outsideheight, Child?.DesiredSize.Height ?? 0 + padding.Top + padding.Bottom));
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
            pf.StartPoint = new Point(0, outsideheight);

            //第一横线
            QuadraticBezierSegment seg1 = new QuadraticBezierSegment() { Point1 = new Point(outsidewidth / 2, 3 - outsideheight), Point2 = new Point(outsidewidth, outsideheight) };
            pf.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment() { Point = new Point(x - 2, outsideheight) };
            pf.Segments.Add(seg2);

            //第二竖线
            ArcSegment seg3 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x, outsideheight + 2) };
            pf.Segments.Add(seg3);

            LineSegment seg4 = new LineSegment() { Point = new Point(x, outsideheight + y - 2) };
            pf.Segments.Add(seg4);

            //第三横线
            ArcSegment seg5 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x - 2, outsideheight + y) };
            pf.Segments.Add(seg5);

            LineSegment seg6 = new LineSegment() { Point = new Point(insideoffset + insidewidth, outsideheight + y) };
            pf.Segments.Add(seg6);

            //第四in
            QuadraticBezierSegment seg7 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + insidewidth - 1, outsideheight + y), Point2 = new Point(insideoffset + insidewidth - 2, outsideheight + y + 2) };
            pf.Segments.Add(seg7);

            QuadraticBezierSegment seg8 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + insidewidth - 4, outsideheight + y + insideheight), Point2 = new Point(insideoffset + insidewidth - 5, outsideheight + y + insideheight) };
            pf.Segments.Add(seg8);

            LineSegment seg9 = new LineSegment() { Point = new Point(insideoffset + 5, outsideheight + y + insideheight) };
            pf.Segments.Add(seg9);

            QuadraticBezierSegment seg10 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + 4, outsideheight + y + insideheight), Point2 = new Point(insideoffset + 2, outsideheight + y + 2) };
            pf.Segments.Add(seg10);

            QuadraticBezierSegment seg11 = new QuadraticBezierSegment() { Point1 = new Point(insideoffset + 1, outsideheight + y), Point2 = new Point(insideoffset, outsideheight + y) };
            pf.Segments.Add(seg11);

            //第五横线
            LineSegment seg12 = new LineSegment() { Point = new Point(2, outsideheight + y) };
            pf.Segments.Add(seg12);

            ArcSegment seg13 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(0, outsideheight + y - 2) };
            pf.Segments.Add(seg13);

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
