using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_HourList, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_MinuteList, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_SecondList, Type = typeof(ListBox))]

    public class ListClock : ClockBase
    {
        #region Constants

        private const string PART_HourList = "PART_HourList";
        private const string PART_MinuteList = "PART_MinuteList";
        private const string PART_SecondList = "PART_SecondList";
        private const string PART_ClearDate = "PART_ClearDate";
        private const string PART_NowDate = "PART_NowDate";
        #endregion Constants

        #region ItemHeight
        public double ItemHeight
        {
            get
            {
                return (double)GetValue(ItemHeightProperty);
            }
            set
            {
                SetValue(ItemHeightProperty, value);
            }
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(ListClock), new PropertyMetadata(30.0));
        #endregion

        #region Fields

        #endregion

        #region Data

        private ListBox _hourList;

        private ListBox _minuteList;

        private ListBox _secondList;

        public ListClock()
        {
            this.Loaded += ListClock_Loaded;
        }

        private void ListClock_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        #endregion Data

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ButtonConfirm != null)
            {
                ButtonConfirm.Click -= ButtonConfirm_OnClick;
            }
            if (this._clearDate != null)
            {
                this._clearDate.Click -= PART_ClearDate_Click;
            }
            if (this._nowDate != null)
            {
                this._nowDate.Click -= PART_NowDate_Click;
            }

            if (this._hourList != null)
            {
                this._hourList.RemoveHandler(ListBoxItem.MouseLeftButtonDownEvent, new RoutedEventHandler(HourButton_Click));
            }

            if (this._minuteList != null)
            {
                this._minuteList.RemoveHandler(ListBoxItem.MouseLeftButtonDownEvent, new RoutedEventHandler(MinuteButton_Click));
            }

            if (this._secondList != null)
            {
                this._secondList.RemoveHandler(ListBoxItem.MouseLeftButtonDownEvent, new RoutedEventHandler(SecondButton_Click));
            }

            this._hourList = GetTemplateChild(PART_HourList) as ListBox;
            if (this._hourList != null)
            {
                this._hourList.AddHandler(ListBoxItem.MouseLeftButtonDownEvent, new RoutedEventHandler(HourButton_Click), true);
            }

            this._minuteList = GetTemplateChild(PART_MinuteList) as ListBox;
            if (this._minuteList != null)
            {
                this._minuteList.AddHandler(ListBoxItem.MouseLeftButtonDownEvent, new RoutedEventHandler(MinuteButton_Click), true);
            }

            this._secondList = GetTemplateChild(PART_SecondList) as ListBox;
            if (this._secondList != null)
            {
                this._secondList.AddHandler(ListBoxItem.MouseLeftButtonDownEvent, new RoutedEventHandler(SecondButton_Click), true);
            }

            ButtonConfirm = GetTemplateChild(PART_ButtonConfirm) as Button;
            if (ButtonConfirm != null)
            {
                ButtonConfirm.Click += ButtonConfirm_OnClick;
            }
            if (this._clearDate != null)
            {
                this._clearDate.Click += PART_ClearDate_Click;
            }
            if (this._nowDate != null)
            {
                this._nowDate.Click += PART_NowDate_Click;
            }

            if (SelectedTime.HasValue)
            {
                Update(SelectedTime.Value);
            }
            else
            {
                Update(DateTime.Now);
            }
        }

        private void Init()
        {
            if (this._hourList != null)
            {
                List<ListBoxItem> buttons = new List<ListBoxItem>();
                this.CreateItems(24, buttons);
                this.CreateExtraItem(buttons);
                buttons.ForEach(p => this._hourList.Items.Add(p));
            }

            if (this._minuteList != null)
            {
                List<ListBoxItem> buttons = new List<ListBoxItem>();
                this.CreateItems(60, buttons);
                this.CreateExtraItem(buttons);
                buttons.ForEach(p => this._minuteList.Items.Add(p));
            }

            if (this._secondList != null)
            {
                List<ListBoxItem> buttons = new List<ListBoxItem>();
                this.CreateItems(60, buttons);
                this.CreateExtraItem(buttons);
                buttons.ForEach(p => this._secondList.Items.Add(p));
            }

            if (SelectedTime.HasValue)
            {
                Update(SelectedTime.Value);
            }
            else
            {
                Update(DateTime.Now);
            }
        }

        private void HourButton_Click(object sender, RoutedEventArgs e)
        {
            this._hourList.AnimateScrollIntoView(this._hourList.SelectedIndex);
            Update();
        }


        private void MinuteButton_Click(object sender, RoutedEventArgs e)
        {
            this._minuteList.AnimateScrollIntoView(this._minuteList.SelectedIndex);
            Update();
        }

        private void SecondButton_Click(object sender, RoutedEventArgs e)
        {
            this._secondList.AnimateScrollIntoView(this._secondList.SelectedIndex);
            Update();
        }


        private void CreateItems(int itemsCount, List<ListBoxItem> list)
        {
            for (int i = 0; i < itemsCount; i++)
            {
                ListBoxItem timeButton = new ListBoxItem();
                timeButton.SetValue(ListBoxItem.HeightProperty, this.ItemHeight);
                timeButton.SetValue(ListBoxItem.DataContextProperty, i);
                timeButton.SetValue(ListBoxItem.ContentProperty, (i < 10) ? "0" + i : i.ToString());
                timeButton.SetValue(ListBoxItem.IsSelectedProperty, false);
                list.Add(timeButton);
            }
        }

        private void CreateExtraItem(List<ListBoxItem> list)
        {
            double height = double.IsNaN(this.Height) ? this.ActualHeight : this.Height;

            for (int i = 0; i < (height - this.ItemHeight) / this.ItemHeight; i++)
            {
                ListBoxItem timeButton = new ListBoxItem();
                timeButton.SetValue(ListBoxItem.HeightProperty, this.ItemHeight);
                timeButton.SetValue(ListBoxItem.IsEnabledProperty, false);
                timeButton.SetValue(ListBoxItem.IsSelectedProperty, false);
                list.Add(timeButton);
            }
        }

        private void Update()
        {
            if (_hourList.SelectedIndex >= 0 && _hourList.SelectedIndex < 24 &&
                _minuteList.SelectedIndex >= 0 && _minuteList.SelectedIndex < 60 &&
                _secondList.SelectedIndex >= 0 && _secondList.SelectedIndex < 60)
            {
                var now = DateTime.Now;
                DisplayTime = new DateTime(now.Year, now.Month, now.Day, _hourList.SelectedIndex,
                    _minuteList.SelectedIndex, _secondList.SelectedIndex);
            }
        }

        /// <summary>
        ///     更新
        /// </summary>
        /// <param name="time"></param>
        internal override void Update(DateTime time)
        {
            if (_hourList == null || _minuteList == null || _secondList == null)
                return;

            var h = time.Hour;
            var m = time.Minute;
            var s = time.Second;

            _hourList.SelectedIndex = h;
            _minuteList.SelectedIndex = m;
            _secondList.SelectedIndex = s;

            _hourList.AnimateScrollIntoView(_hourList.SelectedIndex);
            _minuteList.AnimateScrollIntoView(_minuteList.SelectedIndex);
            _secondList.AnimateScrollIntoView(_secondList.SelectedIndex);

            SetValueNoCallback(DisplayTimeProperty, time);
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);

            if (IsMouseOver)
                return;

            if (_hourList == null || _minuteList == null || _secondList == null)
                return;

            _hourList.AnimateScrollIntoView(_hourList.SelectedIndex);
            _minuteList.AnimateScrollIntoView(_minuteList.SelectedIndex);
            _secondList.AnimateScrollIntoView(_secondList.SelectedIndex);
        }
    }
}