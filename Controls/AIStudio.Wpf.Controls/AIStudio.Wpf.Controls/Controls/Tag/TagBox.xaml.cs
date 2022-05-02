using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Core;

namespace AIStudio.Wpf.Controls
{
    public class TagBox : ListBox
    {
        #region private fields
        #endregion

        #region DependencyProperty

        #region IsLineFeed

        /// <summary>
        /// 获取或者设置子项是否换行显示
        /// </summary>
        public bool IsLineFeed
        {
            get
            {
                return (bool)GetValue(IsLineFeedProperty);
            }
            set
            {
                SetValue(IsLineFeedProperty, value);
            }
        }

        public static readonly DependencyProperty IsLineFeedProperty =
            DependencyProperty.Register(nameof(IsLineFeed), typeof(bool), typeof(TagBox), new PropertyMetadata(true));

        #endregion

        #region CanAdd

        /// <summary>
        /// 获取或者设置子项是否换行显示
        /// </summary>
        public bool CanAdd
        {
            get
            {
                return (bool)GetValue(CanAddProperty);
            }
            set
            {
                SetValue(CanAddProperty, value);
            }
        }

        public static readonly DependencyProperty CanAddProperty =
            DependencyProperty.Register(nameof(CanAdd), typeof(bool), typeof(TagBox), new PropertyMetadata(false, OnCanAddChanged));

        private static void OnCanAddChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uploadFile = (TagBox)d;
            if (uploadFile != null)
            {
                //uploadFile.InitAddTag((bool)e.NewValue);
            }
        }

        #endregion

        public string DisplayPath
        {
            get
            {
                return (string)GetValue(DisplayPathProperty);
            }
            set
            {
                SetValue(DisplayPathProperty, value);
            }
        }

        public static readonly DependencyProperty DisplayPathProperty =
            DependencyProperty.Register(nameof(DisplayPath), typeof(string), typeof(TagBox));

        #region CornerRadius

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

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(TagBox));

        #endregion

        #endregion

        #region Constructors

        static TagBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TagBox), new FrameworkPropertyMetadata(typeof(TagBox)));
        }

        #endregion

        #region Override

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new Tag();
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                InitAddTag(CanAdd);
            }

        }
        #endregion

        #region private function
        private void InitAddTag(bool isAdd)
        {
            if (isAdd)
            {
                if (ItemsSource != null)
                {
                    var type = ItemsSource.GetType();
                    var itemtype = type.GetCollectionElementType();
                    var obj = Activator.CreateInstance(itemtype);

                    var _displayMemberPropertyInfo = itemtype.GetProperty("IsAdd");

                    if (_displayMemberPropertyInfo != null)
                        _displayMemberPropertyInfo.SetValue(obj, true);

                    (ItemsSource as IList).Add(obj);
                }
                else
                {
                    var addtag = new Tag() { IsAdd = true };
                    this.AddChild(addtag);
                }
            }
        }
        #endregion

        #region Event Implement Function

        #endregion
    }
}
