using AIStudio.Wpf.GridControls.Initializer;
using AIStudio.Wpf.GridControls.Model;
using AIStudio.Wpf.GridControls.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace AIStudio.Wpf.GridControls
{
    // <summary>
    // FilterPresenter performs the role of a manager that manages the instantiation of filters and their connection to the CollectionView.
    // </summary>
    public sealed class FilterPresenter : DependencyObject
    {
        private static Dictionary<ICollectionView, WeakReference> filterPresenters = new Dictionary<ICollectionView, WeakReference>();
        private ReadOnlyCollection<ItemPropertyInfo> itemProperties;
        private int itemsDeferRefreshCount = 0;
        private IDisposable itemsDeferRefresh = null;
        private Predicate<object> filterFunction;
        private bool isFilterActive;
        private readonly ICollectionView collectionView;
        private readonly Dictionary<string, FiltersCollection> filters;
        private event FilterEventHandler _Filter;
        private readonly FilteredEventArgs filteredEventArgs;
        // <summary>
        // Returns FilterPresenter, connected to a pass source .
        // If pass instance of ICollectionView, FilterPresenter connected to passed instance, otherwise, filterPresenter connected to default view for passed collection.
        // </summary>
        // <param name="source">ICollectionView for source or source</param>
        // <returns>FilterPresenter, connected to source, or null if source is null.</returns>
        public static FilterPresenter TryGet(IEnumerable source)
        {
            if (source == null)
                return null;
            ICollectionView sourceCollectionView = source as ICollectionView;
            if (sourceCollectionView == null)
                sourceCollectionView = CollectionViewSource.GetDefaultView(source);
            FilterPresenter instance = null;
            //GC.Collect();
            foreach (var entry in filterPresenters.ToArray())
            {
                if (!entry.Value.IsAlive)
                    filterPresenters.Remove(entry.Key);
            }
            if (filterPresenters.ContainsKey(sourceCollectionView))
            {
                var wr = filterPresenters[sourceCollectionView];
                instance = wr.Target as FilterPresenter;
            }
            if (instance == null)
            {
                instance = new FilterPresenter(sourceCollectionView);
                if (filterPresenters.ContainsKey(sourceCollectionView))
                    filterPresenters[sourceCollectionView] = new WeakReference(instance);
                else
                    filterPresenters.Add(sourceCollectionView, new WeakReference(instance));
            }
            return instance;
        }

        private FilterPresenter(ICollectionView source)
        {
            collectionView = source;
            filteredEventArgs = new FilteredEventArgs(source);
            itemProperties = (IItemProperties)source == null ? null
                : ((IItemProperties)source).ItemProperties;
            filterFunction = new Predicate<object>(FilterFunction);
            filters = new Dictionary<string, FiltersCollection>();
        }

        // <summary>
        // Returns the connected  collection.
        // </summary>
        public ICollectionView CollectionView
        {
            get { return collectionView; }
        }

        /// <summary>
        /// Get or set a value that indicates whether the defined filter set to attached ItemsControl.Items.PropertyFilter.
        /// </summary>
        public bool IsFilterActive
        {
            get
            {
                return isFilterActive;
            }
            set
            {
                if (isFilterActive != value)
                {
                    isFilterActive = value;
                    DeferRefresh().Dispose();
                }
            }
        }
        // <summary>
        // Initializes and configures the ViewModel for FilterControl.
        // </summary>
        // <param name="viewKey">A string representing the key for a set of filters.</param>
        // <param name="filterInitializers"> Filter initialisers to determine permissible set of the filters in the FilterControlVm.</param>
        // <returns>Instance of FilterControlVm that was bind to view.</returns>
        public FilterControlVm TryGetFilterControlVm(string viewKey, IEnumerable<FilterInitializer> filterInitializers)
        {
            //string viewKey = view.Key;
            FilterControlVm viewModel = null;
            if (viewKey != null)
            {
                FiltersCollection filtersEntry;
                // Get registered collection by key.
                if (filters.ContainsKey(viewKey))
                    filtersEntry = filters[viewKey];
                else
                {
                    filtersEntry = new FiltersCollection(this);
                    filters.Add(viewKey, filtersEntry);
                }
                filterInitializers = filterInitializers ?? FilterInitializersManager.Default;

                foreach (FilterInitializer initializer in filterInitializers)
                {
                    Type filterKey = initializer.GetType();
                    Filter filter;
                    if (filtersEntry.ContainsKey(filterKey))
                        filter = filtersEntry[filterKey];
                    else
                    {
                        filter = initializer.NewFilter(this, viewKey);
                        if (filter != null)
                            filtersEntry[filterKey] = filter;
                    }
                    if (filter != null)
                    {
                        viewModel = viewModel ?? new FilterControlVm();
                        viewModel.Add(filter);

                    }
                }
                //view.ItemsSource = viewModel; 
            }
            return viewModel;
        }
        /// <summary>
        /// Retrieves  or tries to create the filter model, using as a key pair {viewKey, initializer}.
        /// </summary>
        /// <param name="viewKey">A string representing a key of the set of filters.</param>
        /// <param name="initializer">Initialiser filter that defines filter in the collection of filters.</param>
        /// <returns>FilterPresenter instance, if it is possible provide for couples viewKey and initializer. Otherwise, null.</returns>
        public Filter TryGetFilter(string viewKey, FilterInitializer initializer)
        {
            Filter filter = null;
            if (viewKey != null)
            {
                FiltersCollection filtersEntry;
                // Get registered collection by key.
                if (filters.ContainsKey(viewKey))
                    filtersEntry = filters[viewKey];
                else
                {
                    filtersEntry = new FiltersCollection(this);
                    filters.Add(viewKey, filtersEntry);
                }
                Type filterKey = initializer.GetType();
                if (filtersEntry.ContainsKey(filterKey))
                    filter = filtersEntry[filterKey];
                else
                {
                    filter = initializer.NewFilter(this, viewKey);
                    if (filter != null)
                        filtersEntry[filterKey] = filter;
                }
            }
            return filter;
        }
        // Represent a set of Predicate<Object> that used to generate filter function.
        internal event FilterEventHandler Filter
        {
            add
            {
                if (filterFunction == null)
                    filterFunction = new Predicate<object>(FilterFunction);
                var deferRefresh = DeferRefresh();
                _Filter += value;
                IsFilterActive = true;
                deferRefresh.Dispose();
            }
            remove
            {
                var deferRefresh = DeferRefresh();
                _Filter -= value;
                //if (itemsControl != null && _Filter==null)
                //    itemsControl.Items.PropertyFilter = null;
                IsFilterActive = _Filter != null;
                if (_Filter == null)
                    filterFunction = null;
                deferRefresh.Dispose();
            }
        }

        /// <summary>
        ///  Enters a defer cycle that you can use to change filter of the view and delay automatic refresh.
        /// </summary>
        /// <returns> An System.IDisposable object that you can use to dispose of the calling object. </returns>
        public IDisposable DeferRefresh()
        {
            return new DisposeItemsDeferRefresh(this);
        }
        /// <summary>
        /// Gets a collection that contains information about the properties that are
        //     available on the items in a collection.
        /// </summary>
        public ReadOnlyCollection<ItemPropertyInfo> ItemProperties
        {
            get { return itemProperties; }
            private set
            {
                if (itemProperties != value)
                {
                    itemProperties = value;
                }
            }
        }
        /// <summary>
        /// Occurs after filtration when changing the filter conditions.
        /// </summary>
        public EventHandler<FilteredEventArgs> Filtered;
        // Сообщает FilterPresenter об изменении состояния фильтра.
        // Для экземпляра фильтра в активном состоянии, производится включение фильтра в условие фильтрации представления коллекции.
        // Для экземпляра фильтра в пассивном состоянии, производится исключение фильтра из условия фильтрации коллекции.
        /// <summary>
        /// Receives notice of the change filter conditions and IsActive property.
        /// </summary>
        /// <param name="filter"></param>
        internal void ReceiveFilterChanged(IFilter filter)
        {
            var defer = DeferRefresh();
            Filter -= filter.IsMatch;
            if (filter.IsActive)
                Filter += filter.IsMatch;
            defer.Dispose();
        }
        private void RaiseFiltered()
        {
            lock (filteredEventArgs)
            {
                if (Filtered != null)
                    Filtered(this, filteredEventArgs);
            }
        }
        private bool FilterFunction(object obj)
        {
            if (_Filter != null)
            {
                FilterEventArgs args = new FilterEventArgs(obj);
                _Filter(this, args);
                return args.Accepted;
            }
            else
                return true;
        }
        private class DisposeItemsDeferRefresh : IDisposable
        {
            private FilterPresenter filterPr;
            private bool isDisposed = false;
            internal DisposeItemsDeferRefresh(FilterPresenter filterVm)
            {
                this.filterPr = filterVm;
                IEditableCollectionView cv = filterPr.CollectionView as IEditableCollectionView;
                if (cv != null)
                {
                    if (cv.IsAddingNew)
                        cv.CommitNew();
                    if (cv.IsEditingItem)
                        cv.CommitEdit();
                }
                if (filterPr.itemsDeferRefreshCount == 0)
                    filterPr.itemsDeferRefresh = filterPr.CollectionView.DeferRefresh();
                filterPr.itemsDeferRefreshCount++;
            }
            public void Dispose()
            {
                if (!isDisposed)
                {
                    filterPr.itemsDeferRefreshCount--;
                    if (filterPr.itemsDeferRefreshCount <= 0)
                    {
                        filterPr.itemsDeferRefreshCount = 0;
                        IEditableCollectionView cv = filterPr.CollectionView as IEditableCollectionView;
                        if (cv != null)
                        {
                            if (cv.IsAddingNew)
                                cv.CancelNew();
                            if (cv.IsEditingItem)
                                cv.CancelEdit();
                        }
                        if (filterPr.isFilterActive)
                        {
                            filterPr.CollectionView.Filter = filterPr.filterFunction;
                        }
                        else //if (filterVm.items.PropertyFilter==null)
                            filterPr.CollectionView.Filter = null;
                        filterPr.RaiseFiltered();
                        if (filterPr.itemsDeferRefresh != null)
                        {
                            filterPr.itemsDeferRefresh.Dispose();
                        }
                        filterPr.itemsDeferRefresh = null;
                    }
                    isDisposed = true;
                }
                else throw new ObjectDisposedException("FilterPresenter(" + filterPr.CollectionView.ToString() + ").GetDeferRefresh()");
            }
        }

    }
}
