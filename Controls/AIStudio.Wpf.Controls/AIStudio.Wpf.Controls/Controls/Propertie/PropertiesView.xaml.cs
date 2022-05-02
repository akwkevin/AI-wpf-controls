using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for PropertiesView.xaml
    /// </summary>
    public partial class PropertiesView : UserControl
    {
        #region GotFocusProperty 
        /// <summary>
        /// Border圆角
        /// </summary>
        public static readonly DependencyProperty GotFocusPropertyProperty = DependencyProperty.RegisterAttached(
            "GotFocusProperty", typeof(PropertiesView), typeof(PropertiesView), new FrameworkPropertyMetadata(null, OnGotFocusChanged));

        private static void OnGotFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is PropertiesView oldvalue)
            {
                if (d is Panel panel)
                {
                    foreach (var child in panel.Children.OfType<UIElement>())
                    {
                        child.GotFocus -= Child_GotFocus;
                    }
                }
            }

            if (e.NewValue is PropertiesView newvalue)
            {
                if (d is Panel panel)
                {
                    foreach (var child in panel.Children.OfType<UIElement>())
                    {
                        PropertiesView.SetGotFocusProperty(child, newvalue);
                        child.GotFocus -= Child_GotFocus;
                        child.GotFocus += Child_GotFocus;
                    }
                }
            }
        }

        private static void Child_GotFocus(object sender, RoutedEventArgs e)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                var property = PropertiesView.GetGotFocusProperty(element);
                property.SelectedObject = element;
            }
        }

        public static PropertiesView GetGotFocusProperty(DependencyObject d)
        {
            return (PropertiesView)d.GetValue(GotFocusPropertyProperty);
        }

        public static void SetGotFocusProperty(DependencyObject obj, PropertiesView value)
        {
            obj.SetValue(GotFocusPropertyProperty, value);
        }
        #endregion


        #region SelectedObject

        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertiesView), new UIPropertyMetadata(null, OnSelectedObjectChanged));
        public object SelectedObject
        {
            get
            {
                return (object)GetValue(SelectedObjectProperty);
            }
            set
            {
                SetValue(SelectedObjectProperty, value);
            }
        }

        private static void OnSelectedObjectChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PropertiesView propertyInspector = o as PropertiesView;
            if (propertyInspector != null)
                propertyInspector.OnSelectedObjectChanged((object)e.OldValue, (object)e.NewValue);
        }

        protected virtual void OnSelectedObjectChanged(object oldValue, object newValue)
        {
            // We do not want to process the change now if the grid is initializing (ie. BeginInit/EndInit).
            var obj = oldValue as INotifyPropertyChanged;
            if (obj != null)
                obj.PropertyChanged -= PropertyChanged;
            DisplayProperties();
            obj = newValue as INotifyPropertyChanged;
            if (obj != null)
                obj.PropertyChanged += PropertyChanged;
        }

        #endregion //SelectedObject

        public static readonly DependencyProperty NeedBrowsableProperty = DependencyProperty.Register("NeedBrowsable", typeof(bool), typeof(PropertiesView), new UIPropertyMetadata(false));
        public bool NeedBrowsable
        {
            get
            {
                return (bool)GetValue(NeedBrowsableProperty);
            }
            set
            {
                SetValue(NeedBrowsableProperty, value);
            }
        }

        public PropertiesView()
        {
            InitializeComponent();
            DisplayProperties();
        }

        void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DisplayProperties();
        }

        private void DisplayProperties()
        {
            this.Dispatcher.BeginInvoke((Action)delegate () {
                _panel.Children.Clear();
                ClearGrid();
                if (SelectedObject != null)
                {
                    int row = 0;
                    foreach (var prop in SelectedObject.GetType().GetProperties())
                    {
                        var attr = prop.GetCustomAttributes(typeof(BrowsableAttribute), true);
                        if (NeedBrowsable == false && (attr.Length == 0 || (attr[0] as BrowsableAttribute).Browsable))
                        {
                            DisplayProperty(prop, row);
                            row++;
                        }
                        else if (NeedBrowsable == true && (attr.Length > 0 && (attr[0] as BrowsableAttribute).Browsable))
                        {
                            DisplayProperty(prop, row);
                            row++;
                        }
                    }
                    _panel.Children.Add(_gridContainer);
                }
                else
                {
                    _panel.Children.Add(_label);
                }
            }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        private void ClearGrid()
        {
            _grid.RowDefinitions.Clear();
            for (int i = _grid.Children.Count - 1; i >= 0; i--)
            {
                if (_grid.Children[i] != _vLine && _grid.Children[i] != _splitter)
                    _grid.Children.RemoveAt(i);
            }
        }

        private void DisplayProperty(PropertyInfo prop, int row)
        {
            var rowDef = new RowDefinition();
            rowDef.Height = new GridLength(Math.Max(20, this.FontSize * 2));
            _grid.RowDefinitions.Add(rowDef);

            var tb = new TextBlock() { Text = prop.Name };
            var displayAttr = prop.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (displayAttr.Length > 0)
            {
                tb.Text = (displayAttr[0] as DisplayNameAttribute).DisplayName;
            }
            tb.Margin = new Thickness(4);
            Grid.SetColumn(tb, 0);
            Grid.SetRow(tb, _grid.RowDefinitions.Count - 1);
            _grid.Children.Add(tb);

            var line = new Line();
            line.Style = (Style)border.Resources["gridHorizontalLineStyle"];
            Grid.SetRow(line, row);
            _grid.Children.Add(line);

            Style style = null;
            var styleNameAttr = prop.GetCustomAttributes(typeof(StyleNameAttribute), true);
            if (styleNameAttr.Length > 0)
            {
                style = this.FindResource((styleNameAttr[0] as StyleNameAttribute).Name) as Style;
                if (style != null)
                {
                    ContentControl content = new ContentControl();
                    content.Style = style;
                    content.DataContext = SelectedObject;

                    Grid.SetColumn(content, 1);
                    Grid.SetRow(content, _grid.RowDefinitions.Count - 1);

                    _grid.Children.Add(content);
                }
            }

            if (style == null)
            {
                var ed = new TextBox();
                ed.PreviewKeyDown += new KeyEventHandler(ed_KeyDown);
                ed.Margin = new Thickness(0);
                ed.BorderThickness = new Thickness(0);
                Grid.SetColumn(ed, 1);
                Grid.SetRow(ed, _grid.RowDefinitions.Count - 1);

                var binding = new Binding(prop.Name);
                binding.Source = SelectedObject;
                binding.ValidatesOnExceptions = true;
                binding.Mode = BindingMode.OneWay;
                if (prop.CanWrite)
                {
                    var mi = prop.GetSetMethod();
                    if (mi != null && mi.IsPublic)
                        binding.Mode = BindingMode.TwoWay;
                }
                ed.SetBinding(TextBox.TextProperty, binding);

                var template = (ControlTemplate)border.Resources["validationErrorTemplate"];
                Validation.SetErrorTemplate(ed, template);

                _grid.Children.Add(ed);
            }
        }

        void ed_KeyDown(object sender, KeyEventArgs e)
        {
            var ed = sender as TextBox;
            if (ed != null)
            {
                if (e.Key == Key.Enter)
                {
                    ed.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    e.Handled = true;
                }
                else if (e.Key == Key.Escape)
                    ed.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class StyleNameAttribute : Attribute
    {
        private string _name;
        public StyleNameAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}
