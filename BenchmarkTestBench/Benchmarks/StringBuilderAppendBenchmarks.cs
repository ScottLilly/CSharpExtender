using BenchmarkDotNet.Attributes;
using CSharpExtender.ExtensionMethods;
using CSharpExtender.Options;
using System.Text;

namespace BenchmarkTestBench.Benchmarks;

// Issue #80. The shipped AppendLineIfNotEmpty allocates a closure, and every path
// through ProcessText allocates a StringBuilderOptions when none is passed.
//
// Each benchmark appends _appendCount lines to a StringBuilder that is cleared
// but keeps its capacity, so the numbers are the cost of appending rather than
// the cost of growing a buffer. Divide by _appendCount for a per-call figure.
[ShortRunJob]
[MemoryDiagnoser]
public class StringBuilderAppendBenchmarks
{
    private const string _text = "the quick brown fox jumped over the lazy dog";
    private const int _appendCount = 100;

    private static readonly StringBuilderOptions _sharedDefaultOptions = new();

    private readonly StringBuilder _stringBuilder = new(8192);

    [Benchmark(Baseline = true, OperationsPerInvoke = _appendCount)]
    public void CurrentAppendLineIfNotEmpty()
    {
        _stringBuilder.Clear();

        for (int i = 0; i < _appendCount; i++)
        {
            _stringBuilder.AppendLineIfNotEmpty(_text);
        }
    }

    [Benchmark(OperationsPerInvoke = _appendCount)]
    public void DirectTestWithSharedOptions()
    {
        _stringBuilder.Clear();

        for (int i = 0; i < _appendCount; i++)
        {
            if (!string.IsNullOrWhiteSpace(_text))
            {
                _stringBuilder.AppendLine(ProcessTextWithSharedOptions(_text, null));
            }
        }
    }

    [Benchmark(OperationsPerInvoke = _appendCount)]
    public void CurrentAppendLine()
    {
        _stringBuilder.Clear();

        for (int i = 0; i < _appendCount; i++)
        {
            _stringBuilder.AppendLine(_text, null);
        }
    }

    [Benchmark(OperationsPerInvoke = _appendCount)]
    public void AppendLineWithSharedOptions()
    {
        _stringBuilder.Clear();

        for (int i = 0; i < _appendCount; i++)
        {
            _stringBuilder.AppendLine(ProcessTextWithSharedOptions(_text, null));
        }
    }

    // The same body as the shipped ProcessText, except for the default instance
    private static string ProcessTextWithSharedOptions(string text, StringBuilderOptions? options)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        options ??= _sharedDefaultOptions;

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
