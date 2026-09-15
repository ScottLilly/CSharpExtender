using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpExtender.ExtensionMethods;
using System.ComponentModel;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85 follow-up. GetEnumValues built a fresh array on every call, through a
// non-generic Enum.GetValues that returns an Array and has to be cast. An enum's
// members are fixed when it is compiled, so the answer never changes.
// Uncached is what the method used to do, so the pair reads as before and after.
[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class GetEnumValuesBenchmarks
{
    // Returned as concrete types rather than IEnumerable, which BenchmarkDotNet
    // rejects as deferred. Both just hand back a reference either way.
    [Benchmark(Baseline = true)]
    public Level[] Uncached() => (Level[])Enum.GetValues(typeof(Level));

    [Benchmark]
    public object Current() => EnumExtensionMethods.GetEnumValues<Level>();

    [Benchmark]
    public int CountThemUncached()
    {
        int count = 0;

        foreach (var value in (Level[])Enum.GetValues(typeof(Level)))
        {
            count += (int)value;
        }

        return count;
    }

    [Benchmark]
    public int CountThemCurrent()
    {
        int count = 0;

        foreach (var value in EnumExtensionMethods.GetEnumValues<Level>())
        {
            count += (int)value;
        }

        return count;
    }

    public enum Level
    {
        [Description("Not set")]
        None = 0,
        [Description("Low priority")]
        Low = 1,
        [Description("Normal priority")]
        Normal = 2,
        [Description("High priority")]
        High = 3,
        [Description("Drop everything")]
        Critical = 4
    }
}
