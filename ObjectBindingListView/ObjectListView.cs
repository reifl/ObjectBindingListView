using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ObjectBindingListView
{
    public class ObjectListView<T> : IBindingListView
    {
        private IList<T> dataSource = null;
        private IList<T> filtered;
        private string filter;
        public virtual IList<T> DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                dataSource = value;
                filtered = dataSource;
                ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        public virtual T this[int index]
        {
            get
            {
                return filtered[index];
            }
            set => throw new NotImplementedException();
        }

        object IList.this[int index] { get => this[index]; set => throw new NotImplementedException(); }

        public virtual string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                filter = value;
                if (string.IsNullOrEmpty(filter))
                    filtered = dataSource;
                else
                {
                    filtered = dataSource.Where(filter)?.ToList();
                }
                if (IsSorted)
                {
                    if (SortDescriptions.Count > 0)
                        ApplySort(SortDescriptions);
                    else
                        ApplySort(SortProperty, SortDirection);
                }
                ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        public ListSortDescriptionCollection SortDescriptions
        {
            get;
            private set;
        } = new ListSortDescriptionCollection();

        public virtual bool SupportsAdvancedSorting => true;

        public virtual bool SupportsFiltering => true;

        public virtual bool AllowNew => true;

        public virtual bool AllowEdit => true;

        public virtual bool AllowRemove => true;

        public virtual bool SupportsChangeNotification => true;

        public virtual bool SupportsSearching => false;

        public virtual bool SupportsSorting => true;

        public virtual bool IsSorted { get; protected set; }

        public virtual PropertyDescriptor SortProperty { get; protected set; }

        public virtual ListSortDirection SortDirection { get; protected set; }

        public virtual bool IsReadOnly => true;

        public virtual bool IsFixedSize => false;

        public virtual int Count => filtered.Count;

        public virtual object SyncRoot => null;

        public virtual bool IsSynchronized => false;

        public event ListChangedEventHandler ListChanged;

        public virtual int Add(object value)
        {
            if (value is T)
            {
                DataSource.Add((T)value);
                Filter = Filter;
                ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemAdded, DataSource.Count - 1));
                return DataSource.Count - 1;
            }
            return -1;
        }

        public virtual void AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public virtual object AddNew()
        {
            
            var newElement = typeof(T).GetConstructor(new Type[] { }).Invoke(new object[] { });
            DataSource.Add((T)newElement);
            return newElement;
        }

        public virtual void ApplySort(ListSortDescriptionCollection sorts)
        {
            SortDescriptions = sorts;
            bool first = false;
            var buffered = filtered.AsEnumerable();
            foreach (ListSortDescription y in sorts)
            {
                var property = y.PropertyDescriptor;
                var direction = y.SortDirection;
                
                try
                {
                    if (!first)
                    {
                        if (direction == ListSortDirection.Ascending)
                        {
                            buffered = buffered.OrderBy(x => x.GetType().GetProperty(property.Name).GetValue(x));
                        }
                        else
                        {
                            buffered = buffered.OrderByDescending(x => x.GetType().GetProperty(property.Name).GetValue(x));
                        }
                    } else
                    {
                        if (direction == ListSortDirection.Ascending)
                        {
                            buffered = (buffered as IOrderedEnumerable<T>).ThenBy(x => x.GetType().GetProperty(property.Name).GetValue(x));
                        }
                        else
                        {
                            buffered = (buffered as IOrderedEnumerable<T>).ThenByDescending(x => x.GetType().GetProperty(property.Name).GetValue(x));
                        }
                    }
                }
                catch (Exception)                      // Fallback String Comparer. Example: Column with different Object Types
                {
                    if (!first)
                    {
                        if (direction == ListSortDirection.Ascending)
                        {
                            buffered = buffered.OrderBy(x => x.GetType().GetProperty(property.Name).GetValue(x)?.ToString()).ToList();
                        }
                        else
                        {
                            buffered = buffered.OrderByDescending(x => x.GetType().GetProperty(property.Name).GetValue(x)?.ToString()).ToList();
                        }
                    } else
                    {
                        if (direction == ListSortDirection.Ascending)
                        {
                            buffered = (buffered as IOrderedEnumerable<T>).ThenBy(x => x.GetType().GetProperty(property.Name).GetValue(x)?.ToString()).ToList();
                        }
                        else
                        {
                            buffered = (buffered as IOrderedEnumerable<T>).ThenByDescending(x => x.GetType().GetProperty(property.Name).GetValue(x)?.ToString()).ToList();
                        }
                    }
                }
                
                first = true;
            }
            filtered = buffered.ToList();
            IsSorted = true;
            ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
            //throw new NotImplementedException();
        }

        public virtual void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            if(property == null)
            {
                IsSorted = false;
                return;
            }
            try
            {
                if (direction == ListSortDirection.Ascending)
                    filtered = filtered.OrderBy(x => x.GetType().GetProperty(property.Name).GetValue(x)).ToList();
                else
                    filtered = filtered.OrderByDescending(x => x.GetType().GetProperty(property.Name).GetValue(x)).ToList();
            }
            catch (Exception)                      // Fallback String Comparer. Example: Column with different Object Types
            {
                if (direction == ListSortDirection.Ascending)
                    filtered = filtered.OrderBy(x => x.GetType().GetProperty(property.Name).GetValue(x)?.ToString()).ToList();
                else
                    filtered = filtered.OrderByDescending(x => x.GetType().GetProperty(property.Name).GetValue(x)?.ToString()).ToList();
            }
            SortProperty = property;
            SortDirection = direction;
            IsSorted = true;
            //throw new NotImplementedException();
        }

        public virtual void Clear()
        {
            DataSource.Clear();
            Filter = Filter;
            ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        public virtual bool Contains(object value)
        {
            if (value is T)
            {
                return DataSource.Contains((T)value);
            }
            return false;
        }

        public virtual void CopyTo(Array array, int index)
        {
            filtered.CopyTo((T[])array, index);
        }

        public virtual int Find(PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerator GetEnumerator()
        {
            return filtered.GetEnumerator();
        }

        public virtual int IndexOf(object value)
        {
            if (value is T)
            {
                return DataSource.IndexOf((T)value);
            }
            return -1;
        }

        public virtual void Insert(int index, object value)
        {
            if (value is T)
            {
                DataSource.Insert(index, (T)value);
                ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemAdded, index));
            }
        }

        public virtual void Remove(object value)
        {
            if (value is T)
            {
                DataSource.Remove((T)value);
                ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, 0));
                Filter = Filter;
            }
        }

        public virtual void RemoveAt(int index)
        {
            var x = filtered[index];
            DataSource.Remove(x);
            ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
            Filter = Filter;
        }

        public virtual void RemoveFilter()
        {
            Filter = "";
        }

        public virtual void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveSort()
        {
            Filter = Filter;
            SortProperty = null;
            IsSorted = false;
        }
    }
}
