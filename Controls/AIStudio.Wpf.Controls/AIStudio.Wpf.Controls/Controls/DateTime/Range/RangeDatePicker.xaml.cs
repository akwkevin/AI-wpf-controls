using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_Popup_New, Type = typeof(Popup))]
    [TemplatePart(Name = PART_Popup_TimeSelector, Type = typeof(Popup))]
    [TemplatePart(Name = PART_Calendar, Type = typeof(ACalendar))]
    [TemplatePart(Name = PART_Calendar_Second, Type = typeof(ACalendar))]
    [TemplatePart(Name = PART_TimeSelector, Type = typeof(ClockBase))]
    [TemplatePart(Name = PART_TimeSelector_Second, Type = typeof(ClockBase))]
    [TemplatePart(Name = PART_TextBox_New, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_TextBox_Display, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Btn_RecentlyAWeek, Type = typeof(Button))]
    [TemplatePart(Name = PART_Btn_RecentlyAMonth, Type = typeof(Button))]
    [TemplatePart(Name = PART_Btn_RecentlyThreeMonth, Type = typeof(Button))]
    [TemplatePart(Name = PART_ConfirmSelected, Type = typeof(Button))]
    [TemplatePart(Name = PART_ClearDate, Type = typeof(Button))]
    /// <summary>
    /// 日期选择控件
    /// </summary>
    public class RangeDatePicker : Control
    {
        protected const string PART_Popup_New = "PART_Popup_New";
        protected const string PART_Popup_TimeSelector = "PART_Popup_TimeSelector";
        protected const string PART_Calendar = "PART_Calendar";
        protected const string PART_Calendar_Second = "PART_Calendar_Second";
        protected const string PART_TimeSelector = "PART_TimeSelector";
        protected const string PART_TimeSelector_Second = "PART_TimeSelector_Second";
        protected const string PART_TextBox_New = "PART_TextBox_New";
        protected const string PART_TextBox_Display = "PART_TextBox_Display";
        protected const string PART_Btn_RecentlyAWeek = "PART_Btn_RecentlyAWeek";
        protected const string PART_Btn_RecentlyAMonth = "PART_Btn_RecentlyAMonth";
        protected const string PART_Btn_RecentlyThreeMonth = "PART_Btn_RecentlyThreeMonth";
        protected const string PART_ConfirmSelected = "PART_ConfirmSelected";
        protected const string PART_ClearDate = "PART_ClearDate";
        #region Private属性

        #region 控件内部元素
        private Popup _popup_New;
        private Popup _popup_TimeSelector;
        /// <summary>
        /// 日历：单个日历
        /// </summary>
        private ACalendar _calendar;
        /// <summary>
        /// 日历：双日期模式下的第二个日历
        /// </summary>
        private ACalendar _calendar_Second;
        /// <summary>
        /// 时间选择器
        /// </summary>
        private ListClock _timeSelector;
        /// <summary>
        /// 时间选择器
        /// </summary>
        private ListClock _timeSelector_Second;
        /// <summary>
        /// 文本框：显示选中的日期
        /// </summary>
        private TextBox _textBox_New;
        /// <summary>
        /// 文本框：显示选中的日期
        /// </summary>
        private TextBox _textBox_Display;
        /// <summary>
        /// 快捷菜单：最近一周
        /// </summary>
        private Button _btn_RecentlyAWeek;
        /// <summary>
        /// 快捷菜单：最近一个月
        /// </summary>
        private Button _btn_RecentlyAMonth;
        /// <summary>
        /// 快捷菜单：最近三个月
        /// </summary>
        private Button _btn_RecentlyThreeMonth;
        /// <summary>
        /// 按钮：清除所选日期
        /// </summary>
        private Button _clearDate;
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
            DependencyProperty.Register("Type", typeof(EnumDatePickerType), typeof(RangeDatePicker), new PropertyMetadata(EnumDatePickerType.DateRange, TypeChangedCallback));

        private static void TypeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeDatePicker datePicker = d as RangeDatePicker;
            datePicker.SetSelectionMode(datePicker, (EnumDatePickerType)e.NewValue);
        }
        #endregion

        #region IsShowShortCuts 是否显示快捷操作菜单
        /// <summary>
        /// 是否显示快捷操作菜单
        /// </summary>
        public bool IsShowShortCuts
        {
            get
            {
                return (bool)GetValue(IsShowShortCutsProperty);
            }
            set
            {
                SetValue(IsShowShortCutsProperty, value);
            }
        }

        public static readonly DependencyProperty IsShowShortCutsProperty =
            DependencyProperty.Register("IsShowShortCuts", typeof(bool), typeof(RangeDatePicker), new PropertyMetadata(false));
        #endregion

        #region SelectedDates
        public ObservableCollection<DateTime> SelectedDates
        {
            get
            {
                return (ObservableCollection<DateTime>)GetValue(SelectedDatesProperty);
            }
            set
            {
                SetValue(SelectedDatesProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedDatesProperty =
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(RangeDatePicker), new PropertyMetadata(null));

        #endregion

        #region SelectedDateStart

        public DateTime? SelectedDateStart
        {
            get
            {
                return (DateTime?)GetValue(SelectedDateStartProperty);
            }
            set
            {
                SetValue(SelectedDateStartProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedDateStartProperty =
            DependencyProperty.Register("SelectedDateStart", typeof(DateTime?), typeof(RangeDatePicker), new PropertyMetadata(null));

        #endregion

        #region SelectedDateEnd

        public DateTime? SelectedDateEnd
        {
            get
            {
                return (DateTime?)GetValue(SelectedDateEndProperty);
            }
            set
            {
                SetValue(SelectedDateEndProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedDateEndProperty =
            DependencyProperty.Register("SelectedDateEnd", typeof(DateTime?), typeof(RangeDatePicker), new PropertyMetadata(null, SelectedDateEndCallback));


        private static void SelectedDateEndCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeDatePicker datePicker = d as RangeDatePicker;
            DateTime? dateTime = (DateTime?)e.NewValue;
            datePicker.SetDateToTextBox(datePicker.SelectedDateStart, dateTime);
        }

        #endregion       

        #region DisplayDateStart

        public DateTime DisplayDateStart
        {
            get
            {
                return (DateTime)GetValue(DisplayDateStartProperty);
            }
            set
            {
                SetValue(DisplayDateStartProperty, value);
            }
        }

        public static readonly DependencyProperty DisplayDateStartProperty =
            DependencyProperty.Register("DisplayDateStart", typeof(DateTime), typeof(RangeDatePicker));

        #endregion

        #region DisplayDateEnd

        public DateTime DisplayDateEnd
        {
            get
            {
                return (DateTime)GetValue(DisplayDateEndProperty);
            }
            set
            {
                SetValue(DisplayDateEndProperty, value);
            }
        }

        public static readonly DependencyProperty DisplayDateEndProperty =
            DependencyProperty.Register("DisplayDateEnd", typeof(DateTime), typeof(RangeDatePicker));

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
            DependencyProperty.Register("DateTimeFormat", typeof(string), typeof(RangeDatePicker), new PropertyMetadata("yyyy-MM-dd"));

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
            DependencyProperty.Register("IsShowConfirm", typeof(bool), typeof(RangeDatePicker), new PropertyMetadata(false));

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
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(RangeDatePicker), new PropertyMetadata(false));

        #endregion

        #endregion

        #region Constructors
        static RangeDatePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeDatePicker), new FrameworkPropertyMetadata(typeof(RangeDatePicker)));
        }

        public RangeDatePicker()
        {
            this.SelectedDates = new ObservableCollection<DateTime>();
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

            if (this._calendar_Second != null)
            {
                this._calendar_Second.DateClick -= PART_Calendar_Second_DateClick;
            }

            if (this._timeSelector != null)
            {
                this._timeSelector.SelectedTimeChanged -= PART_TimeSelector_SelectedTimeChanged;
            }

            if (this._timeSelector_Second != null)
            {
                this._timeSelector_Second.SelectedTimeChanged -= PART_TimeSelector_Second_SelectedTimeChanged;
            }

            if (this._btn_RecentlyAWeek != null)
            {
                this._btn_RecentlyAWeek.Click -= PART_Btn_RecentlyAWeek_Click;
            }
            if (this._btn_RecentlyAMonth != null)
            {
                this._btn_RecentlyAMonth.Click -= PART_Btn_RecentlyAMonth_Click;
            }
            if (this._btn_RecentlyThreeMonth != null)
            {
                this._btn_RecentlyThreeMonth.Click -= PART_Btn_RecentlyThreeMonth_Click;
            }

            if (this._confirmSelected != null)
            {
                this._confirmSelected.Click -= _confirmSelected_Click;
            }

            if (this._clearDate != null)
            {
                this._clearDate.Click -= PART_ClearDate_Click;
            }

            this._popup_New = GetTemplateChild(PART_Popup_New) as Popup;
            this._popup_TimeSelector = GetTemplateChild(PART_Popup_TimeSelector) as Popup;
            this._calendar = GetTemplateChild(PART_Calendar) as ACalendar;
            this._calendar_Second = GetTemplateChild(PART_Calendar_Second) as ACalendar;
            this._timeSelector = GetTemplateChild(PART_TimeSelector) as ListClock;
            this._timeSelector_Second = GetTemplateChild(PART_TimeSelector_Second) as ListClock;
            this._textBox_New = GetTemplateChild(PART_TextBox_New) as TextBox;
            this._textBox_Display = GetTemplateChild(PART_TextBox_Display) as TextBox;
            this._btn_RecentlyAWeek = GetTemplateChild(PART_Btn_RecentlyAWeek) as Button;
            this._btn_RecentlyAMonth = GetTemplateChild(PART_Btn_RecentlyAMonth) as Button;
            this._btn_RecentlyThreeMonth = GetTemplateChild(PART_Btn_RecentlyThreeMonth) as Button;
            this._confirmSelected = GetTemplateChild(PART_ConfirmSelected) as Button;
            this._clearDate = GetTemplateChild(PART_ClearDate) as Button;

            if (this._popup_New != null)
            {
                this._popup_New.Opened += PART_Popup_New_Opened;
            }

            if (this._calendar != null)
            {
                this._calendar.DisplayMode = CalendarMode.Month;
                this._calendar.DisplayDate = new DateTime(this.DisplayDateStart.Year, this.DisplayDateStart.Month, this.DisplayDateStart.Day);

                this._calendar.DateClick += PART_Calendar_DateClick;
            }

            if (this._calendar_Second != null)
            {
                this._calendar_Second.DisplayMode = CalendarMode.Month;

                if (this.DisplayDateEnd == DateTime.MinValue)
                {
                    if (this.DisplayDateStart == DateTime.MinValue)
                    {
                        this._calendar_Second.DisplayDate = DateTime.Today.AddMonths(1);
                    }
                    else
                    {
                        this._calendar_Second.DisplayDate = this.DisplayDateStart.AddMonths(1);
                    }
                }
                else
                {
                    this._calendar_Second.DisplayDate = new DateTime(this.DisplayDateEnd.Year, this.DisplayDateEnd.Month, this.DisplayDateEnd.Day);
                }

                this._calendar_Second.DateClick += PART_Calendar_Second_DateClick;
            }

            if (this._timeSelector != null)
            {
                this._timeSelector.SelectedTimeChanged += PART_TimeSelector_SelectedTimeChanged;
            }

            if (this._timeSelector_Second != null)
            {
                this._timeSelector_Second.SelectedTimeChanged += PART_TimeSelector_Second_SelectedTimeChanged;
            }

            if (this._btn_RecentlyAWeek != null)
            {
                this._btn_RecentlyAWeek.Click += PART_Btn_RecentlyAWeek_Click;
            }
            if (this._btn_RecentlyAMonth != null)
            {
                this._btn_RecentlyAMonth.Click += PART_Btn_RecentlyAMonth_Click;
            }
            if (this._btn_RecentlyThreeMonth != null)
            {
                this._btn_RecentlyThreeMonth.Click += PART_Btn_RecentlyThreeMonth_Click;
            }

            if (this._confirmSelected != null)
            {
                this._confirmSelected.Click += _confirmSelected_Click;
            }

            if (this._clearDate != null)
            {
                this._clearDate.Click += PART_ClearDate_Click;
            }

            this.SetSelectionMode(this, this.Type);
            this.SetDateToTextBox(SelectedDateStart, SelectedDateEnd);
        }

        private void PART_TimeSelector_SelectedTimeChanged(object sender, Event.FunctionEventArgs<DateTime?> e)
        {
            UpdateDisplayTime(0, true);
        }

        private void PART_TimeSelector_Second_SelectedTimeChanged(object sender, Event.FunctionEventArgs<DateTime?> e)
        {
            UpdateDisplayTime(1, true);
        }
        #endregion

        #region Private方法
        /// <summary>
        /// 日期点击处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_Calendar_DateClick(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            UpdateDisplayTime(0);
        }

        /// <summary>
        /// 双日历模式下的第二个日历的日期点击处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_Calendar_Second_DateClick(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            UpdateDisplayTime(1);

            if (!IsShowConfirm)
            {
                //选了两个日期之后，关闭日期选择框
                this.IsDropDownOpen = false;
            }
        }

        private void UpdateDisplayTime(int index, bool autodate = false)
        {
            if (index == 0)
            {
                if (_calendar.SelectedDate == null && autodate == true)
                {
                    _calendar.SelectedDate = DateTime.Today;
                }
            }
            else
            {
                if (_calendar_Second.SelectedDate == null && autodate == true)
                {
                    _calendar_Second.SelectedDate = DateTime.Today;
                }
            }

            if (_calendar.SelectedDate != null)
            {
                var date = _calendar.SelectedDate.Value;
                var time = _timeSelector.DisplayTime;

                var result = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                DisplayDateStart = result;
            }

            if (_calendar_Second.SelectedDate != null)
            {
                var date = _calendar_Second.SelectedDate.Value;
                var time = _timeSelector_Second.DisplayTime;

                var result = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                DisplayDateEnd = result;
            }

            SetDateToDisplayTextBox(DisplayDateStart, DisplayDateEnd);
            SetSelectedDates(DisplayDateStart, DisplayDateEnd);
            if (!IsShowConfirm)
            {
                SelectedDateStart = DisplayDateStart;
                SelectedDateEnd = DisplayDateEnd;
            }
        }


        private void SetSelectedDate()
        {
            if (this._calendar != null)
            {
                this._calendar.SelectedDate = this.SelectedDateStart;
            }
            if (this._timeSelector != null)
            {
                this._timeSelector.SelectedTime = this.SelectedDateStart;
            }
            if (this._calendar_Second != null)
            {
                this._calendar_Second.SelectedDate = this.SelectedDateEnd;
            }
            if (this._timeSelector_Second != null)
            {
                this._timeSelector_Second.SelectedTime = this.SelectedDateEnd;
            }

            if (SelectedDateStart != null)
            {
                DisplayDateStart = SelectedDateStart.Value;
            }
            else
            {
                DisplayDateStart = DateTime.MinValue;
            }

            if (SelectedDateEnd != null)
            {
                DisplayDateEnd = SelectedDateEnd.Value;
            }
            else
            {
                DisplayDateEnd = DateTime.MinValue;
            }

            SetDateToDisplayTextBox(SelectedDateStart, SelectedDateEnd);
            SetSelectedDates(SelectedDateStart, SelectedDateEnd);
        }

        private void PART_ClearDate_Click(object sender, RoutedEventArgs e)
        {
            if (this._textBox_New != null)
            {
                this._textBox_New.Text = string.Empty;
            }
            SelectedDateStart = null;
            SelectedDateEnd = null;
            this.ClearSelectedDates();
        }

        private void _confirmSelected_Click(object sender, RoutedEventArgs e)
        {
            SelectedDateStart = DisplayDateStart;
            SelectedDateEnd = DisplayDateEnd;
            this.IsDropDownOpen = false;
        }

        private void HandleSelectedDatesChanged()
        {
            RangeDatePicker datePicker = this;
            if (datePicker._calendar != null)
            {
                datePicker._calendar.SelectedDates = new ObservableCollection<DateTime>(datePicker.SelectedDates);
            }
            if (datePicker._calendar_Second != null)
            {
                datePicker._calendar_Second.SelectedDates = new ObservableCollection<DateTime>(datePicker.SelectedDates);
            }
        }


        private void PART_Popup_New_Opened(object sender, EventArgs e)
        {
            if (this._calendar == null)
            {
                return;
            }

            this._calendar.DisplayMode = CalendarMode.Month;
            this._calendar_Second.DisplayMode = CalendarMode.Month;
            this.SetSelectedDate();
        }


        #region 快捷菜单点击事件     
        /// <summary>
        /// 点击了“最近一周”快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_Btn_RecentlyAWeek_Click(object sender, RoutedEventArgs e)
        {
            this.ClearSelectedDates();
            this.FastSetSelectedDates(DateTime.Today.AddDays(-7), DateTime.Today);
        }

        /// <summary>
        /// 点击了“最近一个月”快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_Btn_RecentlyAMonth_Click(object sender, RoutedEventArgs e)
        {
            this.ClearSelectedDates();
            this.FastSetSelectedDates(DateTime.Today.AddMonths(-1), DateTime.Today);
        }

        /// <summary>
        /// 点击了“最近三个月”快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_Btn_RecentlyThreeMonth_Click(object sender, RoutedEventArgs e)
        {
            this.ClearSelectedDates();
            this.FastSetSelectedDates(DateTime.Today.AddMonths(-3), DateTime.Today);
        }
        #endregion

        private void FastSetSelectedDates(DateTime? startDate, DateTime? endDate)
        {
            if (this._calendar == null || this._calendar_Second == null)
            {
                return;
            }

            this.SelectedDateStart = startDate;
            this.SelectedDateEnd = endDate;
            this._calendar_Second.SelectedDate = null;
            this._calendar.SelectedDate = null;

            this._calendar.DisplayDate = new DateTime(startDate.Value.Date.Year, startDate.Value.Date.Month, 1);
            this._calendar_Second.DisplayDate = new DateTime(endDate.Value.Date.Year, endDate.Value.Date.Month, 1);

            this.SetSelectedDates(this.SelectedDateStart, this.SelectedDateEnd);
        }

        /// <summary>
        /// 根据起始日期与结束日期，计算总共的选中日期
        /// </summary>
        /// <param name="selectedDateStart"></param>
        /// <param name="selectedDateEnd"></param>
        private void SetSelectedDates(DateTime? selectedDateStart, DateTime? selectedDateEnd)
        {
            this.SelectedDates.Clear();

            if (selectedDateStart != null && selectedDateStart != DateTime.MinValue && selectedDateEnd != null && selectedDateEnd != DateTime.MinValue)
            {
                DateTime? dtTemp = selectedDateStart;
                while (dtTemp <= selectedDateEnd)
                {
                    this.SelectedDates.Add(dtTemp.Value.Date);
                    dtTemp = dtTemp.Value.Date.AddDays(1);
                }
            }
            this.HandleSelectedDatesChanged();
        }

        /// <summary>
        /// 将当前选择的日期段显示到文本框中
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        private void SetDateToTextBox(DateTime? startDate, DateTime? endDate)
        {
            if (this._textBox_New != null)
            {
                if (startDate == DateTime.MinValue)
                {
                    startDate = null;
                }
                if (endDate == DateTime.MinValue)
                {
                    endDate = null;
                }

                if (startDate != null || endDate != null)
                {
                    this._textBox_New.Text = (startDate != null ? startDate.Value.ToString(this.DateTimeFormat) : "          ")
                                    + " ~ " +
                                  (endDate != null ? endDate.Value.ToString(this.DateTimeFormat) : "          ");
                }
                else
                {
                    this._textBox_New.Text = "";
                }
            }
        }

        /// <summary>
        /// 将当前选择的日期显示到文本框中
        /// </summary>
        /// <param name="selectedDate"></param>
        private void SetDateToDisplayTextBox(DateTime? startDate, DateTime? endDate)
        {
            if (this._textBox_Display != null)
            {
                if (startDate == DateTime.MinValue)
                {
                    startDate = null;
                }
                if (endDate == DateTime.MinValue)
                {
                    endDate = null;
                }

                if (startDate != null || endDate != null)
                {
                    this._textBox_Display.Text = (startDate != null ? startDate.Value.ToString(this.DateTimeFormat) : "          ")
                                    + " ~ " +
                                  (endDate != null ? endDate.Value.ToString(this.DateTimeFormat) : "          ");
                }
                else
                {
                    this._textBox_Display.Text = "";
                }
            }
        }

        /// <summary>
        /// 设置控件的类型
        /// </summary>
        /// <param name="datePicker"></param>
        /// <param name="type"></param>
        private void SetSelectionMode(RangeDatePicker datePicker, EnumDatePickerType type)
        {
            switch (type)
            {
                case EnumDatePickerType.DateRange:
                    if (datePicker._calendar != null)
                    {
                        datePicker._calendar.SelectionMode = CalendarSelectionMode.SingleRange;
                    }
                    if (datePicker._calendar_Second != null)
                    {
                        datePicker._calendar_Second.SelectionMode = CalendarSelectionMode.SingleRange;
                    }
                    break;
                case EnumDatePickerType.DateTimeRange:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 清除已选择的日期
        /// </summary>
        private void ClearSelectedDates()
        {
            this.SelectedDates.Clear();
            this.SelectedDateStart = null;
            this.SelectedDateEnd = null;

            this.IsDropDownOpen = false;
        }
        #endregion
    }

}