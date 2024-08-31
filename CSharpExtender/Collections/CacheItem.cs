using System;

public class CacheItem<T>
{
    public T Value { get; set; }
    public DateTime ExpirationTime { get; set; }
}