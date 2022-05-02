using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AIStudio.Wpf.Controls
{
    public class HorizontalAlignmentPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new System.Windows.Controls.ComboBox
        {
            Style = Application.Current.TryFindResource("ComboBoxCapsule") as Style,
            ItemsSource = Enum.GetValues(propertyItem.PropertyType),
            ItemTemplateSelector = Application.Current.TryFindResource("HorizontalAlignmentPathTemplateSelector") as DataTemplateSelector,
            HorizontalAlignment = HorizontalAlignment.Left
        };

        public override DependencyProperty GetDependencyProperty() => Selector.SelectedValueProperty;
    }

    public class HorizontalAlignmentPathTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is HorizontalAlignment horizontalAlignment)
            {
                var dataTemplate = new DataTemplate
                {
                    DataType = typeof(System.Windows.Controls.ComboBox)
                };

                var factory = new FrameworkElementFactory(typeof(Path));
                factory.SetValue(FrameworkElement.WidthProperty, 12.0);
                factory.SetValue(FrameworkElement.HeightProperty, 10.0);
                factory.SetBinding(Shape.FillProperty, new Binding(Control.ForegroundProperty.Name)
                {
                    RelativeSource = new RelativeSource
                    {
                        AncestorType = typeof(ComboBoxItem)
                    }
                });

                switch (horizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        factory.SetValue(Path.DataProperty, Application.Current.TryFindResource("AlignLeftGeometry") as Geometry);
                        break;
                    case HorizontalAlignment.Center:
                        factory.SetValue(Path.DataProperty, Application.Current.TryFindResource("AlignHCenterGeometry") as Geometry);
                        break;
                    case HorizontalAlignment.Right:
                        factory.SetValue(Path.DataProperty, Application.Current.TryFindResource("AlignRightGeometry") as Geometry);
                        break;
                    case HorizontalAlignment.Stretch:
                        factory.SetValue(Path.DataProperty, Application.Current.TryFindResource("AlignHStretchGeometry") as Geometry);
                        break;
                }

                dataTemplate.VisualTree = factory;
                return dataTemplate;
            }

            return null;
        }
    }
}
