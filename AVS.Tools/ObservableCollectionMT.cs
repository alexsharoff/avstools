using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;


namespace AVS.Tools
{
    public class ObservableCollectionMT<T> : IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged, IWeakEventListener
    {
        volatile bool m_preventUpdate = false;
        public void ReplaceCollection(IEnumerable<T> collection)
        {
            m_preventUpdate = true;
            foreach (T item in collection)
                AsICollectionT.Add(item);
            m_preventUpdate = false;
            internalOC_CollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddRange(IEnumerable<T> collection)
        {
            m_preventUpdate = true;
            foreach (T item in collection)
                AsICollectionT.Add(item);
            m_preventUpdate = false;
            internalOC_CollectionRangeAdded(collection);
        }

        #region Private Fields

        private INotifyCollectionChanged internalOC;
        private IList<T> AsIListT;
        private IList AsIList;
        private ICollection AsICollection;
        private ICollection<T> AsICollectionT;
        private IEnumerable AsIEnumerable;
        private IEnumerable<T> AsIEnumerableT;

        #endregion Private Fields

        #region Constructors

        public ObservableCollectionMT()
        {
            internalOC = new ObservableCollection<T>();
            collectionChangedHandlers = new Dictionary<NotifyCollectionChangedEventHandler, ObservableCollectionMTCollectionChangedEventData>();
            collectionRangeChangedHandlers = new Dictionary<NotifyCollectionChangedEventHandler, ObservableCollectionMTCollectionRangeAddedEventData>();

            AsIListT = internalOC as IList<T>;
            AsIList = internalOC as IList;
            AsICollection = internalOC as ICollection;
            AsICollectionT = internalOC as ICollection<T>;
            AsIEnumerable = internalOC as IEnumerable;
            AsIEnumerableT = internalOC as IEnumerable<T>;

            CollectionChangedEventManager.AddListener(internalOC, this);
        }

        #endregion Constructors

        #region Handlers

        void internalOC_CollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (m_preventUpdate)
                return;
            KeyValuePair<NotifyCollectionChangedEventHandler, ObservableCollectionMTCollectionChangedEventData>[] handlers = collectionChangedHandlers.ToArray();

            if (handlers.Length > 0)
            {
                foreach (KeyValuePair<NotifyCollectionChangedEventHandler, ObservableCollectionMTCollectionChangedEventData> kvp in handlers)
                {
                    if (kvp.Value.Dispatcher == null)
                    {
                        kvp.Value.Action(e);
                    }
                    else
                    {
                        kvp.Value.Dispatcher.Invoke(kvp.Value.Action, DispatcherPriority.DataBind, e);
                    }
                }
            }
        }

        void internalOC_CollectionRangeAdded(IEnumerable range)
        {
            if (m_preventUpdate)
                return;
            var handlers = collectionRangeChangedHandlers.ToArray();

            if (handlers.Length > 0)
            {
                foreach (var kvp in handlers)
                {
                    if (kvp.Value.Dispatcher == null)
                    {
                        kvp.Value.Action(range);
                    }
                    else
                    {
                        kvp.Value.Dispatcher.Invoke(kvp.Value.Action, DispatcherPriority.DataBind, range);
                    }
                }
            }
        }

        #endregion Handlers

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return AsIListT.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            AsIListT.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            AsIListT.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return AsIListT[index];
            }
            set
            {
                AsIListT[index] = value;
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            AsICollectionT.Add(item);
        }

        public void Clear()
        {
            AsICollectionT.Clear();
        }

        public bool Contains(T item)
        {
            return AsICollectionT.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            AsICollectionT.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return AsICollectionT.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return AsICollectionT.IsReadOnly;
            }
        }

        public bool Remove(T item)
        {
            return AsICollectionT.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return AsIEnumerableT.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return AsIEnumerable.GetEnumerator();
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            return AsIList.Add(value);
        }

        public bool Contains(object value)
        {
            return AsIList.Contains(value);
        }

        public int IndexOf(object value)
        {
            return AsIList.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            AsIList.Insert(index, value);
        }

        public bool IsFixedSize
        {
            get
            {
                return AsIList.IsFixedSize;
            }
        }

        public void Remove(object value)
        {
            AsIList.Remove(value);
        }

        object IList.this[int index]
        {
            get
            {
                try
                {
                    return AsIList[index];
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                AsIList[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            AsICollection.CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get
            {
                return AsICollection.IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return AsICollection.SyncRoot;
            }
        }

        #endregion

        #region INotifyCollectionChanged Members

        private Dictionary<NotifyCollectionChangedEventHandler, ObservableCollectionMTCollectionRangeAddedEventData> collectionRangeChangedHandlers;
        private Dictionary<NotifyCollectionChangedEventHandler, ObservableCollectionMTCollectionChangedEventData> collectionChangedHandlers;
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
                collectionChangedHandlers.Add(value, new ObservableCollectionMTCollectionChangedEventData(dispatcher,
                        new Action<NotifyCollectionChangedEventArgs>((args) => value(this, args))));
                collectionRangeChangedHandlers.Add(value, new ObservableCollectionMTCollectionRangeAddedEventData(dispatcher,
                        new Action<IEnumerable>(
                            (collection) =>
                            {
                                foreach(object o in collection)
                                    value(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, o));
                            })));
            }
            remove
            {
                collectionChangedHandlers.Remove(value);
                collectionRangeChangedHandlers.Remove(value);
            }
        }

        #endregion
        
        #region IWeakEventListener Members

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            internalOC_CollectionChanged(e as NotifyCollectionChangedEventArgs);
            return true;
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
