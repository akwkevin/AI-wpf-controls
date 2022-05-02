using AIStudio.Wpf.GridControls.View;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace AIStudio.Wpf.GridControls.Model
{
    /// <summary>
    /// Defines a string filter 
    /// </summary>
    [View(typeof(StringFilterView))]
    public class StringFilter : PropertyFilter, IStringFilter
    {

        Func<object, string> getter;
        private StringFilterMode _filterMode = StringFilterMode.Contains;
        private string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringFilter"/> class.
        /// </summary>
        /// <param name="getter">Func that return from item string value to compare.</param>
        protected StringFilter(Func<object, string> getter)
        {
            Debug.Assert(getter != null, "getter is null.");
            this.getter = getter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringFilter"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="filterMode">The filter mode.</param>
        public StringFilter(ItemPropertyInfo propertyInfo, StringFilterMode filterMode = StringFilterMode.Contains)
            : base()
        {
            //if (!typeof(string).IsAssignableFrom(propertyInfo.PropertyType))
            //    throw new ArgumentOutOfRangeException("propertyInfo", "typeof(string).IsAssignableFrom(propertyInfo.PropertyType) return False.");
            Debug.Assert(propertyInfo != null, "propertyInfo is null.");
            Debug.Assert(typeof(IComparable).IsAssignableFrom(propertyInfo.PropertyType), "The typeof(IComparable).IsAssignableFrom(propertyInfo.PropertyType) return False.");
            base.PropertyInfo = propertyInfo;
            _filterMode = filterMode;
            Func<object, object> getterItem = ((PropertyDescriptor)(PropertyInfo.Descriptor)).GetValue;
            this.getter = t => ((string)(getterItem(t)));
            base.Name = "String";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringFilter"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="filterMode">The filter mode.</param>
        /// <param name="value">The value.</param>
        public StringFilter(ItemPropertyInfo propertyInfo, StringFilterMode filterMode, string value)
            : this(propertyInfo, filterMode)
        {
            _value = value;
            base.IsActive = !String.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Gets or sets the string filter mode.
        /// </summary>
        /// <value>The mode.</value>
        public StringFilterMode Mode
        {
            get
            {
                return _filterMode;
            }
            set
            {
                if (_filterMode != value)
                {
                    _filterMode = value;
                    OnIsActiveChanged();
                    OnPropertyChanged("Mode");
                }
            }
        }

        /// <summary>
        /// Gets or sets the value to look for.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    base.IsActive = !String.IsNullOrEmpty(value);
                    OnIsActiveChanged();
                    OnPropertyChanged("Value");
                }
            }
        }
        /// <summary>
        /// Provide derived clases IsActiveChanged event.
        /// </summary>
        protected override void OnIsActiveChanged()
        {
            if (!IsActive)
                Value = null;
            base.OnIsActiveChanged();
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
                    string toCompare = getter(e.Item);
                    if (String.IsNullOrEmpty(toCompare))
                        e.Accepted = false;
                    else switch (_filterMode)
                        {
                            case StringFilterMode.Contains:
                                e.Accepted = toCompare.Contains(_value);
                                break;
                            case StringFilterMode.StartsWith:
                                e.Accepted = toCompare.StartsWith(_value);
                                break;
                            case StringFilterMode.EndsWith:
                                e.Accepted = toCompare.EndsWith(_value);
                                break;
                            default:
                                e.Accepted = toCompare.Equals(_value);
                                break;
                        }
                }
            }
        }
    }
}
