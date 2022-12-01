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
    [TemplatePart(Name = PART_ToggleCode, Type = typeof(ToggleButton))]
    [TemplatePart(Name = PART_OptionsPanel, Type = typeof(OptionsPanel))]
    public class AIStudioUserControl : UserControl, INotifyPropertyChanged
    {
        private const string PART_RootGrid = "PART_RootGrid";
        private const string PART_ControlStatus = "PART_ControlStatus";       
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
        }

        public AIStudioUserControl()
        {
            this.CancelCommmad = new RoutedUICommand();

            this.BindCommand(CancelCommmad, this.Cancel);
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

            _rootGrid = GetTemplateChild(PART_RootGrid) as Grid;
            ControlStatus = GetTemplateChild(PART_ControlStatus) as ComboBox;
            CodeToggleButton = GetTemplateChild(PART_ToggleCode) as ToggleButton; 
            OptionsPanel = GetTemplateChild(PART_OptionsPanel) as OptionsPanel;

            if (ControlStatus != null)
            {
                ControlStatus.SelectionChanged += _controlStatus_SelectionChanged;
            }      

        }

        List<FrameworkElement> _controls;

        private void _controlStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_controls == null)
            {
                _controls = GetPanelChildren(_rootGrid).OfType<FrameworkElement>().ToList();
            }

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


        public static List<UIElement> GetPanelChildren(Panel panel)
        {
            List<UIElement> elements = new List<UIElement>();
            if (panel != null)
            {

                foreach (var child in panel.Children)
                {
                    if (child is ContentPresenter contentPresenter)
                    {
                        if (contentPresenter.Content is Panel contentPresenterPanel)
                        {
                            elements.AddRange(GetPanelChildren(contentPresenterPanel));
                        }
                        else
                        {
                            elements.AddRange(GetPanelChildren(VisualHelper.FindVisualChild<Panel>(contentPresenter.Content as DependencyObject)));
                        }
                    }
                    else if (child is ScrollViewer scroll)
                    {
                        if (scroll.Content is Panel scrollPanel)
                        {
                            elements.AddRange(GetPanelChildren(scrollPanel));
                        }
                        else
                        {
                            elements.AddRange(GetPanelChildren(VisualHelper.FindVisualChild<Panel>(scroll.Content as DependencyObject)));
                        }
                    }
                    else if (child is Panel chiledpanel)
                    {
                        elements.AddRange(GetPanelChildren(chiledpanel));
                    }
                    else if (child is GroupBox groupBox)
                    {
                        if (groupBox.Content is Panel contentpanel)
                        {
                            elements.AddRange(GetPanelChildren(contentpanel));
                        }
                        else if (groupBox.Content is TextBlock)
                        {
                            elements.Add(groupBox);
                        }
                        else
                        {
                            elements.Add(groupBox.Content as UIElement);
                        }
                    }
                    else if (child is ButtonGroup buttonGroup)
                    {
                        foreach (var item in buttonGroup.Items)
                        {
                            elements.Add(buttonGroup.ItemContainerGenerator.ContainerFromItem(item) as UIElement);
                        }
                    }
                    else if (child is TabControl tabControl)
                    {
                        var tabitems = tabControl.FindChildren<TabItem>();
                        foreach (var tabitem in tabitems)
                        {
                            if (tabitem.Content is Panel tabitemPanel)
                            {
                                elements.AddRange(GetPanelChildren(tabitemPanel));
                            }
                            else if (tabitem.Content is DependencyObject)
                            {
                                elements.AddRange(GetPanelChildren(VisualHelper.FindVisualChild<Panel>(tabitem.Content as DependencyObject)));
                            }
                        }
                    }
                    else if (child is ToolBarTray toolBarTray)
                    {
                        foreach (var childtoolbar in toolBarTray.ToolBars)
                        {
                            foreach (var item in childtoolbar.Items.OfType<UIElement>())
                            {
                                elements.Add(item);
                            }
                        }
                    }
                    else if (child is ToolBar toolbar)
                    {
                        foreach (var item in toolbar.Items.OfType<UIElement>())
                        {
                            elements.Add(item);
                        }
                    }
                    else if (child is UIElement element)
                    {
                        elements.Add(element);
                    }
                }
            }
            return elements;
        }

    }
}
