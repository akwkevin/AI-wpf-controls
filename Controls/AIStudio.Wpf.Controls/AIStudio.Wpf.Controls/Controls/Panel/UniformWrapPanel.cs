using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AIStudio.Wpf.Controls
{
    public class UniformWrapPanel : WrapPanelFill
    {
        #region Public Properties      
        /// <summary>
        /// Specifies the number of columns in the grid
        /// A value of 0 indicates that the column count should be dynamically 
        /// computed based on the number of rows (if specified) and the 
        /// number of non-collapsed children in the grid
        /// </summary>
        public int Columns
        {
            get
            {
                return (int)GetValue(ColumnsProperty);
            }
            set
            {
                SetValue(ColumnsProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="Columns" /> property.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
                DependencyProperty.Register(
                        nameof(Columns),
                        typeof(int),
                        typeof(UniformWrapPanel),
                        new FrameworkPropertyMetadata(
                                (int)3,
                                FrameworkPropertyMetadataOptions.AffectsMeasure, OnColumnsChanged),
                        new ValidateValueCallback(ValidateColumns));

        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UniformWrapPanel panel)
            {
                panel.SetItemWidthHeight();
            }
        }

        private static bool ValidateColumns(object o)
        {
            return (int)o >= 0;
        }

        #endregion Public Properties

        public void SetItemWidthHeight()
        {
            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                if (this.ActualWidth > 0)
                {
                    this.ItemWidth = this.ActualWidth / this.Columns;
                    this.ItemHeight = double.NaN;
                }
            }
            else
            {
                if (this.ActualHeight > 0)
                {
                    this.ItemHeight = this.ActualHeight / this.Columns;
                    this.ItemWidth = double.NaN;
                }
            }
        }

        public UniformWrapPanel()
        {
            FillDefault = false;
            this.Loaded += UniformWrapPanel_Loaded;
            this.SizeChanged += UniformWrapPanel_SizeChanged;         
        }

        private void UniformWrapPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetItemWidthHeight();
        }

        private void UniformWrapPanel_Loaded(object sender, RoutedEventArgs e)
        {
            SetItemWidthHeight();
        }
    }
}
