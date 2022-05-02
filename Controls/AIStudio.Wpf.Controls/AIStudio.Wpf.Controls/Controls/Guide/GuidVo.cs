using System.Windows;

namespace AIStudio.Wpf.Controls
{
    public class GuidVo
    {
        public FrameworkElement Uc
        {
            get; set;
        }
        public string Content
        {
            get; set;
        }
        public int? Width { get; set; } = 220;
        public int? Height
        {
            get; set;
        }
    }
}
