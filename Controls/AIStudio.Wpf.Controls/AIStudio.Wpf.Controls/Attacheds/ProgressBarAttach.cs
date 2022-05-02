using System.Windows;

namespace AIStudio.Wpf.Controls
{
    public static class ProgressBarAttach
    {
        #region Text文本
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text", typeof(string), typeof(ProgressBarAttach), new PropertyMetadata(default(string)));

        public static void SetText(DependencyObject element, string value)
            => element.SetValue(TextProperty, value);

        public static string GetText(DependencyObject element)
            => (string)element.GetValue(TextProperty);
        #endregion

        #region StartAngle
        public static double GetStartAngle(DependencyObject obj)
        {
            return (double)obj.GetValue(StartAngleProperty);
        }

        public static void SetStartAngle(DependencyObject obj, double value)
        {
            obj.SetValue(StartAngleProperty, value);
        }

        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.RegisterAttached("StartAngle", typeof(double), typeof(ProgressBarAttach), new PropertyMetadata(-130d));
        #endregion

        #region EndAngle
        public static double GetEndAngle(DependencyObject obj)
        {
            return (double)obj.GetValue(EndAngleProperty);
        }

        public static void SetEndAngle(DependencyObject obj, double value)
        {
            obj.SetValue(EndAngleProperty, value);
        }

        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.RegisterAttached("EndAngle", typeof(double), typeof(ProgressBarAttach), new PropertyMetadata(130d));
        #endregion
    }
}
