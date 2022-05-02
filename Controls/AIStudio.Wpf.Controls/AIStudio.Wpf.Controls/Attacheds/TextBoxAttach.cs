using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace AIStudio.Wpf.Controls
{
    public static class TextBoxAttach
    {
        public static readonly DependencyProperty RegexStringProperty = DependencyProperty.RegisterAttached(
             "RegexString", typeof(string), typeof(TextBoxAttach), new PropertyMetadata(null, OnRegexStringChanged));

        public static void SetRegexString(DependencyObject element, string value)
            => element.SetValue(RegexStringProperty, value);

        public static string GetRegexString(DependencyObject element)
            => (string)element.GetValue(RegexStringProperty);

        private static void OnRegexStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                InputMethod.SetIsInputMethodEnabled(textBox, false);

                textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                textBox.PreviewTextInput += TextBox_PreviewTextInput;
            }
        }

        private static void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex re = new Regex(GetRegexString(sender as TextBox));
            e.Handled = re.IsMatch(e.Text);
        }

        public static readonly DependencyProperty EnterUpdateSourceProperty = DependencyProperty.RegisterAttached(
            "EnterUpdateSource", typeof(bool), typeof(TextBoxAttach), new PropertyMetadata(false, OnEnterUpdateSourceChanged));

        public static void SetEnterUpdateSource(DependencyObject element, bool value)
            => element.SetValue(EnterUpdateSourceProperty, value);

        public static bool GetEnterUpdateSource(DependencyObject element)
            => (bool)element.GetValue(EnterUpdateSourceProperty);

        private static void OnEnterUpdateSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
                textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            }
        }

        private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var text = sender as TextBox;
            if (e.Key == Key.Return)
            {
                BindingExpression binding = text.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }
        }
    }


}
