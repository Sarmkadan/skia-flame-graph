# Collapsed Stacks Parser Documentation

## Overview

The `CollapsedStacksParser` class parses Brendan Gregg's collapsed stack format (also known as "folded" format) and converts it into a flame graph data structure represented by a tree of `FlameNode` objects. This format is commonly used with profiling tools like `perf`, `dtrace`, and `SystemTrace` to represent stack traces with sample counts.

## Collapsed Stacks Format

The parser expects input in the Brendan Gregg collapsed stack format where each line represents a single stack trace with its sample count:

```
frame1;frame2;frame3 ... frameN count
```

- Frames are separated by semicolons (`;`)
- The last field after the final space is the sample count (a positive number)
- Frames represent the call stack from bottom (first frame) to top (last frame)
- Empty lines are ignored
- Malformed lines (missing count, invalid count, no frames) are skipped

### Example Input

```
;__libc_start_main;main;foo;bar 1
;__libc_start_main;main;foo;baz 2
;__libc_start_main;main;qux 1
```

This example represents:
- 1 sample of the stack: `__libc_start_main` → `main` → `foo` → `bar`
- 2 samples of the stack: `__libc_start_main` → `main` → `foo` → `baz`
- 1 sample of the stack: `__libc_start_main` → `main` → `qux`

## Public API

### `ParseFile(string path, CancellationToken cancellationToken = default)`

Parses a collapsed stacks file from disk into a `FlameNode` tree.

```csharp
public static FlameNode ParseFile(string path, CancellationToken cancellationToken = default)
```

**Parameters:**
- `path`: Path to the collapsed stacks file
- `cancellationToken`: Optional token to monitor for cancellation requests

**Returns:**
- A `FlameNode` tree with "root" as the root node containing all stacks

**Exceptions:**
- `ArgumentNullException`: If `path` is null
- `FileNotFoundException`: If the file does not exist

### `Parse(IEnumerable<string> lines, CancellationToken cancellationToken = default)`

Parses collapsed stacks from an enumerable of lines (e.g., from a string array or stream reader).

```csharp
public static FlameNode Parse(IEnumerable<string> lines, CancellationToken cancellationToken = default)
```

**Parameters:**
- `lines`: Lines of text in Brendan Gregg collapsed stack format
- `cancellationToken`: Optional token to monitor for cancellation requests

**Returns:**
- A `FlameNode` tree with "root" as the root node containing all stacks

**Exceptions:**
- `ArgumentNullException`: If `lines` is null

## Parsing Algorithm

The parser converts collapsed stack lines into a hierarchical flame graph through these steps:

### 1. Line Processing
Each non-empty line is processed individually:
- Split on the last space to separate frames from the count
- Parse the count value (must be a positive double)
- Split the frames portion by semicolon (`;`) to get individual frames
- Skip lines that don't conform to the format

### 2. Tree Construction (Bottom-Up)
For each valid line, frames are processed in reverse order (from top of stack to bottom):
- The last frame (top of stack) becomes a leaf node with the count value
- Each preceding frame becomes a parent node of the current node
- If identical stacks appear multiple times, their counts are aggregated by merging trees

### 3. Frame Interning
To reduce memory usage, frame strings are interned using a dictionary so identical frame strings share the same reference.

## FlameNode Structure

The resulting tree consists of `FlameNode` objects with:

- **`Name`**: The frame/function name (interned string)
- **`Value`**: The sample count (aggregated for duplicate stacks)
- **`Children`**: Child nodes representing calling functions (parents in the stack)
- **`File`/`Line`**: Not set (always null/empty) as this format doesn't contain source location
- **`Depth`**: Depth in the tree (0 for root, increasing toward leaves)
- **`Parent`**: Reference to the parent node

The root node is named "root" and its `Value` represents the total sample count across all stacks.

## Error Handling

The parser is resilient to malformed input:
- **Null/empty path**: `ArgumentNullException` for `ParseFile`
- **Missing file**: `FileNotFoundException` with descriptive message
- **Null lines enumerable**: `ArgumentNullException` for `Parse`
- **Blank lines**: Skipped
- **Lines without count**: Skipped (no space found)
- **Invalid count** (non-numeric, ≤0): Skipped
- **Lines with no frames**: Skipped
- **Exceptions during line parsing**: Individual malformed lines are skipped, processing continues

## Usage Examples

### Basic File Parsing

```csharp
using SkiaFlameGraph.Core.Parsing;

// Parse a collapsed stacks file
FlameNode root = CollapsedStacksParser.ParseFile("stacks.folded");

// Access the total sample count
Console.WriteLine($"Total samples: {root.Value}");

// Process each top-level stack (children of root)
foreach (var stackNode in root.Children)
{
    Console.WriteLine($"Stack '{stackNode.Name}': {stackNode.Value} samples");
    // Process the stack trace hierarchy...
}
```

### Parsing from String Array

```csharp
using SkiaFlameGraph.Core.Parsing;
using System.Collections.Generic;

// Sample data in collapsed stack format
string[] lines = new[]
{
    ";__libc_start_main;main;foo;bar 1",
    ";__libc_start_main;main;foo;baz 2",
    ";__libc_start_main;main;qux 1"
};

// Parse the lines
FlameNode root = CollapsedStacksParser.Parse(lines);

// The root should have a value of 4 (1+2+1)
Console.WriteLine(root.Value); // Outputs: 4
```

### Integration with FlameGraph Generation

```csharp
using SkiaFlameGraph.Core.Parsing;
using SkiaFlameGraph.Core;

// Parse collapsed stacks data
FlameNode root = CollapsedStacksParser.ParseFile("profile.folded");

// Generate SVG flame graph (using the core rendering logic)
var flameGraph = new FlameGraph();
string svg = flameGraph.GenerateSvg(root);
File.WriteAllText("flamegraph.svg", svg);
```

## Implementation Notes

### Thread Safety
- Static methods (`ParseFile`, `Parse`) are thread-safe for concurrent calls with different inputs
- Shared resources (like the frame pool dictionary) are created per-parsing operation
- Returned `FlameNode` trees are mutable; external synchronization needed for concurrent modification

### Performance
- Time complexity: O(n × m) where n = number of lines, m = average frame depth
- Space complexity: O(u × m) where u = number of unique frames (due to interning)
- Memory efficient: frame strings are interned to avoid duplication
- Processes lines sequentially with low memory overhead per line

### Edge Cases Handled
- **Empty input**: Results in root node with no children and zero value
- **Single frame lines**: Creates a direct child of root with the frame name and count
- **Duplicate frames in stack**: Handled normally (each frame position is distinct)
- **Very deep stacks**: Limited only by available memory
- **Special characters in frames**: Semicolons and spaces in frame names must be escaped in the source data (parser treats them as delimiters)
- **Very large counts**: Uses double precision for count values

## See Also

- `FlameNode`: The data structure representing flame graph nodes
- Brendan Gregg's FlameGraphs: http://www.brendangregg.com/flamegraphs.html
- `ChromeTraceParser`: For parsing Chrome trace JSON format