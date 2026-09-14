# FlameNode Model

The `FlameNode` class represents a node in the aggregated call tree used by both flame graph and treemap renderers. It is a format-independent shape that captures profiling data for visualization.

## Properties

### Name
- **Type**: `string`
- **Description**: The name of the frame represented by this node (e.g., function or method name).
- **Remarks**: This is set via the constructor and is immutable.

### File
- **Type**: `string?` (nullable)
- **Description**: The source file where the frame was defined, when available from the profiler.
- **Remarks**: Can be `null` if the profiler did not record source file information.

### Line
- **Type**: `int?` (nullable)
- **Description**: The source line number for the frame, when available from the profiler.
- **Remarks**: Can be `null` if the profiler did not record line information.

### Value
- **Type**: `double`
- **Description**: The total weight (time) of this subtree, including the node itself and all its descendants.
- **Remarks**: This is the primary metric used for sizing nodes in the flame graph or treemap. Units match the profile's time unit (e.g., milliseconds, nanoseconds).

### Depth
- **Type**: `int`
- **Description**: The depth of this node from the synthetic root node (where the root has depth 0).
- **Remarks**: Indicates the call stack depth; root-level frames have depth 0, their direct children depth 1, and so on.

### Children
- **Type**: `List<FlameNode>`
- **Description**: A collection of child frames called by this frame.
- **Remarks**: Populated via**: The `AddChild` method, which merges children with identical identity (same name, file, and line) to avoid duplicate slivers in the flame graph.

### Parent
- **Type**: `FlameNode?` (nullable)
- **Description**: The parent node, or `null` for the synthetic root node.
- **Remarks**: Set automatically when a node is added as a child via `AddChild`.

### SelfValue
- **Type**: `double` (computed property)
- **Description**: The weight attributed to this frame alone, excluding the weight of its children.
- **Remarks**: Calculated as `Value - sum(children.Value)`. Represents the exclusive time spent in this frame.

## Methods

### AddChild(string name, string? file = null, int? line = null)
- **Description**: Adds a child frame or returns an existing child with the same identity (name, file, line).
- **Returns**: The matching existing child or the newly created child.
- **Remarks**: This method ensures that recursive calls with the same frame identity collapse into a single node, preventing a stack of slivers in the flame graph.

### MaxDepth()
- **Description**: Returns the maximum depth of the subtree rooted at this node, including this node.
- **Returns**: An `int` representing the greatest depth found in the subtree.

## Equality and Comparison
The `FlameNode` class implements `IEquatable<FlameNode>` and overrides `Equals` and `GetHashCode`. Two nodes are considered equal if they have the same:
- `Name`
- `File`
- `Line`
- `Value`
- `Depth`
- `Parent` (recursively)

Equality operators (`==` and `!=`) are also provided.

## ToString()
Overrides `ToString()` to return a debug string: `FlameNode(Name={Name}, Value={Value})`.

## Usage Example
See the [FlameNodeBuilder section in README](../README.md#flamenodebuilder) for usage examples.