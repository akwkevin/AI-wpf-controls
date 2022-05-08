using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using AIStudio.Wpf.Controls;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.GridControls.Demo.Models;
using AIStudio.Wpf.GridControls.Demo.Servers;
using Newtonsoft.Json;

namespace AIStudio.Wpf.GridControls.Demo.ViewModels
{
    class MainViewModel : BindableBase
    {
        private ObservableCollection<IBaseViewModel> _datas = new ObservableCollection<IBaseViewModel>();
        public ObservableCollection<IBaseViewModel> Datas
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

        public MainViewModel()
        {
            Datas.Add(new DeviceViewModel());

            DicBaseViewModel deviceviewmodel = new DicBaseViewModel("设备(无模型方法)", "Device");
            Datas.Add(deviceviewmodel);
        }
    }


}
