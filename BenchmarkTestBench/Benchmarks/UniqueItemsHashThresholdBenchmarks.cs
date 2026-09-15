using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace BenchmarkTestBench.Benchmarks;

// Issue #93. Finds the list length at which putting the items in a set starts to
// cost less than comparing every pair. Below it, allocating and filling the set is
// more work than the handful of comparisons it saves, so UniqueItemsHashCheck
// declines to judge and lets the pairwise walk run.
//
// A custom job rather than ShortRunJob: three iterations put the error bars wider
// than the differences, and rather than MediumRunJob, which takes twenty minutes
// on a matrix this size.
[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class UniqueItemsHashThresholdBenchmarks
{
    private List<object> _items = [];

    [Params(4, 8, 12, 16, 24, 32, 64)]
    public int ItemCount;

    [GlobalSetup]
    public void Setup()
    {
        _items = Enumerable.Range(0, ItemCount).Select(i => (object)$"item-{i}").ToList();
    }

    [Benchmark(Baseline = true)]
    public bool PairwiseOnly()
    {
        for (int i = 0; i < _items.Count - 1; i++)
        {
            for (int j = i + 1; j < _items.Count; j++)
            {
                if (AreSimpleItemsEqual(_items[i], _items[j]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    [Benchmark]
    public bool HashFirst()
    {
        var seen = new HashSet<object>(_items.Count);

        for (int i = 0; i < _items.Count; i++)
        {
            if (!seen.Add(_items[i]))
            {
                return true;
            }
        }

        return false;
    }

    // The shipped comparison, for items already known to be strings
    private static bool AreSimpleItemsEqual(object item1, object item2)
    {
        if (item1.GetType() != item2.GetType())
        {
            return false;
        }

        return item1.Equals(item2);
    }
}
