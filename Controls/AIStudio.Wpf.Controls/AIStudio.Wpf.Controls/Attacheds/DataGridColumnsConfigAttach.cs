using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AIStudio.Wpf.Controls.Bindings;

namespace AIStudio.Wpf.Controls
{
    public class DataGridColumnsConfigAttach
    {
    
        public static readonly DependencyProperty ShowConfigProperty =
            DependencyProperty.RegisterAttached(
                "ShowConfig",
                typeof(bool),
                typeof(DataGridColumnsConfigAttach),
                new UIPropertyMetadata(false, OnShowConfigChanged));

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static void SetShowConfig(DependencyObject element, bool value)
        {
            element.SetValue(ShowConfigProperty, value);
        }

        [Category(AppName.AIStudio)]
        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static bool GetShowConfig(DependencyObject element)
        {
            return (bool)element.GetValue(ShowConfigProperty);
        }

        private static void OnShowConfigChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                ContextMenu menu = null;
                if (dataGrid.ContextMenu != null)
                {
                    menu = dataGrid.ContextMenu;
                }
                else
                {
                    menu = new ContextMenu();
                    dataGrid.ContextMenu = menu;
                }

                if ((bool)e.NewValue == true)
                {
                    var item = new MenuItem() { Header = "表格配置" };
                    item.Click += Item_Click;
                    item.Tag = dataGrid;
                    menu.Items.Add(item);
                }
                else
                {
                    var item = menu.Items.OfType<MenuItem>().Where(p => p.Header?.ToString() == "表格配置");
                    menu.Items.Remove(item);
                }

            }
        }

        private static void Item_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                var window = new DataGridColumnConfigWindow(menuItem.Tag as DataGrid);
                window.Owner = Application.Current.MainWindow;
                window.ShowDialog();
            }
        }
    }


   
}
