using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AIStudio.Wpf.Controls.Event;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = ElementRoot, Type = typeof(Image))]
    public class Avatar : ContentControl
    {
        private const string ElementRoot = "PART_Content";

        private Image _image;

        public Avatar() : base()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _image = GetTemplateChild(ElementRoot) as Image;
            if (_image != null)
            {
                _image.ImageFailed += _image_ImageFailed;
            }
        }

        private void _image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            RaiseEvent(new FunctionEventArgs<Exception>(ImageFailedEvent, e.ErrorException));
        }

        /// <summary>
        /// ImageFailedEvent is a routed event.
        /// </summary>
        public static readonly RoutedEvent ImageFailedEvent = EventManager.RegisterRoutedEvent(nameof(ImageFailed), RoutingStrategy.Bubble, typeof(EventHandler<FunctionEventArgs<Exception>>), typeof(Avatar));

        /// <summary>
        /// Raised when there is a failure in image.
        /// </summary>
        public event EventHandler<FunctionEventArgs<Exception>> ImageFailed
        {
            add
            {
                AddHandler(ImageFailedEvent, value);
            }
            remove
            {
                RemoveHandler(ImageFailedEvent, value);
            }
        }

        #region 尺寸
        public ControlSize Size
        {
            get
            {
                return (ControlSize)GetValue(SizeProperty);
            }
            set
            {
                SetValue(SizeProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register(nameof(Size), typeof(ControlSize), typeof(Avatar), new PropertyMetadata(ControlSize.Large, OnSizeChanged));

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Avatar avatar)
            {
                if (double.TryParse(e.NewValue?.ToString(), out double size))
                {
                    avatar.Width = avatar.Height = size;
                }
            }
        }
        #endregion

        #region 形状
        public AvatarShape Shape
        {
            get
            {
                return (AvatarShape)GetValue(ShapeProperty);
            }
            set
            {
                SetValue(ShapeProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Shape.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register(nameof(Shape), typeof(AvatarShape), typeof(Avatar), new PropertyMetadata(AvatarShape.Circle));
        #endregion

        #region 图片头像的资源地址
        public ImageSource Src
        {
            get
            {
                return (ImageSource)GetValue(SrcProperty);
            }
            set
            {
                SetValue(SrcProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Src.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SrcProperty =
            DependencyProperty.Register(nameof(Src), typeof(ImageSource), typeof(Avatar), new PropertyMetadata(null));
        #endregion

        #region 描述如何调整内容大小以填充为其分配的空间。

        public Stretch Stretch
        {
            get
            {
                return (Stretch)GetValue(StretchProperty);
            }
            set
            {
                SetValue(StretchProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Stretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(Avatar), new PropertyMetadata(Stretch.UniformToFill));

        #endregion

        #region 表示矩形的角的半径。
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(Avatar), new PropertyMetadata(new CornerRadius(4)));
        #endregion


    }

    public enum AvatarShape
    {
        Circle,
        Square,
    }
}
