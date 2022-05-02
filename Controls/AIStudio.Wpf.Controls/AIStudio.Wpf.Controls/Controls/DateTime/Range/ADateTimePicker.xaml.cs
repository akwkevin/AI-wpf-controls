using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_Popup_New, Type = typeof(Popup))]
    [TemplatePart(Name = PART_Popup_TimeSelector, Type = typeof(Popup))]
    [TemplatePart(Name = PART_Calendar, Type = typeof(ACalendar))]
    [TemplatePart(Name = PART_TimeSelector, Type = typeof(ClockBase))]
    [TemplatePart(Name = PART_TextBox_New, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_TextBox_Display, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_ConfirmSelected, Type = typeof(Button))]
    [TemplatePart(Name = PART_ClearDate, Type = typeof(Button))]
    [TemplatePart(Name = PART_NowDate, Type = typeof(Button))]
    /// <summary>
    /// 日期选择控件
    /// </summary>
    public class ADateTimePicker : Control
    {
        protected const string PART_Popup_New = "PART_Popup_New";
        protected const string PART_Popup_TimeSelector = "PART_Popup_TimeSelector";
        protected const string PART_Calendar = "PART_Calendar";
        protected const string PART_TimeSelector = "PART_TimeSelector";
        protected const string PART_TextBox_New = "PART_TextBox_New";
        protected const string PART_ConfirmSelected = "PART_ConfirmSelected";
        protected const string PART_ClearDate = "PART_ClearDate";
        protected const string PART_NowDate = "PART_NowDate";
        protected const string PART_TextBox_Display = "PART_TextBox_Display";
        #region Private属性

        #region 控件内部元素
        private Popup _popup_New;
        private Popup _popup_TimeSelector;
        /// <summary>
        /// 日历：单个日历
        /// </summary>
        private ACalendar _calendar;
        /// <summary>
        /// 时间选择器
        /// </summary>
        private ListClock _timeSelector;
        /// <summary>
        /// 文本框：显示选中的日期
        /// </summary>
        private TextBox _textBox_New;
        /// <summary>
        /// 文本框：显示选中的日期
        /// </summary>
        private TextBox _textBox_Display;
        /// <summary>
        /// 按钮：清除所选日期
        /// </summary>
        private Button _clearDate;
        /// <summary>
        /// 当前日期
        /// </summary>
        private Button _nowDate;
        /// <summary>
        /// 按钮：确认选择所选日期
        /// </summary>
        private Button _confirmSelected;
        #endregion

        #endregion

        #region 依赖属性set get
        #region Type 日历类型
        /// <summary>
        /// 日历类型。SingleDate：单个日期，SingleDateRange：连续的多个日期
        /// </summary>
        public EnumDatePickerType Type
        {
            get
            {
                return (EnumDatePickerType)GetValue(TypeProperty);
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(EnumDatePickerType), typeof(ADateTimePicker), new PropertyMetadata(EnumDatePickerType.Date, TypeChangedCallback));

        private static void TypeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ADateTimePicker datePicker = d as ADateTimePicker;
            datePicker.SetSelectionMode(datePicker, (EnumDatePickerType)e.NewValue);
        }
        #endregion

        #region SelectedDate

        /// <summary>
        /// 获取或设置选中的日期
        /// </summary>
        public DateTime? SelectedDateTime
        {
            get
            {
                return (DateTime?)GetValue(SelectedDateTimeProperty);
            }
            set
            {
                SetValue(SelectedDateTimeProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register("SelectedDateTime", typeof(DateTime?), typeof(ADateTimePicker), new PropertyMetadata(null, SelectedDateTimeCallback));

        private static void SelectedDateTimeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ADateTimePicker datePicker = d as ADateTimePicker;
            DateTime? dateTime = (DateTime?)e.NewValue;
            if (dateTime.HasValue)
            {
                DateTime dt = dateTime.Value;
                datePicker.SetDateToTextBox(dt);
            }
            else
            {
                //TODO
                //显示水印
            }
        }

        #endregion

        #region DisplayDate

        public DateTime DisplayDateTime
        {
            get
            {
                return (DateTime)GetValue(DisplayDateTimeProperty);
            }
            set
            {
                SetValue(DisplayDateTimeProperty, value);
            }
        }

        public static readonly DependencyProperty DisplayDateTimeProperty =
            DependencyProperty.Register("DisplayDateTime", typeof(DateTime), typeof(ADateTimePicker));

        #endregion

        #region DateTimeFormat

        public string DateTimeFormat
        {
            get
            {
                return (string)GetValue(DateTimeFormatProperty);
            }
            set
            {
                SetValue(DateTimeFormatProperty, value);
            }
        }

        public static readonly DependencyProperty DateTimeFormatProperty =
            DependencyProperty.Register("DateTimeFormat", typeof(string), typeof(ADateTimePicker), new PropertyMetadata("yyyy-MM-dd HH:mm:ss"));

        #endregion

        #region IsShowConfirm
        /// <summary>
        /// 获取或设置是否显示确认按钮
        /// </summary>
        public bool IsShowConfirm
        {
            get
            {
                return (bool)GetValue(IsShowConfirmProperty);
            }
            set
            {
                SetValue(IsShowConfirmProperty, value);
            }
        }

        public static readonly DependencyProperty IsShowConfirmProperty =
            DependencyProperty.Register("IsShowConfirm", typeof(bool), typeof(ADateTimePicker), new PropertyMetadata(false));

        #endregion

        #region IsDropDownOpen

        public bool IsDropDownOpen
        {
            get
            {
                return (bool)GetValue(IsDropDownOpenProperty);
            }
            set
            {
                SetValue(IsDropDownOpenProperty, value);
            }
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(ADateTimePicker), new PropertyMetadata(false));

        #endregion

        #endregion

        #region Constructors
        static ADateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ADateTimePicker), new FrameworkPropertyMetadata(typeof(ADateTimePicker)));
        }

        public ADateTimePicker()
        {

        }
        #endregion

        #region Override方法
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this._popup_New != null)
            {
                this._popup_New.Opened -= PART_Popup_New_Opened;
            }

            if (this._calendar != null)
            {
                this._calendar.DateClick -= PART_Calendar_DateClick;
            }

            if (this._timeSelector != null)
            {
                this._timeSelector.SelectedTimeChanged -= PART_TimeSelector_SelectedTimeChanged;
            }

            if (this._confirmSelected != null)
            {
                this._confirmSelected.Click -= _confirmSelected_Click;
            }

            if (this._clearDate != null)
            {
                this._clearDate.Click -= PART_ClearDate_Click;
            }

            if (this._nowDate != null)
            {
                this._nowDate.Click -= PART_NowDate_Click;
            }

            this._popup_New = GetTemplateChild(PART_Popup_New) as Popup;
            this._popup_TimeSelector = GetTemplateChild(PART_Popup_TimeSelector) as Popup;
            this._calendar = GetTemplateChild(PART_Calendar) as ACalendar;
            this._timeSelector = GetTemplateChild(PART_TimeSelector) as ListClock;
            this._textBox_New = GetTemplateChild(PART_TextBox_New) as TextBox;
            this._textBox_Display = GetTemplateChild(PART_TextBox_Display) as TextBox;
            this._confirmSelected = GetTemplateChild(PART_ConfirmSelected) as Button;
            this._clearDate = GetTemplateChild(PART_ClearDate) as Button;
            this._nowDate = GetTemplateChild(PART_NowDate) as Button;

            if (this._popup_New != null)
            {
                this._popup_New.Opened += PART_Popup_New_Opened;
            }

            if (this._calendar != null)
            {
                this._calendar.DateClick += PART_Calendar_DateClick;
            }

            if (this._timeSelector != null)
            {
                this._timeSelector.SelectedTimeChanged += PART_TimeSelector_SelectedTimeChanged;
            }

            if (this._confirmSelected != null)
            {
                this._confirmSelected.Click += _confirmSelected_Click;
            }

            if (this._clearDate != null)
            {
                this._clearDate.Click += PART_ClearDate_Click;
            }

            if (this._nowDate != null)
            {
                this._nowDate.Click += PART_NowDate_Click;
            }

            this.SetSelectionMode(this, this.Type);
            this.SetDateToTextBox(SelectedDateTime);
        }

        /// <summary>
        /// 日期点击处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_Calendar_DateClick(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            UpdateDisplayTime();
            if (Type == EnumDatePickerType.Date)
            {
                this.IsDropDownOpen = false;
            }
        }

        private void PART_TimeSelector_SelectedTimeChanged(object sender, Event.FunctionEventArgs<DateTime?> e)
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
                var time = _timeSelector.DisplayTime;

                var result = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                DisplayDateTime = result;
                SetDateToDisplayTextBox(DisplayDateTime);
                if (!IsShowConfirm)
                {
                    SelectedDateTime = result;
                }
            }
        }
        #endregion

        #region Private方法

        private void SetSelectedDate()
        {
            if (this._calendar != null)
            {
                this._calendar.SelectedDate = this.SelectedDateTime;
            }
            if (this._timeSelector != null)
            {
                this._timeSelector.SelectedTime = this.SelectedDateTime;
            }

            SetDateToDisplayTextBox(SelectedDateTime);
        }

        private void PART_ClearDate_Click(object sender, RoutedEventArgs e)
        {
            if (this._textBox_New != null)
            {
                this._textBox_New.Text = string.Empty;
            }
            SelectedDateTime = null;
            this.IsDropDownOpen = false;
        }

        private void PART_NowDate_Click(object sender, RoutedEventArgs e)
        {
            if (Type == EnumDatePickerType.DateTime)
            {
                SelectedDateTime = DateTime.Now;
            }
            else
            {
                SelectedDateTime = DateTime.Today;
            }
            this.IsDropDownOpen = false;
        }

        private void _confirmSelected_Click(object sender, RoutedEventArgs e)
        {
            SelectedDateTime = DisplayDateTime;
            this.IsDropDownOpen = false;
        }

        private void PART_Popup_New_Opened(object sender, EventArgs e)
        {
            if (this._calendar == null)
            {
                return;
            }

            this._calendar.DisplayMode = CalendarMode.Month;
            this.SetSelectedDate();

        }

        /// <summary>
        /// 将当前选择的日期显示到文本框中
        /// </summary>
        /// <param name="selectedDate"></param>
        private void SetDateToTextBox(DateTime? selectedDate)
        {
            if (this._textBox_New != null)
            {
                if (selectedDate.HasValue)
                {
                    this._textBox_New.Text = selectedDate.Value.ToString(this.DateTimeFormat);
                }
            }
        }

        /// <summary>
        /// 将当前选择的日期显示到文本框中
        /// </summary>
        /// <param name="selectedDate"></param>
        private void SetDateToDisplayTextBox(DateTime? selectedDate)
        {
            if (this._textBox_Display != null)
            {
                if (selectedDate.HasValue)
                {
                    this._textBox_Display.Text = selectedDate.Value.ToString(this.DateTimeFormat);
                }
                else
                {
                    this._textBox_Display.Text = null;
                }
            }
        }

        /// <summary>
        /// 设置控件的类型
        /// </summary>
        /// <param name="datePicker"></param>
        /// <param name="type"></param>
        private void SetSelectionMode(ADateTimePicker datePicker, EnumDatePickerType type)
        {
            switch (type)
            {
                case EnumDatePickerType.Date:
                    if (datePicker._calendar != null)
                    {
                        datePicker._calendar.SelectionMode = CalendarSelectionMode.SingleDate;
                    }
                    break;
                case EnumDatePickerType.DateTime:
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}