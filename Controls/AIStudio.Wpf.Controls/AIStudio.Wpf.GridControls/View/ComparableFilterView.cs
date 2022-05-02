using AIStudio.Wpf.GridControls.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AIStudio.Wpf.GridControls.View
{
    /// <summary>
    /// Defile View control for IComparableFilter model.
    /// </summary>
    [ModelView]
    [TemplatePart(Name = PART_Input, Type = typeof(TextBox))]
    public class ComparableFilterView : FilterViewBase<IComparableFilter>
    {
        const string PART_Input = "PART_Input";
        static ComparableFilterView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComparableFilterView),
                new FrameworkPropertyMetadata(typeof(ComparableFilterView)));
        }
        private TextBox _textBox;
        /// <summary>
        /// Create new instance of ComparableFilterView.
        /// </summary>
        public ComparableFilterView()
        {
        }
        /// <summary>
        /// Create new instance of ComparableFilterView and accept model.
        /// </summary>
        /// <param name="model">IComparableFilter model</param>
        public ComparableFilterView(object model)
            : this()
        {
            base.Model = model as IComparableFilter;
        }
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (_textBox != null)
            {
                _textBox.RemoveHandler(TextBox.KeyDownEvent, new KeyEventHandler(TextBox_KeyDown));
                _textBox.RemoveHandler(TextBox.LostFocusEvent, new RoutedEventHandler(TextBox_LostFocus));
            }
            base.OnApplyTemplate();
            _textBox = GetTemplateChild(PART_Input) as TextBox;
            if (_textBox != null)
            {
                _textBox.AddHandler(TextBox.KeyDownEvent, new KeyEventHandler(TextBox_KeyDown), true);
                _textBox.AddHandler(TextBox.LostFocusEvent, new RoutedEventHandler(TextBox_LostFocus), true);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateTextBoxSource((TextBox)sender);
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        UpdateTextBoxSource((TextBox)sender);
                        e.Handled = true;
                        return;
                    }
            }
        }
        private static void UpdateTextBoxSource(TextBox tb)
        {
            tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}
