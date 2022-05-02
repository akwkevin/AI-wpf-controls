using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    public class ACalendarButton : Button
    {
        #region Private属性

        #endregion

        #region Fields
        public ACalendar Owner
        {
            get; set;
        }
        #endregion

        #region 依赖属性set get

        #region HasSelectedDates
        public bool HasSelectedDates
        {
            get
            {
                return (bool)GetValue(HasSelectedDatesProperty);
            }
            set
            {
                SetValue(HasSelectedDatesProperty, value);
            }
        }

        public static readonly DependencyProperty HasSelectedDatesProperty =
            DependencyProperty.Register("HasSelectedDates", typeof(bool), typeof(ACalendarButton), new PropertyMetadata(false));
        #endregion

        #endregion

        #region Constructors
        static ACalendarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ACalendarButton), new FrameworkPropertyMetadata(typeof(ACalendarButton)));
        }
        #endregion

        #region Override方法

        #endregion

        #region Private方法

        #endregion
    }
}
