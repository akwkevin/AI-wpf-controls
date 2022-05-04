using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace AIStudio.Wpf.Controls
{
    public class AutoGeneratingColumnEventToCommandBehaviour : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            AssociatedObject.AutoGeneratingColumn += new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
            AutoGeneratingColumnEventToCommandBehaviour.SetCommand(AssociatedObject, AutoGeneratingColumnEventToCommandBehaviour.GetCommand(this));
        }

        protected override void OnDetaching()
        {
            AssociatedObject.AutoGeneratingColumn -= new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(AutoGeneratingColumnEventToCommandBehaviour),
                new PropertyMetadata(
                    null,
                    OnCommandChanged));

        public static void SetCommand(DependencyObject o, ICommand value)
        {
            o.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject o)
        {
            return o.GetValue(CommandProperty) as ICommand;
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = d as DataGrid;
            //if (dataGrid != null)
            //{
            //    if (e.OldValue != null)
            //    {
            //        dataGrid.AutoGeneratingColumn -= OnAutoGeneratingColumn;
            //    }
            //    if (e.NewValue != null)
            //    {
            //        dataGrid.AutoGeneratingColumn += OnAutoGeneratingColumn;
            //    }
            //}
        }

        private static void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var dependencyObject = sender as DependencyObject;
            if (dependencyObject != null)
            {
                var command = dependencyObject.GetValue(CommandProperty) as ICommand;
                if (command != null && command.CanExecute(e))
                {
                    command.Execute(e);
                }
            }
        }
    }
}