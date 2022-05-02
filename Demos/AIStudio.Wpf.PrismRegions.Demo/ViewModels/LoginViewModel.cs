using System.Windows;
using System.Windows.Input;
using AIStudio.Wpf.PrismDragablzExtensions.ViewModels;
using AIStudio.Wpf.PrismRegions.Demo.Views;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;

namespace AIStudio.Wpf.PrismRegions.Demo.ViewModels
{
    public class LoginViewModel : NavigationTitleViewModel
    {
        IContainerExtension _container;
        IRegionManager _regionManager;

        public LoginViewModel(IContainerExtension container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
            Title = "Login";
        }

        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                SetProperty(ref _userName, value);
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                SetProperty(ref _password, value);

            }
        }

        private bool _isRmembered = true;
        public bool IsRmembered
        {
            get
            {
                return _isRmembered;
            }
            set
            {
                SetProperty(ref _isRmembered, value);
            }
        }

        private ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                return this._loginCommand ?? (this._loginCommand = new DelegateCommand(() => this.Login()));
            }
        }



        private void Login()
        {
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("登录成功！");

                _regionManager.RequestNavigate("MainContentRegion", nameof(IntroduceView));
                _regionManager.RequestNavigate("MainContentRegion2", nameof(IntroduceView));
                _regionManager.RequestNavigate("MainContentRegion3", nameof(IntroduceView));
            }
            else
            {
                MessageBox.Show("请输入用户名或密码！");
            }


        }

    }
}
