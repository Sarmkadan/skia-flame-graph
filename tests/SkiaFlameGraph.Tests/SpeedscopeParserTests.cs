using SkiaFlameGraph.Core.Parsing;
using Xunit;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Contains unit tests for the <see cref="SpeedscopeParser"/> class.
/// These tests verify the correct parsing and tree construction behavior
/// for different types of speedscope profiles (sampled and evented).
/// </summary>
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

    /// <summary>
    /// Tests that sampled profiles correctly aggregate weights up the call stack.
    /// When multiple frames are on the stack, their weights should be summed
    /// to determine the total weight for each node in the flame graph tree.
    /// </summary>
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

    /// <summary>
    /// Tests that recursive frames are merged into a single box in the flame graph.
    /// When the same frame appears consecutively in a call stack, it should be
    /// collapsed into a single box rather than creating multiple sibling boxes.
    /// </summary>
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

    /// <summary>
    /// Tests that evented profiles correctly attribute elapsed time to call frames.
    /// For evented profiles, the time between open and close events should be
    /// calculated and assigned to the corresponding frame in the flame graph.
    /// </summary>
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

    /// <summary>
    /// Tests that an empty document throws an exception during deserialization.
    /// When both shared frames and profiles are empty, the parser should throw
    /// an exception as this represents an invalid speedscope file.
    /// </summary>
    [Fact]
    public void EmptyDocument_Throws()
    {
        const string json = """{ "shared": { "frames": [] }, "profiles": [] }""";
        Assert.ThrowsAny<System.Exception>(() => SpeedscopeParser.Deserialize(json));
    }
}
