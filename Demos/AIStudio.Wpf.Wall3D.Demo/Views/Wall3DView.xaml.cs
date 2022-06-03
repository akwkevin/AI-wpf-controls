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
using AIStudio.Wpf.Wall3D.Demo.Models;
using AIStudio.Wpf.Wall3D.Demo.ViewModels;
using AIStudio.Wpf.Wall3D.Wall;

namespace AIStudio.Wpf.Wall3D.Demo.Views
{
    /// <summary>
    /// Wall3DView.xaml 的交互逻辑
    /// </summary>
    public partial class Wall3DView : UserControl
    {
        public Wall3DView()
        {
            InitializeComponent();

            this.DataContext = new Wall3DViewModel();
        }
    }
}
