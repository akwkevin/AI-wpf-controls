using AIStudio.Wpf.GridControls.Model;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace AIStudio.Wpf.GridControls.Initializer
{
    public class EqualFilterInitializer : PropertyFilterInitializer
    {
        private const string _filterName = "Equality";

        protected override PropertyFilter NewFilter(FilterPresenter filterPresenter, ItemPropertyInfo propertyInfo)
        {
            Debug.Assert(filterPresenter != null);
            Debug.Assert(propertyInfo != null);

            Type propertyType = propertyInfo.PropertyType;
            if (filterPresenter.ItemProperties.Contains(propertyInfo)
                && !propertyType.IsEnum
                )
            {
                PropertyFilter filter = (PropertyFilter)Activator.CreateInstance(
                    typeof(EqualFilter<>).MakeGenericType(propertyInfo.PropertyType),
                    propertyInfo,
                    GetAvailableValuesQuery(filterPresenter, propertyInfo)
                );
                return filter;
            }
            return null;
        }
        /// <summary>
        /// Returns a query that returns the unique item property values in the ItemsSource collection..
        /// </summary>
        public static IEnumerable GetAvailableValuesQuery(FilterPresenter filterPresenter, ItemPropertyInfo propInfo)
        {
            IEnumerable source = filterPresenter.CollectionView.SourceCollection;
            if (source == null)
                return new object[0];
            var propertyDescriptor = propInfo.Descriptor as PropertyDescriptor;
            var sourceQuery = source.OfType<object>().Select(item => propertyDescriptor.GetValue(item));
            var propType = propertyDescriptor.PropertyType;
            if (typeof(IComparable).IsAssignableFrom(propType))
            {
                sourceQuery = sourceQuery.OrderBy(item => item);
            }
            else
                sourceQuery = sourceQuery.OrderBy(item => item == null ? "" : item.ToString());
            sourceQuery = sourceQuery.Distinct();
            return sourceQuery;
        }
    }
}
