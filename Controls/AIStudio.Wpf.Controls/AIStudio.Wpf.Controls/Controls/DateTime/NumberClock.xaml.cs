using System;
using System.Windows;
using AIStudio.Wpf.Controls.Event;

namespace AIStudio.Wpf.Controls
{
    public class NumberClock : ClockBase
    {

        #region 依赖属性
        public static readonly DependencyProperty HourProperty = DependencyProperty.Register("Hour"
            , typeof(int), typeof(NumberClock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// 小时
        /// </summary>
        public int Hour
        {
            get
            {
                return (int)GetValue(HourProperty);
            }
            set
            {
                SetValue(HourProperty, value);
            }
        }

        public static readonly DependencyProperty MinuteProperty = DependencyProperty.Register("Minute"
            , typeof(int), typeof(NumberClock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// 分钟
        /// </summary>
        public int Minute
        {
            get
            {
                return (int)GetValue(MinuteProperty);
            }
            set
            {
                SetValue(MinuteProperty, value);
            }
        }

        public static readonly DependencyProperty SecondProperty = DependencyProperty.Register("Second"
            , typeof(int), typeof(NumberClock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// 秒数
        /// </summary>
        public int Second
        {
            get
            {
                return (int)GetValue(SecondProperty);
            }
            set
            {
                SetValue(SecondProperty, value);
            }
        }

        public static readonly DependencyProperty IsDefaultValueProperty = DependencyProperty.Register(
            "IsDefaultValue", typeof(bool), typeof(NumberClock), new PropertyMetadata(false));

        public bool IsDefaultValue
        {
            get => (bool)GetValue(IsDefaultValueProperty);
            set => SetValue(IsDefaultValueProperty, value);
        }

        protected override void OnSelectedTimeChanged(FunctionEventArgs<DateTime?> e)
        {
            NumberClock timePicker = (NumberClock)e.OriginalSource;
            DateTime dt = Convert.ToDateTime(e.Info);

            timePicker.Hour = dt.Hour;
            timePicker.Minute = dt.Minute;
            timePicker.Second = dt.Second;

            base.OnSelectedTimeChanged(e);
        }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NumberClock timePicker = (NumberClock)sender;

            string time = string.Format("{0}:{1}:{2}", timePicker.Hour, timePicker.Minute, timePicker.Second);
            DateTime dt1 = Convert.ToDateTime(time);
            timePicker.SelectedTime = dt1;

            timePicker.OnSelectedTimeChanged(new FunctionEventArgs<DateTime?>(SelectedTimeChangedEvent, timePicker)
            {
                Info = dt1
            });
        }


        #endregion

        public NumberClock()
        {

        }

        static NumberClock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberClock), new FrameworkPropertyMetadata(typeof(NumberClock)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Update(DateTime.Now);
        }

        internal override void Update(DateTime time)
        {
            if (IsDefaultValue)
            {
                SelectedTime = time;
            }
        }
    }
}
