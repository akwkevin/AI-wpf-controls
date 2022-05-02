using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Demo.Models;
using AIStudio.Wpf.Controls.Demo.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class TagViewModel : BindableBase
    {
        private IList<TagDataModel> _dataList;
        public IList<TagDataModel> DataList
        {
            get
            {
                return _dataList;
            }
            set
            {
                if (_dataList != value)
                {
                    _dataList = value;
                    RaisePropertyChanged("DataList");
                }
            }
        }

        private IList<TagDataModel> _dataList2;
        public IList<TagDataModel> DataList2
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
        public TagViewModel()
        {
            DataList = new ObservableCollection<TagDataModel>();
            for (var i = 1; i <= 5; i++)
            {
                var index = i % 6 + 1;

                var model = new TagDataModel
                {
                    Index = i,
                    IsSelected = i % 2 == 0,
                    Name = $"Name{i}",
                    ImgPath = $"/AIStudio.Wpf.Controls.Demo;component/Resources/Images/{i % 5 + 1}.jpg",
                    Remark = new string(i.ToString()[0], 10)
                };
                DataList.Add(model);
            }

            DataList2 = new ObservableCollection<TagDataModel>();
            for (var i = 1; i <= 5; i++)
            {
                var index = i % 6 + 1;

                var model = new TagDataModel
                {
                    Index = i,
                    IsSelected = i % 2 == 0,
                    Name = $"Name{i}",
                    ImgPath = $"/AIStudio.Wpf.Controls.Demo;component/Resources/Images/{i % 5 + 1}.jpg",
                    Remark = new string(i.ToString()[0], 10)
                };
                DataList2.Add(model);
            }
        }
    }

    public class TagDataModel : DemoDataModel
    {
        public bool IsAdd
        {
            get;set;
        }
    }
}
