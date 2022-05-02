using AIStudio.Wpf.GridControls.View;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace AIStudio.Wpf.GridControls.Model
{
    /// <summary>
    /// Define the logic for Enum values filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [View(typeof(EnumFilterView))]
    public class EnumFilter<T> : EqualFilter<T>, IMultiValueFilter
    {

        Array allValues;
        /// <summary>
        /// Create new instance of EnumFilter.
        /// </summary>
        /// <param name="propertyInfo">propertyInfo, used to access a property of the collection item</param>
        public EnumFilter(ItemPropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            Debug.Assert(propertyInfo.PropertyType == typeof(T), "Invalid property type, the return type is not matching the class generic type.");
        }
        /// <summary>
        /// TryGet list of defined enum values.
        /// Throw <exception cref="NotImplementedException"/> if attempt to set value.
        /// </summary>
        public override IEnumerable AvailableValues
        {
            get
            {
                if (allValues == null)
                {
                    Type enumType = typeof(T);
                    if (enumType.IsEnum)
                    {
                        allValues = enumType.GetEnumValues();
                    }
                    else
                        allValues = new string[0];
                }
                return allValues;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public override void IsMatch(FilterPresenter sender, FilterEventArgs e)
        {
            if (e.Accepted)
            {
                if (e.Item == null)
                    e.Accepted = false;
                else
                {
                    object value = base.getter(e.Item);
                    e.Accepted = base.SelectedValues.Contains(value);
                }
            }
        }
    }
}
