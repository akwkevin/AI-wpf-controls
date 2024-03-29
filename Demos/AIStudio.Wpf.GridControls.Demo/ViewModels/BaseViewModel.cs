﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.GridControls.Demo.Attributes;
using AIStudio.Wpf.GridControls.Demo.Commons;
using AIStudio.Wpf.GridControls.Demo.Models;
using AIStudio.Wpf.GridControls.Demo.Servers;
using Newtonsoft.Json;

namespace AIStudio.Wpf.GridControls.Demo.ViewModels
{
    public class BaseViewModel<T,Q>: BindableBase, IBaseViewModel where T : new()
    {
        private IDataProvider _dataProvider = new DataProvider();

        private ObservableCollection<T> _datas;
        public ObservableCollection<T> Datas
        {
            get
            {
                return _datas;
            }
            set
            {
                SetProperty(ref _datas, value);
            }
        }

        private T _selectedData;
        public T SelectedData
        {
            get
            {
                return _selectedData;
            }
            set
            {
                if (SetProperty(ref _selectedData, value))
                {
                    SelectedDataChanged(value);
                }
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
                SetProperty(ref _queryConditionConfigIsOpen, value);
            }
        }

        private string _header;
        public string Header
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

        public Pagination Pagination { get; set; } = new Pagination() { PageRows = 100 };

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

        private ICommand _currentIndexChangedComamnd;
        public ICommand CurrentIndexChangedComamnd
        {
            get
            {
                return this._currentIndexChangedComamnd ?? (this._currentIndexChangedComamnd = new DelegateCommand<object>(para => Query()));
            }
        }

        public BaseViewModel()
        {
            var properties_query = typeof(Q).GetProperties();
            var queryConditionItems = new List<QueryConditionItem>();
            foreach (System.Reflection.PropertyInfo info in properties_query)
            {
                QueryConditionItem queryConditionItem = QueryConditionItem.GetQueryConditionItem(info);
                if (queryConditionItem != null)
                {
                    queryConditionItems.Add(queryConditionItem);
                }
            }
            queryConditionItems.OrderBy(p => p.DisplayIndex).ToList().ForEach(p => QueryConditionItems.Add(p)); 

            QueryConditionItems.Add(new QueryConditionItem() { Header = "查询", ControlType = ControlType.Query, Visibility = System.Windows.Visibility.Visible });

            var properties = typeof(T).GetProperties();
            foreach (System.Reflection.PropertyInfo info in properties)
            {
                DataGridColumnCustom dataGridColumnCustom = ColumnHeaderAttribute.GetDataGridColumnCustom(info);
                if (dataGridColumnCustom != null)
                {
                    DataGridColumns.Add(dataGridColumnCustom);
                }
            }

            var editFormItems = new List<EditFormItem>();
            foreach (System.Reflection.PropertyInfo info in properties)
            {
                EditFormItem editFormItem = EditFormItem.GetEditFormItem(info);
                if (editFormItem != null)
                {
                    editFormItems.Add(editFormItem);
                }
            }
            editFormItems.OrderBy(p => p.DisplayIndex).ToList().ForEach(p => EditFormItems.Add(p));
            EditFormItems.Add(new EditFormItem() { Header = "提交", ControlType = ControlType.Submit, Visibility = System.Windows.Visibility.Visible });

            Query();
        }

        public async void Query()
        {
            var dic = QueryConditionItem.ListToDictionary(QueryConditionItems);
            Pagination.Keywords = dic;

            var result = await _dataProvider.GetDataList<T>($"{typeof(T).Name}/GetDataList", JsonConvert.SerializeObject(Pagination));
            if (result.Success == true)
            {
                Datas = new ObservableCollection<T>(result.Data);
                Pagination.Total = result.Total;
            }
        }

        public async void Submit()
        {
            T device = new T();
            BaseControlItem.ListToObject(device, EditFormItems);
            var result = await _dataProvider.GetData<AjaxResult>($"{typeof(T).Name}/SaveData", JsonConvert.SerializeObject(device));
            if (result.Success == true)
            {
                Query();
            }
        }

        public void QueryConditionConfig()
        {
            QueryConditionConfigIsOpen = true;
        }

        private void SelectedDataChanged(T value)
        {
            if (value != null)
            {
                BaseControlItem.ObjectToList(value, EditFormItems);
            }
        }

    }
}
