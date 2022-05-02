using AIStudio.Wpf.GridControls.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace AIStudio.Wpf.GridControls.Initializer
{
    /// <summary>
    /// Represent initializer for StringFilter.
    /// </summary>
    public class StringFilterInitializer : PropertyFilterInitializer
    {
        /// <summary>
        /// Create new instance of StringFilter, if it is possible for filterPresenter in current state and key.
        /// </summary>
        protected override PropertyFilter NewFilter(FilterPresenter filterPresenter, ItemPropertyInfo propertyInfo)
        {
            Debug.Assert(filterPresenter != null);
            Debug.Assert(propertyInfo != null);
            Type propertyType = propertyInfo.PropertyType;
            if (filterPresenter.ItemProperties.Contains(propertyInfo)
                && typeof(String).IsAssignableFrom(propertyInfo.PropertyType)
                && !propertyType.IsEnum
                )
            {
                return new StringFilter(propertyInfo);
            }
            return null;
        }
    }
}
