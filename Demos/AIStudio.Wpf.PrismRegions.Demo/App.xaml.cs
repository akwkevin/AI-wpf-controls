using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AIStudio.Wpf.PrismAvalonExtensions.Regions;
using AIStudio.Wpf.PrismDragablzExtensions.Regions;
using AIStudio.Wpf.PrismRegions.Demo.ViewModels;
using Dragablz;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Xceed.Wpf.AvalonDock;

namespace AIStudio.Wpf.PrismRegions.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;
        }
       
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //记录严重错误 
            Console.WriteLine(e.Exception);
            e.Handled = true;//使用这一行代码告诉运行时，该异常被处理了，不再作为UnhandledException抛出了。            
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //记录严重错误                  
            Console.WriteLine(e.ExceptionObject);
        }

        protected override Window CreateShell()
        {
            var window = Container.Resolve<MainWindow>();
            return window;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            var mainWindowModule = typeof(MainWindowModule);
            moduleCatalog.AddModule(new ModuleInfo()
            {
                ModuleName = mainWindowModule.Name,
                ModuleType = mainWindowModule.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();

        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            regionAdapterMappings.RegisterMapping(typeof(TabablzControl), Container.Resolve<TabablzControlRegionAdapter>());
            regionAdapterMappings.RegisterMapping(typeof(DockingManager), Container.Resolve<DockingManagerRegionAdapter>());
        }
    }
}
