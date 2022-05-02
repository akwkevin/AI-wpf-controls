using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_RootGrid, Type = typeof(Grid))]
    [TemplatePart(Name = PART_ControlStatus, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_CornerRadius, Type = typeof(NumericUpDown))]
    [TemplatePart(Name = PART_MinHeight, Type = typeof(NumericUpDown))]
    [TemplatePart(Name = PART_IconWidth, Type = typeof(NumericUpDown))]
    [TemplatePart(Name = PART_FontFamily, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_FontSize, Type = typeof(NumericUpDown))]
    [TemplatePart(Name = PART_ToggleCode, Type = typeof(ToggleButton))]
    [TemplatePart(Name = PART_OptionsPanel, Type = typeof(OptionsPanel))]
    public class AIStudioUserControl : UserControl, INotifyPropertyChanged
    {
        private const string PART_RootGrid = "PART_RootGrid";
        private const string PART_ControlStatus = "PART_ControlStatus";
        private const string PART_CornerRadius = "PART_CornerRadius";
        private const string PART_MinHeight = "PART_MinHeight";
        private const string PART_IconWidth = "PART_IconWidth";
        private const string PART_FontFamily = "PART_FontFamily";
        private const string PART_FontSize = "PART_FontSize";
        private const string PART_ToggleCode = "PART_ToggleCode";
        private const string PART_OptionsPanel = "PART_OptionsPanel";

        Grid _rootGrid;

        private ComboBox _controlStatus;
        public ComboBox ControlStatus
        {
            get
            {
                return _controlStatus;
            }
            set
            {
                _controlStatus = value;
                OnPropertyChanged(nameof(ControlStatus));
            }
        }

        private NumericUpDown _controlCornerRadius;
        public NumericUpDown ControlCornerRadius
        {
            get
            {
                return _controlCornerRadius;
            }
            set
            {
                _controlCornerRadius = value;
                OnPropertyChanged(nameof(ControlCornerRadius));
            }
        }

        private NumericUpDown _controlMinHeight;
        public NumericUpDown ControlMinHeight
        {
            get
            {
                return _controlMinHeight;
            }
            set
            {
                _controlMinHeight = value;
                OnPropertyChanged(nameof(ControlMinHeight));
            }
        }

        private NumericUpDown _controlIconWidth;
        public NumericUpDown ControlIconWidth
        {
            get
            {
                return _controlIconWidth;
            }
            set
            {
                _controlIconWidth = value;
                OnPropertyChanged(nameof(ControlIconWidth));
            }
        }

        private ComboBox _controlFontFamily;
        public ComboBox ControlFontFamily
        {
            get
            {
                return _controlFontFamily;
            }
            set
            {
                _controlFontFamily = value;
                OnPropertyChanged(nameof(ControlFontFamily));
            }
        }

        private NumericUpDown _controlFontSize;
        public NumericUpDown ControlFontSize
        {
            get
            {
                return _controlFontSize;
            }
            set
            {
                _controlFontSize = value;
                OnPropertyChanged(nameof(ControlFontSize));
            }
        }

        private ToggleButton _codeToggleButton;
        public ToggleButton CodeToggleButton
        {
            get
            {
                return _codeToggleButton;
            }
            set
            {
                _codeToggleButton = value;
                OnPropertyChanged(nameof(CodeToggleButton));
            }
        }

        private OptionsPanel _optionsPanel;
        public OptionsPanel OptionsPanel
        {
            get
            {
                return _optionsPanel;
            }
            set
            {
                _optionsPanel = value;
                OnPropertyChanged(nameof(OptionsPanel));
            }
        }

        public static List<FontFamily> SystemFontFamilies
        {
            get; set;
        }

        #region IsWaiting 等待中
        public static readonly DependencyProperty IsWaitingProperty =
            DependencyProperty.Register(nameof(IsWaiting), typeof(bool), typeof(AIStudioUserControl), new PropertyMetadata(false));
        /// <summary>
        /// 等待中
        /// </summary>
        public bool IsWaiting
        {
            get
            {
                return (bool)GetValue(IsWaitingProperty);
            }
            set
            {
                SetValue(IsWaitingProperty, value);
            }
        }
        #endregion

        #region ShowOverlay 显示蒙板
        public static readonly DependencyProperty ShowOverlayProperty =
            DependencyProperty.Register(nameof(ShowOverlay), typeof(bool), typeof(AIStudioUserControl), new PropertyMetadata(false));
        /// <summary>
        /// 显示蒙板
        /// </summary>
        public bool ShowOverlay
        {
            get
            {
                return (bool)GetValue(ShowOverlayProperty);
            }
            set
            {
                SetValue(ShowOverlayProperty, value);
            }
        }
        #endregion

        #region WaitInfo 显示消息
        public static readonly DependencyProperty WaitInfoProperty =
            DependencyProperty.Register(nameof(WaitInfo), typeof(string), typeof(AIStudioUserControl), new PropertyMetadata(null));
        /// <summary>
        /// 等待消息
        /// </summary>
        public string WaitInfo
        {
            get
            {
                return (string)GetValue(WaitInfoProperty);
            }
            set
            {
                SetValue(WaitInfoProperty, value);
            }
        }
        #endregion

        #region ShowCancel 可以取消
        public static readonly DependencyProperty ShowCancelProperty =
            DependencyProperty.Register(nameof(ShowCancel), typeof(bool), typeof(AIStudioUserControl), new PropertyMetadata(false));
        /// <summary>
        /// 可以取消
        /// </summary>
        public bool ShowCancel
        {
            get
            {
                return (bool)GetValue(ShowCancelProperty);
            }
            set
            {
                SetValue(ShowCancelProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets text.
        /// </summary>
        public string Code
        {
            get
            {
                return (string)GetValue(CodeProperty);
            }
            set
            {
                SetValue(CodeProperty, value);
            }
        }
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register(nameof(Code), typeof(string), typeof(AIStudioUserControl));

        public ICommand CancelCommmad
        {
            get; protected set;
        }

        static AIStudioUserControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AIStudioUserControl), new FrameworkPropertyMetadata(typeof(AIStudioUserControl)));

            SystemFontFamilies = new List<FontFamily>();
            foreach (FontFamily _f in Fonts.SystemFontFamilies)
            {
                LanguageSpecificStringDictionary _fontDic = _f.FamilyNames;
                if (_fontDic.ContainsKey(XmlLanguage.GetLanguage("zh-cn")))
                {
                    string _fontName = null;
                    if (_fontDic.TryGetValue(XmlLanguage.GetLanguage("zh-cn"), out _fontName))
                    {
                        SystemFontFamilies.Add(new FontFamily(_fontName));
                    }
                }
                //else
                //{
                //    string _fontName = null;
                //    if (_fontDic.TryGetValue(XmlLanguage.GetLanguage("en-us"), out _fontName))
                //    {
                //        SystemFontFamilies.Add(new FontFamily(_fontName));
                //    }
                //}
            }
        }

        public AIStudioUserControl()
        {
            this.CancelCommmad = new RoutedUICommand();

            this.BindCommand(CancelCommmad, this.Cancel);

            this.Loaded += AIStudioUserControl_Loaded;
        }

        private void Cancel(object sender, ExecutedRoutedEventArgs e)
        {
            SetCurrentValue(IsWaitingProperty, false);
        }

        protected virtual void ShowWait(string info, bool showOverlay = false)
        {
            SetCurrentValue(IsWaitingProperty, true);
            SetCurrentValue(WaitInfoProperty, info);
            SetCurrentValue(ShowOverlayProperty, showOverlay);
        }

        protected virtual void HideWait()
        {
            SetCurrentValue(IsWaitingProperty, false);
            SetCurrentValue(WaitInfoProperty, "");
            SetCurrentValue(ShowOverlayProperty, false);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ControlStatus != null)
            {
                ControlStatus.SelectionChanged -= _controlStatus_SelectionChanged;
            }
            if (ControlCornerRadius != null)
            {
                ControlCornerRadius.ValueChanged -= _cornerRadius_ValueChanged;
            }
            if (ControlMinHeight != null)
            {
                ControlMinHeight.ValueChanged -= _minHeight_ValueChanged;
            }
            if (ControlIconWidth != null)
            {
                ControlIconWidth.ValueChanged -= _iconWidth_ValueChanged;
            }
            if (ControlFontFamily != null)
            {
                ControlFontFamily.SelectionChanged -= _fontFamily_SelectionChanged;
            }
            if (ControlFontSize != null)
            {
                ControlFontSize.ValueChanged -= _iconHeigt_ValueChanged;
            }

            _rootGrid = GetTemplateChild(PART_RootGrid) as Grid;
            ControlStatus = GetTemplateChild(PART_ControlStatus) as ComboBox;
            ControlCornerRadius = GetTemplateChild(PART_CornerRadius) as NumericUpDown;
            ControlMinHeight = GetTemplateChild(PART_MinHeight) as NumericUpDown;
            ControlIconWidth = GetTemplateChild(PART_IconWidth) as NumericUpDown;
            ControlFontFamily = GetTemplateChild(PART_FontFamily) as ComboBox;
            ControlFontSize = GetTemplateChild(PART_FontSize) as NumericUpDown;
            CodeToggleButton = GetTemplateChild(PART_ToggleCode) as ToggleButton;
            OptionsPanel = GetTemplateChild(PART_OptionsPanel) as OptionsPanel;

            if (ControlStatus != null)
            {
                ControlStatus.SelectionChanged += _controlStatus_SelectionChanged;
            }
            if (ControlCornerRadius != null)
            {
                ControlCornerRadius.ValueChanged += _cornerRadius_ValueChanged;
            }
            if (ControlMinHeight != null)
            {
                ControlMinHeight.ValueChanged += _minHeight_ValueChanged; ;
            }
            if (ControlIconWidth != null)
            {
                ControlIconWidth.ValueChanged += _iconWidth_ValueChanged; ;
            }
            if (ControlFontFamily != null)
            {
                ControlFontFamily.SelectedItem = SystemFontFamilies.FirstOrDefault(p => p.Source == "微软雅黑");
                ControlFontFamily.SelectionChanged += _fontFamily_SelectionChanged;
            }
            if (ControlFontSize != null)
            {
                ControlFontSize.ValueChanged += _iconHeigt_ValueChanged; ;
            }

        }

        List<FrameworkElement> _controls = new List<FrameworkElement>();

        private void AIStudioUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _controls = VisualHelper.GetPanelChildren(_rootGrid).OfType<FrameworkElement>().ToList();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);


        }


        private void _controlStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null)
                return;

            if (e.AddedItems.Count > 0)
            {
                foreach (var child in _controls)
                {
                    if (child.GetType().GetProperty("Status") != null)
                    {
                        child.GetType().GetProperty("Status").SetValue(child, e.AddedItems[0]);
                    }
                    else
                    {
                        child.SetValue(ControlAttach.StatusProperty, e.AddedItems[0]);
                    }
                }
            }
        }

        private void _cornerRadius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
                return;

            foreach (var child in _controls)
            {
                if (child.GetType().GetProperty("CornerRadius") != null)
                {
                    child.GetType().GetProperty("CornerRadius").SetValue(child, new CornerRadius((double)e.NewValue));
                }
                else
                {
                    child.SetValue(ControlAttach.CornerRadiusProperty, new CornerRadius((double)e.NewValue));
                }
            }
        }

        private void _minHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
                return;

            foreach (var child in _controls)
            {
                child.SetValue(FrameworkElement.MinHeightProperty, (double)e.NewValue);
            }
        }

        private void _iconWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
                return;

            foreach (var child in _controls)
            {
                child.SetValue(IconAttach.WidthProperty, (double)e.NewValue);
                child.SetValue(IconAttach.HeightProperty, (double)e.NewValue);
            }
        }

        private void _iconHeigt_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
                return;

            foreach (var child in _controls)
            {
                child.SetValue(Control.FontSizeProperty, (double)e.NewValue);
            }
        }

        private void _fontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null)
                return;

            if (e.AddedItems.Count > 0)
            {
                foreach (var child in _controls)
                {
                    child.SetValue(Control.FontFamilyProperty, (FontFamily)e.AddedItems[0]);
                }
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="args">The PropertyChangedEventArgs</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            //this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
