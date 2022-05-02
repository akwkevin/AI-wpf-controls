using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Commands;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class NoticeCardViewModel : BindableBase
    {
        private ICommand _showCommand;
        public ICommand ShowCommand
        {
            get
            {
                return this._showCommand ?? (this._showCommand = new DelegateCommand<ControlStatus>(para => this.ShowBox(para)));
            }
        }

        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                return this._clearCommand ?? (this._showCommand = new DelegateCommand(() => this.Clear()));
            }
        }

        public void ShowBox(ControlStatus para)
        {
            Notice.Show("a notice.",
                "This is ",
                3,
                para, 
                NoticeCardStyle,
                string2double(Width),
                string2double(Height),
                HorizontalAlignment,
                VerticalAlignment,
                ShowSure,
                ShowClose, 
                null,
                Desktop ? "Desktop" : "RootWindow");
        }

        public void Clear()
        {
            Notice.Clear(Desktop ? "Desktop" : "RootWindow");
        }

        private HorizontalAlignment _horizontalAlignment;

        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return _horizontalAlignment;
            }
            set
            {
                SetProperty(ref _horizontalAlignment, value);
            }
        }

        private VerticalAlignment _verticalAlignment;

        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return _verticalAlignment;
            }
            set
            {
                SetProperty(ref _verticalAlignment, value);
            }
        }

        private NoticeCardStyle _noticeCardStyle;

        public NoticeCardStyle NoticeCardStyle
        {
            get
            {
                return _noticeCardStyle;
            }
            set
            {
                SetProperty(ref _noticeCardStyle, value);
            }
        }        

        private bool _desktop = false;

        public bool Desktop
        {
            get
            {
                return _desktop;
            }
            set
            {
                SetProperty(ref _desktop, value);
            }
        }

        private bool _showSure = false;

        public bool ShowSure
        {
            get
            {
                return _showSure;
            }
            set
            {
                SetProperty(ref _showSure, value);
            }
        }

        private bool _showClose = false;

        public bool ShowClose
        {
            get
            {
                return _showClose;
            }
            set
            {
                SetProperty(ref _showClose, value);
            }
        }

        private string _width;

        public string Width
        {
            get
            {
                return _width;
            }
            set
            {
                SetProperty(ref _width, value);
            }
        }

        private string _height;

        public string Height
        {
            get
            {
                return _height;
            }
            set
            {
                SetProperty(ref _height, value);
            }
        }

        private double? string2double(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            if (str.ToLower() == "auto")
                return double.NaN;

            double val;
            if (double.TryParse(str, out val))
            {
                return val;
            }

            return null;
        }
    }
}
