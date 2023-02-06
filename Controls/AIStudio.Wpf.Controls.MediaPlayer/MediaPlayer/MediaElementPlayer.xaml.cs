using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AIStudio.Wpf.Controls.MediaPlayer.Commands;
using Microsoft.Win32;

namespace AIStudio.Wpf.Controls.MediaPlayer
{
    [TemplatePart(Name = PART_MediaElement, Type = typeof(MediaElement))]
    [TemplatePart(Name = PART_Slider_Progress, Type = typeof(Slider))]
    [TemplatePart(Name = PART_OpenMenuItem, Type = typeof(MenuItem))]
    public class MediaElementPlayer : Control
    {
        private const string PART_MediaElement = "PART_MediaElement";
        private const string PART_Slider_Progress = "PART_Slider_Progress";
        private const string PART_OpenMenuItem = "PART_OpenMenuItem";

        MediaElement _mediaElement;
        Slider _slider_Progress;
        MenuItem _openMenuItem;
        DispatcherTimer _timer = null;

        #region Volume 音量
        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register(
            "Volume", typeof(double), typeof(MediaElementPlayer), new PropertyMetadata(0.7, OnVolumePropertyCallback));

        public double Volume
        {
            get
            {
                return (double)GetValue(VolumeProperty);
            }
            set
            {
                SetValue(VolumeProperty, value);
            }
        }

        private static void OnVolumePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var player = (MediaElementPlayer)d;
            if (e.NewValue != e.OldValue)
            {
                player.OnVolumePropertyCallback((double)e.NewValue);
            }
        }

        private void OnVolumePropertyCallback(double volume)
        {
            _mediaElement.Volume = volume;
        }
        #endregion

        #region TotalSeconds 总时间
        public static readonly DependencyProperty TotalSecondsProperty = DependencyProperty.Register(
            "TotalSeconds", typeof(double), typeof(MediaElementPlayer), new PropertyMetadata(0.0));

        public double TotalSeconds
        {
            get
            {
                return (double)GetValue(TotalSecondsProperty);
            }
            set
            {
                SetValue(TotalSecondsProperty, value);
            }
        }
        #endregion

        #region Position 当前时间
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position", typeof(double), typeof(MediaElementPlayer), new PropertyMetadata(0.0));

        public double Position
        {
            get
            {
                return (double)GetValue(PositionProperty);
            }
            set
            {
                SetValue(PositionProperty, value);
            }
        }
        #endregion

        #region PositionTime 当前时间
        public static readonly DependencyProperty PositionTimeProperty = DependencyProperty.Register(
            "PositionTime", typeof(string), typeof(MediaElementPlayer), new PropertyMetadata("00:00"));

        public string PositionTime
        {
            get
            {
                return (string)GetValue(PositionTimeProperty);
            }
            set
            {
                SetValue(PositionTimeProperty, value);
            }
        }
        #endregion

        #region TotalSecondsTime 总时间
        public static readonly DependencyProperty TotalSecondsTimeProperty = DependencyProperty.Register(
            "TotalSecondsTime", typeof(string), typeof(MediaElementPlayer), new PropertyMetadata("00:00"));

        public string TotalSecondsTime
        {
            get
            {
                return (string)GetValue(TotalSecondsTimeProperty);
            }
            set
            {
                SetValue(TotalSecondsTimeProperty, value);
            }
        }
        #endregion

        #region PlayStatus 播放状态
        public static readonly DependencyProperty PlayStatusProperty = DependencyProperty.Register(
            "PlayStatus", typeof(PlayStatus), typeof(MediaElementPlayer), new PropertyMetadata(PlayStatus.Stop));

        public PlayStatus PlayStatus
        {
            get
            {
                return (PlayStatus)GetValue(PlayStatusProperty);
            }
            set
            {
                SetValue(PlayStatusProperty, value);
            }
        }
        #endregion

        #region IsMute 是否静音
        public static readonly DependencyProperty IsMuteProperty = DependencyProperty.Register(
            "IsMute", typeof(bool), typeof(MediaElementPlayer), new PropertyMetadata(false));

        public bool IsMute
        {
            get
            {
                return (bool)GetValue(IsMuteProperty);
            }
            set
            {
                SetValue(IsMuteProperty, value);
            }
        }
        #endregion

