﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AIStudio.Wpf.Controls.Helper
{
    /// <summary>
    /// ItemsControl扩展方法
    /// </summary>
    public static class ItemsControlHelper
    {
        /// <summary>
        /// 平滑滚动至指定元素
        /// </summary>
        /// <param name="itemsControl"></param>
        /// <param name="item"></param>
        public static void AnimateScrollIntoView(this ItemsControl itemsControl, object item)
        {
            ScrollViewer scrollViewer = VisualHelper.FindVisualChild<ScrollViewer>(itemsControl);
            if (scrollViewer == null)
            {
                return;
            }

            UIElement container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as UIElement;

            if (container == null)
            {
                return;
            }

            int index = itemsControl.ItemContainerGenerator.IndexFromContainer(container);

            //平滑滚动到元素所在位置
            //double toValue = scrollViewer.ScrollableHeight * ((double)index / itemsControl.Items.Count);
            //平滑滚动，将选中的元素置顶
            double toValue = VisualTreeHelper.GetOffset(container).Y;

            DoubleAnimation verticalAnimation = new DoubleAnimation();
            verticalAnimation.From = scrollViewer.VerticalOffset;
            verticalAnimation.To = toValue;
            verticalAnimation.DecelerationRatio = .2;
            verticalAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(verticalAnimation);
            Storyboard.SetTarget(verticalAnimation, scrollViewer);
            Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollViewerAttach.VerticalOffsetProperty));
            storyboard.Begin();
        }


        /// <summary>
        /// 平滑滚动至指定元素
        /// </summary>
        /// <param name="itemsControl"></param>
        /// <param name="item"></param>
        public static void AnimateScrollIntoView(this ItemsControl itemsControl, int index)
        {
            ScrollViewer scrollViewer = VisualHelper.FindVisualChild<ScrollViewer>(itemsControl);
            if (scrollViewer == null)
            {
                return;
            }

            DoubleAnimation verticalAnimation = new DoubleAnimation();
            verticalAnimation.From = scrollViewer.VerticalOffset;
            verticalAnimation.To = index;
            verticalAnimation.DecelerationRatio = .2;
            verticalAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(verticalAnimation);
            Storyboard.SetTarget(verticalAnimation, scrollViewer);
            Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollViewerAttach.VerticalOffsetProperty));
            storyboard.Begin();
        }
    }


}
