using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AIStudio.Wpf.DragablzControls.Demo.Views;
using Dragablz;

namespace AIStudio.Wpf.DragablzControls.Demo.ViewModels
{
    public class MainViewModel : HeaderedItemWindowViewModel
    {
        public MainViewModel()
        {
            Items = new System.Collections.ObjectModel.ObservableCollection<Dragablz.HeaderedItemViewModel> {
               new HeaderedItemViewModel
               {
                   Header = "First",
                   Content = "There is First TabItem"
               },
               new HeaderedItemViewModel
               {
                   Header = new CustomHeaderViewModel { Header = "Second" },
                   Content ="There is Second TabItem"
               },
               new HeaderedItemViewModel { Header = "Third", Content = "There is Third TabItem" },
               new HeaderedItemViewModel { Header = "Sample1", Content = new HeaderedItemSample1() },
               new HeaderedItemViewModel { Header = "Sample2", Content = new HeaderedItemSample2() }
           };

            //ToolItems.Add(
            //    new HeaderedItemViewModel { Header = "January", Content = "Welcome to the January tool/float item." });
            //ToolItems.Add(
            //    new HeaderedItemViewModel
            //    {
            //        Header = "July",
            //        Content =
            //            new Border
            //            {
            //                Background = Brushes.Yellow,
            //                Child = new TextBlock { Text = "Welcome to the July tool/float item." },
            //                HorizontalAlignment = HorizontalAlignment.Stretch
            //            }
            //    });
        }
    }
}
