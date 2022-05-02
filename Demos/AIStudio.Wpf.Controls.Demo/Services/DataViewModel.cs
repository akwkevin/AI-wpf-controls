using System.Collections.Generic;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Demo.Models;

namespace AIStudio.Wpf.Controls.Demo.Services
{
    class DataViewModel : BindableBase
    {
        private IList<DemoDataModel> _dataList;
        public IList<DemoDataModel> DataList
        {
            get { return _dataList; }
            set
            {
                if (_dataList != value)
                {
                    _dataList = value;
                    RaisePropertyChanged("DataList");
                }
            }
        }
        public DataViewModel()
        {
            DataList = DataService.GetDemoDataList(5);
        }
    }
}
