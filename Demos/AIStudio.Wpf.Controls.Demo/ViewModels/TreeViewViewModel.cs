using System.Collections.Generic;
using AIStudio.Wpf.Controls.Demo.Models;
using AIStudio.Wpf.Controls.Demo.Services;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class TreeViewViewModel : DataViewModel
    {
        private IList<DemoDataModel> _dataList2;
        public IList<DemoDataModel> DataList2
        {
            get
            {
                return _dataList2;
            }
            set
            {
                if (_dataList2 != value)
                {
                    _dataList2 = value;
                    RaisePropertyChanged("DataList2");
                }
            }
        }

        public TreeViewViewModel()
        {
            DataList2 = DataService.GetDemoDataList(1);
        }
    }
}
