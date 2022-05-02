using AIStudio.Wpf.GridControls.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace AIStudio.Wpf.GridControls.Initializer
{
    /// <summary>
    /// Define RangeFilter initializer.
    /// </summary>
    public class RangeFilterInitializer : PropertyFilterInitializer
    {
        private const string _filterName = "Between";
        #region IPropertyFilterInitializer Members

        protected override PropertyFilter NewFilter(FilterPresenter filterPresenter, ItemPropertyInfo propertyInfo)
        {
            Debug.Assert(filterPresenter != null);
            Debug.Assert(propertyInfo != null);

            Type propertyType = propertyInfo.PropertyType;
            if (filterPresenter.ItemProperties.Contains(propertyInfo)
                && typeof(IComparable).IsAssignableFrom(propertyType)
                && propertyType != typeof(String)
                && propertyType != typeof(bool)
                && !propertyType.IsEnum
                )
            {
                return (PropertyFilter)Activator.CreateInstance(typeof(RangeFilter<>).MakeGenericType(propertyInfo.PropertyType), propertyInfo);
            }
            return null;
        }

        #endregion
    }
}
