﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using AIStudio.Wpf.Controls.Event;

namespace AIStudio.Wpf.Controls
{
    public class DateTimeUpDown : DateTimeUpDownBase<DateTime?>
    {
        #region Members

        private DateTime? _lastValidDate; //null
        private bool _setKindInternal = false;

        #endregion

        #region Properties

        #region Format

        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(DateTimeFormat), typeof(DateTimeUpDown), new UIPropertyMetadata(DateTimeFormat.FullDateTime, OnFormatChanged));
        public DateTimeFormat Format
        {
            get
            {
                return (DateTimeFormat)GetValue(FormatProperty);
            }
            set
            {
                SetValue(FormatProperty, value);
            }
        }

        private static void OnFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DateTimeUpDown dateTimeUpDown = o as DateTimeUpDown;
            if (dateTimeUpDown != null)
                dateTimeUpDown.OnFormatChanged((DateTimeFormat)e.OldValue, (DateTimeFormat)e.NewValue);
        }

        protected virtual void OnFormatChanged(DateTimeFormat oldValue, DateTimeFormat newValue)
        {
            FormatUpdated();
        }

        #endregion //Format

        #region FormatString

        public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register("FormatString", typeof(string), typeof(DateTimeUpDown), new UIPropertyMetadata(default(String), OnFormatStringChanged), IsFormatStringValid);
        public string FormatString
        {
            get
            {
                return (string)GetValue(FormatStringProperty);
            }
            set
            {
                SetValue(FormatStringProperty, value);
            }
        }

        internal static bool IsFormatStringValid(object value)
        {
            try
            {
                // Test the format string if it is used.
                DateTime.MinValue.ToString((string)value, CultureInfo.CurrentCulture);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static void OnFormatStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DateTimeUpDown dateTimeUpDown = o as DateTimeUpDown;
            if (dateTimeUpDown != null)
                dateTimeUpDown.OnFormatStringChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnFormatStringChanged(string oldValue, string newValue)
        {
            FormatUpdated();
        }

        #endregion //FormatString

        #region Kind

        public static readonly DependencyProperty KindProperty = DependencyProperty.Register("Kind", typeof(DateTimeKind), typeof(DateTimeUpDown),
          new FrameworkPropertyMetadata(DateTimeKind.Unspecified, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnKindChanged));
        public DateTimeKind Kind
        {
            get
            {
                return (DateTimeKind)GetValue(KindProperty);
            }
            set
            {
                SetValue(KindProperty, value);
            }
        }

        private static void OnKindChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DateTimeUpDown dateTimeUpDown = o as DateTimeUpDown;
            if (dateTimeUpDown != null)
                dateTimeUpDown.OnKindChanged((DateTimeKind)e.OldValue, (DateTimeKind)e.NewValue);
        }

        protected virtual void OnKindChanged(DateTimeKind oldValue, DateTimeKind newValue)
        {
            //Upate the value based on kind. (Postpone to EndInit if not yet initialized)
            if (!_setKindInternal
              && this.Value != null
              && this.IsInitialized)
            {
                this.Value = this.ConvertToKind(this.Value.Value, newValue);
            }
        }

        private void SetKindInternal(DateTimeKind kind)
        {
            _setKindInternal = true;
            try
            {
#if VS2008
        // Warning : Binding could be lost
        this.Kind = kind;
#else
                //We use SetCurrentValue to not erase the possible underlying 
                //OneWay Binding. (This will also update correctly any
                //possible TwoWay bindings).
                this.SetCurrentValue(DateTimeUpDown.KindProperty, kind);
#endif
            }
            finally
            {
                _setKindInternal = false;
            }
        }

        #endregion //Kind

        #region ContextNow (Private)

        internal DateTime ContextNow
        {
            get
            {
                return DateTimeUtilities.GetContextNow(this.Kind);
            }
        }

        #endregion

        #endregion //Properties

        #region Constructors

        static DateTimeUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimeUpDown), new FrameworkPropertyMetadata(typeof(DateTimeUpDown)));
            MaximumProperty.OverrideMetadata(typeof(DateTimeUpDown), new FrameworkPropertyMetadata(DateTime.MaxValue));
            MinimumProperty.OverrideMetadata(typeof(DateTimeUpDown), new FrameworkPropertyMetadata(DateTime.MinValue));
        }

        public DateTimeUpDown()
        {
            this.Loaded += this.DateTimeUpDown_Loaded;
        }

        #endregion //Constructors

        #region Base Class Overrides

        public override bool CommitInput()
        {
            bool isSyncValid = this.SyncTextAndValueProperties(true, Text);
            _lastValidDate = this.Value;
            return isSyncValid;
        }

        protected override void OnCultureInfoChanged(CultureInfo oldValue, CultureInfo newValue)
        {
            FormatUpdated();
        }

        protected override void OnIncrement()
        {
            if (this.IsCurrentValueValid())
            {
                this.Increment(this.Step);
            }
        }

        protected override void OnDecrement()
        {
            if (this.IsCurrentValueValid())
            {
                this.Increment(-this.Step);
            }
        }

        protected override void OnTextChanged(string previousValue, string currentValue)
        {
            if (!_processTextChanged)
                return;

            base.OnTextChanged(previousValue, currentValue);
        }

        protected override DateTime? ConvertTextToValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            DateTime result;
            this.TryParseDateTime(text, out result);

            //Do not force "unspecified" to a time-zone specific
            //parsed text value. This would result in a lost of precision and
            //corrupt data. Let the value impose the Kind to the
            //DateTimePicker. 
            if (this.Kind != DateTimeKind.Unspecified)
            {

                //Keep the current kind (Local or Utc) 
                //by imposing it to the parsed text value.
                //
                //Note: A parsed UTC text value may be
                //      adjusted with a Local kind and time.
                result = this.ConvertToKind(result, this.Kind);
            }

            if (this.ClipValueToMinMax)
            {
                return this.GetClippedMinMaxValue(result);
            }

            this.ValidateDefaultMinMax(result);

            return result;
        }

        protected override string ConvertValueToText()
        {
            if (Value == null)
                return string.Empty;

            return Value.Value.ToString(GetFormatString(Format), CultureInfo);
        }

        protected override void SetValidSpinDirection()
        {
            ValidSpinDirections validDirections = ValidSpinDirections.None;

            if (!IsReadOnly)
            {
                if (this.IsLowerThan(this.Value, this.Maximum) || !this.Value.HasValue || !this.Maximum.HasValue)
                    validDirections = validDirections | ValidSpinDirections.Increase;

                if (this.IsGreaterThan(this.Value, this.Minimum) || !this.Value.HasValue || !this.Minimum.HasValue)
                    validDirections = validDirections | ValidSpinDirections.Decrease;
            }

            if (this.Spinner != null)
                this.Spinner.ValidSpinDirection = validDirections;
        }

        protected override object OnCoerceValue(object newValue)
        {
            //Since only changing the "kind" of a date
            //Ex. "2001-01-01 12:00 AM, Kind=Utc" to "2001-01-01 12:00 AM Kind=Local"
            //by setting the "Value" property won't trigger a property changed,
            //but will call this callback (coerce), we update the Kind here.
            DateTime? value = (DateTime?)base.OnCoerceValue(newValue);

            //Let the initialized determine the final "kind" value.
            if (value != null && this.IsInitialized)
            {
                //Update kind based on value kind
                this.SetKindInternal(value.Value.Kind);
            }

            return value;
        }

        protected override void OnValueChanged(DateTime? oldValue, DateTime? newValue)
        {
            DateTimeInfo info = _selectedDateTimeInfo;

            //this only occurs when the user manually type in a value for the Value Property
            if (info == null)
                info = (this.CurrentDateTimePart != DateTimePart.Other) ? this.GetDateTimeInfo(this.CurrentDateTimePart) : _dateTimeInfoList[0];

            //whenever the value changes we need to parse out the value into out DateTimeInfo segments so we can keep track of the individual pieces
            //but only if it is not null
            if (newValue != null)
                ParseValueIntoDateTimeInfo(this.Value);

            base.OnValueChanged(oldValue, newValue);

            if (!_isTextChangedFromUI)
            {
                _lastValidDate = newValue;
            }

            if (TextBox != null)
            {
                //we loose our selection when the Value is set so we need to reselect it without firing the selection changed event
                _fireSelectionChangedEvent = false;
                TextBox.Select(info.StartPosition, info.Length);
                _fireSelectionChangedEvent = true;
            }
        }

        protected override bool IsCurrentValueValid()
        {
            DateTime result;

            if (string.IsNullOrEmpty(this.TextBox.Text))
                return true;

            return this.TryParseDateTime(this.TextBox.Text, out result);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (this.Value != null)
            {
                DateTimeKind valueKind = this.Value.Value.Kind;

                if (valueKind != this.Kind)
                {
                    //Conflit between "Kind" property and the "Value.Kind" value.
                    //Priority to the one that is not "Unspecified".
                    if (this.Kind == DateTimeKind.Unspecified)
                    {
                        this.SetKindInternal(valueKind);
                    }
                    else
                    {
                        this.Value = this.ConvertToKind(this.Value.Value, this.Kind);
                    }
                }
            }
        }

        protected override void PerformMouseSelection()
        {
            if (this.UpdateValueOnEnterKey)
            {
                this.ParseValueIntoDateTimeInfo(this.ConvertTextToValue(this.TextBox.Text));
            }
            base.PerformMouseSelection();
        }

        protected internal override void PerformKeyboardSelection(int nextSelectionStart)
        {
            if (this.UpdateValueOnEnterKey)
            {
                this.ParseValueIntoDateTimeInfo(this.ConvertTextToValue(this.TextBox.Text));
            }
            base.PerformKeyboardSelection(nextSelectionStart);
        }

        protected override void InitializeDateTimeInfoList(DateTime? value)
        {
            _dateTimeInfoList.Clear();
            _selectedDateTimeInfo = null;

            string format = GetFormatString(Format);

            if (string.IsNullOrEmpty(format))
                return;

            while (format.Length > 0)
            {
                int elementLength = GetElementLengthByFormat(format);
                DateTimeInfo info = null;

                switch (format[0])
                {
                    case '"':
                    case '\'':
                        {
                            int closingQuotePosition = format.IndexOf(format[0], 1);
                            info = new DateTimeInfo
                            {
                                IsReadOnly = true,
                                Type = DateTimePart.Other,
                                Length = 1,
                                Content = format.Substring(1, Math.Max(1, closingQuotePosition - 1))
                            };
                            elementLength = Math.Max(1, closingQuotePosition + 1);
                            break;
                        }
                    case 'D':
                    case 'd':
                        {
                            string d = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                d = "%" + d;

                            if (elementLength > 2)
                                info = new DateTimeInfo
                                {
                                    IsReadOnly = true,
                                    Type = DateTimePart.DayName,
                                    Length = elementLength,
                                    Format = d
                                };
                            else
                                info = new DateTimeInfo
                                {
                                    IsReadOnly = false,
                                    Type = DateTimePart.Day,
                                    Length = elementLength,
                                    Format = d
                                };
                            break;
                        }
                    case 'F':
                    case 'f':
                        {
                            string f = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                f = "%" + f;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = false,
                                Type = DateTimePart.Millisecond,
                                Length = elementLength,
                                Format = f
                            };
                            break;
                        }
                    case 'h':
                        {
                            string h = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                h = "%" + h;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = false,
                                Type = DateTimePart.Hour12,
                                Length = elementLength,
                                Format = h
                            };
                            break;
                        }
                    case 'H':
                        {
                            string H = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                H = "%" + H;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = false,
                                Type = DateTimePart.Hour24,
                                Length = elementLength,
                                Format = H
                            };
                            break;
                        }
                    case 'M':
                        {
                            string M = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                M = "%" + M;

                            if (elementLength >= 3)
                                info = new DateTimeInfo
                                {
                                    IsReadOnly = false,
                                    Type = DateTimePart.MonthName,
                                    Length = elementLength,
                                    Format = M
                                };
                            else
                                info = new DateTimeInfo
                                {
                                    IsReadOnly = false,
                                    Type = DateTimePart.Month,
                                    Length = elementLength,
                                    Format = M
                                };
                            break;
                        }
                    case 'S':
                    case 's':
                        {
                            string s = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                s = "%" + s;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = false,
                                Type = DateTimePart.Second,
                                Length = elementLength,
                                Format = s
                            };
                            break;
                        }
                    case 'T':
                    case 't':
                        {
                            string t = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                t = "%" + t;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = false,
                                Type = DateTimePart.AmPmDesignator,
                                Length = elementLength,
                                Format = t
                            };
                            break;
                        }
                    case 'Y':
                    case 'y':
                        {
                            string y = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                y = "%" + y;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = false,
                                Type = DateTimePart.Year,
                                Length = elementLength,
                                Format = y
                            };
                            break;
                        }
                    case '\\':
                        {
                            if (format.Length >= 2)
                            {
                                info = new DateTimeInfo
                                {
                                    IsReadOnly = true,
                                    Content = format.Substring(1, 1),
                                    Length = 1,
                                    Type = DateTimePart.Other
                                };
                                elementLength = 2;
                            }
                            break;
                        }
                    case 'g':
                        {
                            string g = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                g = "%" + g;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = true,
                                Type = DateTimePart.Period,
                                Length = elementLength,
                                Format = format.Substring(0, elementLength)
                            };
                            break;
                        }
                    case 'm':
                        {
                            string m = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                m = "%" + m;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = false,
                                Type = DateTimePart.Minute,
                                Length = elementLength,
                                Format = m
                            };
                            break;
                        }
                    case 'z':
                        {
                            string z = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                z = "%" + z;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = true,
                                Type = DateTimePart.TimeZone,
                                Length = elementLength,
                                Format = z
                            };
                            break;
                        }
                    default:
                        {
                            elementLength = 1;
                            info = new DateTimeInfo
                            {
                                IsReadOnly = true,
                                Length = 1,
                                Content = format[0].ToString(),
                                Type = DateTimePart.Other
                            };
                            break;
                        }
                }

                _dateTimeInfoList.Add(info);
                format = format.Substring(elementLength);
            }
        }

        protected override bool IsLowerThan(DateTime? value1, DateTime? value2)
        {
            if (value1 == null || value2 == null)
                return false;

            return (value1.Value < value2.Value);
        }

        protected override bool IsGreaterThan(DateTime? value1, DateTime? value2)
        {
            if (value1 == null || value2 == null)
                return false;

            return (value1.Value > value2.Value);
        }

        protected override void OnUpdateValueOnEnterKeyChanged(bool oldValue, bool newValue)
        {
            throw new NotSupportedException("DateTimeUpDown controls do not support modifying UpdateValueOnEnterKey property.");
        }

        #endregion //Base Class Overrides

        #region Methods

        public void SelectAll()
        {
            _fireSelectionChangedEvent = false;
            TextBox.SelectAll();
            _fireSelectionChangedEvent = true;
        }

        private void FormatUpdated()
        {
            InitializeDateTimeInfoList(this.Value);
            if (Value != null)
                ParseValueIntoDateTimeInfo(this.Value);

            // Update the Text representation of the value.
            _processTextChanged = false;

            this.SyncTextAndValueProperties(false, null);

            _processTextChanged = true;

        }

        private static int GetElementLengthByFormat(string format)
        {
            for (int i = 1; i < format.Length; i++)
            {
                if (String.Compare(format[i].ToString(), format[0].ToString(), false) != 0)
                {
                    return i;
                }
            }
            return format.Length;
        }

        private void Increment(int step)
        {
            // if UpdateValueOnEnterKey is true, 
            // Sync Value on Text only when Enter Key is pressed.
            if (this.UpdateValueOnEnterKey)
            {
                _fireSelectionChangedEvent = false;

                var currentValue = this.ConvertTextToValue(this.TextBox.Text);
                if (currentValue.HasValue)
                {
                    var newValue = this.UpdateDateTime(currentValue, step);
                    this.TextBox.Text = newValue.Value.ToString(this.GetFormatString(this.Format), this.CultureInfo);
                }
                else
                {
                    this.TextBox.Text = (this.DefaultValue != null)
                                        ? this.DefaultValue.Value.ToString(this.GetFormatString(this.Format), this.CultureInfo)
                                        : this.ContextNow.ToString(this.GetFormatString(this.Format), this.CultureInfo);
                }

                if (this.TextBox != null)
                {
                    DateTimeInfo info = _selectedDateTimeInfo;
                    //this only occurs when the user manually type in a value for the Value Property
                    if (info == null)
                        info = (this.CurrentDateTimePart != DateTimePart.Other) ? this.GetDateTimeInfo(this.CurrentDateTimePart) : _dateTimeInfoList[0];

                    //whenever the value changes we need to parse out the value into out DateTimeInfo segments so we can keep track of the individual pieces
                    this.ParseValueIntoDateTimeInfo(this.ConvertTextToValue(this.TextBox.Text));

                    //we loose our selection when the Value is set so we need to reselect it without firing the selection changed event
                    this.TextBox.Select(info.StartPosition, info.Length);
                }
                _fireSelectionChangedEvent = true;
            }
            else
            {
                if (this.Value.HasValue)
                {
                    this.Value = this.UpdateDateTime(this.Value, step);
                }
                else
                {
                    this.Value = this.DefaultValue ?? this.ContextNow;
                }
            }
        }

        private void ParseValueIntoDateTimeInfo(DateTime? newDate)
        {
            string text = string.Empty;

            _dateTimeInfoList.ForEach(info =>
            {
                if (info.Format == null)
                {
                    info.StartPosition = text.Length;
                    info.Length = info.Content.Length;
                    text += info.Content;
                }
                else if (newDate != null)
                {
                    DateTime date = newDate.Value;
                    info.StartPosition = text.Length;
                    info.Content = date.ToString(info.Format, CultureInfo.DateTimeFormat);
                    info.Length = info.Content.Length;
                    text += info.Content;
                }
            });
        }

        internal string GetFormatString(DateTimeFormat dateTimeFormat)
        {
            switch (dateTimeFormat)
            {
                case DateTimeFormat.ShortDate:
                    return CultureInfo.DateTimeFormat.ShortDatePattern;
                case DateTimeFormat.LongDate:
                    return CultureInfo.DateTimeFormat.LongDatePattern;
                case DateTimeFormat.ShortTime:
                    return CultureInfo.DateTimeFormat.ShortTimePattern;
                case DateTimeFormat.LongTime:
                    return CultureInfo.DateTimeFormat.LongTimePattern;
                case DateTimeFormat.FullDateTime:
                    return CultureInfo.DateTimeFormat.FullDateTimePattern;
                case DateTimeFormat.MonthDay:
                    return CultureInfo.DateTimeFormat.MonthDayPattern;
                case DateTimeFormat.RFC1123:
                    return CultureInfo.DateTimeFormat.RFC1123Pattern;
                case DateTimeFormat.SortableDateTime:
                    return CultureInfo.DateTimeFormat.SortableDateTimePattern;
                case DateTimeFormat.UniversalSortableDateTime:
                    return CultureInfo.DateTimeFormat.UniversalSortableDateTimePattern;
                case DateTimeFormat.YearMonth:
                    return CultureInfo.DateTimeFormat.YearMonthPattern;
                case DateTimeFormat.Custom:
                    {
                        switch (this.FormatString)
                        {
                            case "d":
                                return CultureInfo.DateTimeFormat.ShortDatePattern;
                            case "t":
                                return CultureInfo.DateTimeFormat.ShortTimePattern;
                            case "T":
                                return CultureInfo.DateTimeFormat.LongTimePattern;
                            case "D":
                                return CultureInfo.DateTimeFormat.LongDatePattern;
                            case "f":
                                return CultureInfo.DateTimeFormat.LongDatePattern + " " + CultureInfo.DateTimeFormat.ShortTimePattern;
                            case "F":
                                return CultureInfo.DateTimeFormat.FullDateTimePattern;
                            case "g":
                                return CultureInfo.DateTimeFormat.ShortDatePattern + " " + CultureInfo.DateTimeFormat.ShortTimePattern;
                            case "G":
                                return CultureInfo.DateTimeFormat.ShortDatePattern + " " + CultureInfo.DateTimeFormat.LongTimePattern;
                            case "m":
                                return CultureInfo.DateTimeFormat.MonthDayPattern;
                            case "y":
                                return CultureInfo.DateTimeFormat.YearMonthPattern;
                            case "r":
                                return CultureInfo.DateTimeFormat.RFC1123Pattern;
                            case "s":
                                return CultureInfo.DateTimeFormat.SortableDateTimePattern;
                            case "u":
                                return CultureInfo.DateTimeFormat.UniversalSortableDateTimePattern;
                            default:
                                return FormatString;
                        }
                    }
                default:
                    throw new ArgumentException("Not a supported format");
            }
        }

        private DateTime? UpdateDateTime(DateTime? currentDateTime, int value)
        {
            DateTimeInfo info = _selectedDateTimeInfo;

            //this only occurs when the user manually type in a value for the Value Property
            if (info == null)
                info = (this.CurrentDateTimePart != DateTimePart.Other) ? this.GetDateTimeInfo(this.CurrentDateTimePart) : _dateTimeInfoList[0];

            DateTime? result = null;

            try
            {
                switch (info.Type)
                {
                    case DateTimePart.Year:
                        {
                            result = ((DateTime)currentDateTime).AddYears(value);
                            break;
                        }
                    case DateTimePart.Month:
                    case DateTimePart.MonthName:
                        {
                            result = ((DateTime)currentDateTime).AddMonths(value);
                            break;
                        }
                    case DateTimePart.Day:
                    case DateTimePart.DayName:
                        {
                            result = ((DateTime)currentDateTime).AddDays(value);
                            break;
                        }
                    case DateTimePart.Hour12:
                    case DateTimePart.Hour24:
                        {
                            result = ((DateTime)currentDateTime).AddHours(value);
                            break;
                        }
                    case DateTimePart.Minute:
                        {
                            result = ((DateTime)currentDateTime).AddMinutes(value);
                            break;
                        }
                    case DateTimePart.Second:
                        {
                            result = ((DateTime)currentDateTime).AddSeconds(value);
                            break;
                        }
                    case DateTimePart.Millisecond:
                        {
                            result = ((DateTime)currentDateTime).AddMilliseconds(value);
                            break;
                        }
                    case DateTimePart.AmPmDesignator:
                        {
                            result = ((DateTime)currentDateTime).AddHours(value * 12);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch
            {
                //this can occur if the date/time = 1/1/0001 12:00:00 AM which is the smallest date allowed.
                //I could write code that would validate the date each and everytime but I think that it would be more
                //efficient if I just handle the edge case and allow an exeption to occur and swallow it instead.
            }

            return this.CoerceValueMinMax(result);
        }

        private bool TryParseDateTime(string text, out DateTime result)
        {
            bool isValid = false;
            result = this.ContextNow;

            DateTime current = this.ContextNow;
            try
            {
                current = (this.Value.HasValue)
                            ? this.Value.Value
                            : DateTime.Parse(this.ContextNow.ToString(), this.CultureInfo.DateTimeFormat);

                isValid = DateTimeParser.TryParse(text, this.GetFormatString(Format), current, this.CultureInfo, out result);
            }
            catch (FormatException)
            {
                isValid = false;
            }

            if (!isValid)
            {
                isValid = DateTime.TryParseExact(text, this.GetFormatString(this.Format), this.CultureInfo, DateTimeStyles.None, out result);
            }

            if (!isValid)
            {
                result = (_lastValidDate != null) ? _lastValidDate.Value : current;
            }

            return isValid;
        }

        private DateTime ConvertToKind(DateTime dateTime, DateTimeKind kind)
        {
            //Same kind, just return same value.
            if (kind == dateTime.Kind)
                return dateTime;

            //"ToLocalTime()" from an unspecified will assume
            // That the time was originaly Utc and affect the datetime value. 
            // Just "Force" the "Kind" instead.
            if (dateTime.Kind == DateTimeKind.Unspecified
              || kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(dateTime, kind);

            return (kind == DateTimeKind.Local)
               ? dateTime.ToLocalTime()
               : dateTime.ToUniversalTime();
        }

        #endregion //Methods

        #region Event Handlers

        private void DateTimeUpDown_Loaded(object sender, RoutedEventArgs e)
        {
            if ((this.Format == DateTimeFormat.Custom) && (string.IsNullOrEmpty(this.FormatString)))
            {
                throw new InvalidOperationException("A FormatString is necessary when Format is set to Custom.");
            }
        }

        #endregion
    }

    public enum DateTimeFormat
    {
        Custom,
        FullDateTime,
        LongDate,
        LongTime,
        MonthDay,
        RFC1123,
        ShortDate,
        ShortTime,
        SortableDateTime,
        UniversalSortableDateTime,
        YearMonth
    }

    internal class DateTimeParser
    {
        public static bool TryParse(string value, string format, DateTime currentDate, CultureInfo cultureInfo, out DateTime result)
        {
            bool success = false;
            result = currentDate;

            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(format))
                return false;

            var dateTimeString = ComputeDateTimeString(value, format, currentDate, cultureInfo).Trim();

            if (!String.IsNullOrEmpty(dateTimeString))
                success = DateTime.TryParse(dateTimeString, cultureInfo.DateTimeFormat, DateTimeStyles.None, out result);

            if (!success)
                result = currentDate;

            return success;
        }

        private static string ComputeDateTimeString(string dateTime, string format, DateTime currentDate, CultureInfo cultureInfo)
        {
            Dictionary<string, string> dateParts = GetDateParts(currentDate, cultureInfo);
            string[] timeParts = new string[3] { currentDate.Hour.ToString(), currentDate.Minute.ToString(), currentDate.Second.ToString() };
            string millisecondsPart = currentDate.Millisecond.ToString();
            string designator = "";
            string[] dateTimeSeparators = new string[] { ",", " ", "-", ".", "/", cultureInfo.DateTimeFormat.DateSeparator, cultureInfo.DateTimeFormat.TimeSeparator };

            UpdateSortableDateTimeString(ref dateTime, ref format, cultureInfo);

            var dateTimeParts = new List<string>();
            var formats = new List<string>();
            var isContainingDateTimeSeparators = dateTimeSeparators.Any(s => dateTime.Contains(s));
            if (isContainingDateTimeSeparators)
            {
                dateTimeParts = dateTime.Split(dateTimeSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
                formats = format.Split(dateTimeSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            else
            {
                string currentformat = "";
                string currentString = "";
                var formatArray = format.ToCharArray();
                for (int i = 0; i < formatArray.Count(); ++i)
                {
                    var c = formatArray[i];
                    if (!currentformat.Contains(c))
                    {
                        if (!string.IsNullOrEmpty(currentformat))
                        {
                            formats.Add(currentformat);
                            dateTimeParts.Add(currentString);
                        }
                        currentformat = c.ToString();
                        currentString = (i < dateTime.Length) ? dateTime[i].ToString() : "";
                    }
                    else
                    {
                        currentformat = string.Concat(currentformat, c);
                        currentString = string.Concat(currentString, (i < dateTime.Length) ? dateTime[i] : '\0');
                    }
                }
                if (!string.IsNullOrEmpty(currentformat))
                {
                    formats.Add(currentformat);
                    dateTimeParts.Add(currentString);
                }
            }

            //Auto-complete missing date parts
            if (dateTimeParts.Count < formats.Count)
            {
                while (dateTimeParts.Count != formats.Count)
                {
                    dateTimeParts.Add("0");
                }
            }

            //something went wrong
            if (dateTimeParts.Count != formats.Count)
                return string.Empty;

            for (int i = 0; i < formats.Count; i++)
            {
                var f = formats[i];
                if (!f.Contains("ddd") && !f.Contains("GMT"))
                {
                    if (f.Contains("M"))
                        dateParts["Month"] = dateTimeParts[i];
                    else if (f.Contains("d"))
                        dateParts["Day"] = dateTimeParts[i];
                    else if (f.Contains("y"))
                    {
                        dateParts["Year"] = dateTimeParts[i] != "0" ? dateTimeParts[i] : "0000";

                        if (dateParts["Year"].Length == 2)
                            dateParts["Year"] = string.Format("{0}{1}", currentDate.Year / 100, dateParts["Year"]);
                    }
                    else if (f.Contains("h") || f.Contains("H"))
                        timeParts[0] = dateTimeParts[i];
                    else if (f.Contains("m"))
                        timeParts[1] = dateTimeParts[i];
                    else if (f.Contains("s"))
                        timeParts[2] = dateTimeParts[i];
                    else if (f.Contains("f"))
                        millisecondsPart = dateTimeParts[i];
                    else if (f.Contains("t"))
                        designator = dateTimeParts[i];
                }
            }

            var date = string.Join(cultureInfo.DateTimeFormat.DateSeparator, dateParts.Select(x => x.Value).ToArray());
            var time = string.Join(cultureInfo.DateTimeFormat.TimeSeparator, timeParts);
            time += "." + millisecondsPart;

            return String.Format("{0} {1} {2}", date, time, designator);
        }

        private static void UpdateSortableDateTimeString(ref string dateTime, ref string format, CultureInfo cultureInfo)
        {
            if (format == cultureInfo.DateTimeFormat.SortableDateTimePattern)
            {
                format = format.Replace("'", "").Replace("T", " ");
                dateTime = dateTime.Replace("'", "").Replace("T", " ");
            }
            else if (format == cultureInfo.DateTimeFormat.UniversalSortableDateTimePattern)
            {
                format = format.Replace("'", "").Replace("Z", "");
                dateTime = dateTime.Replace("'", "").Replace("Z", "");
            }
        }

        private static Dictionary<string, string> GetDateParts(DateTime currentDate, CultureInfo cultureInfo)
        {
            Dictionary<string, string> dateParts = new Dictionary<string, string>();
            var dateTimeSeparators = new[] { ",", " ", "-", ".", "/", cultureInfo.DateTimeFormat.DateSeparator, cultureInfo.DateTimeFormat.TimeSeparator };
            var dateFormatParts = cultureInfo.DateTimeFormat.ShortDatePattern.Split(dateTimeSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
            dateFormatParts.ForEach(item => {
                string key = string.Empty;
                string value = string.Empty;

                if (item.Contains("M"))
                {
                    key = "Month";
                    value = currentDate.Month.ToString();
                }
                else if (item.Contains("d"))
                {
                    key = "Day";
                    value = currentDate.Day.ToString();
                }
                else if (item.Contains("y"))
                {
                    key = "Year";
                    value = currentDate.Year.ToString("D4");
                }
                if (!dateParts.ContainsKey(key))
                {
                    dateParts.Add(key, value);
                }
            });
            return dateParts;
        }
    }

    internal static class DateTimeUtilities
    {
        public static DateTime GetContextNow(DateTimeKind kind)
        {
            if (kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            return (kind == DateTimeKind.Utc)
              ? DateTime.UtcNow
              : DateTime.Now;
        }

        public static bool IsSameDate(DateTime? date1, DateTime? date2)
        {
            if (date1 == null || date2 == null)
                return false;

            return (date1.Value.Date == date2.Value.Date);
        }
    }
}
