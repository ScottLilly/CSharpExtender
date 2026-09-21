using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace BenchmarkTestBench.Benchmarks;

// Issue #87. CollapseWhitespace trims the text and reduces every run of whitespace
// inside it to a single space. Four ways to do it, measured before one of them is
// shipped: the idiomatic regex, split and rejoin, one pass over a span, and one pass
// with a scan in front of it that returns the original instance when there is nothing
// to change. The last one is the only candidate that can avoid allocating at all, and
// the question is whether that scan pays for itself on text that does need work.
[ShortRunJob]
[MemoryDiagnoser]
public partial class CollapseWhitespaceBenchmarks
{
    private const string CLEAN =
        "the quick brown fox jumped over the lazy dog";

    private const string DIRTY =
        "  the   quick\tbrown\r\n fox    jumped  over \t the   lazy dog  ";

    private const string LONG_CLEAN =
        "the quick brown fox jumped over the lazy dog " +
        "the quick brown fox jumped over the lazy dog " +
        "the quick brown fox jumped over the lazy dog " +
        "the quick brown fox jumped over the lazy dog " +
        "the quick brown fox jumped over the lazy dog " +
        "the quick brown fox jumped over the lazy do";

    private const string LONG_DIRTY =
        "  the   quick\tbrown\r\n fox    jumped  over \t the   lazy dog  " +
        "  the   quick\tbrown\r\n fox    jumped  over \t the   lazy dog  " +
        "  the   quick\tbrown\r\n fox    jumped  over \t the   lazy dog  " +
        "  the   quick\tbrown\r\n fox    jumped  over \t the   lazy dog  ";

    // Trailing whitespace only, so the fast path's cheap first and last character
    // check catches it before the loop runs
    private const string TRAILING_ONLY =
        "the quick brown fox jumped over the lazy dog  ";

    [Params(CLEAN, DIRTY, LONG_CLEAN, LONG_DIRTY, TRAILING_ONLY)]
    public string Text = "";

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRun();

    [Benchmark(Baseline = true)]
    public string Regex() => WhitespaceRun().Replace(Text.Trim(), " ");

    [Benchmark]
    public string SplitAndJoin() =>
        string.Join(' ', Text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries));

    [Benchmark]
    public string SinglePass() => CollapseAlwaysBuilding(Text);

    [Benchmark]
    public string SinglePassWithFastPath() => CollapseWithFastPath(Text);

    private const int STACK_ALLOC_LIMIT = 256;

    private static string CollapseAlwaysBuilding(string text)
    {
        if (text == null)
        {
            return null;
        }

        var source = text.AsSpan();

        Span<char> collapsed = source.Length <= STACK_ALLOC_LIMIT
            ? stackalloc char[source.Length]
            : new char[source.Length];

        int count = 0;
        bool pendingSpace = false;

        for (int i = 0; i < source.Length; i++)
        {
            char character = source[i];

            if (char.IsWhiteSpace(character))
            {
                pendingSpace = count > 0;

                continue;
            }

            if (pendingSpace)
            {
                collapsed[count++] = ' ';
                pendingSpace = false;
            }

            collapsed[count++] = character;
        }

        return new string(collapsed.Slice(0, count));
    }

    private static string CollapseWithFastPath(string text)
    {
        if (text == null)
        {
            return null;
        }

        if (!NeedsCollapsing(text.AsSpan()))
        {
            return text;
        }

        return CollapseAlwaysBuilding(text);
    }

    private static bool NeedsCollapsing(ReadOnlySpan<char> source)
    {
        if (source.Length == 0)
        {
            return false;
        }

        if (char.IsWhiteSpace(source[0]) || char.IsWhiteSpace(source[source.Length - 1]))
        {
            return true;
        }

        // The first and last characters are known not to be whitespace by here, so
        // the loop can skip both ends and read i + 1 without a bounds check
        for (int i = 1; i < source.Length - 1; i++)
        {
            if (!char.IsWhiteSpace(source[i]))
            {
                continue;
            }

            if (source[i] != ' ' || char.IsWhiteSpace(source[i + 1]))
            {
                return true;
            }
        }

        return false;
    }
}
