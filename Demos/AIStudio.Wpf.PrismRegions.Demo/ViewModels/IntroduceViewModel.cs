using System.Windows;
using System.Windows.Input;
using AIStudio.Wpf.PrismDragablzExtensions.ViewModels;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;

namespace AIStudio.Wpf.PrismRegions.Demo.ViewModels
{
    class IntroduceViewModel : NavigationTitleViewModel
    {
        private string _noticeText = "欢迎来到AIStudio.Wpf.Client,让我们一起从0开始学Wpf框架搭建吧！";
        public string NoticeText
        {
            get { return _noticeText; }
            set
            {
                SetProperty(ref _noticeText, value);
            }
        }

        private ICommand _clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                return this._clickCommand ?? (this._clickCommand = new DelegateCommand(() => this.Click()));
            }
        }

        private void Click()
        {
            MessageBox.Show("HelloWorld, 您点击了一下Button按钮");
        }

        public IntroduceViewModel()
        {
            Title = "Introduce";
            MaxTabItemNumber = 5;
        }

        
    }
}
