using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls.Helper
{
    public class ThemeChangedHelper
    {
        #region ThemeChangedHelper

        private static HashSet<DependencyObject> AcceptThemeObject = new HashSet<DependencyObject>();

        public static void SetAcceptThemeChanged(DependencyObject element, bool value)
        {
            element.SetValue(AcceptThemeChangedProperty, value);
        }

        public static bool GetAcceptThemeChanged(DependencyObject element)
        {
            return (bool)element.GetValue(AcceptThemeChangedProperty);
        }

        public static readonly DependencyProperty AcceptThemeChangedProperty =
            DependencyProperty.RegisterAttached("AcceptThemeChanged", typeof(bool), typeof(ThemeChangedHelper), new PropertyMetadata(false, OnAcceptThemeChanged));
        protected static void OnAcceptThemeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {

                if ((bool)e.NewValue == true)
                {
                    AcceptThemeObject.Add(sender);
                }
                else
                {
                    AcceptThemeObject.Remove(sender);
                }
            }
            catch { }
        }

        private static BindingFlags _bindingFlags { get; } = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

        private static void AccentChanged()
        {
            foreach(var obj in AcceptThemeObject)
            {
                if (obj.GetType().GetProperty("IsThemeChanged", _bindingFlags) != null)
                {
                    obj.GetType().GetProperty("IsThemeChanged", _bindingFlags).SetValue(obj, false);
                    obj.GetType().GetProperty("IsThemeChanged", _bindingFlags).SetValue(obj, true);
                }
            }
        }


        #endregion

        static ThemeChangedHelper()
        {
            IsThemeChanged += AccentChanged;
        }
        public static Action IsThemeChanged { get; set; }
    }
}
