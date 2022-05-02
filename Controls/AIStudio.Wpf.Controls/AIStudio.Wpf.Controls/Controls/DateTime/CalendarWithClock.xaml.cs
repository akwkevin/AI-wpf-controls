using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using AIStudio.Wpf.Controls.Event;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_ButtonConfirm, Type = typeof(Button))]
    [TemplatePart(Name = PART_ClockPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_CalendarPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_ClearDate, Type = typeof(Button))]
    [TemplatePart(Name = PART_NowDate, Type = typeof(Button))]
    public class CalendarWithClock : Control
    {
        #region Constants
        private const string PART_ButtonConfirm = "PART_ButtonConfirm";
        private const string PART_ClockPresenter = "PART_ClockPresenter";
        private const string PART_CalendarPresenter = "PART_CalendarPresenter";
        private const string PART_ClearDate = "PART_ClearDate";
        private const string PART_NowDate = "PART_NowDate";
        #endregion Constants

        #region Data

        private ContentPresenter _clockPresenter;
        private ContentPresenter _calendarPresenter;
        private System.Windows.Controls.Calendar _calendar;
        private Button _buttonConfirm;
        /// <summary>
        /// 按钮：清除所选日期
        /// </summary>
        private Button _clearDate;
        /// <summary>
        /// 当前日期
        /// </summary>
        private Button _nowDate;
        private bool _isLoaded;

        private IDictionary<DependencyProperty, bool> _isHandlerSuspended;

        #endregion Data

        #region Public Events

        public static readonly RoutedEvent SelectedDateTimeChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedDateTimeChanged", RoutingStrategy.Direct,
                typeof(EventHandler<FunctionEventArgs<DateTime?>>), typeof(CalendarWithClock));

        public event EventHandler<FunctionEventArgs<DateTime?>> SelectedDateTimeChanged
        {
            add => AddHandler(SelectedDateTimeChangedEvent, value);
            remove => RemoveHandler(SelectedDateTimeChangedEvent, value);
        }

        public event EventHandler<FunctionEventArgs<DateTime>> DisplayDateTimeChanged;

        public event Action Confirmed;

        #endregion Public Events

        public CalendarWithClock()
        {
            InitCalendarAndClock();
            Loaded += (s, e) => {
                if (_isLoaded) return;
                _isLoaded = true;
            };
        }

        #region Public Properties

        public static readonly DependencyProperty DateTimeFormatProperty = DependencyProperty.Register(
            "DateTimeFormat", typeof(string), typeof(CalendarWithClock), new PropertyMetadata("yyyy-MM-dd HH:mm:ss"));

        public string DateTimeFormat
        {
            get => (string)GetValue(DateTimeFormatProperty);
            set => SetValue(DateTimeFormatProperty, value);
        }

        public static readonly DependencyProperty IsShowConfirmProperty = DependencyProperty.Register(
            "IsShowConfirm", typeof(bool), typeof(CalendarWithClock), new PropertyMetadata(false));

        public bool IsShowConfirm
        {
            get => (bool)GetValue(IsShowConfirmProperty);
            set => SetValue(IsShowConfirmProperty, value);
        }

        public static readonly DependencyProperty IsShowClockProperty = DependencyProperty.Register(
           "IsShowClock", typeof(bool), typeof(CalendarWithClock), new PropertyMetadata(true));

        public bool IsShowClock
        {
            get => (bool)GetValue(IsShowClockProperty);
            set => SetValue(IsShowClockProperty, value);
        }

        public static readonly DependencyProperty SelectedDateTimeProperty = DependencyProperty.Register(
            "SelectedDateTime", typeof(DateTime?), typeof(CalendarWithClock), new PropertyMetadata(default(DateTime?), OnSelectedDateTimeChanged));

        private static void OnSelectedDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (CalendarWithClock)d;
            var v = (DateTime?)e.NewValue;
            ctl.OnSelectedDateTimeChanged(new FunctionEventArgs<DateTime?>(SelectedDateTimeChangedEvent, ctl)
            {
                Info = v
            });

            if (ctl.IsHandlerSuspended(SelectedDateTimeProperty)) return;
            ctl.DisplayDateTime = v ?? DateTime.Now;
        }

        public DateTime? SelectedDateTime
        {
            get => (DateTime?)GetValue(SelectedDateTimeProperty);
            set => SetValue(SelectedDateTimeProperty, value);
        }

        public static readonly DependencyProperty DisplayDateTimeProperty = DependencyProperty.Register(
            "DisplayDateTime", typeof(DateTime), typeof(CalendarWithClock), new FrameworkPropertyMetadata(DateTime.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayDateTimeChanged));

        private static void OnDisplayDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (CalendarWithClock)d;
            if (ctl.IsHandlerSuspended(DisplayDateTimeProperty)) return;
            var v = (DateTime)e.NewValue;
            ctl.Clock.SelectedTime = v;
            ctl._calendar.SelectedDate = v;
            ctl.OnDisplayDateTimeChanged(new FunctionEventArgs<DateTime>(v));

            if (!ctl.IsShowConfirm)
            {
                ctl.SetValueNoCallback(SelectedDateTimeProperty, v);
            }
        }

        public DateTime DisplayDateTime
        {
            get => (DateTime)GetValue(DisplayDateTimeProperty);
            set => SetValue(DisplayDateTimeProperty, value);
        }

        public static readonly DependencyProperty ClockProperty = DependencyProperty.Register(
            "Clock", typeof(ClockBase), typeof(CalendarWithClock), new FrameworkPropertyMetadata(default(Clock), FrameworkPropertyMetadataOptions.NotDataBindable, OnClockChanged));

        private static void OnClockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (CalendarWithClock)d;
            var oldClock = e.OldValue as ClockBase;
            var newClock = e.NewValue as ClockBase;

            if (oldClock != null)
            {
                oldClock.DisplayTimeChanged -= ctl.Clock_DisplayTimeChanged;
            }

            if (newClock != null)
            {
                newClock.DisplayTimeChanged -= ctl.Clock_DisplayTimeChanged;
                newClock.DisplayTimeChanged += ctl.Clock_DisplayTimeChanged;

                if (ctl.ClockName != "Flip")
                {
                    ctl.ClockName = newClock.GetType().Name;
                }
                if (ctl.ClockName != "NumberClock")
                {
                    newClock.BorderThickness = new Thickness();
                    newClock.Background = Brushes.Transparent;
                    Binding binding = new Binding() { Path = new PropertyPath("ActualHeight"), Source = ctl._calendar };
                    newClock.SetBinding(HeightProperty, binding);
                }
                else
                {
                    newClock.Padding = new Thickness(1);
                    newClock.Background = Brushes.Transparent;
                    newClock.SetValue(ControlAttach.CornerRadiusProperty, new CornerRadius(0));
                }
            }

            if (ctl._clockPresenter != null)
            {
                ctl._clockPresenter.Content = newClock;
            }
        }

        public ClockBase Clock
        {
            get => (ClockBase)GetValue(ClockProperty);
            set => SetValue(ClockProperty, value);
        }

        public static readonly DependencyProperty ClockNameProperty = DependencyProperty.Register(
           "ClockName", typeof(string), typeof(CalendarWithClock), new PropertyMetadata("Clock"));

        public string ClockName
        {
            get => (string)GetValue(ClockNameProperty);
            set => SetValue(ClockNameProperty, value);
        }
        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            if (_buttonConfirm != null)
            {
                _buttonConfirm.Click -= ButtonConfirm_OnClick;
            }
            if (this._clearDate != null)
            {
                this._clearDate.Click -= PART_ClearDate_Click;
            }
            if (this._nowDate != null)
            {
                this._nowDate.Click -= PART_NowDate_Click;
            }

            base.OnApplyTemplate();

            _buttonConfirm = GetTemplateChild(PART_ButtonConfirm) as Button;
            _clockPresenter = GetTemplateChild(PART_ClockPresenter) as ContentPresenter;
            _calendarPresenter = GetTemplateChild(PART_CalendarPresenter) as ContentPresenter;
            this._clearDate = GetTemplateChild(PART_ClearDate) as Button;
            this._nowDate = GetTemplateChild(PART_NowDate) as Button;

            CheckNull();

            _clockPresenter.Content = Clock;
            _calendarPresenter.Content = _calendar;

            if (this._buttonConfirm != null)
            {
                _buttonConfirm.Click += ButtonConfirm_OnClick;
            }
            if (this._clearDate != null)
            {
                this._clearDate.Click += PART_ClearDate_Click;
            }
            if (this._nowDate != null)
            {
                this._nowDate.Click += PART_NowDate_Click;
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void OnSelectedDateTimeChanged(FunctionEventArgs<DateTime?> e) => RaiseEvent(e);

        protected virtual void OnDisplayDateTimeChanged(FunctionEventArgs<DateTime> e)
        {
            var handler = DisplayDateTimeChanged;
            handler?.Invoke(this, e);
        }

        #endregion Protected Methods

        #region Private Methods

        private void SetIsHandlerSuspended(DependencyProperty property, bool value)
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

        private void SetValueNoCallback(DependencyProperty property, object value)
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

        private bool IsHandlerSuspended(DependencyProperty property)
        {
            return _isHandlerSuspended != null && _isHandlerSuspended.ContainsKey(property);
        }

        private void CheckNull()
        {
            if (_buttonConfirm == null || _clockPresenter == null || _calendarPresenter == null) throw new Exception();
        }

        private void PART_ClearDate_Click(object sender, RoutedEventArgs e)
        {
            SelectedDateTime = null;
            Confirmed?.Invoke();
        }

        private void PART_NowDate_Click(object sender, RoutedEventArgs e)
        {
            if (IsShowClock)
            {
                SelectedDateTime = DateTime.Now;
            }
            else
            {
                SelectedDateTime = DateTime.Today;
            }
            Confirmed?.Invoke();
        }
        private void ButtonConfirm_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedDateTime = DisplayDateTime;
            Confirmed?.Invoke();
        }

        private void InitCalendarAndClock()
        {
            _calendar = new System.Windows.Controls.Calendar
            {
                BorderThickness = new Thickness(),
                Background = Brushes.Transparent,
                Focusable = false,
            };
            _calendar.SelectedDatesChanged += Calendar_SelectedDatesChanged;

            Clock = new ListClock
            {
                BorderThickness = new Thickness(),
                Background = Brushes.Transparent
            };
            Clock.DisplayTimeChanged += Clock_DisplayTimeChanged;
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.Capture(null);
            UpdateDisplayTime();
        }

        private void Clock_DisplayTimeChanged(object sender, FunctionEventArgs<DateTime> e)
        {
            UpdateDisplayTime(true);
        }

        private void UpdateDisplayTime(bool autodate = false)
        {
            if (_calendar.SelectedDate == null && autodate == true)
            {
                _calendar.SelectedDate = DateTime.Today;
            }

            if (_calendar.SelectedDate != null)
            {
                var date = _calendar.SelectedDate.Value;
                var time = Clock.DisplayTime;

                var result = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                SetValueNoCallback(DisplayDateTimeProperty, result);
            }

            if (!IsShowConfirm)
            {
                SelectedDateTime = DisplayDateTime;
            }
        }

        #endregion
    }
}