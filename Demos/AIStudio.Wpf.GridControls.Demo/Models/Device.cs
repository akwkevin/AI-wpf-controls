using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Converter;

namespace AIStudio.Wpf.GridControls.Demo.Models
{
    public class Device : BindableBase
    {
        private string _model;
        [ColumnHeader("模式1", Visibility = Visibility.Collapsed)]
        public string Mode1
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        private string _mode2;
        [ColumnHeader("模式2", DisplayIndex = 1, CanUserSort = false)]
        public string Mode2
        {
            get => _mode2;
            set => SetProperty(ref _mode2, value);
        }

        [ColumnHeader("名称", IsReadOnly = true, DisplayIndex = 0)]
        public string Name
        {
            get; set;
        }

        private double _value1;
        [ColumnHeader("数值1", StringFormat = "f3")]
        public double Value1
        {
            get => _value1;
            set => SetProperty(ref _value1, value);
        }

        private double _value2;
        [ColumnHeader("数值2", Converter = typeof(AdditionConverter), ConverterParameter = 1)]
        public double Value2
        {
            get => _value2;
            set => SetProperty(ref _value2, value);
        }

        private double _value3;
        [ColumnHeader("数值3", ForegroundExpression = ";_p0<50_#FF0078D7——;_50<=p0 && p0<100_Red")]
        public double Value3
        {
            get => _value3;
            set => SetProperty(ref _value3, value);
        }

        private double _value4;
        [ColumnHeader("数值4", ForegroundExpression = "Value4;Value5_p0<p1_Red")]
        public double Value4
        {
            get => _value4;
            set => SetProperty(ref _value4, value);
        }

        private double _value5;
        public double Value5
        {
            get => _value5;
            set => SetProperty(ref _value5, value);
        }

        private int _value6;
        [ColumnHeader("数值6", BackgroundExpression = ";_p0<50_#FF0078D7——;_50<=p0 && p0<100_Red")]
        public int Value6
        {
            get => _value6;
            set => SetProperty(ref _value6, value);
        }

        private int _value7;
        public int Value7
        {
            get => _value7;
            set => SetProperty(ref _value7, value);
        }

        private int _value8;
        public int Value8
        {
            get => _value8;
            set => SetProperty(ref _value8, value);
        }

        private int _value9;
        public int Value9
        {
            get => _value9;
            set => SetProperty(ref _value9, value);
        }

        private int _value10;
        [ColumnHeader("数值10", Ignore = true)]
        public int Value10
        {
            get => _value10;
            set => SetProperty(ref _value10, value);
        }

        private DateTime _dateTime;
        [ColumnHeader("时间", StringFormat = "yyyy-MM-dd HH:mm:ss")]
        public DateTime DateTime
        {
            get => _dateTime;
            set => SetProperty(ref _dateTime, value);
        }
    }

    public class Device_Query : BindableBase
    {
        [ColumnHeader("名称", IsPin = true)]
        public string Name
        {
            get; set;
        }

        private string _model;
        [ColumnHeader("模式1")]
        public string Mode1
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        private string _mode2;
        [ColumnHeader("模式2")]
        public string Mode2
        {
            get => _mode2;
            set => SetProperty(ref _mode2, value);
        }

        private double _value1;
        [ColumnHeader("数值1", StringFormat = "f3")]
        public double Value1
        {
            get => _value1;
            set => SetProperty(ref _value1, value);
        }

        private DateTime _dateTime;
        [ColumnHeader("时间", StringFormat = "yyyy-MM-dd HH:mm:ss", IsPin = true)]
        public DateTime DateTime
        {
            get => _dateTime;
            set => SetProperty(ref _dateTime, value);
        }
    }
}
