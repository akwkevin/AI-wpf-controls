using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AIStudio.Wpf.GridControls.Demo.ViewModels
{
    class MainViewModel
    {
        public List<Device> Datas
        {
            get; set;
        }

        public MainViewModel()
        {
            List<Device> ds = new List<Device>();
            for (int i = 0; i < 1000; i++)
            {
                var d1 = new Device()
                {
                    Name = "MX33333333333333333333333331_" + i,
                    Manufacturer = "Meizu--" + i,
                    Mode = "M303",
                    OSType = EnumOSType.Android,
                    State = EnumDeviceState.Online,
                    Version = "4,2,1",
                    SerialNumber = i,
                    IsRoot = true,
                    DateTime = DateTime.Now.AddMinutes(0 - i),
                };
                ds.Add(d1);
            }

            Datas = ds;
        }
    }

    public class Device
    {
        public string Name
        {
            get; set;
        }
        public string Mode
        {
            get; set;
        }
        public string Manufacturer
        {
            get; set;
        }
        public int SerialNumber
        {
            get; set;
        }
        public EnumOSType OSType
        {
            get; set;
        }
        public EnumDeviceState State
        {
            get; set;
        }
        public string Version
        {
            get; set;
        }
        public bool IsRoot
        {
            get; set;
        }
        public string IsRootDesc
        {
            get
            {
                return this.IsRoot ? "已Root" : "未Root";
            }
        }

        public DateTime DateTime
        {
            get; set;
        }
        public override string ToString()
        {
            return this.Name;
        }
    }

    public enum EnumOSType
    {
        [Description("IOS")]
        IOS = 1,
        [Description("Android")]
        Android = 2,
    }

    public enum EnumDeviceState
    {
        [Description("在线")]
        Online = 1,
        [Description("离线")]
        Offline = 2,
        [Description("正忙")]
        Busy = 3,
    }
}
