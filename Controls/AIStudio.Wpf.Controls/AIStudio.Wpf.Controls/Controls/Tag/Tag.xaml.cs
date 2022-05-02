using System;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Core;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_TxtAdd, Type = typeof(TextBox))]
    public class Tag : ListBoxItem
    {
        private const string PART_CloseButton = "PART_CloseButton";
        private const string PART_TxtAdd = "PART_TxtAdd";

        #region private fields
        private Button _button;
        private TextBox _textBox;
        PropertyInfo _displayMemberPropertyInfo;
        #endregion

        #region Property
        private TagBox ParentItemsControl
        {
            get
            {
                return this.ParentSelector as TagBox;
            }
        }


        internal ItemsControl ParentSelector
        {
            get
            {
                return ItemsControl.ItemsControlFromItemContainer(this) as ItemsControl;
            }
        }
        #endregion

        #region DependencyProperty

        #region CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius"
            , typeof(CornerRadius), typeof(Tag));
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

        #region IsClosable

        public bool IsClosable
        {
            get
            {
                return (bool)GetValue(IsClosableProperty);
            }
            set
            {
                SetValue(IsClosableProperty, value);
            }
        }

        public static readonly DependencyProperty IsClosableProperty =
            DependencyProperty.Register("IsClosable", typeof(bool), typeof(Tag), new PropertyMetadata(true));

        #endregion

        #region IsAdd

        /// <summary>
        /// 是否为可添加模块
        /// </summary>
        public bool IsAdd
        {
            get
            {
                return (bool)GetValue(IsAddProperty);
            }
            set
            {
                SetValue(IsAddProperty, value);
            }
        }

        public static readonly DependencyProperty IsAddProperty =
            DependencyProperty.Register(nameof(IsAdd), typeof(bool), typeof(Tag), new PropertyMetadata(false));

        #endregion

        #endregion

        #region Events

        #region ClosedEvent

        public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent("Closed",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(Tag));

        public event RoutedPropertyChangedEventHandler<object> Closed
        {
            add
            {
                this.AddHandler(ClosedEvent, value);
            }
            remove
            {
                this.RemoveHandler(ClosedEvent, value);
            }
        }

        public virtual void OnClosed(object oldValue, object newValue)
        {
            RoutedPropertyChangedEventArgs<object> arg = new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, ClosedEvent);
            this.RaiseEvent(arg);
        }

        #endregion

        #endregion

        #region Constructors

        static Tag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Tag), new FrameworkPropertyMetadata(typeof(Tag)));
        }

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this._button != null)
            {
                this._button.Click -= PART_CloseButton_Click;
            }
            if (_textBox != null)
                _textBox.PreviewKeyDown -= _textBox_PreviewKeyDown;

            this._button = this.GetTemplateChild(PART_CloseButton) as Button;
            _textBox = GetTemplateChild(PART_TxtAdd) as TextBox;

            if (this._button != null)
            {
                this._button.Click += PART_CloseButton_Click;
            }
            if (_textBox != null)
                _textBox.PreviewKeyDown += _textBox_PreviewKeyDown;

            VisualStateManager.GoToState(this, "Show", true);
        }
        #endregion

        #region private function

        #endregion

        #region Event Implement Function
        private void PART_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Closed", true);
            this.OnClosed(null, null);
            var parent = this.ParentSelector;
            if (parent != null)
            {
                if (parent.ItemsSource != null)
                {
                    (parent.ItemsSource as IList).Remove(this.DataContext);
                    //parent.Items.Refresh();
                }
                else
                {
                    parent.Items.Remove(this);
                }
            }
        }

        private void _textBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var text = sender as TextBox;
            if (e.Key == Key.Return)
            {
                if (!string.IsNullOrEmpty(text.Text))
                {
                    if (ParentItemsControl.ItemsSource != null)
                    {
                        var type = ParentSelector.ItemsSource.GetType();
                        var itemtype = type.GetCollectionElementType();
                        var obj = Activator.CreateInstance(itemtype);

                        if (itemtype.Name == "System.String")
                        {
                            obj = text.Text;
                        }
                        else
                        {
                            if (_displayMemberPropertyInfo == null || _displayMemberPropertyInfo.Name != ParentItemsControl.DisplayPath)
                                _displayMemberPropertyInfo = itemtype.GetProperty(ParentItemsControl.DisplayPath);

                            if (_displayMemberPropertyInfo != null)
                                _displayMemberPropertyInfo.SetValue(obj, text.Text);
                        }

                        (ParentItemsControl.ItemsSource as IList).Insert((ParentItemsControl.ItemsSource as IList).Count - 1, obj);
                    }
                    else
                    {
                        var tag = new Tag() { Content = text.Text };
                        ParentItemsControl.Items.Insert(ParentItemsControl.Items.Count - 1, tag);
                    }
                    text.Text = "";
                }
            }
        }
        #endregion
    }
}
