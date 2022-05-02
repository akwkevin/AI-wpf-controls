using System;
using System.Threading.Tasks;
using System.Windows;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// 简单等待框
    /// </summary>
    public class WaitingBox : System.Windows.Window
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(WaitingBox), new PropertyMetadata(default(string)));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private Action _Callback;

        static WaitingBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaitingBox), new FrameworkPropertyMetadata(typeof(WaitingBox)));
        }

        public WaitingBox(Action callback, Window owner, string text)
        {
            AllowsTransparency = true;
            WindowStyle = WindowStyle.None;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;
            SizeToContent = SizeToContent.WidthAndHeight;
            this.Owner = owner ?? Application.Current.MainWindow;

            this.Text = text;
            this._Callback = callback;
            this.Loaded += WaitingBox_Loaded;
        }

        void WaitingBox_Loaded(object sender, RoutedEventArgs e)
        {
            var workTask = Task.Run(() => this._Callback.Invoke());
            workTask.ContinueWith(t => {
                this.Dispatcher.Invoke(new Action(() => {
                    this.Close();
                }));
            });
        }

        /// <summary>
        /// 显示等待框，callback为需要执行的方法体（需要自己做异常处理）。
        /// 目前等等框为模式窗体
        /// </summary>
        public static void Show(Action callback, Window owner = null, string message = "有一种幸福，叫做等待...")
        {
            WaitingBox win = new WaitingBox(callback, owner, message);

            win.ShowDialog();
        }
    }
}
