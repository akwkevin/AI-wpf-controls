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
        private double headerheight = 13;
        private double headerwidth = 70;
        private double footerheight = 6;
        private double footerwidth = 21;
        private double footeroffset = 14;
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

                result.Width = Math.Max(headerwidth + 10, Child.DesiredSize.Width + padding.Left + padding.Right);
                result.Height = Child.DesiredSize.Height + padding.Top + padding.Bottom + headerheight;

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
                Child.Arrange(new Rect(new Point(padding.Left, headerheight + padding.Top), Child.DesiredSize));
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
            if (Child != null)
            {
                Geometry cg = null;
                Brush brush = null;
                //DpiScale dpi = base.getd();
                Pen pen = new Pen();

                pen.Brush = this.BorderBrush;
                //pen.Thickness = BorderThickness * 0.5;
                pen.Thickness = this.BorderThickness.Left;// StartBlockBorder.RoundLayoutValue(BorderThickness.Left, DoubleUtil.DpiScaleX);

                //生成小突出头部在右侧的图形和底色
                cg = CreateGeometry(Math.Max(headerwidth + 10, Child.DesiredSize.Width + padding.Left + padding.Right), Child.DesiredSize.Height + padding.Top + padding.Bottom);
                brush = CreateFillBrush();

                GuidelineSet guideLines = new GuidelineSet();
                drawingContext.PushGuidelineSet(guideLines);
                drawingContext.DrawGeometry(brush, pen, cg);
            }
        }
        #endregion

        private static double RoundLayoutValue(double value, double dpiScale)
        {
            double num;
            if (!StartBlockBorder.AreClose(dpiScale, 1.0))
            {
                num = Math.Round(value * dpiScale) / dpiScale;
                if (double.IsInfinity(num) || StartBlockBorder.AreClose(num, 1.7976931348623157E+308))
                {
                    num = value;
                }
            }
            else
            {
                num = Math.Round(value);
            }
            return num;
        }

        private static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }
            double num = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.2204460492503131E-16;
            double num2 = value1 - value2;
            return -num < num2 && num > num2;
        }

        #region 私有方法
        private Geometry CreateGeometry(double x, double y)
        {
            #region 
            PathFigure pf = new PathFigure();
            pf.IsClosed = true;
            pf.StartPoint = new Point(0, headerheight);

            QuadraticBezierSegment seg1 = new QuadraticBezierSegment() { Point1 = new Point(headerwidth / 2, 3 - headerheight), Point2 = new Point(headerwidth, headerheight) };
            pf.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment() { Point = new Point(x - 2, headerheight) };
            pf.Segments.Add(seg2);

            ArcSegment seg3 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x, headerheight + 2) };
            pf.Segments.Add(seg3);

            LineSegment seg4 = new LineSegment() { Point = new Point(x, headerheight + y - 2) };
            pf.Segments.Add(seg4);

            ArcSegment seg5 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(x - 2, headerheight + y) };
            pf.Segments.Add(seg5);

            LineSegment seg6 = new LineSegment() { Point = new Point(footeroffset + footerwidth, headerheight + y) };
            pf.Segments.Add(seg6);

            QuadraticBezierSegment seg7 = new QuadraticBezierSegment() { Point1 = new Point(footeroffset + footerwidth - 1, headerheight + y), Point2 = new Point(footeroffset + footerwidth - 2, headerheight + y + 2) };
            pf.Segments.Add(seg7);

            QuadraticBezierSegment seg8 = new QuadraticBezierSegment() { Point1 = new Point(footeroffset + footerwidth - 4, headerheight + y + footerheight), Point2 = new Point(footeroffset + footerwidth - 5, headerheight + y + footerheight) };
            pf.Segments.Add(seg8);

            LineSegment seg9 = new LineSegment() { Point = new Point(footeroffset + 5, headerheight + y + footerheight) };
            pf.Segments.Add(seg9);

            QuadraticBezierSegment seg10 = new QuadraticBezierSegment() { Point1 = new Point(footeroffset + 4, headerheight + y + footerheight), Point2 = new Point(footeroffset + 2, headerheight + y + 2) };
            pf.Segments.Add(seg10);

            QuadraticBezierSegment seg11 = new QuadraticBezierSegment() { Point1 = new Point(footeroffset + 1, headerheight + y), Point2 = new Point(footeroffset, headerheight + y) };
            pf.Segments.Add(seg11);

            LineSegment seg12 = new LineSegment() { Point = new Point(2, headerheight + y) };
            pf.Segments.Add(seg12);

            ArcSegment seg13 = new ArcSegment() { Size = new Size(2, 2), SweepDirection = SweepDirection.Clockwise, Point = new Point(0, headerheight + y - 2) };
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
