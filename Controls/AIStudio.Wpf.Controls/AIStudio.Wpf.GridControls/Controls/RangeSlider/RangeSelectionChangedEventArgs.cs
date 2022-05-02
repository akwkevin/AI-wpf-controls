using System.Windows;

namespace AIStudio.Wpf.GridControls
{
    /// <summary>
    /// Event arguments created for the Range_Slider's SelectionChanged event.
    /// <see cref="Range_Slider"/>
    /// </summary>
    public class RangeSelectionChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// The value of the new range's beginning.
        /// </summary>
        public double NewLowerValue { get; set; }
        /// <summary>
        /// The value of the new range's ending.
        /// </summary>
        public double NewUpperValue { get; set; }

        public double OldLowerValue { get; set; }

        public double OldUpperValue { get; set; }

        internal RangeSelectionChangedEventArgs(double newLowerValue, double newUpperValue, double oldLowerValue, double oldUpperValue)
        {
            NewLowerValue = newLowerValue;
            NewUpperValue = newUpperValue;
            OldLowerValue = oldLowerValue;
            OldUpperValue = oldUpperValue;
        }

        //internal RangeSelectionChangedEventArgs(Range_Slider slider)
        //    : this(slider.LowerValue, slider.UpperValue)
        //{ }
    }
}