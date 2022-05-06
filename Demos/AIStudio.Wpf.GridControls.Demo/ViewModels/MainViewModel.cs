using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.GridControls.Demo.Models;
using AIStudio.Wpf.GridControls.Demo.Servers;
using Newtonsoft.Json;

namespace AIStudio.Wpf.GridControls.Demo.ViewModels
{
    class MainViewModel : BindableBase
    {
        private IDataProvider _dataProvider = new DataProvider();

        private ObservableCollection<Device> _datas;
        public ObservableCollection<Device> Datas
        {
            get
            {
                return _datas;
            }
            set
            {
                if (_datas != value)
                {
                    _datas = value;
                    RaisePropertyChanged("Datas");
                }
            }
        }

        private Device _selectedData;
        public Device SelectedData
        {
            get
            {
                return _selectedData;
            }
            set
            {
                if (_selectedData != value)
                {
                    _selectedData = value;
                    RaisePropertyChanged("SelectedData");
                    SelectedDataChanged(value);
                }
            }
        }


        public ObservableCollection<DataGridColumnCustom> DataGridColumns
        {
            get; private set;
        } = new ObservableCollection<DataGridColumnCustom>();

        public ObservableCollection<QueryConditionItem> QueryConditionItems
        {
            get; private set;
        } = new ObservableCollection<QueryConditionItem>();

        public ObservableCollection<EditFormItem> EditFormItems
        {
            get; private set;
        } = new ObservableCollection<EditFormItem>();

        private ICommand _queryCommand;
        public ICommand QueryCommand
        {
            get
            {
                return this._queryCommand ?? (this._queryCommand = new DelegateCommand(() => this.Query()));
            }
        }

        private ICommand _submitCommand;
        public ICommand SubmitCommand
        {
            get
            {
                return this._submitCommand ?? (this._submitCommand = new DelegateCommand(() => this.Submit()));
            }
        }

        private ICommand _queryConditionConfigCommand;
        public ICommand QueryConditionConfigCommand
        {
            get
            {
                return this._queryConditionConfigCommand ?? (this._queryConditionConfigCommand = new DelegateCommand(() => this.QueryConditionConfig()));
            }
        }

        private bool _queryConditionConfigIsOpen;
        public bool QueryConditionConfigIsOpen
        {
            get
            {
                return _queryConditionConfigIsOpen;
            }
            set
            {
                if (_queryConditionConfigIsOpen != value)
                {
                    _queryConditionConfigIsOpen = value;
                    RaisePropertyChanged("QueryConditionConfigIsOpen");
                }
            }
        }

        public MainViewModel()
        {
            var properties = typeof(Device).GetProperties();
            foreach (System.Reflection.PropertyInfo info in properties)
            {
                DataGridColumnCustom dataGridColumnCustom = ColumnHeaderAttribute.GetDataGridColumnCustom(info);
                if (dataGridColumnCustom != null)
                {
                    DataGridColumns.Add(dataGridColumnCustom);
                }
            }
            foreach (System.Reflection.PropertyInfo info in properties)
            {
                QueryConditionItem queryConditionItem = QueryConditionItem.GetQueryConditionItem(info);
                if (queryConditionItem != null)
                {
                    QueryConditionItems.Add(queryConditionItem);
                }
            }
            foreach (System.Reflection.PropertyInfo info in properties)
            {
                EditFormItem editFormItem = EditFormItem.GetEditFormItem(info);
                if (editFormItem != null)
                {
                    EditFormItems.Add(editFormItem);
                }
            }
            QueryConditionItems.Add(new QueryConditionItem() { Header = "查询", ControlType = ControlType.Query, Visibility = System.Windows.Visibility.Collapsed });
            EditFormItems.Add(new EditFormItem() { Header = "提交", ControlType = ControlType.Submit });

            Query();
        }

        public async void Query()
        {      
            var dic = QueryConditionItem.ListToDictionary(QueryConditionItems);
            var result = await _dataProvider.GetData<List<Device>>("Device", dic);
            Datas = new ObservableCollection<Device>(result.Data);
        }

        public void Submit()
        {
            var dic = QueryConditionItem.ListToDictionary(EditFormItems);
        }

        public void QueryConditionConfig()
        {
            QueryConditionConfigIsOpen = true;
        }

        private void SelectedDataChanged(Device value)
        {
            QueryConditionItem.ObjectToList(value, EditFormItems);

        }


    }


}
