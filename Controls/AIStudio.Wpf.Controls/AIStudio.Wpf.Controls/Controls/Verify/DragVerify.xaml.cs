using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AIStudio.Wpf.Controls
{

    [TemplatePart(Name = PART_Canvas, Type = typeof(Canvas))]
    [TemplatePart(Name = PART_Info, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_Reset, Type = typeof(Button))]
    public class DragVerify : Control
    {
        private const string PART_Canvas = "PART_Canvas";
        private const string PART_Info = "PART_Info";
        private const string PART_Reset = "PART_Reset";

        private Canvas _myCanvas;
        private TextBlock _txtInfo;
        private Button _btnReset;
        private List<Rectangle> rects;
        private Rectangle dragrect;
        private Rectangle overlayrect;
        private Rectangle temprect;
        private Point mousePoint;
        private int width;
        private int height;

        static DragVerify()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragVerify), new FrameworkPropertyMetadata(typeof(DragVerify)));
        }
        public DragVerify()
        {
            this.Loaded += TextClickVerify_Loaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_myCanvas != null)
            {
                _myCanvas.MouseLeftButtonDown -= MyCanvas_MouseLeftButtonDown;
                _myCanvas.MouseMove -= MyCanvas_MouseMove;
                _myCanvas.MouseLeftButtonUp -= MyCanvas_MouseLeftButtonUp;
            }
            if (_btnReset != null)
            {
                _btnReset.Click -= new RoutedEventHandler(BtnReset_Click);
            }


            _myCanvas = GetTemplateChild(PART_Canvas) as Canvas;
            _txtInfo = GetTemplateChild(PART_Info) as TextBlock;
            _btnReset = GetTemplateChild(PART_Reset) as Button;

            if (_btnReset != null)
            {
                _myCanvas.MouseLeftButtonDown += MyCanvas_MouseLeftButtonDown;
                _myCanvas.MouseMove += MyCanvas_MouseMove;
                _myCanvas.MouseLeftButtonUp += MyCanvas_MouseLeftButtonUp;
            }
            if (_btnReset != null)
            {
                _btnReset.Click += new RoutedEventHandler(BtnReset_Click);
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            Restart();
        }


        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_btnReset.Visibility == Visibility.Visible)
            {
                Restart();
                return;
            }

            if (e.OriginalSource.GetType() == typeof(Rectangle))
            {
                dragrect = (Rectangle)e.OriginalSource;
                dragrect.Stroke = this.Foreground;
                mousePoint = e.GetPosition(_myCanvas);

                Canvas.SetZIndex(dragrect, RowNum * ColumnNum);
            }
        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragrect != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point theMousePoint = e.GetPosition((Canvas)sender);

                    Canvas.SetLeft(dragrect, theMousePoint.X - (mousePoint.X - Canvas.GetLeft(dragrect)));
                    Canvas.SetTop(dragrect, theMousePoint.Y - (mousePoint.Y - Canvas.GetTop(dragrect)));

                    mousePoint = theMousePoint;

                    var oldoverlayrect = overlayrect;
                    overlayrect = GetOverlay(dragrect);
                    if (oldoverlayrect != overlayrect || oldoverlayrect == null || overlayrect == null)
                    {
                        if (oldoverlayrect != null)
                        {
                            oldoverlayrect.Stroke = Brushes.Transparent;
                        }
                        if (temprect != null)
                        {
                            _myCanvas.Children.Remove(temprect);
                        }
                        if (overlayrect != null)
                        {
                            overlayrect.Stroke = this.Foreground;
                            temprect = CopyRectangle(overlayrect);
                            temprect.Tag = dragrect.Tag;
                            ArrayRectangle(temprect, width, height);
                        }
                    }
                }
            }
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (overlayrect != null && dragrect != null)
            {
                var overlayrectTag = overlayrect.Tag as RectangleTag;
                var dragrectTag = dragrect.Tag as RectangleTag;
                if (dragrectTag.Row == overlayrectTag.NewRow && dragrectTag.Column == overlayrectTag.NewColumn)
                {
                    _myCanvas.Children.Remove(overlayrect);
                    dragrect.Tag = overlayrect.Tag;
                    ArrayRectangle(dragrect, width, height);
                    dragrect.Stroke = Brushes.Transparent;
                    temprect.Stroke = Brushes.Transparent;
                    Result = true;
                    RaiseResultChanged(Result);
                    _txtInfo.Visibility = Visibility.Collapsed;
                    _btnReset.Visibility = Visibility.Visible;
                    _btnReset.Content = "验证成功";
                    _btnReset.Background = Brushes.Green;

                    return;
                }
            }
            if (dragrect != null)
            {
                Canvas.SetZIndex(dragrect, 0);
                ArrayRectangle(dragrect, width, height);
                dragrect.Stroke = Brushes.Transparent;
                dragrect = null;
            }
            if (temprect != null)
            {
                _myCanvas.Children.Remove(temprect);
                temprect = null;
            }
            if (overlayrect != null)
            {
                overlayrect.Stroke = Brushes.Transparent;
                overlayrect = null;

                RaiseResultChanged(Result);
                _txtInfo.Visibility = Visibility.Collapsed;
                _btnReset.Visibility = Visibility.Visible;
                _btnReset.Content = "验证失败，请重试";
                _btnReset.Background = Brushes.Red;
            }
        }

        public bool Result
        {
            get
            {
                return (bool)GetValue(ResultProperty);
            }
            set
            {
                SetValue(ResultProperty, value);
            }
        }

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register(nameof(Result), typeof(bool), typeof(DragVerify), new PropertyMetadata(false));

        public string ImageUri
        {
            get
            {
                return (string)GetValue(ImageUriProperty);
            }
            set
            {
                SetValue(ImageUriProperty, value);
            }
        }

        public static readonly DependencyProperty ImageUriProperty =
            DependencyProperty.Register(nameof(ImageUri), typeof(string), typeof(DragVerify), new PropertyMetadata(null));

        public int RowNum
        {
            get
            {
                return (int)GetValue(RowNumProperty);
            }
            set
            {
                SetValue(RowNumProperty, value);
            }
        }

        public static readonly DependencyProperty RowNumProperty =
            DependencyProperty.Register(nameof(RowNum), typeof(int), typeof(DragVerify), new PropertyMetadata(3));

        public int ColumnNum
        {
            get
            {
                return (int)GetValue(ColumnNumProperty);
            }
            set
            {
                SetValue(ColumnNumProperty, value);
            }
        }

        public static readonly DependencyProperty ColumnNumProperty =
            DependencyProperty.Register(nameof(ColumnNum), typeof(int), typeof(DragVerify), new PropertyMetadata(3));

        #region Routed Event
        public static readonly RoutedEvent ResultChangedEvent = EventManager.RegisterRoutedEvent(nameof(ResultChanged), RoutingStrategy.Bubble, typeof(EventHandler), typeof(DragVerify));
        public event EventHandler ResultChanged
        {
            add
            {
                AddHandler(ResultChangedEvent, value);
            }
            remove
            {
                RemoveHandler(ResultChangedEvent, value);
            }
        }
        void RaiseResultChanged(bool result)
        {
            var arg = new RoutedEventArgs(ResultChangedEvent, result);
            RaiseEvent(arg);
        }
        #endregion

        private void TextClickVerify_Loaded(object sender, RoutedEventArgs e)
        {
            //Restart();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Restart();
        }

        private void Restart()
        {
            if (!_myCanvas.IsVisible)
                return;

            Result = false;

            width = (int)(_myCanvas.ActualWidth);
            height = (int)(_myCanvas.ActualHeight);
            BitmapImage image = GetBitmapImage(width, height);

            _myCanvas.Children.Clear();
            rects = new List<Rectangle>();
            dragrect = null;
            overlayrect = null;

            for (int i = 0; i < RowNum; i++)
            {
                for (int j = 0; j < ColumnNum; j++)
                {
                    Rectangle rect = GetChild(image, i, j, width, height);
                    rects.Add(rect);
                }
            }

            Random ran = new Random();
            var changedrect = rects.OrderBy(p => ran.NextDouble()).Take(2).ToArray();
            var rectTag0 = changedrect[0].Tag as RectangleTag;
            var rectTag1 = changedrect[1].Tag as RectangleTag;
            rectTag0.NewRow = rectTag1.Row;
            rectTag0.NewColumn = rectTag1.Column;
            rectTag1.NewRow = rectTag0.Row;
            rectTag1.NewColumn = rectTag0.Column;

            foreach (var rect in rects)
            {
                ArrayRectangle(rect, width, height);
            }

            _txtInfo.Visibility = Visibility.Visible;
            _txtInfo.Text = "拖动交换2个图块复原图片";
            _btnReset.Visibility = Visibility.Collapsed;
            _btnReset.Background = Brushes.Transparent;
        }

        private Rectangle GetChild(BitmapImage image, int row, int column, int width, int height)
        {
            Rectangle rectangle = new Rectangle()
            {
                Width = width / ColumnNum,
                Height = height / RowNum,
                StrokeThickness = 2,
                Stroke = Brushes.Transparent,
            };

            ImageBrush ib = new ImageBrush();
            ib.AlignmentX = AlignmentX.Left;
            ib.AlignmentY = AlignmentY.Top;
            ib.ImageSource = image;
            ib.Viewbox = new Rect(column * width / ColumnNum, row * height / RowNum, width, height);
            ib.ViewboxUnits = BrushMappingMode.Absolute; //按百分比设置宽高
            ib.TileMode = TileMode.None; //按百分比应该不会出现 image小于要切的值的情况
            ib.Stretch = Stretch.None;

            rectangle.Fill = ib;
            //SetLeft(rectangle, column * _width / ColumnNum + column * 3);
            //SetTop(rectangle, row * height / RowNum  + row * 3);

            RectangleTag rectangleTag = new RectangleTag()
            {
                Row = row,
                Column = column,
                NewRow = row,
                NewColumn = column,
            };

            rectangle.Tag = rectangleTag;

            return rectangle;
        }

        public void ArrayRectangle(Rectangle rectangle, int width, int height)
        {
            var rectTag = rectangle.Tag as RectangleTag;
            SetLeft(rectangle, rectTag.NewColumn * width / ColumnNum);
            SetTop(rectangle, rectTag.NewRow * height / RowNum);

            if (!_myCanvas.Children.Contains(rectangle))
            {
                _myCanvas.Children.Add(rectangle);
            }
        }



        private BitmapImage GetBitmapImage(int width, int height)
        {
            Random ran = new Random();
            int value = ran.Next(1, 3);

            if (!string.IsNullOrEmpty(ImageUri))
            {
                // Create source.
                BitmapImage image = new BitmapImage();
                // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                image.BeginInit();
                image.UriSource = new Uri(ImageUri);
                image.DecodePixelWidth = width;
                image.DecodePixelHeight = height;
                image.EndInit();

                return image;
            }

            return null;
        }


        private void SetLeft(FrameworkElement element, double left)
        {
            Canvas.SetLeft(element, left);
        }

        private void SetTop(FrameworkElement element, double top)
        {
            Canvas.SetTop(element, top);
        }

        private Rectangle GetOverlay(Rectangle rectangle)
        {
            var left = Canvas.GetLeft(rectangle);
            var top = Canvas.GetTop(rectangle);
            var rect = rects.Where(p => p != rectangle && (Math.Sqrt(Math.Pow(Canvas.GetLeft(p) - left, 2) + Math.Pow(Canvas.GetTop(p) - top, 2)) < (rectangle.Width + rectangle.Height) / 4)).FirstOrDefault();
            return rect;
        }

        private Rectangle CopyRectangle(Rectangle rectangle)
        {
            string xaml = System.Windows.Markup.XamlWriter.Save(rectangle);
            return System.Windows.Markup.XamlReader.Parse(xaml) as Rectangle;
        }

    }

    public class RectangleTag
    {
        public int Row
        {
            get; set;
        }
        public int Column
        {
            get; set;
        }
        public int NewRow
        {
            get; set;
        }
        public int NewColumn
        {
            get; set;
        }
    }
}
