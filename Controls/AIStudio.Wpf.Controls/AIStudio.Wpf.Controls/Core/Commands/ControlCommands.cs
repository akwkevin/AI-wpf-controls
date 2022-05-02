using System;
using System.Windows;
using System.Windows.Input;

namespace AIStudio.Wpf.Controls.Commands
{
    public static class ControlCommands
    {
        /// <summary>
        ///     清除
        /// </summary>
        public static RoutedCommand Clear { get; } = new RoutedCommand(nameof(Clear), typeof(ControlCommands));

        /// <summary>
        ///     关闭
        /// </summary>
        public static RoutedCommand Close { get; } = new RoutedCommand(nameof(Close), typeof(ControlCommands));

        /// <summary>
        ///     取消
        /// </summary>
        public static RoutedCommand Cancel { get; } = new RoutedCommand(nameof(Cancel), typeof(ControlCommands));

        /// <summary>
        ///     确定
        /// </summary>
        public static RoutedCommand Confirm { get; } = new RoutedCommand(nameof(Confirm), typeof(ControlCommands));

        /// <summary>
        ///     是
        /// </summary>
        public static RoutedCommand Yes { get; } = new RoutedCommand(nameof(Yes), typeof(ControlCommands));

        /// <summary>
        ///     否
        /// </summary>
        public static RoutedCommand No { get; } = new RoutedCommand(nameof(No), typeof(ControlCommands));

        /// <summary>
        ///     关闭所有
        /// </summary>
        public static RoutedCommand CloseAll { get; } = new RoutedCommand(nameof(CloseAll), typeof(ControlCommands));

        /// <summary>
        ///     关闭其他
        /// </summary>
        public static RoutedCommand CloseOther { get; } = new RoutedCommand(nameof(CloseOther), typeof(ControlCommands));

        /// <summary>
        ///     确认
        /// </summary>
        public static RoutedCommand Sure { get; } = new RoutedCommand(nameof(Sure), typeof(ControlCommands));

        /// <summary>
        ///     按照类别排序
        /// </summary>
        public static RoutedCommand SortByCategory { get; } = new RoutedCommand(nameof(SortByCategory), typeof(ControlCommands));

        /// <summary>
        ///     按照名称排序
        /// </summary>
        public static RoutedCommand SortByName { get; } = new RoutedCommand(nameof(SortByName), typeof(ControlCommands));

        /// <summary>
        ///     关闭窗口
        /// </summary>
        public static CloseWindowCommand CloseWindow { get; } = new CloseWindowCommand();

        /// <summary>
        ///     打开
        /// </summary>
        public static RoutedCommand Open { get; } = new RoutedCommand(nameof(Open), typeof(ControlCommands));
    }

    public class CloseWindowCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (parameter is DependencyObject dependencyObject)
            {
                if (System.Windows.Window.GetWindow(dependencyObject) is System.Windows.Window window)
                {
                    window.Close();
                }
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
