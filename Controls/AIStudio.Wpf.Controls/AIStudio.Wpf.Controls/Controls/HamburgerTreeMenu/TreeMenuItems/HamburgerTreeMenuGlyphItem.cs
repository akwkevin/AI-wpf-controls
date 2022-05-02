namespace AIStudio.Wpf.Controls
{
    public class HamburgerTreeMenuGlyphItem : HamburgerTreeMenuItem
    {
        private string glyph;
        public string Glyph
        {
            get
            {
                return glyph;
            }
            set
            {
                if (value == glyph) return;
                glyph = value;
                OnPropertyChanged("Glyph");
            }
        }
    }
}
