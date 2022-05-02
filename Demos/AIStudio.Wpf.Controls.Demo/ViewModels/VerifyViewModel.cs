using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Commands;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class VerifyViewModel : BindableBase
    {

        public VerifyViewModel()
        {

        }

        private ICommand _resultChangedComamnd;
        public ICommand ResultChangedComamnd
        {
            get
            {
                return this._resultChangedComamnd ?? (this._resultChangedComamnd = new DelegateCommand<object>(obj => this.ResultChanged(obj)));
            }
        }

        private void ResultChanged(object para)
        {

        }

    }
}
