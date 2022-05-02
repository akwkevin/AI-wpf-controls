using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.ComeCapture.Controls
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public class TextBoxControl : Control
    {
        public TextBox _TextBox;

        static TextBoxControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxControl), new FrameworkPropertyMetadata(typeof(TextBoxControl)));
        }

        public TextBoxControl()
        {
            AddHandler(GotFocusEvent, new RoutedEventHandler((sender, e) =>
            {
                MyFocus = true;
            }));
            AddHandler(LostFocusEvent, new RoutedEventHandler((sender, e) =>
            {
                MyFocus = false;
            }));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _TextBox = GetTemplateChild("PART_TextBox") as TextBox;
            _TextBox.MaxWidth = MainWindow.Current.MainImage.Width - MainWindow.Current.MainImage.point.X - 3;
            _TextBox.MaxHeight = MainWindow.Current.MainImage.Height - MainWindow.Current.MainImage.point.Y - 3;
        }

        #region BorderColor DependencyProperty
        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
        public static readonly DependencyProperty BorderColorProperty =
                DependencyProperty.Register("BorderColor", typeof(Color), typeof(TextBoxControl),
                new PropertyMetadata(Colors.Transparent, new PropertyChangedCallback(TextBoxControl.OnBorderColorPropertyChanged)));

        private static void OnBorderColorPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TextBoxControl)
            {
                (obj as TextBoxControl).OnBorderColorValueChanged();
            }
        }

        protected void OnBorderColorValueChanged()
        {

        }
        #endregion

        #region MyFocus DependencyProperty
        public bool MyFocus
        {
            get { return (bool)GetValue(MyFocusProperty); }
            set { SetValue(MyFocusProperty, value); }
        }
        public static readonly DependencyProperty MyFocusProperty =
                DependencyProperty.Register("MyFocus", typeof(bool), typeof(TextBoxControl),
                new PropertyMetadata(true, new PropertyChangedCallback(TextBoxControl.OnMyFocusPropertyChanged)));

        private static void OnMyFocusPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TextBoxControl)
            {
                (obj as TextBoxControl).OnMyFocusValueChanged();
            }
        }

        protected void OnMyFocusValueChanged()
        {
            if (!MyFocus)
            {
                if (string.IsNullOrEmpty(_TextBox.Text))
                {
                    MainWindow.RemoveControl(this);
                }
                else
                {
                    MainWindow.Current.MainImage.ResetLimit(Canvas.GetLeft(this), Canvas.GetTop(this), (Canvas.GetLeft(this) + ActualWidth), (Canvas.GetTop(this) + ActualHeight));
                    MainWindow.Register(this);
                }
                MainWindow.Current.MainImage._Text = null;
            }
        }
        #endregion
    }
}
