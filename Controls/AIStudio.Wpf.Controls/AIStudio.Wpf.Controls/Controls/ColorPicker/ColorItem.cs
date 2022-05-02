using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class ColorItem
    {
        public Color? Color
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }

        public ColorItem(Color? color, string name)
        {
            Color = color;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var ci = obj as ColorItem;
            if (ci == null)
                return false;
            return (ci.Color.Equals(Color) && ci.Name.Equals(Name));
        }

        public override int GetHashCode()
        {
            return this.Color.GetHashCode() ^ this.Name.GetHashCode();
        }
    }
}
