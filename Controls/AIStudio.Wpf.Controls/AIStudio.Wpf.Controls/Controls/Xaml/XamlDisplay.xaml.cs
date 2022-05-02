using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace AIStudio.Wpf.Controls
{
    public class XamlDisplay : Control
    {
        #region GotFocusProperty 
        /// <summary>
        /// Border圆角
        /// </summary>
        public static readonly DependencyProperty GotFocusPropertyProperty = DependencyProperty.RegisterAttached(
            "GotFocusProperty", typeof(XamlDisplay), typeof(XamlDisplay), new FrameworkPropertyMetadata(null, OnGotFocusChanged));

        private static void OnGotFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is XamlDisplay oldvalue)
            {
                if (d is Panel panel)
                {
                    foreach (var child in GetChildren(panel).OfType<UIElement>())
                    {
                        child.GotFocus -= Child_GotFocus;
                    }
                }
            }

            if (e.NewValue is XamlDisplay newvalue)
            {
                if (d is Panel panel)
                {
                    foreach (var child in GetChildren(panel).OfType<UIElement>())
                    {
                        XamlDisplay.SetGotFocusProperty(child, newvalue);

                        child.GotFocus -= Child_GotFocus;
                        child.GotFocus += Child_GotFocus;
                    }
                }
            }
        }

        private static List<UIElement> GetChildren(Panel panel)
        {
            List<UIElement> elements = new List<UIElement>();
            foreach (var child in panel.Children)
            {
                if (child is Panel chiledpanel)
                {
                    elements.AddRange(GetChildren(chiledpanel));
                }
                else if (child is GroupBox groupBox)
                {
                    if (groupBox.Content is Panel contentpanel)
                    {
                        elements.AddRange(GetChildren(contentpanel));
                    }
                    else
                    {
                        elements.Add(groupBox.Content as UIElement);
                    }
                }
                else if (child is UIElement element)
                {
                    elements.Add(element);
                }
            }
            return elements;
        }

        private static void Child_GotFocus(object sender, RoutedEventArgs e)
        {
            var element = sender as UIElement;
            if (element != null)
            {
                var property = XamlDisplay.GetGotFocusProperty(element);
                property.SelectedObject = element;
            }
        }

        public static XamlDisplay GetGotFocusProperty(DependencyObject d)
        {
            return (XamlDisplay)d.GetValue(GotFocusPropertyProperty);
        }

        public static void SetGotFocusProperty(DependencyObject obj, XamlDisplay value)
        {
            obj.SetValue(GotFocusPropertyProperty, value);
        }
        #endregion

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
           "Text", typeof(string), typeof(XamlDisplay), new FrameworkPropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly RoutedEvent SelectedObjectChangedEvent =
           EventManager.RegisterRoutedEvent("SelectedObjectChanged", RoutingStrategy.Bubble,
               typeof(RoutedPropertyChangedEventHandler<object>), typeof(XamlDisplay));

        public event RoutedPropertyChangedEventHandler<object> SelectedObjectChanged
        {
            add => AddHandler(SelectedObjectChangedEvent, value);
            remove => RemoveHandler(SelectedObjectChangedEvent, value);
        }

        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register(
            "SelectedObject", typeof(object), typeof(XamlDisplay), new PropertyMetadata(default, OnSelectedObjectChanged));

        private static void OnSelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (XamlDisplay)d;
            ctl.OnSelectedObjectChanged(e.OldValue, e.NewValue);
        }

        public object SelectedObject
        {
            get => GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }

        protected virtual void OnSelectedObjectChanged(object oldValue, object newValue)
        {
            UpdateItems(newValue);
            RaiseEvent(new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, SelectedObjectChangedEvent));
        }

        private void UpdateItems(object obj)
        {
            if (obj == null)
                return;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = new string(' ', 4);
            settings.NewLineOnAttributes = true;
            StringBuilder sb = new StringBuilder();
            XmlWriter xmlWriter = XmlWriter.Create(sb, settings);
            XamlWriter.Save(obj, xmlWriter);
            Text = sb.ToString();
            xmlWriter.Close();
            sb = null;

            //Text = System.Windows.Markup.XamlWriter.Save(obj);

        }

        public XamlDisplay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XamlDisplay), new FrameworkPropertyMetadata(typeof(XamlDisplay)));
        }
    }
}
