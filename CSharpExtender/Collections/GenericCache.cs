using System;
using System.Collections.Concurrent;

namespace CSharpExtender.Collections;

/// <summary>
/// Generic cache implementation. Default expiration is 15 minutes.
/// Backed by ConcurrentDictionary, to be thread-safe.
/// </summary>
/// <typeparam name="TKey">Key value datatype</typeparam>
/// <typeparam name="TValue">Cached value datatype</typeparam>
public class GenericCache<TKey, TValue> where TKey : IEquatable<TKey>
{
    private readonly ConcurrentDictionary<TKey, CacheItem<TValue>> _cache = new();
    private readonly TimeSpan _defaultExpiration;

    /// <summary>
    /// Instance constructor. 
    /// Uses default expiration of 15 minutes, if no value is passed in during instantiation.
    /// </summary>
    /// <param name="defaultExpiration"></param>
    public GenericCache(TimeSpan? defaultExpiration = null)
    {
        if (defaultExpiration == null)
        {
            _defaultExpiration = TimeSpan.FromMinutes(15);
        }
        else
        {
            _defaultExpiration = (TimeSpan)defaultExpiration;
        }
    }

    public void Set(TKey key, TValue value, TimeSpan? expiration = null)
    {
        var expirationTime = DateTime.UtcNow.Add(expiration ?? _defaultExpiration);
        var newItem = new CacheItem<TValue>
        {
            Value = value,
            ExpirationTime = expirationTime
        };

        _cache.AddOrUpdate(key, newItem, (_, _) => newItem);
    }

    public TValue Get(TKey key)
    {
        if (_cache.TryGetValue(key, out CacheItem<TValue> item))
        {
            if (DateTime.UtcNow < item.ExpirationTime)
            {
                return item.Value;
            }
            else
            {
                // Remove expired item
                Remove(key);
            }
        }

        return default;
    }

    public bool TryGet(TKey key, out TValue value)
    {
        if (_cache.TryGetValue(key, out var item))
        {
            if (DateTime.UtcNow < item.ExpirationTime)
            {
                value = item.Value;
                return true;
            }

            Remove(key);
        }

        value = default;
        return false;
    }

    public bool Remove(TKey key)
    {
        return _cache.TryRemove(key, out _);
    }

    public void Clear()
    {
        _cache.Clear();
    }

    public void RemoveExpiredItems()
    {
        var now = DateTime.UtcNow;

        foreach (var kvp in _cache)
        {
            if (now >= kvp.Value.ExpirationTime)
            {
                _cache.TryRemove(kvp.Key, out _);
            }
        }
    }

    private class CacheItem<T>
    {
        public T Value { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}