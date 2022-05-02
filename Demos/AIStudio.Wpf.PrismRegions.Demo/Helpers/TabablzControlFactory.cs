using System;
using System.Windows;
using AIStudio.Wpf.PrismRegions.Demo.ViewModels;
using AIStudio.Wpf.PrismRegions.Demo.Views;
using Dragablz;

namespace AIStudio.Wpf.PrismRegions.Demo.Helpers
{
    public static class TabablzControlFactory
    {
        public static TabablzControlTabClient TabClientFactory
        {
            get
            {
                return new TabablzControlTabClient();
            }
        }

        public class TabablzControlTabClient : IInterTabClient
        {
            public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
            {
                var view = new TabablzHostWindow();
                var model = new TabablzHostWindowViewModel();
                view.DataContext = model;
                return new NewTabHost<Window>(view, view.InitialTabablzControl);
            }

            public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
            {
                if (window is DragablzWindow)
                {
                    return TabEmptiedResponse.CloseWindowOrLayoutBranch;
                }
                else
                {
                    return TabEmptiedResponse.DoNothing;
                }
            }
        }
    }
}
