namespace AIStudio.Wpf.Controls
{
    public class NumericUpDown : DoubleUpDown
    {
        static NumericUpDown()
        {
            UpdateMetadata(typeof(NumericUpDown), 1d, double.NegativeInfinity, double.PositiveInfinity);
        }

        public NumericUpDown() : base()
        {
        }
    }

    public enum AutoSelectBehavior
    {
        Never,
        OnFocus
    }
}
