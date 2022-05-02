using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace AIStudio.Wpf.Controls.Behaviors
{
    public class StylizedBehaviors
    {
        public static readonly DependencyProperty BehaviorsProperty
            = DependencyProperty.RegisterAttached(
                "Behaviors",
                typeof(StylizedBehaviorCollection),
                typeof(StylizedBehaviors),
                new FrameworkPropertyMetadata(null, OnPropertyChanged));

        public static StylizedBehaviorCollection GetBehaviors(DependencyObject uie)
        {
            return (StylizedBehaviorCollection)uie.GetValue(BehaviorsProperty);
        }

        public static void SetBehaviors(DependencyObject uie, StylizedBehaviorCollection value)
        {
            uie.SetValue(BehaviorsProperty, value);
        }

        private static void OnPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            if (dpo is FrameworkElement frameworkElement)
            {
                var newBehaviors = e.NewValue as StylizedBehaviorCollection;
                var oldBehaviors = e.OldValue as StylizedBehaviorCollection;
                if (newBehaviors == oldBehaviors)
                {
                    return;
                }

                var itemBehaviors = Interaction.GetBehaviors(frameworkElement);

                frameworkElement.Unloaded -= FrameworkElementUnloaded;

                if (oldBehaviors != null)
                {
                    foreach (var behavior in oldBehaviors)
                    {
                        var index = GetIndexOf(itemBehaviors, behavior);
                        if (index >= 0)
                        {
                            itemBehaviors.RemoveAt(index);
                        }
                    }
                }

                if (newBehaviors != null)
                {
                    foreach (var behavior in newBehaviors)
                    {
                        var index = GetIndexOf(itemBehaviors, behavior);
                        if (index < 0)
                        {
                            var clone = (Behavior)behavior.Clone();
                            SetOriginalBehavior(clone, behavior);
                            itemBehaviors.Add(clone);
                        }
                    }
                }

                if (itemBehaviors.Count > 0)
                {
                    frameworkElement.Unloaded += FrameworkElementUnloaded;
                }
            }
        }

        private static void FrameworkElementUnloaded(object sender, RoutedEventArgs e)
        {
            // BehaviorCollection doesn't call Detach, so we do this
            if (sender is FrameworkElement frameworkElement)
            {
                var itemBehaviors = Interaction.GetBehaviors(frameworkElement);
                foreach (var behavior in itemBehaviors)
                {
                    behavior.Detach();
                }

                frameworkElement.Loaded += FrameworkElementLoaded;
            }
        }

        private static void FrameworkElementLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement frameworkElement)
            {
                frameworkElement.Loaded -= FrameworkElementLoaded;
                var itemBehaviors = Interaction.GetBehaviors(frameworkElement);
                foreach (var behavior in itemBehaviors)
                {
                    behavior.Attach(frameworkElement);
                }
            }
        }

        private static int GetIndexOf(BehaviorCollection itemBehaviors, Behavior behavior)
        {
            var index = -1;

            var originalBehavior = GetOriginalBehavior(behavior);

            for (var i = 0; i < itemBehaviors.Count; i++)
            {
                var currentBehavior = itemBehaviors[i];
                if (currentBehavior == behavior || currentBehavior == originalBehavior)
                {
                    index = i;
                    break;
                }

                var currentOriginalBehavior = GetOriginalBehavior(currentBehavior);
                if (currentOriginalBehavior == behavior || currentOriginalBehavior == originalBehavior)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private static readonly DependencyProperty OriginalBehaviorProperty
            = DependencyProperty.RegisterAttached(
                "OriginalBehavior",
                typeof(Behavior),
                typeof(StylizedBehaviors),
                new UIPropertyMetadata(null));

        private static Behavior GetOriginalBehavior(DependencyObject obj)
        {
            return (Behavior)obj.GetValue(OriginalBehaviorProperty);
        }

        private static void SetOriginalBehavior(DependencyObject obj, Behavior value)
        {
            obj.SetValue(OriginalBehaviorProperty, value);
        }
    }
}
