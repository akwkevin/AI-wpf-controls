using AIStudio.Wpf.GridControls.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AIStudio.Wpf.GridControls.Initializer
{
    /// <summary>
    /// Define ValueFilter initializer.
    /// </summary>
    public class ValueFilterInitializer : FilterInitializer
    {

        /// <summary>
        /// Generate new instance of Filter class, if it is possible for filterPresenter and key.
        /// </summary>
        /// <param name="filterPresenter">FilterPresenter, which can be attached Filter</param>
        /// <param name="key">Key for generated Filter. For PropertyFilter, key used as the name for binding property in filterPresenter.Parent collection.</param>
        /// <returns>Instance of Filter class or null.</returns>
        public override Model.Filter NewFilter(FilterPresenter filterPresenter, object key)
        {
            Debug.Assert(filterPresenter != null);
            Debug.Assert(key != null);
            EqualFilter<object> filter = new EqualFilter<object>(item => item, filterPresenter.CollectionView.SourceCollection);
            return filter;
        }
    }
}
