using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using AIStudio.Wpf.PrismDragablzExtensions;
using Prism.Mvvm;

namespace AIStudio.Wpf.PrismRegions.Demo.ViewModels
{
    public class TabablzHostWindowViewModel: BindableBase
    {
        public TabablzHostWindowViewModel()
        {
            Items = new ObservableCollection<TabablzProxy>();
        }

        private ObservableCollection<TabablzProxy> items;

        public ObservableCollection<TabablzProxy> Items
        {
            get
            {
                return this.items;
            }
            set
            {
                this.items = value;
                this.RaisePropertyChanged("Items");
            }
        }
    }
}
