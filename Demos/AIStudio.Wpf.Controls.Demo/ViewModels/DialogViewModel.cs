using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.Controls.Demo.Views;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class DialogViewModel
    {
        private ICommand _showCommand;
        public ICommand ShowCommand
        {
            get
            {
                return this._showCommand ?? (this._showCommand = new DelegateCommand<ControlStatus>(para => this.ShowBox(para)));
            }
        }

        public async void ShowBox(ControlStatus para)
        {
            var dialog = new DialogTest() { CanDragMove = false };
            dialog.SetValue(ControlAttach.StatusProperty, para);
            var res = await WindowBase.ShowDialogAsync2(dialog, "RootWindow");
        }
    }
}
