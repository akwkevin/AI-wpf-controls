using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    ///     弹出窗口
    /// </summary>
    [TemplatePart(Name = ElementMainBorder, Type = typeof(Border))]
    [TemplatePart(Name = ElementTitleBlock, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_CancelButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_OkButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_scale, Type = typeof(ScaleTransform))]
    [TemplatePart(Name = PART_rot, Type = typeof(RotateTransform))]
    public class PopupWindow : System.Windows.Window
    {
        #region Constants

        private const string ElementMainBorder = "PART_MainBorder";
        private const string ElementTitleBlock = "PART_TitleBlock";
        private const string PART_CancelButton = "PART_CancelButton";
        private const string PART_OkButton = "PART_OkButton";
        private const string PART_CloseButton = "PART_CloseButton";
        private const string PART_scale = "PART_scale";
        private const string PART_rot = "PART_rot";
        #endregion Constants

        private Border _mainBorder;
        private TextBlock _titleBlock;
        private Button _cancelButton;
        private Button _okButton;
        private Button _closeButton;
        private ScaleTransform _scale;
        private RotateTransform _rot;

        private bool _showBackground = true;

        private FrameworkElement _targetElement;

        public override void OnApplyTemplate()
        {
            if (_titleBlock != null)
            {
                _titleBlock.MouseLeftButtonDown -= TitleBlock_OnMouseLeftButtonDown;
            }
            if (_cancelButton != null)
            {
                _cancelButton.Click -= Button_Click;
            }
            if (_okButton != null)
            {
                _okButton.Click -= Button_Click;
            }
            if (_closeButton != null)
            {
                _closeButton.Click -= Button_Click;
            }

            base.OnApplyTemplate();

            _mainBorder = GetTemplateChild(ElementMainBorder) as Border;
            _titleBlock = GetTemplateChild(ElementTitleBlock) as TextBlock;
            _cancelButton = GetTemplateChild(PART_CancelButton) as Button;
            _okButton = GetTemplateChild(PART_OkButton) as Button;
            _closeButton = GetTemplateChild(PART_CloseButton) as Button;
            _scale = GetTemplateChild(PART_scale) as ScaleTransform;
            _rot = GetTemplateChild(PART_rot) as RotateTransform;

            if (_titleBlock != null)
            {
                _titleBlock.MouseLeftButtonDown += TitleBlock_OnMouseLeftButtonDown;
            }

            if (_cancelButton != null)
            {
                _cancelButton.Click += Button_Click;
            }
            if (_okButton != null)
            {
                _okButton.Click += Button_Click;
            }
            if (_closeButton != null)
            {
                _closeButton.Click += Button_Click;
            }

            if (PopupElement != null)
            {
                _mainBorder.Child = PopupElement;
            }
        }

        internal static readonly DependencyProperty ContentStrProperty = DependencyProperty.Register(
            "ContentStr", typeof(string), typeof(PopupWindow), new PropertyMetadata(default(string)));

        internal string ContentStr
        {
            get => (string)GetValue(ContentStrProperty);
            set => SetValue(ContentStrProperty, value);
        }

        private bool IsDialog
        {
            get; set;
        }

        static PopupWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupWindow), new FrameworkPropertyMetadata(typeof(PopupWindow)));
        }

        public PopupWindow()
        {

            Closing += (sender, args) => {
                if (!IsDialog)
                    Owner?.Focus();
            };
            Loaded += (s, e) => {
                if (!_showBackground)
                {
                    var point = CalSafePoint(_targetElement, PopupElement, BorderThickness);
                    Left = point.X;
                    Top = point.Y;
                    Opacity = 1;
                }

                if (PopupAnimation == CustomizePopupAnimation.Pop)
                {
                    PopAppear();
                }
                else if (PopupAnimation == CustomizePopupAnimation.Rotate)
                {
                    RotateAppear();
                }
            };
        }

        public static Point CalSafePoint(FrameworkElement element, FrameworkElement showElement, Thickness thickness = default)
        {
            if (element == null || showElement == null) return default;
            var point = element.PointToScreen(new Point(0, 0));

            if (point.X < 0) point.X = 0;
            if (point.Y < 0) point.Y = 0;

            var maxLeft = SystemParameters.WorkArea.Width -
                          ((double.IsNaN(showElement.Width) ? showElement.ActualWidth : showElement.Width) +
                           thickness.Left + thickness.Right);
            var maxTop = SystemParameters.WorkArea.Height -
                         ((double.IsNaN(showElement.Height) ? showElement.ActualHeight : showElement.Height) +
                          thickness.Top + thickness.Bottom);
            return new Point(maxLeft > point.X ? point.X : maxLeft, maxTop > point.Y ? point.Y : maxTop);
        }

        public PopupWindow(System.Windows.Window owner) : this() => Owner = owner;

        private void CloseButton_OnClick(object sender, RoutedEventArgs e) => AnimateClose();

        public FrameworkElement PopupElement
        {
            get; set;
        }
        public CustomizePopupAnimation PopupAnimation { get; set; } = CustomizePopupAnimation.None;

        public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register(
            "ShowTitle", typeof(bool), typeof(PopupWindow), new PropertyMetadata(true));

        public bool ShowTitle
        {
            get => (bool)GetValue(ShowTitleProperty);
            set => SetValue(ShowTitleProperty, value);
        }

        public static readonly DependencyProperty ShowCancelProperty = DependencyProperty.Register(
            "ShowCancel", typeof(bool), typeof(PopupWindow), new PropertyMetadata(false));

        public bool ShowCancel
        {
            get => (bool)GetValue(ShowCancelProperty);
            set => SetValue(ShowCancelProperty, value);
        }

        public static readonly DependencyProperty ShowBorderProperty = DependencyProperty.Register(
            "ShowBorder", typeof(bool), typeof(PopupWindow), new PropertyMetadata(false));

        public bool ShowBorder
        {
            get => (bool)GetValue(ShowBorderProperty);
            set => SetValue(ShowBorderProperty, value);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;

            if (button == null)
                return;

            switch (button.Name)
            {
                case PART_CancelButton:
                    ButtonCancle_OnClick(sender, e);
                    break;
                case PART_OkButton:
                    ButtonOk_OnClick(sender, e);
                    break;
                case PART_CloseButton:
                    CloseButton_OnClick(sender, e);
                    break;
            }

            e.Handled = true;
        }

        private void TitleBlock_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        public void Show(FrameworkElement element, bool showBackground = true)
        {
            if (!showBackground)
            {
                Opacity = 0;
                AllowsTransparency = true;
                WindowStyle = WindowStyle.None;
                ShowTitle = false;
                MinWidth = 0;
                MinHeight = 0;
            }

            _showBackground = showBackground;
            _targetElement = element;
            Show();
        }

        public void ShowDialog(FrameworkElement element, bool showBackground = true)
        {
            if (!showBackground)
            {
                Opacity = 0;
                AllowsTransparency = true;
                WindowStyle = WindowStyle.None;
                ShowTitle = false;
                MinWidth = 0;
                MinHeight = 0;
            }

            _showBackground = showBackground;
            _targetElement = element;
            ShowDialog();
        }

        public void Show(System.Windows.Window element, Point point)
        {
            Left = element.Left + point.X;
            Top = element.Top + point.Y;
            Show();
        }

        public static void Show(string message, CustomizePopupAnimation popupAnimation = CustomizePopupAnimation.None)
        {
            var window = new PopupWindow
            {
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                ContentStr = message,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                //Background = ResourceHelper.GetResource<Brush>("MahApps.Brushes.Accent"),
                PopupAnimation = popupAnimation,
            };
            window.Show();
        }

        public static bool? ShowDialog(string message, string title = default(string), bool showCancel = false, CustomizePopupAnimation popupAnimation = CustomizePopupAnimation.None)
        {
            var window = new PopupWindow
            {
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                ContentStr = message,
                IsDialog = true,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ShowBorder = true,
                Title = string.IsNullOrEmpty(title) ? "Tip" : title,
                ShowCancel = showCancel,
                PopupAnimation = popupAnimation,
            };
            return window.ShowDialog();
        }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsDialog)
            {
                DialogResult = true;
            }
            AnimateClose();
        }

        private void ButtonCancle_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsDialog)
            {
                DialogResult = false;
            }
            AnimateClose();
        }

        private async void AnimateClose()
        {
            if (PopupAnimation == CustomizePopupAnimation.Pop)
            {
                PopDisAppear();
                await Task.Delay(1000);
            }
            else if (PopupAnimation == CustomizePopupAnimation.Rotate)
            {
                RotateDisAppear();
                await Task.Delay(1000);
            }
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            PopupElement = null;
        }

        //旋转出现
        private void RotateAppear()
        {
            DoubleAnimation Ani = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(.5))
            };

            DoubleAnimation Rni = new DoubleAnimation()
            {
                From = 0,
                To = 720,
                Duration = new Duration(TimeSpan.FromSeconds(.5))

            };
            _scale.BeginAnimation(ScaleTransform.ScaleXProperty, Ani);
            _scale.BeginAnimation(ScaleTransform.ScaleYProperty, Ani);
            _rot.BeginAnimation(RotateTransform.AngleProperty, Rni);
        }

        //弹出来
        private void PopAppear()
        {
            DoubleAnimation Ani = new DoubleAnimation()
            {
                From = 0.2,
                To = 0.5,
                Duration = new Duration(TimeSpan.FromSeconds(.1))
            };
            DoubleAnimation Ani2 = new DoubleAnimation()
            {
                From = 0.5,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(.1)),
                BeginTime = TimeSpan.FromSeconds(.2)
            };
            DoubleAnimation Ani3 = new DoubleAnimation()
            {
                From = 1,
                To = 1.2,
                Duration = new Duration(TimeSpan.FromSeconds(.1)),
                BeginTime = TimeSpan.FromSeconds(.2),
                AutoReverse = true,
            };

            _scale.BeginAnimation(ScaleTransform.ScaleXProperty, Ani);
            _scale.BeginAnimation(ScaleTransform.ScaleYProperty, Ani);
            _scale.BeginAnimation(ScaleTransform.ScaleXProperty, Ani2);
            _scale.BeginAnimation(ScaleTransform.ScaleYProperty, Ani2);
            _scale.BeginAnimation(ScaleTransform.ScaleXProperty, Ani3);
            _scale.BeginAnimation(ScaleTransform.ScaleYProperty, Ani3);
        }

        //旋转消失
        private void RotateDisAppear()
        {
            DoubleAnimation Ani = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(.5))
            };

            DoubleAnimation Rni = new DoubleAnimation()
            {
                From = 720,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(.5))

            };
            _scale.BeginAnimation(ScaleTransform.ScaleXProperty, Ani);
            _scale.BeginAnimation(ScaleTransform.ScaleYProperty, Ani);
            _rot.BeginAnimation(RotateTransform.AngleProperty, Rni);

        }

        //弹回去
        private void PopDisAppear()
        {
            DoubleAnimation Ani = new DoubleAnimation()
            {
                From = 1,
                To = 1.2,
                Duration = new Duration(TimeSpan.FromSeconds(.04)),
                AutoReverse = true,
            };
            DoubleAnimation Ani2 = new DoubleAnimation()
            {
                From = 1,
                To = 0.5,
                Duration = new Duration(TimeSpan.FromSeconds(.04)),
                BeginTime = TimeSpan.FromSeconds(.04)
            };
            DoubleAnimation Ani3 = new DoubleAnimation()
            {
                From = 0.5,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(.04)),
                BeginTime = TimeSpan.FromSeconds(.08),
            };

            _scale.BeginAnimation(ScaleTransform.ScaleXProperty, Ani);
            _scale.BeginAnimation(ScaleTransform.ScaleYProperty, Ani);
            _scale.BeginAnimation(ScaleTransform.ScaleXProperty, Ani2);
            _scale.BeginAnimation(ScaleTransform.ScaleYProperty, Ani2);
            _scale.BeginAnimation(ScaleTransform.ScaleXProperty, Ani3);
            _scale.BeginAnimation(ScaleTransform.ScaleYProperty, Ani3);
        }
    }

    public enum CustomizePopupAnimation
    {
        None,
        Rotate,
        Pop,
    }
}