using System.ComponentModel;

namespace AIStudio.Wpf.GridControls.Model
{
    /// <summary>
    /// Base class for filter that use property of item.
    /// </summary>
    public abstract class PropertyFilter : Filter, IPropertyFilter
    {
        private ItemPropertyInfo _propertyInfo;

        /// <summary>
        /// Gets the property info whose property name is filtered.
        /// </summary>
        /// <value>The property info.</value>
        public ItemPropertyInfo PropertyInfo
        {
            get { return _propertyInfo; }
            protected set
            {
                _propertyInfo = value;
            }
        }
    }
}
