using System.Data;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpExtender.ExtensionMethods;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85 follow-up. Two small repeats of work that never changes.
// PrettyPrintJson(string) builds a JsonSerializerOptions per call, which throws
// away the converter and metadata cache System.Text.Json keeps on the instance,
// the same thing issue #78 fixed for DeepClone. DataRow.Get looks the column name
// up twice, once to ask whether it exists and once to read it.
[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class PrettyPrintAndDataRowBenchmarks
{
    private const string JSON =
        """{"name":"Scott","age":50,"address":{"city":"Charlotte","state":"NC"},"tags":["a","b"]}""";

    private static readonly JsonSerializerOptions s_indented = new() { WriteIndented = true };

    private DataRow _row = null!;

    [GlobalSetup]
    public void Setup()
    {
        var table = new DataTable();

        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Created", typeof(DateTime));
        table.Columns.Add("Balance", typeof(decimal));

        _row = table.Rows.Add(1, "Scott", DateTime.Today, 10m);
    }

    [Benchmark(Baseline = true)]
    public string PrettyPrint_Current() => JSON.PrettyPrintJson();

    [Benchmark]
    public string PrettyPrint_SharedOptions()
    {
        using JsonDocument document = JsonDocument.Parse(JSON);

        return JsonSerializer.Serialize(document, s_indented);
    }

    [Benchmark]
    public string? DataRowGet_Current() => _row.Get<string>("Name");

    [Benchmark]
    public string DataRowGet_SingleLookup() => GetByIndex<string>(_row, "Name");

    [Benchmark]
    public string? DataRowGet_MissingColumn_Current() => _row.Get<string>("Nothing");

    [Benchmark]
    public string DataRowGet_MissingColumn_SingleLookup() => GetByIndex<string>(_row, "Nothing");

    private static T GetByIndex<T>(DataRow row, string columnName)
    {
        if (row == null)
        {
            return default!;
        }

        int index = row.Table.Columns.IndexOf(columnName);

        if (index < 0)
        {
            return default!;
        }

        object value = row[index];

        if (value == DBNull.Value)
        {
            return default!;
        }

        if (value is T typedValue)
        {
            return typedValue;
        }

        return (T)Convert.ChangeType(value, typeof(T));
    }
}
