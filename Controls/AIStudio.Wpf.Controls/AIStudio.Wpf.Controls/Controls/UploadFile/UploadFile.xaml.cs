using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Helper;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace AIStudio.Wpf.Controls
{
    public class UploadFile : Control
    {
        public static readonly DependencyProperty UploadFileTypeProperty = DependencyProperty.Register(
            "UploadFileType", typeof(UploadFileType), typeof(UploadFile), new PropertyMetadata(UploadFileType.File));

        public UploadFileType UploadFileType
        {
            get
            {
                return (UploadFileType)this.GetValue(UploadFileTypeProperty);
            }
            set
            {
                this.SetValue(UploadFileTypeProperty, value);
            }
        }

        public static readonly DependencyProperty FilesProperty = DependencyProperty.Register(
            "Files", typeof(ObservableCollection<string>), typeof(UploadFile), new PropertyMetadata(null, FilesPropertyChangedCallback));

        public ObservableCollection<string> Files
        {
            get
            {
                return (ObservableCollection<string>)this.GetValue(FilesProperty);
            }
            set
            {
                this.SetValue(FilesProperty, value);
            }
        }

        public static readonly DependencyProperty MaxCountProperty = DependencyProperty.Register(
            "MaxCount", typeof(int), typeof(UploadFile), new PropertyMetadata(1));

        public int MaxCount
        {
            get
            {
                return (int)this.GetValue(MaxCountProperty);
            }
            set
            {
                this.SetValue(MaxCountProperty, value);
            }
        }

        public static readonly DependencyProperty FileProperty = DependencyProperty.Register(
            "File", typeof(string), typeof(UploadFile), new PropertyMetadata(default(string), FilePropertyChangedCallback));

        public string File
        {
            get
            {
                return (string)this.GetValue(FileProperty);
            }
            set
            {
                this.SetValue(FileProperty, value);
            }
        }

        public static readonly DependencyProperty UploadVisibleProperty = DependencyProperty.Register(
            "UploadVisible", typeof(Visibility), typeof(UploadFile), new PropertyMetadata(Visibility.Visible));

        public Visibility UploadVisible
        {
            get
            {
                return (Visibility)this.GetValue(UploadVisibleProperty);
            }
            set
            {
                this.SetValue(UploadVisibleProperty, value);
            }
        }

        public static readonly DependencyProperty FilesDisplayProperty = DependencyProperty.Register(
            "FilesDisplay", typeof(ObservableCollection<UploadFileDisplay>), typeof(UploadFile), new PropertyMetadata(new ObservableCollection<UploadFileDisplay>() { new UploadFileDisplay() { IsUploadTemplate = true } }));

        public ObservableCollection<UploadFileDisplay> FilesDisplay
        {
            get
            {
                return (ObservableCollection<UploadFileDisplay>)this.GetValue(FilesDisplayProperty);
            }
            set
            {
                this.SetValue(FilesDisplayProperty, value);
            }
        }

        public static readonly DependencyProperty DisableProperty = DependencyProperty.Register(
           "Disable", typeof(bool), typeof(UploadFile), new PropertyMetadata(false));

        public bool Disable
        {
            get
            {
                return (bool)this.GetValue(DisableProperty);
            }
            set
            {
                this.SetValue(DisableProperty, value);
            }
        }

        public static readonly DependencyProperty UploadUrlProperty = DependencyProperty.Register(
            "UploadUrl", typeof(string), typeof(UploadFile), new PropertyMetadata(default(string), FilePropertyChangedCallback));

        public string UploadUrl
        {
            get
            {
                return (string)this.GetValue(UploadUrlProperty);
            }
            set
            {
                this.SetValue(UploadUrlProperty, value);
            }
        }

        public static readonly DependencyProperty UploadProperty = DependencyProperty.Register(
            "Upload", typeof(Func<string, Task<Tuple<string, string>>>), typeof(UploadFile), new PropertyMetadata(null));

        public Func<string, Task<Tuple<string, string>>> Upload
        {
            get
            {
                return (Func<string, Task<Tuple<string, string>>>)this.GetValue(UploadProperty);
            }
            set
            {
                this.SetValue(UploadProperty, value);
            }
        }

        private static void FilePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var uploadFile = (UploadFile)dependencyObject;
            if (uploadFile != null)
            {
                if (!string.IsNullOrEmpty(uploadFile.File) && uploadFile.MaxCount == 1 && !uploadFile.Files.Contains(uploadFile.File))
                {
                    uploadFile.Files = new ObservableCollection<string>() { uploadFile.File };
                }
            }
        }

        private static void FilesPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var uploadFile = (UploadFile)dependencyObject;
            if (uploadFile != null)
            {
                uploadFile.InitDisplay();
            }
        }

        private void InitDisplay()
        {
            if (Files != null)
            {
                List<UploadFileDisplay> files = new List<UploadFileDisplay>();
                foreach (var file in Files)
                {
                    string filename = System.IO.Path.GetFileName(file);
                    files.Add(new UploadFileDisplay() { DisplayName = filename, Url = file });
                }
                files.Add(new UploadFileDisplay() { IsUploadTemplate = true });
                FilesDisplay = new ObservableCollection<UploadFileDisplay>(files);
            }
            else
            {
                FilesDisplay = new ObservableCollection<UploadFileDisplay>() { new UploadFileDisplay() { IsUploadTemplate = true } };
            }
            SetDisplay();
        }

        private void SetDisplay()
        {

            if (Files != null && Files.Count >= MaxCount)
            {
                UploadVisible = Visibility.Collapsed;
            }
            else
            {
                UploadVisible = Visibility.Visible;
            }

            if (MaxCount == 1 && Files != null && Files.Count > 0)
            {
                File = Files.FirstOrDefault();
            }
        }

        public ICommand AddCommand
        {
            get; protected set;
        }
        public ICommand DeleteCommand
        {
            get; protected set;
        }
        public ICommand OpenCommand
        {
            get; protected set;
        }
        static UploadFile()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UploadFile), new FrameworkPropertyMetadata(typeof(UploadFile)));
        }


        public UploadFile()
        {
            this.AddCommand = new RoutedUICommand();
            this.DeleteCommand = new RoutedUICommand();
            this.OpenCommand = new RoutedUICommand();
            this.BindCommand(AddCommand, this.AddExecute);
            this.BindCommand(DeleteCommand, this.DeleteExecute);
            this.BindCommand(OpenCommand, this.OpenExecute);
            Files = new ObservableCollection<string>();

        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Array files = (System.Array)e.Data.GetData(DataFormats.FileDrop);
                this.OnFileUpload(files.OfType<string>().ToArray());
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
        }

        private void AddExecute(object sender, ExecutedRoutedEventArgs e)
        {
            UploadFile uploadFile = sender as UploadFile;
            if (uploadFile != null)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (uploadFile.UploadFileType == UploadFileType.File)
                {
                    ofd.Filter = "所有文件|*.*";
                }
                else if (uploadFile.UploadFileType == UploadFileType.Image)
                {
                    ofd.Filter = "All Image Files|*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.png;*.tif;*.tiff|" +
                        "Windows Bitmap(*.bmp)|*.bmp|" +
                        "Windows Icon(*.ico)|*.ico|" +
                        "Graphics Interchange Format (*.gif)|(*.gif)|" +
                        "JPEG File Interchange Format (*.jpg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphics (*.png)|*.png|" +
                        "Tag Image File Format (*.tif)|*.tif;*.tiff";
                }
                ofd.ValidateNames = true;
                ofd.CheckPathExists = true;
                ofd.CheckFileExists = true;
                if (ofd.ShowDialog() == true)
                {
                    string[] paths = ofd.FileNames;
                    OnFileUpload(paths);
                }
                e.Handled = true;
            }
        }

        private async void OnFileUpload(string[] paths)
        {
            if (Files != null)
            {
                try
                {
                    foreach (var path in paths)
                    {
                        if (Upload != null)
                        {
                            var result = await Upload(path);
                            if (!string.IsNullOrEmpty(result.Item1))
                            {
                                Files.Add(result.Item1);
                                FilesDisplay.Insert(FilesDisplay.Count - 1, new UploadFileDisplay() { DisplayName = result.Item2, Url = result.Item1 });
                                SetDisplay();
                            }
                            else
                            {
                                MessageBox.Show($"上传失败:{result.Item2}");
                            }
                        }
                        else if (!string.IsNullOrEmpty(UploadUrl))
                        {
                            var data = new MultipartFormDataContent();

                            using (System.IO.FileStream fStream = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                            {
                                data.Add(new StreamContent(fStream, (int)fStream.Length), "file", System.IO.Path.GetFileName(path));

                                using (HttpClient client = new HttpClient())
                                {
                                    HttpResponseMessage response = await client.PostAsync(UploadUrl, data);
                                    response.EnsureSuccessStatusCode();
                                    var responseBody = await response.Content.ReadAsStringAsync();

                                    var result = JsonConvert.DeserializeObject<UploadResult>(responseBody);

                                    if (result.status == "done")
                                    {
                                        Files.Add(result.url);
                                        FilesDisplay.Insert(FilesDisplay.Count - 1, new UploadFileDisplay() { DisplayName = result.name, Url = result.url });
                                        SetDisplay();
                                    }
                                    else
                                    {
                                        MessageBox.Show($"上传失败:{ result.status }");
                                    }
                                }
                            }
                        }
                    }
                    throw new Exception("没有找到上传方法");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void DeleteExecute(object sender, ExecutedRoutedEventArgs e)
        {
            UploadFile uploadFile = sender as UploadFile;
            if (uploadFile != null)
            {
                uploadFile.Files.Remove((e.Parameter as UploadFileDisplay).Url);
                uploadFile.FilesDisplay.Remove(e.Parameter as UploadFileDisplay);
                uploadFile.SetDisplay();
                e.Handled = true;
            }
        }
        private void OpenExecute(object sender, ExecutedRoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", e.Parameter as string);
            e.Handled = true;
        }

    }

    public class UploadFileDisplay
    {
        public bool IsUploadTemplate
        {
            get; set;
        }
        public string Url
        {
            get; set;
        }
        public string DisplayName
        {
            get; set;
        }
        public bool IsImage
        {
            get; set;
        }
    }

    public enum UploadFileType
    {
        File,
        Image
    }

    public class UploadResult
    {
        public string name
        {
            get; set;
        }
        public string status
        {
            get; set;
        }
        public string thumbUrl
        {
            get; set;
        }
        public string url
        {
            get; set;
        }
        public int uploaded
        {
            get; set;
        }
    }

}
