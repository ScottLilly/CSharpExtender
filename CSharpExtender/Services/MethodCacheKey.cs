using System;

namespace CSharpExtender.Services;

/// <summary>
/// Identifies a method for <see cref="SmartReflection"/>'s method cache: the type
/// declaring it, its name, and the runtime types of the arguments a call supplied.
/// </summary>
/// <remarks>
/// A struct, and compared field by field, so a lookup allocates nothing beyond the
/// array of argument types. The cache was keyed on a string built by interpolating
/// those same parts together, which cost several times what the reflection call it
/// was there to avoid cost.
/// </remarks>
internal readonly struct MethodCacheKey : IEquatable<MethodCacheKey>
{
    private readonly Type _type;
    private readonly string _methodName;
    private readonly int _argumentCount;

    // Null when the arguments could not all be typed, which is a different key from
    // any list of types
    private readonly Type[] _argumentTypes;

    internal MethodCacheKey(Type type, string methodName, int argumentCount, Type[] argumentTypes)
    {
        _type = type;
        _methodName = methodName;
        _argumentCount = argumentCount;
        _argumentTypes = argumentTypes;
    }

    public bool Equals(MethodCacheKey other)
    {
        if (_type != other._type ||
            _methodName != other._methodName ||
            _argumentCount != other._argumentCount)
        {
            return false;
        }

        if (_argumentTypes == null || other._argumentTypes == null)
        {
            return _argumentTypes == other._argumentTypes;
        }

        for (int i = 0; i < _argumentTypes.Length; i++)
        {
            if (_argumentTypes[i] != other._argumentTypes[i])
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object obj) =>
        obj is MethodCacheKey other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(_type);
        hash.Add(_methodName);
        hash.Add(_argumentCount);

        if (_argumentTypes != null)
        {
            for (int i = 0; i < _argumentTypes.Length; i++)
            {
                hash.Add(_argumentTypes[i]);
            }
        }

        return hash.ToHashCode();
    }
}
