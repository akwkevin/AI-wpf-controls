using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Serialization;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.Controls.Helper;
using Microsoft.Win32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Documents;

namespace AIStudio.Wpf.Controls
{

    [TemplatePart(Name = PART_Right, Type = typeof(DataGridScrollView))]
    [TemplatePart(Name = PART_ScrollViewer, Type = typeof(ScrollViewer))]
    public class DataGridRightFrozen : DataGrid
    {
        #region 常量
        private const string PART_Right = "PART_Right";
        private const string PART_ScrollViewer = "PART_ScrollViewer";

        private DataGridScrollView _rightDataGrid;
        private ScrollViewer _rightScrollViewer;
        private ScrollViewer _scrollViewer;
        #endregion

        public int RightFrozenCount
        {
            get
            {
                return (int)GetValue(RightFrozenCountProperty);
            }
            set
            {
                SetValue(RightFrozenCountProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for RightFrozenCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightFrozenCountProperty =
            DependencyProperty.Register(nameof(RightFrozenCount), typeof(int), typeof(DataGridRightFrozen), new PropertyMetadata(0, OnRightFrozenCountChanged));

        private static void OnRightFrozenCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGridRightFrozen dataGridRightFrozen)
            {
                dataGridRightFrozen.OnRightFrozenCountChanged();
            }
        }

        private void OnRightFrozenCountChanged()
        {
            if (_rightDataGrid != null)
            {
                if (RightFrozenCount > 0)
                {
                    for (int i = 0; i < _rightDataGrid.Columns.Count; i++)
                    {
                        var column = _rightDataGrid.Columns[i];
                        _rightDataGrid.Columns.Remove(column);
                        this.Columns.Add(column);
                    }
                    for (int i = 0; i < RightFrozenCount; i++)
                    {
                        var last = Columns[Columns.Count - 1];
                        this.Columns.Remove(last);

                        _rightDataGrid.Columns.Insert(0, last);
                    }
                    _rightDataGrid.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                }
                else
                {
                    _rightDataGrid.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                }
            }
        }
        #region ctor
        static DataGridRightFrozen()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridRightFrozen),
                new FrameworkPropertyMetadata(typeof(DataGridRightFrozen)));
        }

        public DataGridRightFrozen()
        {
            this.Loaded += DataGridRightFrozen_Loaded;
        }

        private void DataGridRightFrozen_Loaded(object sender, RoutedEventArgs e)
        {
            OnRightFrozenCountChanged();
        }
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
            }
            if (_rightScrollViewer != null)
            {
                _rightScrollViewer.ScrollChanged -= _rightScrollViewer_ScrollChanged;
            }
            if (_rightDataGrid != null)
            {
                _rightDataGrid.ScrollViewerChanged -= ScrollViewerChanged;
                _rightDataGrid.SelectionChanged -= _rightDataGrid_SelectionChanged;
            }

            _scrollViewer = GetTemplateChild(PART_ScrollViewer) as ScrollViewer;
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }

            _rightDataGrid = GetTemplateChild(PART_Right) as DataGridScrollView;
            if (_rightDataGrid != null)
            {
                _rightDataGrid.ScrollViewerChanged += ScrollViewerChanged;
                _rightDataGrid.SelectionChanged += _rightDataGrid_SelectionChanged;
            }
            this.SelectionChanged += DataGridRightFrozen_SelectionChanged;
        }

        private void ScrollViewerChanged(ScrollViewer viewer)
        {
            _rightScrollViewer = viewer;
            _rightScrollViewer.ScrollChanged += _rightScrollViewer_ScrollChanged;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_rightScrollViewer != null)
            {
                _rightScrollViewer.ScrollToVerticalOffset(_scrollViewer.VerticalOffset);
            }
        }

        private void _rightScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollToVerticalOffset(_rightScrollViewer.VerticalOffset);
            }
        }

        private void _rightDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SetCurrentValue(SelectedItemProperty, _rightDataGrid.SelectedItem);
        }

        private void DataGridRightFrozen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _rightDataGrid.SetCurrentValue(SelectedItemProperty, SelectedItem);
        }
    }
    [TemplatePart(Name = PART_ScrollViewer, Type = typeof(ScrollViewer))]
    public class DataGridScrollView : DataGrid
    {
        private const string PART_ScrollViewer = "PART_ScrollViewer";
        public ScrollViewer ScrollViewer
        {
            get; set;
        }

        public Action<ScrollViewer> ScrollViewerChanged;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ScrollViewer = GetTemplateChild(PART_ScrollViewer) as ScrollViewer;
            if (ScrollViewer != null)
            {
                ScrollViewerChanged?.Invoke(ScrollViewer);
            }
        }
    }
}
