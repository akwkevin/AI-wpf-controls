﻿using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// 导航条控件：用于实现类似html中的锚的快速定位功能
    /// </summary>
    /// <remarks>add by zhidf 2016.8.21</remarks>
    [TemplatePart(Name = "PART_LeftLine", Type = typeof(Border))]
    [TemplatePart(Name = "PART_RightLine", Type = typeof(Border))]
    public class NavigationBar : ListBox
    {
        static NavigationBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationBar), new FrameworkPropertyMetadata(typeof(NavigationBar)));
        }

        #region 依赖属性
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
           nameof(Orientation), typeof(Orientation), typeof(NavigationBar), new FrameworkPropertyMetadata(default(Orientation)));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public static readonly DependencyProperty BindScrollViewerProperty = DependencyProperty.Register(nameof(BindScrollViewer)
            , typeof(ScrollViewer), typeof(NavigationBar));

        /// <summary>
        /// 待导航区域所在的ScrollViewer
        /// </summary>
        public ScrollViewer BindScrollViewer
        {
            get
            {
                return (ScrollViewer)GetValue(BindScrollViewerProperty);
            }
            set
            {
                SetValue(BindScrollViewerProperty, value);
            }
        }

        public static readonly DependencyProperty BindNavigationControlProperty = DependencyProperty.Register(nameof(BindNavigationControl)
            , typeof(Panel), typeof(NavigationBar));

        /// <summary>
        /// 待导航界面所在的容器
        /// </summary>
        public Panel BindNavigationControl
        {
            get
            {
                return (Panel)GetValue(BindNavigationControlProperty);
            }
            set
            {
                SetValue(BindNavigationControlProperty, value);
            }
        }
        #endregion

        public NavigationBar() : base()
        {
            this.Loaded += NavigationBar_Loaded;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            ListBoxItem item = new ListBoxItem();
            //给Item增加鼠标左键单击事件，不使用SelectionChanged事件
            item.MouseLeftButtonUp += Item_MouseLeftButtonUp;
            return item;
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            var items = this.Items;
        }

        private void Item_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!this.Check()) return;
            ScrollToSelection(((System.Windows.Controls.ContentControl)sender).Content);
        }

        private void NavigationBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.SelectedIndex == -1)
            {
                this.SelectedIndex = 0;
            }

            if (!this.Check()) return;

            if (this.BindScrollViewer != null)
            {
                this.BindScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }

            if (this.SelectedIndex != -1 && this.SelectedIndex < this.Items.Count)
            {
                this.ScrollToSelection(this.Items[this.SelectedIndex]);
            }

            this.RemoveLeftLine();
            this.RemoveRightLine();
        }

        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);

            this.SelectedIndex = 0;
            this.RemoveLeftLine();
            this.RemoveRightLine();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!this.Check()) return;

            var verticalOffset = this.BindScrollViewer.VerticalOffset;
            if (verticalOffset > 0)
            {
                double scrollOffset = 0.0;
                for (int i = 0; i < this.BindNavigationControl.Children.Count; i++)
                {
                    var child = this.BindNavigationControl.Children[i];
                    if (child is FrameworkElement)
                    {
                        FrameworkElement element = child as FrameworkElement;
                        if (element == null) return;

                        scrollOffset += element.ActualHeight;

                        if (scrollOffset > verticalOffset && i < this.Items.Count)
                        {
                            this.SelectedItem = this.Items[i];
                            break;
                        }
                    }
                }
            }
        }

        private bool Check()
        {
            bool flag = true;
            if (this.BindScrollViewer == null)
            {
                flag = false;
            }

            if (this.BindNavigationControl == null)
            {
                flag = false;
            }

            if (_interChanged)
            {
                flag = false;
            }
            return flag;
        }

        bool _interChanged = false;
        /// <summary>
        /// 滚动至指定Item
        /// </summary>
        /// <param name="selection"></param>
        private void ScrollToSelection(object selection)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i] == selection)
                {
                    if (i < this.BindNavigationControl.Children.Count)
                    {
                        var vector = VisualTreeHelper.GetOffset(this.BindNavigationControl.Children[i]);


                        DoubleAnimation verticalAnimation = new DoubleAnimation(this.BindScrollViewer.VerticalOffset, vector.Y, new Duration(TimeSpan.FromMilliseconds(500)));
                        Storyboard storyboard = new Storyboard();
                        storyboard.Children.Add(verticalAnimation);
                        Storyboard.SetTarget(verticalAnimation, this.BindScrollViewer);
                        Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollViewerAttach.VerticalOffsetProperty));

                        _interChanged = true;
                        storyboard.Completed += (e, args) => {
                            _interChanged = false;
                        };
                        storyboard.Begin();

                        //this.BindScrollViewer.ScrollToVerticalOffset(vector.Y);
                    }
                }
            }
        }

        /// <summary>
        /// 隐藏最左边的线
        /// </summary>
        private void RemoveLeftLine()
        {
            ListBoxItem item = (ListBoxItem)this.ItemContainerGenerator.ContainerFromIndex(0);
            if (item != null)
            {
                var border = VisualHelper.FindChild<Border>(item, "PART_LeftLine");
                if (border != null)
                {
                    border.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 隐藏最右边的线
        /// </summary>
        private void RemoveRightLine()
        {
            ListBoxItem item = (ListBoxItem)this.ItemContainerGenerator.ContainerFromIndex(this.Items.Count - 1);
            if (item != null)
            {
                var border = VisualHelper.FindChild<Border>(item, "PART_RightLine");
                if (border != null)
                {
                    border.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
