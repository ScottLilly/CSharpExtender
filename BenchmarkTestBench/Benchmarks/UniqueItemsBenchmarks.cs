using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using CSharpExtender.DataAnnotations;

namespace BenchmarkTestBench.Benchmarks;

// Issue #93. UniqueItemsAttribute compares every pair of items, and compares each
// pair property by property through reflection, so an item's properties are read
// again on every comparison it takes part in. A list with no duplicates is the
// worst case, because nothing short-circuits the scan.
//
// A MediumRunJob rather than the ShortRunJob the other classes use: three
// iterations put the error bars wider than the differences being measured.
//
// The baseline is CurrentAlgorithm, which is the shipped comparison lifted out of
// the attribute, so the candidates are compared against the same shape. Current is
// the real attribute, and the gap between the two is the ValidationAttribute
// plumbing, which none of the candidates can change.
[MediumRunJob]
[MemoryDiagnoser]
public class UniqueItemsBenchmarks
{
    private readonly UniqueItemsAttribute _attribute = new();

    private IList _items = new List<string>();
    private ValidationContext _context = null!;

    [Params(10, 200)]
    public int ItemCount;

    // "similar" objects share every property but the last, so no comparison
    // short-circuits early. "objects" differ on the first property, which is what
    // makes the shipped version look cheaper than it is.
    [Params("strings", "objects", "similar")]
    public string ItemKind = "";

    [GlobalSetup]
    public void Setup()
    {
        _items = ItemKind switch
        {
            "strings" => Enumerable.Range(0, ItemCount).Select(i => $"item-{i}").ToList(),
            "objects" => Enumerable.Range(0, ItemCount).Select(i => new WideItem(i)).ToList(),
            _ => Enumerable.Range(0, ItemCount).Select(i => new WideItem(0, i)).ToList()
        };

        _context = new ValidationContext(new object());
    }

    [Benchmark]
    public ValidationResult? Current() => _attribute.GetValidationResult(_items, _context);

    [Benchmark(Baseline = true)]
    public bool CurrentAlgorithm() => HasDuplicate_Pairwise(_items);

    [Benchmark]
    public bool PerItemCachedValues() => HasDuplicate_PerItemCached(_items);

    [Benchmark]
    public bool LazyCachedValues() => HasDuplicate_LazyCached(_items);

    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> s_propertyCache = new();

