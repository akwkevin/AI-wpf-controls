using System.Windows;

namespace AIStudio.Wpf.Controls
{
    public class ColorPropertyEditor : PropertyEditorBase
    {
        public ColorPropertyEditor()
        {

        }


        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var colorPicker = new ColorPicker
            {
                IsEnabled = !propertyItem.IsReadOnly,
                Padding = new Thickness(0),
            };
            ControlAttach.SetCornerRadius(colorPicker, new CornerRadius());
            return colorPicker;
        }

        public override DependencyProperty GetDependencyProperty() => ColorPicker.SelectedColorProperty;
    }
}
