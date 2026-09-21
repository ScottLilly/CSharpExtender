using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using CSharpExtender.Collections;

namespace BenchmarkTestBench.Benchmarks;

// Issue #81. The shipped Set captures the new item in an AddOrUpdate lambda, and
// holds it in a class. The candidates drop the closure and the heap object.
[ShortRunJob]
[MemoryDiagnoser]
public class GenericCacheBenchmarks
{
    private sealed class ItemAsClass<T>
    {
        public T Value { get; set; } = default!;
        public DateTime ExpirationTime { get; set; }
    }

    private readonly struct ItemAsStruct<T>(T value, DateTime expirationTime)
    {
        public T Value { get; } = value;
        public DateTime ExpirationTime { get; } = expirationTime;
    }

    private static readonly TimeSpan s_expiration = TimeSpan.FromMinutes(15);

    private readonly GenericCache<int, string> _shippedCache = new();
    private readonly ConcurrentDictionary<int, ItemAsClass<string>> _classCache = new();
    private readonly ConcurrentDictionary<int, ItemAsStruct<string>> _structCache = new();

    [Params(1, 1000)]
    public int Key;

    [Benchmark(Baseline = true)]
    public void CurrentSet() => _shippedCache.Set(Key, "value");

    [Benchmark]
    public void IndexerWithClassItem()
    {
        _classCache[Key] = new ItemAsClass<string>
        {
            Value = "value",
            ExpirationTime = DateTime.UtcNow.Add(s_expiration)
        };
    }

    [Benchmark]
    public void IndexerWithStructItem()
    {
        _structCache[Key] = new ItemAsStruct<string>("value", DateTime.UtcNow.Add(s_expiration));
    }

    [Benchmark]
    public void AddOrUpdateWithClassItem()
    {
        var item = new ItemAsClass<string>
        {
            Value = "value",
            ExpirationTime = DateTime.UtcNow.Add(s_expiration)
        };

        _classCache.AddOrUpdate(Key, item, (_, _) => item);
    }
}
