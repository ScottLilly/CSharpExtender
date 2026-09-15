using BenchmarkDotNet.Attributes;
using CSharpExtender.ExtensionMethods;

namespace BenchmarkTestBench.Benchmarks;

// Issue #59. The shipped RemoveText calls Replace in a loop until no match is
// left, allocating a whole new string each pass. The candidate copies into one
// buffer and rewinds the write position whenever the tail matches, which handles
// the same overlap cases in a single pass.
[ShortRunJob]
[MemoryDiagnoser]
public class RemoveTextBenchmarks
{
    private const string REPEATED_MATCHES =
        "the abc quick abc brown abc fox abc jumped abc over abc the abc lazy abc dog abc " +
        "the abc quick abc brown abc fox abc jumped abc over abc the abc lazy abc dog abc";

    private const string NO_MATCHES =
        "the quick brown fox jumped over the lazy dog " +
        "the quick brown fox jumped over the lazy dog";

    [Params(REPEATED_MATCHES, NO_MATCHES, "aaabbbaaabbb")]
    public string Text = "";

    [Params("abc", "ab")]
    public string TextToRemove = "";

    [Benchmark(Baseline = true)]
    public string CurrentOrdinal() => Text.RemoveText(TextToRemove, StringComparison.Ordinal);

    [Benchmark]
    public string SinglePassOrdinal() =>
        RemoveInOnePass(Text, TextToRemove, StringComparison.Ordinal);

    [Benchmark]
    public string CurrentCultureIgnoreCase() =>
        Text.RemoveText(TextToRemove, StringComparison.CurrentCultureIgnoreCase);

    [Benchmark]
    public string SinglePassOrdinalIgnoreCase() =>
        RemoveInOnePass(Text, TextToRemove, StringComparison.OrdinalIgnoreCase);

    // The shipped version calls Contains and then Replace, scanning twice per
    // pass. Replace already returns the original instance when it finds nothing,
    // so the Contains call may be redundant.
    [Benchmark]
    public string ReplaceOnlyOrdinal()
    {
        string text = Text;

        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(TextToRemove))
        {
            return text;
        }

        while (true)
        {
            string replaced = text.Replace(TextToRemove, "", StringComparison.Ordinal);

            if (ReferenceEquals(replaced, text))
            {
                return text;
            }

            text = replaced;
        }
    }

    // Only valid for ordinal comparisons. A culture-aware comparison can match a
    // run of a different length than the needle, which this fixed-length tail
    // check would get wrong.
    private static string RemoveInOnePass(string text, string textToRemove,
        StringComparison comparison)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(textToRemove))
        {
            return text;
        }

        var source = text.AsSpan();
        var needle = textToRemove.AsSpan();

        Span<char> buffer = new char[source.Length];
        int written = 0;

        for (int i = 0; i < source.Length; i++)
        {
            buffer[written++] = source[i];

            // Called through MemoryExtensions, because Span<char>.Equals would
            // bind to object.Equals instead
            if (written >= needle.Length &&
                MemoryExtensions.Equals(
                    buffer.Slice(written - needle.Length, needle.Length),
                    needle, comparison))
            {
                written -= needle.Length;
            }
        }

        return written == source.Length ? text : new string(buffer.Slice(0, written));
    }
}
