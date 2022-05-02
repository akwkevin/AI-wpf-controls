using AIStudio.Wpf.GridControls.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AIStudio.Wpf.GridControls.Initializer
{
    /// <summary>
    /// Define GreaterOrEqualFilter Initializer.
    /// </summary>
    public class GreaterOrEqualFilterInitializer : LessOrEqualFilterInitializer
    {

        private const string _filterName = "Greater or Equal";
        /// <summary>
        /// Create PropertyFilter for instance of FilterPresenter, if it is possible.
        /// </summary>
        /// <param name="filterPresenter">filterPresenter for that filter will be created.</param>
        /// <param name="key">ItemPropertyInfo cpecified property that PropertyFilter will be use.</param>
        /// <returns>Instance of GreaterOrEqualFilter if:
        ///  filterPresenter.ItemProperties.Contains(propertyInfo)
        ///           && typeof(IComparable).IsAssignableFrom(propertyInfo.PropertyType)
        ///           && propertyInfo.PropertyType!=typeof(String)
        ///           && propertyInfo.PropertyType != typeof(bool)
        ///           && !propertyType.IsEnum
        ///  otherwise, null.</returns>
        protected override PropertyFilter NewFilter(FilterPresenter filterPresenter, ItemPropertyInfo key)
        {
            if (filterPresenter == null)
                return null;
            if (key == null)
                return null;
            ItemPropertyInfo propertyInfo = (ItemPropertyInfo)key;
            Type propertyType = propertyInfo.PropertyType;
            if (filterPresenter.ItemProperties.Contains(propertyInfo)
                && typeof(IComparable).IsAssignableFrom(propertyInfo.PropertyType)
                && propertyInfo.PropertyType != typeof(String)
                && propertyInfo.PropertyType != typeof(bool)
                && !propertyType.IsEnum
               )
            {
                return (PropertyFilter)Activator.CreateInstance(typeof(GreaterOrEqualFilter<>).MakeGenericType(propertyInfo.PropertyType), propertyInfo);
            }
            return null;
        }

    }
}
