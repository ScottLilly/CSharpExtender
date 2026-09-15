using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CSharpExtender.DataAnnotations;

/// <summary>
/// A one pass check for whether a collection holds any duplicates, for
/// <see cref="UniqueItemsAttribute"/> to use before it walks every pair.
/// </summary>
/// <remarks>
/// Comparing every pair is quadratic. Putting the items in a set answers the same
/// question in one pass, but only tells the caller whether a duplicate exists, not
/// where it is, and the attribute's message names the two indexes. So this is a
/// filter: a true answer ends the validation, and a false one sends the caller to
/// the pairwise walk, which is the path that stops early anyway.
/// </remarks>
internal static class UniqueItemsHashCheck
{
    // Where the set starts to cost less than comparing every pair, and where the
    // saving starts to be worth the set's own allocation. Measured on strings: the
    // two are level at 12 items, the set is 1.35x ahead at 16, and 6.6x at 64.
    // See UniqueItemsHashThresholdBenchmarks.
    private const int _minimumItemCount = 16;

    private static readonly ConcurrentDictionary<Type, bool> s_dependableHashCode =
        new ConcurrentDictionary<Type, bool>();

    /// <summary>
    /// True only when every item is of a type this can judge, and no two of them
    /// are equal. False means "walk the pairs": either there is a duplicate, or
    /// the collection holds something this cannot rule on.
    /// </summary>
    internal static bool IsProvablyUnique(List<object?> items)
    {
        if (items.Count < _minimumItemCount)
        {
            return false;
        }

        var seen = new HashSet<object>(items.Count);

        for (int i = 0; i < items.Count; i++)
        {
            object? item = items[i];

            if (!CanBeJudgedByHashCode(item))
            {
                return false;
            }

            if (!seen.Add(item))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Whether putting this item in a set reaches the same answer as comparing it
    /// against every other item would.
    /// </summary>
    /// <remarks>
    /// Two things have to hold. The item's type has to be one the attribute judges
    /// by Equals in the first place, which is a value type or a string: a class is
    /// judged property by property instead, and its own Equals may say something
    /// different. And its Equals and GetHashCode have to agree, because a set finds
    /// a duplicate by hash first, so a type whose equal values hash differently
    /// would have its duplicates go unseen. Passing invalid data is worse than
    /// validating it slowly, so anything this cannot establish goes to the pairwise
    /// walk that the attribute has always used.
    /// A null is excluded for tidiness rather than any real risk: it keeps the one
    /// value a set treats specially out of the fast path.
    /// </remarks>
    private static bool CanBeJudgedByHashCode([NotNullWhen(true)] object? item)
    {
        if (item == null)
        {
            return false;
        }

        var type = item.GetType();

        // The attribute compares strings by Equals, so a set agrees with it
        if (type == typeof(string))
        {
            return true;
        }

        // A class is compared property by property, which is not what its own
        // Equals and GetHashCode describe, so a set would answer a different
        // question rather than the same question faster
        if (!type.IsValueType)
        {
            return false;
        }

        return s_dependableHashCode.GetOrAdd(type, HonorsTheHashCodeContract);
    }

    /// <summary>
    /// Whether a value type overrides Equals and GetHashCode together.
    /// </summary>
    /// <remarks>
    /// Overriding both is the contract, and a type that has done the work is taken
    /// at its word. Overriding neither is equally safe: ValueType compares field by
    /// field and hashes from the same field data, so values it calls equal hash
    /// alike. One without the other is the case that cannot be trusted, in either
    /// direction, because the two are then describing different things.
    /// A type that overrides both and still disagrees with itself is beyond
    /// detecting, and is already broken everywhere else a Dictionary or HashSet
    /// touches it.
    /// </remarks>
    private static bool HonorsTheHashCodeContract(Type type) =>
        Overrides(type, nameof(Equals), new[] { typeof(object) }) ==
        Overrides(type, nameof(GetHashCode), Type.EmptyTypes);

    private static bool Overrides(Type type, string methodName, Type[] parameterTypes)
    {
        var method = type.GetMethod(methodName,
            BindingFlags.Public | BindingFlags.Instance, null, parameterTypes, null);

        // Still declared by the type it was inherited from means not overridden.
        // Enum and the primitives declare their own, which is what makes them
        // usable here.
        return method != null
            && method.DeclaringType != typeof(object)
            && method.DeclaringType != typeof(ValueType);
    }
}
