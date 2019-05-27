using System;
using System.Collections.Generic;

namespace RealismOverhaul.Utils
{
    public class Storage<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> fetchFunc)
        {
            if (!_dict.TryGetValue(key, out var result))
            {
                result = fetchFunc(key);
                _dict.Add(key, result);
            }
            return result;
        }

        public TValue Get(TKey key, TValue def = default(TValue))
        {
            if (!_dict.TryGetValue(key, out var result))
            {
                result = def;
            }
            return result;
        }

        public void Add(TKey key, TValue value)
        {
            _dict[key] = value;
        }

        public void Clear() => _dict.Clear();
        public void Remove(TKey key) => _dict.Remove(key);
    }

    public class StorageView<TKey, TInternalKey, TValue>
    {
        private Func<TKey, TInternalKey> _keyConverter;
        private Storage<TInternalKey, TValue> _storage;

        public StorageView(Storage<TInternalKey, TValue> storage, Func<TKey, TInternalKey> keyConverter) {
            _storage = storage;
            _keyConverter = keyConverter;
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> fetchFunc)
        {
            return _storage.GetOrAdd(_keyConverter(key), x => fetchFunc(key));
        }

        public void Remove(TKey key) => _storage.Remove(_keyConverter(key));
    }

    public static class StorageView
    {
        public static StorageView<TKey, TInternalKey, TValue> Create<TKey, TInternalKey, TValue>(Storage<TInternalKey, TValue> storage, Func<TKey, TInternalKey> keyConverter)
        {
            return new StorageView<TKey, TInternalKey, TValue>(storage, keyConverter);
        }
    }
}
