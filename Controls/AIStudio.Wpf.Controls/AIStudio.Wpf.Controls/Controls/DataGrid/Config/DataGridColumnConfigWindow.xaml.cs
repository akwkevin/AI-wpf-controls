using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Commands;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// DataGridColumnConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DataGridColumnConfigWindow : WindowBase
    {
        public DataGridColumnConfigWindow(DataGrid dataGrid)
        {
            InitializeComponent();

            var viewmodel = new DataGridColumnConfigWindowViewModel(dataGrid, this);
            this.DataContext = viewmodel;
        }


    }

    public class DataGridColumnConfigWindowViewModel : BindableBase
    {
        DataGrid DataGrid;
        Window Window;

        public DataGridColumnConfigWindowViewModel(DataGrid dataGrid, Window window)
        {
            DataGrid = dataGrid;
            Window = window;

            DataGridColumns = new ObservableCollection<DataGridColumnConfig>(dataGrid.Columns.Select(p => new DataGridColumnConfig(p)));
        }

        private ObservableCollection<DataGridColumnConfig> _dataGridColumns;
        public ObservableCollection<DataGridColumnConfig> DataGridColumns
        {
            get
            {
                return _dataGridColumns;
            }
            set
            {
                SetProperty(ref _dataGridColumns, value);
            }
        }

        public ObservableCollection<Tuple<string, string>> Types
        {
            get; set;
        } = new ObservableCollection<Tuple<string, string>>()
        {
            new Tuple<string, string>("文本", typeof(DataGridTextColumn).Name),
            new Tuple<string, string>("单项", typeof(DataGridCheckBoxColumn).Name),
            new Tuple<string, string>("下拉", typeof(DataGridComboBoxColumn).Name),
            new Tuple<string, string>("链接", typeof(DataGridHyperlinkColumn).Name),
        };

        private ICommand _addCommand;
        public ICommand AddCommand
        {
            get
            {
                return this._addCommand ?? (this._addCommand = new DelegateCommand(() => this.Add()));
            }
        }

        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                return this._deleteCommand ?? (this._deleteCommand = new DelegateCommand(() => this.Delete()));
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return this._saveCommand ?? (this._saveCommand = new DelegateCommand(() => this.Save()));
            }
        }

        private void Add()
        {
            DataGridColumns.Add(new DataGridColumnConfig()
            {
                Header = "新增字段",
                PropertyName = "Field",
                Visibility = Visibility.Visible,
                Type = "DataGridTextColumn",
                DisplayIndex = DataGridColumns.Count,
            });
        }

        private void Delete()
        {
            foreach (var item in DataGridColumns.Where(p => p.IsChecked).ToList())
            {
                DataGridColumns.Remove(item);
            }
        }

        private void Save()
        {
            DataGrid.Columns.Clear();
            foreach (var item in DataGridColumns.OrderBy(p => p.DisplayIndex))
            {
                DataGrid.Columns.Add(item.GetColumn());
            }
            Window.Close();
        }
    }

    public class DataGridColumnConfig : BindableBase
    {
        public DataGridColumnConfig()
        {

        }

        public DataGridColumnConfig(DataGridColumn dataGridColumn)
        {
            DisplayIndex = dataGridColumn.DisplayIndex;
            CanUserSort = dataGridColumn.CanUserSort;
            CanUserResize = dataGridColumn.CanUserResize;
            CanUserReorder = dataGridColumn.CanUserReorder;
            Width = dataGridColumn.Width;
            Visibility = dataGridColumn.Visibility;
            Header = dataGridColumn.Header;

            if (dataGridColumn is DataGridTextColumn dataGridTextColumn)
            {
                PropertyName = (dataGridTextColumn.Binding as Binding).Path.Path;
                StringFormat = (dataGridTextColumn.Binding as Binding).StringFormat;
                Converter = (dataGridTextColumn.Binding as Binding).Converter;
                ConverterParameter = (dataGridTextColumn.Binding as Binding).ConverterParameter;
            }
            else if (dataGridColumn is DataGridCheckBoxColumn dataGridCheckBoxColumn)
            {
                PropertyName = (dataGridCheckBoxColumn.Binding as Binding).Path.Path;
                StringFormat = (dataGridCheckBoxColumn.Binding as Binding).StringFormat;
                Converter = (dataGridCheckBoxColumn.Binding as Binding).Converter;
                ConverterParameter = (dataGridCheckBoxColumn.Binding as Binding).ConverterParameter;
            }
            else if (dataGridColumn is DataGridComboBoxColumn dataGridComboBoxColumn)
            {
                PropertyName = (dataGridComboBoxColumn.SelectedValueBinding as Binding).Path.Path;
                StringFormat = (dataGridComboBoxColumn.SelectedValueBinding as Binding).StringFormat;
                Converter = (dataGridComboBoxColumn.SelectedValueBinding as Binding).Converter;
                ConverterParameter = (dataGridComboBoxColumn.SelectedValueBinding as Binding).ConverterParameter;
            }
            else if (dataGridColumn is DataGridHyperlinkColumn dataGridHyperlinkColumn)
            {
                PropertyName = (dataGridHyperlinkColumn.Binding as Binding).Path.Path;
                StringFormat = (dataGridHyperlinkColumn.Binding as Binding).StringFormat;
                Converter = (dataGridHyperlinkColumn.Binding as Binding).Converter;
                ConverterParameter = (dataGridHyperlinkColumn.Binding as Binding).ConverterParameter;
            }
            Type = dataGridColumn.GetType().Name;
            DataGridColumn = dataGridColumn;

            this.PropertyChanged += DataGridColumnConfig_PropertyChanged;
        }

        bool _isChanged;

        private void DataGridColumnConfig_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IsChecked))
            {
                _isChanged = true;
            }
        }

        public DataGridColumn GetColumn()
        {
            DataGridColumn dataGridColumn = null;

            if (_isChanged == false && DataGridColumn != null)
            {
                dataGridColumn = DataGridColumn;
            }
            else
            {
                if (Type == typeof(DataGridTextColumn).Name)
                {
                    dataGridColumn = new DataGridTextColumn();
                    var binding = new Binding(PropertyName);
                    binding.StringFormat = StringFormat;
                    binding.Converter = Converter;
                    binding.ConverterParameter = ConverterParameter;
                    (dataGridColumn as DataGridTextColumn).Binding = binding;

                }
                else if (Type == typeof(DataGridCheckBoxColumn).Name)
                {
                    dataGridColumn = new DataGridCheckBoxColumn();
                    var binding = new Binding(PropertyName);
                    binding.StringFormat = StringFormat;
                    binding.Converter = Converter;
                    binding.ConverterParameter = ConverterParameter;
                    (dataGridColumn as DataGridCheckBoxColumn).Binding = binding;
                }
                else if (Type == typeof(DataGridComboBoxColumn).Name)
                {
                    dataGridColumn = new DataGridComboBoxColumn();
                    var binding = new Binding(PropertyName);
                    binding.StringFormat = StringFormat;
                    binding.Converter = Converter;
                    binding.ConverterParameter = ConverterParameter;
                    (dataGridColumn as DataGridComboBoxColumn).SelectedValueBinding = binding;
                    (dataGridColumn as DataGridComboBoxColumn).DisplayMemberPath = "Text";
                    (dataGridColumn as DataGridComboBoxColumn).SelectedValuePath = "Value";
                }
                else if (Type == typeof(DataGridHyperlinkColumn).Name)
                {
                    dataGridColumn = new DataGridHyperlinkColumn();
                    var binding = new Binding(PropertyName);
                    binding.StringFormat = StringFormat;
                    binding.Converter = Converter;
                    binding.ConverterParameter = ConverterParameter;
                    (dataGridColumn as DataGridHyperlinkColumn).Binding = binding;
                }
                else
                {
                    throw new Exception("该类型未支持");
                }

                dataGridColumn.CanUserSort = CanUserSort;
                dataGridColumn.CanUserResize = CanUserResize;
                dataGridColumn.CanUserReorder = CanUserReorder;
                dataGridColumn.Width = Width;
                dataGridColumn.Visibility = Visibility;
                dataGridColumn.Header = Header;
            }
            return dataGridColumn;
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                SetProperty(ref _isChecked, value);
            }
        }

        private int _displayIndex;
        public int DisplayIndex
        {
            get
            {
                return _displayIndex;
            }
            set
            {
                SetProperty(ref _displayIndex, value);
            }
        }

        private string _propertyName;
        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                SetProperty(ref _propertyName, value);
            }
        }

        private bool _canUserSort = true;
        public bool CanUserSort
        {
            get
            {
                return _canUserSort;
            }
            set
            {
                SetProperty(ref _canUserSort, value);
            }
        }

        private bool _canUserResize = true;
        public bool CanUserResize
        {
            get
            {
                return _canUserResize;
            }
            set
            {
                SetProperty(ref _canUserResize, value);
            }
        }

        private bool _canUserReorder = true;
        public bool CanUserReorder
        {
            get
            {
                return _canUserReorder;
            }
            set
            {
                SetProperty(ref _canUserReorder, value);
            }
        }

        private DataGridLength _width = new DataGridLength(0, DataGridLengthUnitType.Auto);
        public DataGridLength Width
        {
            get
            {
                return _width;
            }
            set
            {
                SetProperty(ref _width, value);
            }
        }

        private Visibility _visibility = new Visibility();
        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                SetProperty(ref _visibility, value);
            }
        }

        private object _header;
        public object Header
        {
            get
            {
                return _header;
            }
            set
            {
                SetProperty(ref _header, value);
            }
        }

        private string _stringFormat;
        public string StringFormat
        {
            get
            {
                return _stringFormat;
            }
            set
            {
                SetProperty(ref _stringFormat, value);
            }
        }

        private IValueConverter _converter;
        public IValueConverter Converter
        {
            get
            {
                return _converter;
            }
            set
            {
                SetProperty(ref _converter, value);
            }
        }

        private object _converterParameter;
        public object ConverterParameter
        {
            get
            {
                return _converterParameter;
            }
            set
            {
                SetProperty(ref _converterParameter, value);
            }
        }

        public string Type
        {
            get; set;
        }

        public DataGridColumn DataGridColumn
        {
            get; set;
        }



    }
}
