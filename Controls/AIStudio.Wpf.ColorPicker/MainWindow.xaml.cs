using System.Windows;
using AIStudio.Wpf.ColorPicker.ViewModels;

namespace AIStudio.Wpf.ColorPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel)?.Dispose();
            this.Close();
        }
    }
}
