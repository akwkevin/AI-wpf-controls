using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.Controls.Core;
using Microsoft.Win32;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class ImageViewModel : BindableBase
    {
        private ImageSource _cutImageSource;
        public ImageSource CutImageSource
        {
            get
            {
                return _cutImageSource;
            }
            set
            {
                _cutImageSource = value;
                RaisePropertyChanged("CutImageSource");
            }
        }

        private ImageSource _saveImageSource;
        public ImageSource SaveImageSource
        {
            get
            {
                return _saveImageSource;
            }
            set
            {
                _saveImageSource = value;
                RaisePropertyChanged("SaveImageSource");
            }
        }

        private ICommand _importCommand;
        public ICommand ImportCommand
        {
            get
            {
                return this._importCommand ?? (this._importCommand = new DelegateCommand(() => {
                    Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                    openFileDialog.Filter = "图像文件(*.jpg;*.jpeg;*.png;)|*.jpg;*.jpeg;*.png;";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        CutImageSource = new BitmapImage(new Uri(openFileDialog.FileName));
                    }
                }));
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return this._saveCommand ?? (this._saveCommand = new DelegateCommand(() => {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.FileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
                    dlg.DefaultExt = ".jpg";
                    dlg.Filter = "image file|*.jpg";

                    if (dlg.ShowDialog() == true)
                    {
                        BitmapEncoder pngEncoder = new PngBitmapEncoder();
                        pngEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)SaveImageSource));
                        using (var fs = System.IO.File.OpenWrite(dlg.FileName))
                        {
                            pngEncoder.Save(fs);
                            fs.Dispose();
                            fs.Close();
                        }
                    }
                }));
            }
        }

        public RelayCommand OpenImgCmd => new Lazy<RelayCommand>(() =>
         new RelayCommand(() =>
             new ImageBrowser(new Uri("pack://application:,,,/AIStudio.Wpf.Controls.Demo;component/Resources/Images/1.jpg")).Show())).Value;
    }
}
