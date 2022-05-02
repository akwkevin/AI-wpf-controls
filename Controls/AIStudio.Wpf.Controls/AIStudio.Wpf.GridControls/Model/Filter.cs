using AIStudio.Wpf.GridControls.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace AIStudio.Wpf.GridControls.Model
{
    /// <summary>
    /// Base class for a filter.
    /// </summary>
    public abstract class Filter : IFilter, INotifyPropertyChanged
    {
        private string name = "Filter:";
        private bool isActive;
        private FilterPresenter filterPresenter;
        private readonly List<FilterControlVm> attachedFilterControlVmodels = new List<FilterControlVm>();
        /// <summary>
        /// Get attached FilterPresenter.
        /// </summary>
        public FilterPresenter FilterPresenter
        {
            get { return filterPresenter; }
        }

        /// <summary>
        /// Represent action that determine is item match filter.
        /// </summary>
        /// <param name="sender">FilterPresenter that contains filter.</param>
        /// <param name="e">FilterEventArgs include Item and Accepted fields.</param>
        public abstract void IsMatch(FilterPresenter sender, FilterEventArgs e);
        /// <summary>
        /// Get or set Name of filter.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        /// <summary>
        /// Get or set value, determines is filter IsMatch action include in parentCollection filter.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    IDisposable defer = this.FilterPresenter == null ? null : this.FilterPresenter.DeferRefresh();
                    OnIsActiveChanged();
                    if (defer != null)
                        defer.Dispose();
                    OnPropertyChanged("IsActive");
                }
            }
        }
        /// <summary>
        /// Provides class handling for the AttachPresenter event that occurs when FilterPresenter is attached.
        /// </summary>
        protected virtual void OnAttachPresenter(FilterPresenter presenter)
        {

        }
        /// <summary>
        /// Provides class handling for the DetachPresenter event that occurs when FilterPresenter is detached.
        /// </summary>
        protected virtual void OnDetachPresenter(FilterPresenter presenter)
        {
        }
        /// <summary>
        /// Provide derived class IsActiveChanged event.
        /// </summary>
        protected virtual void OnIsActiveChanged()
        {
            RaiseFilterChanged();
        }
        /// <summary>
        /// Report attached listeners that filter changed.
        /// </summary>
        protected void RaiseFilterChanged()
        {
            if (filterPresenter != null)
                filterPresenter.ReceiveFilterChanged(this);
            foreach (var vm in attachedFilterControlVmodels)
            {
                vm.FilterChanged(this);
            }
        }
        /// <summary>
        /// Number attached to filter instances FilterControlVm.
        /// </summary>
        public int CountAttachedFilterControls
        {
            get { return attachedFilterControlVmodels.Count; }
        }
        internal void Attach(FilterControlVm vm)
        {
            if (!attachedFilterControlVmodels.Contains(vm))
                attachedFilterControlVmodels.Add(vm);
        }
        internal void Detach(FilterPresenter presenter)
        {
            if (presenter != null)
                presenter.Filter -= IsMatch;
            if (presenter == filterPresenter)
                filterPresenter = null;
            OnDetachPresenter(presenter);
        }
        internal void Attach(FilterPresenter presenter)
        {
            filterPresenter = presenter;
            if (filterPresenter != null)
                filterPresenter.ReceiveFilterChanged(this);
            OnAttachPresenter(presenter);
        }
        internal void Detach(FilterControlVm vm)
        {
            attachedFilterControlVmodels.Remove(vm);
        }
        #region Члены INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        protected void VerifyPropertyName(string propertyName)
        {
            var myType = this.GetType();

#if NETFX_CORE
            if (!string.IsNullOrEmpty(propertyName)
                && myType.GetTypeInfo().GetDeclaredProperty(propertyName) == null)
            {
                throw new ArgumentException("Property not found", propertyName);
            }
#else
            if (!string.IsNullOrEmpty(propertyName)
                && myType.GetProperty(propertyName) == null)
            {
#if !SILVERLIGHT
                var descriptor = this as ICustomTypeDescriptor;

                if (descriptor != null)
                {
                    if (descriptor.GetProperties()
                        .Cast<PropertyDescriptor>()
                        .Any(property => property.Name == propertyName))
                    {
                        return;
                    }
                }
#endif

                throw new ArgumentException("Property not found", propertyName);
            }
#endif
        }
        #endregion

    }
}