        #region PlayMode 播放模式
        public static readonly DependencyProperty PlayModeProperty = DependencyProperty.Register(
            "PlayMode", typeof(PlayMode), typeof(MediaElementPlayer), new PropertyMetadata(PlayMode.Sequence));

        public PlayMode PlayMode
        {
            get
            {
                return (PlayMode)GetValue(PlayModeProperty);
            }
            set
            {
                SetValue(PlayModeProperty, value);
            }
        }
        #endregion

        #region ShowMode 
        public static readonly DependencyProperty ShowModeProperty = DependencyProperty.Register(
            "ShowMode", typeof(ShowMode), typeof(MediaElementPlayer), new PropertyMetadata(ShowMode.PathMode));

        public ShowMode ShowMode
        {
            get
            {
                return (ShowMode)GetValue(ShowModeProperty);
            }
            set
            {
                SetValue(ShowModeProperty, value);
            }
        }
        #endregion

        #region MusicPath 音乐路径(简约模式)
        public static readonly DependencyProperty MusicPathProperty = DependencyProperty.Register(
            "MusicPath", typeof(string), typeof(MediaElementPlayer), new PropertyMetadata(default(string), OnMusicPathPropertyCallback));

        public string MusicPath
        {
            get
            {
                return (string)GetValue(MusicPathProperty);
            }
            set
            {
                SetValue(MusicPathProperty, value);
            }
        }

