using System.ComponentModel;

namespace AIStudio.Wpf.Panels
{
    public interface IResizableItemData
    {
    }

    public class ResizableItemData : IResizableItemData, INotifyPropertyChanged
    {
        private double width = double.NaN;
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                OnPropertyChanged("Width");
            }
        }

        private double height = double.NaN;
        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                OnPropertyChanged("Height");
            }
        }

        private int widthPix = 1;
        public int WidthPix
        {
            get
            {
                return widthPix;
            }
            set
            {
                widthPix = value;
                OnPropertyChanged("WidthPix");
            }
        }

        private int heightPix = 1;
        public int HeightPix
        {
            get
            {
                return heightPix;
            }
            set
            {
                heightPix = value;
                OnPropertyChanged("HeightPix");
            }
        }

        public int Number
        {
            get; set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
