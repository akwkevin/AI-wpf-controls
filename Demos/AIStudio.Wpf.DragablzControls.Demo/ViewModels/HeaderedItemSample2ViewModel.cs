using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AIStudio.Wpf.DragablzControls.Demo.ViewModels
{
    public class HeaderedItemSample2ViewModel : ViewModelBase
    { 
        public HeaderedItemSample2ViewModel(params HeaderedItemWindowViewModel[] items)
        {
            BranchItems = new ObservableCollection<HeaderedItemWindowViewModel>(items);
        }

        private ObservableCollection<HeaderedItemWindowViewModel> _branchItems;
        public ObservableCollection<HeaderedItemWindowViewModel> BranchItems
        {
            get
            {
                return this._branchItems;
            }
            set
            {
                this._branchItems = value;
                this.RaisePropertyChanged("BranchItems");
            }
        }
    }
}
