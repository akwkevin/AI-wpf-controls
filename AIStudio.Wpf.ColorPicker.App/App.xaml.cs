using System;
using System.Configuration;
using System.Data;
using System.Windows;
using AIStudio.Wpf.ColorPicker.ViewModels;

namespace AIStudio.Wpf.ColorPicker.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Startup += this.Application_Startup;
        }
        protected void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                AIStudio.Wpf.ColorPicker.MainWindow window = new AIStudio.Wpf.ColorPicker.MainWindow();
                window.Show();
            }
            catch (Exception ex)
            {
            
            }

        }
    }



}
