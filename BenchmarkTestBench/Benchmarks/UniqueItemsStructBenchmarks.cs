using System.Collections;
using System.ComponentModel.DataAnnotations;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpExtender.DataAnnotations;

namespace BenchmarkTestBench.Benchmarks;

// Issue #93. Shows that the one pass check tells a user defined struct that honors
// the Equals and GetHashCode contract from one that does not. The trusted struct
// should take the set, the untrusted one should fall to the pairwise walk, and the
// gap between them is the proof. Both answer the same question; only the cost
// differs.
[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class UniqueItemsStructBenchmarks
{
    private readonly UniqueItemsAttribute _attribute = new();

    private IList _items = new List<int>();
    private ValidationContext _context = null!;

    [Params(200)]
    public int ItemCount;

    // "trusted" overrides both, "untrusted" overrides Equals only, "neither"
    // overrides nothing and leans on ValueType
    [Params("trusted", "untrusted", "neither")]
    public string StructKind = "";

    [GlobalSetup]
    public void Setup()
    {
        _items = StructKind switch
        {
            "trusted" => Enumerable.Range(0, ItemCount).Select(i => new Trusted(i)).ToList(),
            "untrusted" => Enumerable.Range(0, ItemCount).Select(i => new Untrusted(i)).ToList(),
            _ => Enumerable.Range(0, ItemCount).Select(i => new Neither(i)).ToList()
        };

        _context = new ValidationContext(new object());
    }

    [Benchmark]
    public ValidationResult? Validate() => _attribute.GetValidationResult(_items, _context);

    private readonly struct Trusted(int id) : IEquatable<Trusted>
    {
        public int Id { get; } = id;

        public bool Equals(Trusted other) => other.Id == Id;

        public override bool Equals(object? obj) => obj is Trusted other && Equals(other);

        public override int GetHashCode() => Id.GetHashCode();
    }

#pragma warning disable CS0659
    private readonly struct Untrusted(int id)
    {
        public int Id { get; } = id;

        public override bool Equals(object? obj) => obj is Untrusted other && other.Id == Id;
    }
#pragma warning restore CS0659

    private readonly struct Neither(int id)
    {
        public int Id { get; } = id;
    }
}
