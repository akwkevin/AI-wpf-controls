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

namespace AIStudio.Wpf.Controls.App.Views
{
    /// <summary>
    /// ScreenshotView.xaml 的交互逻辑
    /// </summary>
    public partial class ColorPickerView : UserControl
    {
        public ColorPickerView()
        {
            InitializeComponent();
        }

        private void ColorPicker_Click(object sender, RoutedEventArgs e)
        {
            AIStudio.Wpf.ColorPicker.MainWindow window = new AIStudio.Wpf.ColorPicker.MainWindow();
            window.Show();
        }
    }
}
