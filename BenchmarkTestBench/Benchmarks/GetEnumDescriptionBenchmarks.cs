using System.Collections.Concurrent;
using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using CSharpExtender.ExtensionMethods;

namespace BenchmarkTestBench.Benchmarks;

public enum BenchmarkEnum
{
    [Description("The first option")]
    FirstOption,
    [Description("The second option")]
    SecondOption,
    NoDescription
}

// Issue #79. The shipped GetEnumDescription boxes the value on every call, even
// on a cache hit. The candidate keys a per-type dictionary on the enum itself.
[ShortRunJob]
[MemoryDiagnoser]
public class GetEnumDescriptionBenchmarks
{
    [Params(BenchmarkEnum.FirstOption, BenchmarkEnum.NoDescription)]
    public BenchmarkEnum Value;

    [GlobalSetup]
    public void Setup()
    {
        // Both caches are warm, so the benchmark measures lookups rather than
        // first-call reflection
        Value.GetEnumDescription();
        Candidate(Value);
    }

    [Benchmark(Baseline = true)]
    public string Current() => Value.GetEnumDescription();

    [Benchmark]
    public string GenericStaticCache() => Candidate(Value);

    private static string Candidate<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        return DescriptionCache<TEnum>.Values.GetOrAdd(value, static enumValue =>
        {
            var fieldInfo = typeof(TEnum).GetField(enumValue.ToString());

            if (fieldInfo == null)
            {
                return enumValue.ToString();
            }

            var attributes =
                (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : enumValue.ToString();
        });
    }

    private static class DescriptionCache<TEnum> where TEnum : struct, Enum
    {
        internal static readonly ConcurrentDictionary<TEnum, string> Values = new();
    }
}
