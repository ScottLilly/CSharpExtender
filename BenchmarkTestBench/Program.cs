using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CSharpExtender.ExtensionMethods;

// Select assemblies to benchmark
BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args);

// Create a method in the MyBenchmarks class for each benchmark to perform

// Uncomment one the following lines to run the benchmarks in faster mode
//[MediumRunJob]
[ShortRunJob]
[MemoryDiagnoser]
public class MyBenchmarks
{
    #region Constants for parameters

    private const string LONG_NUMERIC_STRING =
        @"
1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111";

    #endregion

    [Params("1234567890", "1234567890a", "1234567890!@#$%^&*()_+", LONG_NUMERIC_STRING)]
    public string _testString = "";

    [GlobalSetup]
    public void Setup()
    {
        // Setup code here
    }

    //[Benchmark]
    public void Benchmark_IsDigitOnly()
    {
        var test = _testString.IsDigitsOnly();
    }
}
