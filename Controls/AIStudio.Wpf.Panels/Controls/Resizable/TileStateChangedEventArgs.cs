using System.Windows;

namespace AIStudio.Wpf.Panels
{
    public sealed class TileStateChangedEventArgs : RoutedEventArgs
    {
        public TileState OldValue
        {
            get; private set;
        }
        public TileState NewValue
        {
            get; private set;
        }

        public TileStateChangedEventArgs(RoutedEvent routedEvent, TileState oldValue, TileState newValue)
           : base(routedEvent)
        {
            this.NewValue = newValue;
            this.OldValue = oldValue;
        }
    }
}
