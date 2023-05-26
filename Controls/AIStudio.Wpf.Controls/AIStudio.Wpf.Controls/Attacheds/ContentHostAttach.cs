using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    public static class ContentHostAttach
    {
        #region Padding

        public static readonly DependencyProperty PaddingProperty = DependencyProperty.RegisterAttached(
            "Padding", typeof(Thickness), typeof(ContentHostAttach), new FrameworkPropertyMetadata(new Thickness(-1), OnThicknessChanged));

        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        [Category(AppName.AIStudio)]
        public static void SetPadding(DependencyObject element, Thickness value)
        {
            element.SetValue(PaddingProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        [Category(AppName.AIStudio)]
        public static Thickness GetPadding(DependencyObject element)
        {
            return (Thickness)element.GetValue(PaddingProperty);
        }

        private static void OnThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer contentHost)
            {
                contentHost.Tag = (Thickness)e.NewValue;
                contentHost.Loaded -= ContentHost_Loaded;
                contentHost.Loaded += ContentHost_Loaded;
            }
            //if (d is ScrollViewer contentHost && contentHost.Content != null && contentHost.Content is FrameworkElement textBoxView)
            //{
            //    textBoxView.Margin = (Thickness)e.NewValue;
            //}
        }

        private static void ContentHost_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ScrollViewer contentHost)
            {
                contentHost.Padding = (Thickness)contentHost.Tag;
                contentHost.Margin = new Thickness(0);
            }
        }
        #endregion
    }
}
