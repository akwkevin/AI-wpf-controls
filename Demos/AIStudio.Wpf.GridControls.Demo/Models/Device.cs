using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using AIStudio.Wpf.Controls.Converter;

namespace AIStudio.Wpf.GridControls.Demo.Models
{
    public class Device
    {
        [ColumnHeader("名称")]
        public string Name
        {
            get; set;
        }

        [ColumnHeader("模式1", Visibility = Visibility.Collapsed)]
        public string Mode1
        {
            get; set;
        }

        [ColumnHeader("模式2")]
        public string Mode2
        {
            get; set;
        }

        [ColumnHeader("数值1", StringFormat = "f3")]
        public double Value1
        {
            get; set;
        }
        [ColumnHeader("数值2", Converter = typeof(AdditionConverter), ConverterParameter = 1)]
        public double Value2
        {
            get; set;
        }
        [ColumnHeader("数值3", ForegroundExpression = ";_p0<50_#FF0078D7——;_50<=p0 && p0<100_Red")]
        public double Value3
        {
            get; set;
        }
        [ColumnHeader("数值4", ForegroundExpression = "Value4;Value5_p0<p1_Red")]
        public double Value4
        {
            get; set;
        }
        public double Value5
        {
            get; set;
        }
        [ColumnHeader("数值6", BackgroundExpression = ";_p0<50_#FF0078D7——;_50<=p0 && p0<100_Red")]
        public int Value6
        {
            get; set;
        }
        public int Value7
        {
            get; set;
        }
        public int Value8
        {
            get; set;
        }
        public int Value9
        {
            get; set;
        }

        [ColumnHeader("数值10", Ignore = true)]
        public int Value10
        {
            get; set;
        }

        [ColumnHeader("时间", StringFormat = "yyyy-MM-dd HH:mm:ss")]
        public DateTime DateTime
        {
            get; set;
        }
    }
}
