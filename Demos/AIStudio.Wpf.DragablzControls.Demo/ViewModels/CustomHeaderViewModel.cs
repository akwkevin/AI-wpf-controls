using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AIStudio.Wpf.DragablzControls.Demo.ViewModels
{
    public class CustomHeaderViewModel : ViewModelBase
    {
        private string _header;
        private bool _isSelected;

        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                if (value == _header) return;
                _header = value;
#if NET40
                OnPropertyChanged("Header");
#else
                OnPropertyChanged();
#endif                
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value.Equals(_isSelected)) return;
                _isSelected = value;
#if NET40
                OnPropertyChanged("IsSelected");
#else
                OnPropertyChanged();
#endif                
            }
        }
    }
}
