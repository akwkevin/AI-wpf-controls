using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using AIStudio.Wpf.Controls.Event;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_OkButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_CancelButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]
    public class NoticeCard : Control
    {
        private const string PART_OkButton = "PART_OkButton";
        private const string PART_CancelButton = "PART_CancelButton";
        private const string PART_CloseButton = "PART_CloseButton";

        private Button _okButton;
        private Button _cancelButton;
        private Button _closeButton;

        public bool ShowOK { get; set; } = false;
        public bool ShowCancel { get; set; } = false;
        public bool ShowClose { get; set; } = true;
        public Action<MessageBoxResult> ActionClose
        {
            get; set;
        }

        private MessageBoxResult _messageBoxResult = MessageBoxResult.Cancel;
        public MessageBoxResult MessageBoxResult
        {
            get
            {
                return _messageBoxResult;
            }
        }
        #region private
        private DispatcherTimer _dispatcherTimer;
        #endregion

        static NoticeCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NoticeCard), new FrameworkPropertyMetadata(typeof(NoticeCard)));
        }

        public NoticeCard(double? durationSeconds)
        {
            if (durationSeconds == null)
                return;

            _dispatcherTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds((double)durationSeconds),
            };
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _dispatcherTimer.Start();
        }

        public NoticeCard()
        {
            ShowClose = false;

            this.Timeup += (sender, e) => {
                var parent = this.Parent as Panel;
                if (parent != null)
                {
                    parent.Children.Remove(this);
                }
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_cancelButton != null)
            {
                _cancelButton.Click -= Button_Click;
            }
            if (_okButton != null)
            {
                _okButton.Click -= Button_Click;
            }
            if (_closeButton != null)
            {
                _closeButton.Click -= Button_Click;
            }

            _okButton = GetTemplateChild(PART_OkButton) as Button;
            _cancelButton = GetTemplateChild(PART_CancelButton) as Button;
            _closeButton = GetTemplateChild(PART_CloseButton) as Button;

            if (_okButton != null)
            {
                _okButton.IsEnabled = ShowOK;
                if (ShowOK)
                {
                    _okButton.Focus();
                }
                _okButton.Click += Button_Click;
            }
            if (_cancelButton != null)
            {
                _cancelButton.IsEnabled = ShowCancel;
                _cancelButton.Click += Button_Click;
            }
            if (_closeButton != null)
            {
                _closeButton.IsEnabled = ShowClose;
                _closeButton.Click += Button_Click;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;

            if (button == null)
                return;

            switch (button.Name)
            {
                case PART_CancelButton:
                    _messageBoxResult = MessageBoxResult.Cancel;
                    RaiseTimeup();
                    break;
                case PART_OkButton:
                    _messageBoxResult = MessageBoxResult.OK;
                    RaiseTimeup();
                    break;
                case PART_CloseButton:
                    Close_Click(sender, e);
                    break;
            }

            e.Handled = true;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var storyboard = new Storyboard();

            DoubleAnimation opacityAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(1000)));
            Storyboard.SetTarget(opacityAnimation, this);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));

            DoubleAnimation heightAnimation = new DoubleAnimation(this.ActualHeight, 0, new Duration(TimeSpan.FromMilliseconds(300))) { BeginTime = TimeSpan.FromMilliseconds(1000) };
            Storyboard.SetTarget(heightAnimation, this);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(FrameworkElement.HeightProperty));

            storyboard.Children.Add(opacityAnimation);
            storyboard.Children.Add(heightAnimation);

            storyboard.Completed += (s, ev) => {
                _messageBoxResult = MessageBoxResult.Cancel;
                RaiseTimeup();
            };
            storyboard.Begin();


        }



        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            RaiseTimeup(!ShowOK);

            _dispatcherTimer.Stop();
        }

        #region RoutedEvent
        public static readonly RoutedEvent TimeupEvent = EventManager.RegisterRoutedEvent(nameof(Timeup), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NoticeCard));
        public event RoutedEventHandler Timeup
        {
            add
            {
                AddHandler(TimeupEvent, value);
            }
            remove
            {
                RemoveHandler(TimeupEvent, value);
            }
        }

        internal void RaiseTimeup(bool close = true)
        {
            var arg = new FunctionEventArgs<bool>(TimeupEvent, this)
            {
                Info = close,
            };
            RaiseEvent(arg);
        }
        #endregion

        #region Property
        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(NoticeCard));

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

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(NoticeCard));


        public ControlStatus Status
        {
            get
            {
                return (ControlStatus)GetValue(StatusProperty);
            }
            set
            {
                SetValue(StatusProperty, value);
            }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(nameof(Status), typeof(ControlStatus), typeof(NoticeCard));

        public NoticeCardStyle ControlStyle
        {
            get
            {
                return (NoticeCardStyle)GetValue(ControlStyleProperty);
            }
            set
            {
                SetValue(ControlStyleProperty, value);
            }
        }

        public static readonly DependencyProperty ControlStyleProperty =
            DependencyProperty.Register(nameof(ControlStyle), typeof(NoticeCardStyle), typeof(NoticeCard), new PropertyMetadata(NoticeCardStyle.Standard));



        public ICommand CloseCommand
        {
            get
            {
                return (ICommand)GetValue(CloseCommandProperty);
            }
            set
            {
                SetValue(CloseCommandProperty, value);
            }
        }

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(NoticeCard), new PropertyMetadata(new CloseCommand()));


        public static readonly DependencyProperty IdentifierProperty = DependencyProperty.RegisterAttached(
            "Identifier", typeof(string), typeof(NoticeCard), new PropertyMetadata(default(string), OnIdentifierChanged));

        private static void OnIdentifierChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Panel panel)
            {
                if (e.NewValue == null)
                {
                    Unregister(panel);
                }
                else
                {
                    Register(e.NewValue.ToString(), panel);
                }
            }
        }

        public static void SetIdentifier(DependencyObject element, string value)
            => element.SetValue(IdentifierProperty, value);

        public static string GetIdentifier(DependencyObject element)
            => (string)element.GetValue(IdentifierProperty);

        private static readonly Dictionary<string, Panel> PanelDic = new Dictionary<string, Panel>();

        public static void Register(string token, Panel panel)
        {
            if (string.IsNullOrEmpty(token) || panel == null) return;
            PanelDic[token] = panel;
        }

        public static void Unregister(Panel panel)
        {
            if (panel == null) return;
            var first = PanelDic.FirstOrDefault(item => ReferenceEquals(panel, item.Value));
            if (!string.IsNullOrEmpty(first.Key))
            {
                PanelDic.Remove(first.Key);
            }
        }
        #endregion

        #region Calling Method
        public static void AddNotice(
            string message, 
            string title, 
            double? durationSeconds, 
            ControlStatus messageBoxIcon, 
            NoticeCardStyle noticeCardStyle, 
            double? width,
            double? height,
            HorizontalAlignment? horizontalAlignment = null, 
            VerticalAlignment? verticalAlignment = null, 
            bool? showSure = null, 
            bool showClose = true, 
            Action<MessageBoxResult> actionClose = null, 
            string identifier = null)
        {
            bool show = false;
            if (showSure != null)
            {
                show = showSure.Value;
            }
            else
            {
                if (messageBoxIcon == ControlStatus.Info)
                {
                    show = true;
                }
            }

            var noticeCard = new NoticeCard(durationSeconds)
            {
                Message = message,
                Title = title,
                Status = messageBoxIcon,
                ShowOK = show,
                ShowCancel = show,
                ShowClose = showClose,
                ActionClose = actionClose,   
                ControlStyle = noticeCardStyle
            };

            if (width != null)
            {
                noticeCard.Width = width.Value;
                noticeCard.Height = height.Value;
            }

            noticeCard.Timeup += NoticeCard_Timeup;

            if (horizontalAlignment != null)
            {
                PanelDic[identifier ?? NoticeWindow.Identifier].HorizontalAlignment = horizontalAlignment.Value;
            }
            if (verticalAlignment != null)
            {
                PanelDic[identifier ?? NoticeWindow.Identifier].VerticalAlignment = verticalAlignment.Value;
            }

            PanelDic[identifier??NoticeWindow.Identifier].Children.Insert(0, noticeCard);
        }

        private static void NoticeCard_Timeup(object sender, RoutedEventArgs e)
        {
            var noticeCard = sender as NoticeCard;

            var args = e as FunctionEventArgs<bool>;
            if (args.Info == true)
            {
                if (noticeCard.Parent is Panel panel)
                {
                    panel.Children.Remove(noticeCard);
                    if (noticeCard.ActionClose != null)
                    {
                        noticeCard.ActionClose(noticeCard.MessageBoxResult);
                    }
                }
            }
        }

        public static void ClearNotice(string identifier = null)
        {
            PanelDic[identifier ?? NoticeWindow.Identifier].Children.Clear();
        }
        #endregion
    }

    internal class CloseCommand : ICommand
    {
        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var card = (parameter as NoticeCard);
            card.RaiseTimeup();
        }
    }
}
