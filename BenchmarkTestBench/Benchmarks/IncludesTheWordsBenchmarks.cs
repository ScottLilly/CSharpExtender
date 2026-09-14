using BenchmarkDotNet.Attributes;
using CSharpExtender.ExtensionMethods;

namespace BenchmarkTestBench.Benchmarks;

// Issue #59. The shipped IncludesTheWords searches with CurrentCultureIgnoreCase.
// The candidates test whether a span search or an ordinal comparison is cheaper.
// Ordinal is a behavior change, not a drop-in replacement.
[ShortRunJob]
[MemoryDiagnoser]
public class IncludesTheWordsBenchmarks
{
    private const string _longText =
        "the quick brown fox jumped over the lazy dog while the cat watched " +
        "from the window sill and the mouse hid behind the kitchen cupboard " +
        "the quick brown fox jumped over the lazy dog while the cat watched " +
        "from the window sill and the mouse hid behind the kitchen cupboard";

    private static readonly string[] _wordsThatMatch = ["fox", "cupboard", "window"];
    private static readonly string[] _wordsThatMiss = ["fox", "elephant", "window"];

    [Params(true, false)]
    public bool AllWordsPresent;

    private string[] Words => AllWordsPresent ? _wordsThatMatch : _wordsThatMiss;

    [Benchmark(Baseline = true)]
    public bool Current() => _longText.IncludesTheWords(Words);

    [Benchmark]
    public bool LoopWithCurrentCultureIgnoreCase()
    {
        var words = Words;

        for (int i = 0; i < words.Length; i++)
        {
            if (!_longText.Contains(words[i], StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public bool SpanWithCurrentCultureIgnoreCase()
    {
        var words = Words;
        var source = _longText.AsSpan();

        for (int i = 0; i < words.Length; i++)
        {
            if (source.IndexOf(words[i].AsSpan(),
                    StringComparison.CurrentCultureIgnoreCase) < 0)
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public bool LoopWithOrdinalIgnoreCase()
    {
        var words = Words;

        for (int i = 0; i < words.Length; i++)
        {
            if (!_longText.Contains(words[i], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }
}
