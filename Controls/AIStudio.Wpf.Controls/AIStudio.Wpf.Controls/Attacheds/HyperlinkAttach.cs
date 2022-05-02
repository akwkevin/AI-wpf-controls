using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace AIStudio.Wpf.Controls
{
    public static class HyperlinkAttach
    {
        public static readonly DependencyProperty ViewInBrowerProperty = DependencyProperty.RegisterAttached(
        "ViewInBrower", typeof(bool), typeof(HyperlinkAttach), new FrameworkPropertyMetadata(default(bool), OnViewInBrowerChanged));

        public static void SetViewInBrower(DependencyObject element, bool value)
        {
            element.SetValue(ViewInBrowerProperty, value);
        }

        public static bool GetViewInBrower(DependencyObject element)
        {
            return (bool)element.GetValue(ViewInBrowerProperty);
        }

        private static void OnViewInBrowerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var checkbox = dependencyObject as Hyperlink;
            if (checkbox != null)
            {
                checkbox.Click -= Checkbox_Click;
                if ((bool)dependencyPropertyChangedEventArgs.NewValue)
                {
                    checkbox.Click += Checkbox_Click;
                }
            }
        }

        private static void Checkbox_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            // 激活的是当前默认的浏览器
            Process.Start("explorer.exe", link.NavigateUri.AbsoluteUri);
        }
    }
}
