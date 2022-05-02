using System;
using System.ComponentModel;
using System.Windows.Data;

namespace AIStudio.Wpf.Controls
{
    public class SortCompositeCollection : CompositeCollection, IBindingList
    {
        public bool AllowEdit
        {
            get
            {
                return true;
            }
        }

        public bool AllowNew
        {
            get
            {
                return true;
            }
        }

        public bool AllowRemove
        {
            get
            {
                return true;
            }
        }

        public bool IsSorted
        {
            get
            {
                return true;
            }
        }

        public ListSortDirection SortDirection => throw new NotImplementedException();

        public PropertyDescriptor SortProperty => throw new NotImplementedException();

        public bool SupportsChangeNotification => throw new NotImplementedException();

        public bool SupportsSearching
        {
            get
            {
                return true;
            }
        }

        public bool SupportsSorting
        {
            get
            {
                return true;
            }
        }

        public event ListChangedEventHandler ListChanged;

        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public object AddNew()
        {
            throw new NotImplementedException();
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotImplementedException();
        }

        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public void RemoveSort()
        {
            throw new NotImplementedException();
        }
    }
}
