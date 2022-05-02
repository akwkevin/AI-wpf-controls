using System;
using System.Windows;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.Controls.Event;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class DatePickerViewModel
    {
        private ICommand _selectedDateTimeChangedComamnd;
        public ICommand SelectedDateTimeChangedComamnd
        {
            get
            {
                return this._selectedDateTimeChangedComamnd ?? (this._selectedDateTimeChangedComamnd = new DelegateCommand<RoutedEventArgs>(para => this.SelectedDateTimeChanged(para)));
            }
        }

        public void SelectedDateTimeChanged(RoutedEventArgs para)
        {
            var args = para as FunctionEventArgs<DateTime?>;
            if (args != null)
            {
                WindowBase.ShowMessageQueue(args.Info == null ? "" : args.Info.ToString(), "RootWindow");
            }
        }
    }
}
