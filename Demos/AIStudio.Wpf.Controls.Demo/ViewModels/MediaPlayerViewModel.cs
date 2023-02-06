using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.Controls.MediaPlayer;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class MediaPlayerViewModel
    {
        private ICommand _showCommand;
        public ICommand ShowCommand
        {
            get
            {
                return this._showCommand ?? (this._showCommand = new DelegateCommand<ControlStatus>(para => this.ShowBox(para)));
            }
        }

        int k = 0;
        public void ShowBox(ControlStatus para)
        {
            MediaElementPlayerWindow.Show($"{System.AppDomain.CurrentDomain.BaseDirectory}/Resources/Media/intro.wmv", (ShowMode)k);

            if (k < (int)ShowMode.MusicInfoVideoFullMode)
            {
                k++;
            }
            else
            {
                k = 0;
            }
        }


    }
}
