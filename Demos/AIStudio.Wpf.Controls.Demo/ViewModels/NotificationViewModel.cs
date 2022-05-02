using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.Controls.Demo.Services;
using AIStudio.Wpf.Controls.Demo.Views;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class NotificationViewModel : BindableBase
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
            Notification.Show(new NotificationContent(), ShowAnimation, StaysOpen, para);
        }

        private ShowAnimation _showAnimation;

        public ShowAnimation ShowAnimation
        {
            get
            {
                return _showAnimation;
            }
            set
            {
                if (_showAnimation != value)
                {
                    _showAnimation = value;
                    RaisePropertyChanged("ShowAnimation");
                }
            }
        }

        private bool _staysOpen = true;

        public bool StaysOpen
        {
            get
            {
                return _staysOpen;
            }
            set
            {
                if (_staysOpen != value)
                {
                    _staysOpen = value;
                    RaisePropertyChanged("StaysOpen");
                }
            }
        }
    }
}
