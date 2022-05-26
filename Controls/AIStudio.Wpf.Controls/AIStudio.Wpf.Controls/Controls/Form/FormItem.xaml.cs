using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AIStudio.Wpf.Controls
{
    public class FormItem : HeaderedContentControl
    {
        static FormItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormItem), new FrameworkPropertyMetadata(typeof(FormItem), FrameworkPropertyMetadataOptions.Inherits));
        }

        public FormItem()
        {
        }
     
    }
}
