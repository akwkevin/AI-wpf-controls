using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// FloatExpander.xaml 的交互逻辑
    /// </summary>
    [TemplatePart(Name = PART_Thumb, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_Pin, Type = typeof(ToggleButton))]
    [TemplatePart(Name = PART_Esc, Type = typeof(Button))]
    [TemplatePart(Name = PART_ToggleSite, Type = typeof(ToggleButton))]
    [TemplatePart(Name = PART_Min, Type = typeof(Button))]
    public partial class FloatExpander : Expander
    {
        private const string PART_Thumb = "PART_Thumb";
        private const string PART_Pin = "PART_Pin";
        private const string PART_Esc = "PART_Esc";
        private const string PART_ToggleSite = "PART_ToggleSite";
        private const string PART_Min = "PART_Min";

        private Thumb _thumb;
        private ToggleButton _pin;
        private Button _esc;
        private ToggleButton _toggle;
        private Button _min;
        private FrameworkElement _parent;
        private System.Windows.Threading.DispatcherTimer timer;

        public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register(
           "IsPinned", typeof(bool), typeof(FloatExpander), new PropertyMetadata(false));

        public bool IsPinned
        {
            get => (bool)GetValue(IsPinnedProperty);
            set => SetValue(IsPinnedProperty, value);
        }

        public static readonly DependencyProperty ShowMinboxProperty = DependencyProperty.Register(
         "ShowMinbox", typeof(bool), typeof(FloatExpander), new PropertyMetadata(false));

        public bool ShowMinbox
        {
            get => (bool)GetValue(ShowMinboxProperty);
            set => SetValue(ShowMinboxProperty, value);
        }

        public static readonly DependencyProperty EscCommandProperty = DependencyProperty.Register(
           "EscCommand", typeof(ICommand), typeof(FloatExpander), new PropertyMetadata(default(ICommand), OnCommandChanged));

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (FloatExpander)d;
            if (e.OldValue is ICommand oldCommand)
            {
                oldCommand.CanExecuteChanged -= ctl.CanExecuteChanged;
            }
            if (e.NewValue is ICommand newCommand)
            {
                newCommand.CanExecuteChanged += ctl.CanExecuteChanged;
            }
        }

        public ICommand EscCommand
        {
            get => (ICommand)GetValue(EscCommandProperty);
            set => SetValue(EscCommandProperty, value);
        }

        public static readonly DependencyProperty EscCommandParameterProperty = DependencyProperty.Register(
          "EscCommandParameter", typeof(object), typeof(FloatExpander), new PropertyMetadata(default(object)));

        public object EscCommandParameter
        {
            get => GetValue(EscCommandParameterProperty);
            set => SetValue(EscCommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(
           "CommandTarget", typeof(IInputElement), typeof(FloatExpander), new PropertyMetadata(default(IInputElement)));

        public IInputElement CommandTarget
        {
            get => (IInputElement)GetValue(CommandTargetProperty);
            set => SetValue(CommandTargetProperty, value);
        }

        private void CanExecuteChanged(object sender, EventArgs e)
        {
            if (EscCommand == null) return;

            IsEnabled = EscCommand is RoutedCommand command
                ? command.CanExecute(EscCommandParameter, CommandTarget)
                : EscCommand.CanExecute(EscCommandParameter);
        }

        public FloatExpander()
        {
            this.Loaded += FloatExpander_Loaded;
        }

        private void FloatExpander_Loaded(object sender, RoutedEventArgs e)
        {
            _parent = this.Parent as FrameworkElement;
            _parent.SizeChanged += Parent_SizeChanged;

            if (ExpandDirection == ExpandDirection.Down)
            {
                this.HorizontalAlignment = HorizontalAlignment.Center;
                this.VerticalAlignment = VerticalAlignment.Bottom;
            }
            else if (ExpandDirection == ExpandDirection.Up)
            {
                this.HorizontalAlignment = HorizontalAlignment.Center;
                this.VerticalAlignment = VerticalAlignment.Top;
            }
            else if (ExpandDirection == ExpandDirection.Left)
            {
                this.HorizontalAlignment = HorizontalAlignment.Left;
                this.VerticalAlignment = VerticalAlignment.Center;
            }
            else if (ExpandDirection == ExpandDirection.Right)
            {
                this.HorizontalAlignment = HorizontalAlignment.Right;
                this.VerticalAlignment = VerticalAlignment.Center;
            }
        }

        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _thumb = GetTemplateChild(PART_Thumb) as Thumb;
            _pin = GetTemplateChild(PART_Pin) as ToggleButton;
            _esc = GetTemplateChild(PART_Esc) as Button;
            _toggle = GetTemplateChild(PART_ToggleSite) as ToggleButton;
            _min = GetTemplateChild(PART_Min) as Button;
            if (_thumb != null)
            {
                _thumb.DragStarted += _thumb_DragStarted;
                _thumb.DragDelta += _thumb_DragDelta;
                _thumb.DragCompleted += _thumb_DragCompleted;
            }
            if (_pin != null)
            {

            }
            if (_esc != null)
            {
                _esc.Click += _esc_Click;
            }
            if (_toggle != null)
            {
                _toggle.MouseEnter += _toggle_MouseEnter;
            }
            if (_min != null)
            {
                _min.Click += _min_Click;
            }
        }

        private void _min_Click(object sender, RoutedEventArgs e)
        {
            IsExpanded = false;
        }

        private void _toggle_MouseEnter(object sender, MouseEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
            }
            if (timer == null)
            {
                timer = new System.Windows.Threading.DispatcherTimer();
            }

            timer.Tick -= timeOpen;
            timer.Tick -= timeFold;
            timer.Tick += timeOpen;
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Start();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (timer != null)
            {
                timer.Stop();
            }
            if (timer == null)
            {
                timer = new System.Windows.Threading.DispatcherTimer();
            }

            timer.Tick -= timeOpen;
            timer.Tick -= timeFold;
            timer.Tick += timeFold;
            timer.Interval = new TimeSpan(0, 0, 0, 3);
            timer.Start();
        }

        private void timeOpen(object sender, EventArgs e)
        {
            if (IsPinned) return;

            IsExpanded = true;
        }

        private void timeFold(object sender, EventArgs e)
        {
            if (IsPinned) return;

            IsExpanded = false;
        }

        private void _esc_Click(object sender, RoutedEventArgs e)
        {
            switch (EscCommand)
            {
                case null:
                    return;
                case RoutedCommand command:
                    command.Execute(EscCommandParameter, CommandTarget);
                    break;
                default:
                    EscCommand.Execute(EscCommandParameter);
                    break;
            }
        }

        private void _thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;

            if (ExpandDirection == ExpandDirection.Down)
            {
                this.Margin = SetBottom();

            }
            else if (ExpandDirection == ExpandDirection.Up)
            {
                this.Margin = SetTop();
            }
            else if (ExpandDirection == ExpandDirection.Left)
            {
                this.Margin = SetLeft();
            }
            else if (ExpandDirection == ExpandDirection.Right)
            {
                this.Margin = SetRight();
            }
        }

        private void _thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double minLeft = double.MaxValue;
            double minTop = double.MaxValue;

            double left = Margin.Left;
            double top = Margin.Top;
            minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
            minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);

            double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
            double deltaVertical = Math.Max(-minTop, e.VerticalChange);
            Margin = new Thickness(Margin.Left + deltaHorizontal, Margin.Top + deltaVertical, 0, 0);
        }

        private void _thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            double left = this.Margin.Left;
            double top = this.Margin.Top;
            double centerleft = left + this.ActualWidth / 2;
            double centertop = top + this.ActualHeight / 2;
            Thickness newMargin;
            ExpandDirection expandDirection;
            HorizontalAlignment horizontalAlignment;
            VerticalAlignment verticalAlignment;

            if (JudgePointToLine(new Point(0, 0), new Point(_parent.ActualWidth, _parent.ActualHeight), new Point(centerleft, centertop)) < 0)
            {
                if (JudgePointToLine(new Point(_parent.ActualWidth, 0), new Point(0, _parent.ActualHeight), new Point(centerleft, centertop)) >= 0)
                {
                    newMargin = SetTop();
                    expandDirection = ExpandDirection.Up;
                    horizontalAlignment = HorizontalAlignment.Center;
                    verticalAlignment = VerticalAlignment.Top;
                }
                else
                {
                    newMargin = SetRight();
                    expandDirection = ExpandDirection.Right;
                    horizontalAlignment = HorizontalAlignment.Right;
                    verticalAlignment = VerticalAlignment.Center;
                }
            }
            else
            {
                if (JudgePointToLine(new Point(_parent.ActualWidth, 0), new Point(0, _parent.ActualHeight), new Point(centerleft, centertop)) >= 0)
                {
                    newMargin = SetLeft();
                    expandDirection = ExpandDirection.Left;
                    horizontalAlignment = HorizontalAlignment.Left;
                    verticalAlignment = VerticalAlignment.Center;
                }
                else
                {
                    newMargin = SetBottom();
                    expandDirection = ExpandDirection.Down;
                    horizontalAlignment = HorizontalAlignment.Center;
                    verticalAlignment = VerticalAlignment.Bottom;
                }
            }

            if (this.Margin != newMargin)
            {
                ThicknessAnimation marginAnimation = new ThicknessAnimation();
                marginAnimation.From = this.Margin;
                marginAnimation.To = newMargin;
                marginAnimation.Duration = TimeSpan.FromMilliseconds(200);

                Storyboard story = new Storyboard();
                story.FillBehavior = FillBehavior.Stop;
                story.Children.Add(marginAnimation);
                Storyboard.SetTarget(marginAnimation, this);
                Storyboard.SetTargetProperty(marginAnimation, new PropertyPath("(0)", Border.MarginProperty));

                story.Begin(this);
            }
            this.Margin = new Thickness();
            this.ExpandDirection = expandDirection;
            this.HorizontalAlignment = horizontalAlignment;
            this.VerticalAlignment = verticalAlignment;
        }

        private Thickness SetBottom()
        {
            double left = (_parent.ActualWidth - this.ActualWidth) / 2;
            double top = (_parent.ActualHeight - this.ActualHeight);
            return new Thickness(left, top, 0, 0);
        }

        private Thickness SetTop()
        {
            double left = (_parent.ActualWidth - this.ActualWidth) / 2;
            double top = 0;
            return new Thickness(left, top, 0, 0);
        }

        private Thickness SetLeft()
        {
            double left = 0;
            double top = (_parent.ActualHeight - this.ActualHeight) / 2;
            return new Thickness(left, top, 0, 0);
        }

        private Thickness SetRight()
        {
            double left = (_parent.ActualWidth - this.ActualWidth);
            double top = (_parent.ActualHeight - this.ActualHeight) / 2;
            return new Thickness(left, top, 0, 0);
        }

        /// <summary>
        /// 判断点和直线的位置关系
        /// </summary>
        /// <param name="LinePntA">直线上的一点</param>
        /// <param name="LinePntB">直线上的另一点</param>
        /// <param name="PntM">需要判断的点</param>
        /// <returns></returns>
        private int JudgePointToLine(Point LinePntA, Point LinePntB, Point PntM)
        {
            int nResult = 0;
            double ax = LinePntB.X - LinePntA.X;
            double ay = LinePntB.Y - LinePntA.Y;
            double bx = PntM.X - LinePntA.X;
            double by = PntM.Y - LinePntA.Y;
            double judge = ax * by - ay * bx;
            if (judge > 0)
            {
                nResult = 1;
            }
            else if (judge < 0)
            {
                nResult = -1;
            }
            else
            {
                nResult = 0;
            }
            return nResult;
        }
    }
}