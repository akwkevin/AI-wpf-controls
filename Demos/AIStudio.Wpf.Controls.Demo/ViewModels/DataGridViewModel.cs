using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using AIStudio.Wpf.Controls.Demo.Models;
using AIStudio.Wpf.Controls.Demo.Services;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class DataGridViewModel : BaseTreeItemViewModel
    {
        private ObservableCollection<DemoDataModel> _dataList;
        public ObservableCollection<DemoDataModel> DataList
        {
            get { return _dataList; }
            set
            {
                if (_dataList != value)
                {
                    _dataList = value;
                    OnPropertyChanged("DataList");
                }
            }
        }

        private ObservableCollection<DemoDataModel> _dataList0;
        public ObservableCollection<DemoDataModel> DataList0
        {
            get { return _dataList0; }
            set
            {
                if (_dataList0 != value)
                {
                    _dataList0 = value;
                    OnPropertyChanged("DataList0");
                }
            }
        }

        private ObservableCollection<DemoDataModel> _dataList1;
        public ObservableCollection<DemoDataModel> DataList1
        {
            get { return _dataList1; }
            set
            {
                if (_dataList1 != value)
                {
                    _dataList1 = value;
                    OnPropertyChanged("DataList1");
                }
            }
        }

        private ObservableCollection<DemoDataModel> _dataList2;
        public ObservableCollection<DemoDataModel> DataList2
        {
            get { return _dataList2; }
            set
            {
                if (_dataList2 != value)
                {
                    _dataList2 = value;
                    OnPropertyChanged("DataList2");
                }
            }
        }

        private readonly ICollectionView _view;
        public ICollectionView View { get { return _view; } }

        public DataGridViewModel()
        {
            DataList = DataService.GetDemoDataList(5);
            DataList0 = DataService.GetDemoDataList(5);
            DataList1 = DataService.GetDemoDataList(5);
            DataList2 = DataService.GetDemoDataList(5);

            CompositeCollection compositeCollection = new CompositeCollection();
            compositeCollection.Add(new CollectionContainer() { Collection = DataList });
            compositeCollection.Add(new CollectionContainer() { Collection = DataList0 });
            _view = new ListCollectionView(DataList);

        }
    }
}
