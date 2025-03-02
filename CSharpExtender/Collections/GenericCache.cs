using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Generic cache implementation. Default expiration is 15 minutes.
/// Backed by ConcurrentDictionary, to be thread-safe.
/// </summary>
/// <typeparam name="TKey">Key value datatype</typeparam>
/// <typeparam name="TValue">Cached value datatype</typeparam>
public class GenericCache<TKey, TValue> where TKey : IEquatable<TKey>
{
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(15);

    private readonly ConcurrentDictionary<TKey, CacheItem<TValue>> _cache = 
        new ConcurrentDictionary<TKey, CacheItem<TValue>>();

    /// <summary>
    /// Instance constructor. 
    /// Uses default expiration of 15 minutes, if no value is passed in during instantiation.
    /// </summary>
    /// <param name="defaultExpiration"></param>
    public GenericCache(TimeSpan? defaultExpiration = null)
    {
        if(defaultExpiration != null)
        {
            _defaultExpiration = (TimeSpan)defaultExpiration;
        }
    }

    public void Set(TKey key, TValue value, TimeSpan? expiration = null)
    {
        _cache[key] =
            new CacheItem<TValue>
            {
                Value = value,
                ExpirationTime = DateTime.Now.Add(expiration ?? _defaultExpiration)
            };
    }

    public TValue Get(TKey key)
    {
        if (_cache.TryGetValue(key, out CacheItem<TValue> item))
        {
            if (DateTime.Now < item.ExpirationTime)
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

    public void Remove(TKey key)
    {
        if (!_cache.TryRemove(key, out _))
        {
            throw new Exception("Unable to remove item from cache.");
        }
    }

    public void Clear()
    {
        _cache.Clear();
    }

    public void RemoveExpiredItems()
    {
        List<TKey> expiredKeys = 
            _cache.Where(kvp => DateTime.Now >= kvp.Value.ExpirationTime)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (TKey key in expiredKeys)
        {
            Remove(key);
        }
    }
}