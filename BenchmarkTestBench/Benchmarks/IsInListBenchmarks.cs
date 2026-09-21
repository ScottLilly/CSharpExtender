using System.ComponentModel.DataAnnotations;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpExtender.DataAnnotations;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85 follow-up. IsInListAttribute tests its allowed values with
// Any(allowed => ...), and the predicate captures the value being validated and
// the attribute, so every validation allocates a closure and a delegate.
[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class IsInListBenchmarks
{
    private readonly Order _matchesLast = new() { Status = "Cancelled" };
    private readonly Order _noMatch = new() { Status = "Unknown" };

    // Held rather than built per call, because an attribute is constructed once by
    // the validation framework and the params array would otherwise be measured too
    private readonly IsInListAttribute _attribute =
        new("New", "Paid", "Shipped", "Cancelled");

    private ValidationContext _context = null!;

    [GlobalSetup]
    public void Setup() => _context = new ValidationContext(_matchesLast);

    [Benchmark(Baseline = true)]
    public ValidationResult? Matching() =>
        _attribute.GetValidationResult(_matchesLast.Status, _context);

    [Benchmark]
    public ValidationResult? NotMatching() =>
        _attribute.GetValidationResult(_noMatch.Status, _context);

    private class Order
    {
        public string Status { get; set; } = "";
    }
}
