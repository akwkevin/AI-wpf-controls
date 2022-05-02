using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_Button, Type = typeof(Button))]
    [TemplatePart(Name = PART_ToggleButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    public class DropDown : ContentControl
    {
        private const string PART_Button = "PART_Button";
        private const string PART_ToggleButton = "PART_ToggleButton";
        private const string PART_Popup = "PART_Popup";

        private Button _button;
        private ToggleButton _toggleButton;
        private Popup _popup;

        static DropDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDown), new FrameworkPropertyMetadata(typeof(DropDown)));
        }

        public DropDown()
        {
            AddHandler(NotTopMostPopup.OpeningEvent, new RoutedEventHandler(OnOpening));
            AddHandler(NotTopMostPopup.ClosingEvent, new RoutedEventHandler(OnClosing));

            AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(ItemsOnClick));
        }

        private void ItemsOnClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem)
            {
                SetCurrentValue(IsOpenProperty, false);
            }
        }

        public override void OnApplyTemplate()
        {
            if (_button != null)
            {
                _button.Click -= _button_Click;
            }
            if (_toggleButton != null)
            {
                _toggleButton.MouseEnter -= _toggleButton_MouseEnter;
            }

            base.OnApplyTemplate();

            _button = GetTemplateChild(PART_Button) as Button;
            _toggleButton = GetTemplateChild(PART_ToggleButton) as ToggleButton;
            _popup = GetTemplateChild(PART_Popup) as Popup;
            if (_button != null)
            {
                _button.Click += _button_Click;
            }
            if (_toggleButton != null)
            {
                _toggleButton.MouseEnter += _toggleButton_MouseEnter;
            }
        }

        private void _button_Click(object sender, RoutedEventArgs e)
        {
            switch (Command)
            {
                case null:
                    return;
                case RoutedCommand command:
                    command.Execute(CommandParameter, CommandTarget);
                    break;
                default:
                    Command.Execute(CommandParameter);
                    break;
            }
        }

        private void _toggleButton_MouseEnter(object sender, RoutedEventArgs e)
        {
            if (HoverMode)
            {
                SetCurrentValue(IsOpenProperty, true);
            }
        }

        #region Event
        public event EventHandler Opened;

        public event EventHandler Closed;
        #endregion

        #region RoutedEvent 
        private void OnOpening(object sender, RoutedEventArgs e)
        {
            Opened?.Invoke(this, null);
        }

        private void OnClosing(object sender, RoutedEventArgs e)
        {
            Closed?.Invoke(this, null);
        }

        #endregion

        #region Property
        /// <summary>
        /// Gets or sets shadow color.
        /// </summary>
        public Color ShadowColor
        {
            get
            {
                return (Color)GetValue(ShadowColorProperty);
            }
            set
            {
                SetValue(ShadowColorProperty, value);
            }
        }

        public static readonly DependencyProperty ShadowColorProperty =
            DependencyProperty.Register("ShadowColor", typeof(Color), typeof(DropDown));

        /// <summary>
        /// Gets or sets child
        /// </summary>
        public UIElement Child
        {
            get
            {
                return (UIElement)GetValue(ChildProperty);
            }
            set
            {
                SetValue(ChildProperty, value);
            }
        }

        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(UIElement), typeof(DropDown));


        /// <summary>
        /// Gets or sets drop down placement.
        /// </summary>
        public PlacementMode Placement
        {
            get
            {
                return (PlacementMode)GetValue(PlacementProperty);
            }
            set
            {
                SetValue(PlacementProperty, value);
            }
        }

        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(DropDown), new PropertyMetadata(PlacementMode.Bottom));

        ///// <summary>
        ///// Gets or sets corner radius.
        ///// </summary>
        //public double CornerRadius
        //{
        //    get { return (double)GetValue(CornerRadiusProperty); }
        //    set { SetValue(CornerRadiusProperty, value); }
        //}

        //public static readonly DependencyProperty CornerRadiusProperty =
        //    DependencyProperty.Register("CornerRadius", typeof(double), typeof(DropDown));

        /// <summary>
        /// Gets or sets is open.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return (bool)GetValue(IsOpenProperty);
            }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DropDown));

        /// <summary>
        /// Gets or sets is angle visible.
        /// </summary>
        public bool IsAngleVisible
        {
            get
            {
                return (bool)GetValue(IsAngleVisibleProperty);
            }
            set
            {
                SetValue(IsAngleVisibleProperty, value);
            }
        }

        public static readonly DependencyProperty IsAngleVisibleProperty =
            DependencyProperty.Register("IsAngleVisible", typeof(bool), typeof(DropDown));

        /// <summary>
        /// Gets or sets is angle visible.
        /// </summary>
        public bool IsDropVisible
        {
            get
            {
                return (bool)GetValue(IsDropVisibleProperty);
            }
            set
            {
                SetValue(IsDropVisibleProperty, value);
            }
        }

        public static readonly DependencyProperty IsDropVisibleProperty =
            DependencyProperty.Register("IsDropVisible", typeof(bool), typeof(DropDown), new PropertyMetadata(true));
        #endregion

        /// <summary>
        /// Pagination style.
        /// </summary>
        public DropDownControlStyle ControlStyle
        {
            get
            {
                return (DropDownControlStyle)GetValue(ControlStyleProperty);
            }
            set
            {
                SetValue(ControlStyleProperty, value);
            }
        }

        public static readonly DependencyProperty ControlStyleProperty =
            DependencyProperty.Register(nameof(ControlStyle), typeof(DropDownControlStyle), typeof(DropDown), new PropertyMetadata(DropDownControlStyle.Outlined));

        public static readonly DependencyProperty HoverModeProperty = DependencyProperty.Register(
           nameof(HoverMode), typeof(bool), typeof(DropDown), new PropertyMetadata(default(bool)));

        public bool HoverMode
        {
            get => (bool)GetValue(HoverModeProperty);
            set => SetValue(HoverModeProperty, value);
        }


        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
          nameof(Command), typeof(ICommand), typeof(DropDown), new PropertyMetadata(default(ICommand), OnCommandChanged));

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (DropDown)d;
            if (e.OldValue is ICommand oldCommand)
            {
                oldCommand.CanExecuteChanged -= ctl.CanExecuteChanged;
            }
            if (e.NewValue is ICommand newCommand)
            {
                newCommand.CanExecuteChanged += ctl.CanExecuteChanged;
            }
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            nameof(CommandParameter), typeof(object), typeof(DropDown), new PropertyMetadata(default(object)));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(
            nameof(CommandTarget), typeof(IInputElement), typeof(DropDown), new PropertyMetadata(default(IInputElement)));

        public IInputElement CommandTarget
        {
            get => (IInputElement)GetValue(CommandTargetProperty);
            set => SetValue(CommandTargetProperty, value);
        }

        private void CanExecuteChanged(object sender, EventArgs e)
        {
            if (Command == null) return;

            IsEnabled = Command is RoutedCommand command
                ? command.CanExecute(CommandParameter, CommandTarget)
                : Command.CanExecute(CommandParameter);
        }
    }

    public enum HoverMode
    {
        Click,
        Hover,
        HoverAny,
        Focus,
        None
    }
}
