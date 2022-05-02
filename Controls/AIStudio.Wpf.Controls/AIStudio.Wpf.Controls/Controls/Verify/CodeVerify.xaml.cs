using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_Image, Type = typeof(Image))]
    [TemplatePart(Name = PART_Refresh, Type = typeof(Button))]
    public class CodeVerify : Control
    {
        private const string PART_Image = "PART_Image";
        private const string PART_Refresh = "PART_Refresh";
        private Button _btnRefresh;
        #region 公开属性

        #region ImageStyle 验证码图片的样式

        /// <summary>
        /// 验证码图片的样式
        /// </summary>
        public Style ImageStyle
        {
            get
            {
                return (Style)GetValue(ImageStyleProperty);
            }
            set
            {
                SetValue(ImageStyleProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ImageStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageStyleProperty =
            DependencyProperty.Register("ImageStyle", typeof(Style), typeof(CodeVerify), new UIPropertyMetadata(null));

        #endregion

        #region DisplayMode 验证码显示种类
        public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register("DisplayMode", typeof(string), typeof(CodeVerify),
            new FrameworkPropertyMetadata("1", FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string DisplayMode
        {
            get
            {
                return (string)GetValue(DisplayModeProperty);
            }
            set
            {
                SetValue(DisplayModeProperty, value);
            }
        }
        #endregion

        #region ImgHeight 验证码的高度

        /// <summary>
        /// 验证码的高度
        /// </summary>
        public int ImgHeight
        {
            get
            {
                return (int)GetValue(ImgHeightProperty);
            }
            set
            {
                SetValue(ImgHeightProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ImageStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImgHeightProperty =
            DependencyProperty.Register("ImgHeight", typeof(int), typeof(CodeVerify), new FrameworkPropertyMetadata(23, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion

        #region ImgWidth 验证码的宽度

        /// <summary>
        /// 刷新按钮的样式
        /// </summary>
        public int ImgWidth
        {
            get
            {
                return (int)GetValue(ImgWidthProperty);
            }
            set
            {
                SetValue(ImgWidthProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for RefreshButtonStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImgWidthProperty =
            DependencyProperty.Register("ImgWidth", typeof(int), typeof(CodeVerify), new FrameworkPropertyMetadata(70, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion

        #endregion

        static CodeVerify()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CodeVerify), new FrameworkPropertyMetadata(typeof(CodeVerify)));
        }
        public CodeVerify()
        {
            this.Loaded += new RoutedEventHandler(CheckCode_Loaded);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_btnRefresh != null)
            {
                _btnRefresh.Click -= new RoutedEventHandler(btnRefresh_Click);
            }

            _btnRefresh = GetTemplateChild(PART_Refresh) as Button;

            if (_btnRefresh != null)
            {
                _btnRefresh.Click += new RoutedEventHandler(btnRefresh_Click);
            }
        }

        private void CheckCode_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshCode();
        }

        #region IndentifyingCode 随机生成的验证码

        public static readonly DependencyProperty IndentifyingCodeProperty = DependencyProperty.Register("IndentifyingCode", typeof(string), typeof(CodeVerify),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 随机生成的验证码
        /// </summary>
        public string IndentifyingCode
        {
            get
            {
                return (string)GetValue(IndentifyingCodeProperty);
            }
            set
            {
                SetValue(IndentifyingCodeProperty, value);
            }
        }
        #endregion

        #region imageSource 图片数据源

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(CodeVerify),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 随机生成的验证码
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)GetValue(ImageSourceProperty);
            }
            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }

        #endregion

        #region Refresh 刷新标志

        public static readonly DependencyProperty RefreshProperty = DependencyProperty.Register("Refresh", typeof(bool), typeof(CodeVerify),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (x, y) => {
                if (y.NewValue is bool && (bool)y.NewValue)
                {
                    CodeVerify checkCode = x as CodeVerify;
                    if (checkCode != null)
                        checkCode.RefreshCode();
                }
            }));

        /// <summary>
        /// 刷新标志
        /// </summary>
        public bool Refresh
        {
            get
            {
                return (bool)GetValue(RefreshProperty);
            }
            set
            {
                SetValue(RefreshProperty, value);
            }
        }
        #endregion

        #region 事件处理
        public void RefreshCode()
        {
            VerificationCode code = VerificationCode.GetNewIndentifyCode(4, DisplayMode, (int)ImgWidth, (int)ImgHeight);
            if (code != null)
            {
                IndentifyingCode = code.IndentifyingCode;
                ImageSource = code.CodeImageSource;
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshCode();
        }

        #endregion
    }
}