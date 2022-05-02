using System.Collections.ObjectModel;

namespace AIStudio.Wpf.Controls
{
    public class RepeatCollection : Collection<object>
    {
        private int _offset;

        public object Next
        {
            get
            {
                if (this.Count == 0)
                    return null;

                var result = this[_offset];
                _offset++;
                if (_offset > this.Count - 1)
                    _offset = 0;

                return result;
            }
        }
    }
}
