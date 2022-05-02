using System;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Commands;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class DropDownViewModel
    {
        public ICommand ShowSnackbarCommand => new Lazy<DelegateCommand>(() =>
                new DelegateCommand(ShowSnackbar)).Value;

        private void ShowSnackbar()
        {
            WindowBase.ShowMessageQueue("Click DropDown", "RootWindow");
        }
    }
}
