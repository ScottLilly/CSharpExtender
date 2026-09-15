using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpExtender.Models;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85 follow-up. ObservableModel.SetProperty compared through the static
// object.Equals, which boxed both sides for a value-typed property on every set.
// EqualityComparer replaced it and the boxing is gone.
//
// The candidate below also holds its PropertyChangedEventArgs per property name,
// which was measured and NOT taken. It removes the 24 byte allocation on a raise
// but the dictionary lookup costs more than the allocation did: with a subscriber,
// 9.41ns and nothing allocated against 5.78ns and 24 bytes. A caller passing a
// name it had computed would also have grown the cache without limit. Kept here as
// the record of why the shipped class builds its args each time.
[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class ObservableModelBenchmarks
{
    private readonly ShippedModel _shipped = new();
    private readonly CandidateModel _candidate = new();
    private readonly ShippedModel _shippedWatched = new();
    private readonly CandidateModel _candidateWatched = new();

    private int _counter;

    [GlobalSetup]
    public void Setup()
    {
        _shippedWatched.PropertyChanged += (_, _) => { };
        _candidateWatched.PropertyChanged += (_, _) => { };
    }

    // A changing value, so the set goes all the way through and raises
    [Benchmark(Baseline = true)]
    public int Shipped_Changing()
    {
        _shipped.Count = ++_counter;

        return _shipped.Count;
    }

    [Benchmark]
    public int Candidate_Changing()
    {
        _candidate.Count = ++_counter;

        return _candidate.Count;
    }

    // The same value again, so the comparison decides nothing has changed. This is
    // the path that boxes for nothing.
    [Benchmark]
    public int Shipped_Unchanged()
    {
        _shipped.Count = 7;

        return _shipped.Count;
    }

    [Benchmark]
    public int Candidate_Unchanged()
    {
        _candidate.Count = 7;

        return _candidate.Count;
    }

    [Benchmark]
    public int Shipped_ChangingWithSubscriber()
    {
        _shippedWatched.Count = ++_counter;

        return _shippedWatched.Count;
    }

    [Benchmark]
    public int Candidate_ChangingWithSubscriber()
    {
        _candidateWatched.Count = ++_counter;

        return _candidateWatched.Count;
    }

    private class ShippedModel : ObservableModel
    {
        private int _count;

        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }
    }

    // ObservableModel as it could be: a comparison that does not box, and event
    // args held per property name
    private class CandidateModel : INotifyPropertyChanged
    {
        private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> s_eventArgs =
            new();

        private int _count;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        private bool SetProperty<T>(ref T backingField, T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;

            PropertyChanged?.Invoke(this,
                s_eventArgs.GetOrAdd(propertyName ?? "", static n => new PropertyChangedEventArgs(n)));

            return true;
        }
    }
}
