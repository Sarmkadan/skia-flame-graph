# ChromeTraceParser

The `ChromeTraceParser` class provides functionality to deserialize Chrome trace event data (as produced by Chrome's tracing infrastructure, e.g., `chrome://tracing`) and to construct a flame graph tree from those events. It supports both direct deserialization of JSON strings and file-based parsing, with the ability to build a hierarchical `FlameNode` structure suitable for flame graph rendering.

## API

### Static Methods

#### `public static ChromeTraceEvent[] Deserialize(string json)`

Deserializes a JSON string containing an array of Chrome trace events into an array of `ChromeTraceEvent` objects.

- **Parameters**  
  `json` – A string containing valid JSON representing a Chrome trace event array (typically the `traceEvents` array from a Chrome trace file).

- **Returns**  
  An array of `ChromeTraceEvent` instances. Returns an empty array if the input JSON is an empty array.

- **Throws**  
  - `System.ArgumentNullException` if `json` is `null`.  
  - `System.Text.Json.JsonException` if the JSON is malformed or does not match the expected structure.

#### `public static FlameNode ParseFile(string path)`

Reads a Chrome trace file from disk, deserializes its events, and builds a flame graph tree from them.

- **Parameters**  
  `path` – The file path to a Chrome trace JSON file.

- **Returns**  
  A `FlameNode` representing the root of the flame graph tree constructed from all events in the file.

- **Throws**  
  - `System.ArgumentNullException` if `path` is `null`.  
  - `System.IO.FileNotFoundException` if the file does not exist.  
  - `System.Text.Json.JsonException` if the file content is not valid JSON or does not conform to the expected trace event schema.

#### `public static FlameNode BuildTree(ChromeTraceEvent[] events)`

Constructs a flame graph tree from an array of `ChromeTraceEvent` objects.

- **Parameters**  
  `events` – An array of `ChromeTraceEvent` instances, typically obtained from `Deserialize`.

- **Returns**  
  A `FlameNode` that is the root of the flame graph tree. Events are aggregated by call stack (using `Ph`, `Name`, `Ts`, `Dur`, `Tid`, `Pid`, etc.) to form parent-child relationships.

- **Throws**  
  - `System.ArgumentNullException` if `events` is `null`.

### Instance Properties

Each `ChromeTraceEvent` instance exposes the following properties, which correspond to fields in the Chrome trace event format.

| Property | Type | Description |
|----------|------|-------------|
| `Ph` | `string?` | The phase of the event (e.g., `"B"` for begin, `"E"` for end, `"X"` for complete). |
| `Name` | `string?` | The name of the event (e.g., function or operation name). |
| `Ts` | `double` | The timestamp of the event in microseconds. |
| `Dur` | `double?` | The duration of the event in microseconds (typically present for complete events). |
| `Tid` | `int?` | The thread ID on which the event occurred. |
| `Pid` | `int?` | The process ID in which the event occurred. |
| `File` | `string?` | The source file name associated with the event (if available). |
| `Line` | `int?` | The source line number associated with the event (if available). |
| `Category` | `string?` | The category of the event (e.g., `"blink"`, `"v8"`). |
| `Args` | `Dictionary<string, object>?` | Additional arguments attached to the event, as a dictionary of key-value pairs. |

All properties are read-only after deserialization.

## Usage

### Example 1: Parse a trace file and build a flame graph

```csharp
using SkiaFlameGraph;

string traceFilePath = "trace.json";
FlameNode root = ChromeTraceParser.ParseFile(traceFilePath);

// The root node can now be used for rendering or further analysis.
Console.WriteLine($"Flame graph root: {root.Name}, total samples: {root.TotalSamples}");
```

### Example 2: Deserialize from a JSON string and manually build the tree

```csharp
using SkiaFlameGraph;
using System.Text.Json;

string json = File.ReadAllText("trace.json");
ChromeTraceEvent[] events = ChromeTraceParser.Deserialize(json);

// Filter events if needed, then build the tree.
FlameNode root = ChromeTraceParser.BuildTree(events);

// Access event properties for debugging.
foreach (var evt in events.Take(5))
{
    Console.WriteLine($"Event: {evt.Name} (phase: {evt.Ph}, ts: {evt.Ts})");
}
```

## Notes

- **Edge Cases**  
  - Events with missing or `null` phase (`Ph`) are skipped during tree construction.  
  - Incomplete event pairs (e.g., a begin event without a matching end) may produce unexpected tree shapes; the parser attempts to handle them gracefully by ignoring orphaned events.  
  - Empty event arrays result in a `FlameNode` with no children and zero samples.  
  - The `Args` dictionary may contain nested objects; values are deserialized as `System.Text.Json.JsonElement` instances and can be cast to the expected type.

- **Thread Safety**  
  - The static methods `Deserialize`, `ParseFile`, and `BuildTree` are thread-safe and can be called concurrently from multiple threads.  
  - Instances of `ChromeTraceEvent` are immutable after construction and can be safely shared across threads.  
  - The `FlameNode` returned by `BuildTree` or `ParseFile` is mutable; concurrent modification of the tree is not thread-safe and should be synchronized externally.
