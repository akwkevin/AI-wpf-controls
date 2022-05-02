using AIStudio.Wpf.ComeCapture.Helpers;
using AIStudio.Wpf.ComeCapture.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AIStudio.Wpf.ComeCapture.Controls
{
    public class MainImage : Control
    {
        public Point point;

        private Rectangle _Rectangle = null;
        private Ellipse _Ellipse = null;
        private List<Point> points = null;
        private Path _Line = null;
        private StreamGeometry geometry = new StreamGeometry();
        public TextBoxControl _Text = null;
        private Path _Arrow = null;

        static MainImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainImage), new FrameworkPropertyMetadata(typeof(MainImage)));
        }

        public MainImage()
        {
            _Current = this;
            AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(OnDragStart));
            AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnDragCompleted));
            AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnDragDelta));
            AddHandler(MouseMoveEvent, new MouseEventHandler(OnMove));
            Limit = new Limit();
        }

        #region 属性 Current
        private static MainImage _Current = null;
        public static MainImage Current
        {
            get
            {
                return _Current;
            }
            set
            {
                _Current = value;
            }
        }
        #endregion

        #region MoveCursor DependencyProperty
        public Cursor MoveCursor
        {
            get { return (Cursor)GetValue(MoveCursorProperty); }
            set { SetValue(MoveCursorProperty, value); }
        }
        public static readonly DependencyProperty MoveCursorProperty =
                DependencyProperty.Register("MoveCursor", typeof(Cursor), typeof(MainImage),
                new PropertyMetadata(Cursors.SizeAll, new PropertyChangedCallback(MainImage.OnMoveCursorPropertyChanged)));

        private static void OnMoveCursorPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is MainImage)
            {
                (obj as MainImage).OnMoveCursorValueChanged();
            }
        }

        protected void OnMoveCursorValueChanged()
        {

        }
        #endregion

        #region Direction DependencyProperty
        public Direction Direction
        {
            get { return (Direction)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }
        public static readonly DependencyProperty DirectionProperty =
                DependencyProperty.Register("Direction", typeof(Direction), typeof(MainImage),
                new PropertyMetadata(Direction.Null, new PropertyChangedCallback(MainImage.OnDirectionPropertyChanged)));

        private static void OnDirectionPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is MainImage)
            {
                (obj as MainImage).OnDirectionValueChanged();
            }
        }

        protected void OnDirectionValueChanged()
        {

        }
        #endregion

        #region Limit DependencyProperty
        public Limit Limit
        {
            get { return (Limit)GetValue(LimitProperty); }
            set { SetValue(LimitProperty, value); }
        }
        public static readonly DependencyProperty LimitProperty =
                DependencyProperty.Register("Limit", typeof(Limit), typeof(MainImage),
                new PropertyMetadata(null, new PropertyChangedCallback(MainImage.OnLimitPropertyChanged)));

        private static void OnLimitPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is MainImage)
            {
                (obj as MainImage).OnLimitValueChanged();
            }
        }

        protected void OnLimitValueChanged()
        {

        }
        #endregion

        #region ZoomThumbVisibility DependencyProperty
        public Visibility ZoomThumbVisibility
        {
            get { return (Visibility)GetValue(ZoomThumbVisibilityProperty); }
            set { SetValue(ZoomThumbVisibilityProperty, value); }
        }
        public static readonly DependencyProperty ZoomThumbVisibilityProperty =
                DependencyProperty.Register("ZoomThumbVisibility", typeof(Visibility), typeof(MainImage),
                new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(MainImage.OnZoomThumbVisibilityPropertyChanged)));

        private static void OnZoomThumbVisibilityPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is MainImage)
            {
                (obj as MainImage).OnZoomThumbVisibilityValueChanged();
            }
        }

        protected void OnZoomThumbVisibilityValueChanged()
        {

        }
        #endregion

        #region 开始滑动事件
        private void OnDragStart(object sender, DragStartedEventArgs e)
        {
            Direction = (e.OriginalSource as ZoomThumb).Direction;
            if (SizeColorBar.Current.Selected != Tool.Null)
            {
                point = Mouse.GetPosition(this);
                if (SizeColorBar.Current.Selected == Tool.Text)
                {
                    DrawText();
                }
            }
        }
        #endregion

        #region 滑动中事件
        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var X = e.HorizontalChange;
            var Y = e.VerticalChange;
            switch (Direction)
            {
                case Direction.Move:
                    if (SizeColorBar.Current.Selected == Tool.Null)
                    {
                        OnMove(X, Y);
                    }
                    else
                    {
                        switch (SizeColorBar.Current.Selected)
                        {
                            case Tool.Rectangle:
                                DrawRectangle(X, Y);
                                break;
                            case Tool.Ellipse:
                                DrawEllipse(X, Y);
                                break;
                            case Tool.Arrow:
                                DrawArrow(X, Y);
                                break;
                            case Tool.Line:
                                DrawLine(X, Y);
                                break;
                            case Tool.Text:
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case Direction.Null:
                    break;
                default:
                    var str = Direction.ToString();
                    if (X != 0)
                    {
                        if (str.Contains("Left"))
                        {
                            Left(X);
                        }
                        if (str.Contains("Right"))
                        {
                            Right(X);
                        }
                    }
                    if (Y != 0)
                    {
                        if (str.Contains("Top"))
                        {
                            Top(Y);
                        }
                        if (str.Contains("Bottom"))
                        {
                            Bottom(Y);
                        }
                    }
                    AppModel.Current.ChangeShowSize();
                    break;
            }
            ImageEditBar.Current.ResetCanvas();
            SizeColorBar.Current.ResetCanvas();
        }
        #endregion

        #region 滑动结束事件
        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (Direction == Direction.Move && SizeColorBar.Current.Selected != Tool.Null)
            {
                switch (SizeColorBar.Current.Selected)
                {
                    case Tool.Rectangle:
                        if (_Rectangle != null)
                        {
                            ResetLimit(Canvas.GetLeft(_Rectangle), Canvas.GetTop(_Rectangle), Canvas.GetLeft(_Rectangle) + _Rectangle.Width, Canvas.GetTop(_Rectangle) + _Rectangle.Height);
                            MainWindow.Register(_Rectangle);
                            _Rectangle = null;
                        }
                        break;
                    case Tool.Ellipse:
                        if (_Ellipse != null)
                        {
                            ResetLimit(Canvas.GetLeft(_Ellipse), Canvas.GetTop(_Ellipse), Canvas.GetLeft(_Ellipse) + _Ellipse.Width, Canvas.GetTop(_Ellipse) + _Ellipse.Height);
                            MainWindow.Register(_Ellipse);
                            _Ellipse = null;
                        }
                        break;
                    case Tool.Arrow:
                        if (_Arrow != null)
                        {
                            geometry.Clear();
                            ResetLimit(points.Min(p => p.X), points.Min(p => p.Y), points.Max(p => p.X), points.Max(p => p.Y));
                            points = null;
                            MainWindow.Register(_Arrow);
                            _Arrow = null;
                        }
                        break;
                    case Tool.Line:
                        if (_Line != null)
                        {
                            geometry.Clear();
                            ResetLimit(points.Min(p => p.X), points.Min(p => p.Y), points.Max(p => p.X), points.Max(p => p.Y));
                            points = null;
                            MainWindow.Register(_Line);
                            _Line = null;
                        }
                        break;
                    case Tool.Text:
                        break;
                    default:
                        break;
                }
            }
            Direction = Direction.Null;
        }
        #endregion

        #region 画矩形
        private void DrawRectangle(double X, double Y)
        {
            if (_Rectangle == null)
            {
                _Rectangle = new Rectangle()
                {
                    Fill = new SolidColorBrush(Colors.Transparent),
                    Stroke = RectangleTool.Current.LineBrush,
                    StrokeThickness = RectangleTool.Current.LineThickness
                };
                Panel.SetZIndex(_Rectangle, -1);
                MainWindow.AddControl(_Rectangle);
            }
            if (X > 0)
            {
                Canvas.SetLeft(_Rectangle, point.X + AppModel.Current.MaskLeftWidth);
                _Rectangle.Width = X < Width - point.X ? X : Width - point.X;
            }
            else
            {
                Canvas.SetLeft(_Rectangle, -X < point.X ? point.X + X + AppModel.Current.MaskLeftWidth : AppModel.Current.MaskLeftWidth);
                _Rectangle.Width = -X < point.X ? -X : point.X;
            }
            if (Y > 0)
            {
                Canvas.SetTop(_Rectangle, point.Y + AppModel.Current.MaskTopHeight);
                _Rectangle.Height = Y < Height - point.Y ? Y : Height - point.Y;
            }
            else
            {
                Canvas.SetTop(_Rectangle, -Y < point.Y ? point.Y + Y + AppModel.Current.MaskTopHeight : AppModel.Current.MaskTopHeight);
                _Rectangle.Height = -Y < point.Y ? -Y : point.Y;
            }
        }
        #endregion

        #region 画椭圆
        private void DrawEllipse(double X, double Y)
        {
            if (_Ellipse == null)
            {
                _Ellipse = new Ellipse()
                {
                    Fill = new SolidColorBrush(Colors.Transparent),
                    Stroke = EllipseTool.Current.LineBrush,
                    StrokeThickness = EllipseTool.Current.LineThickness
                };
                Panel.SetZIndex(_Ellipse, -1);
                MainWindow.AddControl(_Ellipse);
            }
            if (X > 0)
            {
                Canvas.SetLeft(_Ellipse, point.X + AppModel.Current.MaskLeftWidth);
                _Ellipse.Width = X < Width - point.X ? X : Width - point.X;
            }
            else
            {
                Canvas.SetLeft(_Ellipse, -X < point.X ? point.X + X + AppModel.Current.MaskLeftWidth : AppModel.Current.MaskLeftWidth);
                _Ellipse.Width = -X < point.X ? -X : point.X;
            }
            if (Y > 0)
            {
                Canvas.SetTop(_Ellipse, point.Y + AppModel.Current.MaskTopHeight);
                _Ellipse.Height = Y < Height - point.Y ? Y : Height - point.Y;
            }
            else
            {
                Canvas.SetTop(_Ellipse, -Y < point.Y ? point.Y + Y + AppModel.Current.MaskTopHeight : AppModel.Current.MaskTopHeight);
                _Ellipse.Height = -Y < point.Y ? -Y : point.Y;
            }
        }
        #endregion

        #region 画箭头
        private void DrawArrow(double X, double Y)
        {
            var screen = new Point(point.X + AppModel.Current.MaskLeftWidth, point.Y + AppModel.Current.MaskTopHeight);
            if (_Arrow == null)
            {
                _Arrow = new Path()
                {
                    Fill = ArrowTool.Current.LineBrush,
                    StrokeThickness = ArrowTool.Current.LineThickness
                };
                Panel.SetZIndex(_Arrow, -1);
                MainWindow.AddControl(_Arrow);
            }
            var point2 = new Point(screen.X + X, screen.Y + Y);
            point2.X = point2.X < AppModel.Current.MaskLeftWidth ? AppModel.Current.MaskLeftWidth : point2.X > AppModel.Current.MaskLeftWidth + Width ? AppModel.Current.MaskLeftWidth + Width : point2.X;
            point2.Y = point2.Y < AppModel.Current.MaskTopHeight ? AppModel.Current.MaskTopHeight : point2.Y > AppModel.Current.MaskTopHeight + Height ? AppModel.Current.MaskTopHeight + Height : point2.Y;
            points = ArrowTool.Current.CreateArrow(screen, point2);

            using (var ctx = geometry.Open())
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (i == 0)
                    {
                        ctx.BeginFigure(points[0], true, false);
                    }
                    else
                    {
                        ctx.LineTo(points[i], true, true);
                    }
                }
            }
            _Arrow.Data = geometry.Clone();
        }
        #endregion

        #region 画刷
        private void DrawLine(double X, double Y)
        {
            var screen = new Point(point.X + AppModel.Current.MaskLeftWidth, point.Y + AppModel.Current.MaskTopHeight);
            if (_Line == null)
            {
                _Line = new Path()
                {
                    Stroke = LineTool.Current.LineBrush,
                    StrokeThickness = LineTool.Current.LineThickness
                };
                points = new List<Point>
                {
                    screen
                };
                Panel.SetZIndex(_Line, -1);
                MainWindow.AddControl(_Line);
            }
            var point2 = new Point(screen.X + X, screen.Y + Y);
            point2.X = point2.X < AppModel.Current.MaskLeftWidth ? AppModel.Current.MaskLeftWidth : point2.X > AppModel.Current.MaskLeftWidth + Width ? AppModel.Current.MaskLeftWidth + Width : point2.X;
            point2.Y = point2.Y < AppModel.Current.MaskTopHeight ? AppModel.Current.MaskTopHeight : point2.Y > AppModel.Current.MaskTopHeight + Height ? AppModel.Current.MaskTopHeight + Height : point2.Y;
            points.Add(point2);
            using (var ctx = geometry.Open())
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (i == 0)
                    {
                        ctx.BeginFigure(points[0], true, false);
                    }
                    else
                    {
                        ctx.LineTo(points[i], true, true);
                    }
                }
            }
            _Line.Data = geometry.Clone();
        }
        #endregion

        #region 添加输入框
        private void DrawText()
        {
            if (_Text != null)
            {
                Focus();
            }
            else
            {
                _Text = new TextBoxControl()
                {
                    FontSize = TextTool.Current.FontSize,
                    Foreground = TextTool.Current.LineBrush
                };
                if (point.X > Width - 36)
                {
                    point.X = Width - 36;
                }
                if (point.Y > Height - 22)
                {
                    point.Y = Height - 22;
                }
                var screen = new Point(point.X + AppModel.Current.MaskLeftWidth, point.Y + AppModel.Current.MaskTopHeight);
                Canvas.SetLeft(_Text, screen.X);
                Canvas.SetTop(_Text, screen.Y);
                MainWindow.AddControl(_Text);
            }
        }
        #endregion

        #region 拖动截图区域
        private void OnMove(double X, double Y)
        {
            #region X轴移动
            if (X > 0)
            {
                var max = AppModel.Current.MaskRightWidth > Limit.Left - AppModel.Current.MaskLeftWidth ? Limit.Left - AppModel.Current.MaskLeftWidth : AppModel.Current.MaskRightWidth;
                if (X > max)
                {
                    X = max;
                }
            }
            else
            {
                var max = AppModel.Current.MaskLeftWidth > AppModel.Current.MaskLeftWidth + Width - Limit.Right ? AppModel.Current.MaskLeftWidth + Width - Limit.Right : AppModel.Current.MaskLeftWidth;
                if (-X > max)
                {
                    X = -max;
                }
            }
            if (X != 0)
            {
                AppModel.Current.MaskLeftWidth += X;
                AppModel.Current.MaskRightWidth -= X;
                Canvas.SetLeft(this, Canvas.GetLeft(this) + X);
            }
            #endregion

            #region Y轴移动
            if (Y > 0)
            {
                var max = AppModel.Current.MaskBottomHeight > Limit.Top - AppModel.Current.MaskTopHeight ? Limit.Top - AppModel.Current.MaskTopHeight : AppModel.Current.MaskBottomHeight;
                if (Y > max)
                {
                    Y = max;
                }
            }
            else
            {
                var max = AppModel.Current.MaskTopHeight > AppModel.Current.MaskTopHeight + Height - Limit.Bottom ? AppModel.Current.MaskTopHeight + Height - Limit.Bottom : AppModel.Current.MaskTopHeight;
                if (-Y > max)
                {
                    Y = -max;
                }
            }
            if (Y != 0)
            {
                AppModel.Current.MaskTopHeight += Y;
                AppModel.Current.MaskBottomHeight -= Y;
                Canvas.SetTop(this, Canvas.GetTop(this) + Y);
            }
            #endregion
        }
        #endregion

        #region 左缩放
        private void Left(double X)
        {
            if (X > 0)
            {
                var max = MainWindow.Current.list.Count == 0 ? Width - MainWindow.MinSize
                    : Limit.Left - AppModel.Current.MaskLeftWidth < Width - MainWindow.MinSize ? Limit.Left - AppModel.Current.MaskLeftWidth
                    : Width - MainWindow.MinSize;
                if (X > max)
                {
                    X = max;
                }
            }
            else
            {
                var max = AppModel.Current.MaskLeftWidth;
                if (-X > max)
                {
                    X = -max;
                }
            }
            if (X != 0)
            {
                Width -= X;
                Canvas.SetLeft(this, Canvas.GetLeft(this) + X);
                AppModel.Current.MaskLeftWidth += X;
                AppModel.Current.MaskTopWidth -= X;
            }
        }
        #endregion

        #region 右缩放
        private void Right(double X)
        {
            if (X > 0)
            {
                var max = AppModel.Current.MaskRightWidth;
                if (X > max)
                {
                    X = max;
                }
            }
            else
            {
                var max = MainWindow.Current.list.Count == 0 ? Width - MainWindow.MinSize
                    : AppModel.Current.MaskLeftWidth + Width - Limit.Right < Width - MainWindow.MinSize ? AppModel.Current.MaskLeftWidth + Width - Limit.Right
                    : Width - MainWindow.MinSize;
                if (-X > max)
                {
                    X = -max;
                }
            }
            if (X != 0)
            {
                Width += X;
                AppModel.Current.MaskRightWidth -= X;
                AppModel.Current.MaskTopWidth += X;
            }
        }
        #endregion

        #region 上缩放
        private void Top(double Y)
        {
            if (Y > 0)
            {
                var max = MainWindow.Current.list.Count == 0 ? Height - MainWindow.MinSize
                    : Limit.Top - AppModel.Current.MaskTopHeight < Height - MainWindow.MinSize ? Limit.Top - AppModel.Current.MaskTopHeight
                    : Height - MainWindow.MinSize;
                if (Y > max)
                {
                    Y = max;
                }
            }
            else
            {
                var max = AppModel.Current.MaskLeftWidth;
                if (-Y > max)
                {
                    Y = -max;
                }
            }
            if (Y != 0)
            {
                Height -= Y;
                Canvas.SetTop(this, Canvas.GetTop(this) + Y);
                AppModel.Current.MaskTopHeight += Y;
            }
        }
        #endregion

        #region 下缩放
        private void Bottom(double Y)
        {
            if (Y > 0)
            {
                var max = AppModel.Current.MaskBottomHeight;
                if (Y > max)
                {
                    Y = max;
                }
            }
            else
            {
                var max = MainWindow.Current.list.Count == 0 ? Height - MainWindow.MinSize
                    : AppModel.Current.MaskTopHeight + Height - Limit.Bottom < Height - MainWindow.MinSize ? AppModel.Current.MaskTopHeight + Height - Limit.Bottom
                    : Height - MainWindow.MinSize;
                if (-Y > max)
                {
                    Y = -max;
                }
            }
            if (Y != 0)
            {
                Height += Y;
                AppModel.Current.MaskBottomHeight -= Y;
            }
        }
        #endregion

        #region 刷新RGB
        private void OnMove(object sender, MouseEventArgs e)
        {
            var point = PointToScreen(e.GetPosition(this));
            AppModel.Current.ShowRGB = ImageHelper.GetRGB((int)point.X, (int)point.Y);
        }
        #endregion

        #region 计算图片移动的极限值
        public void ResetLimit(double left, double top, double right, double bottom)
        {
            ResetLeft(left);
            ResetTop(top);
            ResetRight(right);
            ResetButtom(bottom);
        }

        private void ResetLeft(double value)
        {
            if (value < Limit.Left)
            {
                Limit.Left = value;
            }
        }

        private void ResetTop(double value)
        {
            if (value < Limit.Top)
            {
                Limit.Top = value;
            }
        }

        private void ResetRight(double value)
        {
            if (value > Limit.Right)
            {
                Limit.Right = value;
            }
        }

        private void ResetButtom(double value)
        {
            if (value > Limit.Bottom)
            {
                Limit.Bottom = value;
            }
        }
        #endregion
    }
}
