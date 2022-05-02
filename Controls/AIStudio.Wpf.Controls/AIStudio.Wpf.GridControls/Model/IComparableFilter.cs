using System;

namespace AIStudio.Wpf.GridControls.Model
{
    /// <summary>
    /// Define the contract for compare property filter.
    /// </summary>
    public interface IComparableFilter : IPropertyFilter
    {
        /// <summary>
        /// Gets or sets the value used in the comparison.
        /// </summary>
        /// <value>The compare to.</value>
        object CompareTo
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Defines the contract for a compare filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IComparableFilter<T> : IComparableFilter
        where T : struct, IComparable
    {
        /// <summary>
        /// Gets or sets the value used in the comparison.
        /// </summary>
        /// <value>The compare to.</value>
        new Nullable<T> CompareTo
        {
            get;
            set;
        }
    }
}
