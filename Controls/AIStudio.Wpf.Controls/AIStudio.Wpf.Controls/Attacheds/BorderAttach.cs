using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AIStudio.Wpf.Controls.Converter;

namespace AIStudio.Wpf.Controls
{
    public static class BorderAttach
    {
        #region Circular
        public static readonly DependencyProperty CircularProperty = DependencyProperty.RegisterAttached(
            "Circular", typeof(bool), typeof(BorderAttach), new PropertyMetadata(false, OnCircularChanged));

        private static void OnCircularChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Border border)
            {
                if ((bool)e.NewValue)
                {
                    var binding = new MultiBinding
                    {
                        Converter = new BorderCircularConverter()
                    };
                    binding.Bindings.Add(new System.Windows.Data.Binding(FrameworkElement.ActualWidthProperty.Name) { Source = border });
                    binding.Bindings.Add(new System.Windows.Data.Binding(FrameworkElement.ActualHeightProperty.Name) { Source = border });
                    border.SetBinding(Border.CornerRadiusProperty, binding);
                }
                else
                {
                    BindingOperations.ClearBinding(border, FrameworkElement.ActualWidthProperty);
                    BindingOperations.ClearBinding(border, FrameworkElement.ActualHeightProperty);
                    BindingOperations.ClearBinding(border, Border.CornerRadiusProperty);
                }
            }
        }

        [AttachedPropertyBrowsableForType(typeof(Border))]
        [Category(AppName.AIStudio)]
        public static void SetCircular(DependencyObject element, bool value)
            => element.SetValue(CircularProperty, value);

        [AttachedPropertyBrowsableForType(typeof(Border))]
        [Category(AppName.AIStudio)]
        public static bool GetCircular(DependencyObject element)
            => (bool)element.GetValue(CircularProperty);
        #endregion
    }
}
