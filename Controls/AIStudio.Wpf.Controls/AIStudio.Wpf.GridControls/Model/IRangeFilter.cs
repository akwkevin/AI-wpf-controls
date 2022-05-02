using System;
using System.Collections.Generic;
using System.Text;

namespace AIStudio.Wpf.GridControls.Model
{
    /// <summary>
    /// Defines the contract for a range filter.
    /// </summary>
    public interface IRangeFilter : IFilter
    {
        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        object CompareFrom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        object CompareTo
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Defines the contract for a range filter.
    /// </summary>
    /// <typeparam name="T">Comparable value Type.</typeparam>
    public interface IRangeFilter<T> : IRangeFilter//, INotifyPropertyChanged
        where T : struct, IComparable
    {
        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>From.</value>
        new T? CompareFrom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>To.</value>
        new T? CompareTo
        {
            get;
            set;
        }
    }
}
