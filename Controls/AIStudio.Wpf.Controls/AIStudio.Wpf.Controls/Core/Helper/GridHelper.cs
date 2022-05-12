using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls.Helper
{

    /// <summary>
    /// 为Grid添加边框
    /// </summary>
    public class GridHelper
    {
        //边框的宽度
        static double myBorderWidth = 0.5;
        //请注意：可以通过propa这个快捷方式生成下面三段代码
        public static bool GetShowBorder(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowBorderProperty);
        }

        public static void SetShowBorder(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowBorderProperty, value);
        }


        public static readonly DependencyProperty ShowBorderProperty =
            DependencyProperty.RegisterAttached("ShowBorder", typeof(bool), typeof(GridHelper), new PropertyMetadata(OnShowBorderChanged));


        //这是一个事件处理程序，需要手工编写，必须是静态方法
        private static void OnShowBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as Grid;
            if ((bool)e.OldValue)
            {
                grid.Loaded -= Grid_Loaded;
            }

            if ((bool)e.NewValue)
            {
                grid.Loaded += Grid_Loaded;
            }
        }

        private static void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            //根据Grid的顶层子控件的个数去添加边框，同时考虑合并的情况
            var controls = grid.Children;
            var count = controls.Count;
            if (grid.ColumnDefinitions.Count == 0)
            {
                //设置边框线的颜色
                var border = new Border();
                border.BorderBrush = new SolidColorBrush(Colors.Gray);
                border.BorderThickness = new Thickness(myBorderWidth, 0, myBorderWidth, myBorderWidth);
                Grid.SetRow(border, grid.RowDefinitions.Count - 1);
                Grid.SetColumn(border, 0);
                grid.Children.Add(border);
            }

            //最后一行
            for (int k = 0; k < grid.ColumnDefinitions.Count; k++)
            {
                var border = new Border();
                border.BorderBrush = new SolidColorBrush(Colors.Gray);
                if (k == 0)
                {
                    border.BorderThickness = new Thickness(myBorderWidth, 0, 0, myBorderWidth);
                }
                else if (k == grid.ColumnDefinitions.Count - 1)
                {
                    border.BorderThickness = new Thickness(0, 0, myBorderWidth, myBorderWidth);
                }
                else
                {
                    border.BorderThickness = new Thickness(0, 0, 0, myBorderWidth);
                }

                Grid.SetRow(border, grid.RowDefinitions.Count - 1);
                Grid.SetColumn(border, k);
                grid.Children.Add(border);
            }

            for (int i = 0; i < count; i++)
            {
                var item = controls[i] as FrameworkElement;
                var row = Grid.GetRow(item);
                var column = Grid.GetColumn(item);
                var rowspan = Grid.GetRowSpan(item);
                var columnspan = Grid.GetColumnSpan(item);

                var border = new Border();
                border.BorderBrush = new SolidColorBrush(Colors.Gray);
                if (row != grid.RowDefinitions.Count - 1)
                {
                    if (row == 0 && column == 0)
                    {
                        border.BorderThickness = new Thickness(myBorderWidth, myBorderWidth, myBorderWidth, myBorderWidth);
                    }
                    else if (row == 0 && column != 0)
                    {
                        border.BorderThickness = new Thickness(0, myBorderWidth, myBorderWidth, myBorderWidth);
                    }
                    else if (row > 0 && column == 0)
                    {
                        border.BorderThickness = new Thickness(myBorderWidth, 0, myBorderWidth, myBorderWidth);
                    }
                    else if (row > 0 && column > 0)
                    {
                        border.BorderThickness = new Thickness(0, 0, myBorderWidth, myBorderWidth);
                    }

                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, column);
                    Grid.SetRowSpan(border, rowspan);
                    Grid.SetColumnSpan(border, columnspan);
                    grid.Children.Add(border);
                }
            }
        }

        public static bool GetAllowDrop(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowDropProperty);
        }

        public static void SetAllowDrop(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowDropProperty, value);
        }


        public static readonly DependencyProperty AllowDropProperty =
            DependencyProperty.RegisterAttached("AllowDrop", typeof(bool), typeof(GridHelper), new PropertyMetadata(OnAllowDropChanged));

        private static void OnAllowDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as Grid;
            if (grid != null)
            {
                if ((bool)e.OldValue)
                {
                    grid.AllowDrop = false;
                    grid.Drop -= Grid_Drop;
                    grid.MouseLeftButtonDown -= Grid_MouseLeftButtonDown;
                }

                if ((bool)e.NewValue)
                {
                    grid.AllowDrop = true;
                    grid.Drop += Grid_Drop;
                    grid.MouseLeftButtonDown += Grid_MouseLeftButtonDown;
                }
            }
        }

        private static void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            var pos = e.GetPosition(grid);
            HitTestResult result = VisualTreeHelper.HitTest(grid, pos);
            if (result == null)
            {
                return;
            }
            var dragItem = GetRootChild(grid, result.VisualHit);
            if (dragItem == null)
            {
                return;
            }

            DragDrop.DoDragDrop(grid, new DragObject(dragItem), DragDropEffects.Move);
        }

        private static void Grid_Drop(object sender, DragEventArgs e)
        {
            var grid = sender as Grid;
            var pos = e.GetPosition(grid);
            var result = VisualTreeHelper.HitTest(grid, pos);
            if (result == null)
            {
                return;
            }

            //查找元数据
            var sourceItemObject = e.Data.GetData(typeof(DragObject)) as DragObject;
            if (sourceItemObject == null)
            {
                return;
            }

            var sourceItem = sourceItemObject.Source;
            if (sourceItem == null)
            {
                return;
            }

            //查找目标数据
            var targetItem = GetRootChild(grid, result.VisualHit);
            if (targetItem == null)
            {
                return;
            }

            if (sourceItem == targetItem)
            {
                return;
            }

            var sourceRow = Grid.GetRow(sourceItem);
            var sourceColumn = Grid.GetColumn(sourceItem);
            var sourceRowSpan = Grid.GetRowSpan(sourceItem);
            var sourceColumnSpan = Grid.GetColumnSpan(sourceItem);

            var targetRow = Grid.GetRow(targetItem);
            var targetColumn = Grid.GetColumn(targetItem);
            var targetRowSpan = Grid.GetRowSpan(targetItem);
            var targetColumnSpan = Grid.GetColumnSpan(targetItem);

            Grid.SetRow(targetItem, sourceRow);
            Grid.SetColumn(targetItem, sourceColumn);
            Grid.SetRowSpan(targetItem, sourceRowSpan);
            Grid.SetColumnSpan(targetItem, sourceColumnSpan);

            Grid.SetRow(sourceItem, targetRow);
            Grid.SetColumn(sourceItem, targetColumn);
            Grid.SetRowSpan(sourceItem, targetRowSpan);
            Grid.SetColumnSpan(sourceItem, targetColumnSpan);
        }

        private static FrameworkElement GetRootChild(Grid grid, DependencyObject dependency)
        {
            if (dependency == null)
                return null;

            if (dependency is FrameworkElement element)
            {
                if (grid.Children.Contains(element))
                {
                    return element;
                }
            }

            DependencyObject parentObject = dependency.GetParentObject();
            return GetRootChild(grid, parentObject);
        }
    }

    public class DragObject
    {
        public FrameworkElement Source { get; set; }

        public DragObject(FrameworkElement source)
        {
            Source = source;
        }
    }
}
