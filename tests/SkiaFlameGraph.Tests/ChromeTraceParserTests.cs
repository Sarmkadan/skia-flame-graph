using SkiaFlameGraph.Core.Parsing;
using Xunit;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Contains unit tests for the <see cref="ChromeTraceParser"/> class.
/// These tests verify the correct parsing and tree construction behavior
/// for Chrome trace event format.
/// </summary>
public class ChromeTraceParserTests
{
    private const string SimpleCompleteEventsJson = """
    [
      { "name": "main", "ph": "X", "ts": 0, "dur": 100, "tid": 1 },
      { "name": "work", "ph": "X", "ts": 10, "dur": 50, "tid": 1 },
      { "name": "helper", "ph": "X", "ts": 20, "dur": 30, "tid": 1 }
    ]
    """;

    private const string NestedEventsJson = """
    [
      { "name": "main", "ph": "B", "ts": 0, "tid": 1 },
      { "name": "a", "ph": "B", "ts": 5, "tid": 1 },
      { "name": "b", "ph": "X", "ts": 10, "dur": 20, "tid": 1 },
      { "name": "a", "ph": "E", "ts": 30, "tid": 1 },
      { "name": "main", "ph": "E", "ts": 40, "tid": 1 }
    ]
    """;

    private const string MultipleThreadsJson = """
    [
      { "name": "thread1-work", "ph": "X", "ts": 0, "dur": 50, "tid": 1 },
      { "name": "thread2-work", "ph": "X", "ts": 0, "dur": 30, "tid": 2 }
    ]
    """;

    private const string EventsWithArgsJson = """
    [
      { "name": "func", "ph": "X", "ts": 0, "dur": 100, "tid": 1, "args": { "arg1": "value1" } }
    ]
    """;

    /// <summary>
    /// Tests that simple complete events are parsed correctly.
    /// </summary>
    [Fact]
    public void SimpleCompleteEvents_AreParsedCorrectly()
    {
        var events = ChromeTraceParser.Deserialize(SimpleCompleteEventsJson);
        Assert.Equal(3, events.Length);
        Assert.Equal("main", events[0].Name);
        Assert.Equal("X", events[0].Ph);
        Assert.Equal(0, events[0].Ts);
        Assert.Equal(100, events[0].Dur);
        Assert.Equal(1, events[0].Tid);
    }

    /// <summary>
    /// Tests that complete events with file and line information are parsed correctly.
    /// </summary>
    [Fact]
    public void CompleteEvents_WithFileAndLine_AreParsedCorrectly()
    {
        const string json = """
        [
          { "name": "func", "ph": "X", "ts": 0, "dur": 100, "tid": 1, "file": "test.cs", "line": 42 }
        ]
        """;

        var events = ChromeTraceParser.Deserialize(json);
        Assert.Single(events);
        Assert.Equal("test.cs", events[0].File);
        Assert.Equal(42, events[0].Line);
    }

    /// <summary>
    /// Tests that nested begin/end events build a proper call tree.
    /// </summary>
    [Fact]
    public void NestedBeginEndEvents_BuildProperCallTree()
    {
        var root = ChromeTraceParser.BuildTree(ChromeTraceParser.Deserialize(NestedEventsJson));

        // Should have one thread node
        Assert.Single(root.Children);
        var threadNode = root.Children[0];
        Assert.StartsWith("thread", threadNode.Name);

        // Thread should have main as child
        Assert.Single(threadNode.Children);
        var main = threadNode.Children[0];
        Assert.Equal("main", main.Name);
        Assert.Equal(40, main.Value); // Total duration

        // main should have 'a' as child
        Assert.Single(main.Children);
        var a = main.Children[0];
        Assert.Equal("a", a.Name);

        // 'a' should have 'b' as child
        Assert.Single(a.Children);
        var b = a.Children[0];
        Assert.Equal("b", b.Name);
        Assert.Equal(20, b.Value);
    }

    /// <summary>
    /// Tests that events from multiple threads are grouped correctly.
    /// </summary>
    [Fact]
    public void MultipleThreads_AreGroupedCorrectly()
    {
        var root = ChromeTraceParser.BuildTree(ChromeTraceParser.Deserialize(MultipleThreadsJson));

        // Should have two thread nodes
        Assert.Equal(2, root.Children.Count);

        var thread1 = root.Children.FirstOrDefault(c => c.Name.Contains("1"));
        var thread2 = root.Children.FirstOrDefault(c => c.Name.Contains("2"));

        Assert.NotNull(thread1);
        Assert.NotNull(thread2);
        Assert.Equal(50, thread1.Value);
        Assert.Equal(30, thread2.Value);
    }

    /// <summary>
    /// Tests that events without tid are ignored.
    /// </summary>
    [Fact]
    public void EventsWithoutTid_AreIgnored()
    {
        const string json = """
        [
          { "name": "func", "ph": "X", "ts": 0, "dur": 100 }
        ]
        """;

        var root = ChromeTraceParser.BuildTree(ChromeTraceParser.Deserialize(json));
        Assert.Empty(root.Children); // No thread nodes since no tid
    }

    /// <summary>
    /// Tests that non-complete/begin/end events are skipped.
    /// </summary>
    [Fact]
    public void NonCompleteBeginEndEvents_AreSkipped()
    {
        const string json = """
        [
          { "name": "func", "ph": "I", "ts": 0, "tid": 1 },
          { "name": "func2", "ph": "X", "ts": 10, "dur": 50, "tid": 1 }
        ]
        """;

        var root = ChromeTraceParser.BuildTree(ChromeTraceParser.Deserialize(json));
        Assert.Single(root.Children); // Only the valid complete event
    }

    /// <summary>
    /// Tests that events are sorted by timestamp during parsing.
    /// </summary>
    [Fact]
    public void Events_AreSortedByTimestamp()
    {
        const string unsortedJson = """
        [
          { "name": "late", "ph": "X", "ts": 100, "dur": 50, "tid": 1 },
          { "name": "early", "ph": "X", "ts": 10, "dur": 50, "tid": 1 }
        ]
        """;

        var root = ChromeTraceParser.BuildTree(ChromeTraceParser.Deserialize(unsortedJson));
        Assert.Single(root.Children);
        var threadNode = root.Children[0];
        Assert.Single(threadNode.Children);
        var func = threadNode.Children[0];
        Assert.Equal("early", func.Name);
    }

    /// <summary>
    /// Tests that empty events array throws an exception.
    /// </summary>
    [Fact]
    public void EmptyEventsArray_Throws()
    {
        const string emptyJson = "[]";
        Assert.ThrowsAny<System.Exception>(() => ChromeTraceParser.Deserialize(emptyJson));
    }

    /// <summary>
    /// Tests that null deserialization throws an exception.
    /// </summary>
    [Fact]
    public void NullDeserialization_Throws()
    {
        const string nullJson = "null";
        Assert.ThrowsAny<System.Exception>(() => ChromeTraceParser.Deserialize(nullJson));
    }
}
