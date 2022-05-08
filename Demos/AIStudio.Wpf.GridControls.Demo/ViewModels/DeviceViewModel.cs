using System;
using System.Collections.Generic;
using System.Text;
using AIStudio.Wpf.GridControls.Demo.Models;

namespace AIStudio.Wpf.GridControls.Demo.ViewModels
{
    public class DeviceViewModel : BaseViewModel<Device, Device_Query>
    {
        public DeviceViewModel() : base()
        {
            Header = "设备";
        }
    }
}
