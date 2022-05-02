using System.Windows.Controls;
using AIStudio.Wpf.DragablzControls.Demo.ViewModels;
using Dragablz;

namespace AIStudio.Wpf.DragablzControls.Demo.Views
{
    /// <summary>
    /// HeaderedItemSample1.xaml 的交互逻辑
    /// </summary>
    public partial class HeaderedItemSample2 : UserControl
    {
        public HeaderedItemSample2()
        {
            InitializeComponent();

            var boundExampleModelFirst = new HeaderedItemWindowViewModel(
                new HeaderedItemViewModel
                {
                    Header = "First",
                    Content = "There is First TabItem"
                },
                new HeaderedItemViewModel
                {
                    Header = "Second",
                    Content = "There is Second TabItem"
                },
                new HeaderedItemViewModel
                {
                    Header = "Third",
                    Content = "There is Third TabItem"
                },
                new HeaderedItemViewModel { Header = "Sample", Content = new HeaderedItemSample1() }
            );

            var boundExampleModelSecond = new HeaderedItemWindowViewModel(
                new HeaderedItemViewModel
                {
                    Header = "First",
                    Content = "There is First TabItem"
                },
                new HeaderedItemViewModel
                {
                    Header = "Second",
                    Content = "There is Second TabItem"
                },
                new HeaderedItemViewModel
                {
                    Header = "Third",
                    Content = "There is Third TabItem"
                },
                new HeaderedItemViewModel { Header = "Sample", Content = new HeaderedItemSample1() }
            );

            var boundExampleModelThird = new HeaderedItemWindowViewModel(
                new HeaderedItemViewModel
                {
                    Header = "First",
                    Content = "There is First TabItem"
                },
                new HeaderedItemViewModel
                {
                    Header = "Second",
                    Content = "There is Second TabItem"
                },
                new HeaderedItemViewModel
                {
                    Header = "Third",
                    Content = "There is Third TabItem"
                },
              new HeaderedItemViewModel { Header = "Sample", Content = new HeaderedItemSample1() }
            );
            var viewModel = new HeaderedItemSample2ViewModel(boundExampleModelFirst, boundExampleModelSecond, boundExampleModelThird);

            this.DataContext = viewModel;
        }
    }
}
