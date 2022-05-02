using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class EmojiTextBlock : TextBlock
    {
        static EmojiTextBlock()
        {
            TextProperty.OverrideMetadata(typeof(EmojiTextBlock), new FrameworkPropertyMetadata(
                (string)TextBlock.TextProperty.GetMetadata(typeof(TextBlock)).DefaultValue,
                (o, e) =>
                (o as EmojiTextBlock).OnTextChanged(e.NewValue as string)
                ));

            ForegroundProperty.OverrideMetadata(typeof(EmojiTextBlock), new FrameworkPropertyMetadata(
                (Brush)ForegroundProperty.GetMetadata(typeof(TextBlock)).DefaultValue,
                (o, e) => (o as EmojiTextBlock).OnForegroundChanged(e.NewValue as Brush)));

            FontSizeProperty.OverrideMetadata(typeof(EmojiTextBlock), new FrameworkPropertyMetadata(
                (double)FontSizeProperty.GetMetadata(typeof(TextBlock)).DefaultValue,
                (o, e) => (o as EmojiTextBlock).OnFontSizeChanged((double)e.NewValue)));
        }

        private void OnTextChanged(string text)
        {
            text = text.Replace("\r\n", "\n");
            Inlines.Clear();
            if (string.IsNullOrEmpty(text))
                return;

            int pos = 0;
            foreach (Match m in Regex.Matches(text, EmojiDataFactory.DataPattern))
            {
                Inlines.Add(text.Substring(pos, m.Index - pos));
                Inlines.Add(new EmojiInline()
                {
                    Foreground = Foreground,
                    Text = text.Substring(m.Index, m.Length),
                    FontSize = FontSize,
                });

                pos = m.Index + m.Length;
            }
            Inlines.Add(text.Substring(pos));
        }

        private void OnForegroundChanged(Brush brush)
        {
            foreach (var inline in Inlines)
                if (inline is EmojiInline)
                    (inline as EmojiInline).Foreground = brush;
        }

        private void OnFontSizeChanged(double size)
        {
            foreach (var inline in Inlines)
                if (inline is EmojiInline)
                    (inline as EmojiInline).FontSize = size;
        }


        //TextPointer startpoz;
        //TextPointer endpoz;
        //MenuItem copyMenu;
        //MenuItem selectAllMenu;

        //public TextRange Selection { get; private set; }
        //public bool HasSelection
        //{
        //    get { return Selection != null && !Selection.IsEmpty; }
        //}

        //#region SelectionBrush

        //public static readonly DependencyProperty SelectionBrushProperty =
        //    DependencyProperty.Register("SelectionBrush", typeof(Brush), typeof(EmojiTextBlock),
        //        new FrameworkPropertyMetadata(Brushes.Orange));

        //public Brush SelectionBrush
        //{
        //    get { return (Brush)GetValue(SelectionBrushProperty); }
        //    set { SetValue(SelectionBrushProperty, value); }
        //}

        //#endregion

        //public EmojiTextBlock()
        //{
        //    Focusable = true;
        //}

        //protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        //{
        //    Keyboard.Focus(this);
        //    ReleaseMouseCapture();
        //    base.OnMouseLeftButtonUp(e);
        //}

        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    var point = e.GetPosition(this);
        //    startpoz = GetPositionFromPoint(point, true);
        //    CaptureMouse();
        //    base.OnMouseLeftButtonDown(e);
        //}

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    if (IsMouseCaptured)
        //    {
        //        var point = e.GetPosition(this);
        //        endpoz = GetPositionFromPoint(point, true);

        //        ClearSelection();
        //        Selection = new TextRange(startpoz, endpoz);
        //        Selection.ApplyPropertyValue(TextElement.BackgroundProperty, SelectionBrush);
        //        CommandManager.InvalidateRequerySuggested();

        //        OnSelectionChanged(EventArgs.Empty);
        //    }

        //    base.OnMouseMove(e);
        //}

        //protected override void OnKeyUp(KeyEventArgs e)
        //{
        //    if (Keyboard.Modifiers == ModifierKeys.Control)
        //    {
        //        if (e.Key == Key.C)
        //            Copy();
        //        else if (e.Key == Key.A)
        //            SelectAll();
        //    }

        //    base.OnKeyUp(e);
        //}

        //protected override void OnLostFocus(RoutedEventArgs e)
        //{
        //    ClearSelection();
        //    //base.OnLostFocus(e);
        //}

        //public bool Copy()
        //{
        //    if (HasSelection)
        //    {
        //        Clipboard.SetDataObject(Selection.Text);
        //        return true;
        //    }
        //    return false;
        //}

        //public void ClearSelection()
        //{

        //    var contentRange = new TextRange(ContentStart, ContentEnd);
        //    if (contentRange != null)
        //    {
        //        contentRange.ApplyPropertyValue(TextElement.BackgroundProperty, null);
        //        Selection = null;
        //    }
        //}

        //public void SelectAll()
        //{
        //    Selection = new TextRange(ContentStart, ContentEnd);
        //    Selection.ApplyPropertyValue(TextElement.BackgroundProperty, SelectionBrush);
        //}

        //public event EventHandler SelectionChanged;

        //protected virtual void OnSelectionChanged(EventArgs e)
        //{
        //    var handler = this.SelectionChanged;
        //    if (handler != null)
        //        handler(this, e);
        //}


    }
}
