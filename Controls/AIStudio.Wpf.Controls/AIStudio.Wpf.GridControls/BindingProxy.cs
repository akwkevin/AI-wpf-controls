using System.Windows;

namespace AIStudio.Wpf.GridControls
{
    public class BindingProxy : Freezable
    {
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public object Data
        {
            get
            {
                return (object)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Data.
        // This enables animation, styling, binding, etc...
        public static readonly System.Windows.DependencyProperty DataProperty =
            System.Windows.DependencyProperty.Register("Data", typeof(object),
            typeof(BindingProxy), new System.Windows.UIPropertyMetadata(null));
    }
}
