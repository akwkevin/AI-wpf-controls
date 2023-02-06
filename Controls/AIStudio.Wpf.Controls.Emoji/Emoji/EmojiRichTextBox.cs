using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace AIStudio.Wpf.Controls.Emoji
{
    public class EmojiRichTextBox : RichTextBox
    {
        public EmojiRichTextBox()
        {
            SetValue(Block.LineHeightProperty, 1.0);
            DataObject.AddCopyingHandler(this, new DataObjectCopyingEventHandler(OnCopy));
        }

        protected void OnCopy(object o, DataObjectCopyingEventArgs e)
        {
            string clipboard = "";

            for (TextPointer p = Selection.Start, next = null;
                 p != null && p.CompareTo(Selection.End) < 0;
                 p = next)
            {
                next = p.GetNextInsertionPosition(LogicalDirection.Forward);
                if (next == null)
                    break;

                var emoji = (next.Parent as Run)?.PreviousInline as EmojiInline;
                if (emoji == null && next.Parent != p.Parent)
                    emoji = (p.Parent as Run)?.NextInline as EmojiInline;
                if (emoji != null && (p.Parent as Run).PreviousInline != emoji)
                    clipboard += emoji?.Text;
                else
                    clipboard += new TextRange(p, next).Text;
            }

            Clipboard.SetText(clipboard);
            e.Handled = true;
            e.CancelCommand();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (m_pending_change)
                return;

            m_pending_change = true;

            base.OnTextChanged(e);

            /* This will prevent our operation from polluting the undo buffer, but it
             * will create an infinite undo stack... need to fix this. */
            BeginChange();

            TextPointer cur = Document.ContentStart;
            while (cur.CompareTo(Document.ContentEnd) < 0)
            {
                TextPointer next = cur.GetNextInsertionPosition(LogicalDirection.Forward);
                if (next == null)
                    break;

                TextPointer tempnext = next;

                while (tempnext.CompareTo(Document.ContentEnd) < 0)
                {
                    TextRange tempword = new TextRange(cur, tempnext);
                    if (EmojiDataFactory.DataIndex.Value.Keys.Any(p => p.StartsWith(tempword.Text)))
                    {
                        if (EmojiDataFactory.DataIndex.Value.ContainsKey(tempword.Text))
                        {
                            next = tempnext;
                            break;
                        }
                        tempnext = tempnext.GetNextInsertionPosition(LogicalDirection.Forward);
                        if (tempnext == null)
                            break;
                    }
                    else
                    {
                        break;
                    }
                }

                TextRange word = new TextRange(cur, next);
                if (EmojiDataFactory.DataIndex.Value.ContainsKey(word.Text))
                {
                    Inline inline = new EmojiInline()
                    {
                        Text = word.Text,
                        FontSize = FontSize,
                        Foreground = Foreground,
                    };

                    // Preserve caret position
                    bool caret_was_next = (0 == next.CompareTo(CaretPosition));
                    next = Replace(word, inline);
                    if (caret_was_next)
                        CaretPosition = next;
                }

                cur = next;
            }

            EndChange();

            m_pending_change = false;

            // FIXME: debug
            //Console.WriteLine(XamlWriter.Save(Document));
        }

        private bool m_pending_change = false;

        public TextPointer Replace(TextRange range, Inline inline)
        {
            var run = range.Start.Parent as Run;
            if (run == null)
                return range.End;

            var before = new TextRange(run.ContentStart, range.Start).Text;
            var after = new TextRange(range.End, run.ContentEnd).Text;
            var inlines = run.SiblingInlines;

            /* Insert new inlines in reverse order after the run */
            if (!string.IsNullOrEmpty(after))
                inlines.InsertAfter(run, new Run(after));

            inlines.InsertAfter(run, inline);

            if (!string.IsNullOrEmpty(before))
                inlines.InsertAfter(run, new Run(before));

            TextPointer ret = inline.ContentEnd; // FIXME
            inlines.Remove(run);
            return ret;
        }
    }
}
