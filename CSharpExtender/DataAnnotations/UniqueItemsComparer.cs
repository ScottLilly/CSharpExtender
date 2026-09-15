using CSharpExtender.Services;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSharpExtender.DataAnnotations;

/// <summary>
/// Compares the items of one collection for <see cref="UniqueItemsAttribute"/>,
/// remembering each item's property values as it reads them.
/// </summary>
/// <remarks>
/// Every item is compared against every other one. A property is read the first
/// time a comparison reaches it and remembered after that, so a list costs one read
/// per property actually compared, not one per pair.
/// A comparison stops at the first property that differs, so the values are filled
/// in only as deep as they are asked for.
/// An instance holds the state of a single validation call. It is not shared and
/// not thread-safe. The PropertyInfo cache is static, and is.
/// </remarks>
internal sealed class UniqueItemsComparer
{
    private readonly List<object> _items;

    // Both allocated on the first comparison that walks properties, so a collection
    // of strings or numbers never pays for them
    private object[][] _values;
    private int[] _readCount;

    internal UniqueItemsComparer(List<object> items)
    {
        _items = items;
    }

    /// <summary>
    /// Whether the two items are duplicates: the same type, with every public
    /// instance property equal.
    /// </summary>
    internal bool AreItemsEqual(int first, int second)
    {
        object item1 = _items[first];
        object item2 = _items[second];

        if (item1 == null && item2 == null)
        {
            return true;
        }

        if (item1 == null || item2 == null)
        {
            return false;
        }

        // Items of different types are never duplicates. This also keeps the
        // property walk below from reading item1's PropertyInfo off an item2
        // that does not have that property, which throws TargetException.
        var itemType = item1.GetType();

        if (itemType != item2.GetType())
        {
            return false;
        }

        // Handle simple types (e.g., string, int)
        if (itemType.IsValueType || item1 is string)
        {
            return item1.Equals(item2);
        }

        var properties = PropertyCache.GetProperties(itemType);

        if (properties.Length == 0)
        {
            return true;
        }

        _values = _values ?? new object[_items.Count][];
        _readCount = _readCount ?? new int[_items.Count];

        for (int i = 0; i < properties.Length; i++)
        {
            object value1 = ValueAt(properties, first, i);
            object value2 = ValueAt(properties, second, i);

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

    private object ValueAt(PropertyInfo[] properties, int index, int propertyIndex)
    {
        object[] cached = _values[index];

        if (cached == null)
        {
            cached = new object[properties.Length];
            _values[index] = cached;
        }

        // Properties are always walked in order, so filling up to the one being
        // asked for leaves the count as the number that have been read
        for (int i = _readCount[index]; i <= propertyIndex; i++)
        {
            cached[i] = properties[i].GetValue(_items[index]);
        }

        if (_readCount[index] <= propertyIndex)
        {
            _readCount[index] = propertyIndex + 1;
        }

        return cached[propertyIndex];
    }
}
