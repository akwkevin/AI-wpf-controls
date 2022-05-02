using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Commands;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class MessageBoxViewModel
    {
        private ICommand _showCommand;
        public ICommand ShowCommand
        {
            get
            {
                return this._showCommand ?? (this._showCommand = new DelegateCommand<ControlStatus>(para => this.ShowBox(para)));
            }
        }

        public void ShowBox(ControlStatus para)
        {
            MessageBox.Show("GrowlAsk", "Title", MessageBoxButton.YesNo, MessageBoxImage.Question, controlStatus: para);
        }
    }
}
