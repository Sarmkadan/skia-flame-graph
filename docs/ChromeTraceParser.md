# Chrome Trace Parser Documentation

## Overview

The `ChromeTraceParser` class parses Chrome trace-event format JSON files and converts them into a flame graph data structure represented by a tree of `FlameNode` objects. This parser is used to visualize performance profiling data from Chrome's tracing system (`chrome://tracing`).

## Chrome Trace Event Format

The parser expects Chrome trace events in the following JSON format (as defined by the [Chrome Trace Event Format](https://docs.google.com/document/d/1CvAClvFfyA5R-PhYUmn5OOQtYMH4h6I0nSsKchNAySU/preview)):

```json
{
  "name": "function_name",
  "ph": "X",
  "ts": 123456,
  "dur": 789,
  "tid": 1,
  "pid": 1,
  "file": "source.cpp",
  "line": 42,
  "cat": "benchmark",
  "args": { "arg1": "value" }
}
```

### Key Fields

- **`name`** (string): The name of the event or function being profiled
- **`ph`** (string): The phase of the event:
  - `X`: Complete event (has both timestamp and duration)
  - `B`: Begin event (start only)
  - `E`: End event (end only)
  - Other phases (like `b`, `e`, `n`, `s`, `t`) are supported but skipped
- **`ts`** (number): Timestamp in microseconds
- **`dur`** (number): Duration in microseconds (for complete events)
- **`tid`** (number): Thread ID
- **`pid`** (number): Process ID
- **`file`** (string): Source file (optional)
- **`line`** (number): Source line number (optional)
- **`cat`** (string): Category (optional)
- **`args`** (object): Additional arguments (optional)

## Public API

### `Deserialize(string json)`

Deserializes a Chrome trace JSON string into an array of `ChromeTraceEvent` objects.

```csharp
public static ChromeTraceEvent[] Deserialize(string json)
```

**Parameters:**
- `json`: The JSON string containing Chrome trace events (typically the `traceEvents` array from a Chrome trace file)

**Returns:**
- Array of `ChromeTraceEvent` objects

**Exceptions:**
- `ArgumentNullException`: If `json` is null
- `ArgumentException`: If `json` is null or empty
- `FormatException`: If the JSON deserializes to null or contains no events

### `ParseFile(string path)`

Parses a Chrome trace JSON file from disk into a `FlameNode` tree.

```csharp
public static FlameNode ParseFile(string path)
```

**Parameters:**
- `path`: Path to the Chrome trace JSON file

**Returns:**
- A `FlameNode` tree with "root" as the root node containing all threads

**Exceptions:**
- `ArgumentNullException`: If `path` is null
- `ArgumentException`: If `path` is null or empty
- `FileNotFoundException`: If the file does not exist
- `FormatException`: If the JSON deserializes to null or contains no events

### `BuildTree(ChromeTraceEvent[] events)`

Builds a flame graph tree from an array of Chrome trace events.

```csharp
public static FlameNode BuildTree(ChromeTraceEvent[] events)
```

**Parameters:**
- `events`: Array of Chrome trace events (typically obtained from `Deserialize`)

**Returns:**
- A `FlameNode` tree with "root" as the root node containing all threads

**Exceptions:**
- `ArgumentNullException`: If `events` is null

## Parsing Algorithm

The parser converts Chrome trace events into a hierarchical flame graph through several steps:

### 1. Event Deserialization

JSON is deserialized into `ChromeTraceEvent[]` using `System.Text.Json` with case-insensitive property matching.

### 2. Thread Grouping

Events are grouped by thread ID (`tid`) to process each thread separately:

```csharp
var eventsByThread = events
    .Where(e => e.Tid.HasValue)
    .GroupBy(e => e.Tid!.Value)
    .ToDictionary(g => g.Key, g => g.ToList());
```

Events without a valid thread ID are ignored.

### 3. Per-Thread Tree Construction

For each thread's events:
1. **Sort by timestamp**: Events are sorted by `ts` (timestamp) in ascending order
2. **Stack-based tree building**: 
   - A stack tracks the current call path, starting with a root node
   - For each event:
     - **Begin/Complete events (`B` or `X`)**: 
       - Create a new node for the function name
       - Add it as a child of the current stack top
       - Push the new node onto the stack
     - **End/Complete events (`E` or `X`)**:
       - Pop the current node from the stack
       - For complete events with duration (`X` + `dur`), assign the duration to the node's `Value`

### 4. FlameNode Assembly

- A root node named "root" is created
- For each thread with events:
  - A thread node is created named "thread {threadId}"
  - The thread's call tree is attached as children
  - The thread node's `Value` accumulates the total time of its children

## FlameNode Structure

The resulting tree consists of `FlameNode` objects with:

- **`Name`**: Function name (or "unknown" if missing)
- **`Value`**: Time value in microseconds:
  - For complete events: the duration (`dur`)
  - For thread nodes: sum of children values
  - For root: sum of all thread values
- **`Children`**: Child nodes representing called functions
- **`File`/`Line`**: Source location (when available in the trace event)

## Error Handling

The parser validates inputs and throws meaningful exceptions:

- **Null/empty inputs**: `ArgumentNullException` or `ArgumentException`
- **Missing files**: `FileNotFoundException` with descriptive message
- **Invalid JSON**: `FormatException` for null deserialization or empty event arrays
- **Malformed events**: Handled gracefully (missing fields use defaults/nulls)

## Usage Examples

### Basic File Parsing

```csharp
using SkiaFlameGraph.Core.Parsing;

// Parse a Chrome trace file
FlameNode root = ChromeTraceParser.ParseFile("trace.json");

// Access thread data
foreach (var threadNode in root.Children)
{
    Console.WriteLine($"Thread {threadNode.Name}: {threadNode.Value}μs");
    // Process thread's flame graph...
}
```

### Manual Deserialization and Building

```csharp
using SkiaFlameGraph.Core.Parsing;
using System.IO;
using System.Text.Json;

// Read and deserialize manually
string json = File.ReadAllText("trace.json");
ChromeTraceEvent[] events = ChromeTraceParser.Deserialize(json);

// Build the tree
FlameNode root = ChromeTraceParser.BuildTree(events);

// The root contains thread children
FlameNode? mainThread = root.Children.FirstOrDefault(n => n.Name == "thread 1");
if (mainThread != null)
{
    // Analyze main thread performance
}
```

### Filtering Events Before Building

```csharp
// Get all events
ChromeTraceEvent[] events = ChromeTraceParser.Deserialize(json);

// Filter to specific thread or category
var filtered = events.Where(e => e.Tid == 1 && e.Category == "benchmark").ToArray();

// Build tree from filtered events
FlameNode root = ChromeTraceParser.BuildTree(filtered);
```

## Implementation Notes

### Thread Safety

- Static methods (`Deserialize`, `ParseFile`, `BuildTree`) are thread-safe
- `ChromeTraceEvent` instances are immutable after creation
- Returned `FlameNode` trees are mutable; external synchronization needed for concurrent modification

### Performance

- Time complexity: O(n log n) due to sorting (where n = number of events)
- Space complexity: O(n) for storing events and the resulting tree
- Memory efficient: processes one thread at a time after grouping

### Edge Cases Handled

- Missing thread IDs: Events ignored
- Missing function names: Default to "unknown"
- Unrecognized phases: Skipped (continue processing)
- Mismatched begin/end events: Stack operations guarded by count checks
- Empty trace files: Results in root node with no children
- Null durations: Only complete events with valid durations assign values

## See Also

- `FlameNode`: The data structure representing flame graph nodes
- Chrome Trace Event Format: https://docs.google.com/document/d/1CvAClvFfyA5R-PhYUmn5OOQtYMH4h6I0nSsKchNAySU/preview