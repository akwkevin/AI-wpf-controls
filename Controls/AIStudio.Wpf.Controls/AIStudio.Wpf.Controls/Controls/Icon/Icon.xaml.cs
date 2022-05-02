using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    public class Icon : ContentControl
    {
        public static Lazy<IDictionary<Tuple<string, string>, string>> DataIndex = new Lazy<IDictionary<Tuple<string, string>, string>>(IconDataFactory.Create);

        static Icon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(typeof(Icon)));
        }

        public Icon()
        {


        }

        public static readonly DependencyProperty KindProperty
           = DependencyProperty.Register(nameof(Kind), typeof(string), typeof(Icon), new PropertyMetadata(default(string), OnKindChanged));

        private static void OnKindChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((Icon)dependencyObject).UpdateData();
        }

        /// <summary>
        /// Gets or sets the icon to display.
        /// </summary>
        public string Kind
        {
            get
            {
                return (string)GetValue(KindProperty);
            }
            set
            {
                SetValue(KindProperty, value);
            }
        }

        public static readonly DependencyProperty DataProperty
            = DependencyProperty.Register(nameof(Data), typeof(string), typeof(Icon), new PropertyMetadata(""));

        /// <summary>
        /// Gets the icon path data for the current <see cref="Kind"/>.
        /// </summary>
        public string Data
        {
            get
            {
                return (string)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        private static readonly DependencyProperty ThemeProperty
          = DependencyProperty.Register(nameof(Theme), typeof(string), typeof(Icon), new PropertyMetadata("Awesome"));

        /// <summary>
        /// fill,outline,twotone,default=outline  <see cref="Theme"/>. 
        /// </summary>
        public string Theme
        {
            get
            {
                return (string)GetValue(ThemeProperty);
            }
            set
            {
                SetValue(ThemeProperty, value);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateData();
        }


        internal void UpdateData()
        {
            if (string.IsNullOrEmpty(Kind)) return;
            string data = null;
            if (DataIndex.Value != null)
            {
                if (!string.IsNullOrEmpty(Theme))
                    DataIndex.Value.TryGetValue(new Tuple<string, string>(Theme, Kind), out data);
                if (string.IsNullOrEmpty(data))
                    DataIndex.Value.TryGetValue(new Tuple<string, string>("", Kind), out data);
            }


            Data = data;
        }
    }
}
