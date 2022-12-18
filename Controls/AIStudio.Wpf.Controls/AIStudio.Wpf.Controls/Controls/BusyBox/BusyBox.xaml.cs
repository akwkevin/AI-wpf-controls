using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// BusyBox.xaml 的交互逻辑
    /// </summary>
    public partial class BusyBox : UserControl
    {
        public WaitingStyle WaitingStyle
        {
            get => (WaitingStyle)GetValue(WaitingStyleProperty);
            set => SetValue(WaitingStyleProperty, value);
        }

        public static readonly DependencyProperty WaitingStyleProperty =
            DependencyProperty.Register(nameof(WaitingStyle), typeof(WaitingStyle), typeof(BusyBox), new PropertyMetadata(WaitingStyle.Busy));

        public static readonly DependencyProperty WaitInfoProperty = DependencyProperty.Register(
           nameof(WaitInfo), typeof(string), typeof(BusyBox), new PropertyMetadata("系统加载中，请耐心等待"));

        public string WaitInfo
        {
            get
            {
                return (string)GetValue(WaitInfoProperty);
            }
            set
            {
                SetValue(WaitInfoProperty, value);
            }
        }

        public static readonly DependencyProperty CanCancelProperty = DependencyProperty.Register(
          nameof(CanCancel), typeof(bool), typeof(BusyBox), new PropertyMetadata(false));

        public bool CanCancel
        {
            get
            {
                return (bool)GetValue(CanCancelProperty);
            }
            set
            {
                SetValue(CanCancelProperty, value);
            }
        }

        public static readonly DependencyProperty PercentProperty = DependencyProperty.Register(
         nameof(Percent), typeof(double?), typeof(BusyBox), new PropertyMetadata(default(double?)));

        public double? Percent
        {
            get
            {
                return (double?)GetValue(PercentProperty);
            }
            set
            {
                SetValue(PercentProperty, value);
            }
        }

        static BusyBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyBox), new FrameworkPropertyMetadata(typeof(BusyBox)));
        }

        public BusyBox()
        {
            this.CancelCommmad = new RoutedUICommand();
            this.BindCommand(this.CancelCommmad, this.CancelExcute);
        }

        public ICommand CancelCommmad
        {
            get; protected set;
        }

        public Action CancelAction;
        private void CancelExcute(object sender, ExecutedRoutedEventArgs e)
        {
            if (CancelAction != null)
            {
                CancelAction();
            }
        }

    }
}
