using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpExtender.ExtensionMethods;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85 follow-up. ConvertFromString asks TypeDescriptor for a converter on
// every call, and the answer for a given T never changes. JsonExtensionMethods
// reaches this through GetValueFromJsonPath, so it is on that path too.
[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class ConvertFromStringBenchmarks
{
    private const string NUMBER = "12345";
    private const string DATE = "2026-01-20";

    [Benchmark(Baseline = true)]
    public int Int_Current() => NUMBER.ConvertFromString<int>();

    [Benchmark]
    public int Int_CachedConverter() => ConvertCached<int>(NUMBER);

    [Benchmark]
    public DateTime Date_Current() => DATE.ConvertFromString<DateTime>();

    [Benchmark]
    public DateTime Date_CachedConverter() => ConvertCached<DateTime>(DATE);

    // Held per closed T, so the lookup happens once for each type ever converted
    private static class ConverterFor<T>
    {
        internal static readonly TypeConverter Value = TypeDescriptor.GetConverter(typeof(T));
    }

    private static T ConvertCached<T>(string input)
    {
        var converter = ConverterFor<T>.Value;

        if (converter != null && converter.CanConvertFrom(typeof(string)))
        {
            return (T)converter.ConvertFromString(input)!;
        }

        throw new NotSupportedException(
            $"Conversion from string to type {typeof(T).Name} is not supported.");
    }
}
