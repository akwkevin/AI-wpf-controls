using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    public static class CalendarAttach
    {
        /************************************ Attach Property **************************************/
        public static readonly DependencyProperty NextYearProperty = DependencyProperty.RegisterAttached(
           "NextYear", typeof(Calendar), typeof(CalendarAttach), new FrameworkPropertyMetadata(default(Calendar), FrameworkPropertyMetadataOptions.Inherits, OnNextYearChanged));

        [AttachedPropertyBrowsableForType(typeof(Calendar))]
        [Category(AppName.AIStudio)]
        public static void SetNextYear(DependencyObject element, Calendar value)
            => element.SetValue(NextYearProperty, value);

        [AttachedPropertyBrowsableForType(typeof(Calendar))]
        [Category(AppName.AIStudio)]
        public static Calendar GetNextYear(DependencyObject element)
            => (Calendar)element.GetValue(NextYearProperty);

        private static void OnNextYearChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button button)
            {
                if ((Calendar)e.NewValue != null)
                {
                    button.Click -= NextButton_Click;
                    button.Click += NextButton_Click;
                    button.Tag = e.NewValue;
                }
            }
        }

        private static void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Tag is Calendar calendar)
                {
                    if (calendar.DisplayMode == CalendarMode.Decade)
                    {
                        calendar.DisplayDate = calendar.DisplayDate.AddYears(10);
                    }
                    else
                    {
                        calendar.DisplayDate = calendar.DisplayDate.AddYears(1);
                    }
                }
            }
        }

        /************************************ Attach Property **************************************/
        public static readonly DependencyProperty PreYearProperty = DependencyProperty.RegisterAttached(
           "PreYear", typeof(Calendar), typeof(CalendarAttach), new FrameworkPropertyMetadata(default(Calendar), FrameworkPropertyMetadataOptions.Inherits, OnPreYearChanged));

        [AttachedPropertyBrowsableForType(typeof(Calendar))]
        [Category(AppName.AIStudio)]
        public static void SetPreYear(DependencyObject element, Calendar value)
            => element.SetValue(PreYearProperty, value);

        [AttachedPropertyBrowsableForType(typeof(Calendar))]
        [Category(AppName.AIStudio)]
        public static Calendar GetPreYear(DependencyObject element)
            => (Calendar)element.GetValue(PreYearProperty);

        private static void OnPreYearChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button button)
            {
                if ((Calendar)e.NewValue != null)
                {
                    button.Click -= PreButton_Click;
                    button.Click += PreButton_Click;
                    button.Tag = e.NewValue;
                }
            }
        }

        private static void PreButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Tag is Calendar calendar)
                {
                    if (calendar.DisplayMode == CalendarMode.Decade)
                    {
                        calendar.DisplayDate = calendar.DisplayDate.AddYears(-10);
                    }
                    else
                    {
                        calendar.DisplayDate = calendar.DisplayDate.AddYears(-1);
                    }
                }
            }
        }
    }
}
