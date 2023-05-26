using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    public static class ProgressBarAttach
    {
        #region Text文本
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text", typeof(string), typeof(ProgressBarAttach), new PropertyMetadata(default(string)));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(ProgressBar))]
        public static void SetText(DependencyObject element, string value)
            => element.SetValue(TextProperty, value);

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(ProgressBar))]
        public static string GetText(DependencyObject element)
            => (string)element.GetValue(TextProperty);
        #endregion

        #region StartAngle
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(ProgressBar))]
        public static double GetStartAngle(DependencyObject obj)
        {
            return (double)obj.GetValue(StartAngleProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(ProgressBar))]
        public static void SetStartAngle(DependencyObject obj, double value)
        {
            obj.SetValue(StartAngleProperty, value);
        }

        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.RegisterAttached("StartAngle", typeof(double), typeof(ProgressBarAttach), new PropertyMetadata(-130d));
        #endregion

        #region EndAngle
        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(ProgressBar))]
        public static double GetEndAngle(DependencyObject obj)
        {
            return (double)obj.GetValue(EndAngleProperty);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(ProgressBar))]
        public static void SetEndAngle(DependencyObject obj, double value)
        {
            obj.SetValue(EndAngleProperty, value);
        }

        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.RegisterAttached("EndAngle", typeof(double), typeof(ProgressBarAttach), new PropertyMetadata(130d));
        #endregion
    }
}
