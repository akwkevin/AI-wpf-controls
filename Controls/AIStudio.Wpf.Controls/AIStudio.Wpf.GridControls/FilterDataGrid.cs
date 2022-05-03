using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AIStudio.Wpf.GridControls
{
    /// <summary>
    /// Define a standard DataGrid with the included ColumnFilter in the column header template.
    /// </summary>
    public class FilterDataGrid : DataGrid
    {
        static FilterDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilterDataGrid), new FrameworkPropertyMetadata(typeof(FilterDataGrid)));
        }

        public FilterDataGrid()
        {
            Loaded += FilterDataGrid_Loaded;
        }

        private void FilterDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsAnimated)
            {
                var dg = sender as DataGrid;
                Storyboard storyboard = new Storyboard();
                CircleEase easing = new CircleEase();
                easing.EasingMode = EasingMode.EaseInOut;
                int index = 0;
                foreach (var item in dg.Items)
                {
                    var datagridRow = dg.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    if (datagridRow != null && datagridRow.Visibility == Visibility.Visible)
                    {
                        datagridRow.RenderTransform = new TranslateTransform();
                        DoubleAnimation xAnimation = new DoubleAnimation();
                        xAnimation.EasingFunction = easing;
                        xAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000 + 50 * index));
                        xAnimation.From = 100;
                        xAnimation.To = 0;
                        Storyboard.SetTarget(xAnimation, datagridRow);
                        Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
                        storyboard.Children.Add(xAnimation);

                        DoubleAnimation yAnimation = new DoubleAnimation();
                        yAnimation.EasingFunction = easing;
                        yAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000 + 50 * index));
                        yAnimation.From = 50;
                        yAnimation.To = 0;
                        Storyboard.SetTarget(yAnimation, datagridRow);
                        Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
                        storyboard.Children.Add(yAnimation);

                        DoubleAnimation opcityAnimation = new DoubleAnimation();
                        // opcityAnimation.EasingFunction = easing;
                        opcityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000 + 50 * index));
                        opcityAnimation.From = 0;
                        opcityAnimation.To = 1;
                        Storyboard.SetTarget(opcityAnimation, datagridRow);
                        Storyboard.SetTargetProperty(opcityAnimation, new PropertyPath(DataGridRow.OpacityProperty));
                        storyboard.Children.Add(opcityAnimation);
                        index++;
                    }
                }
                storyboard.Freeze();
                storyboard.Begin();
            }
        }

        public static readonly DependencyProperty IsAnimatedProperty = DependencyProperty.Register(nameof(IsAnimated), typeof(bool), typeof(FilterDataGrid), new PropertyMetadata(false));

        public bool IsAnimated
        {
            get { return (bool)this.GetValue(FilterDataGrid.IsAnimatedProperty); }
            set { this.SetValue(FilterDataGrid.IsAnimatedProperty, value); }
        }
    }
}
