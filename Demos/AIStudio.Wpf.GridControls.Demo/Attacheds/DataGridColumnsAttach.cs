using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using AIStudio.Wpf.GridControls.Demo.Models;

namespace AIStudio.Wpf.GridControls.Demo
{
    public class DataGridColumnsAttach
    {
        public static readonly DependencyProperty BindableColumnsProperty =
            DependencyProperty.RegisterAttached(
                "BindableColumns",
                typeof(ObservableCollection<DataGridColumnCustom>),
                typeof(DataGridColumnsAttach),
                new UIPropertyMetadata(null, OnBindableColumnsChanged));

        public static void SetBindableColumns(DependencyObject element, ObservableCollection<DataGridColumnCustom> value)
        {
            element.SetValue(BindableColumnsProperty, value);
        }

        public static ObservableCollection<DataGridColumnCustom> GetBindableColumns(DependencyObject element)
        {
            return (ObservableCollection<DataGridColumnCustom>)element.GetValue(BindableColumnsProperty);
        }

        private static void OnBindableColumnsChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = source as DataGrid;
            ObservableCollection<DataGridColumnCustom> columns = e.NewValue as ObservableCollection<DataGridColumnCustom>;
            if (dataGrid != null)
            {
                dataGrid.Columns.Clear();
                if (columns == null)
                {
                    return;
                }

                foreach (DataGridColumnCustom column in columns)
                {
                    dataGrid.Columns.Add(GetDataColumn(column));
                }

                columns.CollectionChanged += (sender, e2) => {
                    NotifyCollectionChangedEventArgs ne = e2;
                    if (ne.Action == NotifyCollectionChangedAction.Reset)
                    {
                        dataGrid.Columns.Clear();
                        foreach (DataGridColumnCustom column in ne.NewItems)
                        {
                            dataGrid.Columns.Add(GetDataColumn(column));
                        }
                    }
                    else if (ne.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (DataGridColumnCustom column in ne.NewItems)
                        {
                            dataGrid.Columns.Add(GetDataColumn(column));
                        }
                    }
                    else if (ne.Action == NotifyCollectionChangedAction.Move)
                    {
                        dataGrid.Columns.Move(ne.OldStartingIndex, ne.NewStartingIndex);
                    }
                    else if (ne.Action == NotifyCollectionChangedAction.Remove)
                    {
                        foreach (DataGridColumnCustom column in ne.OldItems)
                        {
                            dataGrid.Columns.Remove(GetDataColumn(column));
                        }
                    }
                    else if (ne.Action == NotifyCollectionChangedAction.Replace)
                    {
                        dataGrid.Columns[ne.NewStartingIndex] = GetDataColumn(ne.NewItems[0] as DataGridColumnCustom);
                    }
                };
            }
        }

        private static DataGridColumn GetDataColumn(DataGridColumnCustom columnCustom)
        {
            var column = new DataGridTemplateColumn();
            column.IsReadOnly = true;
            column.Header = columnCustom.Header;


            DataTemplate dt = new DataTemplate();

            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));

            Binding bind = new Binding(columnCustom.Binding);
            fef.SetBinding(Label.ContentProperty, bind);
            fef.SetValue(Label.VerticalContentAlignmentProperty, VerticalAlignment.Center);
            fef.SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);

            if (!string.IsNullOrEmpty(columnCustom.StringFormat))
            {
                bind.StringFormat = columnCustom.StringFormat;
            }
            if (columnCustom.Converter != null)
            {
                bind.Converter = columnCustom.Converter;
                bind.ConverterParameter = columnCustom.ConverterParameter;
            }
            //**************
            if (!string.IsNullOrEmpty(columnCustom.ForegroundExpression))
            {
                Binding bindColor = new Binding(columnCustom.ForegroundExpression.StartsWith(";") ? columnCustom.Binding : ".");
                bindColor.Converter = new Demo.Converter.ExpressionConverter();
                bindColor.ConverterParameter = columnCustom.ForegroundExpression;
                fef.SetBinding(Label.ForegroundProperty, bindColor);
            }
            if (!string.IsNullOrEmpty(columnCustom.BackgroundExpression))
            {
                Binding bindColor = new Binding(columnCustom.BackgroundExpression.StartsWith(";") ? columnCustom.Binding : ".");
                bindColor.Converter = new Demo.Converter.ExpressionConverter();
                bindColor.ConverterParameter = columnCustom.BackgroundExpression;
                fef.SetBinding(Label.BackgroundProperty, bindColor);
            }
            dt.VisualTree = fef;
            column.CellTemplate = dt;
            return column;
        }
    }
}
