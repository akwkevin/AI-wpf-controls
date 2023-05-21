using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class FourColorBorder : Border
    {
        public Brush LeftBorderBrush
        {
            get
            {
                return (Brush)GetValue(LeftBorderBrushProperty);
            }
            set
            {
                SetValue(LeftBorderBrushProperty, value);
            }
        }


        public static readonly DependencyProperty LeftBorderBrushProperty =
            DependencyProperty.Register(nameof(LeftBorderBrush), typeof(Brush), typeof(FourColorBorder), new PropertyMetadata(null));

        public Brush TopBorderBrush
        {
            get
            {
                return (Brush)GetValue(TopBorderBrushProperty);
            }
            set
            {
                SetValue(TopBorderBrushProperty, value);
            }
        }


        public static readonly DependencyProperty TopBorderBrushProperty =
            DependencyProperty.Register(nameof(TopBorderBrush), typeof(Brush), typeof(FourColorBorder), new PropertyMetadata(null));

        public Brush RightBorderBrush
        {
            get
            {
                return (Brush)GetValue(RightBorderBrushProperty);
            }
            set
            {
                SetValue(RightBorderBrushProperty, value);
            }
        }


        public static readonly DependencyProperty RightBorderBrushProperty =
            DependencyProperty.Register(nameof(RightBorderBrush), typeof(Brush), typeof(FourColorBorder), new PropertyMetadata(null));

        public Brush BottomBorderBrush
        {
            get
            {
                return (Brush)GetValue(BottomBorderBrushProperty);
            }
            set
            {
                SetValue(BottomBorderBrushProperty, value);
            }
        }


        public static readonly DependencyProperty BottomBorderBrushProperty =
            DependencyProperty.Register(nameof(BottomBorderBrush), typeof(Brush), typeof(FourColorBorder), new PropertyMetadata(null));

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {

            base.OnRender(dc);
            bool useLayoutRounding = base.UseLayoutRounding;

            Thickness borderThickness = this.BorderThickness;
            CornerRadius cornerRadius = this.CornerRadius;
            double topLeft = cornerRadius.TopLeft;
            bool flag = !DoubleUtil.IsZero(topLeft);
            Brush borderBrush = null;

            Pen pen = null;
            if (pen == null)
            {
                pen = new Pen();
                borderBrush = LeftBorderBrush;
                pen.Brush = LeftBorderBrush;
                if (useLayoutRounding)
                {
                    pen.Thickness = UlementEx.RoundLayoutValue(borderThickness.Left, DoubleUtil.DpiScaleX);
                }
                else
                {
                    pen.Thickness = borderThickness.Left;
                }
                if (borderBrush != null)
                {
                    if (borderBrush.IsFrozen)
                    {
                        pen.Freeze();
                    }
                }


                if (DoubleUtil.GreaterThan(borderThickness.Left, 0.0))
                {
                    double num = pen.Thickness * 0.5;
                    dc.DrawLine(pen, new Point(num, 0.0), new Point(num, base.RenderSize.Height));
                }
                if (DoubleUtil.GreaterThan(borderThickness.Right, 0.0))
                {

                    pen = new Pen();
                    pen.Brush = RightBorderBrush;
                    if (useLayoutRounding)
                    {
                        pen.Thickness = UlementEx.RoundLayoutValue(borderThickness.Right, DoubleUtil.DpiScaleX);
                    }
                    else
                    {
                        pen.Thickness = borderThickness.Right;
                    }
                    if (borderBrush != null)
                    {
                        if (borderBrush.IsFrozen)
                        {
                            pen.Freeze();
                        }
                    }

                    double num = pen.Thickness * 0.5;
                    dc.DrawLine(pen, new Point(base.RenderSize.Width - num, 0.0), new Point(base.RenderSize.Width - num, base.RenderSize.Height));
                }
                if (DoubleUtil.GreaterThan(borderThickness.Top, 0.0))
                {


                    pen = new Pen();
                    pen.Brush = TopBorderBrush;
                    if (useLayoutRounding)
                    {
                        pen.Thickness = UlementEx.RoundLayoutValue(borderThickness.Top, DoubleUtil.DpiScaleY);
                    }
                    else
                    {
                        pen.Thickness = borderThickness.Top;
                    }
                    if (borderBrush != null)
                    {
                        if (borderBrush.IsFrozen)
                        {
                            pen.Freeze();
                        }
                    }


                    double num = pen.Thickness * 0.5;
                    dc.DrawLine(pen, new Point(0.0, num), new Point(base.RenderSize.Width, num));
                }
                if (DoubleUtil.GreaterThan(borderThickness.Bottom, 0.0))
                {


                    pen = new Pen();
                    pen.Brush = BottomBorderBrush;
                    if (useLayoutRounding)
                    {
                        pen.Thickness = UlementEx.RoundLayoutValue(borderThickness.Bottom, DoubleUtil.DpiScaleY);
                    }
                    else
                    {
                        pen.Thickness = borderThickness.Bottom;
                    }
                    if (borderBrush != null)
                    {
                        if (borderBrush.IsFrozen)
                        {
                            pen.Freeze();
                        }
                    }

                    double num = pen.Thickness * 0.5;
                    dc.DrawLine(pen, new Point(0.0, base.RenderSize.Height - num), new Point(base.RenderSize.Width, base.RenderSize.Height - num));
                }


            }
        }
    }


    public static class UlementEx
    {
        public static double RoundLayoutValue(double value, double dpiScale)
        {
            double num;
            if (!DoubleUtil.AreClose(dpiScale, 1.0))
            {
                num = Math.Round(value * dpiScale) / dpiScale;
                if (DoubleUtil.IsNaN(num) || double.IsInfinity(num) || DoubleUtil.AreClose(num, 1.7976931348623157E+308))
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
    }
}
