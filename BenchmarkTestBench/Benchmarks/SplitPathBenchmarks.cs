using BenchmarkDotNet.Attributes;
using CSharpExtender.ExtensionMethods;

namespace BenchmarkTestBench.Benchmarks;

// Issue #83. The shipped SplitPath allocates a separator array per call, then a
// LINQ iterator and a second string for every segment it trims.
[ShortRunJob]
[MemoryDiagnoser]
public class SplitPathBenchmarks
{
    private static readonly char[] _separators = ['/', '\\'];

    [Params(
        @"docs\notes.md",
        @"D:\Development\OpenSource\NuGetPackages\CSharpExtender\CSharpExtender\ExtensionMethods",
        @" home / user \ projects / csharp \ extender ")]
    public string Path = "";

    [Benchmark(Baseline = true)]
    public int Current() => CountOf(Path.SplitPath());

    [Benchmark]
    public int StaticSeparatorsOnly()
    {
        return CountOf(Path.Split(_separators, StringSplitOptions.RemoveEmptyEntries)
                           .Select(s => s.Trim()));
    }

    // StringSplitOptions.TrimEntries was not available on .NET Standard 2.1, which
    // is what the method's comment is about. The library targets net8.0 now.
    [Benchmark]
    public int SplitWithTrimEntries()
    {
        return CountOf(Path.Split(_separators,
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }

    [Benchmark]
    public int SpanWalk()
    {
        var segments = new List<string>();
        var source = Path.AsSpan();
        int start = 0;

        for (int i = 0; i <= source.Length; i++)
        {
            if (i < source.Length && source[i] != '/' && source[i] != '\\')
            {
                continue;
            }

            var segment = source.Slice(start, i - start).Trim();

            if (!segment.IsEmpty)
            {
                segments.Add(new string(segment));
            }

            start = i + 1;
        }

        return CountOf(segments);
    }

    private static int CountOf(IEnumerable<string> segments)
    {
        int count = 0;

        foreach (var segment in segments)
        {
            count += segment.Length;
        }

        return count;
    }
}
