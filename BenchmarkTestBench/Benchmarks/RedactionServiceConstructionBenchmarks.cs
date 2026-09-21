using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using CSharpExtender.ExtensionMethods;
using CSharpExtender.Services;

namespace BenchmarkTestBench.Benchmarks;

// Issue #82. The shipped constructors leave the pattern sequence lazy and then
// enumerate it three times, re-running Where and Distinct on each pass.
[ShortRunJob]
[MemoryDiagnoser]
public class RedactionServiceConstructionBenchmarks
{
    private string[] _patterns = [];

    [Params(3, 50)]
    public int PatternCount;

    [GlobalSetup]
    public void Setup()
    {
        _patterns = Enumerable.Range(0, PatternCount)
            .Select(i => $"field{i}")
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public CompositeRegexMatcher Current() => new(_patterns);

    [Benchmark]
    public Regex? MaterializedOnce()
    {
        var patterns = _patterns?.Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList()
            ?? [];

        if (patterns.Count == 0)
        {
            return null;
        }

        var combinedPattern = string.Join("|", patterns.Select(p => $"(?:{p})"));

        return new Regex(combinedPattern,
            RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2));
    }

    // Separates the cost of the LINQ passes from the cost of compiling the regex,
    // which is expected to dominate
    [Benchmark]
    public string ThreePassLinqOnly()
    {
        IEnumerable<string> patterns =
            _patterns?.Where(p => !string.IsNullOrEmpty(p)).Distinct() ?? [];

        bool isEmpty = patterns.None() || patterns.All(string.IsNullOrEmpty);

        return isEmpty ? "" : string.Join("|", patterns.Select(p => $"(?:{p})"));
    }

    [Benchmark]
    public string OnePassLinqOnly()
    {
        var patterns = _patterns?.Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList()
            ?? [];

        return patterns.Count == 0 ? "" : string.Join("|", patterns.Select(p => $"(?:{p})"));
    }
}
