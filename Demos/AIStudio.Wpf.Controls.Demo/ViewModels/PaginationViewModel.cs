using System.Windows;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Commands;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class PaginationViewModel
    {
        private ICommand _currentIndexChangedComamnd;
        public ICommand CurrentIndexChangedComamnd
        {
            get
            {
                return this._currentIndexChangedComamnd ?? (this._currentIndexChangedComamnd = new DelegateCommand<RoutedEventArgs>(para => this.CurrentIndexChanged(para)));
            }
        }

        public void CurrentIndexChanged(RoutedEventArgs para)
        {
            var args = para as CurrentIndexChangedEventArgs;
            if (args != null)
            {
               
            }
        }
    }
}
