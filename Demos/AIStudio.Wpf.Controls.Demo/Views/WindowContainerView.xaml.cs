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
    /// WindowContainerView.xaml 的交互逻辑
    /// </summary>
    public partial class WindowContainerView : UserControl
    {
        public WindowContainerView()
        {
            InitializeComponent();

            this.Loaded += WindowContainerView_Loaded;
        }

        private void WindowContainerView_Loaded(object sender, RoutedEventArgs e)
        {
            ChildWindow childWindow = new ChildWindow();
            childWindow.Caption = "Test";
            childWindow.IsModal = true;
            childWindow.WindowStartupLocation = ChildWindowStartupLocation.Center;
            //childWindow.Width = 275;
            //childWindow.Height = 125;
            childWindow.WindowState = ChildWindowState.Open;
            childWindow.Content = new Grid();
            _windowContainer.Children.Add(childWindow);
        }
    }
}
