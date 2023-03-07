using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// 公共附加属性
    /// </summary>
    public static partial class ControlAttach
    {
        /************************************ Attach Property **************************************/
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached(
           "Background", typeof(Brush), typeof(ControlAttach), new FrameworkPropertyMetadata(default(Brush)));

        public static void SetBackground(DependencyObject element, Brush value)
            => element.SetValue(BackgroundProperty, value);

        public static Brush GetBackground(DependencyObject element)
            => (Brush)element.GetValue(BackgroundProperty);

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.RegisterAttached(
            "Foreground", typeof(Brush), typeof(ControlAttach), new FrameworkPropertyMetadata(default(Brush)));

        public static void SetForeground(DependencyObject element, Brush value)
            => element.SetValue(ForegroundProperty, value);

        public static Brush GetForeground(DependencyObject element)
            => (Brush)element.GetValue(ForegroundProperty);

        public static readonly DependencyProperty StatusProperty = DependencyProperty.RegisterAttached(
            "Status", typeof(ControlStatus), typeof(ControlAttach), new FrameworkPropertyMetadata(ControlStatus.Default));

        public static void SetStatus(DependencyObject element, ControlStatus value)
            => element.SetValue(StatusProperty, value);

        public static ControlStatus GetStatus(DependencyObject element)
            => (ControlStatus)element.GetValue(StatusProperty);

        public static readonly DependencyProperty SizeProperty = DependencyProperty.RegisterAttached(
            "Size", typeof(ControlSize), typeof(ControlAttach), new FrameworkPropertyMetadata(ControlSize.Default));

        public static void SetSize(DependencyObject element, ControlSize value)
            => element.SetValue(SizeProperty, value);

        public static ControlSize GetSize(DependencyObject element)
            => (ControlSize)element.GetValue(SizeProperty);


        #region BorderBrush 边框色，输入控件

        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.RegisterAttached(
            "BorderBrush", typeof(Brush), typeof(ControlAttach), new FrameworkPropertyMetadata(null));
        public static void SetBorderBrush(DependencyObject element, Brush value)
        {
            element.SetValue(BorderBrushProperty, value);
        }
        public static Brush GetBorderBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(BorderBrushProperty);
        }

        #endregion

        #region CornerRadiusProperty Border圆角
        /// <summary>
        /// Border圆角
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
            "CornerRadius", typeof(CornerRadius), typeof(ControlAttach), new FrameworkPropertyMetadata(null));

        public static CornerRadius GetCornerRadius(DependencyObject d)
        {
            return (CornerRadius)d.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }
        #endregion

        public static readonly DependencyProperty UniformCornerRadiusProperty = DependencyProperty.RegisterAttached(
            "UniformCornerRadius", typeof(double), typeof(ControlAttach), new PropertyMetadata(2.0, OnUniformCornerRadius));

        private static void OnUniformCornerRadius(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ControlAttach.SetCornerRadius(d, new CornerRadius((double)e.NewValue));

        public static void SetUniformCornerRadius(DependencyObject element, double value)
            => element.SetValue(UniformCornerRadiusProperty, value);

        public static double GetUniformCornerRadius(DependencyObject element)
            => (double)element.GetValue(UniformCornerRadiusProperty);

        #region BorderThickness 边框色，输入控件

        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.RegisterAttached(
            "BorderThickness", typeof(Thickness), typeof(ControlAttach), new FrameworkPropertyMetadata(default(Thickness)));
        public static void SetBorderThickness(DependencyObject element, Thickness value)
        {
            element.SetValue(BorderThicknessProperty, value);
        }
        public static Thickness GetBorderThickness(DependencyObject element)
        {
            return (Thickness)element.GetValue(BorderThicknessProperty);
        }

        #endregion

        #region Margin 边框色，输入控件

        public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached(
            "Margin", typeof(Thickness), typeof(ControlAttach), new FrameworkPropertyMetadata(default(Thickness)));
        public static void SetMargin(DependencyObject element, Thickness value)
        {
            element.SetValue(MarginProperty, value);
        }
        public static Thickness GetMargin(DependencyObject element)
        {
            return (Thickness)element.GetValue(MarginProperty);
        }

        #endregion     

        #region Padding 边框色，输入控件

        public static readonly DependencyProperty PaddingProperty = DependencyProperty.RegisterAttached(
            "Padding", typeof(Thickness), typeof(ControlAttach), new FrameworkPropertyMetadata(default(Thickness)));
        public static void SetPadding(DependencyObject element, Thickness value)
        {
            element.SetValue(PaddingProperty, value);
        }
        public static Thickness GetPadding(DependencyObject element)
        {
            return (Thickness)element.GetValue(PaddingProperty);
        }

        #endregion     

        #region FocusBackground 获得焦点背景色，

        public static readonly DependencyProperty FocusBackgroundProperty = DependencyProperty.RegisterAttached(
            "FocusBackground", typeof(Brush), typeof(ControlAttach), new FrameworkPropertyMetadata(null));

        public static void SetFocusBackground(DependencyObject element, Brush value)
        {
            element.SetValue(FocusBackgroundProperty, value);
        }

        public static Brush GetFocusBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(FocusBackgroundProperty);
        }

        #endregion

        #region FocusForeground 获得焦点前景色，

        public static readonly DependencyProperty FocusForegroundProperty = DependencyProperty.RegisterAttached(
            "FocusForeground", typeof(Brush), typeof(ControlAttach), new FrameworkPropertyMetadata(null));

        public static void SetFocusForeground(DependencyObject element, Brush value)
        {
            element.SetValue(FocusForegroundProperty, value);
        }

        public static Brush GetFocusForeground(DependencyObject element)
        {
            return (Brush)element.GetValue(FocusForegroundProperty);
        }

        #endregion

        #region FocusBorderBrush 焦点边框色，输入控件

        public static readonly DependencyProperty FocusBorderBrushProperty = DependencyProperty.RegisterAttached(
            "FocusBorderBrush", typeof(Brush), typeof(ControlAttach), new FrameworkPropertyMetadata(null));
        public static void SetFocusBorderBrush(DependencyObject element, Brush value)
        {
            element.SetValue(FocusBorderBrushProperty, value);
        }
        public static Brush GetFocusBorderBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(FocusBorderBrushProperty);
        }

        #endregion     

        #region PressedBackground 按下背景色，

        public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.RegisterAttached(
            "PressedBackground", typeof(Brush), typeof(ControlAttach), new FrameworkPropertyMetadata(null));

        public static void SetPressedBackground(DependencyObject element, Brush value)
        {
            element.SetValue(PressedBackgroundProperty, value);
        }

        public static Brush GetPressedBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(PressedBackgroundProperty);
        }

        #endregion

        #region PressedForeground 按下前景色，

        public static readonly DependencyProperty PressedForegroundProperty = DependencyProperty.RegisterAttached(
            "PressedForeground", typeof(Brush), typeof(ControlAttach), new FrameworkPropertyMetadata(null));

        public static void SetPressedForeground(DependencyObject element, Brush value)
        {
            element.SetValue(PressedForegroundProperty, value);
        }

        public static Brush GetPressedForeground(DependencyObject element)
        {
            return (Brush)element.GetValue(PressedForegroundProperty);
        }

        #endregion

        #region PressedBorderBrush 按下边框色，

        public static readonly DependencyProperty PressedBorderBrushProperty = DependencyProperty.RegisterAttached(
            "PressedBorderBrush", typeof(Brush), typeof(ControlAttach), new FrameworkPropertyMetadata(null));

        public static void SetPressedBorderBrush(DependencyObject element, Brush value)
        {
            element.SetValue(PressedBorderBrushProperty, value);
        }

        public static Brush GetPressedBorderBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(PressedBorderBrushProperty);
        }

        #endregion

        #region MouseOverBackgroundProperty 鼠标悬浮背景色

        public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.RegisterAttached(
            "MouseOverBackground", typeof(Brush), typeof(ControlAttach), new FrameworkPropertyMetadata(null));

        public static void SetMouseOverBackground(DependencyObject element, Brush value)
        {
            element.SetValue(MouseOverBackgroundProperty, value);
        }

        public static Brush GetMouseOverBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(MouseOverBackgroundProperty);
        }

        #endregion

        #region MouseOverForegroundProperty 鼠标悬浮前景色

        public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.RegisterAttached(
            "MouseOverForeground", typeof(Brush), typeof(ControlAttach), new FrameworkPropertyMetadata(null));

        public static void SetMouseOverForeground(DependencyObject element, Brush value)
        {
            element.SetValue(MouseOverForegroundProperty, value);
        }

        public static Brush GetMouseOverForeground(DependencyObject element)
        {
            return (Brush)element.GetValue(MouseOverForegroundProperty);
        }

        #endregion

        #region MouseOverBorderBrush 鼠标进入边框色，输入控件

        public static readonly DependencyProperty MouseOverBorderBrushProperty =
            DependencyProperty.RegisterAttached("MouseOverBorderBrush", typeof(Brush), typeof(ControlAttach),
                new FrameworkPropertyMetadata(Brushes.Transparent,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Sets the brush used to draw the mouse over brush.
        /// </summary>
        public static void SetMouseOverBorderBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(MouseOverBorderBrushProperty, value);
        }

        /// <summary>
        /// Gets the brush used to draw the mouse over brush.
        /// </summary>
        public static Brush GetMouseOverBorderBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MouseOverBorderBrushProperty);
        }

        #endregion

        #region ContentForeground 内容前景色，

        public static readonly DependencyProperty ContentForegroundProperty = DependencyProperty.RegisterAttached(
            "ContentForeground", typeof(Brush), typeof(ControlAttach), new FrameworkPropertyMetadata(null));

        public static void SetContentForeground(DependencyObject element, Brush value)
        {
            element.SetValue(ContentForegroundProperty, value);
        }

        public static Brush GetContentForeground(DependencyObject element)
        {
            return (Brush)element.GetValue(ContentForegroundProperty);
        }

        #endregion

        #region IconContentProperty 附加组件模板
        /// <summary>
        /// 附加组件模板
        /// </summary>
        public static readonly DependencyProperty IconContentProperty = DependencyProperty.RegisterAttached(
            "IconContent", typeof(ControlTemplate), typeof(ControlAttach), new FrameworkPropertyMetadata(null));

        public static ControlTemplate GetIconContent(DependencyObject d)
        {
            return (ControlTemplate)d.GetValue(IconContentProperty);
        }

        public static void SetIconContent(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(IconContentProperty, value);
        }
        #endregion

        #region AttachContentProperty 附加组件模板
        /// <summary>
        /// 附加组件模板
        /// </summary>
        public static readonly DependencyProperty AttachContentProperty = DependencyProperty.RegisterAttached(
            "AttachContent", typeof(ControlTemplate), typeof(ControlAttach), new FrameworkPropertyMetadata(null));

        public static ControlTemplate GetAttachContent(DependencyObject d)
        {
            return (ControlTemplate)d.GetValue(AttachContentProperty);
        }

        public static void SetAttachContent(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(AttachContentProperty, value);
        }
        #endregion

        #region WatermarkProperty 水印
        /// <summary>
        /// 水印
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
            "Watermark", typeof(string), typeof(ControlAttach), new FrameworkPropertyMetadata(""));

        public static string GetWatermark(DependencyObject d)
        {
            return (string)d.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }
        #endregion

        #region AllowsAnimationProperty 启用旋转动画
        /// <summary>
        /// 启用旋转动画
        /// </summary>
        public static readonly DependencyProperty AllowsAnimationProperty = DependencyProperty.RegisterAttached("AllowsAnimation"
            , typeof(bool), typeof(ControlAttach), new FrameworkPropertyMetadata(false, AllowsAnimationChanged));

        public static bool GetAllowsAnimation(DependencyObject d)
        {
            return (bool)d.GetValue(AllowsAnimationProperty);
        }

        public static void SetAllowsAnimation(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowsAnimationProperty, value);
        }

        /// <summary>
        /// 旋转动画刻度
        /// </summary>
        private static DoubleAnimation RotateAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(200)));

        /// <summary>
        /// 绑定动画事件
        /// </summary>
        private static void AllowsAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = d as FrameworkElement;
            if (uc == null) return;
            if (uc.RenderTransformOrigin == new Point(0, 0))
            {
                uc.RenderTransformOrigin = new Point(0.5, 0.5);
                RotateTransform trans = new RotateTransform(0);
                uc.RenderTransform = trans;
            }
            var value = (bool)e.NewValue;
            if (value)
            {
                RotateAnimation.To = 180;
                uc.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, RotateAnimation);
            }
            else
            {
                RotateAnimation.To = 0;
                uc.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, RotateAnimation);
            }
        }
        #endregion 

        #region LabelProperty TextBox的头部Label
        /// <summary>
        /// TextBox的头部Label
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.RegisterAttached(
            "Label", typeof(string), typeof(ControlAttach), new FrameworkPropertyMetadata(null));

        [AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.TextBox))]
        public static string GetLabel(DependencyObject d)
        {
            return (string)d.GetValue(LabelProperty);
        }

        public static void SetLabel(DependencyObject obj, string value)
        {
            obj.SetValue(LabelProperty, value);
        }
        #endregion

        #region LabelTemplateProperty TextBox的头部Label模板
        /// <summary>
        /// TextBox的头部Label模板
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty = DependencyProperty.RegisterAttached(
            "LabelTemplate", typeof(ControlTemplate), typeof(ControlAttach), new FrameworkPropertyMetadata(null));

        [AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.TextBox))]
        public static ControlTemplate GetLabelTemplate(DependencyObject d)
        {
            return (ControlTemplate)d.GetValue(LabelTemplateProperty);
        }

        public static void SetLabelTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(LabelTemplateProperty, value);
        }
        #endregion

        #region PopupBackground 获得焦点背景色，

        public static readonly DependencyProperty PopupBackgroundProperty = DependencyProperty.RegisterAttached(
            "PopupBackground", typeof(Brush), typeof(ControlAttach), new FrameworkPropertyMetadata(null));

        public static void SetPopupBackground(DependencyObject element, Brush value)
        {
            element.SetValue(PopupBackgroundProperty, value);
        }

        public static Brush GetPopupBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(PopupBackgroundProperty);
        }

        #endregion

        #region ClearTextButtonProperty 清除输入框Text值按钮行为开关（设为ture时才会绑定事件）
        /// <summary>
        /// 清除输入框Text值按钮行为开关
        /// </summary>
        public static readonly DependencyProperty ClearTextButtonProperty = DependencyProperty.RegisterAttached("ClearTextButton"
            , typeof(bool), typeof(ControlAttach), new FrameworkPropertyMetadata(false, OnClearTextButtonChanged));


        public static bool GetClearTextButton(DependencyObject d)
        {
            return (bool)d.GetValue(ClearTextButtonProperty);
        }

        public static void SetClearTextButton(DependencyObject obj, bool value)
        {
            obj.SetValue(ClearTextButtonProperty, value);
        }

        /// <summary>
        /// 绑定清除Text操作的按钮事件
        /// </summary>
        private static void OnClearTextButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as System.Windows.Controls.Button;
            if (button != null)
            {
                if ((bool)e.NewValue == true)
                {
                    button.CommandBindings.Add(ClearTextCommandBinding);
                }
            }
        }

        //public static readonly DependencyProperty AutoHideClearTextButtonProperty = DependencyProperty.RegisterAttached("AutoHideClearTextButton"
        //   , typeof(bool), typeof(ControlAttach), new FrameworkPropertyMetadata(false));

        //public static bool GetAutoHideClearTextButton(DependencyObject d)
        //{
        //    return (bool)d.GetValue(AutoHideClearTextButtonProperty);
        //}

        //public static void SetAutoHideClearTextButton(DependencyObject obj, bool value)
        //{
        //    obj.SetValue(AutoHideClearTextButtonProperty, value);
        //}

        #endregion

        #region ClearTextCommand 清除输入框Text事件命令

        /// <summary>
        /// 清除输入框Text事件命令，需要使用ClearTextButtonChanged绑定命令
        /// </summary>
        public static RoutedUICommand ClearTextCommand
        {
            get; private set;
        }

        /// <summary>
        /// ClearTextCommand绑定事件
        /// </summary>
        private static readonly CommandBinding ClearTextCommandBinding;

        /// <summary>
        /// 清除输入框文本值
        /// </summary>
        private static void ClearButtonClick(object sender, ExecutedRoutedEventArgs e)
        {
            var tbox = e.Parameter as FrameworkElement;
            if (tbox == null) return;
            if (tbox is System.Windows.Controls.TextBox)
            {
                ((System.Windows.Controls.TextBox)tbox).Clear();
            }
            else if (tbox is RichTextBox)
            {
                ((RichTextBox)tbox).Clear();
            }
            else if (tbox is System.Windows.Controls.RichTextBox)
            {
                ((System.Windows.Controls.RichTextBox)tbox).Document.Blocks.Clear();
            }
            else if (tbox is System.Windows.Controls.PasswordBox)
            {
                ((System.Windows.Controls.PasswordBox)tbox).Clear();
            }       
            else if(tbox is TreeSelect)
            {
                var cb = tbox as TreeSelect;
                cb.SelectedItem = null;
                if (cb.SelectedItems != null)
                {
                    cb.SelectedItems.Clear();
                }
            }
            else if (tbox is TreeComboBox)
            {
                var cb = tbox as TreeComboBox;
                cb.SelectedItem = null;
                cb.Text = string.Empty;
            }
            else if (tbox is System.Windows.Controls.ComboBox)
            {
                var cb = tbox as System.Windows.Controls.ComboBox;
                cb.SelectedItem = null;
                cb.Text = string.Empty;
            }      
            else if(tbox is MultiComboBox)
            {
                var cb = tbox as MultiComboBox;
                cb.SelectedItem = null;
                cb.UnselectAll();
                cb.Text = string.Empty;
            }
            else if(tbox is System.Windows.Controls.DatePicker)
            {
                var dp = tbox as System.Windows.Controls.DatePicker;
                dp.SelectedDate = null;
                dp.Text = string.Empty;
            }
            else if(tbox is ListBoxItem)
            {
                var listboxitem = tbox as ListBoxItem;
                var listbox = listboxitem.TryFindParent<System.Windows.Controls.ListBox>();
                (listbox.ItemsSource as IList).Remove(listboxitem.DataContext);
                listbox.Items.Refresh();
            }
            tbox.Focus();
        }

        #endregion

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static ControlAttach()
        {
            //ClearTextCommand
            ClearTextCommand = new RoutedUICommand();
            ClearTextCommandBinding = new CommandBinding(ClearTextCommand);
            ClearTextCommandBinding.Executed += ClearButtonClick;
        }
    }


}
