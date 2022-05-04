using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AIStudio.Wpf.Controls
{
    public class ChatBubble : ContentControl
    {
        static ChatBubble()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatBubble), new FrameworkPropertyMetadata(typeof(ChatBubble)));
        }

        public ChatBubble()
        {

        }

        public static readonly DependencyProperty RoleProperty = DependencyProperty.Register(
            nameof(Role), typeof(string), typeof(ChatBubble), new PropertyMetadata(default(string)));

        public string Role
        {
            get => (string)GetValue(RoleProperty);
            set => SetValue(RoleProperty, value);
        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            nameof(Type), typeof(int), typeof(ChatBubble), new PropertyMetadata(1));

        public int Type
        {
            get => (int)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public static readonly DependencyProperty AvatarProperty = DependencyProperty.Register(
           nameof(Avatar), typeof(BitmapImage), typeof(ChatBubble), new PropertyMetadata(null));

        public BitmapImage Avatar
        {
            get => (BitmapImage)GetValue(AvatarProperty);
            set => SetValue(AvatarProperty, value);
        }

        public static readonly DependencyProperty IsReadProperty = DependencyProperty.Register(
            nameof(IsRead), typeof(bool), typeof(ChatBubble), new PropertyMetadata(false));

        public bool IsRead
        {
            get => (bool)GetValue(IsReadProperty);
            set => SetValue(IsReadProperty, value);
        }

        public static readonly DependencyProperty ShowTimeProperty = DependencyProperty.Register(
          nameof(ShowTime), typeof(bool), typeof(ChatBubble), new PropertyMetadata(true));

        public bool ShowTime
        {
            get => (bool)GetValue(ShowTimeProperty);
            set => SetValue(ShowTimeProperty, value);
        }

        public static readonly DependencyProperty CreateTimeProperty = DependencyProperty.Register(
         nameof(CreateTime), typeof(DateTime), typeof(ChatBubble), new PropertyMetadata(DateTime.Now));

        public DateTime CreateTime
        {
            get => (DateTime)GetValue(CreateTimeProperty);
            set => SetValue(CreateTimeProperty, value);
        }

        public static void SetMaxWidth(DependencyObject element, double value)
            => element.SetValue(MaxWidthProperty, value);

        public static double GetMaxWidth(DependencyObject element)
            => (double)element.GetValue(MaxWidthProperty);

        public Action<object> ReadAction
        {
            get; set;
        }


        private bool _isMouseLeftButtonDown;

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            _isMouseLeftButtonDown = false;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _isMouseLeftButtonDown = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (_isMouseLeftButtonDown)
            {
                if (SelfManage)
                {
                    if (!IsSelected)
                    {
                        IsSelected = true;
                        OnSelected(new RoutedEventArgs(SelectedEvent, this));
                    }
                    else if (CanDeselect)
                    {
                        IsSelected = false;
                        OnSelected(new RoutedEventArgs(DeselectedEvent, this));
                    }
                }
                else
                {
                    if (CanDeselect)
                    {
                        OnSelected(IsSelected
                            ? new RoutedEventArgs(DeselectedEvent, this)
                            : new RoutedEventArgs(SelectedEvent, this));
                    }
                    else
                    {
                        OnSelected(new RoutedEventArgs(SelectedEvent, this));
                    }
                }
                _isMouseLeftButtonDown = false;
            }
        }

        protected void OnSelected(RoutedEventArgs e)
        {
            RaiseEvent(e);

            IsRead = true;
            ReadAction?.Invoke(Content);
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected), typeof(bool), typeof(ChatBubble), new PropertyMetadata(false));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty SelfManageProperty = DependencyProperty.Register(
            nameof(SelfManage), typeof(bool), typeof(ChatBubble), new PropertyMetadata(false));

        public bool SelfManage
        {
            get => (bool)GetValue(SelfManageProperty);
            set => SetValue(SelfManageProperty, value);
        }

        public static readonly DependencyProperty CanDeselectProperty = DependencyProperty.Register(
            nameof(CanDeselect), typeof(bool), typeof(ChatBubble), new PropertyMetadata(false));

        public bool CanDeselect
        {
            get => (bool)GetValue(CanDeselectProperty);
            set => SetValue(CanDeselectProperty, value);
        }

        public static readonly RoutedEvent SelectedEvent =
            EventManager.RegisterRoutedEvent(nameof(Selected), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(ChatBubble));

        public event RoutedEventHandler Selected
        {
            add => AddHandler(SelectedEvent, value);
            remove => RemoveHandler(SelectedEvent, value);
        }

        public static readonly RoutedEvent DeselectedEvent =
            EventManager.RegisterRoutedEvent(nameof(Deselected), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(ChatBubble));

        public event RoutedEventHandler Deselected
        {
            add => AddHandler(DeselectedEvent, value);
            remove => RemoveHandler(DeselectedEvent, value);
        }
    }
}
