using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AIStudio.Wpf.Controls.Helper;
using AIStudio.Wpf.PrintDialog.Helpers;

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

        [STAThread]
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        private void menuPrism_Click(object sender, RoutedEventArgs e)
        {
#if NETFRAMEWORK
            var domain = AppDomain.CreateDomain("AIStudio.Wpf.PrismRegions.Demo");

            domain.DoCallBack(new CrossAppDomainDelegate(() => {
                Thread thread = new Thread(() => {
                    AIStudio.Wpf.PrismRegions.Demo.App app = new PrismRegions.Demo.App();
                    app.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
                    app.Run();
                });
                thread.Name = AppDomain.CurrentDomain.FriendlyName;
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }));
#else
            System.Diagnostics.Process.Start("PrismRegions.Demo/AIStudio.Wpf.PrismRegions.Demo.exe");
#endif
        }

        private void menuPrint_Click(object sender, RoutedEventArgs e)
        {
            FixedPrintDialog.Print(this, new List<Panel> {
                CreateContent()
            });            
        }

        private StackPanel CreateContent()
        {
            //Create a StackPanel and make it cover entire page
            //FixedPage can contains any UIElement. But VerticalAlignment="Stretch" or HorizontalAlignment="Stretch" doesn't work, so you need calculate its size to make it cover the entire page
            StackPanel stackPanel = new StackPanel();

            //Put some elements into StackPanel (as same way as normal and they have styles, but triggers and animations don't work)
            //You can include any control that override the UIElement class
            stackPanel.Children.Add(new TextBlock() { Text = "This is the page title", FontWeight = FontWeights.Bold, FontSize = 28, TextAlignment = TextAlignment.Center, Margin = new Thickness(10, 5, 10, 35) });
            stackPanel.Children.Add(new TextBlock() { Text = "These are some regular text.", Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBlock() { Text = "These are some bold text.", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBlock() { Text = "These are some italic text.", FontStyle = FontStyles.Italic, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBlock() { Text = "These are some different colored text.", Foreground = Brushes.Red, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new Button() { Content = "This is a button.", Margin = new Thickness(10, 5, 10, 5), Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center });
            stackPanel.Children.Add(new Button() { Content = "This is a button with different color.", BorderBrush = Brushes.Black, Background = Brushes.DarkGray, Foreground = Brushes.White, Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new Button() { Content = "This is a button with different color.", BorderBrush = Brushes.Orange, Background = Brushes.Yellow, Foreground = Brushes.OrangeRed, Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBox() { Text = "This is a textbox, but you can't type text in FixedDocument.", Margin = new Thickness(10, 5, 10, 5), Width = 550, Height = 30, VerticalContentAlignment = VerticalAlignment.Center });

            //Return the StackPanel
            return stackPanel;
        }
    }
}
