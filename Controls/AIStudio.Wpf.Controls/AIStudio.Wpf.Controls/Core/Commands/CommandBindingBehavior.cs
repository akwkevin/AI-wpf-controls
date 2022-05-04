using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace AIStudio.Wpf.Controls.Commands
{
    public static class CommandBindingBehavior
    {
        public static readonly DependencyProperty BindingEventNameProperty = DependencyProperty.RegisterAttached
        ("BindingEventName", typeof(string),
        typeof(CommandBindingBehavior), new PropertyMetadata(null, OnBindingEventChanged));

        public static string GetBindingEventName(DependencyObject obj)
        {
            return (string)obj.GetValue(BindingEventNameProperty);
        }

        public static void SetBindingEventName(DependencyObject obj, string value)
        {
            obj.SetValue(BindingEventNameProperty, value);
        }

        public static readonly DependencyProperty BindingCommandProperty = DependencyProperty.RegisterAttached
        ("BindingCommand", typeof(ICommand), typeof(CommandBindingBehavior), new PropertyMetadata(null));

        public static ICommand GetBindingCommand(DependencyObject obj)
        {
            object com = obj.GetValue(BindingCommandProperty);
            if (com == null)
                return null;

            return (ICommand)com;
        }

        public static void SetBindingCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(BindingCommandProperty, value);
        }

        private static void OnEventRaised<T>(object sender, T arg) where T : EventArgs
        {
            DependencyObject dependencyObject = sender as DependencyObject;
            if (dependencyObject != null)
            {
                ICommand command = GetBindingCommand(dependencyObject);
                if (command != null)
                {
                    if (command.CanExecute(arg))
                    {
                        command.Execute(arg);
                    }
                }
            }
        }

        private static void OnBindingEventChanged(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            Type senderType = sender.GetType();
            EventInfo eventInfo = senderType.GetEvent(arg.NewValue.ToString());
            eventInfo.AddEventHandler(sender, GenerateDelegateForEventHandler(eventInfo));
        }

        private static Delegate GenerateDelegateForEventHandler(EventInfo eventInfo)
        {
            Delegate result = null;

            MethodInfo methodInfo = eventInfo.EventHandlerType.GetMethod("Invoke");
            ParameterInfo[] parameters = methodInfo.GetParameters();
            if (parameters.Length == 2)
            {
                Type currentType = typeof(CommandBindingBehavior);
                Type argType = parameters[1].ParameterType;
                MethodInfo eventRaisedMethod =
                currentType.GetMethod("OnEventRaised", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(argType);
                result = Delegate.CreateDelegate(eventInfo.EventHandlerType, eventRaisedMethod);
            }

            return result;
        }

    }
}
