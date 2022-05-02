using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using AIStudio.Wpf.PrismRegions.Demo.Models;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;

namespace AIStudio.Wpf.PrismRegions.Demo.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        IContainerExtension _container;
        IRegionManager _regionManager;

        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;

            InitMenu();
        }

        private ICommand _menuCommand;
        public ICommand MenuCommand
        {
            get
            {
                return this._menuCommand ?? (this._menuCommand = new DelegateCommand<string>(para => this.Menu(para)));
            }
        }

        private ObservableCollection<AMenuItem> _menuItems = new ObservableCollection<AMenuItem>();
        public ObservableCollection<AMenuItem> MenuItems
        {
            get
            {
                return _menuItems;
            }
            set
            {
                SetProperty(ref _menuItems, value);
            }
        }


        private void Menu(string viewname)
        {
            _regionManager.RequestNavigate("MainContentRegion", viewname);
            _regionManager.RequestNavigate("MainContentRegion2", viewname);
            _regionManager.RequestNavigate("MainContentRegion3", viewname);
        }

        private void InitMenu()
        {
            AMenuItem menu = new AMenuItem() { Label = "菜单", Type = 0 };
            MenuItems.Add(menu);

            menu.AddChildren(new AMenuItem() { Label = "Login", Code = "LoginView", Type = 1, Command = new DelegateCommand<string>(para => this.Menu(para)) });
            menu.AddChildren(new AMenuItem() { Label = "Introduce", Code = "IntroduceView", Type = 1, Command = new DelegateCommand<string>(para => this.Menu(para)) });
        }
    }
}
