using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace AIStudio.Wpf.GridControls.Model
{
    public class FiltersCollection
    {
        private readonly ListDictionary dictionary = new ListDictionary();
        private readonly FilterPresenter parent;
        internal FiltersCollection(FilterPresenter parent)
        {
            this.parent = parent;
        }
        internal bool ContainsKey(Type filterKey)
        {
            return dictionary.Contains(filterKey);
        }
        internal Filter this[Type key]
        {
            get
            {
                return (Filter)dictionary[key];
            }
            set
            {
                var defer = parent.DeferRefresh();
                Filter filter;
                if (dictionary.Contains(key))
                {
                    filter = (Filter)dictionary[key];
                    filter.Detach(parent);
                }
                dictionary[key] = filter = value;
                filter.Attach(parent);
                defer.Dispose();
            }
        }
        internal void Remove(Type key)
        {
            if (dictionary.Contains(key))
            {
                var defer = parent.DeferRefresh();
                Filter filter = (Filter)dictionary[key];
                filter.Detach(parent);
                dictionary.Remove(key);
                defer.Dispose();
            }
        }
        internal void Remove(Filter filter)
        {
            Type key = null;
            IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Value == filter)
                {
                    key = (Type)enumerator.Key;
                    break;
                }
            }
            if (key != null)
                dictionary.Remove(key);
        }
        public IEnumerable<Filter> Filters
        {
            get
            {
                var enumerator = dictionary.Values.GetEnumerator();
                while (enumerator.MoveNext())
                    yield return (Filter)enumerator.Current;
            }
        }

        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }
    }
}
