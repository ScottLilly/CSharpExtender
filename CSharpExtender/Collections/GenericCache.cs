using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

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
    /// <param name="defaultExpiration">
    /// How long an item stays in the cache when <see cref="Set"/> is not given an
    /// expiration of its own. Null uses 15 minutes.
    /// </param>
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

    /// <summary>
    /// Adds a value to the cache, or replaces the one already under that key.
    /// </summary>
    /// <param name="key">Key to store the value under.</param>
    /// <param name="value">Value to cache.</param>
    /// <param name="expiration">
    /// How long this item stays in the cache. Null uses the cache's default expiration.
    /// </param>
    public void Set(TKey key, TValue value, TimeSpan? expiration = null)
    {
        var expirationTime = DateTime.UtcNow.Add(expiration ?? _defaultExpiration);
        var newItem = new CacheItem<TValue>(value, expirationTime);

        // The indexer overwrites whatever is there, and captures nothing
        _cache[key] = newItem;
    }

    /// <summary>
    /// The cached value, or the default for TValue when the key is absent or expired.
    /// </summary>
    /// <param name="key">Key to look for.</param>
    /// <returns>The cached value, or default.</returns>
    public TValue? Get(TKey key)
    {
        if (_cache.TryGetValue(key, out var item))
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

    /// <summary>
    /// Reads a cached value, saying whether there was one, so a cached null and a missing
    /// key can be told apart.
    /// </summary>
    /// <param name="key">Key to look for.</param>
    /// <param name="value">The cached value, or default when this returns false.</param>
    /// <returns>True when the key is present and has not expired.</returns>
    public bool TryGet(TKey key, [MaybeNullWhen(false)] out TValue value)
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

    /// <summary>
    /// Removes an entry from the cache.
    /// </summary>
    /// <param name="key">Key to remove.</param>
    /// <returns>True when the key was in the cache, false when it was not.</returns>
    public bool Remove(TKey key)
    {
        return _cache.TryRemove(key, out _);
    }

    /// <summary>
    /// Removes every entry from the cache, expired or not.
    /// </summary>
    public void Clear()
    {
        _cache.Clear();
    }

    /// <summary>
    /// Removes the entries that have expired. Reading an expired entry drops it anyway,
    /// so this is for reclaiming the memory held by entries nobody is going to read.
    /// </summary>
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
        public T Value { get; }
        public DateTime ExpirationTime { get; }

        public CacheItem(T value, DateTime expirationTime)
        {
            Value = value;
            ExpirationTime = expirationTime;
        }
    }
}