using System.Text.Json;
using System.Text.Json.Serialization;
using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Core.Parsing;

/// <summary>
/// Parser for Chrome trace-event format JSON files.
/// Chrome trace events have the following structure:
/// { "name": "function_name", "ph": "X", "ts": 123456, "dur": 789, "tid": 1 }
///
/// The ph field indicates the event type:
/// - X: Complete event (has both start and duration)
/// - B: Begin event (start only)
/// - E: End event (end only)
/// - Other phases are supported but not commonly used in profiling
///
/// This parser builds a FlameNode tree by tracking nested call stacks based on
    /// timestamp and duration containment.
/// </summary>
public static class ChromeTraceParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Deserialize Chrome trace JSON from a string.
    /// </summary>
    /// <param name="json">The JSON string containing Chrome trace events.</param>
    /// <returns>Array of trace events.</returns>
    public static ChromeTraceEvent[] Deserialize(string json)
    {
        var events = JsonSerializer.Deserialize<ChromeTraceEvent[]>(json, Options)
            ?? throw new FormatException("Chrome trace document deserialized to null");
        if (events.Length == 0)
            throw new FormatException("Chrome trace document contains no events");
        return events;
    }

    /// <summary>
    /// Parse a Chrome trace JSON file into a FlameNode tree.
    /// </summary>
    /// <param name="path">Path to the Chrome trace JSON file.</param>
    /// <returns>A FlameNode tree with "root" as the root node containing all threads.</returns>
    public static FlameNode ParseFile(string path)
    {
        using var stream = File.OpenRead(path);
        var events = JsonSerializer.Deserialize<ChromeTraceEvent[]>(stream, Options)
            ?? throw new FormatException("Chrome trace document deserialized to null");
        if (events.Length == 0)
            throw new FormatException("Chrome trace document contains no events");
        return BuildTree(events);
    }

    /// <summary>
    /// Parse Chrome trace events into a FlameNode tree.
    /// </summary>
    /// <param name="events">Array of Chrome trace events.</param>
    /// <returns>A FlameNode tree with "root" as the root node containing all threads.</returns>
    public static FlameNode BuildTree(ChromeTraceEvent[] events)
    {
        // Group events by thread ID
        var eventsByThread = events
            .Where(e => e.Tid.HasValue)
            .GroupBy(e => e.Tid!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        var root = new FlameNode("root");

        // Build a tree for each thread
        foreach (var threadGroup in eventsByThread)
        {
            var threadRoot = BuildThreadTree(threadGroup.Value);
            if (threadRoot.Children.Count > 0)
            {
                var threadNode = root.AddChild($"thread {threadGroup.Key}", null, null);
                threadNode.Value += threadRoot.Value;
            }
        }

        return root;
    }

    /// <summary>
    /// Build a flame graph tree for a single thread by processing complete events.
    /// </summary>
    /// <param name="events">Events for a single thread.</param>
    /// <returns>A FlameNode tree representing the call stack for this thread.</returns>
    private static FlameNode BuildThreadTree(List<ChromeTraceEvent> events)
    {
        // Sort events by timestamp
        events.Sort((a, b) => a.Ts.CompareTo(b.Ts));

        var root = new FlameNode("root");
        var stack = new Stack<FlameNode>();
        stack.Push(root);

        foreach (var ev in events)
        {
            if (ev.Ph != "X" && ev.Ph != "B" && ev.Ph != "E")
            {
                // Skip non-complete/begin/end events
                continue;
            }

            if (ev.Ph == "X" || ev.Ph == "B")
            {
                // Begin event or Complete event - push new frame onto stack
                // Use the node returned by AddChild to ensure we're working with the same instance
                var frameNode = stack.Peek().AddChild(ev.Name ?? "unknown", ev.File, ev.Line);
                stack.Push(frameNode);
            }

            if (ev.Ph == "X" || ev.Ph == "E")
            {
                // Complete event or End event - pop frame from stack
                // For complete events, attribute the duration to the frame
                if (ev.Ph == "X" && ev.Dur.HasValue && stack.Count > 1)
                {
                    var frameNode = stack.Pop();
                    frameNode.Value = ev.Dur.Value;
                }
                else if (ev.Ph == "E" && stack.Count > 1)
                {
                    stack.Pop();
                }
            }
        }

        return root;
    }
}

/// <summary>
/// Represents a Chrome trace event.
/// Based on the Chrome Trace Event Format specification.
/// </summary>
public sealed class ChromeTraceEvent
{
    /// <summary>Event type/phase (X=Complete, B=Begin, E=End, etc.)</summary>
    [JsonPropertyName("ph")]
    public string? Ph { get; set; }

    /// <summary>Event name/function being profiled</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Timestamp in microseconds</summary>
    [JsonPropertyName("ts")]
    public double Ts { get; set; }

    /// <summary>Duration in microseconds (for complete events)</summary>
    [JsonPropertyName("dur")]
    public double? Dur { get; set; }

    /// <summary>Thread ID</summary>
    [JsonPropertyName("tid")]
    public int? Tid { get; set; }

    /// <summary>Process ID</summary>
    [JsonPropertyName("pid")]
    public int? Pid { get; set; }

    /// <summary>Optional: Source file</summary>
    [JsonPropertyName("file")]
    public string? File { get; set; }

    /// <summary>Optional: Line number</summary>
    [JsonPropertyName("line")]
    public int? Line { get; set; }

    /// <summary>Optional: Category</summary>
    [JsonPropertyName("cat")]
    public string? Category { get; set; }

    /// <summary>Optional: Arguments/attributes</summary>
    [JsonPropertyName("args")]
    public Dictionary<string, object>? Args { get; set; }
}