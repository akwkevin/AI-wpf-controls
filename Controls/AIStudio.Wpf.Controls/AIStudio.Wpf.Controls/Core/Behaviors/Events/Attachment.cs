using System;
using System.Windows;
using System.Windows.Input;

namespace AIStudio.Wpf.Controls.Behaviors
{
    public class Attachment<TargetT, BehaviorT, AttachmentT>
         where TargetT : FrameworkElement
         where BehaviorT : CommandBehaviorBase<TargetT>
    {
        public static DependencyProperty Behavior()
        {
            return DependencyProperty.RegisterAttached(
                typeof(BehaviorT).Name,
                typeof(BehaviorT),
                typeof(TargetT),
                null);
        }

        public static DependencyProperty Command(DependencyProperty behaviorProperty)
        {
            return DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(AttachmentT),
                new PropertyMetadata((target, args) => OnSetCommandCallback(target, args, behaviorProperty)));
        }

        public static DependencyProperty Parameter(DependencyProperty behaviorProperty)
        {
            return DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(object),
                typeof(AttachmentT),
                new PropertyMetadata((target, args) => OnSetParameterCallback(target, args, behaviorProperty)));
        }

        protected static void OnSetCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e, DependencyProperty behaviorProperty)
        {
            var target = dependencyObject as TargetT;
            if (target == null)
                return;

            GetOrCreateBehavior(target, behaviorProperty).Command = e.NewValue as ICommand;
        }

        protected static void OnSetParameterCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e, DependencyProperty behaviorProperty)
        {
            var target = dependencyObject as TargetT;
            if (target != null)
            {
                GetOrCreateBehavior(target, behaviorProperty).CommandParameter = e.NewValue;
            }
        }

        protected static BehaviorT GetOrCreateBehavior(FrameworkElement control, DependencyProperty behaviorProperty)
        {
            var behavior = control.GetValue(behaviorProperty) as BehaviorT;
            if (behavior == null)
            {
                behavior = Activator.CreateInstance(typeof(BehaviorT), control) as BehaviorT;
                control.SetValue(behaviorProperty, behavior);
            }

            return behavior;
        }
    }
}
