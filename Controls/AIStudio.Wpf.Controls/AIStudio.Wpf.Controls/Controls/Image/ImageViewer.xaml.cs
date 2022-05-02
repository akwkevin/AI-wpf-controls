using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using AIStudio.Wpf.Controls.Helper;
using Microsoft.Win32;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = ElementPanelMain, Type = typeof(Panel))]
    [TemplatePart(Name = ElementCanvasSmallImg, Type = typeof(Canvas))]
    [TemplatePart(Name = ElementBorderMove, Type = typeof(Border))]
    [TemplatePart(Name = ElementBorderBottom, Type = typeof(Border))]
    [TemplatePart(Name = ElementImageMain, Type = typeof(Image))]
    [TemplatePart(Name = PART_SaveButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_OpenButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_ReduceButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_EnlargeButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_RestoreButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_RotateLeftButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_RotateRightButton, Type = typeof(Button))]
    public class ImageViewer : Control
    {
        #region Constants

        private const string ElementPanelMain = "PART_PanelMain";
        private const string ElementCanvasSmallImg = "PART_CanvasSmallImg";
        private const string ElementBorderMove = "PART_BorderMove";
        private const string ElementBorderBottom = "PART_BorderBottom";
        private const string ElementImageMain = "PART_ImageMain";
        private const string PART_SaveButton = "PART_SaveButton";
        private const string PART_OpenButton = "PART_OpenButton";
        private const string PART_ReduceButton = "PART_ReduceButton";
        private const string PART_EnlargeButton = "PART_EnlargeButton";
        private const string PART_RestoreButton = "PART_RestoreButton";
        private const string PART_RotateLeftButton = "PART_RotateLeftButton";
        private const string PART_RotateRightButton = "PART_RotateRightButton";


        /// <summary>
        ///     缩放比间隔
        /// </summary>
        private const double ScaleInternal = 0.2;

        #endregion Constants

        #region Data

        /// <summary>
        ///     图片保存对话框
        /// </summary>
        private static readonly SaveFileDialog SaveFileDialog = new SaveFileDialog
        {
            Filter = $"{"PngImg"}|*.png"
        };

        private Panel _panelMain;

        private Canvas _canvasSmallImg;

        private Border _borderMove;

        private Border _borderBottom;

        private Image _imageMain;

        private Button _saveButton;
        private Button _openButton;
        private Button _reduceButton;
        private Button _enlargeButton;
        private Button _restoreButton;
        private Button _rotateleftButton;
        private Button _rotaterightButton;
        /// <summary>
        ///     右下角小图片是否加载过
        /// </summary>
        private bool _borderSmallIsLoaded;

        /// <summary>
        ///     图片是否可以在x轴方向移动
        /// </summary>
        private bool _canMoveX;

        /// <summary>
        ///     图片是否可以在y轴方向移动
        /// </summary>
        private bool _canMoveY;

        /// <summary>
        ///     图片实际位置
        /// </summary>
        private Thickness _imgActualMargin;

        /// <summary>
        ///     图片实际旋转角度
        /// </summary>
        private double _imgActualRotate;

        /// <summary>
        ///     图片实际旋缩放比
        /// </summary>
        private double _imgActualScale = 1;

        /// <summary>
        ///     在图片上鼠标移动时的即时位置
        /// </summary>
        private Point _imgCurrentPoint;

        /// <summary>
        ///     鼠标是否在图片上按下左键
        /// </summary>
        private bool _imgIsMouseLeftButtonDown;

        /// <summary>
        ///     在图片上按下时图片的位置
        /// </summary>
        private Thickness _imgMouseDownMargin;

        /// <summary>
        ///     在图片上按下时鼠标的位置
        /// </summary>
        private Point _imgMouseDownPoint;

        /// <summary>
        ///     在小图片上鼠标移动时的即时位置
        /// </summary>
        private Point _imgSmallCurrentPoint;

        /// <summary>
        ///     鼠标是否在小图片上按下左键
        /// </summary>
        private bool _imgSmallIsMouseLeftButtonDown;

        /// <summary>
        ///     在小图片上按下时图片的位置
        /// </summary>
        private Thickness _imgSmallMouseDownMargin;

        /// <summary>
        ///     在小图片上按下时鼠标的位置
        /// </summary>
        private Point _imgSmallMouseDownPoint;

        /// <summary>
        ///     图片长宽比
        /// </summary>
        private double _imgWidHeiScale;

        /// <summary>
        ///     图片是否倾斜
        /// </summary>
        private bool _isOblique;

        /// <summary>
        ///     缩放高度间隔
        /// </summary>
        private double _scaleInternalHeight;

        /// <summary>
        ///     缩放宽度间隔
        /// </summary>
        private double _scaleInternalWidth;

        /// <summary>
        ///     底部BorderBottom（包含一些图片操作）是否显示中
        /// </summary>
        private bool _showBorderBottom;

        #endregion Data

        #region ctor

        public ImageViewer()
        {
            Loaded += (s, e) => Init();
        }

        /// <summary>
        ///     带一个图片Uri的构造函数
        /// </summary>
        /// <param name="uri"></param>
        public ImageViewer(Uri uri) : this()
        {
            try
            {
                ImageSource = BitmapFrame.Create(uri);
                ImgPath = uri.AbsolutePath;
                if (File.Exists(ImgPath))
                {
                    var info = new FileInfo(ImgPath);
                    ImgSize = info.Length;
                }
            }
            catch
            {
                MessageBox.Show("ErrorImgPath");
            }
        }

        /// <summary>
        ///     带一个图片路径的构造函数
        /// </summary>
        /// <param name="path"></param>
        public ImageViewer(string path) : this(new Uri(path))
        {
        }

        #endregion

        #region Properties

        public static readonly DependencyProperty ShowImgMapProperty = DependencyProperty.Register(
            nameof(ShowImgMap), typeof(bool), typeof(ImageViewer), new PropertyMetadata(false));

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            nameof(ImageSource), typeof(BitmapFrame), typeof(ImageViewer), new PropertyMetadata(default(BitmapFrame), OnImageSourceChanged));

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (ImageViewer)d;
            ctl.Init();
        }

        public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register(
            nameof(IsFullScreen), typeof(bool), typeof(ImageViewer), new PropertyMetadata(false));

        internal static readonly DependencyProperty ImgPathProperty = DependencyProperty.Register(
            nameof(ImgPath), typeof(string), typeof(ImageViewer), new PropertyMetadata(default(string)));

        internal static readonly DependencyProperty ImgSizeProperty = DependencyProperty.Register(
            nameof(ImgSize), typeof(long), typeof(ImageViewer), new PropertyMetadata(-1L));

        /// <summary>
        ///     是否显示全屏按钮
        /// </summary>
        internal static readonly DependencyProperty ShowFullScreenButtonProperty = DependencyProperty.Register(
            nameof(ShowFullScreenButton), typeof(bool), typeof(ImageViewer), new PropertyMetadata(false));

        /// <summary>
        ///     关闭按钮是否显示中
        /// </summary>
        internal static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register(
            nameof(ShowCloseButton), typeof(bool), typeof(ImageViewer), new PropertyMetadata(false));

        internal static readonly DependencyProperty ImageContentProperty = DependencyProperty.Register(
            nameof(ImageContent), typeof(object), typeof(ImageViewer), new PropertyMetadata(default(object)));

        internal static readonly DependencyProperty ImageMarginProperty = DependencyProperty.Register(
            nameof(ImageMargin), typeof(Thickness), typeof(ImageViewer), new PropertyMetadata(default(Thickness)));

        internal static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register(
            nameof(ImageWidth), typeof(double), typeof(ImageViewer), new PropertyMetadata(0d));

        internal static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register(
            nameof(ImageHeight), typeof(double), typeof(ImageViewer), new PropertyMetadata(0d));

        internal static readonly DependencyProperty ImageScaleProperty = DependencyProperty.Register(
            nameof(ImageScale), typeof(double), typeof(ImageViewer), new PropertyMetadata(1d, OnImageScaleChanged));

        internal static readonly DependencyProperty ScaleStrProperty = DependencyProperty.Register(
            nameof(ScaleStr), typeof(string), typeof(ImageViewer), new PropertyMetadata("100%"));

        internal static readonly DependencyProperty ImageRotateProperty = DependencyProperty.Register(
            nameof(ImageRotate), typeof(double), typeof(ImageViewer), new PropertyMetadata(0d));

        internal static readonly DependencyProperty ShowSmallImgInternalProperty = DependencyProperty.Register(
            nameof(ShowSmallImgInternal), typeof(bool), typeof(ImageViewer), new PropertyMetadata(false));

        public bool IsFullScreen
        {
            get => (bool)GetValue(IsFullScreenProperty);
            set => SetValue(IsFullScreenProperty, value);
        }

        public bool ShowImgMap
        {
            get => (bool)GetValue(ShowImgMapProperty);
            set => SetValue(ShowImgMapProperty, value);
        }

        public BitmapFrame ImageSource
        {
            get => (BitmapFrame)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        internal object ImageContent
        {
            get => GetValue(ImageContentProperty);
            set => SetValue(ImageContentProperty, value);
        }

        internal string ImgPath
        {
            get => (string)GetValue(ImgPathProperty);
            set => SetValue(ImgPathProperty, value);
        }

        internal long ImgSize
        {
            get => (long)GetValue(ImgSizeProperty);
            set => SetValue(ImgSizeProperty, value);
        }

        /// <summary>
        ///     是否显示全屏按钮
        /// </summary>
        internal bool ShowFullScreenButton
        {
            get => (bool)GetValue(ShowFullScreenButtonProperty);
            set => SetValue(ShowFullScreenButtonProperty, value);
        }

        internal Thickness ImageMargin
        {
            get => (Thickness)GetValue(ImageMarginProperty);
            set => SetValue(ImageMarginProperty, value);
        }

        internal double ImageWidth
        {
            get => (double)GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }

        internal double ImageHeight
        {
            get => (double)GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }

        internal double ImageScale
        {
            get => (double)GetValue(ImageScaleProperty);
            set => SetValue(ImageScaleProperty, value);
        }

        internal string ScaleStr
        {
            get => (string)GetValue(ScaleStrProperty);
            set => SetValue(ScaleStrProperty, value);
        }

        internal double ImageRotate
        {
            get => (double)GetValue(ImageRotateProperty);
            set => SetValue(ImageRotateProperty, value);
        }

        internal bool ShowSmallImgInternal
        {
            get => (bool)GetValue(ShowSmallImgInternalProperty);
            set => SetValue(ShowSmallImgInternalProperty, value);
        }

        /// <summary>
        ///     图片原始宽度
        /// </summary>
        private double ImageOriWidth
        {
            get; set;
        }

        /// <summary>
        ///     图片原始高度
        /// </summary>
        private double ImageOriHeight
        {
            get; set;
        }

        /// <summary>
        ///     关闭按钮是否显示中
        /// </summary>
        internal bool ShowCloseButton
        {
            get => (bool)GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, value);
        }

        /// <summary>
        ///     底部BorderBottom（包含一些图片操作）是否显示中
        /// </summary>
        internal bool ShowBorderBottom
        {
            get => _showBorderBottom;
            set
            {
                if (_showBorderBottom == value) return;
                _borderBottom?.BeginAnimation(OpacityProperty,
                    value ? AnimationHelper.CreateAnimation(1, 100) : AnimationHelper.CreateAnimation(0, 400));
                _showBorderBottom = value;
            }
        }

        #endregion

        public override void OnApplyTemplate()
        {
            if (_imageMain != null) _imageMain.MouseLeftButtonDown -= ImageMain_OnMouseLeftButtonDown;

            if (_canvasSmallImg != null)
            {
                _canvasSmallImg.MouseLeftButtonDown -= CanvasSmallImg_OnMouseLeftButtonDown;
                _canvasSmallImg.MouseLeftButtonUp -= CanvasSmallImg_OnMouseLeftButtonUp;
                _canvasSmallImg.MouseMove -= CanvasSmallImg_OnMouseMove;
                _canvasSmallImg.MouseLeave -= CanvasSmallImg_OnMouseLeave;
            }

            base.OnApplyTemplate();

            _panelMain = GetTemplateChild(ElementPanelMain) as Panel;
            _canvasSmallImg = GetTemplateChild(ElementCanvasSmallImg) as Canvas;
            _borderMove = GetTemplateChild(ElementBorderMove) as Border;
            _imageMain = GetTemplateChild(ElementImageMain) as Image;
            _borderBottom = GetTemplateChild(ElementBorderBottom) as Border;
            _saveButton = GetTemplateChild(PART_SaveButton) as Button;
            _openButton = GetTemplateChild(PART_OpenButton) as Button;
            _reduceButton = GetTemplateChild(PART_ReduceButton) as Button;
            _enlargeButton = GetTemplateChild(PART_EnlargeButton) as Button;
            _restoreButton = GetTemplateChild(PART_RestoreButton) as Button;
            _rotateleftButton = GetTemplateChild(PART_RotateLeftButton) as Button;
            _rotaterightButton = GetTemplateChild(PART_RotateRightButton) as Button;

            if (_imageMain != null)
            {
                var t = new RotateTransform();
                BindingOperations.SetBinding(t, RotateTransform.AngleProperty, new Binding(ImageRotateProperty.Name) { Source = this });
                _imageMain.LayoutTransform = t;
                _imageMain.MouseLeftButtonDown += ImageMain_OnMouseLeftButtonDown;
            }

            if (_canvasSmallImg != null)
            {
                _canvasSmallImg.MouseLeftButtonDown += CanvasSmallImg_OnMouseLeftButtonDown;
                _canvasSmallImg.MouseLeftButtonUp += CanvasSmallImg_OnMouseLeftButtonUp;
                _canvasSmallImg.MouseMove += CanvasSmallImg_OnMouseMove;
                _canvasSmallImg.MouseLeave += CanvasSmallImg_OnMouseLeave;
            }

            _borderSmallIsLoaded = false;

            if (_saveButton != null)
            {
                _saveButton.Click += ButtonSave_OnClick;
            }
            if (_openButton != null)
            {
                _openButton.Click += ButtonWindowsOpen_OnClick;
            }
            if (_restoreButton != null)
            {
                _restoreButton.Click += ButtonActual_OnClick;
            }
            if (_reduceButton != null)
            {
                _reduceButton.Click += ButtonReduce_OnClick;
            }
            if (_enlargeButton != null)
            {
                _enlargeButton.Click += ButtonEnlarge_OnClick;
            }
            if (_rotateleftButton != null)
            {
                _rotateleftButton.Click += ButtonRotateLeft_OnClick;
            }
            if (_rotaterightButton != null)
            {
                _rotaterightButton.Click += ButtonRotateRight_OnClick;
            }

        }

        private static void OnImageScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewer imageViewer && e.NewValue is double newValue)
            {
                imageViewer.ImageWidth = imageViewer.ImageOriWidth * newValue;
                imageViewer.ImageHeight = imageViewer.ImageOriHeight * newValue;
                imageViewer.ScaleStr = $"{newValue * 100:#0}%";
            }
        }

        /// <summary>
        ///     初始化
        /// </summary>
        private void Init()
        {
            if (ImageSource == null || !IsLoaded) return;

            double width;
            double height;

            if (!_isOblique)
            {
                width = ImageSource.Width;
                height = ImageSource.Height;
            }
            else
            {
                width = ImageSource.Height;
                height = ImageSource.Width;
            }

            ImageWidth = width;
            ImageHeight = height;
            ImageOriWidth = width;
            ImageOriHeight = height;
            _scaleInternalWidth = ImageOriWidth * ScaleInternal;
            _scaleInternalHeight = ImageOriHeight * ScaleInternal;

            if (Math.Abs(height - 0) < 0.001 || Math.Abs(width - 0) < 0.001)
            {
                MessageBox.Show("ErrorImgSize");
                return;
            }

            _imgWidHeiScale = width / height;
            var scaleWindow = ActualWidth / ActualHeight;
            if (_imgWidHeiScale > scaleWindow)
            {
                if (width > ActualWidth)
                    ImageScale = ActualWidth / width;
            }
            else if (height > ActualHeight)
            {
                ImageScale = ActualHeight / height;
            }
            else
            {
                ImageScale = 1;
            }

            ImageMargin = new Thickness((ActualWidth - ImageWidth) / 2, (ActualHeight - ImageHeight) / 2, 0, 0);

            _imgActualScale = ImageScale;
            _imgActualMargin = ImageMargin;

            InitBorderSmall();
        }

        private void ButtonActual_OnClick(object sender, RoutedEventArgs e)
        {
            var scaleAnimation = AnimationHelper.CreateAnimation(1);
            scaleAnimation.FillBehavior = FillBehavior.Stop;
            _imgActualScale = 1;
            scaleAnimation.Completed += (s, e1) => {
                ImageScale = 1;
                _canMoveX = ImageWidth > ActualWidth;
                _canMoveY = ImageHeight > ActualHeight;
                BorderSmallShowSwitch();
            };
            var thickness = new Thickness((ActualWidth - ImageOriWidth) / 2, (ActualHeight - ImageOriHeight) / 2, 0, 0);
            var marginAnimation = AnimationHelper.CreateAnimation(thickness);
            marginAnimation.FillBehavior = FillBehavior.Stop;
            _imgActualMargin = thickness;
            marginAnimation.Completed += (s, e1) => { ImageMargin = thickness; };

            BeginAnimation(ImageScaleProperty, scaleAnimation);
            BeginAnimation(ImageMarginProperty, marginAnimation);
        }

        private void ButtonReduce_OnClick(object sender, RoutedEventArgs e) => ScaleImg(false);

        private void ButtonEnlarge_OnClick(object sender, RoutedEventArgs e) => ScaleImg(true);

        private void ButtonRotateLeft_OnClick(object sender, RoutedEventArgs e) => RotateImg(_imgActualRotate - 90);

        private void ButtonRotateRight_OnClick(object sender, RoutedEventArgs e) => RotateImg(_imgActualRotate + 90);

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (ImageSource == null) return;
            SaveFileDialog.FileName = $"{DateTime.Now:yyyy-M-d-h-m-s.fff}";
            if (SaveFileDialog.ShowDialog() == true)
            {
                var path = SaveFileDialog.FileName;
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(ImageSource));
                    encoder.Save(fileStream);
                }
            }
        }

        private void ButtonWindowsOpen_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(ImgPath);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            MoveImg();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            _imgIsMouseLeftButtonDown = false;
            ShowBorderBottom = false;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            ScaleImg(e.Delta > 0);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            OnRenderSizeChanged();
        }

        private void OnRenderSizeChanged()
        {
            if (ImageWidth < 0.001 || ImageHeight < 0.001) return;

            _canMoveX = true;
            _canMoveY = true;

            var marginX = ImageMargin.Left;
            var marginY = ImageMargin.Top;

            if (ImageWidth <= ActualWidth)
            {
                _canMoveX = false;
                marginX = (ActualWidth - ImageWidth) / 2;
            }

            if (ImageHeight <= ActualHeight)
            {
                _canMoveY = false;
                marginY = (ActualHeight - ImageHeight) / 2;
            }

            ImageMargin = new Thickness(marginX, marginY, 0, 0);
            _imgActualMargin = ImageMargin;

            BorderSmallShowSwitch();
            _imgSmallMouseDownMargin = _borderMove.Margin;
            MoveSmallImg(_imgSmallMouseDownMargin.Left, _imgSmallMouseDownMargin.Top);
        }

        private void ImageMain_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _imgMouseDownPoint = Mouse.GetPosition(_panelMain);
            _imgMouseDownMargin = ImageMargin;
            _imgIsMouseLeftButtonDown = true;
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            _imgIsMouseLeftButtonDown = false;
        }

        /// <summary>
        ///     右下角小图片显示切换
        /// </summary>
        private void BorderSmallShowSwitch()
        {
            if (_canMoveX || _canMoveY)
            {
                if (!_borderSmallIsLoaded)
                {
                    _canvasSmallImg.Background = new VisualBrush(_imageMain);
                    InitBorderSmall();
                    _borderSmallIsLoaded = true;
                }

                ShowSmallImgInternal = true;
                UpdateBorderSmall();
            }
            else
            {
                ShowSmallImgInternal = false;
            }
        }

        /// <summary>
        ///     初始化右下角小图片
        /// </summary>
        private void InitBorderSmall()
        {
            if (_canvasSmallImg == null) return;
            var scaleWindow = _canvasSmallImg.MaxWidth / _canvasSmallImg.MaxHeight;
            if (_imgWidHeiScale > scaleWindow)
            {
                _canvasSmallImg.Width = _canvasSmallImg.MaxWidth;
                _canvasSmallImg.Height = _canvasSmallImg.Width / _imgWidHeiScale;
            }
            else
            {
                _canvasSmallImg.Width = _canvasSmallImg.MaxHeight * _imgWidHeiScale;
                _canvasSmallImg.Height = _canvasSmallImg.MaxHeight;
            }
        }

        /// <summary>
        ///     更新右下角小图片
        /// </summary>
        private void UpdateBorderSmall()
        {
            if (!ShowSmallImgInternal) return;

            var widthMin = Math.Min(ImageWidth, ActualWidth);
            var heightMin = Math.Min(ImageHeight, ActualHeight);

            _borderMove.Width = widthMin / ImageWidth * _canvasSmallImg.Width;
            _borderMove.Height = heightMin / ImageHeight * _canvasSmallImg.Height;

            var marginX = -ImageMargin.Left / ImageWidth * _canvasSmallImg.Width;
            var marginY = -ImageMargin.Top / ImageHeight * _canvasSmallImg.Height;

            var marginXMax = _canvasSmallImg.Width - _borderMove.Width;
            var marginYMax = _canvasSmallImg.Height - _borderMove.Height;

            marginX = Math.Max(0, marginX);
            marginX = Math.Min(marginXMax, marginX);
            marginY = Math.Max(0, marginY);
            marginY = Math.Min(marginYMax, marginY);

            _borderMove.Margin = new Thickness(marginX, marginY, 0, 0);
        }

        /// <summary>
        ///     缩放图片
        /// </summary>
        /// <param name="isEnlarge"></param>
        private void ScaleImg(bool isEnlarge)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) return;
            var oldImageWidth = ImageWidth;
            var olgImageHeight = ImageHeight;

            var tempScale = isEnlarge ? _imgActualScale + ScaleInternal : _imgActualScale - ScaleInternal;
            if (Math.Abs(tempScale) < ScaleInternal || Math.Abs(tempScale - 50) < 0.001)
                return;

            ImageScale = tempScale;

            var posCanvas = Mouse.GetPosition(_panelMain);
            var posImg = new Point(posCanvas.X - _imgActualMargin.Left, posCanvas.Y - _imgActualMargin.Top);

            var marginX = .5 * _scaleInternalWidth;
            var marginY = .5 * _scaleInternalHeight;

            if (ImageWidth > ActualWidth)
            {
                _canMoveX = true;
                if (ImageHeight > ActualHeight)
                {
                    _canMoveY = true;
                    marginX = posImg.X / oldImageWidth * _scaleInternalWidth;
                    marginY = posImg.Y / olgImageHeight * _scaleInternalHeight;
                }
                else
                {
                    _canMoveY = false;
                }
            }
            else
            {
                _canMoveY = ImageHeight > ActualHeight;
                _canMoveX = false;
            }

            Thickness thickness;
            if (isEnlarge)
            {
                thickness = new Thickness(_imgActualMargin.Left - marginX, _imgActualMargin.Top - marginY, 0, 0);
            }
            else
            {
                var marginActualX = _imgActualMargin.Left + marginX;
                var marginActualY = _imgActualMargin.Top + marginY;
                var subX = ImageWidth - ActualWidth;
                var subY = ImageHeight - ActualHeight;

                var right = Math.Abs(_borderMove.Width - _canvasSmallImg.ActualWidth + _borderMove.Margin.Left);
                var top = Math.Abs(_borderMove.Height - _canvasSmallImg.ActualHeight + _borderMove.Margin.Top);
                if (Math.Abs(ImageMargin.Left) < 0.001 || right < 0.001)
                    marginActualX = _imgActualMargin.Left + _borderMove.Margin.Left /
                                    (_canvasSmallImg.ActualWidth - _borderMove.Width) * _scaleInternalWidth;
                if (Math.Abs(ImageMargin.Top) < 0.001 || top < 0.001)
                    marginActualY = _imgActualMargin.Top + _borderMove.Margin.Top /
                                    (_canvasSmallImg.ActualHeight - _borderMove.Height) * _scaleInternalHeight;
                if (subX < 0.001) marginActualX = (ActualWidth - ImageWidth) / 2;
                if (subY < 0.001) marginActualY = (ActualHeight - ImageHeight) / 2;
                thickness = new Thickness(marginActualX, marginActualY, 0, 0);
            }

            ImageMargin = thickness;
            _imgActualScale = tempScale;
            _imgActualMargin = thickness;
            BorderSmallShowSwitch();

            _imgSmallMouseDownMargin = _borderMove.Margin;
            MoveSmallImg(_imgSmallMouseDownMargin.Left, _imgSmallMouseDownMargin.Top);
        }

        /// <summary>
        ///     旋转图片
        /// </summary>
        /// <param name="rotate"></param>
        private void RotateImg(double rotate)
        {
            _imgActualRotate = rotate;

            _isOblique = ((int)_imgActualRotate - 90) % 180 == 0;
            ShowSmallImgInternal = false;
            Init();
            InitBorderSmall();

            var animation = AnimationHelper.CreateAnimation(rotate);
            animation.Completed += (s, e1) => { ImageRotate = rotate; };
            animation.FillBehavior = FillBehavior.Stop;
            BeginAnimation(ImageRotateProperty, animation);
        }

        /// <summary>
        ///     移动图片
        /// </summary>
        private void MoveImg()
        {
            _imgCurrentPoint = Mouse.GetPosition(_panelMain);
            ShowCloseButton = _imgCurrentPoint.Y < 200;
            ShowBorderBottom = _imgCurrentPoint.Y > ActualHeight - 200;

            if (_imgIsMouseLeftButtonDown)
            {
                var subX = _imgCurrentPoint.X - _imgMouseDownPoint.X;
                var subY = _imgCurrentPoint.Y - _imgMouseDownPoint.Y;

                var marginX = _imgMouseDownMargin.Left;
                if (ImageWidth > ActualWidth)
                {
                    marginX = _imgMouseDownMargin.Left + subX;
                    if (marginX >= 0)
                        marginX = 0;
                    else if (-marginX + ActualWidth >= ImageWidth) marginX = ActualWidth - ImageWidth;
                    _canMoveX = true;
                }

                var marginY = _imgMouseDownMargin.Top;
                if (ImageHeight > ActualHeight)
                {
                    marginY = _imgMouseDownMargin.Top + subY;
                    if (marginY >= 0)
                        marginY = 0;
                    else if (-marginY + ActualHeight >= ImageHeight) marginY = ActualHeight - ImageHeight;
                    _canMoveY = true;
                }

                ImageMargin = new Thickness(marginX, marginY, 0, 0);
                _imgActualMargin = ImageMargin;

                UpdateBorderSmall();
            }
        }

        /// <summary>
        ///     移动小图片
        /// </summary>
        private void MoveSmallImg()
        {
            if (_imgSmallIsMouseLeftButtonDown)
            {
                _imgSmallCurrentPoint = Mouse.GetPosition(_canvasSmallImg);

                var subX = _imgSmallCurrentPoint.X - _imgSmallMouseDownPoint.X;
                var subY = _imgSmallCurrentPoint.Y - _imgSmallMouseDownPoint.Y;

                var marginX = _imgSmallMouseDownMargin.Left + subX;
                var marginY = _imgSmallMouseDownMargin.Top + subY;

                MoveSmallImg(marginX, marginY);
            }
        }

        private void MoveSmallImg(double marginX, double marginY)
        {
            if (marginX < 0)
                marginX = 0;
            else if (marginX + _borderMove.Width >= _canvasSmallImg.Width)
                marginX = _canvasSmallImg.Width - _borderMove.Width;
            if (marginY < 0)
                marginY = 0;
            else if (marginY + _borderMove.Height >= _canvasSmallImg.Height)
                marginY = _canvasSmallImg.Height - _borderMove.Height;
            _borderMove.Margin = new Thickness(marginX, marginY, 0, 0);

            var marginActualX = (ActualWidth - ImageWidth) / 2;
            var marginActualY = (ActualHeight - ImageHeight) / 2;

            if (_canMoveX) marginActualX = -marginX / _canvasSmallImg.Width * ImageWidth;
            if (_canMoveY) marginActualY = -marginY / _canvasSmallImg.Height * ImageHeight;

            ImageMargin = new Thickness(marginActualX, marginActualY, 0, 0);
            _imgActualMargin = ImageMargin;
        }

        private void CanvasSmallImg_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _imgSmallMouseDownPoint = Mouse.GetPosition(_canvasSmallImg);
            _imgSmallMouseDownMargin = _borderMove.Margin;
            _imgSmallIsMouseLeftButtonDown = true;
            e.Handled = true;
        }

        private void CanvasSmallImg_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => _imgSmallIsMouseLeftButtonDown = false;

        private void CanvasSmallImg_OnMouseMove(object sender, MouseEventArgs e) => MoveSmallImg();

        private void CanvasSmallImg_OnMouseLeave(object sender, MouseEventArgs e) => _imgSmallIsMouseLeftButtonDown = false;
    }
}