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
                    foreach (var child in GetChildren(panel).OfType<FrameworkElement>())
                    {
                        child.GotFocus -= Child_GotFocus;
                    }
                }
            }

            if (e.NewValue is XamlDisplay newvalue)
            {
                if (d is Panel panel)
                {
                    foreach (var child in GetChildren(panel).OfType<FrameworkElement>())
                    {
                        XamlDisplay.SetGotFocusProperty(child, newvalue);

                        child.GotFocus -= Child_GotFocus;
                        child.GotFocus += Child_GotFocus;
                    }
                }
            }
        }

        private static List<FrameworkElement> GetChildren(Panel panel)
        {
            List<FrameworkElement> elements = new List<FrameworkElement>();
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
                        elements.Add(groupBox.Content as FrameworkElement);
                    }
                }
                else if (child is FrameworkElement element)
                {
                    elements.Add(element);
                }
            }
            return elements;
        }

        private static void Child_GotFocus(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
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
           nameof(Text), typeof(string), typeof(XamlDisplay), new FrameworkPropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty IsStyleProperty = DependencyProperty.Register(
          nameof(IsStyle), typeof(bool), typeof(XamlDisplay), new FrameworkPropertyMetadata(true));

        public bool IsStyle
        {
            get => (bool)GetValue(IsStyleProperty);
            set => SetValue(IsStyleProperty, value);
        }

        public static readonly RoutedEvent SelectedObjectChangedEvent =
           EventManager.RegisterRoutedEvent(nameof(SelectedObjectChanged), RoutingStrategy.Bubble,
               typeof(RoutedPropertyChangedEventHandler<object>), typeof(XamlDisplay));

        public event RoutedPropertyChangedEventHandler<object> SelectedObjectChanged
        {
            add => AddHandler(SelectedObjectChangedEvent, value);
            remove => RemoveHandler(SelectedObjectChangedEvent, value);
        }

        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register(
            nameof(SelectedObject), typeof(FrameworkElement), typeof(XamlDisplay), new PropertyMetadata(default, OnSelectedObjectChanged));

        private static void OnSelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (XamlDisplay)d;
            ctl.OnSelectedObjectChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
        }

        public FrameworkElement SelectedObject
        {
            get => (FrameworkElement)GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }

        protected virtual void OnSelectedObjectChanged(FrameworkElement oldValue, FrameworkElement newValue)
        {
            UpdateItem(newValue);
            RaiseEvent(new RoutedPropertyChangedEventArgs<FrameworkElement>(oldValue, newValue, SelectedObjectChangedEvent));
        }

        private void UpdateItem(FrameworkElement control)
        {
            if (control == null)
                return;

            var appearance = IsStyle ? Application.Current.FindResource(control.GetType()) : control;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = new string(' ', 4);
            settings.NewLineOnAttributes = true;
            StringBuilder sb = new StringBuilder();
            XmlWriter xmlWriter = XmlWriter.Create(sb, settings);
            XamlWriter.Save(appearance, xmlWriter);
            Text = sb.ToString();
            xmlWriter.Close();

        }

        public XamlDisplay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XamlDisplay), new FrameworkPropertyMetadata(typeof(XamlDisplay)));
        }

 
    }
}
