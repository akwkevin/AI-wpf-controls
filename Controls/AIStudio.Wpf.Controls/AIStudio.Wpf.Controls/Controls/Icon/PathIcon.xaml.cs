using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class PathIcon : ContentControl
    {
        public static Lazy<IDictionary<Tuple<string, string>, Geometry>> DataIndex = new Lazy<IDictionary<Tuple<string, string>, Geometry>>(PathDataFactory.Create);

        static PathIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathIcon), new FrameworkPropertyMetadata(typeof(PathIcon)));
        }

        public PathIcon()
        {


        }

        public static readonly DependencyProperty KindProperty
           = DependencyProperty.Register(nameof(Kind), typeof(string), typeof(PathIcon), new PropertyMetadata(default(string), OnKindChanged));

        private static void OnKindChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((PathIcon)dependencyObject).UpdateData();
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
            = DependencyProperty.Register(nameof(Data), typeof(Geometry), typeof(PathIcon), new PropertyMetadata(null));

        /// <summary>
        /// Gets the icon path data for the current <see cref="Kind"/>.
        /// </summary>
        public Geometry Data
        {
            get
            {
                return (Geometry)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        private static readonly DependencyProperty ThemeProperty
          = DependencyProperty.Register(nameof(Theme), typeof(string), typeof(PathIcon), new PropertyMetadata("Geometries"));

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
            Geometry data = null;
            if (DataIndex.Value != null)
            {
                if (!string.IsNullOrEmpty(Theme))
                    DataIndex.Value.TryGetValue(new Tuple<string, string>(Theme, Kind), out data);
                if (data == null)
                    DataIndex.Value.TryGetValue(new Tuple<string, string>("", Kind), out data);
            }

            Data = data;
        }
    }
}
