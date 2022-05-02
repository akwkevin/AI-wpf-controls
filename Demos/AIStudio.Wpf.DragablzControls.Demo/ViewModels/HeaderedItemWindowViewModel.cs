using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Dragablz;
using Dragablz.Dockablz;
using static AIStudio.Wpf.DragablzControls.Demo.Models.HeaderedItemViewModelFactory;

namespace AIStudio.Wpf.DragablzControls.Demo.ViewModels
{
    public class HeaderedItemWindowViewModel : ViewModelBase
    {
        private readonly IInterTabClient _interTabClient = new HeaderedItemInterTabClient();
     
        private readonly ObservableCollection<Dragablz.HeaderedItemViewModel> _toolItems = new ObservableCollection<Dragablz.HeaderedItemViewModel>();
      

        public HeaderedItemWindowViewModel()
        {
            Items = new ObservableCollection<Dragablz.HeaderedItemViewModel>();
        }

        public HeaderedItemWindowViewModel(params Dragablz.HeaderedItemViewModel[] items)
        {
            Items = new ObservableCollection<Dragablz.HeaderedItemViewModel>(items);
        }

        private ObservableCollection<Dragablz.HeaderedItemViewModel> items;

        public ObservableCollection<Dragablz.HeaderedItemViewModel> Items
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

        public static Guid TabPartition
        {
            get
            {
                return new Guid("2AE89D18-F236-4D20-9605-6C03319038E6");
            }
        }

        public IInterTabClient InterTabClient
        {
            get
            {
                return _interTabClient;
            }
        }

        public ObservableCollection<Dragablz.HeaderedItemViewModel> ToolItems
        {
            get
            {
                return _toolItems;
            }
        }

        public ItemActionCallback ClosingTabItemHandler
        {
            get
            {
                return ClosingTabItemHandlerImpl;
            }
        }

        /// <summary>
        /// Callback to handle tab closing.
        /// </summary>        
        private static void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        {
            //in here you can dispose stuff or cancel the close

            //here's your view model:
            var viewModel = args.DragablzItem.DataContext as Dragablz.HeaderedItemViewModel;
            Debug.Assert(viewModel != null);

            //here's how you can cancel stuff:
            //args.Cancel(); 
        }

        public ClosingFloatingItemCallback ClosingFloatingItemHandler
        {
            get
            {
                return ClosingFloatingItemHandlerImpl;
            }
        }

        /// <summary>
        /// Callback to handle floating toolbar/MDI window closing.
        /// </summary>        
        private static void ClosingFloatingItemHandlerImpl(ItemActionCallbackArgs<Layout> args)
        {
            //in here you can dispose stuff or cancel the close

            //here's your view model: 
            var disposable = args.DragablzItem.DataContext as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            //here's how you can cancel stuff:
            //args.Cancel(); 
        }
    }
}
