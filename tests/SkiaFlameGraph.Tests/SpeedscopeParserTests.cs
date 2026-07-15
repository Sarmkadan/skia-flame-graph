using SkiaFlameGraph.Core.Parsing;
using Xunit;

namespace SkiaFlameGraph.Tests;

public class SpeedscopeParserTests
{
    private const string SampledJson = """
    {
      "shared": { "frames": [
        { "name": "a" }, { "name": "b" }, { "name": "c" }
      ]},
      "profiles": [{
        "type": "sampled",
        "unit": "milliseconds",
        "startValue": 0, "endValue": 10,
        "samples": [ [0, 1], [0, 1], [0, 2] ],
        "weights": [3, 2, 5]
      }]
    }
    """;

    [Fact]
    public void SampledProfile_AggregatesWeightsUpTheStack()
    {
        var file = SpeedscopeParser.Deserialize(SampledJson);
        var root = SpeedscopeParser.BuildTree(file);

        Assert.Equal(10, root.Value);          // 3 + 2 + 5
        var a = Assert.Single(root.Children);
        Assert.Equal("a", a.Name);
        Assert.Equal(10, a.Value);

        // a has two children: b (3+2) and c (5)
        Assert.Equal(2, a.Children.Count);
        var b = a.Children.Find(n => n.Name == "b")!;
        var c = a.Children.Find(n => n.Name == "c")!;
        Assert.Equal(5, b.Value);
        Assert.Equal(5, c.Value);
    }

    [Fact]
    public void RecursiveFrames_AreMergedIntoOneBox()
    {
        const string json = """
        {
          "shared": { "frames": [ { "name": "loop" } ] },
          "profiles": [{
            "type": "sampled", "unit": "none", "startValue": 0, "endValue": 3,
            "samples": [ [0], [0, 0], [0, 0, 0] ],
            "weights": [1, 1, 1]
          }]
        }
        """;

        var root = SpeedscopeParser.BuildTree(SpeedscopeParser.Deserialize(json));

        // Every sample starts with the same frame, so the top level collapses
        // to a single child rather than three sibling boxes.
        Assert.Single(root.Children);
    }

    [Fact]
    public void EventedProfile_AttributesElapsedTime()
    {
        const string json = """
        {
          "shared": { "frames": [ { "name": "main" }, { "name": "work" } ] },
          "profiles": [{
            "type": "evented", "unit": "milliseconds", "startValue": 0, "endValue": 10,
            "events": [
              { "type": "O", "frame": 0, "at": 0 },
              { "type": "O", "frame": 1, "at": 2 },
              { "type": "C", "frame": 1, "at": 8 },
              { "type": "C", "frame": 0, "at": 10 }
            ]
          }]
        }
        """;

        var root = SpeedscopeParser.BuildTree(SpeedscopeParser.Deserialize(json));
        var main = Assert.Single(root.Children);
        Assert.Equal("main", main.Name);
        Assert.Equal(10, main.Value);
        var work = Assert.Single(main.Children);
        Assert.Equal(6, work.Value); // 8 - 2
    }

    [Fact]
    public void EmptyDocument_Throws()
    {
        const string json = """{ "shared": { "frames": [] }, "profiles": [] }""";
        Assert.ThrowsAny<System.Exception>(() => SpeedscopeParser.Deserialize(json));
    }
}
