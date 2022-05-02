using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        Button PART_NegativeButton = null;
        /// <summary>
        /// 确定
        /// </summary>
        Button PART_AffirmativeButton = null;
        Button PART_OtherButton = null;
        Button PART_OtherButton2 = null;
        Button PART_OtherButton3 = null;
        public bool AutoNavigation { get; set; } = true;

        public virtual Task<object> WaitForButtonPressAsync()
        {
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            if (PART_NegativeButton == null)
                PART_NegativeButton = this.FindName("PART_NegativeButton") as Button;
            if (PART_AffirmativeButton == null)
                PART_AffirmativeButton = this.FindName("PART_AffirmativeButton") as Button;
            if (PART_OtherButton == null)
                PART_OtherButton = this.FindName("PART_OtherButton") as Button;
            if (PART_OtherButton2 == null)
                PART_OtherButton2 = this.FindName("PART_OtherButton2") as Button;
            if (PART_OtherButton3 == null)
                PART_OtherButton3 = this.FindName("PART_OtherButton3") as Button;

            if (AutoNavigation)
            {
                if (PART_AffirmativeButton != null)
                    ControlNavigationAttach.SetNavigationIndex(PART_AffirmativeButton, 0);
                if (PART_NegativeButton != null)
                    ControlNavigationAttach.SetNavigationIndex(PART_NegativeButton, 1);

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

                if (PART_NegativeButton != null)
                    PART_NegativeButton.Click -= negativeHandler;
                if (PART_AffirmativeButton != null)
                    PART_AffirmativeButton.Click -= affirmativeHandler;
                if (PART_OtherButton != null)
                    PART_OtherButton.Click -= otherHandler;
                if (PART_OtherButton2 != null)
                    PART_OtherButton2.Click -= otherHandler2;
                if (PART_OtherButton3 != null)
                    PART_OtherButton3.Click -= otherHandler3;

                if (PART_NegativeButton != null)
                    PART_NegativeButton.KeyDown -= negativeKeyHandler;
                if (PART_AffirmativeButton != null)
                    PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;


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

            if (PART_NegativeButton != null)
                PART_NegativeButton.KeyDown += negativeKeyHandler;
            if (PART_AffirmativeButton != null)
                PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;

            this.KeyDown += escapeKeyHandler;

            if (PART_NegativeButton != null)
                PART_NegativeButton.Click += negativeHandler;
            if (PART_AffirmativeButton != null)
                PART_AffirmativeButton.Click += affirmativeHandler;
            if (PART_OtherButton != null)
                PART_OtherButton.Click += otherHandler;
            if (PART_OtherButton2 != null)
                PART_OtherButton2.Click += otherHandler2;
            if (PART_OtherButton3 != null)
                PART_OtherButton3.Click += otherHandler3;

            return tcs.Task;
        }

        public BaseDialog()
        {
            this.Loaded += BaseDialog_Loaded;
        }

        #region 拖动使用
        private bool _move = false;
        private Point _lastPos;
        private double _widthRatio;
        private double _heightRatio;

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
                _widthRatio = this.Margin.Left / (parent.ActualWidth - this.ActualWidth);
                _heightRatio = this.Margin.Top / (parent.ActualHeight - this.ActualHeight);
                parent.SizeChanged += BaseDialog_SizeChanged;
            }
        }

        private void BaseDialog_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double left = (e.NewSize.Width - this.ActualWidth) * _widthRatio;
            double top = (e.NewSize.Height - this.ActualHeight) * _heightRatio;
            this.Margin = new Thickness(left, top, 0, 0);
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

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

                        _widthRatio = this.Margin.Left / (parent.ActualWidth - this.ActualWidth);
                        _heightRatio = this.Margin.Top / (parent.ActualHeight - this.ActualHeight);
                    }
                };
            }
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
