using System.Windows;

namespace AIStudio.Wpf.Controls.Demo.Views
{
    /// <summary>
    /// DialogTest.xaml 的交互逻辑
    /// </summary>
    public partial class DialogTest : BaseDialog
    {
        public DialogTest()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            this.OKSource.Cancel();
        }
    }
}
