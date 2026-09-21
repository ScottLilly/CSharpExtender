using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpExtender.DataAnnotations;
using CSharpExtender.ExtensionMethods;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85 follow-up. MaskExtensionMethods and DisplayFormatExtensionMethods read
// a type's properties and every property's attribute on every call, while
// SmartReflection next door caches the same lookups. A type's properties and the
// attributes on them cannot change at runtime, so the answer is the same every
// time it is worked out.
[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class AttributeLookupBenchmarks
{
    private readonly Customer _customer = new();

    [Benchmark(Baseline = true)]
    public IDictionary<string, string> ToMaskedStrings_Current() => _customer.ToMaskedStrings();

    [Benchmark]
    public IDictionary<string, string> ToMaskedStrings_Cached()
    {
        var masked = new Dictionary<string, string>();

        foreach (var entry in MaskedPropertiesOf(_customer.GetType()))
        {
            masked[entry.Property.Name] =
                MaskValue(entry.Property.GetValue(_customer), entry.Attribute);
        }

        return masked;
    }

    [Benchmark]
    public string ToMaskedString_Current() => _customer.ToMaskedString(nameof(Customer.Ssn));

    [Benchmark]
    public string ToMaskedString_Cached()
    {
        var entry = PropertyOf(_customer.GetType(), nameof(Customer.Ssn));

        return MaskValue(entry.Property.GetValue(_customer), entry.Attribute);
    }

    [Benchmark]
    public IDictionary<string, string> ToDisplayStrings_Current() => _customer.ToDisplayStrings();

    private static readonly ConcurrentDictionary<Type, MaskedProperty[]> s_maskedProperties = new();

    private static readonly ConcurrentDictionary<(Type, string), MaskedProperty> s_byName = new();

    private static MaskedProperty[] MaskedPropertiesOf(Type type) =>
        s_maskedProperties.GetOrAdd(type, static t => t
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => new MaskedProperty(p, p.GetCustomAttribute<MaskAttribute>()))
            .Where(e => e.Attribute != null)
            .ToArray());

    private static MaskedProperty PropertyOf(Type type, string propertyName) =>
        s_byName.GetOrAdd((type, propertyName), static key =>
        {
            var property = key.Item1.GetProperty(key.Item2)
                ?? throw new ArgumentException($"Property {key.Item2} not found");

            return new MaskedProperty(property, property.GetCustomAttribute<MaskAttribute>());
        });

    private static string MaskValue(object? value, MaskAttribute? attribute)
    {
        string text = value?.ToString() ?? string.Empty;

        return attribute == null
            ? text
            : text.Mask(attribute.MaskChar, attribute.VisiblePrefixLength,
                attribute.VisibleSuffixLength, attribute.PreserveSeparators);
    }

    private readonly record struct MaskedProperty(PropertyInfo Property, MaskAttribute? Attribute);

    // Ten properties, three of them carrying an attribute, which is the shape these
    // methods are pointed at: a model with a few sensitive or formatted fields
    private class Customer
    {
        public int Id { get; set; } = 1;
        public string FirstName { get; set; } = "Scott";
        public string LastName { get; set; } = "Lilly";

        [Mask(VisibleSuffixLength = 4)]
        public string Ssn { get; set; } = "123-45-6789";

        [Mask(VisiblePrefixLength = 4, VisibleSuffixLength = 4)]
        public string CardNumber { get; set; } = "1234-5678-9012-5678";

        [Mask(VisibleSuffixLength = 4)]
        public string Phone { get; set; } = "(555) 867-5309";

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Created { get; set; } = new DateTime(2026, 1, 1);

        [DisplayFormat(DataFormatString = "C")]
        public decimal Balance { get; set; } = 1234.56m;

        public bool IsActive { get; set; } = true;
        public string Notes { get; set; } = "none";
    }
}
