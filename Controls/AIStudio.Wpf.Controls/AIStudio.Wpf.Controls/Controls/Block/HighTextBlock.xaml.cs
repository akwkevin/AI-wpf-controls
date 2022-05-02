using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// FImage.xaml 的交互逻辑
    /// 注意：目前绑定TextBlock是在Keyword值变更时绑定的。
    /// </summary>
    public partial class HighTextBlock : TextBlock
    {
        public static readonly DependencyProperty HighForegroundProperty = DependencyProperty.Register(
            nameof(HighForeground), typeof(Brush), typeof(HighTextBlock), new PropertyMetadata(default(Brush), OnContentPropertyChanged));

        public Brush HighForeground
        {
            get
            {
                return (Brush)GetValue(HighForegroundProperty);
            }
            set
            {
                SetValue(HighForegroundProperty, value);
            }
        }

        public static readonly DependencyProperty HighBackgroundProperty = DependencyProperty.Register(
            nameof(HighBackground), typeof(Brush), typeof(HighTextBlock), new PropertyMetadata(default(Brush), OnContentPropertyChanged));

        public Brush HighBackground
        {
            get
            {
                return (Brush)GetValue(HighBackgroundProperty);
            }
            set
            {
                SetValue(HighBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty KeywordProperty = DependencyProperty.Register(
            nameof(Keyword), typeof(string), typeof(HighTextBlock), new PropertyMetadata(string.Empty, OnContentPropertyChanged));
        public string Keyword
        {
            get
            {
                return (string)GetValue(KeywordProperty);
            }
            set
            {
                SetValue(KeywordProperty, value);
            }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content), typeof(string), typeof(HighTextBlock), new PropertyMetadata(string.Empty, OnContentPropertyChanged));

        public string Content
        {
            get
            {
                return (string)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        static HighTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HighTextBlock), new FrameworkPropertyMetadata(typeof(HighTextBlock)));
        }

        public HighTextBlock()
        {
            this.Loaded += HighTextBlock_Loaded;
        }

        void HighTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            ContentPropertyChanged();
        }

        private void ContentPropertyChanged()
        {
            try
            {

                var txt = this.Content;
                var key = this.Keyword;
                this.Inlines.Clear();
                this.Text = string.Empty;
                if (string.IsNullOrEmpty(txt) || string.IsNullOrEmpty(key))
                {
                    this.Text = txt;
                    return;
                }
                //分割txt，只处理找到的第一个
                var index = txt.IndexOf(key);
                if (index < 0)
                {
                    this.Text = txt;
                    return;
                }
                if (index > 0)
                {
                    this.Inlines.Add(new Run(txt.Substring(0, index)));
                }
                Run rkey = new Run(txt.Substring(index, key.Length));
                rkey.Background = this.HighBackground;
                rkey.Foreground = this.HighForeground;
                this.Inlines.Add(rkey);
                var pos = index + key.Length;
                var len = txt.Length - pos;
                if (len > 0)
                {
                    this.Inlines.Add(new Run(txt.Substring(pos, len)));
                }

            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 绑定图片资源
        /// </summary>
        private static void OnContentPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var tb = sender as HighTextBlock;
            tb.ContentPropertyChanged();
        }
    }
}
