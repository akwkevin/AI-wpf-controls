using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    public class StreamerLoading : Control
    {
        static StreamerLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StreamerLoading), new FrameworkPropertyMetadata(typeof(StreamerLoading)));
        }
    }
}
