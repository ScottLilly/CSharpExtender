using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System.Collections;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85 follow-up. UniqueItemsAttribute materializes the collection it is
// validating with Cast<object>().ToList(). The Cast iterator cannot report a
// count, so the list grows by doubling and copies as it goes, even when the
// collection could have said how big it is.
[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class CastToListBenchmarks
{
    private IEnumerable _strings = new List<string>();
    private IEnumerable _numbers = new List<int>();

    [Params(10, 100, 1000)]
    public int ItemCount;

    [GlobalSetup]
    public void Setup()
    {
        _strings = Enumerable.Range(0, ItemCount).Select(i => $"item-{i}").ToList();
        _numbers = Enumerable.Range(0, ItemCount).ToList();
    }

    [Benchmark(Baseline = true)]
    public List<object> Strings_CastToList() => _strings.Cast<object>().ToList();

    [Benchmark]
    public List<object> Strings_Presized() => Presized(_strings);

    [Benchmark]
    public List<object> Numbers_CastToList() => _numbers.Cast<object>().ToList();

    [Benchmark]
    public List<object> Numbers_Presized() => Presized(_numbers);

    private static List<object> Presized(IEnumerable collection)
    {
        // The non-generic ICollection is what a List, an array, or anything else
        // with a known size implements, so the list can be built at its final size
        var items = collection is ICollection sized
            ? new List<object>(sized.Count)
            : new List<object>();

        foreach (object item in collection)
        {
            items.Add(item);
        }

        return items;
    }
}