    private static PropertyInfo[] PropertiesOf(Type type) =>
        s_propertyCache.GetOrAdd(type,
            t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));

    #region The shipped algorithm, lifted out of the attribute

    private static bool HasDuplicate_Pairwise(IEnumerable collection)
    {
        var items = collection.Cast<object>().ToList();

        for (int i = 0; i < items.Count - 1; i++)
        {
            for (int j = i + 1; j < items.Count; j++)
            {
                if (AreItemsEqual(items[i], items[j]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool AreItemsEqual(object? item1, object? item2)
    {
        if (item1 == null && item2 == null)
        {
            return true;
        }

        if (item1 == null || item2 == null)
        {
            return false;
        }

        var itemType = item1.GetType();

        if (itemType != item2.GetType())
        {
            return false;
        }

        if (itemType.IsValueType || item1 is string)
        {
            return item1.Equals(item2);
        }

        foreach (var prop in PropertiesOf(itemType))
        {
            var value1 = prop.GetValue(item1);
            var value2 = prop.GetValue(item2);

            if (value1 == null && value2 == null)
            {
                continue;
            }

            if (value1 == null || value2 == null)
            {
                return false;
            }

            if (!value1.Equals(value2))
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region Candidate: read an item's properties once, the first time it is compared

    private static bool HasDuplicate_PerItemCached(IEnumerable collection)
    {
        var items = collection.Cast<object>().ToList();
        var comparer = new PerItemComparer(items);

        for (int i = 0; i < items.Count - 1; i++)
        {
            for (int j = i + 1; j < items.Count; j++)
            {
                if (comparer.AreItemsEqual(i, j))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private sealed class PerItemComparer(List<object> items)
    {
        // Allocated on the first comparison that walks properties, so a list of
        // strings or numbers never pays for it
        private object?[][]? _values;

        public bool AreItemsEqual(int first, int second)
        {
            object? item1 = items[first];
            object? item2 = items[second];

            if (item1 == null && item2 == null)
            {
                return true;
            }

            if (item1 == null || item2 == null)
            {
                return false;
            }

            var itemType = item1.GetType();

            if (itemType != item2.GetType())
            {
                return false;
            }

            if (itemType.IsValueType || item1 is string)
            {
                return item1.Equals(item2);
            }

            var properties = PropertiesOf(itemType);

            if (properties.Length == 0)
            {
                return true;
            }

            _values ??= new object?[items.Count][];

            var left = ValuesOf(properties, first);
            var right = ValuesOf(properties, second);

            for (int p = 0; p < left.Length; p++)
            {
                if (left[p] == null && right[p] == null)
                {
                    continue;
                }

                if (left[p] == null || right[p] == null)
                {
                    return false;
                }

                if (!left[p]!.Equals(right[p]))
                {
                    return false;
                }
            }

            return true;
        }

        private object?[] ValuesOf(PropertyInfo[] properties, int index)
        {
            var cached = _values![index];

            if (cached != null)
            {
                return cached;
            }

            cached = new object?[properties.Length];

            for (int p = 0; p < properties.Length; p++)
            {
                cached[p] = properties[p].GetValue(items[index]);
            }

            _values[index] = cached;

            return cached;
        }
    }

    #endregion

    #region Candidate: read each property the first time a comparison reaches it

    private static bool HasDuplicate_LazyCached(IEnumerable collection)
    {
        var items = collection.Cast<object>().ToList();
        var comparer = new LazyComparer(items);

        for (int i = 0; i < items.Count - 1; i++)
        {
            for (int j = i + 1; j < items.Count; j++)
            {
                if (comparer.AreItemsEqual(i, j))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private sealed class LazyComparer(List<object> items)
    {
        private object?[][]? _values;
        private int[]? _readCount;

        public bool AreItemsEqual(int first, int second)
        {
            object? item1 = items[first];
            object? item2 = items[second];

            if (item1 == null && item2 == null)
            {
                return true;
            }

            if (item1 == null || item2 == null)
            {
                return false;
            }

            var itemType = item1.GetType();

            if (itemType != item2.GetType())
            {
                return false;
            }

            if (itemType.IsValueType || item1 is string)
            {
                return item1.Equals(item2);
            }

            var properties = PropertiesOf(itemType);

            if (properties.Length == 0)
            {
                return true;
            }

            _values ??= new object?[items.Count][];
            _readCount ??= new int[items.Count];

            for (int p = 0; p < properties.Length; p++)
            {
                object? value1 = ValueAt(properties, first, p);
                object? value2 = ValueAt(properties, second, p);

                if (value1 == null && value2 == null)
                {
                    continue;
                }

                if (value1 == null || value2 == null)
                {
                    return false;
                }

                if (!value1.Equals(value2))
                {
                    return false;
                }
            }

            return true;
        }

        private object? ValueAt(PropertyInfo[] properties, int index, int propertyIndex)
        {
            var cached = _values![index] ??= new object?[properties.Length];

            // Properties are always walked in order, so filling up to the one asked
            // for leaves the count as the number read
            for (int p = _readCount![index]; p <= propertyIndex; p++)
            {
                cached[p] = properties[p].GetValue(items[index]);
            }

            if (_readCount[index] <= propertyIndex)
            {
                _readCount[index] = propertyIndex + 1;
            }

            return cached[propertyIndex];
        }
    }

    #endregion

    // Ten properties, so the per-comparison reflection cost is visible
    private class WideItem(int seed, int tail = 0)
    {
        public int Id { get; set; } = seed;
        public string Name { get; set; } = $"name-{seed}";
        public string Description { get; set; } = $"description-{seed}";
        public decimal Amount { get; set; } = seed;
        public DateTime Created { get; set; } = new DateTime(2026, 1, 1).AddDays(seed);
        public bool IsActive { get; set; } = seed % 2 == 0;
        public Guid Reference { get; set; } = Guid.Empty;
        public int? Optional { get; set; } = seed;
        public string Category { get; set; } = $"category-{seed % 7}";

        // The only property that differs when every item is built with the same seed
        public double Weight { get; set; } = tail * 1.5;
    }
}
