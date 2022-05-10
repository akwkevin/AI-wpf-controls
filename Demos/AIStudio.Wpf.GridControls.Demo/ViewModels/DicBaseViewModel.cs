using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.GridControls.Demo.Attributes;
using AIStudio.Wpf.GridControls.Demo.Commons;
using AIStudio.Wpf.GridControls.Demo.Extensions;
using AIStudio.Wpf.GridControls.Demo.Models;
using AIStudio.Wpf.GridControls.Demo.Servers;
using Newtonsoft.Json;

namespace AIStudio.Wpf.GridControls.Demo.ViewModels
{
    public class DicBaseViewModel : BindableBase, IBaseViewModel
    {
        private IDataProvider _dataProvider = new DataProvider();

        public string Name
        {
            get;set;
        }

        private ObservableCollection<ExpandoObject> _datas;
        public ObservableCollection<ExpandoObject> Datas
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

        private ExpandoObject _selectedData;
        public ExpandoObject SelectedData
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

        public DicBaseViewModel(string header, string name)
        {
            Header = header;
            Name = name;

            GetConfig();
            Query();
        }

        public async void GetConfig()
        {
            var result = await _dataProvider.GetData<Tuple<List<QueryConditionItem>, List<DataGridColumnCustom>, List<EditFormItem>>>($"{Name}/GetConfig", JsonConvert.SerializeObject(Name));
            if (result.Success == true)
            {
                result.Data.Item1.OrderBy(p => p.DisplayIndex).ToList().ForEach(p => QueryConditionItems.Add(p));
                result.Data.Item2.OrderBy(p => p.DisplayIndex).ToList().ForEach(p => DataGridColumns.Add(p));
                result.Data.Item3.OrderBy(p => p.DisplayIndex).ToList().ForEach(p => EditFormItems.Add(p));
            }
        }

        public async void Query()
        {
            var dic = QueryConditionItem.ListToDictionary(QueryConditionItems);
            Pagination.Keywords = dic;

            var result = await _dataProvider.GetDataList<Dictionary<string, object>>($"{Name}/GetDicDataList", JsonConvert.SerializeObject(Pagination));
            if (result.Success == true)
            {
                Datas = new ObservableCollection<ExpandoObject>(result.Data.Select(p => p.DicToExpandoObject()));
                Pagination.Total = result.Total;
            }
        }

        public async void Submit()
        {
            var dic = BaseControlItem.ListToDictionary(EditFormItems);
            var result = await _dataProvider.GetData<AjaxResult>($"{Name}/SaveDicData", JsonConvert.SerializeObject(dic));
            if (result.Success == true)
            {
                Query();
            }
        }

        public void QueryConditionConfig()
        {
            QueryConditionConfigIsOpen = true;
        }

        private void SelectedDataChanged(ExpandoObject value)
        {
            if (value != null)
            {
                BaseControlItem.ObjectToList(value, EditFormItems);
            }
        }
    }
}
