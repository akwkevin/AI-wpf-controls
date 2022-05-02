using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AIStudio.Wpf.Controls.Demo.Views
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class NavigationBarView : AIStudioUserControl
    {
        public NavigationBarView()
        {
            InitializeComponent();
            //UseTheScrollViewerScrolling(AnchorPointPanel);
        }

        public void UseTheScrollViewerScrolling(FrameworkElement fElement)
        {
            fElement.PreviewMouseWheel += (sender, e) =>
            {
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                fElement.RaiseEvent(eventArg);
            };
        }
    }
}
