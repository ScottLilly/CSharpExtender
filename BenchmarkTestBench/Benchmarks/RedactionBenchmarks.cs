using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpExtender.Services;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85 follow-up. Both redaction services build the path to the node they are
// looking at by interpolating a new string at every level, for every node, whether
// or not anything matches. Issue #82 measured the constructors and left them
// alone; this is the walk, which runs per document rather than per service.
[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class RedactionBenchmarks
{
    private static readonly List<string> s_patterns =
        ["ssn", "password", "cardNumber", "secret"];

    private JsonRedactionService _jsonService = null!;
    private XmlRedactionService _xmlService = null!;

    // Constructed with no patterns, so the walk returns immediately and what is
    // left is the parse and the serialize. The gap between these and the ones
    // above is what the walk itself costs.
    private JsonRedactionService _jsonNoPatterns = null!;
    private XmlRedactionService _xmlNoPatterns = null!;

    private string _json = "";
    private string _xml = "";

    [Params(10, 100)]
    public int RecordCount;

    [GlobalSetup]
    public void Setup()
    {
        _jsonService = new JsonRedactionService(s_patterns);
        _xmlService = new XmlRedactionService(s_patterns);
        _jsonNoPatterns = new JsonRedactionService([]);
        _xmlNoPatterns = new XmlRedactionService([]);
        _json = BuildJson(RecordCount);
        _xml = BuildXml(RecordCount);
    }

    [Benchmark]
    public string RedactJson() => _jsonService.RedactToString(_json);

    [Benchmark]
    public string RedactXml() => _xmlService.RedactToString(_xml);

    [Benchmark]
    public string JsonParseAndSerializeOnly() => _jsonNoPatterns.RedactToString(_json);

    [Benchmark]
    public string XmlParseAndSerializeOnly() => _xmlNoPatterns.RedactToString(_xml);

    // A document with nesting, so paths get long enough for the cost of building
    // them to show
    private static string BuildJson(int recordCount)
    {
        var root = new JsonObject();
        var people = new JsonArray();

        for (int i = 0; i < recordCount; i++)
        {
            people.Add(new JsonObject
            {
                ["id"] = i,
                ["name"] = $"person-{i}",
                ["ssn"] = "123-45-6789",
                ["contact"] = new JsonObject
                {
                    ["email"] = $"person{i}@example.com",
                    ["phone"] = "555-867-5309"
                },
                ["account"] = new JsonObject
                {
                    ["password"] = "hunter2",
                    ["cardNumber"] = "4111111111111111",
                    ["balance"] = i * 10
                }
            });
        }

        root["people"] = people;
        root["generated"] = "2026-09-15";

        return root.ToString();
    }

    private static string BuildXml(int recordCount)
    {
        var builder = new StringBuilder("<root>");

        for (int i = 0; i < recordCount; i++)
        {
            builder.Append($"<person id=\"{i}\">")
                .Append($"<name>person-{i}</name>")
                .Append("<ssn>123-45-6789</ssn>")
                .Append("<contact><email>a@example.com</email><phone>555</phone></contact>")
                .Append("<account><password>hunter2</password><balance>10</balance></account>")
                .Append("</person>");
        }

        builder.Append("</root>");

        var document = new XmlDocument();
        document.LoadXml(builder.ToString());

        return document.OuterXml;
    }
}
