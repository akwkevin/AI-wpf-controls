using System.Windows;
using System.Windows.Documents;

namespace AIStudio.Wpf.Controls
{
    public class EmojiInline : InlineUIContainer
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(EmojiInline),
            new PropertyMetadata(""));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            // FIXME: split this into two different code paths
            if (e.Property == TextProperty)
            {
                string url = null;
                if (EmojiDataFactory.DataIndex.Value != null)
                    EmojiDataFactory.DataIndex.Value.TryGetValue(Text, out url);
                if (!string.IsNullOrEmpty(url))
                {
                    url = "pack://application:,,,/AIStudio.Wpf.Controls;component" + url;
                    Child = new GifImage() { Source = url, Height = 18, Width = 18, VerticalAlignment = VerticalAlignment.Center };
                }
            }

        }


    }
}
