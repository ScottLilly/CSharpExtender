using BenchmarkDotNet.Attributes;
using CSharpExtender.ExtensionMethods;

namespace BenchmarkTestBench.Benchmarks;

// Issue #59. Compares the shipped LINQ-based ToDigitsOnly against span candidates.
[ShortRunJob]
[MemoryDiagnoser]
public class ToDigitsOnlyBenchmarks
{
    private const int STACK_ALLOC_LIMIT = 256;

    private const string LONG_MIXED_STRING =
        "555-123-4567 ext 890, account 1234-5678-9012-3456, ref ABC/987/XYZ, " +
        "555-123-4567 ext 890, account 1234-5678-9012-3456, ref ABC/987/XYZ, " +
        "555-123-4567 ext 890, account 1234-5678-9012-3456, ref ABC/987/XYZ, " +
        "555-123-4567 ext 890, account 1234-5678-9012-3456, ref ABC/987/XYZ, " +
        "555-123-4567 ext 890, account 1234-5678-9012-3456, ref ABC/987/XYZ";

    [Params("1234567890", "(555) 123-4567", LONG_MIXED_STRING)]
    public string Text = "";

    [Benchmark(Baseline = true)]
    public string? Current() => Text.ToDigitsOnly();

    [Benchmark]
    public string? SpanWithHeapBuffer()
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            return null;
        }

        var source = Text.AsSpan();
        Span<char> buffer = new char[source.Length];
        int count = 0;

        for (int i = 0; i < source.Length; i++)
        {
            if (char.IsDigit(source[i]))
            {
                buffer[count++] = source[i];
            }
        }

        return new string(buffer.Slice(0, count));
    }

    [Benchmark]
    public string? SpanWithStackBuffer()
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            return null;
        }

        var source = Text.AsSpan();

        Span<char> buffer = source.Length <= STACK_ALLOC_LIMIT
            ? stackalloc char[STACK_ALLOC_LIMIT]
            : new char[source.Length];

        int count = 0;

        for (int i = 0; i < source.Length; i++)
        {
            if (char.IsDigit(source[i]))
            {
                buffer[count++] = source[i];
            }
        }

        return new string(buffer.Slice(0, count));
    }

    [Benchmark]
    public string? SpanWithExactStackBuffer()
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            return null;
        }

        var source = Text.AsSpan();

        // Sized to the input rather than to the limit, so only what is needed
        // gets zero-initialized
        Span<char> buffer = source.Length <= STACK_ALLOC_LIMIT
            ? stackalloc char[source.Length]
            : new char[source.Length];

        int count = 0;

        for (int i = 0; i < source.Length; i++)
        {
            if (char.IsDigit(source[i]))
            {
                buffer[count++] = source[i];
            }
        }

        return new string(buffer.Slice(0, count));
    }
}
