using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AIStudio.Wpf.Controls.Helper
{
    public static class ScrollViewerHelper
    {
        public static void AnimateScrollIntoVerticalOffset(this ScrollViewer scrollViewer, double offset, double milliseconds)
        {
            DoubleAnimation verticalAnimation = new DoubleAnimation(scrollViewer.VerticalOffset, offset, new Duration(TimeSpan.FromMilliseconds(milliseconds)));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(verticalAnimation);
            Storyboard.SetTarget(verticalAnimation, scrollViewer);
            Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollViewerAttach.VerticalOffsetProperty));
            storyboard.Begin();
        }
    }
}
