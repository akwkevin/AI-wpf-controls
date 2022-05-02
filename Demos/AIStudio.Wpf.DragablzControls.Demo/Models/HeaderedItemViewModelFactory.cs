using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using AIStudio.Wpf.DragablzControls.Demo.ViewModels;
using AIStudio.Wpf.DragablzControls.Demo.Views;
using Dragablz;

namespace AIStudio.Wpf.DragablzControls.Demo.Models
{
    public static class HeaderedItemViewModelFactory
    {
        public static Func<HeaderedItemViewModel> Factory
        {
            get
            {
                return
                    () => {
                        var dateTime = DateTime.Now;

                        return new HeaderedItemViewModel()
                        {
                            Header = dateTime.ToLongTimeString(),
                            Content = dateTime.ToString("R")
                        };
                    };
            }
        }

        public static HeaderedItemInterTabClient TabClientFactory
        {
            get
            {
                return new HeaderedItemInterTabClient();
            }
        }

        public class HeaderedItemInterTabClient : IInterTabClient
        {
            public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
            {
                var view = new HeaderedItemWindow();
                var model = new HeaderedItemWindowViewModel();
                view.DataContext = model;
                return new NewTabHost<Window>(view, view.mainContent.InitialTabablzControl);
            }

            public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
            {
                return TabEmptiedResponse.CloseWindowOrLayoutBranch;
            }
        }
    }
}
