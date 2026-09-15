using BenchmarkDotNet.Attributes;
using CSharpExtender.ExtensionMethods;
using System.Text.RegularExpressions;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85. SplitPascalCase calls the static Regex.Replace, which looks the
// pattern up in the process-wide regex cache and then runs it interpreted.
// Compares that against a source-generated regex, which .NET Standard 2.1 could
// not use, and against holding a compiled instance in a static field.
[ShortRunJob]
[MemoryDiagnoser]
public partial class SplitPascalCaseBenchmarks
{
    private const string PATTERN = @"(?<!^)(?<![\W_])(?=[A-Z])";

    private static readonly Regex s_compiled =
        new Regex(PATTERN, RegexOptions.Compiled);

    [GeneratedRegex(PATTERN)]
    private static partial Regex Generated();

    [Params("Id", "CustomerFirstName", "TheQuickBrownFoxJumpedOverTheLazyDogAndKeptOnRunning")]
    public string Text = "";

    [Benchmark(Baseline = true)]
    public List<string> Current() => Text.SplitPascalCase();

    [Benchmark]
    public List<string> StaticCompiledRegex()
    {
        if (string.IsNullOrEmpty(Text))
        {
            return new List<string>();
        }

        return s_compiled.Replace(Text, " ").Split(' ').ToList();
    }

    [Benchmark]
    public List<string> GeneratedRegex()
    {
        if (string.IsNullOrEmpty(Text))
        {
            return new List<string>();
        }

        return Generated().Replace(Text, " ").Split(' ').ToList();
    }
}
