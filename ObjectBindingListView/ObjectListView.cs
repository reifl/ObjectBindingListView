﻿using System;
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
        public IList<T> DataSource
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

        public T this[int index]
        {
            get
            {
                return filtered[index];
            }
            set => throw new NotImplementedException();
        }

        object IList.this[int index] { get => this[index]; set => throw new NotImplementedException(); }

        public string Filter
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

        public bool SupportsAdvancedSorting => true;

        public bool SupportsFiltering => true;

        public bool AllowNew => true;

        public bool AllowEdit => true;

        public bool AllowRemove => true;

        public bool SupportsChangeNotification => true;

        public bool SupportsSearching => false;

        public bool SupportsSorting => true;

        public bool IsSorted { get; protected set; }

        public PropertyDescriptor SortProperty { get; protected set; }

        public ListSortDirection SortDirection { get; protected set; }

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        public int Count => filtered.Count;

        public object SyncRoot => null;

        public bool IsSynchronized => false;

        public event ListChangedEventHandler ListChanged;

        public int Add(object value)
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

        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public object AddNew()
        {
            return typeof(T).GetConstructor(new Type[] { }).Invoke(new object[] { });
        }

        public void ApplySort(ListSortDescriptionCollection sorts)
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

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
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

        public void Clear()
        {
            DataSource.Clear();
            Filter = Filter;
            ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        public bool Contains(object value)
        {
            if (value is T)
            {
                return DataSource.Contains((T)value);
            }
            return false;
        }

        public void CopyTo(Array array, int index)
        {
            filtered.CopyTo((T[])array, index);
        }

        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return filtered.GetEnumerator();
        }

        public int IndexOf(object value)
        {
            if (value is T)
            {
                return DataSource.IndexOf((T)value);
            }
            return -1;
        }

        public void Insert(int index, object value)
        {
            if (value is T)
            {
                DataSource.Insert(index, (T)value);
                ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemAdded, index));
            }
        }

        public void Remove(object value)
        {
            if (value is T)
            {
                DataSource.Remove((T)value);
                ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, 0));
                Filter = Filter;
            }
        }

        public void RemoveAt(int index)
        {
            var x = filtered[index];
            DataSource.Remove(x);
            ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
            Filter = Filter;
        }

        public void RemoveFilter()
        {
            Filter = "";
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public void RemoveSort()
        {
            Filter = Filter;
            SortProperty = null;
            IsSorted = false;
        }
    }
}
