using BenchmarkDotNet.Attributes;
using CSharpExtender.ExtensionMethods;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BenchmarkTestBench.Benchmarks;

// Issue #78. The shipped DeepClone builds a JsonSerializerOptions per call, which
// throws away the serializer metadata cache, and round-trips through a string.
[ShortRunJob]
[MemoryDiagnoser]
public class DeepCloneBenchmarks
{
    public class Address
    {
        public string Street { get; set; } = "1 Example Way";
        public string City { get; set; } = "Springfield";
        public string PostalCode { get; set; } = "12345";
    }

    public class Person
    {
        public string Name { get; set; } = "Scott Lilly";
        public int Age { get; set; } = 40;
        public Address Home { get; set; } = new();
        public List<string> Nicknames { get; set; } = ["Scott", "S"];
    }

    private static readonly JsonSerializerOptions _sharedOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve
    };

    private readonly Person _person = new();

    [Benchmark(Baseline = true)]
    public Person? Current() => _person.DeepClone();

    [Benchmark]
    public Person? SharedOptionsViaString()
    {
        string json = JsonSerializer.Serialize(_person, _sharedOptions);

        return JsonSerializer.Deserialize<Person>(json, _sharedOptions);
    }

    [Benchmark]
    public Person? SharedOptionsViaUtf8Bytes()
    {
        byte[] json = JsonSerializer.SerializeToUtf8Bytes(_person, _sharedOptions);

        return JsonSerializer.Deserialize<Person>(json, _sharedOptions);
    }
}
