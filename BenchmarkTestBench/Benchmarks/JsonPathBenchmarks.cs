using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpExtender.ExtensionMethods;
using System.Text.Json;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85 follow-up. GetValueFromJsonPath splits the path on "." into an array
// of strings before walking it, and JsonElement can be asked for a property by
// span, so none of those strings have to exist.
[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class JsonPathBenchmarks
{
    private const string JSON =
        """
        {
          "order": {
            "customer": { "name": "Scott", "id": 42 },
            "totals": { "net": 100, "tax": 7, "gross": 107 }
          }
        }
        """;

    [Params("order.customer.name", "order.totals.gross")]
    public string Path = "";

    [Benchmark(Baseline = true)]
    public string? Current() => JSON.GetValueFromJsonPath(Path);

    [Benchmark]
    public string? SpanWalk()
    {
        using JsonDocument document = JsonDocument.Parse(JSON);

        JsonElement root = document.RootElement;

        ReadOnlySpan<char> remaining = Path.AsSpan();

        while (true)
        {
            int separator = remaining.IndexOf('.');

            ReadOnlySpan<char> segment =
                separator < 0 ? remaining : remaining.Slice(0, separator);

            if (root.ValueKind != JsonValueKind.Object ||
                !root.TryGetProperty(segment, out var value))
            {
                throw new InvalidOperationException(
                    $"Property '{new string(segment)}' not found in JSON path '{Path}'.");
            }

            root = value;

            if (separator < 0)
            {
                break;
            }

            remaining = remaining.Slice(separator + 1);
        }

        return root.ToString().ConvertFromString<string>();
    }
}
