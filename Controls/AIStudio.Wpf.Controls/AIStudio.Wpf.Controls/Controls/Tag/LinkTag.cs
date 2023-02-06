using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// 标签
    /// </summary>
    public class LinkTag : ContentControl
    {
        static LinkTag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LinkTag), new FrameworkPropertyMetadata(typeof(LinkTag)));
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.MouseLeftButtonDown += Tag_MouseLeftButtonDown;
        }

        #region 属性

        /// <summary>
        /// 标题
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(LinkTag), new PropertyMetadata(default(object)));

        /// <summary>
        /// 标题
        /// </summary>
        public object Header
        {
            get
            {
                return (object)GetValue(HeaderProperty);
            }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// 标题背景色
        /// </summary>
        public static readonly DependencyProperty HeadBackgroundProperty =
            DependencyProperty.Register(nameof(HeadBackground), typeof(Brush), typeof(LinkTag), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// 标题背景色
        /// </summary>
        public Brush HeadBackground
        {
            get
            {
                return (Brush)GetValue(HeadBackgroundProperty);
            }
            set
            {
                SetValue(HeadBackgroundProperty, value);
            }
        }

        /// <summary>
        /// 标题前景色
        /// </summary>
        public static readonly DependencyProperty HeadForegroundProperty =
            DependencyProperty.Register(nameof(HeadForeground), typeof(Brush), typeof(LinkTag), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// 标题前景色
        /// </summary>
        public Brush HeadForeground
        {
            get
            {
                return (Brush)GetValue(HeadForegroundProperty);
            }
            set
            {
                SetValue(HeadForegroundProperty, value);
            }
        }

        /// <summary>
        /// 链接
        /// </summary>
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register(nameof(Url), typeof(string), typeof(LinkTag), new PropertyMetadata(default(string)));

        /// <summary>
        /// 链接
        /// </summary>
        public string Url
        {
            get
            {
                return (string)GetValue(UrlProperty);
            }
            set
            {
                SetValue(UrlProperty, value);
            }
        }

        #region CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty = 
            DependencyProperty.Register(nameof(CornerRadius) , typeof(CornerRadius), typeof(LinkTag));
        /// <summary>
        /// 按钮四周圆角
        /// </summary>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        #endregion

        #endregion 属性

        #region 方法

        private void Tag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(Url))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Url) { UseShellExecute = true });
            }
        }

        #endregion 方法
    }
}
