using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    public class TextClickVerify : Control
    {
        private const string PART_Canvas = "PART_Canvas";
        private const string PART_Info = "PART_Info";
        private const string PART_Reset = "PART_Reset";

        private Canvas _myCanvas;
        private TextBlock _txtInfo;
        private Button _btnReset;


        static TextClickVerify()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextClickVerify), new FrameworkPropertyMetadata(typeof(TextClickVerify)));
        }

        public TextClickVerify()
        {
            this.Loaded += TextClickVerify_Loaded; ;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_myCanvas != null)
            {
                _myCanvas.MouseLeftButtonDown -= MyCanvas_MouseLeftButtonDown;
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

            var position = e.GetPosition(_myCanvas);
            AddPath(clicklength - strs.Count + 1, position.X, position.Y);

            if (e.OriginalSource.GetType() == typeof(Grid))
            {
                Grid grid = (Grid)e.OriginalSource;
                if (grid.Tag.ToString() == strs.FirstOrDefault())
                {
                    strs.RemoveAt(0);
                    if (strs.Count == 0)
                    {
                        Result = true;
                        RaiseResultChanged(Result);
                        _txtInfo.Visibility = Visibility.Collapsed;
                        _btnReset.Visibility = Visibility.Visible;
                        _btnReset.Content = "验证成功";
                        _btnReset.Background = Brushes.Green;
                    }
                }
                else
                {

                    RaiseResultChanged(Result);
                    _txtInfo.Visibility = Visibility.Collapsed;
                    _btnReset.Visibility = Visibility.Visible;
                    _btnReset.Content = "验证失败，请重试";
                    _btnReset.Background = Brushes.Red;
                }
            }
            else
            {
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
            DependencyProperty.Register(nameof(Result), typeof(bool), typeof(TextClickVerify), new PropertyMetadata(false));

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
            DependencyProperty.Register(nameof(ImageUri), typeof(string), typeof(TextClickVerify), new PropertyMetadata(null));

        #region Routed Event
        public static readonly RoutedEvent ResultChangedEvent = EventManager.RegisterRoutedEvent(nameof(ResultChanged), RoutingStrategy.Bubble, typeof(EventHandler), typeof(TextClickVerify));
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

        private List<string> strs;
        private int clicklength;
        private void Restart()
        {
            if (!_myCanvas.IsVisible)
                return;

            Result = false;

            Random ran = new Random();

            BitmapImage image = GetBitmapImage();
            SetBackground(image);

            //获取GB2312编码页（表） 
#if NETCOREAPP
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif
            Encoding gb = Encoding.GetEncoding("GB2312");
            string tooltip = string.Empty;
            if (IsEmoji)
            {
                var emoji = EmojiText.Emoji.Value.OrderBy(p => ran.NextDouble()).Take(4).ToList();

                strs = new List<string>();
                strs.Add(emoji[0]);
                strs.Add(emoji[1]);
                strs.Add(emoji[2]);
                strs.Add(emoji[3]);
                clicklength = 3;
            }
            else if (IsIdioms)
            {
                var idioms = IdiomsText.Idioms.Value;
                var idiomkey = idioms.Keys.ToArray()[ran.Next(0, idioms.Count)];
                tooltip = idioms[idiomkey];
                clicklength = idiomkey.Length;

                strs = new List<string>();
                for (int i = 0; i < clicklength; i++)
                {
                    strs.Add(idiomkey.Substring(i, 1));
                }
            }
            else
            {
                //调用函数产生4个随机中文汉字编码 
                object[] bytes = ChineseCode.CreateRegionCode(4);

                //根据汉字编码的字节数组解码出中文汉字 
                strs = new List<string>();
                strs.Add(gb.GetString((byte[])Convert.ChangeType(bytes[0], typeof(byte[]))));
                strs.Add(gb.GetString((byte[])Convert.ChangeType(bytes[1], typeof(byte[]))));
                strs.Add(gb.GetString((byte[])Convert.ChangeType(bytes[2], typeof(byte[]))));
                strs.Add(gb.GetString((byte[])Convert.ChangeType(bytes[3], typeof(byte[]))));
                clicklength = 3;
            }


            int width = (int)(_myCanvas.ActualWidth - 30);
            int height = (int)(_myCanvas.ActualHeight - 40);
            var brush = this.Background;

            var clickstrs = strs.OrderBy(p => ran.NextDouble()).ToArray();
            _myCanvas.Children.Clear();
            for (int i = 0; i < strs.Count; i++)
            {
                AddChild(clickstrs[i], i, strs.Count, brush, width, height, ran);
            }
            strs = strs.Take(clicklength).ToList();

            _txtInfo.Visibility = Visibility.Visible;
            if (clicklength == 3)
            {
                _txtInfo.Text = $"请依次点击\"{strs[0]}\"\"{strs[1]}\"\"{strs[2]}\"";
                _txtInfo.ToolTip = _txtInfo.Text;
            }
            else
            {
                _txtInfo.Text = "请按语序依次点击文字";
                _txtInfo.ToolTip = $"\"{string.Join("", strs)}\"{tooltip}";
            }
            _btnReset.Visibility = Visibility.Collapsed;
            _btnReset.Background = Brushes.Transparent;
        }

        public void AddChild(string str, int index, int totalindex, Brush brush, int width, int height, Random ran)
        {
            Grid grid = new Grid();
            grid.Tag = str;
            OutlineText outlinetext = new OutlineText()
            {
                FontSize = 30,
                Text = str,
                FontWeight = FontWeights.Bold,
                Fill = new SolidColorBrush(Color.FromRgb(Convert.ToByte(ran.Next(0, 255)), Convert.ToByte(ran.Next(0, 255)), Convert.ToByte(ran.Next(0, 255)))),//brush,
                IsHitTestVisible = false,
            };
            grid.Children.Add(outlinetext);

            SetLeft(grid, ran.Next((int)(width * index / totalindex), (int)(width * (index + 1) / totalindex)));
            SetTop(grid, ran.Next(0, (int)height));
            RotateTransform rtf = new RotateTransform(ran.Next(0, 360), 15, 20);
            grid.RenderTransform = rtf;

            grid.Background = new SolidColorBrush(Colors.Transparent);
            _myCanvas.Children.Add(grid);
        }

        private void AddPath(int number, double left, double top)
        {
            Grid grid = new Grid();

            Path path = new Path();
            path.Fill = this.Background;
            path.Stroke = this.Foreground;
            string sData = "M12,2A7,7 0 0,0 5,9C5,14.25 12,22 12,22C12,22 19,14.25 19,9A7,7 0 0,0 12,2Z";
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            path.Data = (Geometry)converter.ConvertFrom(sData);
            path.Height = 40;
            path.Width = 30;
            path.StrokeThickness = 2;
            path.Stretch = Stretch.Fill;
            path.HorizontalAlignment = HorizontalAlignment.Center;
            path.VerticalAlignment = VerticalAlignment.Center;

            grid.Children.Add(path);

            TextBlock text = new TextBlock();
            text.Text = number.ToString();
            text.Foreground = this.Foreground;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;
            text.Margin = new Thickness(0, 0, 0, 2);
            grid.Children.Add(text);

            _myCanvas.Children.Add(grid);
            SetLeft(grid, left - path.Width / 2);
            SetTop(grid, top - path.Height);
        }

        private BitmapImage GetBitmapImage()
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
                image.DecodePixelWidth = (int)_myCanvas.ActualWidth;
                image.DecodePixelHeight = (int)_myCanvas.ActualHeight;
                image.EndInit();

                return image;
            }
            return null;
        }

        private void SetBackground(BitmapImage image)
        {
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = image;

            _myCanvas.Background = ib;
        }


        private void SetVerCenter(FrameworkElement element)
        {
            double top = (_myCanvas.ActualHeight - element.ActualHeight) / 2;
            Canvas.SetTop(element, top);
        }

        private void SetLeft(FrameworkElement element, double left)
        {
            Canvas.SetLeft(element, left);
        }

        private void SetTop(FrameworkElement element, double top)
        {
            Canvas.SetTop(element, top);
        }

        public bool IsEmoji
        {
            get
            {
                return (bool)GetValue(IsEmojiProperty);
            }
            set
            {
                SetValue(IsEmojiProperty, value);
            }
        }

        public static readonly DependencyProperty IsEmojiProperty =
            DependencyProperty.Register("IsEmoji", typeof(bool), typeof(TextClickVerify), new PropertyMetadata(false));

        public bool IsIdioms
        {
            get
            {
                return (bool)GetValue(IsIdiomsProperty);
            }
            set
            {
                SetValue(IsIdiomsProperty, value);
            }
        }

        public static readonly DependencyProperty IsIdiomsProperty =
            DependencyProperty.Register("IsIdioms", typeof(bool), typeof(TextClickVerify), new PropertyMetadata(false));

    }
}
