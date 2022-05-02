using System.Windows;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls
{
    public class ReadOnlyTextPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new System.Windows.Controls.TextBox
        {
            IsReadOnly = true
        };

        public override DependencyProperty GetDependencyProperty() => System.Windows.Controls.TextBox.TextProperty;

        public override BindingMode GetBindingMode(PropertyItem propertyItem) => BindingMode.OneWay;

        protected override IValueConverter GetConverter(PropertyItem propertyItem) => Application.Current.TryFindResource("Object2StringConverter") as IValueConverter;
    }
}
