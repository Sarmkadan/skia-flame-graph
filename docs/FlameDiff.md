# FlameDiff Class

## Responsibilities

The `FlameDiff` class provides functionality to compute the delta between two flame graphs (baseline and current profiles). It compares flame node trees by matching nodes based on frame identity (name, file, line) and calculates the difference in values (current - baseline). Nodes present in only one tree are included with their full value (positive for current-only, negative for baseline-only).

## Public Methods

### `Diff(FlameNode baseline, FlameNode current)`

Computes the delta between two flame graphs.

**Parameters:**
- `baseline`: The baseline flame graph node (typically the older/previous profile).
- `current`: The current flame graph node (typically the newer/current profile).

**Returns:**
A new `FlameNode` tree representing the delta values.

**Exceptions:**
- `ArgumentNullException`: Thrown when `baseline` or `current` is null.

**Remarks:**
The method recursively traverses the flame node trees, matching children by name. For each matched node, the delta value is computed as `current.Value - baseline.Value`. Nodes existing only in the current tree are included with their positive value, while nodes existing only in the baseline tree are included with their negative value (indicating a decrease).

## Example Usage

```csharp
using SkiaFlameGraph.Core;
using SkiaFlameGraph.Core.Models;

// Assume we have two flame graphs: baselineProfile and currentProfile
FlameNode baseline = LoadBaselineFlameGraph(); // Implement loading logic
FlameNode current = LoadCurrentFlameGraph();   // Implement loading logic

// Compute the difference
FlameNode delta = FlameDiff.Diff(baseline, current);

// The 'delta' tree now represents the changes between baseline and current.
// Positive values indicate increases, negative values indicate decreases.
// You can then render this delta tree using a renderer, e.g., to SVG or PNG.
```

## Notes

- The comparison is based on the frame identity (Name, File, Line) of each node.
- The structure of the delta tree mirrors the union of nodes from both trees.
- The `FlameDiff` class is static and thread-safe for concurrent use with different flame graph pairs.