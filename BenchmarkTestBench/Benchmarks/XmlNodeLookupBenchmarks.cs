using System.Xml;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpExtender.ExtensionMethods;

namespace BenchmarkTestBench.Benchmarks;

// Issue #85 follow-up. ElementAsString and ElementAsInt reach their child through
// SelectSingleNode, which parses the name as an XPath expression on every call.
// For a plain element name a walk of the child nodes answers the same question.
[SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 5, iterationCount: 10)]
[MemoryDiagnoser]
public class XmlNodeLookupBenchmarks
{
    private XmlNode _person = null!;

    [GlobalSetup]
    public void Setup()
    {
        var document = new XmlDocument();

        document.LoadXml(
            "<person id=\"42\" active=\"true\">" +
            "<firstName>Scott</firstName>" +
            "<lastName>Lilly</lastName>" +
            "<city>Charlotte</city>" +
            "<state>NC</state>" +
            "<postalCode>28202</postalCode>" +
            "<age>50</age>" +
            "</person>");

        _person = document.DocumentElement!;
    }

    [Benchmark(Baseline = true)]
    public string? ElementAsString_Current() => _person.ElementAsString("age");

    [Benchmark]
    public string? ElementAsString_ChildWalk() => FirstChildNamed(_person, "age")?.InnerText;

    [Benchmark]
    public int ElementAsInt_Current() => _person.ElementAsInt("age");

    [Benchmark]
    public int ElementAsInt_ChildWalk()
    {
        var child = FirstChildNamed(_person, "age");

        return child != null && int.TryParse(child.InnerText, out int result) ? result : default;
    }

    // A missing name is the worst case for the walk, because it visits every child
    [Benchmark]
    public string? MissingElement_Current() => _person.ElementAsString("nothing");

    [Benchmark]
    public string? MissingElement_ChildWalk() => FirstChildNamed(_person, "nothing")?.InnerText;

    [Benchmark]
    public int AttributeAsInt_Current() => _person.AttributeAsInt("id");

    private static XmlNode? FirstChildNamed(XmlNode node, string name)
    {
        for (XmlNode? child = node.FirstChild; child != null; child = child.NextSibling)
        {
            if (child.NodeType == XmlNodeType.Element && child.Name == name)
            {
                return child;
            }
        }

        return null;
    }
}
