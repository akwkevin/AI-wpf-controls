using AIStudio.Wpf.GridControls.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AIStudio.Wpf.GridControls.View
{
    /// <summary>
    /// Defile View control for IRangeFilter model.
    /// </summary>
    [ModelView]
    [TemplatePart(Name = PART_From, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_To, Type = typeof(TextBox))]
    public class DateTimeRangeFilterView : FilterViewBase<IRangeFilter>
    {
        const string PART_From = "PART_From";
        const string PART_To = "PART_To";
        static DateTimeRangeFilterView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimeRangeFilterView),
                new FrameworkPropertyMetadata(typeof(DateTimeRangeFilterView)));

        }
        private TextBox txtBoxFrom, textBoxTo;
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeRangeFilterView"/> class.
        /// </summary>
        public DateTimeRangeFilterView() : base()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeRangeFilterView"/> class and accept model.
        /// </summary>
        /// <param name="model">IRangeFilter model</param>
        public DateTimeRangeFilterView(object model)
        {
            base.Model = model as IRangeFilter;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (txtBoxFrom != null)
            {
                txtBoxFrom.RemoveHandler(TextBox.KeyDownEvent, new KeyEventHandler(TextBox_KeyDown));
                txtBoxFrom.RemoveHandler(TextBox.LostFocusEvent, new RoutedEventHandler(TextBox_LostFocus));
            }
            if (textBoxTo != null)
            {
                textBoxTo.RemoveHandler(TextBox.KeyDownEvent, new KeyEventHandler(TextBox_KeyDown));
                textBoxTo.RemoveHandler(TextBox.LostFocusEvent, new RoutedEventHandler(TextBox_LostFocus));
            }
            base.OnApplyTemplate();
            textBoxTo = GetTemplateChild(PART_To) as TextBox;
            txtBoxFrom = GetTemplateChild(PART_From) as TextBox;
            if (txtBoxFrom != null)
            {
                txtBoxFrom.AddHandler(TextBox.KeyDownEvent, new KeyEventHandler(TextBox_KeyDown), true);
                txtBoxFrom.AddHandler(TextBox.LostFocusEvent, new RoutedEventHandler(TextBox_LostFocus), true);
            }
            if (textBoxTo != null)
            {
                textBoxTo.AddHandler(TextBox.KeyDownEvent, new KeyEventHandler(TextBox_KeyDown), true);
                textBoxTo.AddHandler(TextBox.LostFocusEvent, new RoutedEventHandler(TextBox_LostFocus), true);
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
