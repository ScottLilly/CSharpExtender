using BenchmarkDotNet.Attributes;
using CSharpExtender.ExtensionMethods;
using CSharpExtender.Options;
using System.Text;

namespace BenchmarkTestBench.Benchmarks;

// Issue #80. The shipped AppendLineIfNotEmpty allocates a closure, and every path
// through ProcessText allocates a StringBuilderOptions when none is passed.
//
// Each benchmark appends APPEND_COUNT lines to a StringBuilder that is cleared
// but keeps its capacity, so the numbers are the cost of appending rather than
// the cost of growing a buffer. Divide by APPEND_COUNT for a per-call figure.
[ShortRunJob]
[MemoryDiagnoser]
public class StringBuilderAppendBenchmarks
{
    private const string TEXT = "the quick brown fox jumped over the lazy dog";
    private const int APPEND_COUNT = 100;

    private static readonly StringBuilderOptions s_sharedDefaultOptions = new();

    private readonly StringBuilder _stringBuilder = new(8192);

    [Benchmark(Baseline = true, OperationsPerInvoke = APPEND_COUNT)]
    public void CurrentAppendLineIfNotEmpty()
    {
        _stringBuilder.Clear();

        for (int i = 0; i < APPEND_COUNT; i++)
        {
            _stringBuilder.AppendLineIfNotEmpty(TEXT);
        }
    }

    [Benchmark(OperationsPerInvoke = APPEND_COUNT)]
    public void DirectTestWithSharedOptions()
    {
        _stringBuilder.Clear();

        for (int i = 0; i < APPEND_COUNT; i++)
        {
            if (!string.IsNullOrWhiteSpace(TEXT))
            {
                _stringBuilder.AppendLine(ProcessTextWithSharedOptions(TEXT, null));
            }
        }
    }

    [Benchmark(OperationsPerInvoke = APPEND_COUNT)]
    public void CurrentAppendLine()
    {
        _stringBuilder.Clear();

        for (int i = 0; i < APPEND_COUNT; i++)
        {
            _stringBuilder.AppendLine(TEXT, null);
        }
    }

    [Benchmark(OperationsPerInvoke = APPEND_COUNT)]
    public void AppendLineWithSharedOptions()
    {
        _stringBuilder.Clear();

        for (int i = 0; i < APPEND_COUNT; i++)
        {
            _stringBuilder.AppendLine(ProcessTextWithSharedOptions(TEXT, null));
        }
    }

    // The same body as the shipped ProcessText, except for the default instance
    private static string ProcessTextWithSharedOptions(string text, StringBuilderOptions? options)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        options ??= s_sharedDefaultOptions;

        string result = text;

        if (options.MaxLength.HasValue && result.Length > options.MaxLength.Value)
        {
            result = result.Substring(0, options.MaxLength.Value);
        }

        if (options.ToUpper && !options.ToLower)
        {
            result = result.ToUpper();
        }
        else if (options.ToLower && !options.ToUpper)
        {
            result = result.ToLower();
        }

        if (!string.IsNullOrEmpty(options.PrefixText))
        {
            result = options.PrefixText + result;
        }

        if (!string.IsNullOrEmpty(options.SuffixText))
        {
            result += options.SuffixText;
        }

        return result;
    }
}
