using System.Windows;

namespace AIStudio.Wpf.Controls.MediaPlayer
{
    /// <summary>
    /// MediaElementPlayerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MediaElementPlayerWindow : Window
    {
        public MediaElementPlayerWindow()
        {
            InitializeComponent();
        }

        public MediaElementPlayerWindow(ShowMode showMode)
        {
            InitializeComponent();
            if (showMode.ToString().Contains("Video"))
            {
                this.Height = 400;
                this.Width = 500;
                this.SizeToContent = SizeToContent.Manual;
            }
        }

        public static void Show(string path, ShowMode showMode = ShowMode.PathMode)
        {
            MediaElementPlayerWindow window = new MediaElementPlayerWindow(showMode);
            window.player.ShowMode = showMode;
            window.player.MusicPath = path;
            window.Show();
        }

    }
}
