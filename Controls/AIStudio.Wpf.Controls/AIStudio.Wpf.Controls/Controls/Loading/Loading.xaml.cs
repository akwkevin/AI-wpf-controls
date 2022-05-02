using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AIStudio.Wpf.Controls.Media.Animation;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_Path, Type = typeof(Path))]
    public class Loading : Control
    {
        private const string PART_Path = "PART_Path";

        private Path _path;

        #region Constructor
        static Loading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Loading), new FrameworkPropertyMetadata(typeof(Loading)));
        }

        public Loading()
        {
            this.Loaded += Loading_Loaded;
        }

        private void Loading_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= Loading_Loaded;
            TransPathStoryboard();
        }
        #endregion

        #region Property
        /// <summary>
        /// 获取或设置加载控件的状态。默认为False。
        /// </summary>
        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        public static readonly DependencyProperty IsRunningProperty =
            DependencyProperty.Register(nameof(IsRunning), typeof(bool), typeof(Loading), new PropertyMetadata(true, OnIsRunningChanged));

        private static void OnIsRunningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Loading loading)
            {
                loading.TransPathStoryboard();
            }
        }

        public void TransPathStoryboard()
        {
            if (LoadingStyle == LoadingStyle.TransPath)
            {
                var storyboard = new Storyboard() { RepeatBehavior = RepeatBehavior.Forever, AutoReverse = true };
                GeometryAnimationUsingKeyFrames frames = new GeometryAnimationUsingKeyFrames();
                Storyboard.SetTargetProperty(frames, new PropertyPath("Data"));
                Storyboard.SetTarget(frames, _path);

                DiscreteGeometryKeyFrame keyFrame1 = new DiscreteGeometryKeyFrame(GetValue(IconAttach.GeometryProperty) as Geometry, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.7)));
                frames.KeyFrames.Add(keyFrame1);

                EasingGeometryKeyFrame keyFrame2 = new EasingGeometryKeyFrame(GetValue(IconAttach.GeometrySelectedProperty) as Geometry, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1.2)));
                keyFrame2.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseInOut };
                frames.KeyFrames.Add(keyFrame2);

                storyboard.Children.Add(frames);

                storyboard.Begin();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _path = GetTemplateChild(PART_Path) as Path;
        }

        //public double SpeedRatio
        //{
        //    get => (double)GetValue(SpeedRatioProperty);
        //    set => SetValue(SpeedRatioProperty, value);
        //}

        //public static readonly DependencyProperty SpeedRatioProperty =
        //    DependencyProperty.Register(nameof(SpeedRatio), typeof(double), typeof(Loading), new PropertyMetadata(1d, OnSpeedRatioChanged));

        //private static void OnSpeedRatioChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    var li = (Loading)o;
        //}

        /// <summary>
        /// 获取或设置加载控件的基础样式。默认为Standard。
        /// </summary>
        public LoadingStyle LoadingStyle
        {
            get => (LoadingStyle)GetValue(LoadingStyleProperty);
            set => SetValue(LoadingStyleProperty, value);
        }

        public static readonly DependencyProperty LoadingStyleProperty =
            DependencyProperty.Register(nameof(LoadingStyle), typeof(LoadingStyle), typeof(Loading), new PropertyMetadata(LoadingStyle.Standard));
        #endregion

        #region Internal Property
        internal double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        internal static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(Loading), new PropertyMetadata(0.0));

        internal double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        internal static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(Loading), new PropertyMetadata(100.0));

        internal double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        internal static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(Loading), new PropertyMetadata(0.0));
        #endregion

        #region Calling Methods
        //public ILoadingHandler Show(string message, string title = null)
        //{

        //}
        #endregion
    }


}
