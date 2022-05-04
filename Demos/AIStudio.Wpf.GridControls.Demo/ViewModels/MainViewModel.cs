using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AIStudio.Wpf.GridControls.Demo.Models;
using Newtonsoft.Json;

namespace AIStudio.Wpf.GridControls.Demo.ViewModels
{
    class MainViewModel
    {
        public List<Device> Datas
        {
            get; set;
        }


        public ObservableCollection<DataGridColumnCustom> DataGridColumns
        {
            get; private set;
        } = new ObservableCollection<DataGridColumnCustom>();

        public MainViewModel()
        {
            List<Device> ds = new List<Device>();
            Random rd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var d1 = new Device()
                {
                    Name = "MX33333333333333333333333331_" + i,
                    Mode1 = "M303" + i,
                    Mode2 = "M303" + i,
                    Value1 = i,
                    Value2 = i,
                    Value3 = i,
                    Value4 = i + rd.NextDouble(),
                    Value5 = i + rd.NextDouble(),
                    Value6 = i,
                    Value7 = i,
                    Value8 = i,
                    Value9 = i,
                    Value10 = i,
                    DateTime = DateTime.Now.AddMinutes(0 - i),
                };
                ds.Add(d1);
            }

            Datas = ds;

            var properties = typeof(Device).GetProperties();
            foreach (System.Reflection.PropertyInfo info in properties)
            {
                DataGridColumnCustom dataGridColumnCustom = ColumnHeaderAttribute.GetDataGridColumnCustom(info);
                if (dataGridColumnCustom != null)
                {
                    DataGridColumns.Add(dataGridColumnCustom);
                }                
            }

            var str = JsonConvert.SerializeObject(DataGridColumns);
        }
    }




}
