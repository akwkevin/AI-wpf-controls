using System;
using System.IO;
using System.Net;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Resources;
using System.Windows.Threading;

namespace AIStudio.Wpf.Controls
{
    public class GifImageExceptionRoutedEventArgs : RoutedEventArgs
    {
        public Exception ErrorException;

        public GifImageExceptionRoutedEventArgs(RoutedEvent routedEvent, object obj)
            : base(routedEvent, obj)
        {
        }
    }

    class WebReadState
    {
        public WebRequest webRequest;
        public MemoryStream memoryStream;
        public Stream readStream;
        public byte[] buffer;
    }


    public class GifImage : System.Windows.Controls.UserControl
    {
        private GifAnimation gifAnimation = null;
        private Image image = null;

        public GifImage()
        {
        }

        public static readonly DependencyProperty ForceGifAnimProperty = DependencyProperty.Register("ForceGifAnim", typeof(bool), typeof(GifImage), new FrameworkPropertyMetadata(false));
        public bool ForceGifAnim
        {
            get
            {
                return (bool)this.GetValue(ForceGifAnimProperty);
            }
            set
            {
                this.SetValue(ForceGifAnimProperty, value);
            }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(GifImage), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnSourceChanged)));
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GifImage obj = (GifImage)d;
            string s = (string)e.NewValue;
            obj.CreateFromSourceString(s);
        }
        public string Source
        {
            get
            {
                return (string)this.GetValue(SourceProperty);
            }
            set
            {
                this.SetValue(SourceProperty, value);
            }
        }


        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(GifImage), new FrameworkPropertyMetadata(Stretch.Fill, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnStretchChanged)));
        private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GifImage obj = (GifImage)d;
            Stretch s = (Stretch)e.NewValue;
            if (obj.gifAnimation != null)
            {
                obj.gifAnimation.Stretch = s;
            }
            else if (obj.image != null)
            {
                obj.image.Stretch = s;
            }
        }
        public Stretch Stretch
        {
            get
            {
                return (Stretch)this.GetValue(StretchProperty);
            }
            set
            {
                this.SetValue(StretchProperty, value);
            }
        }

        public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register("StretchDirection", typeof(StretchDirection), typeof(GifImage), new FrameworkPropertyMetadata(StretchDirection.Both, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnStretchDirectionChanged)));
        private static void OnStretchDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GifImage obj = (GifImage)d;
            StretchDirection s = (StretchDirection)e.NewValue;
            if (obj.gifAnimation != null)
            {
                obj.gifAnimation.StretchDirection = s;
            }
            else if (obj.image != null)
            {
                obj.image.StretchDirection = s;
            }
        }
        public StretchDirection StretchDirection
        {
            get
            {
                return (StretchDirection)this.GetValue(StretchDirectionProperty);
            }
            set
            {
                this.SetValue(StretchDirectionProperty, value);
            }
        }

        public delegate void ExceptionRoutedEventHandler(object sender, GifImageExceptionRoutedEventArgs args);

        public static readonly RoutedEvent ImageFailedEvent = EventManager.RegisterRoutedEvent("ImageFailed", RoutingStrategy.Bubble, typeof(ExceptionRoutedEventHandler), typeof(GifImage));

        public event ExceptionRoutedEventHandler ImageFailed
        {
            add
            {
                AddHandler(ImageFailedEvent, value);
            }
            remove
            {
                RemoveHandler(ImageFailedEvent, value);
            }
        }

        void image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            RaiseImageFailedEvent(e.ErrorException);
        }


        void RaiseImageFailedEvent(Exception exp)
        {
            GifImageExceptionRoutedEventArgs newArgs = new GifImageExceptionRoutedEventArgs(ImageFailedEvent, this);
            newArgs.ErrorException = exp;
            RaiseEvent(newArgs);
        }


        private void DeletePreviousImage()
        {
            if (image != null)
            {
                this.RemoveLogicalChild(image);
                image = null;
            }
            if (gifAnimation != null)
            {
                this.RemoveLogicalChild(gifAnimation);
                gifAnimation = null;
            }
        }

        private void CreateNonGifAnimationImage()
        {
            image = new Image();
            image.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(image_ImageFailed);
            ImageSource src = (ImageSource)(new ImageSourceConverter().ConvertFromString(Source));
            image.Source = src;
            image.Stretch = Stretch;
            image.StretchDirection = StretchDirection;
            this.AddChild(image);
        }


        private void CreateGifAnimation(MemoryStream memoryStream)
        {
            gifAnimation = new GifAnimation();
            gifAnimation.CreateGifAnimation(memoryStream);
            gifAnimation.Stretch = Stretch;
            gifAnimation.StretchDirection = StretchDirection;
            this.AddChild(gifAnimation);
        }


        private void CreateFromSourceString(string source)
        {
            DeletePreviousImage();
            Uri uri;

            try
            {
                uri = new Uri(source, UriKind.RelativeOrAbsolute);
            }
            catch (Exception exp)
            {
                RaiseImageFailedEvent(exp);
                return;
            }

            if (source.Trim().ToUpper().EndsWith(".GIF") || ForceGifAnim)
            {
                if (!uri.IsAbsoluteUri)
                {
                    GetGifStreamFromPack(uri);
                }
                else
                {

                    string leftPart = uri.GetLeftPart(UriPartial.Scheme);

                    if (leftPart == "http://" || leftPart == "ftp://" || leftPart == "file://")
                    {
                        GetGifStreamFromHttp(uri);
                    }
                    else if (leftPart == "pack://")
                    {
                        GetGifStreamFromPack(uri);
                    }
                    else
                    {
                        CreateNonGifAnimationImage();
                    }
                }
            }
            else
            {
                CreateNonGifAnimationImage();
            }
        }

        private delegate void WebRequestFinishedDelegate(MemoryStream memoryStream);

        private void WebRequestFinished(MemoryStream memoryStream)
        {
            CreateGifAnimation(memoryStream);
        }

        private delegate void WebRequestErrorDelegate(Exception exp);

        private void WebRequestError(Exception exp)
        {
            RaiseImageFailedEvent(exp);
        }

        private void WebResponseCallback(IAsyncResult asyncResult)
        {
            WebReadState webReadState = (WebReadState)asyncResult.AsyncState;
            WebResponse webResponse;
            try
            {
                webResponse = webReadState.webRequest.EndGetResponse(asyncResult);
                webReadState.readStream = webResponse.GetResponseStream();
                webReadState.buffer = new byte[100000];
                webReadState.readStream.BeginRead(webReadState.buffer, 0, webReadState.buffer.Length, new AsyncCallback(WebReadCallback), webReadState);
            }
            catch (WebException exp)
            {
                this.Dispatcher.Invoke(DispatcherPriority.Render, new WebRequestErrorDelegate(WebRequestError), exp);
            }
        }

        private void WebReadCallback(IAsyncResult asyncResult)
        {
            WebReadState webReadState = (WebReadState)asyncResult.AsyncState;
            int count = webReadState.readStream.EndRead(asyncResult);
            if (count > 0)
            {
                webReadState.memoryStream.Write(webReadState.buffer, 0, count);
                try
                {
                    webReadState.readStream.BeginRead(webReadState.buffer, 0, webReadState.buffer.Length, new AsyncCallback(WebReadCallback), webReadState);
                }
                catch (WebException exp)
                {
                    this.Dispatcher.Invoke(DispatcherPriority.Render, new WebRequestErrorDelegate(WebRequestError), exp);
                }
            }
            else
            {
                this.Dispatcher.Invoke(DispatcherPriority.Render, new WebRequestFinishedDelegate(WebRequestFinished), webReadState.memoryStream);
            }
        }

        private void GetGifStreamFromHttp(Uri uri)
        {
            try
            {
                WebReadState webReadState = new WebReadState();
                webReadState.memoryStream = new MemoryStream();
                webReadState.webRequest = WebRequest.Create(uri);
                webReadState.webRequest.Timeout = 10000;

                webReadState.webRequest.BeginGetResponse(new AsyncCallback(WebResponseCallback), webReadState);
            }
            catch (SecurityException)
            {
                CreateNonGifAnimationImage();
            }
        }


        private void ReadGifStreamSynch(Stream s)
        {
            byte[] gifData;
            MemoryStream memoryStream;
            using (s)
            {
                memoryStream = new MemoryStream((int)s.Length);
                BinaryReader br = new BinaryReader(s);
                gifData = br.ReadBytes((int)s.Length);
                memoryStream.Write(gifData, 0, (int)s.Length);
                memoryStream.Flush();
            }
            CreateGifAnimation(memoryStream);
        }

        private void GetGifStreamFromPack(Uri uri)
        {
            try
            {
                StreamResourceInfo streamInfo;

                if (!uri.IsAbsoluteUri)
                {
                    streamInfo = Application.GetContentStream(uri);
                    if (streamInfo == null)
                    {
                        streamInfo = Application.GetResourceStream(uri);
                    }
                }
                else
                {
                    if (uri.GetLeftPart(UriPartial.Authority).Contains("siteoforigin"))
                    {
                        streamInfo = Application.GetRemoteStream(uri);
                    }
                    else
                    {
                        streamInfo = Application.GetContentStream(uri);
                        if (streamInfo == null)
                        {
                            streamInfo = Application.GetResourceStream(uri);
                        }
                    }
                }
                if (streamInfo == null)
                {
                    throw new FileNotFoundException("Resource not found.", uri.ToString());
                }
                ReadGifStreamSynch(streamInfo.Stream);
            }
            catch (Exception exp)
            {
                RaiseImageFailedEvent(exp);
            }
        }
    }
}
