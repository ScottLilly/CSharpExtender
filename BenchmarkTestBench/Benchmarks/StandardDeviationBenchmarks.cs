using BenchmarkDotNet.Attributes;
using CSharpExtender.ExtensionMethods;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85. The int and decimal overloads convert through Convert.ToDouble(object),
// which boxes every element before the result is materialized into a list.
// Compares that against a cast that stays in the value type.
[ShortRunJob]
[MemoryDiagnoser]
public class StandardDeviationBenchmarks
{
    private int[] _values = [];

    [Params(10, 1000)]
    public int ValueCount;

    [GlobalSetup]
    public void Setup()
    {
        _values = Enumerable.Range(1, ValueCount).ToArray();
    }

    [Benchmark(Baseline = true)]
    public double Current() => _values.StandardDeviation();

    [Benchmark]
    public double WithoutBoxing() =>
        CalculateStandardDeviation(_values.Select(v => (double)v), isSample: true);

    // A copy of the shipped private method, so only the conversion differs
    private static double CalculateStandardDeviation(IEnumerable<double> values, bool isSample)
    {
        var list = values as IReadOnlyList<double> ?? values.ToList();

        double total = 0;

        for (int i = 0; i < list.Count; i++)
        {
            total += list[i];
        }

        double mean = total / list.Count;
        double sumOfSquaredDeviations = 0;

        for (int i = 0; i < list.Count; i++)
        {
            double deviation = list[i] - mean;
            sumOfSquaredDeviations += deviation * deviation;
        }

        return Math.Sqrt(sumOfSquaredDeviations / (isSample ? list.Count - 1 : list.Count));
    }
}
