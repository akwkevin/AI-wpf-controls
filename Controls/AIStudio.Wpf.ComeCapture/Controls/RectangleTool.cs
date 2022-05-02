using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.ComeCapture.Controls
{
    public class RectangleTool : StackPanel
    {
        static RectangleTool()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RectangleTool), new FrameworkPropertyMetadata(typeof(RectangleTool)));
        }

        public RectangleTool()
        {
            _Current = this;
        }

        #region 属性 Current
        private static RectangleTool _Current = null;
        public static RectangleTool Current
        {
            get
            {
                return _Current;
            }
        }
        #endregion

        #region LineThickness DependencyProperty
        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }
        public static readonly DependencyProperty LineThicknessProperty =
                DependencyProperty.Register("LineThickness", typeof(double), typeof(RectangleTool),
                new PropertyMetadata(5.0, new PropertyChangedCallback(RectangleTool.OnLineThicknessPropertyChanged)));

        private static void OnLineThicknessPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is RectangleTool)
            {
                (obj as RectangleTool).OnLineThicknessValueChanged();
            }
        }

        protected void OnLineThicknessValueChanged()
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
                DependencyProperty.Register("LineBrush", typeof(SolidColorBrush), typeof(RectangleTool),
                new PropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(RectangleTool.OnLineBrushPropertyChanged)));

        private static void OnLineBrushPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is RectangleTool)
            {
                (obj as RectangleTool).OnLineBrushValueChanged();
            }
        }

        protected void OnLineBrushValueChanged()
        {

        }
        #endregion
    }
}
