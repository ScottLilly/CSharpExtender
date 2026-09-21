using System.Collections.Concurrent;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using CSharpExtender.Services;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85. SmartReflection finds a property with FirstOrDefault(p => p.Name ==
// propertyName). The predicate captures propertyName, so every lookup allocates a
// closure and a delegate on top of the LINQ iterator, in the class whose whole
// point is to make repeated reflection cheap. Compares that against a for loop
// over the same cached array.
[ShortRunJob]
[MemoryDiagnoser]
public class SmartReflectionLookupBenchmarks
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> s_cache = new();

    private readonly Person _person = new() { FirstName = "Scott", LastName = "Lilly", Age = 50 };

    [Benchmark(Baseline = true)]
    public string? Current() => SmartReflection.GetPropertyValue<string>(_person, "LastName");

    [Benchmark]
    public PropertyInfo? FirstOrDefaultLookup()
    {
        return GetCachedProperties(_person.GetType())
            .FirstOrDefault(p => p.Name == "LastName");
    }

    [Benchmark]
    public PropertyInfo? LoopLookup()
    {
        var properties = GetCachedProperties(_person.GetType());

        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].Name == "LastName")
            {
                return properties[i];
            }
        }

        return null;
    }

    private static PropertyInfo[] GetCachedProperties(Type type) =>
        s_cache.GetOrAdd(type,
            t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));

    private class Person
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public int Age { get; set; }
    }
}
