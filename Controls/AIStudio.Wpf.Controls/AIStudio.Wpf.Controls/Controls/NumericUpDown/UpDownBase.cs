using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Spinner, Type = typeof(Spinner))]
    public abstract class UpDownBase<T> : Control
    {
        #region AutoMoveFocus

        public bool AutoMoveFocus
        {
            get
            {
                return (bool)GetValue(AutoMoveFocusProperty);
            }
            set
            {
                SetValue(AutoMoveFocusProperty, value);
            }
        }

        public static readonly DependencyProperty AutoMoveFocusProperty =
            DependencyProperty.Register("AutoMoveFocus", typeof(bool), typeof(UpDownBase<T>), new UIPropertyMetadata(false));

        #endregion AutoMoveFocus

        #region AutoSelectBehavior

        public AutoSelectBehavior AutoSelectBehavior
        {
            get
            {
                return (AutoSelectBehavior)GetValue(AutoSelectBehaviorProperty);
            }
            set
            {
                SetValue(AutoSelectBehaviorProperty, value);
            }
        }

        public static readonly DependencyProperty AutoSelectBehaviorProperty =
            DependencyProperty.Register("AutoSelectBehavior", typeof(AutoSelectBehavior), typeof(UpDownBase<T>),
          new UIPropertyMetadata(AutoSelectBehavior.OnFocus));

        #endregion AutoSelectBehavior PROPERTY

        /// <summary>Identifies the <see cref="UpDownButtonsWidth"/> dependency property.</summary>
        public static readonly DependencyProperty UpDownButtonsWidthProperty
            = DependencyProperty.Register(nameof(UpDownButtonsWidth), typeof(double), typeof(UpDownBase<T>), new PropertyMetadata(20d));

        /// <summary>
        /// Gets or sets the width of the up/down buttons.
        /// </summary>

        public double UpDownButtonsWidth
        {
            get => (double)this.GetValue(UpDownButtonsWidthProperty);
            set => this.SetValue(UpDownButtonsWidthProperty, value);
        }

        /// <summary>Identifies the <see cref="Delay"/> dependency property.</summary>
        public static readonly DependencyProperty DelayProperty
            = DependencyProperty.Register(nameof(Delay), typeof(int), typeof(UpDownBase<T>), new FrameworkPropertyMetadata(500));



        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, the ButtonSpinner waits while the up/down button is pressed
        /// before it starts increasing/decreasing the <see cref="Value" /> for the specified <see cref="Interval" /> .
        /// The value must be non-negative.
        /// </summary>
        public int Delay
        {
            get => (int)this.GetValue(DelayProperty);
            set => this.SetValue(DelayProperty, value);
        }

        #region FormatString

        public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register("FormatString", typeof(string), typeof(UpDownBase<T>), new UIPropertyMetadata(String.Empty, OnFormatStringChanged));
        public string FormatString
        {
            get
            {
                return (string)GetValue(FormatStringProperty);
            }
            set
            {
                SetValue(FormatStringProperty, value);
            }
        }

        private static void OnFormatStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> numericUpDown = o as UpDownBase<T>;
            if (numericUpDown != null)
                numericUpDown.OnFormatStringChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnFormatStringChanged(string oldValue, string newValue)
        {
            if (IsInitialized)
            {
                this.SyncTextAndValueProperties(false, null);
            }
        }

        #endregion //FormatString

        #region Increment

        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(T), typeof(UpDownBase<T>), new PropertyMetadata(default(T), OnIncrementChanged, OnCoerceIncrement));
        public T Increment
        {
            get
            {
                return (T)GetValue(IncrementProperty);
            }
            set
            {
                SetValue(IncrementProperty, value);
            }
        }

        private static void OnIncrementChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> numericUpDown = o as UpDownBase<T>;
            if (numericUpDown != null)
                numericUpDown.OnIncrementChanged((T)e.OldValue, (T)e.NewValue);
        }

        protected virtual void OnIncrementChanged(T oldValue, T newValue)
        {
            if (this.IsInitialized)
            {
                SetValidSpinDirection();
            }
        }

        private static object OnCoerceIncrement(DependencyObject d, object baseValue)
        {
            UpDownBase<T> numericUpDown = d as UpDownBase<T>;
            if (numericUpDown != null)
                return numericUpDown.OnCoerceIncrement((T)baseValue);

            return baseValue;
        }

        protected virtual T OnCoerceIncrement(T baseValue)
        {
            return baseValue;
        }

        #endregion

        #region AllowTextInput

        public static readonly DependencyProperty AllowTextInputProperty = DependencyProperty.Register("AllowTextInput", typeof(bool), typeof(UpDownBase<T>), new UIPropertyMetadata(true, OnAllowTextInputChanged));
        public bool AllowTextInput
        {
            get
            {
                return (bool)GetValue(AllowTextInputProperty);
            }
            set
            {
                SetValue(AllowTextInputProperty, value);
            }
        }

        private static void OnAllowTextInputChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> inputBase = o as UpDownBase<T>;
            if (inputBase != null)
                inputBase.OnAllowTextInputChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnAllowTextInputChanged(bool oldValue, bool newValue)
        {
        }

        #endregion //AllowTextInput

        #region CultureInfo

        public static readonly DependencyProperty CultureInfoProperty = DependencyProperty.Register("CultureInfo", typeof(CultureInfo), typeof(UpDownBase<T>), new UIPropertyMetadata(CultureInfo.CurrentCulture, OnCultureInfoChanged));
        public CultureInfo CultureInfo
        {
            get
            {
                return (CultureInfo)GetValue(CultureInfoProperty);
            }
            set
            {
                SetValue(CultureInfoProperty, value);
            }
        }

        private static void OnCultureInfoChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> inputBase = o as UpDownBase<T>;
            if (inputBase != null)
                inputBase.OnCultureInfoChanged((CultureInfo)e.OldValue, (CultureInfo)e.NewValue);
        }

        protected virtual void OnCultureInfoChanged(CultureInfo oldValue, CultureInfo newValue)
        {
            if (IsInitialized)
            {
                SyncTextAndValueProperties(false, null);
            }
        }

        #endregion //CultureInfo

        #region IsReadOnly

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(UpDownBase<T>), new UIPropertyMetadata(false, OnReadOnlyChanged));
        public bool IsReadOnly
        {
            get
            {
                return (bool)GetValue(IsReadOnlyProperty);
            }
            set
            {
                SetValue(IsReadOnlyProperty, value);
            }
        }

        private static void OnReadOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> inputBase = o as UpDownBase<T>;
            if (inputBase != null)
                inputBase.OnReadOnlyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnReadOnlyChanged(bool oldValue, bool newValue)
        {
            SetValidSpinDirection();
        }

        #endregion //IsReadOnly

        #region IsUndoEnabled

        public static readonly DependencyProperty IsUndoEnabledProperty = DependencyProperty.Register("IsUndoEnabled", typeof(bool), typeof(UpDownBase<T>), new UIPropertyMetadata(true, OnIsUndoEnabledChanged));
        public bool IsUndoEnabled
        {
            get
            {
                return (bool)GetValue(IsUndoEnabledProperty);
            }
            set
            {
                SetValue(IsUndoEnabledProperty, value);
            }
        }

        private static void OnIsUndoEnabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> inputBase = o as UpDownBase<T>;
            if (inputBase != null)
                inputBase.OnIsUndoEnabledChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnIsUndoEnabledChanged(bool oldValue, bool newValue)
        {
        }

        #endregion //IsUndoEnabled

        #region Text

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(UpDownBase<T>), new FrameworkPropertyMetadata(default(String), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged, null, false, UpdateSourceTrigger.LostFocus));
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        private static void OnTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> inputBase = o as UpDownBase<T>;
            if (inputBase != null)
                inputBase.OnTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnTextChanged(string oldValue, string newValue)
        {
            if (this.IsInitialized)
            {
                // When text is typed, if UpdateValueOnEnterKey is true, 
                // Sync Value on Text only when Enter Key is pressed.
                if (this.UpdateValueOnEnterKey)
                {
                    if (!_isTextChangedFromUI)
                    {
                        this.SyncTextAndValueProperties(true, Text);
                    }
                }
                else
                {
                    this.SyncTextAndValueProperties(true, Text);
                }
            }
        }

        #endregion //Text

        #region TextAlignment

        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(UpDownBase<T>), new UIPropertyMetadata(TextAlignment.Left));
        public TextAlignment TextAlignment
        {
            get
            {
                return (TextAlignment)GetValue(TextAlignmentProperty);
            }
            set
            {
                SetValue(TextAlignmentProperty, value);
            }
        }


        #endregion //TextAlignment

        #region Members

        /// <summary>
        /// Name constant for Text template part.
        /// </summary>
        internal const string PART_TextBox = "PART_TextBox";

        /// <summary>
        /// Name constant for Spinner template part.
        /// </summary>
        internal const string PART_Spinner = "PART_Spinner";

        internal bool _isTextChangedFromUI;

        /// <summary>
        /// Flags if the Text and Value properties are in the process of being sync'd
        /// </summary>
        private bool _isSyncingTextAndValueProperties;
        private bool _internalValueSet;

        #endregion //Members

        #region Properties

        protected Spinner Spinner
        {
            get;
            private set;
        }
        protected TextBox TextBox
        {
            get;
            private set;
        }

        #region AllowSpin

        public static readonly DependencyProperty AllowSpinProperty = DependencyProperty.Register("AllowSpin", typeof(bool), typeof(UpDownBase<T>), new UIPropertyMetadata(true));
        public bool AllowSpin
        {
            get
            {
                return (bool)GetValue(AllowSpinProperty);
            }
            set
            {
                SetValue(AllowSpinProperty, value);
            }
        }

        #endregion //AllowSpin

        #region ButtonSpinnerLocation

        public static readonly DependencyProperty ButtonSpinnerLocationProperty = DependencyProperty.Register(nameof(ButtonSpinnerLocation), typeof(Location), typeof(UpDownBase<T>), new UIPropertyMetadata(Location.Right));
        public Location ButtonSpinnerLocation
        {
            get
            {
                return (Location)GetValue(ButtonSpinnerLocationProperty);
            }
            set
            {
                SetValue(ButtonSpinnerLocationProperty, value);
            }
        }

        #endregion //ButtonSpinnerLocation

        #region ClipValueToMinMax

        public static readonly DependencyProperty ClipValueToMinMaxProperty = DependencyProperty.Register("ClipValueToMinMax", typeof(bool), typeof(

    UpDownBase<T>), new UIPropertyMetadata(false));
        public bool ClipValueToMinMax
        {
            get
            {
                return (bool)GetValue(ClipValueToMinMaxProperty);
            }
            set
            {
                SetValue(ClipValueToMinMaxProperty, value);
            }
        }

        #endregion //ClipValueToMinMax

        #region DisplayDefaultValueOnEmptyText

        public static readonly DependencyProperty DisplayDefaultValueOnEmptyTextProperty = DependencyProperty.Register("DisplayDefaultValueOnEmptyText",

    typeof(bool), typeof(UpDownBase<T>), new UIPropertyMetadata(false, OnDisplayDefaultValueOnEmptyTextChanged));
        public bool DisplayDefaultValueOnEmptyText
        {
            get
            {
                return (bool)GetValue(DisplayDefaultValueOnEmptyTextProperty);
            }
            set
            {
                SetValue(DisplayDefaultValueOnEmptyTextProperty, value);
            }
        }

        private static void OnDisplayDefaultValueOnEmptyTextChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ((UpDownBase<T>)source).OnDisplayDefaultValueOnEmptyTextChanged((bool)args.OldValue, (bool)args.NewValue);
        }

        private void OnDisplayDefaultValueOnEmptyTextChanged(bool oldValue, bool newValue)
        {
            if (this.IsInitialized && string.IsNullOrEmpty(Text))
            {
                this.SyncTextAndValueProperties(false, Text);
            }
        }

        #endregion //DisplayDefaultValueOnEmptyText

        #region DefaultValue

        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(T), typeof(UpDownBase<T>

    ), new UIPropertyMetadata(default(T), OnDefaultValueChanged));
        public T DefaultValue
        {
            get
            {
                return (T)GetValue(DefaultValueProperty);
            }
            set
            {
                SetValue(DefaultValueProperty, value);
            }
        }

        private static void OnDefaultValueChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ((UpDownBase<T>)source).OnDefaultValueChanged((T)args.OldValue, (T)args.NewValue);
        }

        private void OnDefaultValueChanged(T oldValue, T newValue)
        {
            if (this.IsInitialized && string.IsNullOrEmpty(Text))
            {
                this.SyncTextAndValueProperties(true, Text);
            }
        }

        #endregion //DefaultValue

        #region Maximum

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(T), typeof(UpDownBase<T>), new

    UIPropertyMetadata(default(T), OnMaximumChanged, OnCoerceMaximum));
        public T Maximum
        {
            get
            {
                return (T)GetValue(MaximumProperty);
            }
            set
            {
                SetValue(MaximumProperty, value);
            }
        }

        private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> upDown = o as UpDownBase<T>;
            if (upDown != null)
                upDown.OnMaximumChanged((T)e.OldValue, (T)e.NewValue);
        }

        protected virtual void OnMaximumChanged(T oldValue, T newValue)
        {
            if (this.IsInitialized)
            {
                SetValidSpinDirection();
            }
        }

        private static object OnCoerceMaximum(DependencyObject d, object baseValue)
        {
            UpDownBase<T> upDown = d as UpDownBase<T>;
            if (upDown != null)
                return upDown.OnCoerceMaximum((T)baseValue);

            return baseValue;
        }

        protected virtual T OnCoerceMaximum(T baseValue)
        {
            return baseValue;
        }

        #endregion //Maximum

        #region Minimum

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(T), typeof(UpDownBase<T>), new

    UIPropertyMetadata(default(T), OnMinimumChanged, OnCoerceMinimum));
        public T Minimum
        {
            get
            {
                return (T)GetValue(MinimumProperty);
            }
            set
            {
                SetValue(MinimumProperty, value);
            }
        }

        private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> upDown = o as UpDownBase<T>;
            if (upDown != null)
                upDown.OnMinimumChanged((T)e.OldValue, (T)e.NewValue);
        }

        protected virtual void OnMinimumChanged(T oldValue, T newValue)
        {
            if (this.IsInitialized)
            {
                SetValidSpinDirection();
            }
        }

        private static object OnCoerceMinimum(DependencyObject d, object baseValue)
        {
            UpDownBase<T> upDown = d as UpDownBase<T>;
            if (upDown != null)
                return upDown.OnCoerceMinimum((T)baseValue);

            return baseValue;
        }

        protected virtual T OnCoerceMinimum(T baseValue)
        {
            return baseValue;
        }

        #endregion //Minimum

        #region MouseWheelActiveTrigger

        /// <summary>
        /// Identifies the MouseWheelActiveTrigger dependency property
        /// </summary>
        public static readonly DependencyProperty MouseWheelActiveTriggerProperty = DependencyProperty.Register("MouseWheelActiveTrigger", typeof(MouseWheelActiveTrigger), typeof(UpDownBase<T>), new UIPropertyMetadata(MouseWheelActiveTrigger.FocusedMouseOver));

        /// <summary>
        /// Get or set when the mouse wheel event should affect the value.
        /// </summary>
        public MouseWheelActiveTrigger MouseWheelActiveTrigger
        {
            get
            {
                return (MouseWheelActiveTrigger)GetValue(MouseWheelActiveTriggerProperty);
            }
            set
            {
                SetValue(MouseWheelActiveTriggerProperty, value);
            }
        }

        #endregion //MouseWheelActiveTrigger

        #region MouseWheelActiveOnFocus

        [Obsolete("Use MouseWheelActiveTrigger property instead")]
        public static readonly DependencyProperty MouseWheelActiveOnFocusProperty = DependencyProperty.Register("MouseWheelActiveOnFocus", typeof(bool

    ), typeof(UpDownBase<T>), new UIPropertyMetadata(true, OnMouseWheelActiveOnFocusChanged));

        [Obsolete("Use MouseWheelActiveTrigger property instead")]
        public bool MouseWheelActiveOnFocus
        {
            get
            {
                return (bool)GetValue(MouseWheelActiveOnFocusProperty);
            }
            set
            {
                SetValue(MouseWheelActiveOnFocusProperty, value);
            }
        }

        private static void OnMouseWheelActiveOnFocusChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> upDownBase = o as UpDownBase<T>;
            if (upDownBase != null)
                upDownBase.MouseWheelActiveTrigger = ((bool)e.NewValue)
                  ? MouseWheelActiveTrigger.FocusedMouseOver
                  : MouseWheelActiveTrigger.MouseOver;
        }

        #endregion //MouseWheelActiveOnFocus

        #region ShowButtonSpinner

        public static readonly DependencyProperty ShowButtonSpinnerProperty = DependencyProperty.Register("ShowButtonSpinner", typeof(bool), typeof(

    UpDownBase<T>), new UIPropertyMetadata(true));
        public bool ShowButtonSpinner
        {
            get
            {
                return (bool)GetValue(ShowButtonSpinnerProperty);
            }
            set
            {
                SetValue(ShowButtonSpinnerProperty, value);
            }
        }

        #endregion //ShowButtonSpinner

        #region UpdateValueOnEnterKey

        public static readonly DependencyProperty UpdateValueOnEnterKeyProperty = DependencyProperty.Register("UpdateValueOnEnterKey", typeof(bool), typeof(UpDownBase<T>),
          new FrameworkPropertyMetadata(false, OnUpdateValueOnEnterKeyChanged));
        public bool UpdateValueOnEnterKey
        {
            get
            {
                return (bool)GetValue(UpdateValueOnEnterKeyProperty);
            }
            set
            {
                SetValue(UpdateValueOnEnterKeyProperty, value);
            }
        }

        private static void OnUpdateValueOnEnterKeyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var upDownBase = o as UpDownBase<T>;
            if (upDownBase != null)
                upDownBase.OnUpdateValueOnEnterKeyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnUpdateValueOnEnterKeyChanged(bool oldValue, bool newValue)
        {
        }

        #endregion //UpdateValueOnEnterKey

        #region Value

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(T), typeof(UpDownBase<T>),
          new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, OnCoerceValue, false, UpdateSourceTrigger.PropertyChanged));
        public T Value
        {
            get
            {
                return (T)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        private void SetValueInternal(T value)
        {
            _internalValueSet = true;
            try
            {
                this.Value = value;
            }
            finally
            {
                _internalValueSet = false;
            }
        }

        private static object OnCoerceValue(DependencyObject o, object basevalue)
        {
            return ((UpDownBase<T>)o).OnCoerceValue(basevalue);
        }

        protected virtual object OnCoerceValue(object newValue)
        {
            return newValue;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpDownBase<T> upDownBase = o as UpDownBase<T>;
            if (upDownBase != null)
                upDownBase.OnValueChanged((T)e.OldValue, (T)e.NewValue);
        }

        protected virtual void OnValueChanged(T oldValue, T newValue)
        {
            if (!_internalValueSet && this.IsInitialized)
            {
                SyncTextAndValueProperties(false, null, true);
            }

            SetValidSpinDirection();

            this.RaiseValueChangedEvent(oldValue, newValue);
        }

        #endregion //Value

        #endregion //Properties

        #region Constructors

        internal UpDownBase()
        {
            this.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new RoutedEventHandler(this.HandleClickOutsideOfControlWithMouseCapture),

      true);
            this.IsKeyboardFocusWithinChanged += this.UpDownBase_IsKeyboardFocusWithinChanged;
        }

        #endregion //Constructors

        #region Base Class Overrides

        protected override void OnAccessKey(AccessKeyEventArgs e)
        {
            if (TextBox != null)
                TextBox.Focus();

            base.OnAccessKey(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (TextBox != null)
            {
                TextBox.TextChanged -= new TextChangedEventHandler(TextBox_TextChanged);
                TextBox.RemoveHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(this.TextBox_PreviewMouseDown));
            }

            TextBox = GetTemplateChild(PART_TextBox) as TextBox;

            if (TextBox != null)
            {
                TextBox.Text = Text;
                TextBox.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);
                TextBox.AddHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(this.TextBox_PreviewMouseDown), true);
            }

            if (Spinner != null)
                Spinner.Spin -= OnSpinnerSpin;

            Spinner = GetTemplateChild(PART_Spinner) as Spinner;

            if (Spinner != null)
                Spinner.Spin += OnSpinnerSpin;

            SetValidSpinDirection();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        // Commit Text on "Enter" to raise Error event 
                        bool commitSuccess = CommitInput();
                        //Only handle if an exception is detected (Commit fails)
                        e.Handled = !commitSuccess;
                        break;
                    }
            }
        }

        #endregion //Base Class Overrides

        #region Event Handlers

        private void TextBox_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            if (this.MouseWheelActiveTrigger == MouseWheelActiveTrigger.Focused)
            {
                //Capture the spinner when user clicks on the control.
                if (Mouse.Captured != this.Spinner)
                {
                    //Delay the capture to let the DateTimeUpDown select a new DateTime part.
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => {
                        Mouse.Capture(this.Spinner);
                    }
                    ));
                }
            }
        }

        private void HandleClickOutsideOfControlWithMouseCapture(object sender, RoutedEventArgs e)
        {
            if (Mouse.Captured is Spinner)
            {
                //Release the captured spinner when user clicks away from spinner.
                this.Spinner.ReleaseMouseCapture();
            }
        }

        private void OnSpinnerSpin(object sender, SpinEventArgs e)
        {
            if (AllowSpin && !IsReadOnly)
            {
                var activeTrigger = this.MouseWheelActiveTrigger;
                bool spin = !e.UsingMouseWheel;
                spin |= (activeTrigger == MouseWheelActiveTrigger.MouseOver);
                spin |= ((TextBox != null) && TextBox.IsFocused && (activeTrigger == MouseWheelActiveTrigger.FocusedMouseOver));
                spin |= ((TextBox != null) && TextBox.IsFocused && (activeTrigger == MouseWheelActiveTrigger.Focused) && (Mouse.Captured is Spinner));

                if (spin)
                {
                    e.Handled = true;
                    OnSpin(e);
                }
            }
        }

        #endregion //Event Handlers

        #region Events

        public event InputValidationErrorEventHandler InputValidationError;

        public event EventHandler<SpinEventArgs> Spinned;

        #region ValueChanged Event

        //Due to a bug in Visual Studio, you cannot create event handlers for generic T args in XAML, so I have to use object instead.
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(

    RoutedPropertyChangedEventHandler<object>), typeof(UpDownBase<T>));
        public event RoutedPropertyChangedEventHandler<object> ValueChanged
        {
            add
            {
                AddHandler(ValueChangedEvent, value);
            }
            remove
            {
                RemoveHandler(ValueChangedEvent, value);
            }
        }

        #endregion

        #endregion //Events

        #region Methods

        protected virtual void OnSpin(SpinEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            // Raise the Spinned event to user
            EventHandler<SpinEventArgs> handler = this.Spinned;
            if (handler != null)
            {
                handler(this, e);
            }

            if (e.Direction == SpinDirection.Increase)
                DoIncrement();
            else
                DoDecrement();
        }

        protected virtual void RaiseValueChangedEvent(T oldValue, T newValue)
        {
            RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(oldValue, newValue);
            args.RoutedEvent = ValueChangedEvent;
            RaiseEvent(args);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            // When both Value and Text are initialized, Value has priority.
            // To be sure that the value is not initialized, it should
            // have no local value, no binding, and equal to the default value.
            bool updateValueFromText =
              (this.ReadLocalValue(ValueProperty) == DependencyProperty.UnsetValue)
              && (BindingOperations.GetBinding(this, ValueProperty) == null)
              && (object.Equals(this.Value, ValueProperty.DefaultMetadata.DefaultValue));

            this.SyncTextAndValueProperties(updateValueFromText, Text, !updateValueFromText);
        }

        /// <summary>
        /// Performs an increment if conditions allow it.
        /// </summary>
        internal void DoDecrement()
        {
            if (Spinner == null || (Spinner.ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease)
            {
                OnDecrement();
            }
        }

        /// <summary>
        /// Performs a decrement if conditions allow it.
        /// </summary>
        internal void DoIncrement()
        {
            if (Spinner == null || (Spinner.ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase)
            {
                OnIncrement();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (!this.IsKeyboardFocusWithin)
            //    return;

            try
            {
                _isTextChangedFromUI = true;
                Text = ((TextBox)sender).Text;
            }
            finally
            {
                _isTextChangedFromUI = false;
            }
        }

        private void UpDownBase_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                this.CommitInput();
            }
        }

        private void RaiseInputValidationError(Exception e)
        {
            if (InputValidationError != null)
            {
                InputValidationErrorEventArgs args = new InputValidationErrorEventArgs(e);
                InputValidationError(this, args);
                if (args.ThrowException)
                {
                    throw args.Exception;
                }
            }
        }

        public virtual bool CommitInput()
        {
            return this.SyncTextAndValueProperties(true, Text);
        }

        protected bool SyncTextAndValueProperties(bool updateValueFromText, string text)
        {
            return this.SyncTextAndValueProperties(updateValueFromText, text, false);
        }

        private bool SyncTextAndValueProperties(bool updateValueFromText, string text, bool forceTextUpdate)
        {
            if (_isSyncingTextAndValueProperties)
                return true;

            _isSyncingTextAndValueProperties = true;
            bool parsedTextIsValid = true;
            try
            {
                if (updateValueFromText)
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        // An empty input sets the value to the default value.
                        this.SetValueInternal(this.DefaultValue);
                    }
                    else
                    {
                        try
                        {
                            T newValue = this.ConvertTextToValue(text);
                            if (!object.Equals(newValue, this.Value))
                            {
                                this.SetValueInternal(newValue);
                            }
                        }
                        catch (Exception e)
                        {
                            parsedTextIsValid = false;

                            // From the UI, just allow any input.
                            if (!_isTextChangedFromUI)
                            {
                                // This call may throw an exception. 
                                // See RaiseInputValidationError() implementation.
                                this.RaiseInputValidationError(e);
                            }
                        }
                    }
                }

                // Do not touch the ongoing text input from user.
                if (!_isTextChangedFromUI)
                {
                    // Don't replace the empty Text with the non-empty representation of DefaultValue.
                    bool shouldKeepEmpty = !forceTextUpdate && string.IsNullOrEmpty(Text) && object.Equals(Value, DefaultValue) && !this.DisplayDefaultValueOnEmptyText;
                    if (!shouldKeepEmpty)
                    {
                        string newText = ConvertValueToText();
                        if (!object.Equals(this.Text, newText))
                        {
                            Text = newText;
                        }
                    }

                    // Sync Text and textBox
                    if (TextBox != null)
                    {
                        TextBox.Text = Text;
                    }
                }

                if (_isTextChangedFromUI && !parsedTextIsValid)
                {
                    // Text input was made from the user and the text
                    // repesents an invalid value. Disable the spinner
                    // in this case.
                    if (Spinner != null)
                    {
                        Spinner.ValidSpinDirection = ValidSpinDirections.None;
                    }
                }
                else
                {
                    this.SetValidSpinDirection();
                }
            }
            finally
            {
                _isSyncingTextAndValueProperties = false;
            }
            return parsedTextIsValid;
        }

        #region Abstract

        /// <summary>
        /// Converts the formatted text to a value.
        /// </summary>
        protected abstract T ConvertTextToValue(string text);

        /// <summary>
        /// Converts the value to formatted text.
        /// </summary>
        /// <returns></returns>
        protected abstract string ConvertValueToText();

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Increase.
        /// </summary>
        protected abstract void OnIncrement();

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Descrease.
        /// </summary>
        protected abstract void OnDecrement();

        /// <summary>
        /// Sets the valid spin directions.
        /// </summary>
        protected abstract void SetValidSpinDirection();

        #endregion //Abstract

        protected static decimal ParsePercent(string text, IFormatProvider cultureInfo)
        {
            NumberFormatInfo info = NumberFormatInfo.GetInstance(cultureInfo);

            text = text.Replace(info.PercentSymbol, null);

            decimal result = Decimal.Parse(text, NumberStyles.Any, info);
            result = result / 100;

            return result;
        }

        #endregion //Methods
    }

    public delegate void InputValidationErrorEventHandler(object sender, InputValidationErrorEventArgs e);

    public class InputValidationErrorEventArgs : EventArgs
    {
        #region Constructors

        public InputValidationErrorEventArgs(Exception e)
        {
            Exception = e;
        }

        #endregion

        #region Exception Property

        public Exception Exception
        {
            get
            {
                return exception;
            }
            private set
            {
                exception = value;
            }
        }

        private Exception exception;

        #endregion

        #region ThrowException Property

        public bool ThrowException
        {
            get
            {
                return _throwException;
            }
            set
            {
                _throwException = value;
            }
        }

        private bool _throwException;

        #endregion
    }

    /// <summary>
    /// Specify when the mouse wheel is active.
    /// </summary>
    public enum MouseWheelActiveTrigger
    {
        Focused,
        FocusedMouseOver,
        MouseOver,
        Disabled
    }

    public enum Location
    {
        Left,
        Right,
        Opposite,
        UpDown,
    }
}
