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
                StringFormat = dataGridTextColumn.Binding.StringFormat;
            }
            else if (dataGridColumn is DataGridCheckBoxColumn dataGridCheckBoxColumn)
            {
                PropertyName = (dataGridCheckBoxColumn.Binding as Binding).Path.Path;
            }
            else if (dataGridColumn is DataGridComboBoxColumn dataGridComboBoxColumn)
            {
                PropertyName = (dataGridComboBoxColumn.SelectedValueBinding as Binding).Path.Path;
            }
            else if (dataGridColumn is DataGridHyperlinkColumn dataGridHyperlinkColumn)
            {
                PropertyName = (dataGridHyperlinkColumn.Binding as Binding).Path.Path;
            }
            Type = dataGridColumn.GetType().Name;
            DataGridColumn = dataGridColumn;
        }

        public DataGridColumn GetColumn()
        {
            DataGridColumn dataGridColumn = null;
            if (Type == typeof(DataGridTextColumn).Name)
            {
                dataGridColumn = new DataGridTextColumn();
                var binding = new Binding(PropertyName);
                binding.StringFormat = StringFormat;
                (dataGridColumn as DataGridTextColumn).Binding = binding;
            }
            else if (Type == typeof(DataGridCheckBoxColumn).Name)
            {
                dataGridColumn = new DataGridCheckBoxColumn();
                var binding = new Binding(PropertyName);
                (dataGridColumn as DataGridCheckBoxColumn).Binding = binding;
            }
            else if (Type == typeof(DataGridComboBoxColumn).Name)
            {
                dataGridColumn = new DataGridComboBoxColumn();
                var binding = new Binding(PropertyName);
                (dataGridColumn as DataGridComboBoxColumn).SelectedValueBinding = binding;
                (dataGridColumn as DataGridComboBoxColumn).DisplayMemberPath = "Text";
                (dataGridColumn as DataGridComboBoxColumn).SelectedValuePath = "Value";
            }
            else if (Type == typeof(DataGridHyperlinkColumn).Name)
            {
                dataGridColumn = new DataGridHyperlinkColumn();
                var binding = new Binding(PropertyName);
                (dataGridColumn as DataGridHyperlinkColumn).Binding = binding;
            }
            else
            {
                dataGridColumn = DataGridColumn;
            }

            dataGridColumn.CanUserSort = CanUserSort;
            dataGridColumn.CanUserResize = CanUserResize;
            dataGridColumn.CanUserReorder = CanUserReorder;
            dataGridColumn.Width = Width;
            dataGridColumn.Visibility = Visibility;
            dataGridColumn.Header = Header;

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
                if (value != _isChecked)
                {
                    _isChecked = value;
                    RaisePropertyChanged("IsChecked");
                }
            }
        }

        public int DisplayIndex
        {
            get; set;
        }

        public string PropertyName
        {
            get; set;
        }

        public bool CanUserSort
        {
            get; set;
        } = true;

        public bool CanUserResize
        {
            get; set;
        } = true;

        public bool CanUserReorder
        {
            get; set;
        } = true;

        public DataGridLength Width
        {
            get; set;
        } = new DataGridLength(0, DataGridLengthUnitType.Auto);

        public Visibility Visibility
        {
            get; set;
        } = new Visibility();

        public object Header
        {
            get; set;
        }

        public string StringFormat
        {
            get; set;
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
