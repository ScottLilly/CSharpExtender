using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace CSharpExtender.Collections;

/// <summary>
/// Generic cache implementation. Default expiration is 15 minutes.
/// Backed by ConcurrentDictionary, to be thread-safe.
/// </summary>
/// <remarks>
/// Expired entries are dropped as they are read, so a cache that is read regularly needs
/// nothing else. Call <see cref="RemoveExpiredItems"/>, or hand the constructor a cleanup
/// interval, for entries nobody is going to read again.
/// </remarks>
/// <typeparam name="TKey">Key value datatype</typeparam>
/// <typeparam name="TValue">Cached value datatype</typeparam>
public class GenericCache<TKey, TValue> : IDisposable where TKey : IEquatable<TKey>
{
    private readonly ConcurrentDictionary<TKey, CacheItem<TValue>> _cache = new();
    private readonly TimeSpan _defaultExpiration;
    private readonly Timer? _cleanupTimer;

    // 0 between passes, 1 while one is running. A tick that arrives during a pass is
    // dropped rather than queued, because the next tick would only do the same work
    private int _isCleaning;

    private bool _isDisposed;

    /// <summary>
    /// Instance constructor.
    /// Uses default expiration of 15 minutes, if no value is passed in during instantiation.
    /// </summary>
    /// <param name="defaultExpiration">
    /// How long an item stays in the cache when <see cref="Set"/> is not given an
    /// expiration of its own. Null uses 15 minutes.
    /// </param>
    public GenericCache(TimeSpan? defaultExpiration = null)
        : this(defaultExpiration, null)
    {
    }

    /// <summary>
    /// Instance constructor, with a background pass that drops expired entries.
    /// </summary>
    /// <remarks>
    /// The pass runs on a thread pool thread and does exactly what
    /// <see cref="RemoveExpiredItems"/> does. A tick that arrives while the previous pass
    /// is still running is dropped rather than queued.
    /// <para>
    /// A cache with a cleanup interval has to be disposed. The timer holds a reference to
    /// the cache, so until <see cref="Dispose()"/> is called the cache cannot be collected
    /// and the pass keeps running.
    /// </para>
    /// </remarks>
    /// <param name="defaultExpiration">
    /// How long an item stays in the cache when <see cref="Set"/> is not given an
    /// expiration of its own. Null uses 15 minutes.
    /// </param>
    /// <param name="cleanupInterval">
    /// How often to drop expired entries. Null runs no background pass at all, which is
    /// what the other constructor does.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="cleanupInterval"/> is zero or negative.
    /// </exception>
    public GenericCache(TimeSpan? defaultExpiration, TimeSpan? cleanupInterval)
    {
        _defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(15);

        if (cleanupInterval == null)
        {
            return;
        }

        if (cleanupInterval.Value <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(cleanupInterval),
                cleanupInterval.Value, "The cleanup interval must be greater than zero.");
        }

        _cleanupTimer =
            new Timer(_ => Cleanup(), null, cleanupInterval.Value, cleanupInterval.Value);
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

    /// <summary>
    /// Stops the background cleanup pass, if the cache was given a cleanup interval.
    /// Calling it more than once does nothing further, and a cache with no cleanup
    /// interval has nothing to stop.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Stops the background cleanup pass. Override to release a subclass's own
    /// resources, calling this implementation as well.
    /// </summary>
    /// <param name="disposing">
    /// True when called from <see cref="Dispose()"/> rather than from a finalizer.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            _cleanupTimer?.Dispose();
        }

        _isDisposed = true;
    }

    private void Cleanup()
    {
        // A tick that lands while the previous pass is still running is dropped
        if (Interlocked.CompareExchange(ref _isCleaning, 1, 0) != 0)
        {
            return;
        }

        try
        {
            RemoveExpiredItems();
        }
        catch
        {
            // This runs on a thread pool thread with none of the caller's code above it,
            // so anything thrown here takes the process down and reaches nobody who
            // could act on it. RemoveExpiredItems has no throwing path today; this is
            // what stops a future one from becoming a crash in somebody else's app.
        }
        finally
        {
            Interlocked.Exchange(ref _isCleaning, 0);
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