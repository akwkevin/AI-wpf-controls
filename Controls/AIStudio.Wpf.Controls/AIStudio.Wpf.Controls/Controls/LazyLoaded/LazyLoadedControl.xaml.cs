using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// 延迟加载的控件的容器
    /// 将在UI加载之后，在程序空闲时（或者被触发时）将被延迟的控件加载，然后替换当前的控件
    /// </summary>
    [ContentProperty("LazyTemplate")]
    [DefaultProperty("Content")]
    public class LazyLoadedControl : ContentControl
    {
        public class LazyLoadedRoutedEventArgs : RoutedEventArgs
        {
            public FrameworkElement LoadedContent
            {
                get; private set;
            }

            public LazyLoadedRoutedEventArgs(RoutedEvent evt, object obj, FrameworkElement content)
                : base(evt, obj)
            {
                this.LoadedContent = content;
            }
        }

        public delegate void LazyLoadedRoutedEventHandler(object sender, LazyLoadedRoutedEventArgs e);

        #region LazyTemplate 模板。使用模板定义需要延迟加载的控件

        public static readonly DependencyProperty LazyTemplateProperty = DependencyProperty.Register("LazyTemplate", typeof(ControlTemplate), typeof(LazyLoadedControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 模板。使用模板定义需要延迟加载的控件
        /// </summary>
        public ControlTemplate LazyTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(LazyTemplateProperty);
            }
            set
            {
                SetValue(LazyTemplateProperty, value);
            }
        }

        #endregion

        #region CanLoaded 开始加载

        public static readonly DependencyProperty CanLoadedTemplateProperty = DependencyProperty.Register("CanLoaded", typeof(bool), typeof(LazyLoadedControl),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (p, d) => {
                (p as LazyLoadedControl).MakeSureLoaded();
            }));

        /// <summary>
        /// 开始加载
        /// </summary>
        public bool CanLoaded
        {
            get
            {
                return (bool)GetValue(CanLoadedTemplateProperty);
            }
            set
            {
                SetValue(CanLoadedTemplateProperty, value);
            }
        }
        #endregion

        #region WaitInfo
        public static readonly DependencyProperty WaitInfoProperty = DependencyProperty.Register("WaitInfo", typeof(string), typeof(LazyLoadedControl),
          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 等待字符串
        /// </summary>
        public string WaitInfo
        {
            get
            {
                return (string)GetValue(WaitInfoProperty);
            }
            set
            {
                SetValue(WaitInfoProperty, value);
            }
        }
        #endregion

        #region LazyLoaded 是否已经加载

        public static readonly DependencyPropertyKey LazyLoadedPropertyKey = DependencyProperty.RegisterReadOnly("LazyLoaded", typeof(bool), typeof(LazyLoadedControl),
            new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty LazyLoadedProperty
        = LazyLoadedPropertyKey.DependencyProperty;

        /// <summary>
        /// 是否已经加载
        /// </summary>
        public bool LazyLoaded
        {
            get
            {
                return (bool)GetValue(LazyLoadedProperty);
            }
            private set
            {
                SetValue(LazyLoadedPropertyKey, value);
            }
        }
        #endregion

        /// <summary>
        /// 是否已经开始加载
        /// </summary>
        private bool _IsBeginLoad = false;

        /// <summary>
        /// 是否已经卸载
        /// </summary>
        private bool IsDispose = false;

        #region ControlLoadedEvent 控件成功加载路由事件

        public static readonly RoutedEvent ControlLoadedEvent = EventManager.RegisterRoutedEvent("ControlLoaded", RoutingStrategy.Bubble, typeof(LazyLoadedRoutedEventHandler), typeof(LazyLoadedControl));

        public event LazyLoadedRoutedEventHandler ControlLoaded
        {
            add
            {
                AddHandler(ControlLoadedEvent, value);
            }
            remove
            {
                RemoveHandler(ControlLoadedEvent, value);
            }
        }

        #endregion

        public LazyLoadedControl()
        {
            this.Loaded += new RoutedEventHandler(LazyLoadedControl_Loaded);
            this.Unloaded += new RoutedEventHandler(LazyLoadedControl_Unloaded);
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (LazyLoaded || LazyTemplate == null || CanLoaded == false) return;

            Rect rect = GetScreenViewRect(this);

            if (rect.Height > 0 && rect.Width > 0)
                //确保立即加载成功
                MakeSureLoaded();
        }

        private void LazyLoadedControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (LazyLoaded || LazyTemplate == null || CanLoaded == false) return;
            using (new ProcessTime("测试"))
            {
                Window wd = Window.GetWindow(this);
                if (wd != null)
                {
                    wd.Closed -= new EventHandler(wd_Closed);
                    wd.Closed += new EventHandler(wd_Closed);
                }

                //保证在用户操作UI之前加载完毕
                AsynLoadControl(DispatcherPriority.Loaded);
            }
        }

        private void wd_Closed(object sender, EventArgs e)
        {
            IsDispose = true;
        }

        private void LazyLoadedControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //卸载
            //this.Template = null;
            //if (_IsLoaded)
            //    ApplyTemplate();
            //_IsLoaded = false;
            //_IsBeginLoad = false;
        }

        /// <summary>
        /// 同步加载延迟控件
        /// </summary>
        /// <returns></returns>
        private bool LoadDelayControl()
        {
            if (LazyLoaded || LazyTemplate == null || this.Visibility != System.Windows.Visibility.Visible) return true;

            //加载控件
            this.Template = this.LazyTemplate;
            ApplyTemplate();

            //修改加载成功标志
            LazyLoaded = true;
            _IsBeginLoad = false;

            //获取延迟加载的控件对象
            FrameworkElement content = null;
            if (this.VisualChildrenCount > 0)
                content = this.GetVisualChild(0) as FrameworkElement;

            //触发加载事件
            this.RaiseEvent(new LazyLoadedRoutedEventArgs(LazyLoadedControl.ControlLoadedEvent, this, content));

            return LazyLoaded;
        }

        /// <summary>
        /// 确保控件已经加载
        /// </summary>
        /// <returns></returns>
        public bool MakeSureLoaded()
        {
            if (LazyLoaded) return true;

            if (IsDispose)
                return false;

            return LoadDelayControl();
        }

        /// <summary>
        /// 异步加载控件
        /// </summary>
        public void AsynLoadControl(DispatcherPriority dp = DispatcherPriority.ApplicationIdle)
        {
            if (LazyTemplate == null || LazyLoaded || _IsBeginLoad) return;

            _IsBeginLoad = true;

            //低优先级的异步自动加载
            this.Dispatcher.BeginInvoke((Action)delegate {
                if (IsDispose)
                    return;

                LoadDelayControl();
            }, dp);
        }


        /// <summary>
        /// 获得当前控件在屏幕上的可视区域
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.DebuggerStepThrough]
        public Rect GetScreenViewRect(System.Windows.FrameworkElement element)
        {
            if (element == null) return new Rect();
            PresentationSource ps = HwndSource.FromVisual(element);
            if (ps == null) return new Rect();

            GeneralTransform transformToRoot = element.TransformToAncestor(ps.RootVisual);

            Rect screenRect = new Rect(transformToRoot.Transform(new Point(0, 0)), transformToRoot.Transform(new Point(element.ActualWidth, element.ActualHeight)));

            DependencyObject parent = VisualTreeHelper.GetParent(element);

            while (parent != null)
            {
                System.Windows.Controls.Control control = parent as System.Windows.Controls.Control;

                if (control != null && control is Visual && (control is Window || control.ClipToBounds))
                {
                    transformToRoot = (control as Visual).TransformToAncestor(ps.RootVisual);

                    Point pointAncestorTopLeft = transformToRoot.Transform(new Point(0, 0));
                    Point pointAncestorBottomRight = transformToRoot.Transform(new Point(control.ActualWidth, control.ActualHeight));

                    if (control is System.Windows.Controls.ScrollViewer)
                    {
                        System.Windows.Controls.ScrollViewer sv = control as System.Windows.Controls.ScrollViewer;
                        if (sv.HorizontalScrollBarVisibility == System.Windows.Controls.ScrollBarVisibility.Visible
                            || (sv.HorizontalScrollBarVisibility == System.Windows.Controls.ScrollBarVisibility.Auto
                                && sv.ComputedHorizontalScrollBarVisibility == System.Windows.Visibility.Visible)
                            )
                        {
                            pointAncestorBottomRight.Y -= System.Windows.SystemParameters.HorizontalScrollBarHeight;
                        }

                        if (sv.VerticalScrollBarVisibility == System.Windows.Controls.ScrollBarVisibility.Visible
                            || (sv.VerticalScrollBarVisibility == System.Windows.Controls.ScrollBarVisibility.Auto
                                && sv.ComputedVerticalScrollBarVisibility == System.Windows.Visibility.Visible)
                            )
                        {
                            pointAncestorBottomRight.X -= System.Windows.SystemParameters.VerticalScrollBarWidth;
                        }
                    }

                    Rect ancestorRect = new Rect(pointAncestorTopLeft, pointAncestorBottomRight);

                    screenRect.Intersect(ancestorRect);
                }

                parent = VisualTreeHelper.GetParent(parent);

            }

            return screenRect;
        }
    }
}
