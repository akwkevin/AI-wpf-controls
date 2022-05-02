using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Dragablz;

namespace AIStudio.Wpf.DragablzControls.Demo.ViewModels
{
    public class HeaderedItemSample1ViewModel : HeaderedItemWindowViewModel
    { 
        public HeaderedItemSample1ViewModel()
        {
            Items = new System.Collections.ObjectModel.ObservableCollection<Dragablz.HeaderedItemViewModel> {
               new HeaderedItemViewModel
               {
                   Header = "First",
                   Content = "There is First TabItem"
               },
               new HeaderedItemViewModel
               {
                   Header = "Second",
                   Content ="There is Second TabItem"
               },
               new HeaderedItemViewModel 
               { 
                   Header = "Third", 
                   Content = "There is Third TabItem" 
               },
           };
        }      
    }
}
