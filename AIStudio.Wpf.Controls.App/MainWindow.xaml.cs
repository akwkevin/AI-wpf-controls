using System.Windows;

namespace AIStudio.Wpf.Controls.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void menuScreenshot_Click(object sender, RoutedEventArgs e)
        {
            AIStudio.Wpf.ComeCapture.MainWindow window = new AIStudio.Wpf.ComeCapture.MainWindow();
            window.Show();
        }       
    }
}
