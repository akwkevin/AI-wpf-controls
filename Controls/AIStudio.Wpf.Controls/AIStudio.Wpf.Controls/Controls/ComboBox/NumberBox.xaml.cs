using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Event;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// 用于日历显示时分秒的控件
    /// </summary>
    public class NumberBox : ComboBox
    {
        static NumberBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox), new FrameworkPropertyMetadata(typeof(NumberBox)));
        }

        #region 依赖属性
        public static readonly DependencyProperty StartNumberProperty = DependencyProperty.Register("StartNumber"
            , typeof(int), typeof(NumberBox));

        /// <summary>
        /// 起始数字
        /// </summary>
        public int StartNumber
        {
            get
            {
                return (int)GetValue(StartNumberProperty);
            }
            set
            {
                SetValue(StartNumberProperty, value);
            }
        }

        public static readonly DependencyProperty EndNumberProperty = DependencyProperty.Register("EndNumber"
            , typeof(int), typeof(NumberBox));

        /// <summary>
        /// 结束数字
        /// </summary>
        public int EndNumber
        {
            get
            {
                return (int)GetValue(EndNumberProperty);
            }
            set
            {
                SetValue(EndNumberProperty, value);
            }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title"
            , typeof(string), typeof(NumberBox));

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly DependencyProperty MaxDropDownWidthProperty = DependencyProperty.Register("MaxDropDownWidth"
            , typeof(double), typeof(NumberBox));

        /// <summary>
        /// 弹出框的最大宽度
        /// </summary>
        public double MaxDropDownWidth
        {
            get
            {
                return (double)GetValue(MaxDropDownWidthProperty);
            }
            set
            {
                SetValue(MaxDropDownWidthProperty, value);
            }
        }

        public static readonly DependencyProperty DigitProperty = DependencyProperty.Register("Digit"
            , typeof(int), typeof(NumberBox), new PropertyMetadata(2));

        /// <summary>
        /// 是否显示阴影
        /// </summary>
        public int Digit
        {
            get
            {
                return (int)GetValue(DigitProperty);
            }
            set
            {
                SetValue(DigitProperty, value);
            }
        }
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            List<int> list = new List<int>();
            for (int i = StartNumber; i <= EndNumber; i++)
            {
                list.Add(i);
            }
            this.ItemsSource = list;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = new NumberBoxItem();
            item.OnItemSingleClickHandler += Item_OnClickHandler;
            return item;
        }

        private void Item_OnClickHandler(object sender, FunctionEventArgs<object> e)
        {
            NumberBoxItem item = sender as NumberBoxItem;
            this.SelectedItem = item.Content;
        }
    }

    /// <summary>
    /// 重写ListViewItem，定义行单击、双击事件
    /// </summary>
    public class NumberBoxItem : System.Windows.Controls.ComboBoxItem
    {
        #region 事件
        /// <summary>
        /// Item单击事件
        /// </summary>
        public event EventHandler<FunctionEventArgs<object>> OnItemSingleClickHandler;
        #endregion

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var selectedItem = ((System.Windows.FrameworkElement)e.OriginalSource).DataContext;
            this.OnItemSingleClickHandler(this, new FunctionEventArgs<object>(selectedItem));
            base.OnMouseLeftButtonDown(e);
        }
    }
}
