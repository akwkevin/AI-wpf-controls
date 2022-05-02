using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    public class EllipsisBlock : Control
    {
        static EllipsisBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EllipsisBlock), new FrameworkPropertyMetadata(typeof(EllipsisBlock)));
        }
    }
}