        private static void OnMusicPathPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var player = (MediaElementPlayer)d;
            if (e.NewValue != e.OldValue)
            {
                player.OnMusicPathPropertyCallback((string)e.NewValue);
            }
        }

        private void OnMusicPathPropertyCallback(string path)
        {
            Play(path);
        }
        #endregion

        #region PathList 路径列表(简约模式)
        public static readonly DependencyProperty PathListProperty = DependencyProperty.Register(
            "PathList", typeof(ObservableCollection<string>), typeof(MediaElementPlayer), new PropertyMetadata(null));

        public ObservableCollection<string> PathList
        {
            get
            {
                return (ObservableCollection<string>)GetValue(PathListProperty);
            }
            set
            {
                SetValue(PathListProperty, value);
            }
        }
        #endregion

        #region MusicInfo 播放音乐
        public static readonly DependencyProperty MusicInfoProperty = DependencyProperty.Register(
            "MusicInfo", typeof(ObservableCollection<MusicInfo>), typeof(MediaElementPlayer), new PropertyMetadata(null, OnMusicInfoPropertyCallback));

        public MusicInfo MusicInfo
        {
            get
            {
                return (MusicInfo)GetValue(MusicInfoProperty);
            }
            set
            {
                SetValue(MusicInfoProperty, value);
            }
        }

        private static void OnMusicInfoPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var player = (MediaElementPlayer)d;
            if (e.NewValue != e.OldValue)
            {
                MusicInfo info = e.NewValue as MusicInfo;
                player.MusicPath = info == null ? string.Empty : info.FullPath;
            }
        }
        #endregion

        #region MusicList 音乐列表
        public static readonly DependencyProperty MusicListProperty = DependencyProperty.Register(
            "MusicList", typeof(ObservableCollection<MusicInfo>), typeof(MediaElementPlayer), new PropertyMetadata(null));

        public ObservableCollection<MusicInfo> MusicList
        {
            get
            {
                return (ObservableCollection<MusicInfo>)GetValue(MusicListProperty);
            }
            set
            {
                SetValue(MusicListProperty, value);
            }
        }
        #endregion



        #region SliderForeground 滑块颜色
        public static readonly DependencyProperty SliderForegroundProperty = DependencyProperty.Register(
            nameof(SliderForeground), typeof(Brush), typeof(MediaElementPlayer), new PropertyMetadata(default(Brush)));

        public Brush SliderForeground
        {
            get
            {
                return (Brush)GetValue(SliderForegroundProperty);
            }
            set
            {
                SetValue(SliderForegroundProperty, value);
            }
        }
        #endregion

        #region Command

        public ICommand PreCommand => new Lazy<RelayCommand>(() => new RelayCommand(Pre_Execute)).Value;

        public ICommand PlayCommand => new Lazy<RelayCommand>(() =>
            new RelayCommand(Play_Execute)).Value;

        public ICommand NextCommand => new Lazy<RelayCommand>(() =>
            new RelayCommand(Next_Execute)).Value;

        public ICommand MuteCommand => new Lazy<RelayCommand>(() =>
            new RelayCommand(Mute_Execute)).Value;

        public ICommand PlayModeCommand => new Lazy<RelayCommand>(() =>
            new RelayCommand(PlayMode_Execute)).Value;
        #endregion

        static MediaElementPlayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaElementPlayer), new FrameworkPropertyMetadata(typeof(MediaElementPlayer)));
        }

        public MediaElementPlayer()
        {
            this.Unloaded += MediaElementPlayer_Unloaded;
        }

        private void MediaElementPlayer_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _mediaElement = GetTemplateChild(PART_MediaElement) as MediaElement;
            _slider_Progress = GetTemplateChild(PART_Slider_Progress) as Slider;
            _openMenuItem = GetTemplateChild(PART_OpenMenuItem) as MenuItem;

            _mediaElement.MediaOpened += _mediaElement_MediaOpened;
            _mediaElement.MediaEnded += _mediaElement_MediaEnded;
            _mediaElement.MediaFailed += _mediaElement_MediaFailed;
            _openMenuItem.Click += _openMenuItem_Click;

            _slider_Progress.ValueChanged += _slider_Progress_ValueChanged;
            Play(MusicPath);
        }

        private void _openMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "所有文件|*.*";

            if (ofd.ShowDialog() == true)
            {
                MusicPath = ofd.FileName;
            }

        }

        private void Play(string path)
        {
            try
            {
                if (_mediaElement == null)
                    return;

                if (!string.IsNullOrEmpty(path))
                {
                    _mediaElement.Source = new Uri(path, UriKind.Absolute);

                    _mediaElement.Play();
                    if (PlayStatus == PlayStatus.Stop)
                    {
                        _mediaElement.Stop();
                        PlayStatus = PlayStatus.Stop;
                    }
                    else
                    {
                        PlayStatus = PlayStatus.Play;
                    }
                }
                else
                {
                    PlayStatus = PlayStatus.Stop;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                PlayStatus = PlayStatus.Stop;
            }
        }

        private void _slider_Progress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _mediaElement.Position = TimeSpan.FromSeconds(_slider_Progress.Value);
        }

        private void _mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            TotalSeconds = _mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            TotalSecondsTime = _mediaElement.NaturalDuration.TimeSpan.ToString("mm\\:ss");
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += new EventHandler(timer_tick);
                _timer.Start();
            }
        }

        private void _mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (PlayMode == PlayMode.Looping)
            {
                (sender as MediaElement).Stop();
                (sender as MediaElement).Play();
            }
            else
            {
                Next_Execute();
            }
        }

        private void _mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            PlayStatus = PlayStatus.Stop;
        }

        private void timer_tick(object sender, EventArgs e)
        {
            Position = _mediaElement.Position.TotalSeconds;
            PositionTime = _mediaElement.Position.ToString("mm\\:ss");
        }

        private void Play_Execute()
        {
            if (PlayStatus == PlayStatus.Stop)
            {
                _mediaElement.Play();
                PlayStatus = PlayStatus.Play;
            }
            else
            {
                _mediaElement.Pause();
                PlayStatus = PlayStatus.Stop;
            }
        }

        private void Pre_Execute()
        {
            if (ShowMode.ToString().StartsWith("Path"))
            {
                if (PathList != null && PathList.Count > 0)
                {
                    if (PlayMode == PlayMode.Looping || PlayMode == PlayMode.Sequence)
                    {
                        if (string.IsNullOrEmpty(MusicPath))
                        {
                            MusicPath = PathList.FirstOrDefault();
                        }
                        else
                        {
                            int index = PathList.IndexOf(MusicPath);
                            index--;
                            if (index > 0)
                            {
                                MusicPath = PathList[index];
                            }
                        }
                    }
                    else
                    {
                        Random rd = new Random();
                        int index = rd.Next(0, PathList.Count - 1);
                        MusicPath = PathList[index];
                    }
                }
            }
            else
            {
                if (MusicList != null && MusicList.Count > 0)
                {
                    if (PlayMode == PlayMode.Looping || PlayMode == PlayMode.Sequence)
                    {
                        if (MusicInfo == null)
                        {
                            MusicInfo = MusicList.FirstOrDefault();
                        }
                        else
                        {
                            int index = MusicList.IndexOf(MusicInfo);
                            index--;
                            if (index > 0)
                            {
                                MusicInfo = MusicList[index];
                            }
                        }
                    }
                    else
                    {
                        Random rd = new Random();
                        int index = rd.Next(0, MusicList.Count - 1);
                        MusicInfo = MusicList[index];
                    }
                }
            }
        }

        private void Next_Execute()
        {
            if (ShowMode.ToString().StartsWith("Path"))
            {
                if (PathList != null && PathList.Count > 0)
                {
                    if (PlayMode == PlayMode.Looping || PlayMode == PlayMode.Sequence)
                    {
                        if (string.IsNullOrEmpty(MusicPath))
                        {
                            MusicPath = PathList.FirstOrDefault();
                        }
                        else
                        {
                            int index = PathList.IndexOf(MusicPath);
                            index++;
                            if (index < PathList.Count)
                            {
                                MusicPath = PathList[index];
                            }
                        }
                    }
                    else
                    {
                        Random rd = new Random();
                        int index = rd.Next(0, PathList.Count - 1);
                        MusicPath = PathList[index];
                    }
                }
            }
            else
            {
                if (MusicList != null && MusicList.Count > 0)
                {

                    if (PlayMode == PlayMode.Looping || PlayMode == PlayMode.Sequence)
                    {
                        if (MusicInfo == null)
                        {
                            MusicInfo = MusicList.FirstOrDefault();
                        }
                        else
                        {
                            int index = MusicList.IndexOf(MusicInfo);
                            index++;
                            if (index < MusicList.Count)
                            {
                                MusicInfo = MusicList[index];
                            }
                        }
                    }
                    else
                    {
                        Random rd = new Random();
                        int index = rd.Next(0, MusicList.Count - 1);
                        MusicInfo = MusicList[index];
                    }
                }
            }
        }

        private void PlayMode_Execute()
        {
            if (PlayMode == PlayMode.Sequence)
            {
                PlayMode = PlayMode.Looping;
            }
            else if (PlayMode == PlayMode.Looping)
            {
                PlayMode = PlayMode.Randomly;
            }
            else if (PlayMode == PlayMode.Randomly)
            {
                PlayMode = PlayMode.Sequence;
            }
        }

        private double oldVolume;
        private void Mute_Execute()
        {
            IsMute = !IsMute;
            if (IsMute)
            {
                oldVolume = Volume;
                Volume = 0;
            }
            else
            {
                Volume = oldVolume;
            }
        }
    }

    public enum PlayStatus
    {
        Play,
        Stop,
    }

    public enum PlayMode
    {
        Sequence,
        Looping,
        Randomly
    }

    public enum ShowMode
    {
        PathMode,
        PathFullMode,
        PathVideoMode,
        PathVideoFullMode,
        MusicInfoMode,
        MusicInfoFullMode,
        MusicInfoVideoMode,
        MusicInfoVideoFullMode,
    }

    /// <summary>
    /// 音乐、歌曲对象
    /// </summary>
    public class MusicInfo
    {
        public MusicInfo(string fullPath)
        {
            TagLib.File tagFile = TagLib.File.Create(fullPath);
            Name = tagFile.Tag.Title;
            Album = tagFile.Tag.Album;
            Artist = tagFile.Tag.FirstAlbumArtist;
            Duration = (int)tagFile.Properties.Duration.TotalSeconds;
            DurationText = Second2MinuteSecond(Duration);
        }



        public string _id;
        public string Id // 唯一标识符，防止重复添加同一歌曲。生成规则是 Md5(Name + Artist + Album + Duration)
        {
            get
            {
                if (_id == null)
                    _id = CreateMD5(Name + Artist + Album + Duration);
                return _id;
            }
        }
        public int Index
        {
            get; set;
        }      // 在播放列表中的角标序号
        public string Name
        {
            get; set;
        }    // 歌曲名
        public string Artist
        {
            get; set;
        }  // 艺术家，歌手名
        public string Album
        {
            get; set;
        }   // 专辑
        public int Duration
        {
            get; set;
        }   // 时长，秒数
        public string FullPath
        {
            get; set;
        }        // 在本地的全路径。保存到本地数据库中。
        public string DurationText
        {
            get; set;
        }    // 附加属性。时长，用于UI显示。格式为 02：34

        public string Second2MinuteSecond(int sec)
        {
            TimeSpan time = TimeSpan.FromSeconds(sec);
            return time.ToString(@"mm\:ss");
        }

        public string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

    }


}
