using System.Windows;

namespace AIStudio.Wpf.Controls
{
    public class NumberPropertyEditor : PropertyEditorBase
    {
        public NumberPropertyEditor()
        {

        }

        public NumberPropertyEditor(double minimum, double maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public double Minimum
        {
            get; set;
        }

        public double Maximum
        {
            get; set;
        }

        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var numericUpDown = new NumericUpDown
            {
                IsReadOnly = propertyItem.IsReadOnly,
                Minimum = Minimum,
                Maximum = Maximum
            };
            ControlAttach.SetCornerRadius(numericUpDown, new CornerRadius());
            return numericUpDown;
        }

        public override DependencyProperty GetDependencyProperty() => NumericUpDown.ValueProperty;
    }
}
