using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityGUIFramework
{
    public class ObservableList<T> : IList<T>
    {
        readonly IList<T> _storage = new List<T>();

        public event Action<T> Added;

        protected virtual void OnAdded(T obj)
        {
            Action<T> handler = Added;
            if (handler != null) handler(obj);
        }

        public event Action<T> Removed;

        protected virtual void OnRemoved(T obj)
        {
            Action<T> handler = Removed;
            if (handler != null) handler(obj);
        }

        public event Action Changed;

        protected virtual void OnChanged()
        {
            Action handler = Changed;
            if (handler != null) handler();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _storage.Add(item);
            OnAdded(item);
            OnChanged();
        }

        public void Clear()
        {
            for (int i = 0; i < _storage.Count; i++)
            {
                var item = _storage[i];
                OnRemoved(item);
            }
            _storage.Clear();
            OnChanged();
        }

        public bool Contains(T item)
        {
            return _storage.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _storage.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (!_storage.Contains(item))
                return false;
            _storage.Remove(item);
            OnRemoved(item);
            OnChanged();
            return true;
        }

        public int Count
        {
            get { return _storage.Count; }
        }
        public bool IsReadOnly
        {
            get { return _storage.IsReadOnly; }
        }
        public int IndexOf(T item)
        {
            return _storage.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _storage.Insert(index, item);
            OnAdded(item);
            OnChanged();
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _storage.Count)
                throw new IndexOutOfRangeException();
            var item = _storage[index];
            _storage.RemoveAt(index);
            OnRemoved(item);
            OnChanged();
        }

        public T this[int index]
        {
            get { return _storage[index]; }
            set
            {
                if (index < 0 || index >= _storage.Count)
                    throw new IndexOutOfRangeException();
                var oldItem = _storage[index];
                OnRemoved(oldItem);
                _storage[index] = value;
                OnAdded(value);
                OnChanged();
            }
        }
    }
}
