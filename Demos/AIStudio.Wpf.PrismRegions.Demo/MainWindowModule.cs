using System;
using System.Collections.Generic;
using System.Text;
using AIStudio.Wpf.PrismRegions.Demo.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AIStudio.Wpf.PrismRegions.Demo
{
    public class MainWindowModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("MainContentRegion", typeof(LoginView).Name);
            regionManager.RequestNavigate("MainContentRegion2", typeof(LoginView).Name);
            regionManager.RequestNavigate("MainContentRegion3", typeof(LoginView).Name);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<LoginView>();
            containerRegistry.RegisterForNavigation<IntroduceView>();
        }
    }
}
