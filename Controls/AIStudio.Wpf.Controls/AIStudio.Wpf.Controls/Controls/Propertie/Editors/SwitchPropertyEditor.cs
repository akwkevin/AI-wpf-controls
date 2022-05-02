using System.Windows;
using System.Windows.Controls.Primitives;

namespace AIStudio.Wpf.Controls
{
    public class SwitchPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new ToggleButton
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            IsEnabled = !propertyItem.IsReadOnly
        };

        public override DependencyProperty GetDependencyProperty() => ToggleButton.IsCheckedProperty;
    }
}
