using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace BenchmarkTestBench.Benchmarks;

// Issue #107. CompositeRegexMatcher.MatchesAny is a non-virtual method on a class that
// is not sealed, and both redaction services call it once per node path. This measures
// whether sealing the class buys anything, over documents of four sizes, where a
// document's size is the number of paths its walk checks.
//
// The copies below are the real class cut down to the one method, so that sealed and
// not sealed can be measured in the same run. Virtual is the control: it is the same
// code again with the method made virtual and called through the base type, which is
// the only shape sealing could ever fix. If all three measure the same, the benchmark
// is not sensitive enough to answer the question and the result means nothing.
[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class SealedMatcherBenchmarks
{
    private static readonly string[] s_patterns =
        ["ssn", "password", "cardNumber", "secret"];

    private string[] _paths = [];
    private Regex _regex = null!;
    private UnsealedMatcher _unsealedMatcher = null!;
    private SealedMatcher _sealedMatcher = null!;
    private MatcherBase _virtualMatcher = null!;

    [Params(10, 100, 1000, 10000)]
    public int PathCount;

    [GlobalSetup]
    public void Setup()
    {
        _paths = BuildPaths(PathCount);
        _regex = BuildRegex();
        _unsealedMatcher = new UnsealedMatcher();
        _sealedMatcher = new SealedMatcher();
        _virtualMatcher = new VirtualMatcher();
    }

    [Benchmark(Baseline = true)]
    public int RegexDirect()
    {
        int hits = 0;

        for (int i = 0; i < _paths.Length; i++)
        {
            if (_regex.IsMatch(_paths[i]))
            {
                hits++;
            }
        }

        return hits;
    }

    [Benchmark]
    public int ThroughUnsealedClass()
    {
        int hits = 0;

        for (int i = 0; i < _paths.Length; i++)
        {
            if (_unsealedMatcher.MatchesAny(_paths[i]))
            {
                hits++;
            }
        }

        return hits;
    }

    [Benchmark]
    public int ThroughSealedClass()
    {
        int hits = 0;

        for (int i = 0; i < _paths.Length; i++)
        {
            if (_sealedMatcher.MatchesAny(_paths[i]))
            {
                hits++;
            }
        }

        return hits;
    }

    [Benchmark]
    public int ThroughVirtualCall()
    {
        int hits = 0;

        for (int i = 0; i < _paths.Length; i++)
        {
            if (_virtualMatcher.MatchesAny(_paths[i]))
            {
                hits++;
            }
        }

        return hits;
    }

    // The paths a redaction walk actually tests: mostly misses, with the four patterns
    // turning up on roughly one node in five, and long enough to be realistic
    private static string[] BuildPaths(int pathCount)
    {
        string[] leaves =
            ["id", "name", "contact.email", "contact.phone", "account.balance",
             "account.password", "ssn", "account.cardNumber", "meta.created", "meta.source"];

        var paths = new string[pathCount];

        for (int i = 0; i < pathCount; i++)
        {
            paths[i] = $"people[{i}].{leaves[i % leaves.Length]}";
        }

        return paths;
    }

    private static Regex BuildRegex() =>
        new Regex(string.Join("|", s_patterns.Select(p => $"(?:{p})")),
            RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2));

    private class UnsealedMatcher
    {
        private readonly Regex? _combinedRegex;

        public UnsealedMatcher()
        {
            _combinedRegex = BuildRegex();
        }

        public bool MatchesAny(string input) =>
            _combinedRegex != null && _combinedRegex.IsMatch(input);
    }

    private sealed class SealedMatcher
    {
        private readonly Regex? _combinedRegex;

        public SealedMatcher()
        {
            _combinedRegex = BuildRegex();
        }

        public bool MatchesAny(string input) =>
            _combinedRegex != null && _combinedRegex.IsMatch(input);
    }

    private abstract class MatcherBase
    {
        public abstract bool MatchesAny(string input);
    }

    private class VirtualMatcher : MatcherBase
    {
        private readonly Regex? _combinedRegex;

        public VirtualMatcher()
        {
            _combinedRegex = BuildRegex();
        }

        public override bool MatchesAny(string input) =>
            _combinedRegex != null && _combinedRegex.IsMatch(input);
    }
}
