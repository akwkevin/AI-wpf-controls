using System.Windows;

namespace AIStudio.Wpf.Controls.Event
{
    public class CancelRoutedEventArgs : RoutedEventArgs
    {
        public CancelRoutedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
        }

        public bool Cancel
        {
            get; set;
        }
    }
}
