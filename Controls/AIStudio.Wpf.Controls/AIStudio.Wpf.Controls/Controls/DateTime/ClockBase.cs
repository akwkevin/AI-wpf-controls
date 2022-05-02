using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AIStudio.Wpf.Controls.Event;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_ButtonConfirm, Type = typeof(Button))]
    [TemplatePart(Name = PART_ClearDate, Type = typeof(Button))]
    [TemplatePart(Name = PART_NowDate, Type = typeof(Button))]
    public abstract class ClockBase : Control
    {
        protected const string PART_ButtonConfirm = "PART_ButtonConfirm";
        private const string PART_ClearDate = "PART_ClearDate";
        private const string PART_NowDate = "PART_NowDate";

        protected Button ButtonConfirm;
        /// <summary>
        /// 按钮：清除所选日期
        /// </summary>
        protected Button _clearDate;
        /// <summary>
        /// 当前日期
        /// </summary>
        protected Button _nowDate;

        protected bool AppliedTemplate;

        public event Action Confirmed;

        public event EventHandler<FunctionEventArgs<DateTime>> DisplayTimeChanged;

        public static readonly RoutedEvent SelectedTimeChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedTimeChanged", RoutingStrategy.Direct,
                typeof(EventHandler<FunctionEventArgs<DateTime?>>), typeof(ClockBase));

        public event EventHandler<FunctionEventArgs<DateTime?>> SelectedTimeChanged
        {
            add => AddHandler(SelectedTimeChangedEvent, value);
            remove => RemoveHandler(SelectedTimeChangedEvent, value);
        }

        public static readonly DependencyProperty TimeFormatProperty = DependencyProperty.Register(
            "TimeFormat", typeof(string), typeof(ClockBase), new PropertyMetadata("HH:mm:ss"));

        public string TimeFormat
        {
            get => (string)GetValue(TimeFormatProperty);
            set => SetValue(TimeFormatProperty, value);
        }

        public static readonly DependencyProperty SelectedTimeProperty = DependencyProperty.Register(
            "SelectedTime", typeof(DateTime?), typeof(ClockBase), new PropertyMetadata(default(DateTime?), OnSelectedTimeChanged));

        private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (ClockBase)d;
            var v = (DateTime?)e.NewValue;
            ctl.OnSelectedTimeChanged(new FunctionEventArgs<DateTime?>(SelectedTimeChangedEvent, ctl)
            {
                Info = v
            });

            if (ctl.IsHandlerSuspended(SelectedTimeProperty)) return;
            ctl.DisplayTime = v ?? DateTime.Now;
        }

        public DateTime? SelectedTime
        {
            get => (DateTime?)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        public static readonly DependencyProperty DisplayTimeProperty = DependencyProperty.Register(
            "DisplayTime", typeof(DateTime), typeof(ClockBase),
            new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnDisplayTimeChanged));

        private static void OnDisplayTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (ClockBase)d;

            if (ctl.IsHandlerSuspended(DisplayTimeProperty)) return;

            var v = (DateTime)e.NewValue;
            ctl.Update(v);
            ctl.OnDisplayTimeChanged(new FunctionEventArgs<DateTime>(v));
            if (!ctl.IsShowConfirm)
            {
                ctl.SetValueNoCallback(SelectedTimeProperty, v);
            }
        }

        public DateTime DisplayTime
        {
            get => (DateTime)GetValue(DisplayTimeProperty);
            set => SetValue(DisplayTimeProperty, value);
        }

        public static readonly DependencyProperty IsShowConfirmProperty = DependencyProperty.Register(
            "IsShowConfirm", typeof(bool), typeof(ClockBase), new PropertyMetadata(false));

        public bool IsShowConfirm
        {
            get => (bool)GetValue(IsShowConfirmProperty);
            set => SetValue(IsShowConfirmProperty, value);
        }

        protected virtual void OnSelectedTimeChanged(FunctionEventArgs<DateTime?> e) => RaiseEvent(e);

        protected virtual void OnDisplayTimeChanged(FunctionEventArgs<DateTime> e)
        {
            var handler = DisplayTimeChanged;
            handler?.Invoke(this, e);
        }

        protected void PART_ClearDate_Click(object sender, RoutedEventArgs e)
        {
            SelectedTime = null;
            Confirmed?.Invoke();
        }

        protected void PART_NowDate_Click(object sender, RoutedEventArgs e)
        {
            SelectedTime = DateTime.Now;
            Confirmed?.Invoke();
        }

        protected void ButtonConfirm_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedTime = DisplayTime;
            Confirmed?.Invoke();
        }

        internal abstract void Update(DateTime time);

        protected void Clock_SelectedTimeChanged(object sender, FunctionEventArgs<DateTime?> e) => SelectedTime = e.Info;

        private IDictionary<DependencyProperty, bool> _isHandlerSuspended;
        protected void SetIsHandlerSuspended(DependencyProperty property, bool value)
        {
            if (value)
            {
                if (_isHandlerSuspended == null)
                {
                    _isHandlerSuspended = new Dictionary<DependencyProperty, bool>(2);
                }

                _isHandlerSuspended[property] = true;
            }
            else
            {
                _isHandlerSuspended?.Remove(property);
            }
        }

        protected void SetValueNoCallback(DependencyProperty property, object value)
        {
            SetIsHandlerSuspended(property, true);
            try
            {
                SetCurrentValue(property, value);
            }
            finally
            {
                SetIsHandlerSuspended(property, false);
            }
        }

        protected bool IsHandlerSuspended(DependencyProperty property)
        {
            return _isHandlerSuspended != null && _isHandlerSuspended.ContainsKey(property);
        }
    }
}