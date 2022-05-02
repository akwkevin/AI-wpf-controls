using System;
using System.Windows;
using System.Windows.Controls;
using AIStudio.Wpf.Panels.Demo.ViewModels;

namespace AIStudio.Wpf.Panels.Demo
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        int index = 0;
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            txt1.Text = index.ToString();
            Console.WriteLine(index);
            index++;
        }

        int index2 = 0;

        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {
            txt2.Text = "-" + index2.ToString();
            Console.WriteLine("-" + index2);
            index2++;
        }

        int index3 = 0;
        private void Grid_Loaded2(object sender, RoutedEventArgs e)
        {
            txt3.Text = index3.ToString();
            Console.WriteLine(index3);
            index3++;
        }

        int index4 = 0;
        private void Grid_Unloaded2(object sender, RoutedEventArgs e)
        {
            txt4.Text = "-" + index4.ToString();
            Console.WriteLine("-" + index4);
            index4++;
        }
    }
}
