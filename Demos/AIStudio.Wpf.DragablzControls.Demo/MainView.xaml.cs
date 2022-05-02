using System.Windows.Controls;
using AIStudio.Wpf.DragablzControls.Demo.ViewModels;

namespace AIStudio.Wpf.DragablzControls.Demo
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
    }
}
