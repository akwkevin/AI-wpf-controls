using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    ///     图片浏览器
    /// </summary>
    [TemplatePart(Name = ElementPanelTop, Type = typeof(Panel))]
    [TemplatePart(Name = ElementImageViewer, Type = typeof(ImageViewer))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]
    public class ImageBrowser : Window
    {
        #region Constants
        private const string ElementPanelTop = "PART_PanelTop";
        private const string ElementImageViewer = "PART_ImageViewer";
        private const string PART_CloseButton = "PART_CloseButton";
        #endregion Constants

        #region Data
        private Panel _panelTop;
        private ImageViewer _imageViewer;
        private Button _closeButton;
        #endregion Data

        public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register(
         "IsFullScreen", typeof(bool), typeof(ImageBrowser),
         new PropertyMetadata(false));

        public bool IsFullScreen
        {
            get => (bool)GetValue(IsFullScreenProperty);
            set => SetValue(IsFullScreenProperty, value);
        }

        static ImageBrowser()
        {

        }

        public ImageBrowser()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
        }

        /// <summary>
        ///     带一个图片Uri的构造函数
        /// </summary>
        /// <param name="uri"></param>
        public ImageBrowser(Uri uri) : this()
        {
            Loaded += (s, e) => {
                try
                {
                    _imageViewer.ImageSource = BitmapFrame.Create(uri);
                    _imageViewer.ImgPath = uri.AbsolutePath;

                    if (File.Exists(_imageViewer.ImgPath))
                    {
                        var info = new FileInfo(_imageViewer.ImgPath);
                        _imageViewer.ImgSize = info.Length;
                    }
                }
                catch
                {
                    MessageBox.Show("ErrorImgPath");
                }
            };
        }

        /// <summary>
        ///     带一个图片Uri的构造函数
        /// </summary>
        /// <param name="uri"></param>
        public ImageBrowser(BitmapSource imagesource) : this()
        {
            Loaded += (s, e) => {
                try
                {
                    _imageViewer.ImageSource = BitmapFrame.Create(imagesource);
                }
                catch
                {
                    MessageBox.Show("ErrorImageSourcePath");
                }
            };
        }

        /// <summary>
        ///     带一个图片路径的构造函数
        /// </summary>
        /// <param name="path"></param>
        public ImageBrowser(string path) : this(new Uri(path))
        {

        }

        public override void OnApplyTemplate()
        {
            if (_panelTop != null)
            {
                _panelTop.MouseLeftButtonDown -= PanelTopOnMouseLeftButtonDown;
            }

            if (_imageViewer != null)
            {
                _imageViewer.MouseLeftButtonDown -= ImageViewer_MouseLeftButtonDown;
            }

            base.OnApplyTemplate();

            _panelTop = GetTemplateChild(ElementPanelTop) as Panel;
            _imageViewer = GetTemplateChild(ElementImageViewer) as ImageViewer;
            _closeButton = GetTemplateChild(PART_CloseButton) as Button;

            if (_panelTop != null)
            {
                _panelTop.MouseLeftButtonDown += PanelTopOnMouseLeftButtonDown;
            }

            if (_imageViewer != null)
            {
                _imageViewer.MouseLeftButtonDown += ImageViewer_MouseLeftButtonDown;
            }

            if (_closeButton != null)
            {
                _closeButton.Click += ButtonClose_OnClick;

            }
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e) => Close();

        private void PanelTopOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ImageViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !(_imageViewer.ImageWidth > ActualWidth || _imageViewer.ImageHeight > ActualHeight))
            {
                DragMove();
            }
        }



    }
}