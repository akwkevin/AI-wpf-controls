using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace AIStudio.Wpf.Controls.Behaviors
{
    public class StylizedBehaviorCollection : FreezableCollection<Behavior>
    {
        protected override Freezable CreateInstanceCore()
        {
            return new StylizedBehaviorCollection();
        }
    }
}
