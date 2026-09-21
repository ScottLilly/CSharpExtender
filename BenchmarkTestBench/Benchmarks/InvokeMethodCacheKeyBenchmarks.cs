using System.Reflection;
using BenchmarkDotNet.Attributes;
using CSharpExtender.Services;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85. InvokeMethod builds its cache key by interpolating the type name,
// method name, argument count and every argument type name into a string, with a
// LINQ Select and Join, on every call. Measures what that costs next to the
// reflection invoke it exists to avoid, before deciding whether a key that does
// not allocate is worth the extra type.
[ShortRunJob]
[MemoryDiagnoser]
public class InvokeMethodCacheKeyBenchmarks
{
    private readonly Calculator _calculator = new();
    private readonly object[] _arguments = [2, 3];

    [Benchmark(Baseline = true)]
    public int Current() => SmartReflection.InvokeMethod<int>(_calculator, "Add", 2, 3);

    [Benchmark]
    public string StringKeyOnly()
    {
        var arguments = _arguments;

        Type[]? argumentTypes =
            arguments.Length > 0 && arguments.All(p => p != null)
            ? arguments.Select(p => p.GetType()).ToArray()
            : null;

        return $"{_calculator.GetType().FullName}|Add|{arguments.Length}|" +
            (argumentTypes == null ? "?" : string.Join(",", argumentTypes.Select(t => t.FullName)));
    }

    [Benchmark]
    public MethodInfo? StructKeyLookup()
    {
        var arguments = _arguments;

        Type[]? argumentTypes = null;

        if (arguments.Length > 0)
        {
            argumentTypes = new Type[arguments.Length];

            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == null)
                {
                    argumentTypes = null;
                    break;
                }

                argumentTypes[i] = arguments[i].GetType();
            }
        }

        var key = new Key(_calculator.GetType(), "Add", arguments.Length, argumentTypes);

        return s_structCache.TryGetValue(key, out var method) ? method : null;
    }

    // The reflection invoke on its own, for scale
    [Benchmark]
    public object? InvokeOnly() => s_add.Invoke(_calculator, _arguments);

    private static readonly Dictionary<Key, MethodInfo> s_structCache = new()
    {
        [new Key(typeof(Calculator), "Add", 2, [typeof(int), typeof(int)])] =
            typeof(Calculator).GetMethod("Add", [typeof(int), typeof(int)])!
    };

    private readonly struct Key : IEquatable<Key>
    {
        private readonly Type _type;
        private readonly string _methodName;
        private readonly int _argumentCount;
        private readonly Type[]? _argumentTypes;

        public Key(Type type, string methodName, int argumentCount, Type[]? argumentTypes)
        {
            _type = type;
            _methodName = methodName;
            _argumentCount = argumentCount;
            _argumentTypes = argumentTypes;
        }

        public bool Equals(Key other)
        {
            if (_type != other._type || _methodName != other._methodName ||
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

        public override bool Equals(object? obj) => obj is Key other && Equals(other);

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

    private static readonly MethodInfo s_add =
        typeof(Calculator).GetMethod("Add", [typeof(int), typeof(int)])!;

    private class Calculator
    {
        public int Add(int first, int second) => first + second;
    }
}
