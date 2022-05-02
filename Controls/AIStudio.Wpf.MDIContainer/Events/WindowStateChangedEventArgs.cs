using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AIStudio.Wpf.MDIContainer.Events
{
    public sealed class WindowStateChangedEventArgs : RoutedEventArgs
    {
        public WindowState OldValue { get; private set; }
        public WindowState NewValue { get; private set; }

        public WindowStateChangedEventArgs(RoutedEvent routedEvent, WindowState oldValue, WindowState newValue)
           : base(routedEvent)
        {
            this.NewValue = newValue;
            this.OldValue = oldValue;
        }
    }
}
