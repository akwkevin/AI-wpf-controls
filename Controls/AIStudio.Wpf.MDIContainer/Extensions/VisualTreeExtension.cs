using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace AIStudio.Wpf.MDIContainer.Extensions
{
    internal static class VisualTreeExtension
    {
        public static TParent FindSpecificParent<TParent>(FrameworkElement sender)
           where TParent : FrameworkElement
        {
            var current = sender;
            var p = VisualTreeHelper.GetParent(current) as FrameworkElement;

            if (p != null && p.GetType() != typeof(TParent))
            {
                p = FindSpecificParent<TParent>(p);
            }

            return p as TParent;
        }

        public static MDIWindow FindMDIWindow(FrameworkElement sender)
        {
            return FindSpecificParent<MDIWindow>(sender);
        }

        public static List<T> GetChildObjects<T>(DependencyObject obj, string name = null) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child, ""));
            }
            return childList;
        }
    }
}
