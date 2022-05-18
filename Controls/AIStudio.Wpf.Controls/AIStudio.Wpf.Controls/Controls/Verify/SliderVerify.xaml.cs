using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_Canvas, Type = typeof(Canvas))]
    [TemplatePart(Name = PART_Pathfix, Type = typeof(Path))]
    [TemplatePart(Name = PART_Path, Type = typeof(Path))]
    [TemplatePart(Name = PART_Slider, Type = typeof(Slider))]
    public class SliderVerify : Control
    {
        private const string PART_Canvas = "PART_Canvas";
        private const string PART_Pathfix = "PART_Pathfix";
        private const string PART_Path = "PART_Path";
        private const string PART_Slider = "PART_Slider";

        private Canvas _myCanvas;
        private Slider _slider;
        private Path _path;
        private Path _pathfix;

        static SliderVerify()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliderVerify), new FrameworkPropertyMetadata(typeof(SliderVerify)));
        }

        public SliderVerify()
        {
            this.Loaded += SliderVerify_Loaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _myCanvas = GetTemplateChild(PART_Canvas) as Canvas;
            _path = GetTemplateChild(PART_Path) as Path;
            _pathfix = GetTemplateChild(PART_Pathfix) as Path;
            _slider = GetTemplateChild(PART_Slider) as Slider;
            if (_slider != null)
            {
                _slider.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(Slider_DragCompleted));
            }
        }

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
            DependencyProperty.Register(nameof(ImageUri), typeof(string), typeof(SliderVerify), new PropertyMetadata(null, (p, d) => {
                (p as SliderVerify).Restart();
            }));

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
            DependencyProperty.Register(nameof(Result), typeof(bool), typeof(SliderVerify), new PropertyMetadata(false));

        #region Routed Event
        public static readonly RoutedEvent ResultChangedEvent = EventManager.RegisterRoutedEvent(nameof(ResultChanged), RoutingStrategy.Bubble, typeof(EventHandler), typeof(SliderVerify));
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

        private double _width = 48;

        private async void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            var thumb = _slider.Template.FindName("thumb", this._slider) as Thumb;
            Path icon = thumb.Template.FindName("icon", thumb) as Path;
            var data = icon.Data;
            var fill = icon.Fill;

            if (Math.Abs(Canvas.GetLeft(_path) - Canvas.GetLeft(_pathfix)) <= 3)
            {
                icon.Fill = Brushes.Green;
                string sData = "M912 190h-69.9c-9.8 0-19.1 4.5-25.1 12.2L404.7 724.5 207 474a32 32 0 0 0-25.1-12.2H112c-6.7 0-10.4 7.7-6.3 12.9l273.9 347c12.8 16.2 37.4 16.2 50.3 0l488.4-618.9c4.1-5.1.4-12.8-6.3-12.8z";
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                icon.Data = (Geometry)converter.ConvertFrom(sData);
                Result = true;
                RaiseResultChanged(Result);
            }
            else
            {
                icon.Fill = Brushes.Red;

                string sData = "M5.22675 -4.10175A0.5625 0.5625 0 0 0 6.02325 -4.10175L9 -7.079625L11.97675 -4.10175A0.5625 0.5625 0 0 0 12.77325 -4.89825L9.795375 -7.875L12.77325 -10.85175A0.5625 0.5625 0 0 0 11.97675 -11.64825L9 -8.670375L6.02325 -11.64825A0.5625 0.5625 0 0 0 5.22675 -10.85175L8.204625 -7.875L5.22675 -4.89825A0.5625 0.5625 0 0 0 5.22675 -4.10175z";
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                icon.Data = (Geometry)converter.ConvertFrom(sData);
                RaiseResultChanged(Result);
            }

            icon.Fill = fill;
            icon.Data = data;
            Restart();
        }

        private void SliderVerify_Loaded(object sender, RoutedEventArgs e)
        {
            //Restart();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Restart();
        }

        private void Restart()
        {
            if (_myCanvas == null || !_myCanvas.IsVisible)
                return;

            Result = false;

            Random ran = new Random();
            double value = ran.Next((int)(_myCanvas.ActualWidth - _width) / 3, (int)(_myCanvas.ActualWidth - _width));
            SetTopCenter(_pathfix);
            SetLeft(_pathfix, value);

            BitmapImage image = GetBitmapImage();
            SetBackground(image);

            SetTopCenter(_path);
            SetFill(_path, image, value);

            _slider.Value = 0;
            _slider.Maximum = this._myCanvas.ActualWidth - _width;
            _slider.ValueChanged -= Slider_ValueChanged;
            _slider.ValueChanged += Slider_ValueChanged;
        }


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetLeft(_path, _slider.Value);
        }

        private BitmapImage GetBitmapImage()
        {
            //BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/AIStudio.Wpf.Controls;component/Resources/1.jpg"));

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

        private void SetFill(Shape element, BitmapImage image, double value)
        {
            ImageBrush ib = new ImageBrush();
            ib.AlignmentX = AlignmentX.Left;
            ib.AlignmentY = AlignmentY.Top;
            ib.ImageSource = image;
            ib.Viewbox = new Rect(value, (_myCanvas.ActualHeight - _path.ActualHeight) / 2, _path.ActualWidth, _path.ActualHeight);
            ib.ViewboxUnits = BrushMappingMode.Absolute; //按百分比设置宽高
            ib.TileMode = TileMode.None; //按百分比应该不会出现 image小于要切的值的情况
            ib.Stretch = Stretch.None;

            element.Fill = ib;
        }

        private void SetTopCenter(FrameworkElement element)
        {
            double top = (_myCanvas.ActualHeight - element.ActualHeight) / 2;
            Canvas.SetTop(element, top);
        }

        private void SetLeft(FrameworkElement element, double left)
        {
            Canvas.SetLeft(element, left);
        }


    }
}
