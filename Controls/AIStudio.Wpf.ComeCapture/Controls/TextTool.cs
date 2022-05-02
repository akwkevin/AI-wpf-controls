using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.ComeCapture.Controls
{
    public class TextTool : StackPanel
    {
        static TextTool()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextTool), new FrameworkPropertyMetadata(typeof(TextTool)));
        }

        public TextTool()
        {
            _Current = this;
        }

        #region 属性 Current
        private static TextTool _Current = null;
        public static TextTool Current
        {
            get
            {
                return _Current;
            }
        }
        #endregion

        #region 属性 FontSizes
        private static Dictionary<int, int> _FontSizes;
        public static Dictionary<int, int> FontSizes
        {
            get
            {
                if (_FontSizes == null)
                {
                    _FontSizes = new Dictionary<int, int>()
                    {
                        { 8,8},
                        { 10,10},
                        { 12,12},
                        { 14,14},
                        { 16,16},
                        { 18,18},
                        { 20,20},
                        { 22,22}
                    };
                }
                return _FontSizes;
            }
            set
            {
                _FontSizes = value;
            }
        }
        #endregion

        #region FontSize DependencyProperty
        public int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        public static readonly DependencyProperty FontSizeProperty =
                DependencyProperty.Register("FontSize", typeof(int), typeof(TextTool),
                new PropertyMetadata(12, new PropertyChangedCallback(TextTool.OnFontSizePropertyChanged)));

        private static void OnFontSizePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TextTool)
            {
                (obj as TextTool).OnFontSizeValueChanged();
            }
        }

        protected void OnFontSizeValueChanged()
        {

        }
        #endregion

        #region LineBrush DependencyProperty
        public SolidColorBrush LineBrush
        {
            get { return (SolidColorBrush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }
        public static readonly DependencyProperty LineBrushProperty =
                DependencyProperty.Register("LineBrush", typeof(SolidColorBrush), typeof(TextTool),
                new PropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(TextTool.OnLineBrushPropertyChanged)));

        private static void OnLineBrushPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is TextTool)
            {
                (obj as TextTool).OnLineBrushValueChanged();
            }
        }

        protected void OnLineBrushValueChanged()
        {

        }
        #endregion
    }
}
