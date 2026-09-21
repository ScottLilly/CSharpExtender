using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpExtender.ExtensionMethods;
using CSharpExtender.Options;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85 follow-up. Every StringBuilder extension method routes through
// ProcessText, which applies the options one at a time, each producing another
// string. A prefix, a suffix and an indent are three separate concatenations, and
// the indent itself is a fourth string. They can be assembled in one pass.
[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class ProcessTextBenchmarks
{
    private const string TEXT = "the quick brown fox jumped over the lazy dog";

    private readonly StringBuilderOptions _noOptions = new();

    private readonly StringBuilderOptions _wrapping = new()
    {
        PrefixText = "[",
        SuffixText = "]",
        IndentLevel = 3,
        IndentType = IndentType.Spaces,
        IndentDepth = 4
    };

    private readonly StringBuilder _target = new();

    [Benchmark(Baseline = true)]
    public int NoOptions()
    {
        _target.Clear();
        _target.Append(TEXT, _noOptions);

        return _target.Length;
    }

    [Benchmark]
    public int PrefixSuffixAndIndent()
    {
        _target.Clear();
        _target.Append(TEXT, _wrapping);

        return _target.Length;
    }

    // What the same result costs when the pieces are measured once and written
    // straight into the final string
    [Benchmark]
    public int PrefixSuffixAndIndent_OnePass()
    {
        _target.Clear();
        _target.Append(Assemble(TEXT, _wrapping));

        return _target.Length;
    }

    private static string Assemble(string text, StringBuilderOptions options)
    {
        int indentWidth = options.IndentLevel <= 0
            ? 0
            : options.IndentType == IndentType.Tabs
                ? options.IndentLevel
                : options.IndentLevel * options.IndentDepth;

        string prefix = options.PrefixText ?? "";
        string suffix = options.SuffixText ?? "";

        int length = indentWidth + prefix.Length + text.Length + suffix.Length;

        char indentCharacter = options.IndentType == IndentType.Tabs ? '\t' : ' ';

        return string.Create(length, (text, prefix, suffix, indentWidth, indentCharacter),
            static (destination, state) =>
            {
                int position = 0;

                for (int i = 0; i < state.indentWidth; i++)
                {
                    destination[position++] = state.indentCharacter;
                }

                state.prefix.AsSpan().CopyTo(destination.Slice(position));
                position += state.prefix.Length;

                state.text.AsSpan().CopyTo(destination.Slice(position));
                position += state.text.Length;

                state.suffix.AsSpan().CopyTo(destination.Slice(position));
            });
    }
}
