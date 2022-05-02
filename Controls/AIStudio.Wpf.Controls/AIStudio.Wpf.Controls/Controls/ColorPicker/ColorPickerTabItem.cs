using System.Windows.Input;

namespace AIStudio.Wpf.Controls
{
    public class ColorPickerTabItem : TabItemEx
    {
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.Source == this || !this.IsSelected)
                return;

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            //Selection on Mouse Up
            if (e.Source == this || !this.IsSelected)
            {
                base.OnMouseLeftButtonDown(e);
            }

            base.OnMouseLeftButtonUp(e);
        }
    }
}
