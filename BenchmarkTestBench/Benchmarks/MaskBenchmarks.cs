using BenchmarkDotNet.Attributes;
using CSharpExtender.ExtensionMethods;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85. Mask allocates three times for one result: a char array, a List of
// the maskable indexes, and the string. Compares that against counting the
// maskable characters in one pass and writing the result with string.Create.
[ShortRunJob]
[MemoryDiagnoser]
public class MaskBenchmarks
{
    [Params("1234-5678-9012-5678", "(555) 867-5309",
        "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz")]
    public string Text = "";

    [Benchmark(Baseline = true)]
    public string Current() => Text.Mask('*', 4, 4);

    [Benchmark]
    public string StringCreate() => MaskWithStringCreate(Text, '*', 4, 4, true);

    [Benchmark]
    public string StackAllocIndexes() => MaskWithStackAllocIndexes(Text, '*', 4, 4, true);

    // Keeps the shipped shape, and only moves the index list off the heap, so the
    // maskable test still runs once per character rather than twice
    private static string MaskWithStackAllocIndexes(string text, char maskChar,
        int visiblePrefixLength, int visibleSuffixLength, bool preserveSeparators)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        Span<int> maskableIndexes = text.Length <= 256
            ? stackalloc int[text.Length]
            : new int[text.Length];

        var characters = text.ToCharArray();

        int maskableCount = 0;

        for (int i = 0; i < characters.Length; i++)
        {
            if (!preserveSeparators || char.IsLetterOrDigit(characters[i]))
            {
                maskableIndexes[maskableCount++] = i;
            }
        }

        bool maskEverything =
            visiblePrefixLength + visibleSuffixLength >= maskableCount;

        int firstMasked = maskEverything ? 0 : visiblePrefixLength;
        int lastMasked = maskEverything
            ? maskableCount - 1
            : maskableCount - visibleSuffixLength - 1;

        for (int i = firstMasked; i <= lastMasked; i++)
        {
            characters[maskableIndexes[i]] = maskChar;
        }

        return new string(characters);
    }

    private static string MaskWithStringCreate(string text, char maskChar,
        int visiblePrefixLength, int visibleSuffixLength, bool preserveSeparators)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var source = text.AsSpan();

        int maskableCount = 0;

        for (int i = 0; i < source.Length; i++)
        {
            if (!preserveSeparators || char.IsLetterOrDigit(source[i]))
            {
                maskableCount++;
            }
        }

        bool maskEverything =
            visiblePrefixLength + visibleSuffixLength >= maskableCount;

        int firstMasked = maskEverything ? 0 : visiblePrefixLength;
        int lastMasked = maskEverything
            ? maskableCount - 1
            : maskableCount - visibleSuffixLength - 1;

        return string.Create(text.Length,
            (text, maskChar, firstMasked, lastMasked, preserveSeparators),
            static (destination, state) =>
            {
                state.text.AsSpan().CopyTo(destination);

                int maskable = 0;

                for (int i = 0; i < destination.Length; i++)
                {
                    if (state.preserveSeparators && !char.IsLetterOrDigit(destination[i]))
                    {
                        continue;
                    }

                    if (maskable >= state.firstMasked && maskable <= state.lastMasked)
                    {
                        destination[i] = state.maskChar;
                    }

                    maskable++;
                }
            });
    }
}
