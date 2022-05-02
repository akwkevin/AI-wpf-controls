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

        public string ImgPath { get; set; }
    }
}
