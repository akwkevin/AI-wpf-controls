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
    ///     时间日期选择器
    /// </summary>
    [TemplatePart(Name = PART_Root, Type = typeof(Grid))]
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Button, Type = typeof(Button))]
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    public class DateTimePicker : Control
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

        private DateTime? _originalSelectedDateTime;

        #endregion Data

        #region Public Events

        public static readonly RoutedEvent SelectedDateTimeChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedDateTimeChanged", RoutingStrategy.Direct,
                typeof(EventHandler<FunctionEventArgs<DateTime?>>), typeof(DateTimePicker));

        public event EventHandler<FunctionEventArgs<DateTime?>> SelectedDateTimeChanged
        {
            add => AddHandler(SelectedDateTimeChangedEvent, value);
            remove => RemoveHandler(SelectedDateTimeChangedEvent, value);
        }

        public event RoutedEventHandler PickerClosed;

        public event RoutedEventHandler PickerOpened;

        #endregion Public Events

        static DateTimePicker()
        {
            EventManager.RegisterClassHandler(typeof(DateTimePicker), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(false));
        }

        public DateTimePicker()
        {
            InitCalendarWithClock();
        }

        #region Public Properties
        public static readonly DependencyProperty CalendarStyleProperty = DependencyProperty.Register(
            "CalendarStyle", typeof(Style), typeof(DateTimePicker), new PropertyMetadata(default(Style)));

        public Style CalendarStyle
        {
            get => (Style)GetValue(CalendarStyleProperty);
            set => SetValue(CalendarStyleProperty, value);
        }

        public static readonly DependencyProperty DateTimeFormatProperty = DependencyProperty.Register(
            "DateTimeFormat", typeof(string), typeof(DateTimePicker), new PropertyMetadata("yyyy-MM-dd HH:mm:ss"));

        public string DateTimeFormat
        {
            get => (string)GetValue(DateTimeFormatProperty);
            set => SetValue(DateTimeFormatProperty, value);
        }

        public static readonly DependencyProperty DisplayDateTimeProperty = DependencyProperty.Register(
            "DisplayDateTime", typeof(DateTime), typeof(DateTimePicker), new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceDisplayDateTime));

        private static object CoerceDisplayDateTime(DependencyObject d, object value)
        {
            var dp = (DateTimePicker)d;
            dp.CalendarWithClock.DisplayDateTime = (DateTime)value;

            return dp.CalendarWithClock.DisplayDateTime;
        }

        public DateTime DisplayDateTime
        {
            get => (DateTime)GetValue(DisplayDateTimeProperty);
            set => SetValue(DisplayDateTimeProperty, value);
        }

        public static readonly DependencyProperty SelectedDateTimeProperty = DependencyProperty.Register(
            "SelectedDateTime", typeof(DateTime?), typeof(DateTimePicker), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateTimeChanged, CoerceSelectedDateTime));

        private static object CoerceSelectedDateTime(DependencyObject d, object value)
        {
            var dp = (DateTimePicker)d;
            dp.CalendarWithClock.SelectedDateTime = (DateTime?)value;
            return dp.CalendarWithClock.SelectedDateTime;
        }

        private static void OnSelectedDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DateTimePicker dp)) return;

            if (dp.SelectedDateTime != null)
            {
                var time = dp.SelectedDateTime.Value;
                dp.SetTextInternal(dp.DateTimeToString(time));
            }
            else
            {
                dp.SetTextInternal(string.Empty);
            }

            if (dp.IsHandlerSuspended(SelectedDateTimeProperty)) return;

            dp.RaiseEvent(new FunctionEventArgs<DateTime?>(SelectedDateTimeChangedEvent, dp)
            {
                Info = dp.SelectedDateTime
            });
        }

        public DateTime? SelectedDateTime
        {
            get => (DateTime?)GetValue(SelectedDateTimeProperty);
            set => SetValue(SelectedDateTimeProperty, value);
        }

        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
            "IsDropDownOpen", typeof(bool), typeof(DateTimePicker), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsDropDownOpenChanged, OnCoerceIsDropDownOpen));

        private static object OnCoerceIsDropDownOpen(DependencyObject d, object baseValue) => d is DateTimePicker dp && !dp.IsEnabled ? false : baseValue;

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as DateTimePicker;

            var newValue = (bool)e.NewValue;
            if (dp?._popup != null && dp._popup.IsOpen != newValue)
            {
                dp._popup.IsOpen = newValue;
                if (newValue)
                {
                    dp._originalSelectedDateTime = dp.SelectedDateTime;

                    dp.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action)delegate {
                        dp.CalendarWithClock.Focus();
                    });
                }
            }
        }

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(DateTimePicker), new FrameworkPropertyMetadata(string.Empty, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimePicker dp && !dp.IsHandlerSuspended(TextProperty))
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

                    dp.SetSelectedDateTime();
                }
                else
                {
                    dp.SetValueNoCallback(SelectedDateTimeProperty, null);
                }
            }
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Sets the local Text property without breaking bindings
        /// </summary>
        /// <param name="value"></param>
        private void SetTextInternal(string value)
        {
            SetCurrentValue(TextProperty, value);
        }


        public static readonly DependencyProperty ShowClearButtonProperty = DependencyProperty.Register(
            "ShowClearButton", typeof(bool), typeof(DateTimePicker), new PropertyMetadata(false));

        public bool ShowClearButton
        {
            get => (bool)GetValue(ShowClearButtonProperty);
            set => SetValue(ShowClearButtonProperty, value);
        }

        public static readonly DependencyProperty IsDefaultValueProperty = DependencyProperty.Register(
           "IsDefaultValue", typeof(bool), typeof(DateTimePicker), new PropertyMetadata(false));

        public bool IsDefaultValue
        {
            get => (bool)GetValue(IsDefaultValueProperty);
            set => SetValue(IsDefaultValueProperty, value);
        }

        public static readonly DependencyProperty CalendarWithClockProperty = DependencyProperty.Register(
           "CalendarWithClock", typeof(CalendarWithClock), typeof(DateTimePicker), new FrameworkPropertyMetadata(default(CalendarWithClock), FrameworkPropertyMetadataOptions.NotDataBindable, OnCalendarWithClockChanged));

        private static void OnCalendarWithClockChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (DateTimePicker)d;
            var oldClock = e.OldValue as CalendarWithClock;
            var newClock = e.NewValue as CalendarWithClock;

            if (oldClock != null)
            {
                oldClock.SelectedDateTimeChanged -= ctl.CalendarWithClock_SelectedDateTimeChanged;
                oldClock.Confirmed -= ctl.CalendarWithClock_Confirmed;
            }

            if (newClock != null)
            {
                newClock.SelectedDateTimeChanged += ctl.CalendarWithClock_SelectedDateTimeChanged;
                newClock.Confirmed += ctl.CalendarWithClock_Confirmed;
                newClock.SetValue(ShadowAttach.DropShadowEffectProperty, newClock.FindResource("ShadowDepth1"));
            }
        }

        public CalendarWithClock CalendarWithClock
        {
            get => (CalendarWithClock)GetValue(CalendarWithClockProperty);
            set => SetValue(CalendarWithClockProperty, value);
        }
        #endregion

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

            CheckNull();

            _popup.PreviewMouseLeftButtonDown += PopupPreviewMouseLeftButtonDown;
            _popup.Opened += PopupOpened;
            _popup.Closed += PopupClosed;
            _popup.Child = CalendarWithClock;

            if (IsDropDownOpen)
            {
                _popup.IsOpen = true;
            }

            _dropDownButton.Click += DropDownButton_Click;
            _dropDownButton.MouseLeave += DropDownButton_MouseLeave;


            if (SelectedDateTime != null)
            {
                _textBox.Text = DateTimeToString(SelectedDateTime.Value);
            }
            else if (IsDefaultValue)
            {
                _textBox.Text = DateTime.Now.ToString(DateTimeFormat);
                //if (!string.IsNullOrEmpty(_defaultText))
                //{
                //    _textBox.Text = _defaultText;
                //    SetSelectedDateTime();
                //}
                SetValueNoCallback(SelectedDateTimeProperty, DateTime.Now);
            }

            _textBox.KeyDown += TextBox_KeyDown;
            _textBox.TextChanged += TextBox_TextChanged;
            _textBox.LostFocus += TextBox_LostFocus;

            //if (_originalSelectedDateTime == null)
            //{
            //    _originalSelectedDateTime = DateTime.Now;
            //}
            //SetCurrentValue(DisplayDateTimeProperty, _originalSelectedDateTime);
        }

        public override string ToString() => SelectedDateTime?.ToString(DateTimeFormat) ?? string.Empty;

        #endregion

        #region Protected Methods

        protected virtual void OnPickerClosed(RoutedEventArgs e)
        {
            var handler = PickerClosed;
            handler?.Invoke(this, e);
        }

        protected virtual void OnPickerOpened(RoutedEventArgs e)
        {
            var handler = PickerOpened;
            handler?.Invoke(this, e);
        }

        #endregion Protected Methods

        #region Private Methods

        private void CheckNull()
        {
            if (_dropDownButton == null || _popup == null || _textBox == null)
                throw new Exception();
        }

        private void InitCalendarWithClock()
        {
            CalendarWithClock = new CalendarWithClock
            {
                ClockName = "Flip",
                IsShowConfirm = true,
            };
        }

        private void CalendarWithClock_Confirmed() => TogglePopup();

        private void CalendarWithClock_SelectedDateTimeChanged(object sender, FunctionEventArgs<DateTime?> e) => SelectedDateTime = e.Info;

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SetSelectedDateTime();
        }

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

        private bool ProcessDateTimePickerKey(KeyEventArgs e)
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
                        SetSelectedDateTime();
                        return true;
                    }
            }

            return false;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = ProcessDateTimePickerKey(e) || e.Handled;
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

        private void PopupOpened(object sender, EventArgs e)
        {
            if (!IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, true);
            }

            CalendarWithClock?.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

            OnPickerOpened(new RoutedEventArgs());
        }

        private void PopupClosed(object sender, EventArgs e)
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }

            if (CalendarWithClock.IsKeyboardFocusWithin)
            {
                MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }

            OnPickerClosed(new RoutedEventArgs());
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
                    SetDisplayDateTime();
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
                return SelectedDateTime;
            }

            var d = ParseText(s);

            if (d != null)
            {
                SafeSetText(DateTimeToString((DateTime)d));
                return d;
            }

            if (SelectedDateTime != null)
            {
                var newtext = DateTimeToString(SelectedDateTime.Value);
                SafeSetText(newtext);
                return SelectedDateTime;
            }
            SafeSetText(DateTimeToString(DisplayDateTime));
            return DisplayDateTime;
        }

        private void SetDisplayDateTime()
        {
            if (SelectedDateTime != null && SelectedDateTime != DisplayDateTime)
            {
                SetCurrentValue(DisplayDateTimeProperty, SelectedDateTime);
            }
        }


        private void SetSelectedDateTime()
        {
            if (_textBox != null)
            {
                if (!string.IsNullOrEmpty(_textBox.Text))
                {
                    var s = _textBox.Text;

                    if (SelectedDateTime != null)
                    {
                        if (SelectedDateTime != DisplayDateTime)
                        {
                            SetCurrentValue(DisplayDateTimeProperty, SelectedDateTime);
                        }

                        var selectedTime = DateTimeToString(SelectedDateTime.Value);

                        if (string.Compare(selectedTime, s, StringComparison.Ordinal) == 0)
                        {
                            return;
                        }
                    }

                    var d = SetTextBoxValue(s);
                    if (!SelectedDateTime.Equals(d))
                    {
                        SetCurrentValue(SelectedDateTimeProperty, d);
                        SetCurrentValue(DisplayDateTimeProperty, d);
                    }
                }
                else
                {
                    if (SelectedDateTime.HasValue)
                    {
                        SetCurrentValue(SelectedDateTimeProperty, null);
                    }
                }
            }
            else
            {
                var d = SetTextBoxValue(_defaultText);
                if (!SelectedDateTime.Equals(d))
                {
                    SetCurrentValue(SelectedDateTimeProperty, d);
                }
            }
        }

        private string DateTimeToString(DateTime d) => d.ToString(DateTimeFormat);

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            var picker = (DateTimePicker)sender;
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

        #endregion
    }
}