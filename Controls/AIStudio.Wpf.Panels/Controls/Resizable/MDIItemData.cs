namespace AIStudio.Wpf.Panels
{
    public class MDIItemData : ResizableItemData
    {
        private TileState tileState;
        public TileState TileState
        {
            get
            {
                return tileState;
            }
            set
            {
                tileState = value;
                OnPropertyChanged("TileState");
            }
        }

        private object title = "Title";
        public object Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        private bool canClose = false;
        public bool CanClose
        {
            get
            {
                return canClose;
            }
            set
            {
                canClose = value;
                OnPropertyChanged("CanClose");
            }
        }
    }
}
