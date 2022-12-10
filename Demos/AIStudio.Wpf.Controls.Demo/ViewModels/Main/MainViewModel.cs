using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.Controls.Demo.Models;
using AIStudio.Wpf.Controls.Demo.Services;
using AIStudio.Wpf.Controls.Demo.Views;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel()
        {
            DemoInfoList = DataService.GetDemoInfo();
            ShowIntroduce();
        }

        private List<DemoInfoModel> _demoInfoList;
        /// <summary>
        ///     demo信息
        /// </summary>
        public List<DemoInfoModel> DemoInfoList
        {
            get => _demoInfoList;
            set => SetProperty(ref _demoInfoList, value);
        }

        private DemoInfoModel _selectedDemoInfo;
        /// <summary>
        ///     选中demo信息
        /// </summary>
        public DemoInfoModel SelectedDemoInfo
        {
            get => _selectedDemoInfo;
            set
            {
                if (SetProperty(ref _selectedDemoInfo, value))
                {
                    SwitchDemo(value);
                }
            }
        }

        private ObservableCollection<DemoViewModel> _demoViewModel = new ObservableCollection<DemoViewModel>();
        /// <summary>
        ///    选中demo实例
        /// </summary>
        public ObservableCollection<DemoViewModel> DemoViewModel
        {
            get => _demoViewModel;
            set
            {
                SetProperty(ref _demoViewModel, value);
            }
        }

        private DemoViewModel _selectedDemoViewModel = new DemoViewModel();
        /// <summary>
        ///    选中demo实例
        /// </summary>
        public DemoViewModel SelectedDemoViewModel
        {
            get => _selectedDemoViewModel;
            set
            {
                SetProperty(ref _selectedDemoViewModel, value);
            }
        }


        /// <summary>
        /// 切换动画
        /// </summary>
        private TransitionSwitchType _transitionSwitchType;
        public TransitionSwitchType TransitionSwitchType
        {
            get => _transitionSwitchType;
            set => SetProperty(ref _transitionSwitchType, value);
        }

        public ICommand ShowCommand => new Lazy<DelegateCommand<string>>(() =>
               new DelegateCommand<string>(para => {
                   switch (para)
                   {
                       case "Window":
                           ShowWindow();
                           break;
                       case "Flyout":
                           ShowFlyout();
                           break;
                       case "Snackbar":
                           ShowSnackbar();
                           break;
                       case "ChildWindow":
                           ShowChildWindow();
                           break;
                       case "Dialog":
                           ShowDialog();
                           break;
                       case "WaitingBox":
                           ShowWaitingBox(false);
                           break;
                       case "WaitingBoxWithCancel":
                           ShowWaitingBox(true);
                           break;
                       case "Notice":
                           ShowNotice();
                           break;
                       case "MessageBox":
                           ShowMessageBox();
                           break;
                       case "Popup":
                           ShowPopup();
                           break;
                       case "Theme":
                           ShowTheme();
                           break;
                   }
               })).Value;


        private void SwitchDemo(DemoInfoModel item, string mode = "Tab")
        {
            if (item.Level == 0)
                return;

            var old = DemoViewModel.FirstOrDefault(p => p.TargetCtlName == item.TargetCtlName);
            if (old != null)
            {
                SelectedDemoViewModel = old;
                return;
            }
            DemoViewModel demoViewModel = new DemoViewModel();
            demoViewModel.Title = item.Label;
            demoViewModel.Icon = item.Glyph;
            demoViewModel.TargetCtlName = item.TargetCtlName;

            var type = Type.GetType($"{"AIStudio.Wpf.Controls.Demo.Views"}.{item.TargetCtlName}");
            var obj = type == null ? null : Activator.CreateInstance(type);

            if (obj is ContentControl content)
            {
                try
                {
                    var viewmodeltype = Type.GetType($"{"AIStudio.Wpf.Controls.Demo.ViewModels"}.{item.TargetCtlName}Model");
                    var viewmodel = viewmodeltype == null ? null : Activator.CreateInstance(viewmodeltype);
                    if (viewmodel != null)
                    {
                        content.DataContext = viewmodel;
                    }

                    var xamlPath = $"Views/{obj.GetType().Name}.xaml";
                    var dc = content.DataContext;
                    var dcTypeName = dc.GetType().Name;
                    var vmPath = $"ViewModels/{dcTypeName}";

                    demoViewModel.XamlText = DataService.GetCodeByFile(xamlPath);
                    demoViewModel.CsText = DataService.GetCodeByFile($"{xamlPath}.cs");
                    demoViewModel.VmText = DataService.GetCodeByFile($"{vmPath}.cs");

                    if (obj is AIStudioUserControl userControl)
                    {
                        userControl.Code = demoViewModel.XamlText;
                    }
                }
                catch { }
            }
            else
            {
                obj = new FriendlyTipView();
            }

            Random ra = new Random();
            TransitionSwitchType = (TransitionSwitchType)ra.Next((int)TransitionSwitchType.VerticalWipeTransition);

            if (demoViewModel.SubContent is IDisposable disposable)
            {
                disposable.Dispose();
            }
            demoViewModel.SubContent = obj;

            switch (mode)
            {
                case "Tab":
                    DemoViewModel.Add(demoViewModel);
                    SelectedDemoViewModel = demoViewModel;
                    break;
                case "Flyout":
                    WindowBase.ShowFlyout(obj as Flyout, "RootWindow");
                    break;

            }
        }


        private void ShowWindow()
        {
            var window = new WindowTest();
            window.Show();
        }

        public void ShowFlyout()
        {
            var flyout = new DynamicFlyout
            {
                Header = "Dynamic flyout"
            };

            // when the flyout is closed, remove it from the hosting FlyoutsControl
            WindowBase.ShowFlyout(flyout, "RootWindow");
        }

        int snackbar = 0;
        private void ShowSnackbar()
        {
            WindowBase.ShowMessageQueue("Show Snackbar" + snackbar, "RootWindow", "Button", () => {
                System.Diagnostics.Debug.WriteLine("Show Snackbar" + snackbar);
            }, (ControlStatus)snackbar);

            if (snackbar < (int)ControlStatus.Plain)
            {
                snackbar++;
            }
            else
            {
                snackbar = 0;
            }
        }

        int childwindow = 0;
        private async void ShowChildWindow()
        {
            if (childwindow == (int)ControlStatus.Plain)
            {
                var dialogtest = new ChildWindowTest();
                var res = await WindowBase.ShowChildWindowAsync(dialogtest, "ChildWindow", "RootWindow");
            }
            else
            {
                var res = await WindowBase.ShowChildMessageBoxAsync("Diaglog", "MesssageBox", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, "RootWindow", true, (ControlStatus)childwindow);
            }

            if (childwindow < (int)ControlStatus.Plain)
            {
                childwindow++;
            }
            else
            {
                childwindow = 0;
            }
        }


        int dialog = 0;
        private async void ShowDialog()
        {
            if (dialog == (int)ControlStatus.Plain)
            {
                var dialogtest = new DialogTest();
                var res = await WindowBase.ShowDialogAsync2(dialogtest, "RootWindow");
            }
            else
            {
                await MessageBoxDialog.Show("Diaglog", "Title", (ControlStatus)dialog, "RootWindow");
            }

            if (dialog < (int)ControlStatus.Plain)
            {
                dialog++;
            }
            else
            {
                dialog = 0;
            }
        }

        int waitingStyle = 0;
        private void ShowWaitingBox(bool canCancel)
        {
            if (waitingStyle > (int)WaitingStyle.Ring)
            {
                WaitingBox.Show(() => {
                    System.Threading.Thread.Sleep(1000);
                });
            }
            else
            {
                BusyBoxViewModel busyBoxViewModel = new BusyBoxViewModel();
                WindowBase.ShowWaiting((WaitingStyle)waitingStyle, "RootWindow", () => {
                    bool stop = false;
                    busyBoxViewModel.CancelAction = () => {
                        stop = true;
                    };
                    for (int i = 0; i < 10; i++)
                    {
                        System.Threading.Thread.Sleep(300);
                        busyBoxViewModel.Percent += 1d / 10d * 100;
                        if (stop)
                        {
                            break;
                        }
                    }
                },
                busyBoxViewModel, canCancel: canCancel);
            }

            if (waitingStyle <= (int)WaitingStyle.Ring)
            {
                waitingStyle++;
            }
            else
            {
                waitingStyle = 0;
            }
        }

        int k = 0;
        public void ShowNotice()
        {
            Notice.Show("This is a notice.", "Notice", 3, (ControlStatus)k, actionClose: (result) => {
                if (result == MessageBoxResult.OK)
                {
                    WindowBase.ShowMessageQueue("你点击了确认！！！", "RootWindow");
                }
            });

            if (k < (int)ControlStatus.Plain)
            {
                k++;
            }
            else
            {
                k = 0;
            }
        }

        public void ShowMessageBox()
        {
            MessageBox.Show("GrowlAsk", "Title", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        public void ShowPopup()
        {
            PopupWindow.ShowDialog("Popup", popupAnimation: CustomizePopupAnimation.Pop);
        }

        private void ShowTheme()
        {
            DemoInfoModel demoInfoModel = new DemoInfoModel();
            demoInfoModel.Level = 1;
            demoInfoModel.Label = "Theme 主题";
            demoInfoModel.Glyph = "Skin";
            demoInfoModel.TargetCtlName = nameof(PaletteSelectorView);
            SwitchDemo(demoInfoModel, "Flyout");
        }

        private void ShowCustomTheme()
        {
            DemoInfoModel demoInfoModel = new DemoInfoModel();
            demoInfoModel.Level = 1;
            demoInfoModel.Label = "Theme 自定义主题";
            demoInfoModel.Glyph = "PalettePath";
            demoInfoModel.TargetCtlName = nameof(ColorToolView);
            SwitchDemo(demoInfoModel);
        }

        private void ShowIntroduce()
        {
            DemoInfoModel demoInfoModel = new DemoInfoModel();
            demoInfoModel.Level = 1;
            demoInfoModel.Label = "Introduce 介绍";
            demoInfoModel.Glyph = "Introduce";
            demoInfoModel.TargetCtlName = nameof(IntroduceView);
            SwitchDemo(demoInfoModel);
        }
    }
}
