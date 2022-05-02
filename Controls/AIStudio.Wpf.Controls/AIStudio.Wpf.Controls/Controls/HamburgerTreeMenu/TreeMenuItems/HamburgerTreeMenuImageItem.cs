namespace AIStudio.Wpf.Controls
{
    using System.Windows.Media.Imaging;

    public class HamburgerTreeMenuImageItem : HamburgerTreeMenuItem
    {
        private BitmapImage thumbnail;

        public BitmapImage Thumbnail
        {
            get
            {
                return thumbnail;
            }
            set
            {
                if (value == thumbnail) return;
                thumbnail = value;
                OnPropertyChanged("Thumbnail");
            }
        }
    }
}
