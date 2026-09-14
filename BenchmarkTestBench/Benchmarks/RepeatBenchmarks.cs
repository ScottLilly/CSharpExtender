using BenchmarkDotNet.Attributes;
using CSharpExtender.ExtensionMethods;
using System.Text;

namespace BenchmarkTestBench.Benchmarks;

// Issue #58 and #59. Compares the shipped Repeat against candidates that do not
// build an intermediate enumerable.
[ShortRunJob]
[MemoryDiagnoser]
public class RepeatBenchmarks
{
    [Params("a", "abcdefghij", "the quick brown fox jumped over the lazy dog")]
    public string Text = "";

    [Params(3, 100)]
    public int Times;

    [Benchmark(Baseline = true)]
    public string Current() => Text.Repeat(Times);

    [Benchmark]
    public string StringCreate()
    {
        if (string.IsNullOrEmpty(Text) || Times == 0)
        {
            return string.Empty;
        }

        return string.Create(Text.Length * Times, (Text, Times), (span, state) =>
        {
            var (text, times) = state;
            var source = text.AsSpan();

            for (int i = 0; i < times; i++)
            {
                source.CopyTo(span.Slice(i * source.Length));
            }
        });
    }

    [Benchmark]
    public string StringCreateWithSingleCharFastPath()
    {
        if (string.IsNullOrEmpty(Text) || Times == 0)
        {
            return string.Empty;
        }

        // Repeating one character is common enough to be worth its own path
        if (Text.Length == 1)
        {
            return new string(Text[0], Times);
        }

        return string.Create(Text.Length * Times, (Text, Times), (span, state) =>
        {
            var (text, times) = state;
            var source = text.AsSpan();

            for (int i = 0; i < times; i++)
            {
                source.CopyTo(span.Slice(i * source.Length));
            }
        });
    }

    [Benchmark]
    public string StringBuilderInsert()
    {
        if (string.IsNullOrEmpty(Text) || Times == 0)
        {
            return string.Empty;
        }

        return new StringBuilder(Text.Length * Times).Insert(0, Text, Times).ToString();
    }
}
