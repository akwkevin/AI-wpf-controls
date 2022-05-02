using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.ComeCapture.Controls
{
    public class LineTool : StackPanel
    {
        static LineTool()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineTool), new FrameworkPropertyMetadata(typeof(LineTool)));
        }

        public LineTool()
        {
            _Current = this;
        }

        #region 属性 Current
        private static LineTool _Current = null;
        public static LineTool Current
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
                DependencyProperty.Register("LineThickness", typeof(double), typeof(LineTool),
                new PropertyMetadata(5.0, new PropertyChangedCallback(LineTool.OnLineThicknessPropertyChanged)));

        private static void OnLineThicknessPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is LineTool)
            {
                (obj as LineTool).OnLineThicknessValueChanged();
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
                DependencyProperty.Register("LineBrush", typeof(SolidColorBrush), typeof(LineTool),
                new PropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(LineTool.OnLineBrushPropertyChanged)));

        private static void OnLineBrushPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is LineTool)
            {
                (obj as LineTool).OnLineBrushValueChanged();
            }
        }

        protected void OnLineBrushValueChanged()
        {

        }
        #endregion

    }
}
