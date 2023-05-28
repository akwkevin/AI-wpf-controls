using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class BlockContentControl : ContentControl
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
         nameof(Header), typeof(object), typeof(BlockContentControl), new PropertyMetadata(default(object)));

        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            nameof(HeaderTemplate), typeof(DataTemplate), typeof(BlockContentControl), new PropertyMetadata(default(DataTemplate)));

        [Bindable(true), Category("Content")]
        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public static readonly DependencyProperty HeaderTemplateSelectorProperty = DependencyProperty.Register(
            nameof(HeaderTemplateSelector), typeof(DataTemplateSelector), typeof(BlockContentControl), new PropertyMetadata(default(DataTemplateSelector)));

        [Bindable(true), Category("Content")]
        public DataTemplateSelector HeaderTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty);
            set => SetValue(HeaderTemplateSelectorProperty, value);
        }

        public static readonly DependencyProperty HeaderStringFormatProperty = DependencyProperty.Register(
            nameof(HeaderStringFormat), typeof(string), typeof(BlockContentControl), new PropertyMetadata(default(string)));

        [Bindable(true), Category("Content")]
        public string HeaderStringFormat
        {
            get => (string)GetValue(HeaderStringFormatProperty);
            set => SetValue(HeaderStringFormatProperty, value);
        }

        public static readonly DependencyProperty FooterProperty = DependencyProperty.Register(
            nameof(Footer), typeof(object), typeof(BlockContentControl), new PropertyMetadata(default(object)));

        public object Footer
        {
            get => GetValue(FooterProperty);
            set => SetValue(FooterProperty, value);
        }

        public static readonly DependencyProperty FooterTemplateProperty = DependencyProperty.Register(
            nameof(FooterTemplate), typeof(DataTemplate), typeof(BlockContentControl), new PropertyMetadata(default(DataTemplate)));

        [Bindable(true), Category("Content")]
        public DataTemplate FooterTemplate
        {
            get => (DataTemplate)GetValue(FooterTemplateProperty);
            set => SetValue(FooterTemplateProperty, value);
        }

        public static readonly DependencyProperty FooterTemplateSelectorProperty = DependencyProperty.Register(
            nameof(FooterTemplateSelector), typeof(DataTemplateSelector), typeof(BlockContentControl), new PropertyMetadata(default(DataTemplateSelector)));

        [Bindable(true), Category("Content")]
        public DataTemplateSelector FooterTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(FooterTemplateSelectorProperty);
            set => SetValue(FooterTemplateSelectorProperty, value);
        }

        public static readonly DependencyProperty FooterStringFormatProperty = DependencyProperty.Register(
            nameof(FooterStringFormat), typeof(string), typeof(BlockContentControl), new PropertyMetadata(default(string)));

        [Bindable(true), Category("Content")]
        public string FooterStringFormat
        {
            get => (string)GetValue(FooterStringFormatProperty);
            set => SetValue(FooterStringFormatProperty, value);
        }

        public BlockContentControl()
        {

        }    
       

        protected Brush CreateFillBrush()
        {
            Brush result = null;

            GradientStopCollection gsc = new GradientStopCollection();
            gsc.Add(new GradientStop(((SolidColorBrush)this.Background).Color, 0));
            LinearGradientBrush backGroundBrush = new LinearGradientBrush(gsc, new Point(0, 0), new Point(0, 1));
            result = backGroundBrush;

            return result;
        }
    }
}
