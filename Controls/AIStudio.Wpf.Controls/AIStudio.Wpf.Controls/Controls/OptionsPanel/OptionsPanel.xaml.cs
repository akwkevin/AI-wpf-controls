using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// Represents a panel control that can be drag within its parent control's boundaries. 
    /// <para>This control is used in samples that provide options/controls for interacting with IG controls. </para>
    /// </summary>
    [TemplatePart(Name = PART_HeaderPanel, Type = typeof(Border))]
    [TemplatePart(Name = PART_ContentToggleButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = PART_ContentViewer, Type = typeof(ScrollViewer))]
    public class OptionsPanel : ItemsControl
    {
        private const string PART_HeaderPanel = "PART_HeaderPanel";
        private const string PART_ContentToggleButton = "PART_ContentToggleButton";
        private const string PART_ContentViewer = "PART_ContentViewer";

        static OptionsPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OptionsPanel), new FrameworkPropertyMetadata(typeof(OptionsPanel)));
        }

        public OptionsPanel()
        {
            this.Loaded += OnOptionsPanelLoaded;
        }

        Border _headerPanel;
        ScrollViewer _contentViewer;
        ToggleButton _contentToggleButton;
        private bool _recalculateMargins = true;

        #region Events Handlers
        void OnParentLayoutUpdated(object sender, System.EventArgs e)
        {
            CalculateMargins();
            if (tTranslate.X > _maxMarginLeft)
                tTranslate.X = _maxMarginLeft;
            if (tTranslate.Y > _maxMarginTop)
                tTranslate.Y = _maxMarginTop;
        }
        void OnOptionsPanelLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = this.Parent as FrameworkElement;
                parent.LayoutUpdated += OnParentLayoutUpdated;
                parent.SizeChanged += Parent_SizeChanged;

                this.UpdateLayout();
                this.LayoutUpdated += OnOptionsPanelLayoutUpdated;


                if (this.IsMovable)
                    SetTransform();

            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("OptionsPanel Loading Error: " + ex.Message);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_headerPanel != null)
            {
                _headerPanel.MouseMove -= OnWinHandleMouseMove;
                _headerPanel.MouseLeftButtonDown -= OnWinHandleMouseLeftButtonDown;
                _headerPanel.MouseLeftButtonUp -= OnWinHandleMouseLeftButtonUp;
                _headerPanel.MouseRightButtonDown -= OnWinHandleMouseRightButtonDown;
                _headerPanel.MouseRightButtonUp -= OnWinHandleMouseRightButtonUp;
                _headerPanel.MouseLeave -= OnWinHandleMouseLeave;
            }

            _headerPanel = GetTemplateChild(PART_HeaderPanel) as Border;
            _contentToggleButton = GetTemplateChild(PART_ContentToggleButton) as ToggleButton;
            _contentViewer = GetTemplateChild(PART_ContentViewer) as ScrollViewer;

            if (_headerPanel != null)
            {
                _headerPanel.MouseMove += OnWinHandleMouseMove;
                _headerPanel.MouseLeftButtonDown += OnWinHandleMouseLeftButtonDown;
                _headerPanel.MouseLeftButtonUp += OnWinHandleMouseLeftButtonUp;
                _headerPanel.MouseRightButtonDown += OnWinHandleMouseRightButtonDown;
                _headerPanel.MouseRightButtonUp += OnWinHandleMouseRightButtonUp;
                _headerPanel.MouseLeave += OnWinHandleMouseLeave;
            }
        }

        void OnOptionsPanelLayoutUpdated(object sender, System.EventArgs e)
        {
            if (_recalculateMargins)
            {
                CalculateMargins();
                Move(this.tTranslate.X, this.tTranslate.Y);
                _recalculateMargins = false;
            }
        }
        #endregion
        private void SetTransform()
        {
            if (this.Parent is FrameworkElement parent)
            {
                GeneralTransform objGeneralTransform = this.TransformToVisual(parent);
                Point point = objGeneralTransform.Transform(new Point(0, 0));
                tTranslate.X = point.X;
                tTranslate.Y = point.Y;
                this.Margin = new Thickness(0);
                this.VerticalAlignment = VerticalAlignment.Top;
                this.HorizontalAlignment = HorizontalAlignment.Left;
                this.RenderTransform = tTranslate;


            }
        }

        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateMargins();
            double x = ((e.NewSize.Width - this.ActualWidth) / (e.PreviousSize.Width - this.ActualWidth)) * this.tTranslate.X;
            double y = ((e.NewSize.Height - this.ActualHeight) / (e.PreviousSize.Height - this.ActualHeight)) * this.tTranslate.Y;
            Move(x, y);
        }

        #region Moving Logic
        TranslateTransform tTranslate = new TranslateTransform();
        Point _borderPosition;
        Point _currentPosition;
        double _maxMarginLeft;
        double _maxMarginTop;
        bool _dragOn = false;
        private void OnWinHandleMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            this.Opacity = 0.0;
        }
        private void OnWinHandleMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            this.Opacity = 1.0;
        }
        private void OnWinHandleMouseLeave(object sender, MouseEventArgs e)
        {
            this.Opacity = 1.0;
        }
        private void OnWinHandleMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                this.IsExpanded = !this.IsExpanded;
            }

            if (!this.IsMovable) return;

            var c = sender as FrameworkElement;
            _dragOn = true;
            this.Opacity *= 0.5;
            _borderPosition = e.GetPosition(sender as Border);

            CalculateMargins();

            if (c != null) c.CaptureMouse();
        }
        private void OnWinHandleMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_dragOn)
            {
                var c = sender as FrameworkElement;
                this.Opacity = 1;
                if (c != null) c.ReleaseMouseCapture();
                _dragOn = false;
            }
        }

        private void OnWinHandleMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragOn)
            {
                _currentPosition = e.GetPosition(sender as Border);
                var x = tTranslate.X + _currentPosition.X - _borderPosition.X;
                var y = tTranslate.Y + _currentPosition.Y - _borderPosition.Y;

                Move(x, y);
            }
        }

        private void CalculateMargins()
        {
            var parent = (this.Parent as FrameworkElement);
            if (parent != null)
            {
                //TODO: insideoffset by control's Margin
                _maxMarginLeft = parent.ActualWidth - this.ActualWidth;
                _maxMarginTop = parent.ActualHeight - this.ActualHeight;
            }
        }

        private void Move(double x, double y)
        {
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            if (x > _maxMarginLeft)
                x = _maxMarginLeft;
            if (y > _maxMarginTop)
                y = _maxMarginTop;
            tTranslate.X = x;
            tTranslate.Y = y;
        }
        #endregion

        private void ToggleExpandedState(bool isExpanded)
        {

        }

        #region Dependency Properties

        public static DependencyProperty HeaderTextProperty = DependencyProperty.Register(
         nameof(HeaderText), typeof(string), typeof(OptionsPanel), null);

        /// <summary>
        /// Gets or sets Header Text of the OptionsPanel
        /// </summary>
        public string HeaderText
        {
            get
            {
                return (string)GetValue(HeaderTextProperty);
            }
            set
            {
                SetValue(HeaderTextProperty, value);
            }
        }

        public static readonly DependencyProperty IsMovableProperty = DependencyProperty.Register(
         nameof(IsMovable), typeof(bool), typeof(OptionsPanel), new PropertyMetadata(true, (sender, e) => {
             (sender as OptionsPanel).OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsMovable)));
         }));

        /// <summary>
        /// Gets or sets whether the OptionsPanel can be movable within its parent control's boundaries.  
        /// </summary>
        public bool IsMovable
        {
            get
            {
                return (bool)GetValue(IsMovableProperty);
            }
            set
            {
                SetValue(IsMovableProperty, value);
            }
        }

        public static readonly DependencyProperty IsExpandableProperty = DependencyProperty.Register(
            nameof(IsExpandable), typeof(bool), typeof(OptionsPanel), new PropertyMetadata(true, (sender, e) => {
                (sender as OptionsPanel).OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsExpandable)));
            }));

        /// <summary>
        /// Gets or sets whether the OptionsPanel can be movable within its parent control's boundaries.  
        /// </summary>
        public bool IsExpandable
        {
            get
            {
                return (bool)GetValue(IsExpandableProperty);
            }
            set
            {
                SetValue(IsExpandableProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(OptionsPanel), new PropertyMetadata(true, (sender, e) => {
                (sender as OptionsPanel).OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsExpanded)));
            }));

        /// <summary>
        /// Gets or sets whether the OptionsPanel is expanded
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }
            set
            {
                SetValue(IsExpandedProperty, value);
            }
        }

        private void OnPropertyChanged(PropertyChangedEventArgs ea)
        {
            switch (ea.PropertyName)
            {
                case nameof(IsMovable):
                    {
                        if (this.IsMovable)
                            SetTransform();
                        break;
                    }
                case nameof(IsExpandable):
                    {
                        // behavior implemented using binding to this property in the generic style
                        break;
                    }
                case nameof(IsExpanded):
                    {
                        _recalculateMargins = true;
                        break;
                    }
            }
        }
        #endregion


    }
}
