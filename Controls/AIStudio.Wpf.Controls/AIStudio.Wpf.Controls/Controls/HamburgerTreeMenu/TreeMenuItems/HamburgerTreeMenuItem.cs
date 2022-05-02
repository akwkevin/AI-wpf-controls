namespace AIStudio.Wpf.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;
    using AIStudio.Wpf.Controls.Bindings;

    /// <summary>
    /// The HamburgerTreeMenuItem provides an abstract implementation for HamburgerMenu entries.
    /// </summary> 
    public abstract class HamburgerTreeMenuItem : BindableBase
    {

        private string label;
        [Browsable(true)]
        public string Label
        {
            get
            {
                return label;
            }
            set
            {
                if (value == label) return;
                label = value;
                OnPropertyChanged("Label");
            }
        }


        private Type targetPageType;

        public Type TargetPageType
        {
            get
            {
                return targetPageType;
            }
            set
            {
                if (value == targetPageType) return;
                targetPageType = value;
                OnPropertyChanged("TargetPageType");
            }
        }

        private object tag;

        public object Tag
        {
            get
            {
                return tag;
            }
            set
            {
                if (value == tag) return;
                tag = value;
                OnPropertyChanged("Tag");
            }
        }

        private ICommand command;
        public ICommand Command
        {
            get
            {
                return command;
            }
            set
            {
                if (value == command) return;
                command = value;
                OnPropertyChanged("Command");
            }
        }

        private ObservableCollection<HamburgerTreeMenuItem> children = new ObservableCollection<HamburgerTreeMenuItem>();

        public ObservableCollection<HamburgerTreeMenuItem> Children
        {
            get
            {
                return children;
            }
            set
            {
                if (value == children) return;
                children = value;
                OnPropertyChanged("Children");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
