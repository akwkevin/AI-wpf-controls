using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AIStudio.Wpf.GridControls.Model
{
    /// <summary>
    /// Defines the contract for Property filter.
    /// </summary>
    public interface IPropertyFilter : IFilter
    {
        /// <summary>
        /// Gets the property info that use to get property value from item.
        /// </summary>
        /// <value>The property info.</value>
        ItemPropertyInfo PropertyInfo
        {
            get;
        }
    }
}
