using AIStudio.Wpf.GridControls.Initializer;
using AIStudio.Wpf.GridControls.View;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace AIStudio.Wpf.GridControls.Model
{
    [View(typeof(DateTimeRangeFilterView))]
    public class DateTimeRangeFilter : RangeFilter<DateTime>
    {
        Func<object, DateTime> getter;

        public DateTimeRangeFilter(ItemPropertyInfo propertyInfo, DateTime beginDate, DateTime endDate)
            : base(propertyInfo)
        {
            Debug.Assert(propertyInfo != null, "propertyInfo is null.");
            Debug.Assert(typeof(IComparable).IsAssignableFrom(propertyInfo.PropertyType), "The typeof(IComparable).IsAssignableFrom(propertyInfo.PropertyType) return False.");
            base.PropertyInfo = propertyInfo;
            minDateTime = beginDate;
            maxDateTime = endDate;
            CompareFrom = beginDate;
            CompareTo = endDate;
            Func<object, object> getterItem = ((PropertyDescriptor)(PropertyInfo.Descriptor)).GetValue;
            getter = t => ((DateTime)getterItem(t));
            base.Name = "In range:";
        }

        public override void IsMatch(FilterPresenter sender, FilterEventArgs e)
        {
            if (e.Accepted)
            {
                if (e.Item == null)
                    e.Accepted = false;
                else
                {
                    DateTime value = getter(e.Item);
                    e.Accepted = (Object.ReferenceEquals(CompareFrom, null) | value.CompareTo(CompareFrom) >= 0)
                        && (Object.ReferenceEquals(CompareTo, null) | value.CompareTo(CompareTo) <= 0);
                }
            }
        }

        private void RefreshIsActive()
        {
            base.IsActive = !(Object.ReferenceEquals(CompareFrom, null) && Object.ReferenceEquals(CompareTo, null));

        }

        private DateTime maxDateTime;
        public DateTime MaxDateTime
        {
            get
            {
                return maxDateTime;
            }
            set
            {
                if (maxDateTime != value)
                {
                    maxDateTime = value;
                    OnPropertyChanged("MaxDateTime");
                }
            }
        }


        private DateTime minDateTime;
        public DateTime MinDateTime
        {
            get
            {
                return minDateTime;
            }
            set
            {
                if (minDateTime != value)
                {
                    minDateTime = value;
                    OnPropertyChanged("MinDateTime");
                }
            }
        }
    }

    public class DateTimeRangeFilterInitializer : PropertyFilterInitializer
    {
        private const string _filterName = "Between";
        #region IPropertyFilterInitializer Members

        protected override PropertyFilter NewFilter(FilterPresenter filterPresenter, ItemPropertyInfo propertyInfo)
        {
            Debug.Assert(filterPresenter != null);
            Debug.Assert(propertyInfo != null);

            Type propertyType = propertyInfo.PropertyType;
            if (filterPresenter.ItemProperties.Contains(propertyInfo)
                && typeof(DateTime).IsAssignableFrom(propertyType)
                && !propertyType.IsEnum
                )
            {
                IEnumerable source = filterPresenter.CollectionView.SourceCollection;
                if (source != null)
                {
                    var start = source.OfType<object>().Select(p => GetPropertyValue(p, propertyInfo.Name)).OfType<DateTime>().Min();
                    var end = source.OfType<object>().Select(p => GetPropertyValue(p, propertyInfo.Name)).OfType<DateTime>().Max();
                    return new DateTimeRangeFilter(propertyInfo, start, end);
                }

                return new DateTimeRangeFilter(propertyInfo, DateTime.Now.Subtract(TimeSpan.FromHours(12)), DateTime.Now);
            }
            return null;
        }


        /// <summary>
        /// 获取某属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public object GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj);
        }

        /// <summary>
        /// 设置某属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static void SetPropertyValue(object obj, string propertyName, object value)
        {
            obj.GetType().GetProperty(propertyName).SetValue(obj, value);
        }

        /// <summary>
        /// 是否拥有某字段
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static bool ContainsField(object obj, string fieldName)
        {
            return obj.GetType().GetField(fieldName) != null;
        }

        /// <summary>
        /// 获取某字段值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static object GetGetFieldValue(object obj, string fieldName)
        {
            return obj.GetType().GetField(fieldName).GetValue(obj);
        }

        /// <summary>
        /// 设置某字段值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static void SetFieldValue(object obj, string fieldName, object value)
        {
            obj.GetType().GetField(fieldName).SetValue(obj, value);
        }

        #endregion
    }
}
