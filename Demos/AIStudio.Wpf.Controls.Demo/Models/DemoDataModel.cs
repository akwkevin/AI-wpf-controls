using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AIStudio.Wpf.Controls.Demo.Models
{
    public class DemoDataModel : BaseTreeItemViewModel
    {
        public int Index { get; set; }

        public string Remark { get; set; }

        public string Field1
        {
            get; set;
        } = "Field1";

        public string Field2
        {
            get; set;
        } = "Field2";

        public string Field3
        {
            get; set;
        } = "Field3";

        public string Field4
        {
            get; set;
        } = "Field4";

        public string ImgPath { get; set; }
    }
}
