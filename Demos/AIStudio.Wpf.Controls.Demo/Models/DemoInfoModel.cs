using System.Collections.Generic;
using AIStudio.Wpf.Controls.Bindings;

namespace AIStudio.Wpf.Controls.Demo.Models
{
    public class DemoInfoModel : HamburgerTreeMenuGlyphItem
    {
        public int Level { get; set; }
        public string Key { get; set; }
        public string TargetCtlName { get; set; }
        public bool IsNew { get; set; }

        public new IList<DemoInfoModel> Children { get; set; }
    }
}