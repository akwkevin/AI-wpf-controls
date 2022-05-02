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
using System.Windows.Shapes;
using AIStudio.Wpf.DragablzControls.Demo.ViewModels;

namespace AIStudio.Wpf.DragablzControls.Demo.Views
{
    /// <summary>
    /// HeaderedItemSample1.xaml 的交互逻辑
    /// </summary>
    public partial class HeaderedItemSample1 : UserControl
    {
        public HeaderedItemSample1()
        {
            InitializeComponent();

            this.DataContext = new HeaderedItemSample1ViewModel();
        }
    }
}
