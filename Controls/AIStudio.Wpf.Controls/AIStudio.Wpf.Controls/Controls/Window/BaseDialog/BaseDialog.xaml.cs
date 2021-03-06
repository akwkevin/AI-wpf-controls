using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    public class BaseDialog : UserControl
    {
        public SizeChangedEventHandler SizeChangedHandler
        {
            get; set;
        }
        public CancellationTokenSource CancelSource
        {
            get; set;
        }
        public CancellationTokenSource OKSource
        {
            get; set;
        }

        public Func<bool> ValidationAction
        {
            get; set;
        }

        /// <summary>
        /// 取消
        /// </summary>
        protected Button _negativeButton = null;
        /// <summary>
        /// 确定
        /// </summary>
        protected Button _affirmativeButton = null;
        protected Button _otherButton = null;
        protected Button _otherButton2 = null;
        protected Button _otherButton3 = null;
        public bool AutoNavigation { get; set; } = true;

        public virtual Task<object> WaitForButtonPressAsync()
        {
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            if (_negativeButton == null)
                _negativeButton = this.FindName("PART_NegativeButton") as Button;
            if (_affirmativeButton == null)
                _affirmativeButton = this.FindName("PART_AffirmativeButton") as Button;
            if (_otherButton == null)
                _otherButton = this.FindName("PART_OtherButton") as Button;
            if (_otherButton2 == null)
                _otherButton2 = this.FindName("PART_OtherButton2") as Button;
            if (_otherButton3 == null)
                _otherButton3 = this.FindName("PART_OtherButton3") as Button;

            if (AutoNavigation)
            {
                if (_affirmativeButton != null)
                    ControlNavigationAttach.SetNavigationIndex(_affirmativeButton, 0);
                if (_negativeButton != null)
                    ControlNavigationAttach.SetNavigationIndex(_negativeButton, 1);

                ControlNavigationAttach.SetNavWithUpDown(this, true);
                ControlNavigationAttach.SetNavWithUpDownDefaultIndex(this, 0);
            }

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            RoutedEventHandler otherHandler = null;
            RoutedEventHandler otherHandler2 = null;
            RoutedEventHandler otherHandler3 = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = null;

            CancelSource = new CancellationTokenSource();

            var cancellationTokenRegistration = CancelSource.Token.Register(() => {
                cleanUpHandlers();
                tcs.TrySetResult(BaseDialogResult.Cancel);
            });

            OKSource = new CancellationTokenSource();

            var okTokenRegistration = OKSource.Token.Register(() => {
                cleanUpHandlers();
                tcs.TrySetResult(BaseDialogResult.OK);
            });

            cleanUpHandlers = () => {
                this.KeyDown -= escapeKeyHandler;

                if (_negativeButton != null)
                    _negativeButton.Click -= negativeHandler;
                if (_affirmativeButton != null)
                    _affirmativeButton.Click -= affirmativeHandler;
                if (_otherButton != null)
                    _otherButton.Click -= otherHandler;
                if (_otherButton2 != null)
                    _otherButton2.Click -= otherHandler2;
                if (_otherButton3 != null)
                    _otherButton3.Click -= otherHandler3;

                if (_negativeButton != null)
                    _negativeButton.KeyDown -= negativeKeyHandler;
                if (_affirmativeButton != null)
                    _affirmativeButton.KeyDown -= affirmativeKeyHandler;


                cancellationTokenRegistration.Dispose();
                okTokenRegistration.Dispose();
            };

            escapeKeyHandler = (sender, e) => {
                if (e.Key == Key.Escape)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(BaseDialogResult.Cancel);
                }
            };

            negativeKeyHandler = (sender, e) => {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(BaseDialogResult.Cancel);
                }
            };

            affirmativeKeyHandler = (sender, e) => {
                if (ValidationAction != null)
                {
                    if (ValidationAction() == false)
                        return;
                }

                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();
                    tcs.TrySetResult(BaseDialogResult.OK);
                }
            };

            negativeHandler = (sender, e) => {
                cleanUpHandlers();

                tcs.TrySetResult(BaseDialogResult.Cancel);

                e.Handled = true;
            };

            affirmativeHandler = (sender, e) => {
                if (ValidationAction != null)
                {
                    if (ValidationAction() == false)
                        return;
                }

                cleanUpHandlers();

                tcs.TrySetResult(BaseDialogResult.OK);

                e.Handled = true;
            };

            otherHandler = (sender, e) => {
                if (ValidationAction != null)
                {
                    if (ValidationAction() == false)
                        return;
                }

                cleanUpHandlers();

                tcs.TrySetResult(BaseDialogResult.Other1);

                e.Handled = true;
            };

            otherHandler2 = (sender, e) => {
                if (ValidationAction != null)
                {
                    if (ValidationAction() == false)
                        return;
                }

                cleanUpHandlers();

                tcs.TrySetResult(BaseDialogResult.Other2);

                e.Handled = true;
            };

            otherHandler3 = (sender, e) => {
                if (ValidationAction != null)
                {
                    if (ValidationAction() == false)
                        return;
                }

                cleanUpHandlers();

                tcs.TrySetResult(BaseDialogResult.Other3);

                e.Handled = true;
            };

            if (_negativeButton != null)
                _negativeButton.KeyDown += negativeKeyHandler;
            if (_affirmativeButton != null)
                _affirmativeButton.KeyDown += affirmativeKeyHandler;

            this.KeyDown += escapeKeyHandler;

            if (_negativeButton != null)
                _negativeButton.Click += negativeHandler;
            if (_affirmativeButton != null)
                _affirmativeButton.Click += affirmativeHandler;
            if (_otherButton != null)
                _otherButton.Click += otherHandler;
            if (_otherButton2 != null)
                _otherButton2.Click += otherHandler2;
            if (_otherButton3 != null)
                _otherButton3.Click += otherHandler3;

            return tcs.Task;
        }

        public virtual void WaitForButtonPress(Action<object> action)
        {
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            if (_negativeButton == null)
                _negativeButton = this.FindName("PART_NegativeButton") as Button;
            if (_affirmativeButton == null)
                _affirmativeButton = this.FindName("PART_AffirmativeButton") as Button;
            if (_otherButton == null)
                _otherButton = this.FindName("PART_OtherButton") as Button;
            if (_otherButton2 == null)
                _otherButton2 = this.FindName("PART_OtherButton2") as Button;
            if (_otherButton3 == null)
                _otherButton3 = this.FindName("PART_OtherButton3") as Button;

            if (AutoNavigation)
            {
                if (_affirmativeButton != null)
                    ControlNavigationAttach.SetNavigationIndex(_affirmativeButton, 0);
                if (_negativeButton != null)
                    ControlNavigationAttach.SetNavigationIndex(_negativeButton, 1);

                ControlNavigationAttach.SetNavWithUpDown(this, true);
                ControlNavigationAttach.SetNavWithUpDownDefaultIndex(this, 0);
            }

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            RoutedEventHandler otherHandler = null;
            RoutedEventHandler otherHandler2 = null;
            RoutedEventHandler otherHandler3 = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = null;

            CancelSource = new CancellationTokenSource();

            var cancellationTokenRegistration = CancelSource.Token.Register(() => {
                cleanUpHandlers();
                if (action != null)
                {
                    action(BaseDialogResult.Cancel);
                }                
            });

            OKSource = new CancellationTokenSource();

            var okTokenRegistration = OKSource.Token.Register(() => {
                cleanUpHandlers();
                if (action != null)
                {
                    action(BaseDialogResult.OK);
                }
            });

            cleanUpHandlers = () => {
                this.KeyDown -= escapeKeyHandler;

                if (_negativeButton != null)
                    _negativeButton.Click -= negativeHandler;
                if (_affirmativeButton != null)
                    _affirmativeButton.Click -= affirmativeHandler;
                if (_otherButton != null)
                    _otherButton.Click -= otherHandler;
                if (_otherButton2 != null)
                    _otherButton2.Click -= otherHandler2;
                if (_otherButton3 != null)
                    _otherButton3.Click -= otherHandler3;

                if (_negativeButton != null)
                    _negativeButton.KeyDown -= negativeKeyHandler;
                if (_affirmativeButton != null)
                    _affirmativeButton.KeyDown -= affirmativeKeyHandler;


                cancellationTokenRegistration.Dispose();
                okTokenRegistration.Dispose();
            };

            escapeKeyHandler = (sender, e) => {
                if (e.Key == Key.Escape)
                {
                    cleanUpHandlers();

                    if (action != null)
                    {
                        action(BaseDialogResult.Cancel);
                    }
                }
            };

            negativeKeyHandler = (sender, e) => {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    if (action != null)
                    {
                        action(BaseDialogResult.Cancel);
                    }
                }
            };

            affirmativeKeyHandler = (sender, e) => {
                if (ValidationAction != null)
                {
                    if (ValidationAction() == false)
                        return;
                }

                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    if (action != null)
                    {
                        action(BaseDialogResult.OK);
                    }
                }
            };

            negativeHandler = (sender, e) => {
                cleanUpHandlers();

                if (action != null)
                {
                    action(BaseDialogResult.Cancel);
                }

                e.Handled = true;
            };

            affirmativeHandler = (sender, e) => {
                if (ValidationAction != null)
                {
                    if (ValidationAction() == false)
                        return;
                }

                cleanUpHandlers();

                if (action != null)
                {
                    action(BaseDialogResult.OK);
                }

                e.Handled = true;
            };

            otherHandler = (sender, e) => {
                if (ValidationAction != null)
                {
                    if (ValidationAction() == false)
                        return;
                }

                cleanUpHandlers();

                if (action != null)
                {
                    action(BaseDialogResult.Other1);
                }

                e.Handled = true;
            };

            otherHandler2 = (sender, e) => {
                if (ValidationAction != null)
                {
                    if (ValidationAction() == false)
                        return;
                }

                cleanUpHandlers();

                if (action != null)
                {
                    action(BaseDialogResult.Other2);
                }

                e.Handled = true;
            };

            otherHandler3 = (sender, e) => {
                if (ValidationAction != null)
                {
                    if (ValidationAction() == false)
                        return;
                }

                cleanUpHandlers();

                if (action != null)
                {
                    action(BaseDialogResult.Other3);
                }

                e.Handled = true;
            };

            if (_negativeButton != null)
                _negativeButton.KeyDown += negativeKeyHandler;
            if (_affirmativeButton != null)
                _affirmativeButton.KeyDown += affirmativeKeyHandler;

            this.KeyDown += escapeKeyHandler;

            if (_negativeButton != null)
                _negativeButton.Click += negativeHandler;
            if (_affirmativeButton != null)
                _affirmativeButton.Click += affirmativeHandler;
            if (_otherButton != null)
                _otherButton.Click += otherHandler;
            if (_otherButton2 != null)
                _otherButton2.Click += otherHandler2;
            if (_otherButton3 != null)
                _otherButton3.Click += otherHandler3;
        }

        public virtual Task<object> WaitForButtonPressAsync2()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            WaitForButtonPress((object res) => {
                tcs.TrySetResult(res);
            });
            return tcs.Task;

        }
        static BaseDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseDialog), new FrameworkPropertyMetadata(typeof(BaseDialog)));
        }

        public BaseDialog()
        {
            this.Loaded += BaseDialog_Loaded;
        }

        #region 拖动使用
        private bool _move = false;
        private Point _lastPos;
        private double _widthRatio = 0;
        private double _heightRatio = 0;
        private bool _hastag;

        private void BaseDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Parent != null && this.Parent is FrameworkElement)
            {
                FrameworkElement parent = this.Parent as FrameworkElement;
                double left = (parent.ActualWidth - this.ActualWidth) / 2;
                double top = (parent.ActualHeight - this.ActualHeight) / 2;
                this.HorizontalAlignment = HorizontalAlignment.Left;
                this.VerticalAlignment = VerticalAlignment.Top;
                this.Margin = new Thickness(left, top, 0, 0);
                if (parent.ActualWidth == this.ActualWidth)
                {
                    _widthRatio = 1;
                    _heightRatio = 1;
                }
                else
                {
                    _widthRatio = this.Margin.Left / (parent.ActualWidth - this.ActualWidth);
                    _heightRatio = this.Margin.Top / (parent.ActualHeight - this.ActualHeight);
                }
                parent.SizeChanged += BaseDialog_SizeChanged;
            }

            this.MouseLeftButtonDown += BaseDialog_MouseLeftButtonDown;
        }



        private void BaseDialog_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double left = (e.NewSize.Width - this.ActualWidth) * _widthRatio;
            double top = (e.NewSize.Height - this.ActualHeight) * _heightRatio;
            this.Margin = new Thickness(left, top, 0, 0);
        }

        /// <summary>
        /// TitleTag,打上标记支持拖拽
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetTitleTag(DependencyObject obj)
        {
            return (bool)obj.GetValue(TitleTagProperty);
        }
        /// <summary>
        /// TitleTag,打上标记支持拖拽
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTitleTag(DependencyObject obj, bool value)
        {
            obj.SetValue(TitleTagProperty, value);
        }
        /// <summary>
        /// TitleTag,打上标记支持拖拽
        /// </summary>
        public static readonly DependencyProperty TitleTagProperty =
            DependencyProperty.RegisterAttached("TitleTag", typeof(bool), typeof(BaseDialog), new FrameworkPropertyMetadata(false, (d, f) => {
                var element = d as FrameworkElement;
                if (element != null)
                {
                    if (element is BaseDialog)
                    {
                        return;
                    }
                    element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
                    BaseDialog baseDialog = (element as BaseDialog) ?? element.TryFindParent<BaseDialog>();
                    if (baseDialog != null)
                    {
                        SetTitleTag(baseDialog, true);
                    }
                }
            }));

        private static void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            BaseDialog baseDialog = (element as BaseDialog) ?? element.TryFindParent<BaseDialog>();
            if (baseDialog != null)
            {
                baseDialog.TitleBar_MouseLeftButtonDown(sender, e);
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.Parent != null && this.Parent is FrameworkElement)
            {
                FrameworkElement parent = this.Parent as FrameworkElement;
                _move = true;
                _lastPos = e.GetPosition(parent);

                parent.PreviewMouseMove += (s, ee) => {
                    if (_move)
                    {
                        Point pos = ee.GetPosition(parent);
                        double left = this.Margin.Left + pos.X - this._lastPos.X;
                        double top = this.Margin.Top + pos.Y - this._lastPos.Y;
                        this.Margin = new Thickness(left, top, 0, 0);

                        _lastPos = e.GetPosition(parent);
                    }
                };

                parent.PreviewMouseUp += (s, ee) => {
                    if (_move)
                    {
                        _move = false;
                        double left = this.Margin.Left;
                        double top = this.Margin.Top;

                        if (left < 0)
                        {
                            left = 0;
                        }
                        if (top < 0)
                        {
                            top = 0;
                        }
                        if (left > (parent.ActualWidth - 50))
                        {
                            left = parent.ActualWidth - 50;
                        }
                        if (top > (parent.ActualHeight - 50))
                        {
                            top = parent.ActualHeight - 50;
                        }
                        this.Margin = new Thickness(left, top, 0, 0);

                        if (parent.ActualWidth == this.ActualWidth)
                        {
                            _widthRatio = 1;
                            _heightRatio = 1;
                        }
                        else
                        {
                            _widthRatio = this.Margin.Left / (parent.ActualWidth - this.ActualWidth);
                            _heightRatio = this.Margin.Top / (parent.ActualHeight - this.ActualHeight);
                        }
                    }
                };
            }
        }

        private void BaseDialog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GetTitleTag(this) == true)
            {
                return;
            }

            TitleBar_MouseLeftButtonDown(sender, e);
        }
        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }

    }

    public enum BaseDialogResult
    {
        None,
        Cancel,
        OK,
        Other1,
        Other2,
        Other3
    }

}
