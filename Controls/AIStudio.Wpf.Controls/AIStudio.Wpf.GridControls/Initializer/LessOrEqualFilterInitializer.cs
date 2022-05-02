using AIStudio.Wpf.GridControls.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace AIStudio.Wpf.GridControls.Initializer
{
    /// <summary>
    /// Define LessOrEqualFilter initializer.
    /// </summary>
    public class LessOrEqualFilterInitializer : PropertyFilterInitializer
    {
        private const string _filterName = "Less or Equal";
        /// <summary>
        /// Create LessOrEqualFilter for instance of FilterPresenter, if it is possible.
        /// </summary>
        /// <param name="filterPresenter">FilterPresenter, which can be attached Filter</param>
        /// <param name="key">ItemPropertyInfo for binding to property.</param>
        /// <returns>Instance of LessOrEqualFilter class or null</returns>
        protected override PropertyFilter NewFilter(FilterPresenter filterPresenter, ItemPropertyInfo propertyInfo)
        {
            Debug.Assert(filterPresenter != null);
            Debug.Assert(propertyInfo != null);

            //ItemPropertyInfo propertyInfo = (ItemPropertyInfo)key;
            Type propertyType = propertyInfo.PropertyType;
            if (filterPresenter.ItemProperties.Contains(propertyInfo)
                && typeof(IComparable).IsAssignableFrom(propertyInfo.PropertyType)
                && propertyInfo.PropertyType != typeof(String)
                && propertyInfo.PropertyType != typeof(bool)
                && !propertyType.IsEnum
                )
            {
                return (PropertyFilter)Activator.CreateInstance(typeof(LessOrEqualFilter<>).MakeGenericType(propertyInfo.PropertyType), propertyInfo);
            }
            return null;
        }
    }
}
