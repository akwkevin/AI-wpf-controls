using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AIStudio.Wpf.Controls
{
    public interface IMetroThumb : IInputElement
    {
        event DragStartedEventHandler DragStarted;

        event DragDeltaEventHandler DragDelta;

        event DragCompletedEventHandler DragCompleted;

        event MouseButtonEventHandler MouseDoubleClick;
    }
}