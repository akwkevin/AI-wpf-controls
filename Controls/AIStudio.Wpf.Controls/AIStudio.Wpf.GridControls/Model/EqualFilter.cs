using AIStudio.Wpf.GridControls.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace AIStudio.Wpf.GridControls.Model
{
    /// <summary>
    /// Base class for filter that using list of values.
    /// </summary>
    [View(typeof(MultiValueFilterView))]
    public abstract class EqualFilter : PropertyFilter, IMultiValueFilter, IFilter
    {
        private readonly ObservableCollection<object> selectedValues;
        private readonly ReadOnlyObservableCollection<object> _selectedValues;
        /// <summary>
        /// Initialize new instance of EqualFilter from deriver class.
        /// </summary>
        protected EqualFilter()
        {
            selectedValues = new ObservableCollection<object>();
            _selectedValues = new ReadOnlyObservableCollection<object>(selectedValues);
            base.Name = "Equal:";
        }

        public ReadOnlyObservableCollection<object> SelectedValues
        {
            get { return _selectedValues; }
        }


        public abstract IEnumerable AvailableValues
        {
            get;
            set;
        }

        protected override void OnIsActiveChanged()
        {
            if (!IsActive)
                selectedValues.Clear();
            base.OnIsActiveChanged();
        }
        public void SelectedValuesChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                foreach (var item in e.AddedItems)
                {
                    selectedValues.Add(item);
                }
                IsActive = true;
            }
            if (e.RemovedItems != null)
            {
                foreach (var item in e.RemovedItems)
                {
                    selectedValues.Remove(item);
                }
                IsActive = selectedValues.Count > 0;
            }
            OnIsActiveChanged();
        }
    }

    /// <summary>
    /// Defines the logic for equality filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EqualFilter<T> : EqualFilter, IMultiValueFilter
    {
        IEnumerable availableValues;
        protected readonly Func<object, object> getter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualFilter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="getter">Func that return from item values to compare.</param>
        protected EqualFilter(Func<object, object> getter)
        {
            Debug.Assert(getter != null, "getter is null.");
            this.getter = getter;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualFilter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="getter">Func that return values to compare from item.</param>
        /// <param name="availableValues">Predefined set of available values.</param>
        protected internal EqualFilter(Func<object, object> getter, IEnumerable availableValues)
            : this(getter)
        {
            this.availableValues = availableValues;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualFilter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        public EqualFilter(ItemPropertyInfo propertyInfo)
            : base()
        {
            Debug.Assert(propertyInfo != null, "propertyInfo is null.");
            Debug.Assert(propertyInfo.PropertyType == typeof(T), "Invalid propertyInfo.PropertyType, the return type is not matching the class generic type.");
            base.PropertyInfo = propertyInfo;
            getter = ((PropertyDescriptor)(PropertyInfo.Descriptor)).GetValue;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualFilter&lt;T&gt;"/> class
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="availableValues">Predefined set of available values.</param>
        public EqualFilter(ItemPropertyInfo propertyInfo, IEnumerable availableValues)
            : this(propertyInfo)
        {
            this.availableValues = availableValues;
        }
        /// <summary>
        /// Set of available values that can be include in filter.
        /// </summary>
        public override IEnumerable AvailableValues
        {
            get
            {
                return availableValues;
            }
            set
            {
                if (availableValues != value)
                {
                    availableValues = value;
                }
            }
        }
        /// <summary>
        /// Determines whether the specified target is a match.
        /// </summary>
        public override void IsMatch(FilterPresenter sender, FilterEventArgs e)
        {
            if (e.Accepted)
            {
                if (e.Item == null)
                    e.Accepted = false;
                else
                {
                    object value = getter(e.Item);
                    e.Accepted = SelectedValues.Contains(value);
                }
            }
        }
    }
}
