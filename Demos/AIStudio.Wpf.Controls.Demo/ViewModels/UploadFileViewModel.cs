using System;
using System.Threading.Tasks;
using AIStudio.Wpf.Controls.Bindings;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class UploadFileViewModel : BindableBase
    {
        public UploadFileViewModel()
        {
            Upload = OnUpload;
        }

        private Func<string, Task<Tuple<string, string>>> _upload;
        public Func<string, Task<Tuple<string, string>>> Upload
        {
            get { return _upload; }
            set
            {
                if (_upload != value)
                {
                    _upload = value;
                    RaisePropertyChanged("Upload");
                }
            }
        }

        private async Task<Tuple<string, string>> OnUpload(string path)
        {
            await Task.Delay(1000);
            return new Tuple<string, string>(path, path);
        }

    }
}
