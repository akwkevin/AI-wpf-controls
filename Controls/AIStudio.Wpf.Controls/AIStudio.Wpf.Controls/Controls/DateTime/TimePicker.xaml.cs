using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using AIStudio.Wpf.Controls.Event;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    ///     时间选择器
    /// </summary>
    [TemplatePart(Name = PART_Root, Type = typeof(Grid))]
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Button, Type = typeof(Button))]
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    public class TimePicker : Control
    {
        #region Constants

        private const string PART_Root = "PART_Root";
        private const string PART_TextBox = "PART_TextBox";
        private const string PART_Button = "PART_Button";
        private const string PART_Popup = "PART_Popup";

        #endregion Constants

        #region Data

        private string _defaultText;

        private ButtonBase _dropDownButton;

        private Popup _popup;

        private bool _disablePopupReopen;

        private TextBox _textBox;

        private IDictionary<DependencyProperty, bool> _isHandlerSuspended;

        private DateTime? _originalSelectedTime;

        #endregion Data

        #region Public Events

        public static readonly RoutedEvent SelectedTimeChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedTimeChanged", RoutingStrategy.Direct,
                typeof(EventHandler<FunctionEventArgs<DateTime?>>), typeof(TimePicker));

        public event EventHandler<FunctionEventArgs<DateTime?>> SelectedTimeChanged
        {
            add => AddHandler(SelectedTimeChangedEvent, value);
            remove => RemoveHandler(SelectedTimeChangedEvent, value);
        }

        public event RoutedEventHandler ClockClosed;

        public event RoutedEventHandler ClockOpened;

        #endregion Public Events

        static TimePicker()
        {
            EventManager.RegisterClassHandler(typeof(TimePicker), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(TimePicker), new FrameworkPropertyMetadata(false));
        }

        public TimePicker()
        {
            Clock = new Clock();
        }

        #region Public Properties

        #region TimeFormat

        public static readonly DependencyProperty TimeFormatProperty = DependencyProperty.Register(
            "TimeFormat", typeof(string), typeof(TimePicker), new PropertyMetadata("HH:mm:ss"));

        public string TimeFormat
        {
            get => (string)GetValue(TimeFormatProperty);
            set => SetValue(TimeFormatProperty, value);
        }

        #endregion TimeFormat

        #region DisplayTime

        public DateTime DisplayTime
        {
            get => (DateTime)GetValue(DisplayTimeProperty);
            set => SetValue(DisplayTimeProperty, value);
        }

        public static readonly DependencyProperty DisplayTimeProperty =
            DependencyProperty.Register(
                "DisplayTime",
                typeof(DateTime),
                typeof(TimePicker),
                new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceDisplayTime));

        private static object CoerceDisplayTime(DependencyObject d, object value)
        {
            var dp = (TimePicker)d;
            dp.Clock.DisplayTime = (DateTime)value;
            return dp.Clock.DisplayTime;
        }

        #endregion DisplayTime

        #region IsDropDownOpen

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(
            "IsDropDownOpen",
            typeof(bool),
            typeof(TimePicker),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsDropDownOpenChanged, OnCoerceIsDropDownOpen));

        private static object OnCoerceIsDropDownOpen(DependencyObject d, object baseValue) => d is TimePicker dp && !dp.IsEnabled ? false : baseValue;

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as TimePicker;

            var newValue = (bool)e.NewValue;
            if (dp?._popup != null && dp._popup.IsOpen != newValue)
            {
                dp._popup.IsOpen = newValue;
                if (newValue)
                {
                    dp._originalSelectedTime = dp.SelectedTime;

                    dp.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action)delegate {
                        dp.Clock.Focus();
                    });
                }
            }
        }

        #endregion IsDropDownOpen

        #region SelectedTime

        public DateTime? SelectedTime
        {
            get => (DateTime?)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register(
            "SelectedTime",
            typeof(DateTime?),
            typeof(TimePicker),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedTimeChanged, CoerceSelectedTime));

        private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is TimePicker dp)) return;

            if (dp.SelectedTime != null)
            {
                var time = dp.SelectedTime.Value;
                dp.SetTextInternal(dp.DateTimeToString(time));
            }
            else
            {
                dp.SetTextInternal(string.Empty);
            }

            if (dp.IsHandlerSuspended(SelectedTimeProperty)) return;

            dp.RaiseEvent(new FunctionEventArgs<DateTime?>(SelectedTimeChangedEvent, dp)
            {
                Info = dp.SelectedTime
            });
        }

        private static object CoerceSelectedTime(DependencyObject d, object value)
        {
            var dp = (TimePicker)d;
            dp.Clock.SelectedTime = (DateTime?)value;
            return dp.Clock.SelectedTime;
        }

        #endregion SelectedDate       

        #region Text

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(TimePicker),
            new FrameworkPropertyMetadata(string.Empty, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TimePicker dp && !dp.IsHandlerSuspended(TextProperty))
            {
                if (e.NewValue is string newValue)
                {
                    if (dp._textBox != null)
                    {
                        dp._textBox.Text = newValue;
                    }
                    else
                    {
                        dp._defaultText = newValue;
                    }

                    dp.SetSelectedTime();
                }
                else
                {
                    dp.SetValueNoCallback(SelectedTimeProperty, null);
                }
            }
        }

        /// <summary>
        /// Sets the local Text property without breaking bindings
        /// </summary>
        /// <param name="value"></param>
        private void SetTextInternal(string value)
        {
            SetCurrentValue(TextProperty, value);
        }

        #endregion Text

        public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register(
            "IsError", typeof(bool), typeof(TimePicker), new PropertyMetadata(false));

        public bool IsError
        {
            get => (bool)GetValue(IsErrorProperty);
            set => SetValue(IsErrorProperty, value);
        }

        public static readonly DependencyProperty ErrorStrProperty = DependencyProperty.Register(
            "ErrorStr", typeof(string), typeof(TimePicker), new PropertyMetadata(default(string)));

        public string ErrorStr
        {
            get => (string)GetValue(ErrorStrProperty);
            set => SetValue(ErrorStrProperty, value);
        }

        public static readonly DependencyProperty ShowClearButtonProperty = DependencyProperty.Register(
            "ShowClearButton", typeof(bool), typeof(TimePicker), new PropertyMetadata(false));

        public bool ShowClearButton
        {
            get => (bool)GetValue(ShowClearButtonProperty);
            set => SetValue(ShowClearButtonProperty, value);
        }

        public static readonly DependencyProperty IsDefaultValueProperty = DependencyProperty.Register(
            "IsDefaultValue", typeof(bool), typeof(TimePicker), new PropertyMetadata(false));

        public bool IsDefaultValue
        {
            get => (bool)GetValue(IsDefaultValueProperty);
            set => SetValue(IsDefaultValueProperty, value);
        }


        public static readonly DependencyProperty ClockProperty = DependencyProperty.Register(
            "Clock", typeof(ClockBase), typeof(TimePicker), new FrameworkPropertyMetadata(default(Clock), FrameworkPropertyMetadataOptions.NotDataBindable, OnClockChanged));

        private static void OnClockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (TimePicker)d;
            var oldClock = e.OldValue as ClockBase;
            var newClock = e.NewValue as ClockBase;

            if (oldClock != null)
            {
                oldClock.SelectedTimeChanged -= ctl.Clock_SelectedTimeChanged;
                oldClock.Confirmed -= ctl.Clock_Confirmed;
            }

            if (newClock != null)
            {
                newClock.IsShowConfirm = true;
                newClock.SelectedTimeChanged += ctl.Clock_SelectedTimeChanged;
                newClock.Confirmed += ctl.Clock_Confirmed;
                newClock.SetValue(ShadowAttach.DropShadowEffectProperty, newClock.FindResource("ShadowDepth1"));
            }
        }

        public ClockBase Clock
        {
            get => (ClockBase)GetValue(ClockProperty);
            set => SetValue(ClockProperty, value);
        }

        #endregion Public Properties

        #region Public Methods
        public override void OnApplyTemplate()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (_popup != null)
            {
                _popup.PreviewMouseLeftButtonDown -= PopupPreviewMouseLeftButtonDown;
                _popup.Opened -= PopupOpened;
                _popup.Closed -= PopupClosed;
                _popup.Child = null;
            }

            if (_dropDownButton != null)
            {
                _dropDownButton.Click -= DropDownButton_Click;
                _dropDownButton.MouseLeave -= DropDownButton_MouseLeave;
            }

            if (_textBox != null)
            {
                _textBox.KeyDown -= TextBox_KeyDown;
                _textBox.TextChanged -= TextBox_TextChanged;
                _textBox.LostFocus -= TextBox_LostFocus;
            }

            base.OnApplyTemplate();

            _popup = GetTemplateChild(PART_Popup) as Popup;
            _dropDownButton = GetTemplateChild(PART_Button) as Button;
            _textBox = GetTemplateChild(PART_TextBox) as TextBox;

            if (CheckNull())
            {

                _popup.PreviewMouseLeftButtonDown += PopupPreviewMouseLeftButtonDown;
                _popup.Opened += PopupOpened;
                _popup.Closed += PopupClosed;
                _popup.Child = Clock;

                if (IsDropDownOpen)
                {
                    _popup.IsOpen = true;
                }

                _dropDownButton.Click += DropDownButton_Click;
                _dropDownButton.MouseLeave += DropDownButton_MouseLeave;

                if (SelectedTime != null)
                {
                    _textBox.Text = DateTimeToString((DateTime)SelectedTime);
                }
                else if (IsDefaultValue)
                {
                    //if (!string.IsNullOrEmpty(_defaultText))
                    //{
                    //    _textBox.Text = _defaultText;
                    //    SetSelectedTime();
                    //}
                    _textBox.Text = DateTime.Now.ToString(TimeFormat);
                    SetValueNoCallback(SelectedTimeProperty, DateTime.Now);
                }

                _textBox.KeyDown += TextBox_KeyDown;
                _textBox.TextChanged += TextBox_TextChanged;
                _textBox.LostFocus += TextBox_LostFocus;

                //if (_originalSelectedTime == null)
                //{
                //    _originalSelectedTime = DateTime.Now;
                //}
                //SetCurrentValue(DisplayTimeProperty, _originalSelectedTime);               
            }
        }

        public override string ToString() => SelectedTime?.ToString(TimeFormat) ?? string.Empty;

        #endregion

        #region Protected Methods

        protected virtual void OnClockClosed(RoutedEventArgs e)
        {
            var handler = ClockClosed;
            handler?.Invoke(this, e);
        }

        protected virtual void OnClockOpened(RoutedEventArgs e)
        {
            var handler = ClockOpened;
            handler?.Invoke(this, e);
        }

        #endregion Protected Methods

        #region Private Methods

        private bool CheckNull()
        {
            if (_dropDownButton == null || _popup == null || _textBox == null)
            {
                return false;
            }
            return true;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) => SetSelectedTime();

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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueNoCallback(TextProperty, _textBox.Text);
        }

        private bool ProcessTimePickerKey(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.System:
                    {
                        switch (e.SystemKey)
                        {
                            case Key.Down:
                                {
                                    if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                                    {
                                        TogglePopup();
                                        return true;
                                    }

                                    break;
                                }
                        }

                        break;
                    }

                case Key.Enter:
                    {
                        SetSelectedTime();
                        return true;
                    }
            }

            return false;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = ProcessTimePickerKey(e) || e.Handled;
        }

        private void DropDownButton_MouseLeave(object sender, MouseEventArgs e)
        {
            _disablePopupReopen = false;
        }

        private bool IsHandlerSuspended(DependencyProperty property)
        {
            return _isHandlerSuspended != null && _isHandlerSuspended.ContainsKey(property);
        }

        private void PopupPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Popup popup && !popup.StaysOpen)
            {
                if (_dropDownButton?.InputHitTest(e.GetPosition(_dropDownButton)) != null)
                {
                    _disablePopupReopen = true;
                }
            }
        }

        private void Clock_SelectedTimeChanged(object sender, FunctionEventArgs<DateTime?> e) => SelectedTime = e.Info;

        private void Clock_Confirmed() => TogglePopup();

        private void PopupOpened(object sender, EventArgs e)
        {
            if (!IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, true);
            }

            Clock?.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

            OnClockOpened(new RoutedEventArgs());
        }

        private void PopupClosed(object sender, EventArgs e)
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }

            if (Clock.IsKeyboardFocusWithin)
            {
                MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }

            OnClockClosed(new RoutedEventArgs());
        }

        private void DropDownButton_Click(object sender, RoutedEventArgs e) => TogglePopup();

        private void TogglePopup()
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }
            else
            {
                if (_disablePopupReopen)
                {
                    _disablePopupReopen = false;
                }
                else
                {
                    SetSelectedTime();
                    SetCurrentValue(IsDropDownOpenProperty, true);
                }
            }
        }

        private void SafeSetText(string s)
        {
            if (string.Compare(Text, s, StringComparison.Ordinal) != 0)
            {
                SetCurrentValue(TextProperty, s);
            }
        }

        private DateTime? ParseText(string text)
        {
            try
            {
                return DateTime.Parse(text);
            }
            catch
            {
                // ignored
            }

            return null;
        }

        private DateTime? SetTextBoxValue(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                SafeSetText(s);
                return SelectedTime;
            }

            var d = ParseText(s);

            if (d != null)
            {
                SafeSetText(DateTimeToString((DateTime)d));
                return d;
            }

            if (SelectedTime != null)
            {
                var newtext = DateTimeToString((DateTime)SelectedTime);
                SafeSetText(newtext);
                return SelectedTime;
            }
            SafeSetText(DateTimeToString(DisplayTime));
            return DisplayTime;
        }

        private void SetSelectedTime()
        {
            if (_textBox != null)
            {
                if (!string.IsNullOrEmpty(_textBox.Text))
                {
                    var s = _textBox.Text;

                    if (SelectedTime != null)
                    {
                        var selectedTime = DateTimeToString(SelectedTime.Value);

                        if (string.Compare(selectedTime, s, StringComparison.Ordinal) == 0)
                        {
                            return;
                        }
                    }

                    var d = SetTextBoxValue(s);
                    if (!SelectedTime.Equals(d))
                    {
                        SetCurrentValue(SelectedTimeProperty, d);
                        SetCurrentValue(DisplayTimeProperty, d);
                    }
                }
                else
                {
                    if (SelectedTime.HasValue)
                    {
                        SetCurrentValue(SelectedTimeProperty, null);
                    }
                }
            }
            else
            {
                var d = SetTextBoxValue(_defaultText);
                if (!SelectedTime.Equals(d))
                {
                    SetCurrentValue(SelectedTimeProperty, d);
                }
            }
        }

        private string DateTimeToString(DateTime d) => d.ToString(TimeFormat);

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            var picker = (TimePicker)sender;
            if (!e.Handled && picker._textBox != null)
            {
                if (Equals(e.OriginalSource, picker))
                {
                    picker._textBox.Focus();
                    e.Handled = true;
                }
                else if (Equals(e.OriginalSource, picker._textBox))
                {
                    picker._textBox.SelectAll();
                    e.Handled = true;
                }
            }
        }

        #endregion Private Methods
    }
}