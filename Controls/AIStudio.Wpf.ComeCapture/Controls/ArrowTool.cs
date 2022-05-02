using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.ComeCapture.Controls
{
    public class ArrowTool : StackPanel
    {
        private StreamGeometry geometry = new StreamGeometry();

        static ArrowTool()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ArrowTool), new FrameworkPropertyMetadata(typeof(ArrowTool)));
        }

        public ArrowTool()
        {
            _Current = this;
        }

        public List<Point> CreateArrow(Point start, Point end)
        {
            var tan = Math.Atan((end.Y - start.Y) / (end.X - start.X));
            var Move0 = tan - ArrowAngle;
            var Move1 = tan + ArrowAngle;
            var convert = end.X > start.X ? -1 : 1;
            var X0 = end.X + ((convert * ArrowLength) * Math.Cos(Move0));
            var Y0 = end.Y + ((convert * ArrowLength) * Math.Sin(Move0));
            var X1 = end.X + ((convert * ArrowLength) * Math.Cos(Move1));
            var Y1 = end.Y + ((convert * ArrowLength) * Math.Sin(Move1));
            var point3 = new Point(X0, Y0);
            var point5 = new Point(X1, Y1);
            var point2 = new Point(X0 + 0.25 * (X1 - X0), Y0 + 0.25 * (Y1 - Y0));
            var point4 = new Point(X0 + 0.75 * (X1 - X0), Y0 + 0.75 * (Y1 - Y0));
            return new List<Point>()
            {
                start,
                new Point(X0 + 0.25 * (X1 - X0), Y0 + 0.25 * (Y1 - Y0)),
                new Point(X0, Y0),
                end,
                new Point(X1, Y1),
                new Point(X0 + 0.75 * (X1 - X0), Y0 + 0.75 * (Y1 - Y0)),
                start
            };
        }

        #region 属性 Current
        private static ArrowTool _Current = null;
        public static ArrowTool Current
        {
            get
            {
                return _Current;
            }
        }
        #endregion

        #region LineThickness DependencyProperty
        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }
        public static readonly DependencyProperty LineThicknessProperty =
                DependencyProperty.Register("LineThickness", typeof(double), typeof(ArrowTool),
                new PropertyMetadata(5.0, new PropertyChangedCallback(ArrowTool.OnLineThicknessPropertyChanged)));

        private static void OnLineThicknessPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ArrowTool)
            {
                (obj as ArrowTool).OnLineThicknessValueChanged();
            }
        }

        protected void OnLineThicknessValueChanged()
        {
            ArrowLength = LineThickness * 2 + 7;
        }
        #endregion

        #region LineBrush DependencyProperty
        public SolidColorBrush LineBrush
        {
            get { return (SolidColorBrush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }
        public static readonly DependencyProperty LineBrushProperty =
                DependencyProperty.Register("LineBrush", typeof(SolidColorBrush), typeof(ArrowTool),
                new PropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(ArrowTool.OnLineBrushPropertyChanged)));

        private static void OnLineBrushPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ArrowTool)
            {
                (obj as ArrowTool).OnLineBrushValueChanged();
            }
        }

        protected void OnLineBrushValueChanged()
        {

        }
        #endregion

        #region ArrowLength DependencyProperty
        public double ArrowLength
        {
            get { return (double)GetValue(ArrowLengthProperty); }
            set { SetValue(ArrowLengthProperty, value); }
        }
        public static readonly DependencyProperty ArrowLengthProperty =
                DependencyProperty.Register("ArrowLength", typeof(double), typeof(ArrowTool),
                new PropertyMetadata(17.0, new PropertyChangedCallback(ArrowTool.OnArrowLengthPropertyChanged)));

        private static void OnArrowLengthPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ArrowTool)
            {
                (obj as ArrowTool).OnArrowLengthValueChanged();
            }
        }

        protected void OnArrowLengthValueChanged()
        {

        }
        #endregion

        #region ArrowAngle DependencyProperty
        public double ArrowAngle
        {
            get { return (double)GetValue(ArrowAngleProperty); }
            set { SetValue(ArrowAngleProperty, value); }
        }
        public static readonly DependencyProperty ArrowAngleProperty =
                DependencyProperty.Register("ArrowAngle", typeof(double), typeof(ArrowTool),
                new PropertyMetadata(0.45, new PropertyChangedCallback(ArrowTool.OnArrowAnglePropertyChanged)));

        private static void OnArrowAnglePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ArrowTool)
            {
                (obj as ArrowTool).OnArrowAngleValueChanged();
            }
        }

        protected void OnArrowAngleValueChanged()
        {

        }
        #endregion

    }
}
