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

    private const string TraceDeserializedNullError = "Chrome trace document deserialized to null";
    private const string TraceContainsNoEventsError = "Chrome trace document contains no events";
    private const string TraceFileNotFoundError = "Chrome trace JSON file was not found.";
    private const string PhaseComplete = "X";
    private const string PhaseBegin = "B";
    private const string PhaseEnd = "E";
    private const string UnknownName = "unknown";

    /// <summary>
    /// Deserialize Chrome trace JSON from a string.
    /// </summary>
    /// <param name="json">The JSON string containing Chrome trace events.</param>
    /// <returns>Array of trace events.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is null or empty.</exception>
    public static ChromeTraceEvent[] Deserialize(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        ArgumentException.ThrowIfNullOrEmpty(json);
        var events = JsonSerializer.Deserialize<ChromeTraceEvent[]>(json, Options)
            ?? throw new FormatException(TraceDeserializedNullError);
        if (events.Length == 0)
            throw new FormatException(TraceContainsNoEventsError);
        return events;
    }

    /// <summary>
    /// Parse a Chrome trace JSON file into a FlameNode tree.
    /// </summary>
    /// <param name="path">Path to the Chrome trace JSON file.</param>
    /// <returns>A FlameNode tree with "root" as the root node containing all threads.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is null or empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file specified by <paramref name="path"/> does not exist.</exception>
    public static FlameNode ParseFile(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentException.ThrowIfNullOrEmpty(path);
        if (!File.Exists(path))
            throw new FileNotFoundException(TraceFileNotFoundError, path);

        using var stream = File.OpenRead(path);
        var events = JsonSerializer.Deserialize<ChromeTraceEvent[]>(stream, Options)
            ?? throw new FormatException(TraceDeserializedNullError);
        if (events.Length == 0)
            throw new FormatException(TraceContainsNoEventsError);
        return BuildTree(events);
    }

    /// <summary>
    /// Parse Chrome trace events into a FlameNode tree.
    /// </summary>
    /// <param name="events">Array of Chrome trace events.</param>
    /// <returns>A FlameNode tree with "root" as the root node containing all threads.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="events"/> is null.</exception>
    public static FlameNode BuildTree(ChromeTraceEvent[] events)
    {
        ArgumentNullException.ThrowIfNull(events);

        // Group events by thread ID
        var eventsByThread = events
            .Where(e => e.Tid.HasValue)
            .GroupBy(e => e.Tid!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        var root = new FlameNode("root");

        // Build a subtree for each thread; threads with no usable events get no node
        foreach (var threadGroup in eventsByThread)
        {
            var threadNode = root.AddChild($"thread {threadGroup.Key}", null, null);
            BuildThreadTree(threadGroup.Value, threadNode);
            if (threadNode.Children.Count == 0)
            {
                root.Children.Remove(threadNode);
            }
            else
            {
                root.Value += threadNode.Value;
            }
        }

        return root;
    }

    /// <summary>
    /// Builds the call tree for a single thread under <paramref name="threadNode"/> by processing
    /// complete (<c>X</c>) events and begin/end (<c>B</c>/<c>E</c>) pairs.
    /// </summary>
    /// <param name="events">Events for a single thread. Sorted by timestamp in place.</param>
    /// <param name="threadNode">The node that receives the top-level frames of this thread.</param>
    private static void BuildThreadTree(List<ChromeTraceEvent> events, FlameNode threadNode)
    {
        // Sort events by timestamp
        events.Sort((a, b) => a.Ts.CompareTo(b.Ts));

        // Open frames, each with the timestamp it began at so an E event can derive its duration
        var stack = new Stack<(FlameNode Node, double StartTs)>();
        stack.Push((threadNode, 0));

        foreach (var ev in events)
        {
            switch (ev.Ph)
            {
                case PhaseComplete:
                    // A complete event is self-contained: attribute its duration and leave the stack untouched
                    var completeNode = stack.Peek().Node.AddChild(ev.Name ?? UnknownName, ev.File, ev.Line);
                    completeNode.Value += ev.Dur ?? 0;
                    break;

                case PhaseBegin:
                    var beginNode = stack.Peek().Node.AddChild(ev.Name ?? UnknownName, ev.File, ev.Line);
                    stack.Push((beginNode, ev.Ts));
                    break;

                case PhaseEnd when stack.Count > 1:
                    var (endNode, startTs) = stack.Pop();
                    endNode.Value += ev.Ts - startTs;
                    break;

                // Other phases and unmatched end events are skipped
            }
        }

        threadNode.Value = threadNode.Children.Sum(c => c.Value);
    }
}

/// <summary>
/// Represents a Chrome trace event.
/// Based on the Chrome Trace Event Format specification.
/// </summary>
public sealed class ChromeTraceEvent
{
    /// <summary>
    /// Gets or sets the event phase, such as <c>X</c> for complete, <c>B</c> for begin,
    /// or <c>E</c> for end.
    /// </summary>
    [JsonPropertyName("ph")]
    public string? Ph { get; set; }

    /// <summary>
    /// Gets or sets the name of the event or function being profiled.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the timestamp in microseconds.
    /// </summary>
    [JsonPropertyName("ts")]
    public double Ts { get; set; }

    /// <summary>
    /// Gets or sets the duration in microseconds for a complete event.
    /// </summary>
    [JsonPropertyName("dur")]
    public double? Dur { get; set; }

    /// <summary>
    /// Gets or sets the thread identifier.
    /// </summary>
    [JsonPropertyName("tid")]
    public int? Tid { get; set; }

    /// <summary>
    /// Gets or sets the process identifier.
    /// </summary>
    [JsonPropertyName("pid")]
    public int? Pid { get; set; }

    /// <summary>
    /// Gets or sets the source file associated with the event.
    /// </summary>
    [JsonPropertyName("file")]
    public string? File { get; set; }

    /// <summary>
    /// Gets or sets the source line number associated with the event.
    /// </summary>
    [JsonPropertyName("line")]
    public int? Line { get; set; }

    /// <summary>
    /// Gets or sets the event category.
    /// </summary>
    [JsonPropertyName("cat")]
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the event arguments and attributes.
    /// </summary>
    [JsonPropertyName("args")]
    public Dictionary<string, object>? Args { get; set; }
}
